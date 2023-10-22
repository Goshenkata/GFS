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
            if (CharHelper.isUppercaseLetter(c))
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

        if (!accumulator.isEmpty())
        {
            output.AddLast(accumulator.ToString());
        }

        return output.GetArray();
    }

    public static bool isPath(string name)
    {
        return name != null && name[0] == '/';
    }

    public static string Join(string[] items, string delimiter)
    {
        if (items == null || items.Length == 0)
        {
            return string.Empty;
        }

        MyStringBuilder sb = new MyStringBuilder();
        for (int i = 0; i < items.Length; i++)
        {
            sb.Append(items[i]);
            if (i < items.Length - 1)
            {
                sb.Append(delimiter);
            }
        }

        return sb.ToString();
    }

    public static string Join(string[] items, string delimiter, int begin, int end)
    {
        if (items == null || items.Length == 0)
        {
            return string.Empty;
        }

        if (begin < 0 && end >= items.Length)
        {
            throw new IndexOutOfRangeException();
        }

        MyStringBuilder sb = new MyStringBuilder();
        for (int i = begin; i <= end; i++)
        {
            sb.Append(items[i]);
            if (i < items.Length - 1)
            {
                sb.Append(delimiter);
            }
        }

        return sb.ToString();
    }

    public static bool isValidNodeName(string nodeName)
    {
        //starts with letterOrDigit
        if (!CharHelper.isLetterOrDigit(nodeName[0]))
            return false;
        //cannot end with dot
        if (nodeName[nodeName.Length - 1] == '.')
            return false;

        for (var index = 1; index < nodeName.Length; index++)
        {
            var c = nodeName[index];
            if (!(CharHelper.isLetterOrDigit(c) || c == '-' || c == '.' || c == '_'))
            {
                return false;
            }
        }

        return true;
    }
}