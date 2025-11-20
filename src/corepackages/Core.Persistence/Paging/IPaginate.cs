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

    public int From { get; set; }
    public int Index { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
    public int Pages { get; set; }
    public IList<T> Items { get; set; }

    public bool HasPrevious => Index > 0;
    public bool HasNext => Index + 1 < Pages;
}
