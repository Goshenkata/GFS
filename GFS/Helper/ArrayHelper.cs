namespace GFS.helper;

public class ArrayHelper<T>
{
    public static T[] subArray(T[] original, int start, int end)
    {
        if (start < 0 || end < start || end > original.Length)
        {
            throw new ArgumentException("Invalid start or end indices.");
        }

        int length = end - start;
        T[] result = new T[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = original[start + i];
        }
        return result;
    }
}