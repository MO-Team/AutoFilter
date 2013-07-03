using System;
using System.Linq.Expressions;
using AutoFilter.Interfaces;

namespace AutoFilter
{
    /// <summary>
    /// A filter engine which allows to map filters to entities
    /// </summary>
    public class FilterEngine : IFilterEngine
    {
        private readonly IFilterConfiguration _configuration;

        /// <summary>
        /// Creates new Filter Engine using the following Filter configuration
        /// </summary>
        /// <param name="configuration">filter configuration</param>
        public FilterEngine(IFilterConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// build expression from a filter object using a mapping saved in the filter engine
        /// </summary>
        /// <typeparam name="TFilter">filter type</typeparam>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <param name="filter">filter object</param>
        /// <returns>predicate created form the filter</returns>
        public Expression<Func<TEntity, bool>> BuildExpression<TFilter, TEntity>(TFilter filter)
            where TEntity : class
            where TFilter : class
        {
            var expressionBuilder = _configuration.GetFilter<TFilter, TEntity>();
            if (expressionBuilder == null)
            {
                throw new FilterNotMappedException(typeof (TFilter), typeof (TEntity));
            }
            return expressionBuilder.BuildExpression(filter);
        }
    }
}
