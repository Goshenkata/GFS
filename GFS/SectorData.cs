using GFS.DTO;
using GFS.helper;
using GFS.Structures;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace GFS;

public class SectorData : StreamArray
{
    private int _sectorSize;
    private int _dataSize;
    private int _lastSectorId;
    private long _sectorsStart;
    private bool[] _isFreeBitmap;
    private int _lastWrittenSector;

    public SectorData(long dataStart, long dataEnd, FileStream fs, BinaryWriter bw, BinaryReader br,
        int sectorSize) : base( dataStart, dataEnd, fs, bw, br)
    {
        _sectorSize = sectorSize;
        _dataSize = sectorSize - sizeof(long);

        //the total number of sectors that can fit;
        long totalNumberOfSectors = (int)Math.Floor((double)(dataEnd - dataStart) / sectorSize) - 1;
        long bitmapSize = sizeof(bool) *  totalNumberOfSectors;

        _sectorsStart = dataStart + bitmapSize;
        _lastSectorId = (int)Math.Floor((double)(dataEnd - _sectorsStart) / sectorSize) - 1;

        _isFreeBitmap = new bool[_lastSectorId + 1];
    }

    public int getLastWrittenSector()
    {
        return _lastWrittenSector;
    }

    public void UpdateLastWrittenSector(int newVal)
    {
        _fs.Seek(sizeof(long) + sizeof(int), SeekOrigin.Begin);
        _bw.Write(newVal);
        _lastWrittenSector = newVal;
    }

    public long GetStartOfSectorIndex(int sectorId)
    {
        return _sectorsStart + ((long)_sectorSize) * sectorId;
    }

    private long GetStartOfDataIndex(int sectorId)
    {
        return _sectorsStart + sectorId * _sectorSize + (_sectorSize - _dataSize);
    }

    public SectorNode GetSector(int sectorId)
    {
        _fs.Seek(GetStartOfSectorIndex(sectorId), SeekOrigin.Begin);
        SectorNode sectorNode = new SectorNode();
        sectorNode.isFree = _isFreeBitmap[sectorId];
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
            if (_isFreeBitmap[i])
            {
                _fs.Seek(_dataStart + i * sizeof(bool), SeekOrigin.Begin);
                _bw.Write(false);
                _isFreeBitmap[i] = false;
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

        var sw = new Stopwatch();
        var sw2 = new Stopwatch();

        long[] times = new long[6];
        Array.Fill<long>(times, 0);

        sw2.Start();
        for (int i = 0; i < data.Length; i += _dataSize)
        {
            sw.Restart();

            //before writing check if sectorData matches anything
            var count = Math.Min(data.Length - i, _dataSize);
            lastSectorIndexData = count;
            var subData = ArrayHelper<byte>.subArray(data, i, i + count);

            sw.Stop();
            times[0] += sw.ElapsedTicks;


            sw.Restart();
            var hash = ComputeDataHash(subData);

            int sector = TakeNextAvalableSector();

            sectorIds[lastId++] = sector;
            sw.Stop();
            times[1] += sw.ElapsedTicks;

            sw.Restart();
            var matchingSector = GetSectorIdWithSameHash(hash);
            if (matchingSector != -1)
            {
                Free(sectorIds[lastId - 1]);
                sectorIds[lastId - 1] = matchingSector;
                continue;
            }
            sw.Stop();
            times[2] += sw.ElapsedTicks;


            sw.Restart();
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
            sw.Stop();
            times[3] += sw.ElapsedTicks;

            sw.Restart();
            //check if the data has been written correctly
            _fs.Seek(startOfDataIndex, SeekOrigin.Begin);
            var writtenDataHash = ComputeDataHash(_br.ReadBytes(count));
            if (writtenDataHash != hash)
            {
                Console.WriteLine(Messages.CorruptedSector);
                Free(sectorIds);
                return new WriteFileDto(Array.Empty<int>(), 0, true);
            }
            sw.Stop();
            times[4] += sw.ElapsedTicks;

            sw.Restart();
            //write the hash
            _fs.Seek(GetStartOfSectorIndex(sector), SeekOrigin.Begin);
            _bw.Write(hash);
            sw.Stop();
            times[5] += sw.ElapsedTicks;
        }
        sw2.Stop();
        long totalTime = sw2.ElapsedTicks;
        long totalTimeForTasks = 0;
        for (int i = 0; i < times.Length; i++)
        {
            totalTimeForTasks += times[i];
            Debug.WriteLine($"{i}: {times[i]}");
        }
        Debug.WriteLine($"Total time for task {totalTimeForTasks}");
        Debug.WriteLine($"Total time {totalTime}");
        Debug.WriteLine($"Diff in time {totalTime - totalTimeForTasks}");


        if (sectorIds != null && sectorIds.Length != 0 && sectorIds[^1] > getLastWrittenSector())
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
            if (!_isFreeBitmap[i])
            {
                if (_br.ReadInt64() == hash)
                    return i;
            }
        }

        return -1;
    }

    public byte[] ReadFile(FileSystemNode node)
    {
        var dataLength = (node.SectorIds.Count - 1) * _dataSize + node.LastDataIndex;
        if (dataLength <= 0)
        {
            return Array.Empty<byte>();
        }
        byte[] data = new byte[dataLength];
        int lastId = 0;
        for (var index = 0; index < node.SectorIds.Count - 1; index++)
        {
            var sectorId = node.SectorIds[index];
            var sector = GetSector(sectorId);

            //check if the sector is corrupted
            if (sector.hash != ComputeDataHash(sector.data))
            {
                Console.WriteLine(Messages.CorruptedSector);
                node.IsCorrupted = true;
                return Array.Empty<byte>();
            }

            if (sector.isFree)
                Console.Error.WriteLine("Reading from free sector");

            for (int i = 0; i < sector.data.Length; i++)
            {
                data[lastId] = sector.data[i];
                lastId++;
            }
        }

        var lastSector = GetSector(node.SectorIds[^1]);
        byte[] lastSectorData = new byte[node.LastDataIndex];
        for (int i = 0; i < node.LastDataIndex; i++)
        {
            if (lastSector.isFree)
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
        _fs.Seek(_dataStart + sectorId * sizeof(bool), SeekOrigin.Begin);
        _bw.Write(true);
        //update lastWrittenSector;
        var lastWrittenSector = getLastWrittenSector();
        if (sectorId == lastWrittenSector)
        {
            for (int i = lastWrittenSector - 1; i >= 0; i--)
            {
                if (!_isFreeBitmap[i])
                {
                    UpdateLastWrittenSector(i);
                }
            }
        }
    }


    public int[] AppendToFile(byte[] data, ref FileSystemNode node)
    {
        var lastSectorId = node.SectorIds[^1];
        var lastSector = GetSector(lastSectorId);
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

                    _fs.Seek(GetStartOfSectorIndex(newSector), SeekOrigin.Begin);
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
                    _fs.Seek(GetStartOfSectorIndex(lastSectorId), SeekOrigin.Begin);
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

    public void LoadSectorData()
    {
        _fs.Seek(_dataStart, SeekOrigin.Begin);
        for (int i = 0; i <= _lastWrittenSector; i++)
        {
            _isFreeBitmap[i] = _br.ReadBoolean();
        } 
        for (int i = _lastWrittenSector + 1; i <= _lastSectorId; i++)
        {
            _isFreeBitmap[i] = true;
        } 
    }

    public void CreateSectorData()
    {

        _fs.Seek(_dataStart, SeekOrigin.Begin);
        for (int i = 0; i <= _lastSectorId; i++)
        {
            _bw.Write(true);
            _isFreeBitmap[i] = true;
        }
        Console.WriteLine();
    }
}