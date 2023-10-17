using GFS.helper;
using GFS.Structures;

namespace GFS;

public class Program
{
    public static void Main(string[] args)
    {
        FileSystemManager fileSystemManager = new FileSystemManager();
        if (args.Length == 0)
        {
            Console.WriteLine(Messages.EnteringInteractiveMode);
            var input = Console.ReadLine();
            if (fileSystemManager.isInit())
            {
                
            }
            while (input != "exit")
            {
                input = Console.ReadLine();
            }
        }
        else
        {
            if (fileSystemManager.isInit())
            {
                
            }
        }
    }

    bool proccesInput(string command)
    {
        switch (command)
        {
            default:
                Console.WriteLine(Messages.InvalidCommand);
                return false;
        }
    }
}