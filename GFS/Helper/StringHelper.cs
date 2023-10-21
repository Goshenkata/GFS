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

        var accumulator = new MyStringBuilder();
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
        var output = new MyStringBuilder();
        foreach (var c in s)
        {
            if (c >= 'A' && c <= 'Z')
                output.Append((char)(c + 32));
            else
                output.Append(c);
        }

        return output.ToString();
    }

    public static string[] Split(string s, char seperator)
    {
        MyList<string> output = new MyList<string>();
        var accumulator = new MyStringBuilder();
        bool metSeperator = false;
        foreach (var c in s.ToCharArray())
        {
            if (c == seperator)
            {
                if (!metSeperator && !accumulator.isEmpty())
                {
                    output.AddLast(accumulator.ToString());
                    accumulator.Clear();
                    metSeperator = true;
                }
            }
            else
            {
                metSeperator = false;
                accumulator.Append(c);
            }
        }

        output.AddLast(accumulator.ToString());

        return output.GetArray();
    }
}