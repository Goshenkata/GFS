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


    public int[] WriteFile(byte[] data)
    {
        int neededSectors = (int)Math.Ceiling((double)data.Length / _dataSize);
        int[] sectorIds = new int[neededSectors];
        var lastId = 0;
        for (int i = 0; i < data.Length; i += _dataSize)
        {
            int sector = TakeNextAvalableSector();
            //if there are no more available sectors, free all the sectors and return an empty array
            sectorIds[lastId++] = sector;
            if (sector == -1)
            {
                Free(sectorIds);

                return Array.Empty<int>();
            }

            //go to the free data section of the array
            _fs.Seek(getStartOfDataIndex(sector), SeekOrigin.Begin);
            _bw.Write(data, i, Math.Min(data.Length - i, _dataSize));
        }

        return sectorIds;
    }

    public byte[] readFile(int[] sectorIds)
    {
        byte[] data = new byte[sectorIds.Length * _dataSize];
        int lastId = 0;
        foreach (var sectorId in sectorIds)
        {
            var sector = GetSector(sectorId);
            if (sectorIds.Length == 1)
            {
                return sector.data;
            }

            for (int i = 0; i < sector.data.Length; i++)
            {
                data[lastId] = sector.data[i];
                lastId++;
            }
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

    public long getIndexOfLastUsefulData(int sectorId)
    {
        _fs.Seek(getStartOfDataIndex(sectorId), SeekOrigin.Begin);
        while (_br.ReadChar() != '\0')
        {
        }

        return _fs.Position - 1;
    }

    public int[] AppendToFile(byte[] data, int lastIndx)
    {
        long s = getStartOfDataIndex(lastIndx);
        long e = getIndexOfLastUsefulData(lastIndx);
        int preAppendLength = (int)(s - e);
        int remaining = _dataSize - preAppendLength;
        //write till the end of the file
        _fs.Seek(e, SeekOrigin.Begin);
        var writtenBytes = Math.Min(data.Length, remaining);
        _bw.Write(data, 0, writtenBytes);
        //if the data can fit
        if (data.Length > remaining)
        {
            return WriteFile(ArrayHelper<byte>.subArray(data, writtenBytes, data.Length));
        }

        return Array.Empty<int>();
    }
}