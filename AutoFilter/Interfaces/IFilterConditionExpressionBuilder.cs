using System;
using System.Linq.Expressions;

namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Provides functionality to create a predicate expression of <typeparamref name="TEntity"/> class by <typeparamref name="TFilter"/>.
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface IFilterConditionExpressionBuilder<in TFilter, TEntity>
        where TFilter : class
        where TEntity : class
    {
        /// <summary>
        /// validates if predicate expression could be built for filter
        /// </summary>
        /// <param name="filter">filter to validate</param>
        /// <returns>true, if predicate could be built from filter</returns>
        bool CanBuildConditionExpression(TFilter filter);

        /// <summary>
        /// Create predicate expression for filter
        /// </summary>
        /// <param name="filter">filter</param>
        /// <returns>predicate expression of filter</returns>
        Expression<Func<TEntity, bool>> BuildConditionExpression(TFilter filter);
    }
}
