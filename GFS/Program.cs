using System.Runtime.CompilerServices;
using GFS.helper;
using GFS.Structures;

namespace GFS;

public class Program
{
    public static void Main(string[] args)
    {
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
                    if (maxSize == "")
                    {
                        maxSize = "1 GB ";
                    }

                    command += maxSize + " ";

                    Console.Write(Messages.EnterSectorSize);
                    var sectorSize = Console.ReadLine();
                    if (sectorSize == "")
                    {
                        sectorSize = "64";
                    }

                    command += sectorSize;
                    Console.WriteLine(command);
                } while (!ProcessInput(StringHelper.SplitCommand(command), fileSystemManager) &&
                         !fileSystemManager.IsInit());
            }

            Console.Write(Messages.Prompt);
            var input = Console.ReadLine();
            while (input != "exit")
            {
                var command = StringHelper.SplitCommand(input);
                ProcessInput(command, fileSystemManager);
                Console.Write(Messages.Prompt);
                input = Console.ReadLine();
            }
        }
        //else use args
        else
        {
            if (fileSystemManager.IsInit()) Console.WriteLine(Messages.FilesystemNotFoundHelp);
            do
            {
                ProcessInput(args, fileSystemManager);
            } while (!fileSystemManager.IsInit());

            ProcessInput(args, fileSystemManager);
        }
    }

    private static bool ProcessInput(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length == 0)
        {
            Console.Error.WriteLine(Messages.InvalidCommand);
            return false;
        }

        switch (command[0])
        {
            case "create":
                return CreateCommand(command, fileSystemManager);
            default:
                Console.Error.WriteLine(Messages.InvalidCommand);
                return false;
        }
    }

    private static bool CreateCommand(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length != 4)
        {
            Console.Error.WriteLine(Messages.InvalidArgumentList);
            return false;
        }

        bool isValid = int.TryParse(command[1], out int maxSize);
        command[2] = StringHelper.ToLowerCase(command[2]);
        isValid = isValid && (command[2] == "kb" | command[2] == "mb" || command[2] == "gb");
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
        if ((sectorSize * 16) > maxSize)
        {
            Console.Error.WriteLine(Messages.Atleast16Sectors);
            return false;
        }

        if (isValid)
        {
            fileSystemManager.CreateFilesystem(maxSize, sectorSize);
            return true;
        }

        Console.Error.WriteLine(Messages.ErrorCreatingFileSystem);
        return false;
    }
}