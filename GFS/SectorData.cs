using GFS.helper;

namespace GFS;

public class SectorData : StreamArray
{
    private int _sectorSize;
    private int _dataSize;
    private int _lastSectorId;
    private int _lastTakenSector;

    public SectorData(long dataStart, long dataEnd, Stream fs, BinaryWriter bw, BinaryReader br, int sectorSize) : base(
        dataStart, dataEnd, fs, bw, br)
    {
        _sectorSize = sectorSize;
        _dataSize = sectorSize - sizeof(bool);
        _lastSectorId = (int)Math.Floor((double)(dataEnd - dataStart) / sectorSize) - 1;
    }

    private long getStartOfSectorIndex(int sectorId)
    {
        return _dataStart + _sectorSize * sectorId;
    }

    private long getStartOfDataIndex(int sectorId)
    {
        return _dataStart + sectorId * _sectorSize + (_sectorSize - _dataSize);
    }

    private SectorNode GetSector(int sectorId)
    {
        _fs.Seek(getStartOfSectorIndex(sectorId), SeekOrigin.Begin);
        SectorNode sectorNode = new SectorNode();
        sectorNode.IsFree = _br.ReadBoolean();
        sectorNode.data = _br.ReadBytes(_dataSize);
        return sectorNode;
    }

    private int TakeNextAvalableSector()
    {
        for (int i = _lastTakenSector + 1; i <= _lastSectorId; i++)
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

        for (int i = 0; i < _lastTakenSector; i++)
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
            int sector = TakeNextAvalableSector();
            //if there are no more available sectors, free all the sectors and return an empty array
            sectorIds[lastId++] = sector;
            if (sector == -1)
            {
                Free(sectorIds);

                return new WriteFileDto(Array.Empty<int>(), 0);
            }

            //go to the free data section of the array
            var count = Math.Min(data.Length - i, _dataSize);
            lastSectorIndexData = count;
            _fs.Seek(getStartOfDataIndex(sector), SeekOrigin.Begin);
            _bw.Write(data, i, count);
        }

        return new WriteFileDto(sectorIds, lastSectorIndexData);
    }

    public byte[] readFile(FileSystemNode node)
    {
        byte[] data = new byte[(node.SectorIds.Count -1) * _dataSize + node.LastDataIndex];
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
        _fs.Seek(_dataStart + sectorId * _dataSize, SeekOrigin.Begin);
        _bw.Write(false);
    }

    public void InitTestData()
    {
        WriteFile("This is a test string\0"u8.ToArray());
    }


    public int[] AppendToFile(byte[] data, ref FileSystemNode node)
    {
        var endP = getStartOfDataIndex(node.SectorIds[^1]) + node.LastDataIndex;
        int remaining = _dataSize - node.LastDataIndex;
        //write till the end of the file
        _fs.Seek(endP, SeekOrigin.Begin);
        var writtenBytes = Math.Min(data.Length, remaining);
        _bw.Write(data, 0, writtenBytes);
        node.LastDataIndex += writtenBytes;
        //if the data can fit
        if (data.Length > remaining)
        {
            var res = WriteFile(ArrayHelper<byte>.subArray(data, writtenBytes, data.Length));
            node.LastDataIndex = res.LastDataIndex;
            return res.Sectors;
        }

        return Array.Empty<int>();
    }
}