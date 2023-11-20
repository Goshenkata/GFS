using System.Collections;

namespace GFS.Structures;

public class MyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
    private MyList<KeyValuePair<TKey, TValue>> items;

    public MyDictionary()
    {
        items = new MyList<KeyValuePair<TKey, TValue>>();
    }

    public bool Add(TKey key, TValue value)
    {
        if (!ContainsKey(key))
        {
            items.AddLast(new KeyValuePair<TKey, TValue>(key, value));
            return true;
        }

        return false;
    }

    public TValue Get(TKey key)
    {
        foreach (var item in items)
        {
            if (EqualityComparer<TKey>.Default.Equals(item.Key, key))
            {
                return item.Value;
            }
        }
        return default(TValue);
    }

    public void Remove(TKey key)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(items[i].Key, key))
            {
                items.RemoveAt(i);
            }
        }
    }

    public bool ContainsKey(TKey key)
    {
        foreach (var item in items)
        {
            if (EqualityComparer<TKey>.Default.Equals(item.Key, key))
            {
                return true;
            }
        }

        return false;
    }

    internal class KeyValuePair
    {
        public TKey Key { get; }
        public TValue Value { get; }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}