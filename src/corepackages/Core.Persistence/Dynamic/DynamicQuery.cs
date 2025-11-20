namespace Core.Persistence.Dynamic;

public class DynamicQuery
{
    public string? Sort { get; set; }
    public string? Filter { get; set; }

    public DynamicQuery()
    {
    }

    public DynamicQuery(string? sort, string? filter)
    {
        Sort = sort;
        Filter = filter;
    }
}

