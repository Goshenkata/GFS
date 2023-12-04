using GFS.DTO;
using GFS.helper;

namespace GFS;

public class SectorData : StreamArray
{
    private int _sectorSize;
    private int _dataSize;
    private int _lastSectorId;

    public SectorData(long dataStart, long dataEnd, FileStream fs, BinaryWriter bw, BinaryReader br,
        int sectorSize) : base(
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

    public void UpdateLastWrittenSector(int newVal)
    {
        _fs.Seek(sizeof(long) + sizeof(int), SeekOrigin.Begin);
        _bw.Write(newVal);
    }

    public long GetStartOfSectorIndex(int sectorId)
    {
        return _dataStart + ((long)_sectorSize) * sectorId;
    }

    private long GetStartOfDataIndex(int sectorId)
    {
        return _dataStart + sectorId * _sectorSize + (_sectorSize - _dataSize);
    }

    public SectorNode GetSector(int sectorId, bool flush = false)
    {
        if (flush)
        {
            Flush();
        }

        _fs.Seek(GetStartOfSectorIndex(sectorId), SeekOrigin.Begin);
        SectorNode sectorNode = new SectorNode();
        sectorNode.IsTaken = _br.ReadBoolean();
        sectorNode.hash = _br.ReadInt64();
        sectorNode.data = _br.ReadBytes(_dataSize);
        return sectorNode;
    }

    public SectorMetadataDTO GetSectorMetaData(int sectorId)
    {
        _fs.Seek(GetStartOfSectorIndex(sectorId), SeekOrigin.Begin);
        var sectorNode = new SectorMetadataDTO();
        sectorNode.IsTaken = _br.ReadBoolean();
        sectorNode.Hash = _br.ReadInt64();
        return sectorNode;
    }


    private int TakeNextAvalableSector()
    {
        for (int i = 0; i < _lastSectorId; i++)
        {
            var startIndx = GetStartOfSectorIndex(i);
            _fs.Seek(startIndx, SeekOrigin.Begin);
            bool isTaken = _br.ReadBoolean();
            if (!isTaken)
            {
                //move to the boolean
                _fs.Seek(startIndx, SeekOrigin.Begin);
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

            int sector = TakeNextAvalableSector();
            sectorIds[lastId++] = sector;

            var matchingSector = GetSectorIdWithSameHash(hash);
            if (matchingSector != -1)
            {
                Free(sectorIds[lastId - 1]);
                sectorIds[lastId - 1] = matchingSector;
                continue;
            }


            //if there are no more available sectors, free all the sectors and return an empty array
            if (sector == -1)
            {
                Free(sectorIds);
                return new WriteFileDto(Array.Empty<int>(), 0);
            }

            //go to the free data section of the array
            var startOfDataIndex = GetStartOfDataIndex(sector);
            _fs.Seek(startOfDataIndex, SeekOrigin.Begin);
            _bw.Write(subData);

            //check if the data has been written correctly
            _fs.Seek(startOfDataIndex, SeekOrigin.Begin);
            var writtenDataHash = ComputeDataHash(_br.ReadBytes(count));
            if (writtenDataHash != hash)
            {
                Console.WriteLine(Messages.CorruptedSector);
                Free(sectorIds);
                return new WriteFileDto(Array.Empty<int>(), 0, true);
            }

            //write the hash
            _fs.Seek(GetStartOfSectorIndex(sector) + sizeof(bool), SeekOrigin.Begin);
            _bw.Write(hash);

        }


        if (sectorIds != null && sectorIds[^1] > getLastWrittenSector())
        {
            UpdateLastWrittenSector(sectorIds[^1]);
        }

        return new WriteFileDto(sectorIds, lastSectorIndexData);
    }

    public int GetSectorIdWithSameHash(long hash)
    {
        var lastWrittenSector = getLastWrittenSector();
        for (int i = 0; i <= lastWrittenSector; i++)
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
            var sector = GetSector(sectorId, true);

            //check if the sector is corrupted
            if (sector.hash != ComputeDataHash(sector.data))
            {
                Console.WriteLine(Messages.CorruptedSector);
                node.IsCorrupted = true;
                return Array.Empty<byte>();
            }

            if (!sector.IsTaken)
                Console.Error.WriteLine("Reading from free sector");

            for (int i = 0; i < sector.data.Length; i++)
            {
                data[lastId] = sector.data[i];
                lastId++;
            }
        }

        var lastSector = GetSector(node.SectorIds[^1], true);
        byte[] lastSectorData = new byte[node.LastDataIndex];
        for (int i = 0; i < node.LastDataIndex; i++)
        {
            if (!lastSector.IsTaken)
                Console.Error.WriteLine("Reading from free sector");
            data[lastId++] = lastSector.data[i];
            lastSectorData[i] = lastSector.data[i];
        }

        if (lastSector.hash != ComputeDataHash(lastSectorData))
        {
            Console.Error.WriteLine($"Sector {node.SectorIds[^1]} is corrupted");
            return Array.Empty<byte>();
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
        //update lastWrittenSector;
        var lastWrittenSector = getLastWrittenSector();
        if (sectorId == lastWrittenSector)
        {
            for (int i = lastWrittenSector - 1; i >= 0; i--)
            {
                _fs.Seek(GetStartOfSectorIndex(i), SeekOrigin.Begin);
                var isTaken = _br.ReadBoolean();
                if (isTaken)
                {
                    UpdateLastWrittenSector(i);
                }
            }
        }
    }


    public int[] AppendToFile(byte[] data, ref FileSystemNode node)
    {
        var lastSectorId = node.SectorIds[^1];
        var lastSector = GetSector(lastSectorId, true);
        int remaining = _dataSize - node.LastDataIndex;

        var currentData = ArrayHelper<byte>.subArray(lastSector.data, 0, node.LastDataIndex);

        var fullNewData = ArrayHelper<byte>.mergeArrays(currentData, data);

        var writtenBytes = Math.Min(data.Length, remaining);
        node.LastDataIndex += writtenBytes;

        var preAppendHash = ComputeDataHash(currentData);
        if (preAppendHash != lastSector.hash)
        {
            Console.WriteLine(Messages.CorruptedSector);
            node.IsCorrupted = true;
            return Array.Empty<int>();
        }

        {
            var afterAppendHash = ComputeDataHash(fullNewData);
            var repeatSector = GetSectorIdWithSameHash(afterAppendHash);
            if (repeatSector != -1)
            {
                node.SectorIds[^1] = repeatSector;
                lastSectorId = repeatSector;
                Free(lastSectorId);
            }
            else
            {
                var sharedSector = GetSectorIdWithSameHash(preAppendHash);

                //if the sector is shared
                if (sharedSector != -1)
                {
                    var newSector = TakeNextAvalableSector();
                    node.SectorIds[^1] = newSector;

                    _fs.Seek(GetStartOfSectorIndex(newSector) + sizeof(bool), SeekOrigin.Begin);
                    _bw.Write(afterAppendHash);

                    _fs.Seek(GetStartOfDataIndex(newSector), SeekOrigin.Begin);
                    _bw.Write(fullNewData);


                    //update lastWritten sector
                    if (sharedSector > getLastWrittenSector())
                    {
                        UpdateLastWrittenSector(sharedSector);
                    }
                }
                else
                {
                    _fs.Seek(GetStartOfSectorIndex(lastSectorId) + sizeof(bool), SeekOrigin.Begin);
                    _bw.Write(afterAppendHash);

                    _fs.Seek(GetStartOfDataIndex(lastSectorId), SeekOrigin.Begin);
                    _bw.Write(fullNewData);
                }
            }
        }

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