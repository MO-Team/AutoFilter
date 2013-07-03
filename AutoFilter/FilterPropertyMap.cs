using System;
using System.Linq.Expressions;
using AutoFilter.Interfaces;
using System.Collections;
using System.Reflection;
using System.Diagnostics;

namespace AutoFilter
{
    /// <summary>
    /// Represents a mapping between a filter property and an entity property.
    /// </summary>
    /// <typeparam name="TFilter">The filter type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TFilterProperty">The mapped filter property type.</typeparam>
    /// <typeparam name="TEntityProperty">The mapped entity property type.</typeparam>
    [DebuggerDisplay("FilterProperty = {FilterProperty}, EntityProperty = {EntityProperty}, Predicate = {Predicate}")]
    public class FilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty> : IFilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty>, 
                                                                                         IWhenNullPropertyMapConfiguration<TFilter, TEntity>
        where TFilter : class
        where TEntity : class
    {
        #region Ctor

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="filterProperty"></param>
        /// <param name="entityProperty"></param>
        /// <param name="definition"></param>
        /// <param name="predicate"></param>
        /// <param name="ignoreNullValues"></param>
        public FilterPropertyMap(Expression<Func<TFilter, TFilterProperty>> filterProperty,
                                 Expression<Func<TEntity, TEntityProperty>> entityProperty,
                                 IFilterDefinition<TFilter, TEntity> definition,
                                 Expression<Func<TFilterProperty, TEntityProperty, bool>> predicate = null, bool ignoreNullValues = true)
        {
            Ensure.ArgumentNotNull(() => filterProperty);
            Ensure.ArgumentNotNull(() => entityProperty);
            Ensure.ArgumentNotNull(() => definition);

            FilterProperty = filterProperty;
            EntityProperty = entityProperty;
            FilterDefinition = definition;
            Predicate = predicate;
            IgnoreNullValues = ignoreNullValues;
        }

        #endregion

        #region Properties

        /// <summary>
        /// filter property in map
        /// </summary>
        public Expression<Func<TFilter, TFilterProperty>> FilterProperty
        {
            get
            {
                return _filterProperty;
            }
            protected internal set
            {
                ValidatePropertyLambda(value);
                _filterProperty = value;
            }
        }
        private Expression<Func<TFilter, TFilterProperty>> _filterProperty;

        /// <summary>
        /// entity property in map
        /// </summary>
        public Expression<Func<TEntity, TEntityProperty>> EntityProperty
        {
            get
            {
                return _entityProperty;
            }
            protected internal set
            {
                ValidatePropertyLambda(value);
                _entityProperty = value;
            }
        }
        private Expression<Func<TEntity, TEntityProperty>> _entityProperty;

        /// <summary>
        /// custom predicate of map
        /// </summary>
        public Expression<Func<TFilterProperty, TEntityProperty, bool>> Predicate
        { get; protected internal set; }

        private IFilterDefinition<TFilter, TEntity> FilterDefinition { get; set; }
        internal bool IgnoreNullValues { get; set; }

        #endregion

        #region IFilterPropertyMap Implementation

        /// <summary>
        /// validates that a predicate can be built from filter
        /// </summary>
        /// <param name="filter">filter</param>
        /// <returns>True, if a predicate expression could be build from filter</returns>
        public bool CanBuildConditionExpression(TFilter filter)
        {
            if (filter == null || Predicate == null)
                return false;

            if (!IgnoreNullValues)
                return true;

            var filterPropertyValue = GetFilterValue(filter);
            return filterPropertyValue != null && !IsEmptyEnumerable(filterPropertyValue);
        }

        /// <summary>
        /// Build predicate expression from filter
        /// </summary>
        /// <param name="filter">filter</param>
        /// <returns>predicate expression</returns>
        public Expression<Func<TEntity, bool>> BuildConditionExpression(TFilter filter)
        {
            Ensure.ArgumentNotNull(() => filter);

            if (!CanBuildConditionExpression(filter))
                throw new InvalidOperationException("Cannot build a condition expression");

            var filterPropertyValue = (TFilterProperty)GetFilterValue(filter);

            Expression<Func<TEntityProperty, bool>> nullCondition = entityProperty => entityProperty == null;
            var propertyCondition = PropertyIsNull(filterPropertyValue)
                                      ? nullCondition
                                      :ReplacePradicateFilterParameterWithActualValue(Predicate, filterPropertyValue);

            return GetFullConditionFromPropertyCondition(propertyCondition);
        }

        /// <summary>
        /// Replace the predicate expression that will be used for the mapped entity property filter expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IFilterDefinition<TFilter, TEntity> UsePredicate(Expression<Func<TFilterProperty, TEntityProperty, bool>> predicate)
        {
            Predicate = predicate;

            return this;
        }

        /// <summary>
        /// Ignore the matched filter and entity properties
        /// </summary>
        public IFilterDefinition<TFilter, TEntity> Ignore()
        {
            return UsePredicate(null);
        }

        /// <summary>
        /// Defines the behavior when filter property has a null value
        /// </summary>
        /// <returns></returns>
        public IWhenNullPropertyMapConfiguration<TFilter, TEntity> WhenNull()
        {
            return this;
        }

        /// <summary>
        /// Maps custom expression to filter property
        /// </summary>
        /// <typeparam name="TFilterProperty1"></typeparam>
        /// <typeparam name="TEntityProperty1"></typeparam>
        /// <param name="filterProperty">filter property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <returns></returns>
        public IFilterPropertyMap<TFilter, TEntity, TFilterProperty1, TEntityProperty1> Map<TFilterProperty1, TEntityProperty1>(
                        Expression<Func<TFilter, TFilterProperty1>> filterProperty, 
                        Expression<Func<TEntity, TEntityProperty1>> entityProperty)
        {
            return FilterDefinition.Map(filterProperty, entityProperty);
        }

        /// <summary>
        /// Maps custom expression to filter property
        /// </summary>
        /// <typeparam name="TFilterProperty1"></typeparam>
        /// <typeparam name="TEntityProperty1"></typeparam>
        /// <param name="filterProperty">filter property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <param name="predicateComparison">predicate comparison</param>
        /// <returns></returns>
        public IFilterPropertyMap<TFilter, TEntity, TFilterProperty1, TEntityProperty1> Map<TFilterProperty1, TEntityProperty1>(
            Expression<Func<TFilter, TFilterProperty1>> filterProperty, 
            Expression<Func<TEntity, TEntityProperty1>> entityProperty, 
            ComparisonType predicateComparison)
        {
            return FilterDefinition.Map(filterProperty, entityProperty, predicateComparison);
        }

        /// <summary>
        /// Maps a range property from a filter object to a valve property in the destination type
        /// </summary>
        /// <typeparam name="TFilterProperty1"></typeparam>
        /// <typeparam name="TEntityProperty1"></typeparam>
        /// <param name="filterProperty">filter range property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <returns></returns>
        [Obsolete("Use the Map method instead")]
        public IFilterDefinition<TFilter, TEntity> MapRange<TFilterProperty1, TEntityProperty1>(Expression<Func<TFilter, TFilterProperty1>> filterProperty, Expression<Func<TEntity, TEntityProperty1>> entityProperty)
            where TFilterProperty1 : IRangeFilter<TEntityProperty1>
            where TEntityProperty1 : struct, IComparable<TEntityProperty1>
        {
            return FilterDefinition.Map(filterProperty, entityProperty);
        }

        /// <summary>
        /// Ignore a filter property and exclude it from the filtering expression
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <param name="filterProperty">The filter property to ignore</param>
        /// <returns></returns>
        public IFilterDefinition<TFilter, TEntity> Ignore<TFilterProperty>(Expression<Func<TFilter, TFilterProperty>> filterProperty)
        {
            return FilterDefinition.Ignore(filterProperty);
        }

        #endregion

        #region IWhenNullPropertyMapConfiguration Implementation

        /// <summary>
        /// Don't filter by the property when it's null (Default)
        /// </summary>
        /// <returns></returns>
        public IFilterDefinition<TFilter, TEntity> IgnoreProperty()
        {
            IgnoreNullValues = true;
            return this;
        }

        /// <summary>
        /// Filter by the property when it's null instead of ignoring this property
        /// </summary>
        public IFilterDefinition<TFilter, TEntity> FilterByNull()
        {
            IgnoreNullValues = false;
            return this;
        }

        #endregion

        #region Private Methods

        private static void ValidatePropertyLambda(LambdaExpression value)
        {
            Ensure.ArgumentNotNull(() => value);

            if (value.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("The property Lambda Expression is not valid");
        }

        private object GetFilterValue(TFilter filter)
        {
            try
            {
                return FilterProperty.Compile().DynamicInvoke(filter);          
            }
            catch (TargetInvocationException exc)
            {
                if (exc.InnerException is NullReferenceException)
                    return null;

                throw exc.InnerException;
            }
        }

        private static bool IsEmptyEnumerable(object enumerable)
        {
            return enumerable is IEnumerable && !((IEnumerable)enumerable).GetEnumerator().MoveNext();
        }
        
        private static Expression<Func<TEntityField, bool>> ReplacePradicateFilterParameterWithActualValue<TFilterField, TEntityField>
                                                                    (Expression<Func<TFilterField, TEntityField, bool>> predicateExpression,
                                                                     TFilterProperty filterFieldValue)
        {
            var filterValueExpr = Expression.Constant(filterFieldValue, typeof(TFilterField));
            var predicateWithActualValue = predicateExpression.Body.ReplaceExpression(predicateExpression.Parameters[0], filterValueExpr);

            return Expression.Lambda<Func<TEntityField, bool>>(predicateWithActualValue, predicateExpression.Parameters[1]);
        }

        private Expression<Func<TEntity, bool>> GetFullConditionFromPropertyCondition(Expression<Func<TEntityProperty, bool>> propertyCondition)
        {
            var fullCondition = propertyCondition.ReplaceExpression(propertyCondition.Parameters[0], EntityProperty.Body);
            return Expression.Lambda<Func<TEntity, bool>>(fullCondition.Body, EntityProperty.Parameters[0]);
        }

        private static bool PropertyIsNull(TFilterProperty filterPropertyValue)
        {
            return filterPropertyValue == null || IsEmptyEnumerable(filterPropertyValue);
        }

        #endregion
    }
}
