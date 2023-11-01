using System.Collections;
using System.Text;
using GFS.helper;
using GFS.Structures;

namespace GFS;

public class FileSystemNode : IEnumerable<FileSystemNode>
{
    public string Name { get; set; }
    public bool IsDirectory { get; }
    public string Path { get; }
    public MyList<FileSystemNode> Children { get; set; } = new();
    private MyList<int> sectorIds = new();

    public FileSystemNode(string path, string name, bool isDirectory)
    {
        Name = name;
        Path = path;
        IsDirectory = isDirectory;
    }

    public string Serialize()
    {
        var output = new MyStringBuilder();
        output.Append(Path + " ");
        output.Append(Name + " ");
        output.Append(IsDirectory + " ");
        if (!IsDirectory)
        {
            foreach (var sectorId in sectorIds)
            {
                output.Append(sectorId);
            }
        }

        output.Append("\n");
        foreach (var child in Children)
        {
            output.Append(child.Serialize());
        }

        return output.ToString();
    }


    public FileSystemNode? getChildByName(string name)
    {
        foreach (var fileSystemNode in Children)
        {
            if (fileSystemNode.Name == name)
            {
                return fileSystemNode;
            }
        }

        return null;
    }

    public IEnumerator<FileSystemNode> GetEnumerator()
    {
        MyQueue<FileSystemNode> queue = new MyQueue<FileSystemNode>();
        queue.Enqueue(this);
        while (!queue.isEmpty())
        {
            var current = queue.Dequeue();
            yield return current;
            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    public override string ToString()
    {
        return $"{Path} {Name} {IsDirectory}";
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void PrintTree(int level = 0)
    {
        string indentation = new string('-', level * 2);
        Console.WriteLine(indentation + Name + " " + IsDirectory);

        foreach (var child in Children)
        {
            child.PrintTree(level + 1);
        }
    }
}