using GFS.helper;

namespace GFS;

public class SectorData : StreamArray
{
    private int _sectorSize;
    private int _dataSize;
    private int _lastSectorId;

    public SectorData(long dataStart, long dataEnd, Stream fs, BinaryWriter bw, BinaryReader br, int sectorSize) : base(
        dataStart, dataEnd, fs, bw, br)
    {
        _sectorSize = sectorSize;
        _dataSize = sectorSize - sizeof(bool) - sizeof(long);
        _lastSectorId = (int)Math.Floor((double)(dataEnd - dataStart) / sectorSize) - 1;
    }

    public int getLastWrittenSector()
    {
        _fs.Seek(sizeof(long) + sizeof(int), SeekOrigin.Begin);
        return _br.ReadInt32();
    }

    private void UpdateLastWrittenSector(int newVal)
    {
        _fs.Seek(sizeof(long) + sizeof(int), SeekOrigin.Begin);
        _bw.Write(newVal);
    }

    private long GetStartOfSectorIndex(int sectorId)
    {
        return _dataStart + ((long)_sectorSize) * sectorId;
    }

    private long GetStartOfDataIndex(int sectorId)
    {
        return _dataStart + sectorId * _sectorSize + (_sectorSize - _dataSize);
    }

    public SectorNode GetSector(int sectorId)
    {
        _fs.Seek(GetStartOfSectorIndex(sectorId), SeekOrigin.Begin);
        SectorNode sectorNode = new SectorNode();
        sectorNode.IsTaken = _br.ReadBoolean();
        sectorNode.hash = _br.ReadInt64();
        sectorNode.data = _br.ReadBytes(_dataSize);
        return sectorNode;
    }

    private int TakeNextAvalableSector()
    {
        for (int i = 0; i < _lastSectorId; i++)
        {
            _fs.Seek(_dataStart + i * _sectorSize, SeekOrigin.Begin);
            bool isTaken = _br.ReadBoolean();
            if (!isTaken)
            {
                //move to the boolean
                _fs.Seek(-sizeof(bool), SeekOrigin.Current);
                _bw.Write(true);
                return i;
            }
        }

        return -1;
    }


    public WriteFileDto WriteFile(byte[] data)
    {
        int neededSectors = (int)Math.Ceiling((double)data.Length / _dataSize);
        int[] sectorIds = new int[neededSectors];
        var lastId = 0;
        int lastSectorIndexData = 0;
        for (int i = 0; i < data.Length; i += _dataSize)
        {
            //before writing check if sectorData matches anything
            var count = Math.Min(data.Length - i, _dataSize);
            lastSectorIndexData = count;
            var subData = ArrayHelper<byte>.subArray(data, i, i + count);
            var hash = ComputeDataHash(subData);
            var matchingSector = GetSectorIdWithSameHash(hash);
            if (matchingSector != -1)
            {
                Free(sectorIds[lastId - 1]);
                sectorIds[lastId++] = matchingSector;
                continue;
            }


            int sector = TakeNextAvalableSector();
            //if there are no more available sectors, free all the sectors and return an empty array
            sectorIds[lastId++] = sector;
            if (sector == -1)
            {
                Free(sectorIds);
                return new WriteFileDto(Array.Empty<int>(), 0);
            }

            //go to the free data section of the array
            _fs.Seek(GetStartOfDataIndex(sector), SeekOrigin.Begin);
            _bw.Write(subData);

            //write the hash
            _fs.Seek(GetStartOfSectorIndex(sector) + sizeof(bool), SeekOrigin.Begin);
            _bw.Write(hash);
        }


        if (sectorIds[^1] > getLastWrittenSector())
        {
            UpdateLastWrittenSector(sectorIds[^1]);
        }

        return new WriteFileDto(sectorIds, lastSectorIndexData);
    }

    private int GetSectorIdWithSameHash(long hash)
    {
        for (int i = 0; i <= getLastWrittenSector(); i++)
        {
            _fs.Seek(GetStartOfSectorIndex(i), SeekOrigin.Begin);
            var isTaken = _br.ReadBoolean();
            if (isTaken)
            {
                if (_br.ReadInt64() == hash)
                    return i;
            }
        }

        return -1;
    }

    public byte[] ReadFile(FileSystemNode node)
    {
        byte[] data = new byte[(node.SectorIds.Count - 1) * _dataSize + node.LastDataIndex];
        int lastId = 0;
        for (var index = 0; index < node.SectorIds.Count - 1; index++)
        {
            var sectorId = node.SectorIds[index];
            var sector = GetSector(sectorId);
            for (int i = 0; i < sector.data.Length; i++)
            {
                data[lastId] = sector.data[i];
                lastId++;
            }
        }

        for (int i = 0; i < node.LastDataIndex; i++)
        {
            var sector = GetSector(node.SectorIds[^1]);
            data[lastId++] = sector.data[i];
        }

        return data;
    }

    public void Free(int[] sectors)
    {
        foreach (var sector in sectors)
        {
            Free(sector);
        }
    }

    public void Free(int sectorId)
    {
        _fs.Seek(GetStartOfSectorIndex(sectorId), SeekOrigin.Begin);
        _bw.Write(false);
    }


    public int[] AppendToFile(byte[] data, ref FileSystemNode node)
    {
        var lastSector = node.SectorIds[^1];
        var endP = GetStartOfDataIndex(lastSector) + node.LastDataIndex;
        int remaining = _dataSize - node.LastDataIndex;
        //write till the end of the file
        _fs.Seek(endP, SeekOrigin.Begin);
        var writtenBytes = Math.Min(data.Length, remaining);
        node.LastDataIndex += writtenBytes;

        var subData = ArrayHelper<byte>.subArray(data, 0, writtenBytes);
        //todo hash must not be subdata
        var hash = ComputeDataHash(subData);

        var repeatSector = GetSectorIdWithSameHash(hash);
        if (repeatSector != -1)
        {
            node.SectorIds[^1] = repeatSector;
            lastSector = repeatSector;
        }
        else
        {
            _bw.Write(subData);
        }

        //update hash
        _fs.Seek(GetStartOfDataIndex(lastSector), SeekOrigin.Begin);
        _fs.Seek(GetStartOfSectorIndex(lastSector) + sizeof(bool), SeekOrigin.Begin);
        _bw.Write(hash);

        //if the data can fit
        if (data.Length > remaining)
        {
            //write the remaining sectors;
            var res = WriteFile(ArrayHelper<byte>.subArray(data, writtenBytes, data.Length));
            node.LastDataIndex = res.LastDataIndex;

            return res.Sectors;
        }

        return Array.Empty<int>();
    }

    private long ComputeDataHash(byte[] input)
    {
        long hash = 17L;
        foreach (byte b in input)
        {
            hash = hash * 31L + b;
        }

        return hash;
    }
}