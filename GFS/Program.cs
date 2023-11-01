﻿using GFS.enums;
using GFS.helper;

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
            else
            {
                fileSystemManager.LoadFs();
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
                    command = new[] { "create", "10", "GB", "32" };
                }

                return CreateCommand(command, fileSystemManager);
            case "help":
                Console.WriteLine(Messages.HelpCommand);
                return true;
            case "mkdir":
                return directoryCommand(command, fileSystemManager, DirectoryCommand.MKDIR);
            case "tree":
                fileSystemManager.PrintTree();
                return true;
            case "rmdir":
                return directoryCommand(command, fileSystemManager, DirectoryCommand.RMDIR);
            default:
                Console.Error.WriteLine(Messages.InvalidCommand);
                return false;
        }
    }

    private static bool directoryCommand(string[] command, FileSystemManager fileSystemManager, DirectoryCommand directoryCommand)
    {
        if (command.Length < 2)
        {
            Console.Error.WriteLine(Messages.InvalidArgumentList);
            return false;
        }

        for (int i = 1; i < command.Length; i++)
        {
            string dirName = command[i];
            string parentPath = fileSystemManager.CurrentPath;
            if (StringHelper.isPath(dirName))
            {
                if (fileSystemManager.DirExists(dirName))
                {
                    Console.Error.WriteLine(Messages.DirExists);
                    return false;
                }

                var splitPath = StringHelper.Split(dirName, '/');
                parentPath = "/" + StringHelper.Join(splitPath, "/", 0, splitPath.Length - 2);
                dirName = splitPath[splitPath.Length - 1];

                if (!fileSystemManager.DirExists(parentPath))
                {
                    Console.Error.WriteLine(Messages.DirectoryDoesNotExist);
                    return false;
                }
            }

            if (!StringHelper.isValidNodeName(dirName))
            {
                Console.Error.WriteLine(Messages.InvalidDirName);
                return false;
            }

            switch (directoryCommand)
            {
                case DirectoryCommand.MKDIR:
                    fileSystemManager.Mkdir(parentPath, dirName);
                    break;
                case DirectoryCommand.RMDIR:
                    fileSystemManager.Rmdir(parentPath, dirName);
                    break;
                case DirectoryCommand.CD:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(directoryCommand), directoryCommand, null);
            }
        }

        return true;
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

        if ((sectorSize * 16) > maxSize)
        {
            Console.Error.WriteLine(Messages.Atleast16Sectors);
            return false;
        }

        //turn kb to bytes;
        maxSize *= 1024;
        sectorSize *= 1024;
        //todo remove, just for testing
        maxSize = 10000;
        fileSystemManager.CreateFilesystem(maxSize, sectorSize);
        return true;
    }
}
