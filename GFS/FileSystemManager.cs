using GFS.helper;

namespace GFS;

public class FileSystemManager
{
    private bool b;
    public bool IsInit()
    {
        return b;
    }

    public void CreateFilesystem(int maxSize, int sectorSize)
    {
        Console.WriteLine($"maxSize = {maxSize}, sectorSize = {sectorSize}");
        b = true;
    }
}