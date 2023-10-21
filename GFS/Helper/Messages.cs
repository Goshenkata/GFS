namespace GFS.helper;

public static class Messages
{
    public const string DefaultFilename = "Filesystem.GFS";
    public const string InvalidCommand = "Invalid command, type help for a list of available commands.";
    public const string EnteringInteractiveMode = "Entering interactive mode...";
    public const string EnterMaxFSSize = "Enter the maximum filesystem size in format {number unit} (default 1 GB): ";
    public const string EnterSectorSize = "Enter the sector size in KB (default 64KB): ";
    public const string FilesystemNotFound = "Filesystem not found.";

    public const string FilesystemNotFoundHelp =
        "File system not found please create one using GFS create {max size} {sector size}\nFor more information type GFS create";

    public const string Prompt = "GFS: ";

    public const string ErrorCreatingFileSystem =
        "Invalid input, please use GFS create {max size} {sector size in KB}, for example: GFS create 1 GB 16";

    public const string InvalidArgumentList = "Invalid argument list, type help for documentation.";
    public const string Atleast16Sectors = "The filesystem should be large enough for atleast 16 sectors";
}