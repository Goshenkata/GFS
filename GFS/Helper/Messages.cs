namespace GFS.helper;

public static class Messages
{
    public const string InvalidCommand = "Invalid command, type help for a list of available commands.";
    public const string EnteringInteractiveMode = "Entering interactive mode...";
    public const string EnterMaxFSSize = "Enter the maximum filesystem size in format {number unit} (default 10 GB): ";
    public const string EnterSectorSize = "Enter the sector size in KB (default 8KB): ";
    public const string FilesystemNotFound = "Filesystem not found.";

    public const string FilesystemNotFoundHelp =
        "File system not found please create one using GFS create {max size} {sector size}";

    public const string Prompt = "GFS [{0}]: ";

    public const string ErrorCreatingFileSystem =
        "Invalid input, please use GFS create {max size} {sector size in KB}, for example: GFS create 1 GB 16";

    public const string InvalidArgumentList = "Invalid argument list, type help for documentation.";
    public const string Atleast16Sectors = "The filesystem should be large enough for atleast 16 sectors";
    public const string FilesystemLoadedSuccessfully = "Filesystem loaded succesfully";
    public const string FilesystemCreated = "Filesystem created succesfully";

    public const string HelpCommand = @"Gosho File System list of commands, the * symbol denotes an optional paramether: 

> create {sectorSize}* {sectorSizeUnit}* {maxFilesystemSizeKb}*          Creates the file system if it does not exist
> mkdir {dirName}                                                        Creates a directory
> tree                                                                   Prints the entire file system tree
> ls {dirName}*                                                          Lists the contents of a directorCreates a directory
> rmdir {dirName}                                                        Removes a directory
> cd {dirName}                                                           Changes the current directory
> rm {fileName}                                                          Removes the file
> write +append* {file} ""data""                                       Creates a file if it does not exists and writes the data
> import +append* {source} {destination}                                 Creates a file if it does not exists and writes the data
> export {source} {destination}                                          Creates a file if it does not exists and writes the data
";

    public const string DirectoryDoesNotExist = "Directory does not exists";
    public const string FileDoesNotExist = "File does not exists";
    public const string DirExists = "Directory already exists";
    public const string InvalidDirName = "Invalid directory name";
    public const string DirectoryIsNotEmpty = "Directory is not empty";
    public const string CorruptedSector = "Detected corrupted sector..";
    public const string CreateDir = "Create Directory";
    public const string EnterData = "Enter a data size unit";
    public const string FilesystemIsInit = "Filesystem is already initialized";
    public const string InvalidName = "Invalid name";
    public const string AlreadyExists = "This node already exists";
    public const string NothingSelected = "Nothing is selected";
}