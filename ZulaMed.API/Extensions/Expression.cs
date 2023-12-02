using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ZulaMed.API.Extensions;

public static class SetPropertyCallsExtensions 
{
    public static Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> AppendSetProperty<TEntity>(
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> left,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> right)
    {
        var replace = new ReplacingExpressionVisitor(right.Parameters, new []{left.Body});
        var combined = replace.Visit(right.Body);
        return Expression.Lambda<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>(combined, left.Parameters);
    }
 
}