using System.Text;
using GFS.Structures;

namespace GFS.helper;

public class StringHelper
{
    public static string[] SplitCommand(string? command)
    {
        if (command is null)
        {
            return Array.Empty<string>();
        }

        StringBuilder accumulator = new StringBuilder();
        MyList<string> list = new MyList<string>();
        bool isInWhiteSpace = false;
        foreach (var c in command)
        {
            if (c == ' ')
            {
                if (!isInWhiteSpace)
                {
                    list.AddLast(accumulator.ToString());
                    accumulator.Clear();
                    isInWhiteSpace = true;
                }
            }
            else
            {
                accumulator.Append(c);
                isInWhiteSpace = false;
            }
        }

        if (accumulator.Length > 0)
        {
            list.AddLast(accumulator.ToString());
        }

        return list.GetArray();
    }

    public static string ToLowerCase(string s)
    {
        StringBuilder output = new StringBuilder();
        foreach (var c in s)
        {
            if (c >= 'A' && c <= 'Z')
                output.Append((char)(c + 32));
            else
                output.Append(c);
        }

        return output.ToString();
    }
}