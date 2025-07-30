using System;
using System.Collections;
using System.Collections.Generic;


public class ReversedList<T> : IList<T>
{
    public Func<IList<T>> originalListProvider;
    IList<T> _originalList => originalListProvider();

    // public ReversedList(IList<T> originalList)
    // {
    //     _originalList = originalList ?? throw new ArgumentNullException(nameof(originalList));
    // }

    public T this[int index]
    {
        get => _originalList[Count - 1 - index];
        set => _originalList[Count - 1 - index] = value;
    }

    public int Count => _originalList.Count;

    public bool IsReadOnly => _originalList.IsReadOnly;

    public void Add(T item)
    {
        _originalList.Insert(0, item);
    }

    public void Clear()
    {
        _originalList.Clear();
    }

    public bool Contains(T item)
    {
        return _originalList.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            yield return _originalList[i];
        }
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(this[i], item))
                return i;
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        _originalList.Insert(Count - index, item);
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        _originalList.RemoveAt(Count - 1 - index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
