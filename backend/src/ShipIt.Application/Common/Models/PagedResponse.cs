namespace ShipIt.Application.Common.Models;

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages)
{
    public static PagedResponse<T> Create(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        var totalPages = 0;
        if (totalCount > 0)
        {
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        return new PagedResponse<T>(
            items,
            pageNumber,
            pageSize,
            totalCount,
            totalPages);
    }
}

public static class PagedResponseExtensions
{
    public static PagedResponse<T> ToPagedResponse<T>(
        this IReadOnlyList<T> source,
        int pageNumber,
        int pageSize)
    {
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return PagedResponse<T>.Create(
            items,
            pageNumber,
            pageSize,
            source.Count);
    }
}
