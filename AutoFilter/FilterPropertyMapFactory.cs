using System;
using System.Linq.Expressions;
using AutoFilter.Interfaces;

namespace AutoFilter
{   
    /// <summary>
    /// Provides functionality to create a new <see cref="IFilterPropertyMap{TFilter, TEntity, TFilterProperty, TEntityProperty}"/>.
    /// </summary>
    public class FilterPropertyMapFactory<TFilter, TEntity> : IFilterPropertyMapFactory<TFilter, TEntity>
        where TFilter : class
        where TEntity : class
    {

        #region Ctor
        ///<summary>
        /// ctor
        ///</summary>
        ///<param name="definition"></param>
        public FilterPropertyMapFactory(IFilterDefinition<TFilter, TEntity> definition)
        {
            Ensure.ArgumentNotNull(() => definition);
            FilterDefinition = definition;
        }
        #endregion

        private IFilterDefinition<TFilter,TEntity> FilterDefinition { get; set; }

        #region IFilterPropertyMapFactory Members

        /// <summary>
        /// Create new filter property map
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <typeparam name="TEntityProperty"></typeparam>
        /// <param name="filterProperty">property to filter by</param>
        /// <param name="entityProperty">entity property to filter from</param>
        /// <param name="predicate">predicate involving the filter property</param>
        /// <returns></returns>
        public IFilterPropertyMap<TFilter, TEntity,TFilterProperty, TEntityProperty> CreateFilterPropertyMap<TFilterProperty, TEntityProperty>
                                                                                               (Expression<Func<TFilter, TFilterProperty>> filterProperty, 
                                                                                                Expression<Func<TEntity, TEntityProperty>> entityProperty, 
                                                                                                Expression<Func<TFilterProperty, TEntityProperty, bool>> predicate)
        {
            return new FilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty>(filterProperty, entityProperty, FilterDefinition, predicate);
        }

        #endregion

    }
}
