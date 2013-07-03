using System;
using System.Linq.Expressions;

namespace AutoFilter.Interfaces
{

    /// <summary>
    /// auto maps Expression from filter and entity 
    /// </summary>
    /// <typeparam name="TFilter">Filter Type</typeparam>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public interface IFilterExpressionBuilder<in TFilter, TEntity>
        where TFilter : class
        where TEntity : class
    {
        /// <summary>
        /// builds predicate expression from filter
        /// </summary>
        /// <param name="filter">filter to build predicate from</param>
        /// <returns>predicate</returns>
        Expression<Func<TEntity, bool>> BuildExpression(TFilter filter);
    }
}
