using GFS.helper;

namespace GFS.Structures;

public class MyStringBuilder
{
    private char[] buffer = new char[16];
    private int length = 0;
    public int Length => length;

    public MyStringBuilder(string initialString)
    {
        Append(initialString);
    }

    public MyStringBuilder() { }
    public void Append(char c)
    {
        CheckCapacity(length + 1);
        buffer[length] = c;
        length++;
    }

    private void CheckCapacity(int requiredLength)
    {
        if (requiredLength > buffer.Length)
        {
            char[] newArr = new char[MyMath.Max(buffer.Length * 2, requiredLength)];
            for (var i = 0; i < buffer.Length; i++)
            {
                newArr[i] = buffer[i];
            }

            buffer = newArr;
        }
    }

    public override string ToString()
    {
        return new string(buffer, 0, length);
    }

    public void Clear()
    {
        length = 0;
        buffer = new char[16];
    }

    public void Append(string s)
    {
        CheckCapacity(length + s.Length);
        for (var i = 0; i < s.Length; i++)
        {
            buffer[length + i] = s[i];
        }
        length += s.Length;
    }

    public void Append(int n)
    {
        Append((char) (n + 32));
    }

    public bool isEmpty()
    {
        return length == 0;
    }
}