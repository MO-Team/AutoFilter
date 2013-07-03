using System.Collections.Generic;

namespace AutoFilter.Interfaces
{
    ///<summary>
    /// Configures AutoFilter mappings
    ///</summary>
    public interface IFilterConfiguration : IFilterConfigurationValidator
    {

        /// <summary>
        /// returns a filter mapping
        /// </summary>
        /// <typeparam name="TFilter">filter type</typeparam>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <returns></returns>
        IFilterExpressionBuilder<TFilter, TEntity> GetFilter<TFilter, TEntity>()
            where TEntity : class
            where TFilter : class;

        /// <summary>
        /// Map a filter type to an entity type
        /// </summary>
        /// <typeparam name="TFilter">filter type</typeparam>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <returns>corresponding filter expression builder</returns>
        IFilterDefinition<TFilter, TEntity> CreateFilter<TFilter, TEntity>()
            where TEntity : class
            where TFilter : class;

        /// <summary>
        /// Returns all filters
        /// </summary>
        /// <returns>List of all filters</returns>
        IEnumerable<object> GetFilters();
    }
}