using GFS.Structures;

namespace GFS.helper;

public class MyQueue<T>
{
    private MyList<T> list = new MyList<T>();

    public void Enqueue(T data)
    {
        list.AddLast(data);
    }
    
    public T Dequeue()
    {
        var output = list.GetFirst();
        list.RemoveFirst();
        return output;
    }

    public T Peek()
    {
        return list.GetFirst();
    }

    public bool isEmpty()
    {
        return list.isEmpty();
    }
}