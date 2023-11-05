using System.Text;

namespace GFS;

public class FileSystemManager
{
    public const string DataFilepath = "GFS.data";

    private string _currentPath = "/";

    public string CurrentPath
    {
        get => _currentPath;
        set => _currentPath = value;
    }

    private FilesystemData _fsData;
    private SectorData _sectorData;

    private Stream _fs;
    private BinaryWriter _bw;
    private BinaryReader _br;

    private int _sectorSizeInBytes;
    private long _maxFsSizeInBytes;

    public bool IsInit()
    {
        return File.Exists(DataFilepath);
    }

    public void CreateFilesystem(long maxSize, int sectorSize)
    {
        _maxFsSizeInBytes = maxSize;
        _sectorSizeInBytes = sectorSize;
        long sectorOffset = maxSize / 10;
        long fsDataOffset = sizeof(long) + sizeof(int);

        //write and read maxFsSizeInBytes and sectorSize, init length;
        _fs = File.Open(DataFilepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _bw = new BinaryWriter(_fs);
        _br = new BinaryReader(_fs);
        _fs.SetLength(_maxFsSizeInBytes);
        _bw.Seek(0, SeekOrigin.Begin);
        _bw.Write(_maxFsSizeInBytes);
        _bw.Write(_sectorSizeInBytes);

        _fsData = new FilesystemData(fsDataOffset, sectorOffset, _fs, _bw, _br);
        _fsData.InitTestData();
        _fsData.WriteMetadata();
        _fsData.LoadFs();

        _sectorData = new SectorData(sectorOffset + 1, maxSize, _fs, _bw, _br, sectorSize);
        _sectorData.InitTestData();
        int[] arr = {1};
        Console.WriteLine(Encoding.UTF8.GetString(_sectorData.readFile(arr)));
    }


    public void LoadFs()
    {
        _fs = File.Open(DataFilepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _bw = new BinaryWriter(_fs);
        _br = new BinaryReader(_fs);

        _fs.Seek(0, SeekOrigin.Begin);
        _maxFsSizeInBytes = _br.ReadInt64();
        _sectorSizeInBytes = _br.ReadInt32();
        long fsDataOffset = sizeof(long) + sizeof(int);
        long sectorOffset = _maxFsSizeInBytes / 10;
        _fsData = new FilesystemData(fsDataOffset, sectorOffset, _fs, _bw, _br);
        _fsData.LoadFs();
    }


    public void Mkdir(string parentPath, string dirName)
    {
        _fsData.Mkdir(parentPath, dirName);
    }

    public void PrintTree()
    {
        _fsData.PrintTree();
    }

    public bool DirExists(string dirName)
    {
        return _fsData.DirExists(dirName);
    }

    public void Rmdir(string parentPath, string dirName)
    {
        _fsData.Rmdir(parentPath, dirName);
    }

    public void Ls(string path)
    {
        var currentDir = _fsData.GetNodeByPath(path);
        foreach (var child in currentDir.Children)
        {
            Console.WriteLine(child.getLsFormat());
        }
    }
}