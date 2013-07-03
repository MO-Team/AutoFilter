using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoFilter.Interfaces;
using System.Diagnostics;

namespace AutoFilter
{
    /// <summary>
    /// auto maps Expression from filter and entity 
    /// </summary>
    /// <typeparam name="TFilter">Filter Type</typeparam>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public class FilterExpressionBuilder<TFilter, TEntity> : IFilterExpressionBuilder<TFilter, TEntity>, 
                                                             IFilterDefinition<TFilter, TEntity>, 
                                                             IFilterConfigurationValidator
        where TFilter : class
        where TEntity : class
    {
        #region Properties

        /// <summary>
        /// returns current PredicateProvider FilterExpressionBuilder
        /// </summary>
        protected internal IPredicateProvider PredicateProvider
        {
            [DebuggerStepThrough]
            get
            {
                return _predicateProvider ?? (_predicateProvider = new LinqPredicateProvider());
            }
            [DebuggerStepThrough]
            set
            {
                Ensure.ArgumentNotNull(() => value);

                _predicateProvider = value;
            }
        }
        private IPredicateProvider _predicateProvider;

        /// <summary>
        /// returns current PropertyMapFactory for FilterExpressionBuilder
        /// </summary>
        protected internal IFilterPropertyMapFactory<TFilter, TEntity> PropertyMapFactory
        {
            [DebuggerStepThrough]
            get
            {
                return _propertyMapFactory ?? (_propertyMapFactory = new FilterPropertyMapFactory<TFilter, TEntity>(this));
            }
            [DebuggerStepThrough]
            set
            {
                Ensure.ArgumentNotNull(() => value);

                _propertyMapFactory = value;
            }
        }
        private IFilterPropertyMapFactory<TFilter, TEntity> _propertyMapFactory;

        /// <summary>
        /// return current PropertyMappings for FilterExpressionBuilder
        /// </summary>
        protected internal IDictionary<string, IFilterConditionExpressionBuilder<TFilter, TEntity>> PropertyMappings
        {
            [DebuggerStepThrough]
            get
            {
                if (_propertyMappings == null)
                {
                    _propertyMappings = new Dictionary<string, IFilterConditionExpressionBuilder<TFilter, TEntity>>();

                    MapPropertiesWithSameName();
                }
                return _propertyMappings;
            }
            [DebuggerStepThrough]
            set
            {
                Ensure.ArgumentNotNull(() => value);

                _propertyMappings = value;
            }
        }
        private IDictionary<string, IFilterConditionExpressionBuilder<TFilter, TEntity>> _propertyMappings;

        #endregion

        #region IFilterExpressionBuilder Implementation

        /// <summary>
        /// Builds query expression from filter
        /// </summary>
        /// <param name="filter">filter to linq expression from</param>
        /// <returns>mapped expression</returns>
        public Expression<Func<TEntity, bool>> BuildExpression(TFilter filter)
        {
            Ensure.ArgumentNotNull(() => filter);


            Expression<Func<TEntity, bool>> conditionExpression = null;

            foreach (var propertyMap in PropertyMappings.Values)
            {
                if (propertyMap == null || !propertyMap.CanBuildConditionExpression(filter))
                    continue;

                var propertyCondition = propertyMap.BuildConditionExpression(filter);
                if (propertyCondition != null)
                {
                    conditionExpression = AddExpressionToAndAlsoExpression(conditionExpression, propertyCondition);
                }
            }

            if (conditionExpression == null)
            {
               conditionExpression = HandleEmptyExpression();
            }

            return conditionExpression;
        }

        /// <summary>
        /// Maps custom expression to filter property
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <typeparam name="TEntityProperty"></typeparam>
        /// <param name="filterProperty">filter property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <returns></returns>
        public IFilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty> Map<TFilterProperty, TEntityProperty>(
            Expression<Func<TFilter, TFilterProperty>> filterProperty,
            Expression<Func<TEntity, TEntityProperty>> entityProperty)
        {
            return Map(filterProperty, entityProperty, ComparisonType.Equal);
        }

        /// <summary>
        /// Maps custom expression to filter property
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <typeparam name="TEntityProperty"></typeparam>
        /// <param name="filterProperty">filter property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <param name="predicateComparison">predicate comparison</param>
        /// <returns></returns>
        public virtual IFilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty> Map<TFilterProperty, TEntityProperty>(
            Expression<Func<TFilter, TFilterProperty>> filterProperty,
            Expression<Func<TEntity, TEntityProperty>> entityProperty,
            ComparisonType predicateComparison)
        {
            Ensure.ArgumentNotNull(() => filterProperty);
            Ensure.ArgumentNotNull(() => entityProperty);

            Expression<Func<TFilterProperty, TEntityProperty, bool>> predicate = null;
            if (PredicateProvider.CanBuildPredicateExpression<TFilterProperty, TEntityProperty>(predicateComparison))
            {
                predicate = PredicateProvider.BuildPredicateExpression<TFilterProperty, TEntityProperty>(predicateComparison);
            }

            var propertyMap = PropertyMapFactory.CreateFilterPropertyMap(filterProperty, entityProperty, predicate);
            var mapKey = GetPropertyMapKey(filterProperty);
            PropertyMappings[mapKey] = propertyMap;
            
            return propertyMap;
        }

        /// <summary>
        /// Maps a range property from a filter object to a valve property in the destination type
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <typeparam name="TEntityProperty"></typeparam>
        /// <param name="filterProperty">filter range property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <returns></returns>
        [Obsolete("Use the Map method instead")]
        public IFilterDefinition<TFilter, TEntity> MapRange<TFilterProperty, TEntityProperty>(
                        Expression<Func<TFilter, TFilterProperty>> filterProperty,
                        Expression<Func<TEntity, TEntityProperty>> entityProperty)
            where TFilterProperty : IRangeFilter<TEntityProperty>
            where TEntityProperty : struct, IComparable<TEntityProperty>
        {
            Ensure.ArgumentNotNull(() => filterProperty);
            Ensure.ArgumentNotNull(() => entityProperty);

            return Map(filterProperty, entityProperty);
        }

        /// <summary>
        /// Ignore a filter property and exclude it from the filtering expression
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <param name="filterProperty">The filter property to ignore</param>
        /// <returns></returns>
        public IFilterDefinition<TFilter, TEntity> Ignore<TFilterProperty>(Expression<Func<TFilter, TFilterProperty>> filterProperty)
        {
            Ensure.ArgumentNotNull(() => filterProperty);

            var mapKey = GetPropertyMapKey(filterProperty);
            PropertyMappings[mapKey] = null;

            return this;
        }

        /// <summary>
        ///  Handling logic for Empty Expression, Override to write custom logic
        /// </summary>
        protected internal virtual Expression<Func<TEntity, bool>> HandleEmptyExpression()
        {
           return (x => true);
        }

        #endregion

        #region IFilterConfigurationValidator Implementation

        /// <summary>
        /// Validate that all properties in all filters were mapped or ignored
        /// </summary>
        public void AssertConfigurationIsValid()
        {
            var unmappedProperties = new List<string>();

            foreach (var property in typeof(TFilter).GetProperties())
            {
                if (property.GetIndexParameters().Any())
                    continue;

                if (!IsPropertyMapped(property))
                {
                    unmappedProperties.Add(property.Name);
                }
            }

            if (unmappedProperties.Any())
                throw new FilterPropertyMissingMappingException(typeof(TFilter), unmappedProperties);
        }

        #endregion

        #region Private Methods

        private IFilterConditionExpressionBuilder<TFilter, TEntity> CreateFilterPropertyMap(
            LambdaExpression filterPropertyExpr,
            LambdaExpression entityPropertyExpr,
            LambdaExpression predicate)
        {
            var filterPropertyType = filterPropertyExpr.Type.GetMethod("Invoke").ReturnType;
            var entityPropertyType = entityPropertyExpr.Type.GetMethod("Invoke").ReturnType;

            var createMapMethod = TypeUtils.GetMethodInfo(() => PropertyMapFactory.CreateFilterPropertyMap<object, object>(null, null, null))
                                        .GetGenericMethodDefinition()
                                        .MakeGenericMethod(filterPropertyType, entityPropertyType);

            return createMapMethod.Invoke(PropertyMapFactory, new object[]
                                                                  {
                                                                      filterPropertyExpr,
                                                                      entityPropertyExpr, 
                                                                      predicate
                                                                  })
                        as IFilterConditionExpressionBuilder<TFilter, TEntity>;
        }

        private void MapPropertiesWithSameName()
        {
            foreach (var filterProperty in typeof(TFilter).GetProperties())
            {
                if (filterProperty.GetIndexParameters().Length > 0)
                    continue;

                var entityProperty = typeof(TEntity).GetProperty(filterProperty.Name);
                if (entityProperty == null)
                    continue;

                var filterPropertyLambda = CreateLambdaThatAccessProperty(typeof(TFilter), filterProperty);
                var entityPropertyLambda = CreateLambdaThatAccessProperty(typeof(TEntity), entityProperty);

                AddOrReplaceMapping(filterPropertyLambda, entityPropertyLambda, ComparisonType.Equal);
            }
        }

        private void AddOrReplaceMapping(LambdaExpression filterPropertyLambda, LambdaExpression entityPropertyLambda, ComparisonType predicateComparison)
        {
            var predicate = CreateComparisionPredicat(filterPropertyLambda, entityPropertyLambda, predicateComparison);

            AssignMapping(filterPropertyLambda, entityPropertyLambda, predicate);
        }

        private void AssignMapping(LambdaExpression filterPropertyLambda, LambdaExpression entityPropertyLambda, LambdaExpression predicate)
        {
                    var propertyMap = CreateFilterPropertyMap(filterPropertyLambda, entityPropertyLambda, predicate);
                    var propertyKey = GetPropertyMapKey(filterPropertyLambda);
                    PropertyMappings[propertyKey] = propertyMap;
        }

        private LambdaExpression CreateComparisionPredicat(LambdaExpression filterPropertyLambda, LambdaExpression entityPropertyLambda, ComparisonType predicateComparison)
        {
            LambdaExpression predicate = null;
            if (PredicateProvider.CanBuildPredicateExpression(filterPropertyLambda.Type.GetMethod("Invoke").ReturnType,
                                                              entityPropertyLambda.Type.GetMethod("Invoke").ReturnType,
                                                              predicateComparison))
            {
                predicate = PredicateProvider.BuildPredicateExpression(filterPropertyLambda.Type.GetMethod("Invoke").ReturnType,
                                                                       entityPropertyLambda.Type.GetMethod("Invoke").ReturnType,
                                                                       predicateComparison);
            }
            return predicate;
        }

        private string GetPropertyMapKey(LambdaExpression filterPropertyLambda)
        {
            var lambdaParameter = filterPropertyLambda.Parameters[0];
            var paramWithConstName = Expression.Parameter(lambdaParameter.Type, "x");
            filterPropertyLambda = filterPropertyLambda.ReplaceExpression(lambdaParameter, paramWithConstName);

            return filterPropertyLambda.ToString();
        }

        private LambdaExpression CreateLambdaThatAccessProperty(Type declaringType, PropertyInfo property)
        {
            var lambdaParameter = Expression.Parameter(declaringType, declaringType.Name);
            return Expression.Lambda(Expression.Property(lambdaParameter, property), lambdaParameter);
        }
        
        private LambdaExpression CreateLambdaThatAccessSubProperty(LambdaExpression propertyAccessLambda, PropertyInfo property)
        {
            var lambdaParameter = propertyAccessLambda.Parameters[0];
            return Expression.Lambda(Expression.Property(propertyAccessLambda.Body, property), lambdaParameter);
        }

        private Expression<Func<T, bool>> AddExpressionToAndAlsoExpression<T>(Expression<Func<T, bool>> andAlsoExpression,
                                                                              Expression<Func<T, bool>> conditionToAdd)
        {
            if (andAlsoExpression == null)
            {
                return conditionToAdd;
            }

            var andAlsoParameter = andAlsoExpression.Parameters[0];
            var newPredicate = conditionToAdd.Body.ReplaceExpression(conditionToAdd.Parameters[0], andAlsoParameter);

            return andAlsoExpression.Update(Expression.AndAlso(andAlsoExpression.Body, newPredicate),
                                            new[] { andAlsoParameter });
        }

        private bool IsPropertyMapped(PropertyInfo property)
        {

            var propertyLambda = CreateLambdaThatAccessProperty(typeof(TFilter), property);
            if (PropertyMappings.ContainsKey(GetPropertyMapKey(propertyLambda)))
                return true;

            var checkSubProperties = (!property.PropertyType.IsPrimitive && !property.PropertyType.IsEnum && !property.PropertyType.IsInheritsFrom(typeof(IEnumerable<>)));
            if (checkSubProperties)
            {
                foreach (var subProperty in property.PropertyType.GetProperties())
                {
                    var subPropertyLambda = CreateLambdaThatAccessSubProperty(propertyLambda, subProperty);
                    if (PropertyMappings.ContainsKey(GetPropertyMapKey(subPropertyLambda)))
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}
