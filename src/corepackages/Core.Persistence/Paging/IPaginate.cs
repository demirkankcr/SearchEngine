namespace Core.Persistence.Paging;

public interface IPaginate<T>
{
    IList<T> Items { get; }
}

public class Paginate<T> : IPaginate<T>
{
    public Paginate(IList<T> items)
    {
        Items = items;
    }

    public IList<T> Items { get; }
}

