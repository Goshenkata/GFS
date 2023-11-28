namespace GFS.Structures
{
    public class MyStack<T>
    {
        MyList<T> data = new MyList<T>();
        public int Count { get { return data.Count; } }
        public T Pop()
        {
            var output = data.GetLast();
            data.RemoveLast();
            return output;
        }
        public T Peek()
        {
            return data.GetLast();
        }
        public void Push(T value)
        {
            data.AddLast(value);
        }
        public bool isEmpty()
        {
            return data.isEmpty();
        }

        public void Clear()
        {
            data.Clear();
        }
    }
}
