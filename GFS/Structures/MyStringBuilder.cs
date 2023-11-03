namespace GFS.Structures;

public class MyStringBuilder
{
    private char[] _buffer = new char[16];
    public int Length { get; private set; }

    public MyStringBuilder(string initialString)
    {
        Append(initialString);
    }

    public MyStringBuilder() { }
    public void Append(char c)
    {
        CheckCapacity(Length + 1);
        _buffer[Length] = c;
        Length++;
    }

    private void CheckCapacity(int requiredLength)
    {
        if (requiredLength > _buffer.Length)
        {
            char[] newArr = new char[Math.Max(_buffer.Length * 2, requiredLength)];
            for (var i = 0; i < _buffer.Length; i++)
            {
                newArr[i] = _buffer[i];
            }

            _buffer = newArr;
        }
    }

    public override string ToString()
    {
        return new string(_buffer, 0, Length);
    }

    public void Clear()
    {
        Length = 0;
        _buffer = new char[16];
    }

    public void Append(string s)
    {
        CheckCapacity(Length + s.Length);
        for (var i = 0; i < s.Length; i++)
        {
            _buffer[Length + i] = s[i];
        }
        Length += s.Length;
    }

    public void Append(int n)
    {
        Append((char) (n + 32));
    }

    public bool IsEmpty()
    {
        return Length == 0;
    }
}