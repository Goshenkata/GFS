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

    public static T[] mergeArrays(T[] a, T[] b)
    {
        var result = new T[a.Length + b.Length];
        var lastIndx = 0;
        for (var i = 0; i < a.Length; i++)
            result[lastIndx++] = a[i];
        for (var i = 0; i < b.Length; i++)
            result[lastIndx++] = b[i];
        return result;
    }

    internal T1[] FilledArray<T1>(T1 v)
    {
        throw new NotImplementedException();
    }
}