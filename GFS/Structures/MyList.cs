using System.Collections;

namespace GFS.Structures;

public class MyList<T> : IEnumerable<T>
{
    private Node<T>? head;
    private Node<T>? tail;
    private int count;
    public int Count => count;

    public T this[int indx]
    {
        get => GetAt(indx);
        set => SetAt(indx, value);
    }

    private Node<T> GetNodeAt(int index)
    {
        if (index < 0 && index >= count)
        {
            throw new IndexOutOfRangeException();
        }

        var current = head;
        for (int i = 0; i < index; i++)
        {
            current = current.Next;
        }

        return current;
    }

    public void SetAt(int index, T data)
    {
        Node<T> current = GetNodeAt(index);
        current.Data = data;
    }

    public void RemoveAt(int index)
    {
        var current = GetNodeAt(index);
        if (current.Prev != null)
        {
            current.Prev.Next = current.Next;
        }
        else
        {
            head = current.Next;
        }

        if (current.Next != null)
        {
            current.Next.Prev = current.Prev;
        }
        else
        {
            tail = current.Prev;
        }

        current = null;
        count--;
    }

    public void AddLast(T data)
    {
        var newNode = new Node<T>(data);
        if (head is null)
        {
            head = newNode;
            tail = newNode;
        }
        else
        {
            newNode.Prev = tail;
            tail.Next = newNode;
            tail = newNode;
        }

        count++;
    }

    public void AddLast(T[] data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            AddLast(data[i]);
        }
    }

    public T GetLast()
    {
        if (tail is not null)
        {
            return tail.Data;
        }

        throw new IndexOutOfRangeException();
    }

    public T GetFirst()
    {
        if (head is not null)
        {
            return head.Data;
        }

        throw new IndexOutOfRangeException();
    }

    public T GetAt(int index)
    {
        var current = GetNodeAt(index);
        return current.Data;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var current = head;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        string output = "[ ";
        var current = head;
        while (current is not null)
        {
            output += current.Data + " ";
            current = current.Next;
        }

        output += "]";
        return output;
    }

    public void AddAt(int index, T data)
    {
        if (index == 0)
        {
            AddFirst(data);
        }
        else if (index == count)
        {
            AddLast(data);
        }
        else
        {
            var newNode = new Node<T>(data);
            var current = GetNodeAt(index - 1);
            newNode.Next = current.Next;
            newNode.Prev = current;
            current.Next.Prev = newNode;
            current.Next = newNode;
            count++;
        }
    }

    public void AddFirst(T data)
    {
        var newNode = new Node<T>(data);

        if (head == null)
        {
            head = newNode;
            tail = newNode;
        }
        else
        {
            newNode.Next = head;
            head.Prev = newNode;
            head = newNode;
        }

        count++;
    }

    public T[] GetArray()
    {
        T[] array = new T[count];
        int i = 0;
        var current = head;
        while (current != null)
        {
            array[i] = current.Data;
            current = current.Next;
            i++;
        }

        return array;
    }

    public bool isEmpty()
    {
        return Count == 0;
    }

    public void RemoveLast()
    {

        Node<T> newTail;
        if (this.tail.Prev == null)
            newTail = head;
        else
            newTail = this.tail.Prev;
        tail.Prev = null;
        tail = newTail;
        count--;
    }

    public void RemoveFirst()
    {
        var newHead = this.head.Next;
        head.Next = null;
        head = newHead;
        count--;
    }

    public bool Contains(T value)
    {
        foreach (var v in this)
        {
            if (EqualityComparer<T>.Default.Equals(v, value))
            {
                return true;
            }
        }

        return false;
    }

    public void Clear()
    {
        count = 0;
        head = null;
        tail = null;
    }
}

internal class Node<T>
{
    public T Data { get; set; }
    public Node<T>? Next { get; set; }
    public Node<T>? Prev { get; set; }

    public Node(T data)
    {
        Data = data;
        Next = null;
        Prev = null;
    }
}