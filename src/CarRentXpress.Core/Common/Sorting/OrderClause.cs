using System.Linq.Expressions;

namespace CarRentXpress.Data.Sorting;

public class OrderClause<TEntity> : IOrderClause<TEntity>
{
    public required Expression<Func<TEntity, object>> Expression { get; init; }
    public bool IsAscending { get; init; } = true;
}