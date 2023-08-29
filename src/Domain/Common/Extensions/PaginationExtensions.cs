using Domain.Models.Pagination;

namespace Domain.Common.Extensions;

public static class PaginationExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize, int totalCount, string keyword)
        => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize, totalCount, keyword);

    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
        => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize);

    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IEnumerable<TDestination> queryable, int pageNumber, int pageSize, int totalCount, string keyword)
        => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize, totalCount, keyword);
}
