using GFS.helper;
using GFS.Structures;

namespace GFS;

public class FilesystemData : StreamArray
{
    public FileSystemNode _root;

    public void InitTestData()
    {
        var sub1 = new FileSystemNode("/", "sub1", true);
        var file1 = new FileSystemNode("/sub1/", "file1", false);
        sub1.Children.AddLast(file1);
        sub1.Children.AddLast(new FileSystemNode("/sub1/", "file2", false));
        _root.Children.AddLast(sub1);
        _root.Children.AddLast(new FileSystemNode("/", "file3", false));
        var sub2 = new FileSystemNode("/", "sub2", true);
        sub2.Children.AddLast(new FileSystemNode("/sub2/", "file4", false));
        var sub3 = new FileSystemNode("/sub2/", "sub3", true);
        sub3.Children.AddLast(new FileSystemNode("/sub2/sub3/", "file5", false));
        sub2.Children.AddLast(sub3);
        _root.Children.AddLast(sub2);
    }

    public void WriteMetadata()
    {
        var data = SerializeFs();
        _fs.Seek(_dataStart, SeekOrigin.Begin);
        _bw.Write(data);
        _bw.Flush();
    }

    private string SerializeFs()
    {
        var output = new MyStringBuilder();
        foreach (var fileSystemNode in _root.Children)
        {
            output.Append(fileSystemNode.Serialize());
        }

        return output.ToString();
    }


    private void DeserializeFs(string serializedFs)
    {
        _root = new FileSystemNode("/", "root", true);
        var lines = StringHelper.Split(serializedFs, '\n');
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var split = StringHelper.Split(line, ' ');
            int[] sectors = new int[split.Length - 4];
            for (int i = 4; i < split.Length; i++)
            {
                sectors[i - 4] = int.Parse(split[i]);
            }

            CreateNode(split[0], split[1], bool.Parse(split[2]), sectors, int.Parse(split[3]));
        }
    }

    public FileSystemNode? GetNodeByPath(string name)
    {
        string[] path = StringHelper.Split(name, '/');
        var current = _root;
        foreach (var s in path)
        {
            current = current.getChildByName(s);
            if (current == null) return null;
        }

        return current;
    }

    public void CreateNode(string filePath, string name, bool isDirectory, int[] sectors, int lastDataIndex = 0)
    {
        var dir = GetNodeByPath(filePath);
        if (dir == null || !dir.IsDirectory)
        {
            return;
        }

        var node = new FileSystemNode(filePath, name, isDirectory, lastDataIndex);
        node.SectorIds.AddLast(sectors);
        dir.Children.AddLast(node);
    }

    public bool Mkdir(string path, string name)
    {
        var node = GetNodeByPath(path);
        if (node == null)
            return false;
        node.Children.AddLast(new FileSystemNode(path, name, true));
        WriteMetadata();
        return true;
    }

    public bool DirExists(string path)
    {
        var node = GetNodeByPath(path);
        return node != null && node.IsDirectory;
    }

    public bool sectorHasDuplicates(int sectorId, FileSystemNode toSkip)
    {
        foreach (var node in _root)
        {
            if (node.IsDirectory) continue;
            if (node.Equals(toSkip)) continue;
            if (node.SectorIds.Contains(sectorId))
                return true;
        }
        return false;
    }

    public void LoadFs()
    {
        _fs.Seek(_dataStart, SeekOrigin.Begin);
        var deserialized = _br.ReadString();
        DeserializeFs(deserialized);
    }

    public void PrintTree()
    {
        _root.PrintTree();
    }

    public bool Rmdir(string parentPath, string dirName)
    {
        var parent = GetNodeByPath(parentPath);
        if (parent == null)
            return false;
        for (var i = 0; i < parent.Children.Count; i++)
        {
            var current = parent.Children[i];
            if (current.Name == dirName)
            {
                if (!current.Children.isEmpty())
                {
                    Console.Error.WriteLine(Messages.DirectoryIsNotEmpty);
                    return false;
                }

                parent.Children.RemoveAt(i);
            }
        }

        WriteMetadata();
        return true;
    }

    public FilesystemData(long dataStart, long dataEnd, Stream fs, BinaryWriter bw, BinaryReader br) : base(dataStart,
        dataEnd, fs, bw, br)
    {
        _root = new FileSystemNode("/", "root", true);
    }

    public bool FileExists(string path)
    {
        var node = GetNodeByPath(path);
        return node != null && !node.IsDirectory;
    }
}