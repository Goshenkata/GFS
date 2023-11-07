using System.Text;
using GFS.helper;
using GFS.Structures;

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

        _sectorData = new SectorData(sectorOffset + 1, _maxFsSizeInBytes, _fs, _bw, _br, _sectorSizeInBytes);
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

    public bool FileExists(string dirName)
    {
        return _fsData.FileExists(dirName);
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

    public bool AppendToFile(string filePath, byte[] data)
    {
        var node = _fsData.GetNodeByPath(filePath);
        int[] newSectors = _sectorData.AppendToFile(data, node.SectorIds.GetLast());
        if (newSectors.Length > 0)
        {
            node.SectorIds.AddLast(newSectors);
            _fsData.WriteMetadata();
        }

        return true;
    }

    public bool CreateFile(string filePath, byte[] data)
    {
        var node = _fsData.GetNodeByPath(filePath);
        var sectors = _sectorData.WriteFile(data);
        if (node == null)
        {
            string fileName;
            string parentPath = StringHelper.GetParentPath(filePath, out fileName);
            _fsData.CreateNode(parentPath, fileName, false, sectors);
        }
        else
        {
            var newSectorsList = new MyList<int>();
            newSectorsList.AddLast(sectors);
            node.SectorIds = newSectorsList;
        }

        _fsData.WriteMetadata();
        return true;
    }

    public string Cat(string path)
    {
        var node = _fsData.GetNodeByPath(path);
        if (node != null)
        {
            return Encoding.UTF8.GetString(_sectorData.readFile(node.SectorIds.GetArray()));
        }

        return string.Empty;
    }

    public bool ImportFile(string outsideFilePath, string myFilePath, bool isAppend)
    {
        byte[] data = File.ReadAllBytes(outsideFilePath);
        if (isAppend && FileExists(myFilePath))
        {
            //append to file
            AppendToFile(myFilePath, data);
        }
        else
        {
            CreateFile(myFilePath, data);
        }

        return true;
    }

    public bool Export(string sourceFromFs, string destinationOnDisk)
    {
        using var bw = new BinaryWriter(File.Open(destinationOnDisk, FileMode.OpenOrCreate, FileAccess.Write));
        var node = _fsData.GetNodeByPath(sourceFromFs);
        var data = _sectorData.readFile(node.SectorIds.ToArray());
        bw.Write(data);

        return true;
    }

    public void RmFile(string path)
    {
        var file = _fsData.GetNodeByPath(path);
        _sectorData.Free(file.SectorIds.ToArray());
        var parent = _fsData.GetNodeByPath(StringHelper.GetParentPath(path));
        for (var index = 0; index < parent.Children.Count; index++)
        {
            if (parent.Children[index].Equals(file))
            {
                parent.Children.RemoveAt(index);
            }
        }
        _fsData.WriteMetadata();
    }
}