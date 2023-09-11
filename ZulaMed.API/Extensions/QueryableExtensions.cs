using System.Linq.Expressions;

namespace ZulaMed.API.Extensions;

public record PaginationOptions(int Page, int PageSize);


public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T, TKey>(
        this IQueryable<T> queryable,
        Expression<Func<T, TKey>> orderBySelector,
        PaginationOptions options)
    {
        var (page, pageSize) = options;
        var from = (page - 1) * pageSize;
        var to = page * pageSize;
        return queryable.OrderByDescending(orderBySelector).Skip(from).Take(to);
    }
}