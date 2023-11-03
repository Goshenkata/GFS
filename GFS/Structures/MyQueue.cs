namespace GFS.Structures;

public class MyQueue<T>
{
    private MyList<T> _list = new MyList<T>();

    public void Enqueue(T data)
    {
        _list.AddLast(data);
    }
    
    public T Dequeue()
    {
        var output = _list.GetFirst();
        _list.RemoveFirst();
        return output;
    }

    public T Peek()
    {
        return _list.GetFirst();
    }

    public bool IsEmpty()
    {
        return _list.isEmpty();
    }
}