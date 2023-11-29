using GFS.DTO;
using GFS.helper;
using GFS.Structures;
using System.Text;

namespace GFS;

public class FileSystemManager
{
    public const string DataFilepath = "GFS.data";

    public string CurrentPath { get; set; } = "/";

    private FilesystemData _fsData;
    private SectorData _sectorData;

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

        _fsData = new FilesystemData(FsDataOffset, sectorOffset, _fs, _bw, _br);
        //_fsData.InitTestData();
        _fsData.WriteMetadata();
        _fsData.LoadFs();

        _sectorData = new SectorData(sectorOffset + 1, maxSize, _fs, _bw, _br, sectorSize);
        return new OperationResult() { Success = true , Message = ""};
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
        _fsData = new FilesystemData(FsDataOffset, sectorOffset, _fs, _bw, _br);
        _fsData.LoadFs();
        _sectorData = new SectorData(sectorOffset + 1, _maxFsSizeInBytes, _fs, _bw, _br, _sectorSizeInBytes);

        //todo fix this bitch
        MyList<int> visitedSectors = new MyList<int>();
        foreach (var node1 in _fsData._root)
        {
            if (node1.IsDirectory) continue;

            foreach (var node1Sector in node1.SectorIds)
            {
                if (visitedSectors.Contains(node1Sector)) continue;
                visitedSectors.AddLast(node1Sector);
                foreach (var node2 in _fsData._root)
                {
                    if (node2.IsDirectory) continue;
                    if (node1.Equals(node2)) continue;
                    if (node2.SectorIds.Contains(node1Sector))
                    {
                        var files = new MyList<string>();
                        files.AddLast(node1.Path + node1.Name);
                        files.AddLast(node2.Path + node2.Name);
                    }
                }
            }
        }
    }


    public OperationResult Mkdir(string parentPath, string dirName)
    {
        if (!StringHelper.IsValidNodeName(dirName))
        {
            return new OperationResult() { Success = false, Message = Messages.InvalidDirName };
                    }

        if (DirExists(parentPath + dirName))
        {
            Console.Error.WriteLine(Messages.DirExists);
            return new OperationResult() { Success = false, Message = Messages.DirExists };
        }

        _fsData.Mkdir(parentPath, dirName);
        return new OperationResult() { Success = true, Message = "" };
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

    public MyList<FileLs> Ls(string path)
    {
        var currentDir = _fsData.GetNodeByPath(path);
        var output = new MyList<FileLs>();
        foreach (var child in currentDir.Children)
        {
            output.AddLast(new FileLs { Name = child.Name, IsDirectory = child.IsDirectory, IsCorrupted = child.IsCorrupted });
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
        int[] newSectors = _sectorData.AppendToFile(data, ref node);
        _fsData._fs = _sectorData._fs;
        _fsData._br = _sectorData._br;
        _fsData._bw = _sectorData._bw;
        if (newSectors.Length > 0)
        {
            node.SectorIds.AddLast(newSectors);
        }

        _fsData.WriteMetadata();
        return true;
    }

    public bool CreateFile(string filePath, byte[] data)
    {
        var node = _fsData.GetNodeByPath(filePath);
        var writeFileDto = _sectorData.WriteFile(data);
        if (writeFileDto.IsCorrupted)
            return false;
        if (node == null)
        {
            string fileName;
            string parentPath = StringHelper.GetParentPath(filePath, out fileName);
            _fsData.CreateNode(parentPath, fileName, false, writeFileDto.Sectors, writeFileDto.LastDataIndex);
        }
        else
        {
            var newSectorsList = new MyList<int>();
            newSectorsList.AddLast(writeFileDto.Sectors);
            node.SectorIds = newSectorsList;
            node.LastDataIndex = writeFileDto.LastDataIndex;
        }


        _fsData.WriteMetadata();
        return true;
    }

    public string Cat(string path)
    {
        var node = _fsData.GetNodeByPath(path);
        if (node != null)
        {
            var output = Encoding.UTF8.GetString(_sectorData.ReadFile(node));
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
        using var bw = new BinaryWriter(File.Open(destinationOnDisk, FileMode.Create, FileAccess.Write));
        var node = _fsData.GetNodeByPath(sourceFromFs);
        var data = _sectorData.ReadFile(node);
        bw.Write(data);

        return true;
    }

    public void RmFile(string path)
    {
        var file = _fsData.GetNodeByPath(path);
        var parent = _fsData.GetNodeByPath(StringHelper.GetParentPath(path));
        for (var index = 0; index < parent.Children.Count; index++)
        {
            if (parent.Children[index].Equals(file))
            {
                parent.Children.RemoveAt(index);
            }
        }

        //free sectors
        foreach (var sectorId in file.SectorIds)
        {
            //Free if the sector does not have duplicates, skip itself
            if (!_fsData.sectorHasDuplicates(sectorId, file))
            {
                _sectorData.Free(sectorId);
            }
        }

        _fsData.WriteMetadata();
    }

    public void PrintSectorInfo(int sector)
    {
        Console.WriteLine("last written sector: " + _sectorData.getLastWrittenSector());
        Console.WriteLine(_sectorData.GetSector(sector));
    }
}