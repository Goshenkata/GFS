using GFS.DTO;
using GFS.helper;
using GFS.Structures;
using System.Text;

namespace GFS;

public class FileSystemManager
{
    public const string DataFilepath = "GFS.data";

    public string CurrentPath { get; set; } = "/";

    public FilesystemData _fsData;
    public SectorData _sectorData;

    private FileStream _fs;
    private BinaryWriter _bw;
    private BinaryReader _br;

    private int _sectorSizeInBytes;
    private long _maxFsSizeInBytes;
    private const long FsDataOffset = sizeof(long) + sizeof(int) + sizeof(int);


    public bool IsInit()
    {
        return File.Exists(DataFilepath);
    }

    public OperationResult CreateFilesystem(long maxSize, int sectorSize)
    {
        if (IsInit())
        {
            return new OperationResult() { Success = false, Message = Messages.FilesystemIsInit };
        }
        if ((sectorSize * 16) > maxSize)
        {
            return new OperationResult() { Success = false, Message = Messages.Atleast16Sectors };
        }


        _maxFsSizeInBytes = maxSize;
        _sectorSizeInBytes = sectorSize;
        long sectorOffset = maxSize / 10;

        //write and read maxFsSizeInBytes and sectorSize, init length;
        _fs = new FileStream(DataFilepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _bw = new BinaryWriter(_fs);
        _br = new BinaryReader(_fs);
        _fs.SetLength(_maxFsSizeInBytes);
        _bw.Seek(0, SeekOrigin.Begin);
        _bw.Write(_maxFsSizeInBytes);
        _bw.Write(_sectorSizeInBytes);
        _bw.Write(0);


        _sectorData = new SectorData(sectorOffset + 1, maxSize, _fs, _bw, _br, sectorSize);
        _sectorData.CreateSectorData();

        _fsData = new FilesystemData(FsDataOffset, sectorOffset, _fs, _bw, _br, ref _sectorData);
        _fsData.InitFs();

        return new OperationResult() { Success = true, Message = "" };
    }


    public void LoadFs()
    {
        _fs = new FileStream(DataFilepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _bw = new BinaryWriter(_fs);
        _br = new BinaryReader(_fs);

        _fs.Seek(0, SeekOrigin.Begin);
        _maxFsSizeInBytes = _br.ReadInt64();
        _sectorSizeInBytes = _br.ReadInt32();
        long sectorOffset = _maxFsSizeInBytes / 10;

        _sectorData = new SectorData(sectorOffset + 1, _maxFsSizeInBytes, _fs, _bw, _br, _sectorSizeInBytes);
        _sectorData.LoadSectorData();

        _fsData = new FilesystemData(FsDataOffset, sectorOffset, _fs, _bw, _br,ref _sectorData);
    }

    public OperationResult RenameNode(string fullPath, string newName)
    {
        if (!StringHelper.IsValidNodeName(newName))
        {
            return new OperationResult() { Success = false, Message = Messages.InvalidName };
        }
        if (_fsData.Exists(fullPath))
        {
            return new OperationResult() { Success = false, Message = Messages.AlreadyExists };
        }
        var node = _fsData.GetNodeByPath(fullPath);
        node.Name = newName;
        _fsData.Save(node);
        return new OperationResult() { Success = true, Message = "" };
    }
    public OperationResult Mkdir(string parentPath, string dirName)
    {
        if (!StringHelper.IsValidNodeName(dirName))
        {
            return new OperationResult() { Success = false, Message = Messages.InvalidDirName };
        }

        if (_fsData.Exists(parentPath + dirName))
        {
            Console.Error.WriteLine(Messages.AlreadyExists);
            return new OperationResult() { Success = false, Message = Messages.AlreadyExists };
        }

        var parent = _fsData.GetNodeByPath(parentPath);
        _fsData.CreateNode(dirName, true, parent.Indx);
        return new OperationResult() { Success = true, Message = "" };
    }

    public void PrintTree()
    {
        _fsData.PrintTree(0, _fsData.Root);
    }

    public bool DirExists(string dirName)
    {
        return _fsData.DirExists(dirName);
    }

    public bool FileExists(string dirName)
    {
        return _fsData.FileExists(dirName);
    }

    public OperationResult Rmdir(string parentPath, string dirName)
    {
        var fullPath = StringHelper.ConcatPath(parentPath, dirName);
        var nodeToRemove = _fsData.GetNodeByPath(fullPath);
        var children = _fsData.GetChildren(nodeToRemove);
        if (!children.isEmpty())
        {
            return new OperationResult() { Success = false, Message = Messages.DirectoryIsNotEmpty };
        }
        var parent = _fsData.GetNodeByPath(parentPath);
        _fsData.RemoveChild(parent, nodeToRemove);
        return new OperationResult() { Success = false, Message = "" };
    }

    public MyList<FileLs> Ls(string path)
    {
        var currentDir = _fsData.GetNodeByPath(path);
        var output = new MyList<FileLs>();
        if (currentDir != null)
        {
            var children = _fsData.GetChildren(currentDir);
            foreach (var childId in children)
            {
                var child = _fsData.LoadById(childId);
                output.AddLast(new FileLs { Name = child.Name, IsDirectory = child.IsDirectory, IsCorrupted = child.IsCorrupted });
            }
        }
        return output;
    }
    public FileSystemNode? GetNode(string fullpath)
    {
        return _fsData.GetNodeByPath(fullpath);
    }


    public bool AppendToFile(string filePath, byte[] data)
    {
        var node = _fsData.GetNodeByPath(filePath);
        var children = _fsData.GetChildren(node);
        int lastDataindex = node.LastDataIndexOfFile;
        int[] newSectors = _sectorData.AppendToFile(data, ref lastDataindex, ref children);
        node.LastDataIndexOfFile = lastDataindex;
        _fsData.SetChildren(node, children);
        _fsData.Save(node);

        if (newSectors.Length > 0)
        {
            children.AddLast(newSectors);
            _fsData.SetChildren(node ,children);
        }
        return true;
    }

    public bool CreateFile(string filePath, byte[] data)
    {
        var node = _fsData.GetNodeByPath(filePath);

        var writeFileDto = _sectorData.WriteFile(data);

        if (data.Length == 0)
        {
            data = new byte[1];
            data[0] = 0;
        }

        if (writeFileDto.IsCorrupted)
            return false;
        if (node == null)
        {
            string fileName;
            string parentPath = StringHelper.GetParentPath(filePath, out fileName);
            var parentNode = _fsData.GetNodeByPath(parentPath);
            _fsData.CreateNode(fileName, false, parentNode.ParentID, writeFileDto.LastDataIndex);
        }
        else
        {
            var newSectorsList = new MyList<int>();
            newSectorsList.AddLast(writeFileDto.Sectors);
            var children = _fsData.GetChildren(node);
            children.AddLast(newSectorsList.GetArray());
            _fsData.SetChildren(node, newSectorsList);
        }
        return true;
    }

    public string Cat(string path)
    {
        var node = _fsData.GetNodeByPath(path);
        if (node != null)
        {
            var children = _fsData.GetChildren(node);
            var output = Encoding.UTF8.GetString(_sectorData.ReadFile(children, node.LastDataIndexOfFile));
            _fsData._fs = _sectorData._fs;
            _fsData._br = _sectorData._br;
            _fsData._bw = _sectorData._bw;
            return output;
        }

        return string.Empty;
    }

    public bool ImportFile(string outsideFilePath, string myFilePath, bool isAppend)
    {
        byte[] data = File.ReadAllBytes(outsideFilePath);
        if (isAppend && _fsData.Exists(myFilePath))
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
        using var bw = new BinaryWriter(File.Open(destinationOnDisk, FileMode.Create, FileAccess.ReadWrite));
        var node = _fsData.GetNodeByPath(sourceFromFs);
        var children = _fsData.GetChildren(node);
        var data = _sectorData.ReadFile(children, node.LastDataIndexOfFile);
        bw.Write(data);

        return true;
    }

    public void RmFile(string path)
    {
        var file = _fsData.GetNodeByPath(path);
        var parent = _fsData.GetNodeByPath(StringHelper.GetParentPath(path));

        //free sectors
        foreach (var sectorId in _fsData.GetChildren(file))
        {
            //Free if the sector does not have duplicates, skip itself
            if (!_fsData.sectorHasDuplicates(sectorId, file))
            {
                _sectorData.Free(sectorId, false);
            }
        }

        _fsData.RemoveChild(parent, file);
    }

    public void PrintSectorInfo(int sector)
    {
        Console.WriteLine(_sectorData.GetSector(sector));
    }

    public byte[] GetBytes(string fullPath)
    {
        var node = _fsData.GetNodeByPath(fullPath);
        var children = _fsData.GetChildren(node);
        return _sectorData.ReadFile(children, node.LastDataIndexOfFile);

    }

    public bool NodeExists(string fullPath)
    {
        return _fsData.Exists(fullPath);
    }

    public string ResolvePath(FileSystemNode node)
    {
        string output = "";
        while (node.ParentID != -1)
        {
            output = node.Name + "/" + output;
            node = _fsData.LoadById(node.ParentID);
        }
        return output;  

    }


}
