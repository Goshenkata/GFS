using GFS.helper;

namespace GFS;

public class FileSystemManager
{
    public bool isInit()
    {
        return File.Exists(Messages.DefaultFilename);
    }
}