using GFS.helper;
using GFS.Structures;

namespace GFS;

public class FileSystemManager
{
    private bool b;
    private FileSystemNode root;
    public bool IsInit()
    {
        return File.Exists("GFS.meta") && File.Exists("GFS.data");
    }

    public void CreateFilesystem(int maxSize, int sectorSize)
    {
        root = new FileSystemNode("/", "", true);
        InitTestData();
        var fs = SerializeFs();
        Console.WriteLine(fs);
        DeserializeFs(fs);
    }

    private void InitTestData()
    {
        var sub1 = new FileSystemNode("/", "sub1", true);
        sub1.Children.AddLast(new FileSystemNode("/sub1/", "file1", false));
        sub1.Children.AddLast(new FileSystemNode("/sub1/", "file2", false));
        root.Children.AddLast(sub1);
        root.Children.AddLast(new FileSystemNode("/", "file3", false));
        var sub2 = new FileSystemNode("/", "sub2", true);
        sub2.Children.AddLast(new FileSystemNode("/sub2/", "file4", false));
        var sub3 = new FileSystemNode("/sub2/", "sub3", true);
        sub3.Children.AddLast(new FileSystemNode("/sub2/sub3/","file5",false));
        sub2.Children.AddLast(sub3);
        root.Children.AddLast(sub2);
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
        root = new FileSystemNode("/", "", true);
        var lines = StringHelper.Split(serializedFS, '\n');
        foreach (var line in lines)
        {
            var split = StringHelper.Split(line, ' ');
            createNode(split[0], split[1], bool.Parse(split[2]));
        }
        Console.WriteLine("Deserialized FS:");
        Console.WriteLine(SerializeFs());
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
}