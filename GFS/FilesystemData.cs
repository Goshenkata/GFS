using System.Text;
using GFS.helper;
using GFS.Structures;

namespace GFS;

public class FilesystemData
{
    private FileSystemNode root;
    private BinaryWriter _bw;
    private Stream _fs;
    private BinaryReader _br;
    private long dataStart, dataEnd;

    public FilesystemData(long dataStart, long dataEnd,
        Stream fs, BinaryWriter bw, BinaryReader br)
    {
        _fs = fs;
        _fs.Seek(dataStart, SeekOrigin.Begin);
        _bw = bw;
        _br = br;
        root = new FileSystemNode("/", "root", true);
        this.dataStart = dataStart;
        this.dataEnd = dataEnd;
    }

    public void InitTestData()
    {
        var sub1 = new FileSystemNode("/", "sub1", true);
        sub1.Children.AddLast(new FileSystemNode("/sub1/", "file1", false));
        sub1.Children.AddLast(new FileSystemNode("/sub1/", "file2", false));
        root.Children.AddLast(sub1);
        root.Children.AddLast(new FileSystemNode("/", "file3", false));
        var sub2 = new FileSystemNode("/", "sub2", true);
        sub2.Children.AddLast(new FileSystemNode("/sub2/", "file4", false));
        var sub3 = new FileSystemNode("/sub2/", "sub3", true);
        sub3.Children.AddLast(new FileSystemNode("/sub2/sub3/", "file5", false));
        sub2.Children.AddLast(sub3);
        root.Children.AddLast(sub2);
    }

    public void WriteMetadata()
    {
        var data = SerializeFs();
        _fs.Seek(dataStart, SeekOrigin.Begin);
        //todo replace with my own methods
        _bw.Write(data);
        _bw.Flush();
    }

    private string SerializeFs()
    {
        var output = new MyStringBuilder();
        foreach (var fileSystemNode in root.Children)
        {
            output.Append(fileSystemNode.Serialize());
        }

        return output.ToString();
    }


    private bool DeserializeFs(string serializedFS)
    {
        root = new FileSystemNode("/", "root", true);
        var lines = StringHelper.Split(serializedFS, '\n');
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var split = StringHelper.Split(line, ' ');
            createNode(split[0], split[1], bool.Parse(split[2]));
        }

        return true;
    }

    public FileSystemNode? getNodeByPath(string name)
    {
        string[] path = StringHelper.Split(name, '/');
        var current = root;
        foreach (var s in path)
        {
            current = current.getChildByName(s);
            if (current == null) return null;
        }

        return current;
    }

    public bool createNode(string filePath, string name, bool isDirectory)
    {
        var dir = getNodeByPath(filePath);
        if (dir == null || !dir.IsDirectory)
        {
            return false;
        }

        dir.Children.AddLast(new FileSystemNode(filePath, name, isDirectory));
        return true;
    }

    public bool Mkdir(string path, string name)
    {
        var node = getNodeByPath(path);
        if (node == null)
            return false;
        node.Children.AddLast(new FileSystemNode(path, name, true));
        WriteMetadata();
        return true;
    }
    
    public bool DirExists(string path)
    {
        var node = getNodeByPath(path);
        return node != null && node.IsDirectory;
    }

    public void LoadFs()
    {
        _fs.Seek(dataStart, SeekOrigin.Begin);
        var deserialized = _br.ReadString();
        DeserializeFs(deserialized);
    }

    public void PrintTree()
    {
        root.PrintTree();
    }

    public bool Rmdir(string parentPath, string dirName)
    {
        var parent = getNodeByPath(parentPath);
        if (parent == null)
            return false;
        for (var i = 0; i < parent.Children.Count; i++)
        {
            var current = parent.Children[i];
            if (current.Name == dirName) {
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
}