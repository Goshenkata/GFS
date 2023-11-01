using System.Reflection.Metadata.Ecma335;
using GFS.helper;
using GFS.Structures;

namespace GFS;

public class FileSystemManager
{
    public static string DATA_FILEPATH = "GFS.data";
    public string CurrentPath { get; } = "/";

    private FilesystemData fsData;

    private Stream _fs;
    private BinaryWriter _bw;
    private BinaryReader _br;

    private int _sectorSizeInBytes;
    private long _maxFsSizeInBytes;

    public bool IsInit()
    {
        return File.Exists(DATA_FILEPATH);
    }

    public void CreateFilesystem(long maxSize, int sectorSize)
    {
        _maxFsSizeInBytes = maxSize;
        _sectorSizeInBytes = sectorSize;
        long sectorOffset = maxSize / 10;
        long fsDataOffset = sizeof(long) + sizeof(int);

        //write and read maxFsSizeInBytes and sectorSize, init length;
        _fs = File.Open(DATA_FILEPATH, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _bw = new BinaryWriter(_fs);
        _br = new BinaryReader(_fs);
        _fs.SetLength(_maxFsSizeInBytes);
        _bw.Seek(0, SeekOrigin.Begin);
        _bw.Write(_maxFsSizeInBytes);
        _bw.Write(_sectorSizeInBytes);

        fsData = new FilesystemData(fsDataOffset, sectorOffset, _fs, _bw, _br);
        fsData.InitTestData();
        fsData.WriteMetadata();
        fsData.LoadFs();
    }


    public void LoadFs()
    {
        try
        {
            _fs = File.Open(DATA_FILEPATH, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _bw = new BinaryWriter(_fs);
            _br = new BinaryReader(_fs);

            _fs.Seek(0, SeekOrigin.Begin);
            _maxFsSizeInBytes = _br.ReadInt64();
            _sectorSizeInBytes = _br.ReadInt32();
            long fsDataOffset = sizeof(long) + sizeof(int);
            long sectorOffset = _maxFsSizeInBytes / 10;
            fsData = new FilesystemData(fsDataOffset, sectorOffset, _fs, _bw, _br);
            fsData.LoadFs();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


    public void Mkdir(string parentPath, string dirName)
    {
        fsData.Mkdir(parentPath, dirName);
    }

    public void PrintTree()
    {
        fsData.PrintTree();
    }

    public bool DirExists(string dirName)
    {
        return fsData.DirExists(dirName);
    }

    public void Rmdir(string parentPath, string dirName)
    {
        fsData.Rmdir(parentPath, dirName);
    }
}