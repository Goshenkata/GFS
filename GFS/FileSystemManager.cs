using GFS.helper;
using GFS.Structures;

namespace GFS;

public class FileSystemManager
{
    private FileSystemNode root;
    public static string METADATA_FILEPATH = "GFS.meta";
    public static string DATA_FILEPATH = "GFS.data";
    private int _sectorSize;
    private int _maxSize;
    public string CurrentPath { get; } = "/";

    public bool IsInit()
    {
        return File.Exists("GFS.meta") && File.Exists("GFS.data");
    }

    public void CreateFilesystem(int maxSize, int sectorSize)
    {
        root = new FileSystemNode("/", "", true);
        this._maxSize = maxSize;
        this._sectorSize = sectorSize;
        InitTestData();
        WriteMetadata();
        CreateFilesystemDataFile();
    }

    private void CreateFilesystemDataFile()
    {
        try
        {
            using (var stream = File.Open(DATA_FILEPATH, FileMode.Append))
            {
                stream.SetLength(_maxSize * 1024);
            }

            Console.WriteLine(Messages.FilesystemCreated);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error creating data file: " + e.Message);
        }
    }

    public void WriteMetadata()
    {
        var data = SerializeFs();
        try
        {
            using (var writer = new StreamWriter(METADATA_FILEPATH))
            {
                writer.Write(data);
            }
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine("Error writing metadata: " + ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.Error.WriteLine("Permission error: " + ex.Message);
        }
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
        sub3.Children.AddLast(new FileSystemNode("/sub2/sub3/", "file5", false));
        sub2.Children.AddLast(sub3);
        root.Children.AddLast(sub2);
    }

    private string SerializeFs()
    {
        var output = new MyStringBuilder($"{_sectorSize} {_maxSize}\n");
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
        var fsSizeMetadata = StringHelper.Split(lines[0], ' ');
        this._sectorSize = int.Parse(fsSizeMetadata[0]);
        this._maxSize = int.Parse(fsSizeMetadata[1]);
        for (var index = 1; index < lines.Length; index++)
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

    public void LoadFs()
    {
        try
        {
            string serializedData = File.ReadAllText(METADATA_FILEPATH);
            DeserializeFs(serializedData);
            Console.WriteLine(Messages.FilesystemLoadedSuccessfully);
        }
        catch (Exception e)
        {
            Console.WriteLine();
        }
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
}