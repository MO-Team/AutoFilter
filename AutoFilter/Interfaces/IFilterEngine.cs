using System;
using System.Linq.Expressions;

namespace AutoFilter.Interfaces
{   
    /// <summary>
    /// A filter engine which allows to map filters to entities
    /// </summary>
    public interface IFilterEngine
    {
        
        /// <summary>
        /// build expression from a filter object using a mapping saved in the filter engine
        /// </summary>
        /// <typeparam name="TFilter">filter type</typeparam>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <param name="filter">filter object</param>
        /// <returns>predicate created form the filter</returns>
        Expression<Func<TEntity, bool>> BuildExpression<TFilter,TEntity>(TFilter filter) where TEntity : class where TFilter : class;
    }
}