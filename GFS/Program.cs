using GFS.enums;
using GFS.helper;
using System.Text;

namespace GFS;

public class Program
{
    public static void Main(string[] args)
    {
        /*
        FileStream fs = new FileStream("test.txt", FileMode.Create,FileAccess.ReadWrite);
        fs.SetLength(100000L);
        BinaryWriter bw = new BinaryWriter(fs);
        BinaryReader br = new BinaryReader(fs);

        SectorData sectorData = new SectorData(20008L, 100000L - 8L, fs, bw, br, 100);
        sectorData.CreateSectorData();

        FilesystemData fsData = new FilesystemData(0L, 20000L, fs, bw, br, ref sectorData);
        fsData.InitFs();
        Console.WriteLine(fsData.LoadById(0));
        fsData.CreateNode("dir1", true, 0);
        fsData.CreateNode("dir2", true, 0);
        fsData.PrintTree(0, fsData.LoadById(0));
        var dir1 = fsData.GetNodeByPath("/dir1");
        var dir2 = fsData.GetNodeByPath("/dir2");
        fsData.RemoveChild(fsData.LoadById(0), dir1);
        Console.WriteLine("After remove===");
        fsData.PrintTree(0, fsData.LoadById(0));
        return;
        */

        FileSystemManager fileSystemManager = new FileSystemManager();

        //Enter interactive mode if no arguments passed;
        if (args.Length == 0)
        {
            Console.WriteLine(Messages.EnteringInteractiveMode);

            if (!fileSystemManager.IsInit())
            {
                Console.WriteLine(Messages.FilesystemNotFound);

                string command;
                do
                {
                    command = "create ";
                    Console.Write(Messages.EnterMaxFSSize);
                    var maxSize = Console.ReadLine();
                    command += maxSize + " ";
                    Console.Write(Messages.EnterSectorSize);
                    var sectorSize = Console.ReadLine();
                    command += sectorSize;
                } while (!ProcessInput(StringHelper.SplitCommand(command), fileSystemManager) &&
                         !fileSystemManager.IsInit());
            }
            else
            {
                fileSystemManager.LoadFs();
            }

            Console.Write(Messages.Prompt, fileSystemManager.CurrentPath);
            var input = Console.ReadLine();
            while (input != "exit")
            {
                var command = StringHelper.SplitCommand(input);
                ProcessInput(command, fileSystemManager);
                Console.Write(Messages.Prompt, fileSystemManager.CurrentPath);
                input = Console.ReadLine();
            }
        }
        //else use args
        else
        {
            if (fileSystemManager.IsInit())
            {
                fileSystemManager.LoadFs();
            }

            ProcessInput(args, fileSystemManager);
            if (!fileSystemManager.IsInit())
            {
                Console.WriteLine(Messages.FilesystemNotFoundHelp);
            }
        }
    }

    private static bool ProcessInput(string[] command, FileSystemManager fileSystemManager)

    {
        if (command.Length == 0)
        {
            return false;
        }

        switch (command[0])
        {
            case "create":
                if (command.Length == 1)
                {
                    command = new[] { "create", "10", "GB", "8" };
                }

                return CreateCommand(command, fileSystemManager);
            case "help":
                Console.WriteLine(Messages.HelpCommand);
                return true;
            case "mkdir":
                return DirectoryCommand(command, fileSystemManager, enums.DirectoryCommand.Mkdir);
            case "tree":
                fileSystemManager.PrintTree();
                return true;
            case "rmdir":
                return DirectoryCommand(command, fileSystemManager, enums.DirectoryCommand.Rmdir);
            case "cd":
                return DirectoryCommand(command, fileSystemManager, enums.DirectoryCommand.Cd);
            case "sector":
                return SectorCommand(command, fileSystemManager);
            case "ls":
                if (command.Length == 1)
                {
                    var data = fileSystemManager.Ls(fileSystemManager.CurrentPath);
                    foreach (var item in data)
                        Console.WriteLine(item);
                    return true;
                }

                if (command[1] == "/")
                {
                    var data = fileSystemManager.Ls("/");
                    foreach (var item in data)
                        Console.WriteLine(item);

                    return true;
                }
                return DirectoryCommand(command, fileSystemManager, enums.DirectoryCommand.Ls);
            case "rm":
                return RmCommand(command, fileSystemManager);
            case "write":
                return WriteCommand(command, fileSystemManager);
            case "cat":
                return CatCommand(command, fileSystemManager);
            case "import":
                return ImportCommand(command, fileSystemManager);
            case "export":
                return ExportCommand(command, fileSystemManager);
            default:
                Console.Error.WriteLine(Messages.InvalidCommand);
                return false;
        }
    }

    //only for debug purposes
    private static bool SectorCommand(string[] command, FileSystemManager fileSystemManager)
    {
        int sector = int.Parse(command[1]);
        fileSystemManager.PrintSectorInfo(sector);
        return true;
    }

    private static bool RmCommand(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length < 2)
        {
            Console.WriteLine(command[1]);
            return false;
        }

        var path = ResolveToFullPath(command[1], fileSystemManager);
        if (!fileSystemManager.FileExists(path))
        {
            Console.WriteLine(Messages.FileDoesNotExist);
            return false;
        }
        fileSystemManager.RmFile(path);
        return true;
    }

    private static bool ExportCommand(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length < 3)
        {
            Console.WriteLine(Messages.InvalidArgumentList);
            return false;
        }

        var source = ResolveToFullPath(command[1], fileSystemManager);
        var destination = command[2];
        if (!fileSystemManager.FileExists(source))
        {
            Console.WriteLine(Messages.FileDoesNotExist);
        }

        return fileSystemManager.Export(source, destination);
    }

    private static bool ImportCommand(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length < 3)
        {
            Console.WriteLine(Messages.InvalidArgumentList);
            return false;
        }

        bool isAppend = command[1] == "+append";
        string source;
        string destination;
        if (isAppend)
        {
            if (command.Length < 4)
            {
                Console.WriteLine(Messages.InvalidArgumentList);
                return false;
            }

            source = command[2];
            destination = ResolveToFullPath(command[3], fileSystemManager);
        }
        else
        {
            source = command[1];
            destination = ResolveToFullPath(command[2], fileSystemManager);
        }

        if (!File.Exists(source))
        {
            Console.WriteLine(Messages.FileDoesNotExist);
            return false;
        }

        return fileSystemManager.ImportFile(source, destination, isAppend);
    }

    private static bool CatCommand(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length < 2)
        {
            Console.WriteLine(Messages.InvalidArgumentList);
            return false;
        }

        var path = ResolveToFullPath(command[1], fileSystemManager);
        if (!fileSystemManager.FileExists(path))
        {
            Console.WriteLine(Messages.FileDoesNotExist);
        }

        Console.WriteLine(fileSystemManager.Cat(path));
        return true;
    }

    private static bool WriteCommand(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length < 2)
        {
            Console.WriteLine(Messages.InvalidArgumentList);
            return false;
        }

        bool isAppend = command[1] == "+append";
        string filePath = isAppend ? command[2] : command[1];
        string data = "";
        if (isAppend)
        {
            if (command.Length >= 4)
            {
                data = command[3];
            }
        }
        else if (command.Length >= 3)
            data = command[2];

        if (data == null || data.Length == 0)
        {
            Console.WriteLine(Messages.DataEmpty);
        }

        filePath = ResolveToFullPath(filePath, fileSystemManager);
        if (isAppend && fileSystemManager.FileExists(filePath))
        {
            return fileSystemManager.AppendToFile(filePath, Encoding.UTF8.GetBytes(data));
        }

        return fileSystemManager.CreateFile(filePath, Encoding.UTF8.GetBytes(data));
    }

    private static string ResolveToFullPath(string name, FileSystemManager fs)
    {
        string output = "/";
        string[] pathParts = StringHelper.Split(name, '/');
        if (!StringHelper.IsPath(name))
        {
            output = fs.CurrentPath;
        }


        for (int i = 0; i < pathParts.Length; i++)
        {
            if (i == pathParts.Length - 1)
            {
                output += pathParts[i];
                break;
            }

            if (pathParts[i] == "..")
            {
                output = StringHelper.GetParentPath(output);
            }
            else
            {
                output += pathParts[i] + "/";
            }
        }

        return output;
    }

    private static bool DirectoryCommand(string[] command, FileSystemManager fileSystemManager,
        DirectoryCommand directoryCommand)
    {
        if (command.Length < 2)
        {
            if (directoryCommand != enums.DirectoryCommand.Ls)
            {
                Console.Error.WriteLine(Messages.InvalidArgumentList);
                return false;
            }
        }

        for (int i = 1; i < command.Length; i++)
        {
            string dirName = command[i];
            string parentPath = fileSystemManager.CurrentPath;

            if (dirName == "/")
            {
                if (directoryCommand == enums.DirectoryCommand.Cd)
                {
                    fileSystemManager.CurrentPath = "/";
                    return true;
                }

                if (directoryCommand != enums.DirectoryCommand.Ls)
                    return false;
            }

            if (StringHelper.IsPath(dirName))
            {
                var splitPath = StringHelper.Split(dirName, '/');
                parentPath = "/" + StringHelper.Join(splitPath, "/", 0, splitPath.Length - 2);
                dirName = splitPath[splitPath.Length - 1];

                if (!fileSystemManager.DirExists(parentPath))
                {
                    Console.Error.WriteLine(Messages.DirectoryDoesNotExist);
                    return false;
                }
            }

            switch (directoryCommand)
            {
                case enums.DirectoryCommand.Mkdir:
                    var mkdirResult = fileSystemManager.Mkdir(parentPath, dirName);
                    if (!mkdirResult.Success)
                    {
                        Console.WriteLine(mkdirResult.Message);
                    }
                    break;
                case enums.DirectoryCommand.Rmdir:

                    if (!fileSystemManager.DirExists(parentPath + dirName))
                    {
                        Console.Error.WriteLine(Messages.DirectoryDoesNotExist);
                        return false;
                    }

                    var rmdirResult = fileSystemManager.Rmdir(parentPath, dirName);
                    if (!rmdirResult.Success)
                    {
                        Console.WriteLine(rmdirResult.Message);
                    }
                    break;
                case enums.DirectoryCommand.Cd:
                    var fullPath = dirName == ".."
                        ? StringHelper.GetParentPath(parentPath)
                        : parentPath + dirName + "/";
                    if (!fileSystemManager.DirExists(fullPath))
                    {
                        Console.Error.WriteLine(Messages.DirectoryDoesNotExist);
                        return false;
                    }

                    fileSystemManager.CurrentPath = fullPath;
                    break;
                case enums.DirectoryCommand.Ls:
                    var dirPath = dirName == ".." ? StringHelper.GetParentPath(parentPath) : parentPath + dirName + "/";
                    if (!fileSystemManager.DirExists(dirPath))
                    {
                        Console.Error.WriteLine(Messages.DirectoryDoesNotExist);
                        return false;
                    }

                    foreach (var item in fileSystemManager.Ls(dirPath))
                        Console.WriteLine(item);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(directoryCommand), directoryCommand, null);
            }
        }

        return true;
    }

    private static bool CreateCommand(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length != 4)
        {
            Console.Error.WriteLine(Messages.InvalidArgumentList);
            return false;
        }

        bool isValid = long.TryParse(command[1], out long maxSize);
        command[2] = StringHelper.ToLowerCase(command[2]);
        isValid = isValid && (command[2] == "kb" || command[2] == "mb" || command[2] == "gb");
        switch (command[2])
        {
            case "mb":
                maxSize *= 1024;
                break;
            case "gb":
                maxSize *= 1024 * 1024;
                break;
        }

        isValid = int.TryParse(command[3], out int sectorSize) && isValid;

        if (!isValid)
        {
            Console.Error.WriteLine(Messages.ErrorCreatingFileSystem);
            return false;
        }

        //turn kb to bytes;
        maxSize *= 1024;
        sectorSize *= 1024;
        var result = fileSystemManager.CreateFilesystem(maxSize, sectorSize);
        if (!result.Success)
        {
            Console.WriteLine(result.Message);
            return false;
        }
        return true;
    }
}