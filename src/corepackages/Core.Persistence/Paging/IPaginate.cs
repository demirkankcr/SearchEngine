using System;
using System.Collections.Generic;

namespace Core.Persistence.Paging;

public interface IPaginate<T>
{
    int From { get; }
    int Index { get; }
    int Size { get; }
    int Count { get; }
    int Pages { get; }
    IList<T> Items { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
}

public class Paginate<T> : IPaginate<T>
{
    public Paginate()
    {
        Items = new List<T>();
    }

    public Paginate(IList<T> items, int index, int size, int count)
    {
        Items = items;
        Index = index;
        Size = size;
        Count = count;
        From = count == 0 ? 0 : index * size + 1;
        Pages = size == 0 ? 0 : (int)Math.Ceiling(count / (double)size);
    }

    public int From { get; }
    public int Index { get; }
    public int Size { get; }
    public int Count { get; }
    public int Pages { get; }
    public IList<T> Items { get; }
    public bool HasPrevious => Index > 0;
    public bool HasNext => Index + 1 < Pages;
}

