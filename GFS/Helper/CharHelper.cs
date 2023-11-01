namespace GFS.helper;

public class CharHelper
{
    public static bool isUppercaseLetter(char c)
    {
        return c is >= 'A' and <= 'Z';
    }
    public static bool isLowercaseLetter(char c)
    {
        return c is >= 'a' and <= 'z';
    }

    public static bool isLetter(char c)
    {
        return isUppercaseLetter(c) || isLowercaseLetter(c);
    }
    
    public static bool isDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    public static bool isLetterOrDigit(char c)
    {
        return isLetter(c) || isDigit(c);
    }
}