using GFS.helper;
using GFS.Structures;
using System.Collections;

namespace GFS;

public class FilesystemData : StreamArray, IEnumerable<FileSystemNode>
{
    public FileSystemNode Root;
    private bool[] _bitmap;
    private long _maxNumOfNodes;
    private long _nodesStart;
    private SectorData _sectorData;

    public FilesystemData(long dataStart, long dataEnd, FileStream fs, BinaryWriter bw, BinaryReader br, ref SectorData sectorData) : base(dataStart,
        dataEnd, fs, bw, br)
    {
        long availableBytes = dataEnd - dataStart - 8;
        _maxNumOfNodes = availableBytes / FileSystemNode.ELEMENT_SIZE;
        _bitmap = new bool[_maxNumOfNodes];
        int bitmapSize = sizeof(bool) * _bitmap.Length;
        _maxNumOfNodes = (availableBytes - bitmapSize) / FileSystemNode.ELEMENT_SIZE - 1;
        LoadBitmap();
        _nodesStart = dataStart + bitmapSize + 8;

        _sectorData = sectorData;
        Root = LoadById(0);
    }

    private void LoadBitmap()
    {
        _fs.Seek(_dataStart, SeekOrigin.Begin);
        for (int i = 0; i <= _maxNumOfNodes; i++)
        {
            _bitmap[i] = _br.ReadBoolean();
        }
    }
    public void InitFs()
    {
        _fs.Seek(_dataStart, SeekOrigin.Begin);
        for (int i = 0; i <= _maxNumOfNodes; i++)
        {
            _bw.Write(true);
            _bitmap[i] = true;
        }
        CreateNode("root", true, -1);
        Root = LoadById(0);
    }

    public FileSystemNode? GetNodeByPath(string name)
    {
        if (name == "/")
        {
            return Root;
        }
        string[] path = StringHelper.Split(name, '/');
        var current = Root;
        foreach (var s in path)
        {
            var childIds = GetChildren(current);
            foreach (var childId in childIds)
            {
                var node = LoadById(childId);
                if (node.Name == s)
                {
                    current = node;
                }
            }
            if (current == null) return null;
        }

        return current;
    }

    private int GetFreeIndx()
    {
        for (int i = 0; i <= _maxNumOfNodes; i++)
        {
            if (_bitmap[i]) return i;
        }
        return -1;
    }

    public void CreateNode(string name, bool isDirectory, int parentId, int lastDataIndex = 0, bool isCorrupted = false)
    {
        int i = GetFreeIndx();
        FileSystemNode node = new FileSystemNode(isDirectory, isCorrupted, parentId, i, 0, lastDataIndex, name);
        Save(node);
    }


    public FileSystemNode LoadById(int id)
    {
        _fs.Seek(_nodesStart + id * FileSystemNode.ELEMENT_SIZE, SeekOrigin.Begin);
        bool isDir = _br.ReadBoolean();
        bool isCorrupt = _br.ReadBoolean();

        int parentId = _br.ReadInt32();
        int index = _br.ReadInt32();
        int lastDataIndexOfChildrenSector = _br.ReadInt32();
        int lastDataIndexOfFile = _br.ReadInt32();

        string name = new string(_br.ReadChars(FileSystemNode.NAME_LENGTH));
        MyList<int> sectors = new MyList<int>();
        for (int i = 0; i < FileSystemNode.CHILDREN_DATA_SECTORS_LENGTH; i++)
        {
            var el = _br.ReadInt32();
            if (el == -1)
                break;
            sectors.AddLast(el);
        }

        FileSystemNode fileSystemNode = new FileSystemNode(isDir, isCorrupt, parentId, index, lastDataIndexOfChildrenSector, lastDataIndexOfFile, name);
        fileSystemNode.ChildrenSectorIds.AddLast(sectors.GetArray());
        return fileSystemNode;
    }

    public MyList<int> GetChildren(FileSystemNode node)
    {
        byte[] data = _sectorData.ReadFile(node.ChildrenSectorIds, node.LastDataIndexOfChildrenSector);
        int[] intArray = new int[data.Length / 4];

        for (int i = 0; i < intArray.Length; i++)
        {
            intArray[i] = BitConverter.ToInt32(data, i * 4);
        }
        var list = new MyList<int>();
        list.AddLast(intArray);
        return list;
    }
    public void Save(FileSystemNode node)
    {
        _fs.Seek(_nodesStart + FileSystemNode.ELEMENT_SIZE * node.Indx, SeekOrigin.Begin);

        //bools
        _bw.Write(node.IsDirectory);
        _bw.Write(node.IsCorrupted);

        //ints
        _bw.Write(node.ParentID);
        _bw.Write(node.Indx);
        _bw.Write(node.LastDataIndexOfChildrenSector);
        _bw.Write(node.LastDataIndexOfFile);

        //arrs
        _bw.Write(node._nameArr);
        foreach (var sectorId in node.ChildrenSectorIds)
        {
            _bw.Write(sectorId);
        }
    }

    public bool DirExists(string path)
    {
        var node = GetNodeByPath(path);
        return node != null && node.IsDirectory;
    }

    public void SetChildren(FileSystemNode node, MyList<int> children)
    {
        _sectorData.Free(node.ChildrenSectorIds.GetArray());

        var intArr = children.GetArray();
        byte[] byteArray = new byte[intArr.Length * sizeof(int)];
        Buffer.BlockCopy(intArr, 0, byteArray, 0, byteArray.Length);

        //write the children inside other sectors
        var output = _sectorData.WriteFile(byteArray);

        node.LastDataIndexOfChildrenSector = output.LastDataIndex;
        foreach (var sector in output.Sectors)
        {
            node.ChildrenSectorIds.AddLast(sector);
        }
        Save(node);
    }
    public void RemoveChild(FileSystemNode parent, FileSystemNode child)
    {
        var parentChildren = GetChildren(parent);
        for (int i = 0; i < parentChildren.Count; i++)
        {
            if (parentChildren[i] == child.Indx)
            {
                parentChildren.RemoveAt(i);
                RemoveNode(child);
            }
        }
        SetChildren(parent, parentChildren);

    }
    public bool sectorHasDuplicates(int sectorId, FileSystemNode toSkip)
    {
        foreach (var node in this)
        {
            if (node.IsDirectory) continue;
            if (node.Equals(toSkip)) continue;
            var sectorIds = GetChildren(node);
            if (sectorIds.Contains(sectorId))
                return true;
        }
        return false;
    }

    public IEnumerator<FileSystemNode> GetEnumerator()
    {
        MyQueue<FileSystemNode> queue = new MyQueue<FileSystemNode>();
        queue.Enqueue(Root);
        while (!queue.IsEmpty())
        {
            var current = queue.Dequeue();
            yield return current;
            foreach (var child in GetChildren(current))
            {
                queue.Enqueue(LoadById(child));
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void RemoveNode(FileSystemNode node)
    {
        _sectorData.Free(node.ChildrenSectorIds.GetArray());
        _bitmap[node.Indx] = true;
    }


    public bool FileExists(string path)
    {
        var node = GetNodeByPath(path);
        return node != null && !node.IsDirectory;
    }

    public bool Exists(string path)
    {
        return GetNodeByPath(path) != null;
    }

    public void PrintTree(int level, FileSystemNode current)
    {
        string indentation = new string('-', level * 2);

        var sectors = current.IsDirectory ? "" : current.ChildrenSectorIds.ToString();
        Console.WriteLine(indentation + current.Name + " " + current.IsDirectory + " " + current.LastDataIndexOfFile + " " + " " + current.IsCorrupted + sectors);
        var kids = GetChildren(current);
        foreach (var child in kids)
        {
            PrintTree(level + 1, LoadById(child));
        }
    }
}
