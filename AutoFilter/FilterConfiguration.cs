using System;
using System.Collections.Generic;
using AutoFilter.Interfaces;

namespace AutoFilter
{
    ///<summary>
    /// Creates filter configuration mapping
    ///</summary>
    public class FilterConfiguration : IFilterConfiguration
    {
        private readonly IDictionary<KeyValuePair<Type, Type>, IFilterConfigurationValidator> _filters = new Dictionary<KeyValuePair<Type, Type>, IFilterConfigurationValidator>();

        /// <summary>
        /// returns FilterExpressionBuilder 
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IFilterExpressionBuilder<TFilter, TEntity> GetFilter<TFilter, TEntity>() where TFilter : class where TEntity : class
        {
            var key = new KeyValuePair<Type, Type>(typeof(TFilter), typeof(TEntity));
            if (_filters.ContainsKey(key))
            {
                return _filters[key] as IFilterExpressionBuilder<TFilter, TEntity>;
            }
            return null;
        }

        /// <summary>
        /// Map a filter type to an entity type
        /// </summary>
        /// <typeparam name="TFilter">filter type</typeparam>
        /// <typeparam name="TEntity">filter type</typeparam>
        /// <returns>corresponding filter expression builder</returns>
        public IFilterDefinition<TFilter, TEntity> CreateFilter<TFilter, TEntity>()
            where TEntity : class
            where TFilter : class
        {
            var key = new KeyValuePair<Type, Type>(typeof(TFilter), typeof(TEntity));

            if (!_filters.ContainsKey(key))
            {
                _filters.Add(key, new FilterExpressionBuilder<TFilter, TEntity>());
            }

            return (FilterExpressionBuilder<TFilter, TEntity>)_filters[key];
        }

        /// <summary>
        /// Returns a list of all mapped filters
        /// </summary>
        /// <returns>List of all filters</returns>
        public IEnumerable<object> GetFilters()
        {
            return _filters.Values;
        }

        /// <summary>
        /// Validate that all properties in all filters were mapped or ignored
        /// </summary>
        public void AssertConfigurationIsValid()
        {
            foreach (var filter in _filters.Values)
            {
                filter.AssertConfigurationIsValid();
            }
        }
    }
}
