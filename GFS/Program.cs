using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
                    command += maxSize + " ";
                    Console.Write(Messages.EnterSectorSize);
                    var sectorSize = Console.ReadLine();
                    command += sectorSize;
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
            ProcessInput(args, fileSystemManager);
            if (!fileSystemManager.IsInit())
            {
                Console.WriteLine(Messages.FilesystemNotFoundHelp);
            }
        }
    }

    private static bool ProcessInput(string[] command, FileSystemManager fileSystemManager)
    {
        if (command.Length == 0 || fileSystemManager.IsInit())
        {
            return false;
        }

        switch (command[0])
        {
            case "create":
                if (command.Length == 1)
                {
                    command = new[] { "create", "1", "GB", "64" };
                }

                return CreateCommand(command, fileSystemManager);
            default:
                Console.Error.WriteLine(Messages.InvalidCommand);
                return false;
        }
    }

    private static bool CreateCommand(string[] command, FileSystemManager fileSystemManager)
    {
        if (fileSystemManager.IsInit())
        {
            Console.Error.WriteLine("Filesystem is already initialized");
            return false;
        }

        if (command.Length != 4)
        {
            Console.Error.WriteLine(Messages.InvalidArgumentList);
            return false;
        }

        bool isValid = int.TryParse(command[1], out int maxSize);
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

        if ((sectorSize * 16) > maxSize)
        {
            Console.Error.WriteLine(Messages.Atleast16Sectors);
            return false;
        }
        fileSystemManager.CreateFilesystem(maxSize, sectorSize);
        return true;
    }
}