using System.Linq.Expressions;

namespace CarRentXpress.Data.Sorting;

public interface IOrderClause<TEntity>
{
    Expression<Func<TEntity, object>> Expression { get; init; }
    bool IsAscending { get; init; }
}