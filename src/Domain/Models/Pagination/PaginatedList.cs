namespace Domain.Models.Pagination;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public int PageSize { get; }
    public string Keyword { get; }

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize, string keyword)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        Keyword = keyword;
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        var count = items.Count;

        return await Task.FromResult(new PaginatedList<T>(items, count, pageNumber, pageSize, string.Empty));
    }
    public static async Task<PaginatedList<T>> CreateAsync(IEnumerable<T> source, int pageNumber, int pageSize, int totalCount, string keyword)
    {
        return await Task.FromResult(new PaginatedList<T>(source.ToList(), totalCount, pageNumber, pageSize, keyword));
    }
}
