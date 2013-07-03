using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFilter.Interfaces;
using System.Reflection;

namespace AutoFilter
{
    /// <summary>
    /// Provides functionality to create an expression that can logically compare two different types.
    /// </summary>
    public class LinqPredicateProvider : BasePredicateProvider
    {
        #region Public Methods
        /// <summary>
        /// Determine if a predicate expression can be created for the specified arguments types.
        /// </summary>
        /// <param name="filterFieldType">First predicate argument type.</param>
        /// <param name="entityFieldType">Second predicate argument type.</param>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>True if  predicate expression can be created, otherwise false.</returns>
        public override bool CanBuildPredicateExpression(Type filterFieldType, Type entityFieldType, ComparisonType predicateComparison)
        {
            var filterEnumerableItemType = filterFieldType.GetItemTypeIfICollection();
            var entityEnumerableItemType = entityFieldType.GetItemTypeIfIEnumerable();
            
            if (filterEnumerableItemType != null && predicateComparison != ComparisonType.Equal)
                return false;

            var rangeFilterType = filterFieldType.GetInheritanceDefinition(typeof(IRangeFilter<>));
            if (rangeFilterType != null)
                filterFieldType =  rangeFilterType.GetGenericArguments()[0];

            return CanCompareDirectly(filterEnumerableItemType ?? filterFieldType,
                                      entityEnumerableItemType ?? entityFieldType,
                                      predicateComparison);
        }

        /// <summary>
        /// Creates a Lambda Expression that will return true if the two arguments are logically equal or false otherwise.
        /// </summary>
        /// <typeparam name="TFilterField">First predicate argument type.</typeparam>
        /// <typeparam name="TEntityField">Second predicate argument type.</typeparam>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>The predicate function that compare the two types.</returns>
        public override Expression<Func<TFilterField, TEntityField, bool>> BuildPredicateExpression<TFilterField, TEntityField>(ComparisonType predicateComparison)
        {
            if (!CanBuildPredicateExpression(typeof(TFilterField), typeof(TEntityField), predicateComparison))
                throw new InvalidOperationException("Can not build predicate expression");

            var filterEnumerableItemType = typeof(TFilterField).GetItemTypeIfICollection();
            var entityEnumerableItemType = typeof(TEntityField).GetItemTypeIfIEnumerable();
            var rangeFilterType = typeof(TFilterField).GetInheritanceDefinition(typeof(IRangeFilter<>));

            LambdaExpression predicateExpression;

            if (rangeFilterType != null)
            {
                predicateExpression = BuildRangePredicateExpression<TFilterField, TEntityField>(rangeFilterType);
            }
            else if (filterEnumerableItemType == null)
            {
                predicateExpression = BuildPredicateExpressionForSingleValueTypes(typeof(TFilterField),
                                                                                  entityEnumerableItemType ?? typeof(TEntityField),
                                                                                  predicateComparison);
            }
            else
            {
                predicateExpression = CreateContainsMethodCallExpression(typeof(TFilterField), 
                                                                         filterEnumerableItemType, 
                                                                         entityEnumerableItemType ?? typeof(TEntityField));
            }

            if (entityEnumerableItemType != null)
            {
                predicateExpression = CreateAnyMethodCallExpression(predicateExpression, predicateExpression.Parameters[1], typeof(TEntityField));
            }

            return predicateExpression as Expression<Func<TFilterField, TEntityField, bool>>;
        }

       

        #endregion

        #region Protected Methods

        /// <summary>
        /// Builds predicate expression for a single property in filter
        /// </summary>
        /// <param name="filterFieldType">filter field type</param>
        /// <param name="entityFieldType">entity field type</param>
        /// <param name="predicateComparison">comparison enumeration</param>
        /// <returns>predicate expression</returns>
        protected virtual LambdaExpression BuildPredicateExpressionForSingleValueTypes(Type filterFieldType, Type entityFieldType, ComparisonType predicateComparison)
        {
            LambdaExpression predicate = null;

            var isFilterFieldNullable = filterFieldType.IsInheritsFrom(typeof(Nullable<>));
            var isEntityFieldNullable = entityFieldType.IsInheritsFrom(typeof(Nullable<>));

            if (isFilterFieldNullable || isEntityFieldNullable)
            {
                filterFieldType = filterFieldType.GetNullableType();
                entityFieldType = entityFieldType.GetNullableType();
            }

            if (CanCompareDirectly(filterFieldType, entityFieldType, predicateComparison))
            {
                predicate = CreateDirectCompareExpression(filterFieldType, entityFieldType, predicateComparison);

                if (!isFilterFieldNullable)
                    predicate = ChangeParameterTypeToNotNullable(predicate, predicate.Parameters[0]);
                if (!isEntityFieldNullable)
                    predicate = ChangeParameterTypeToNotNullable(predicate, predicate.Parameters[1]);
            }

            return predicate;
        }

        #endregion

        #region Private Methods

        private bool CanCompareDirectly(Type t1, Type t2, ComparisonType predicateComparison)
        {
            t1 = t1.GetNotNullableType();
            t2 = t2.GetNotNullableType();

            if (t1 != t2)
                return false;

            if (predicateComparison != ComparisonType.Equal && (t1 == typeof(bool) || 
                                                                t1 == typeof(string) || 
                                                                t1.IsEnum))
                return false;

        
            return (t1.IsPrimitive ||
                    t1.IsEnum ||
                    t1 == typeof(string) ||
                    t1 == typeof(DateTime) ||
                    t1 == typeof(decimal));
        }

        private LambdaExpression CreateDirectCompareExpression(Type param1Type, Type param2Type, ComparisonType predicateComparison)
        {
            var param1 = Expression.Parameter(param1Type,null);
            var param2 = Expression.Parameter(param2Type,null);
            var binaryType = GetBinaryExpressionType(predicateComparison);

            return Expression.Lambda(Expression.MakeBinary(binaryType, param1, param2), 
                                     param1, param2);
        }

        private LambdaExpression ChangeParameterTypeToNotNullable(LambdaExpression predicate, ParameterExpression nullableParameter)
        {
            var notNullableParam = Expression.Parameter(nullableParameter.Type.GetNotNullableType(), nullableParameter.Name);
            var convertToNullableExpr = Expression.Convert(notNullableParam, nullableParameter.Type);

            return Expression.Lambda(predicate.Body.ReplaceExpression(nullableParameter, convertToNullableExpr),
                                     predicate.Parameters.ReplaceExpressionInCollection(nullableParameter, notNullableParam).ToArray());
        }

        private LambdaExpression CreateContainsMethodCallExpression(Type filterEnumerableType, Type filterSingleItemType, Type entityParameterType)
        {
            var containsMethod = TypeUtils.GetGenericTypeMethodInfo((ICollection<object> c) => c.Contains(null), 
                                                                    filterSingleItemType);

            var filterParameter = Expression.Parameter(filterEnumerableType,null);
            var entityParameter = Expression.Parameter(entityParameterType,null);
            var callContains = Expression.Call(filterParameter, containsMethod, entityParameter);
            return Expression.Lambda(callContains, filterParameter, entityParameter);
        }

        private LambdaExpression CreateAnyMethodCallExpression(LambdaExpression singleItemPredicate, ParameterExpression predicateSingleItemParameter, Type enumerableType)
        {
            var anyMethod = TypeUtils.GetMethodInfo((IQueryable<object> q) => q.Any(x => true))
                                        .GetGenericMethodDefinition()
                                        .MakeGenericMethod(predicateSingleItemParameter.Type);
            var asQueryableMethod = TypeUtils.GetMethodInfo((IEnumerable<object> q) => q.AsQueryable())
                                        .GetGenericMethodDefinition()
                                        .MakeGenericMethod(predicateSingleItemParameter.Type);

            var lambdaParameter = Expression.Parameter(enumerableType,null);
            var callAsQueryable = Expression.Call(null, asQueryableMethod, lambdaParameter);
            var anyMethodParameter = Expression.Quote(Expression.Lambda(singleItemPredicate.Body, predicateSingleItemParameter));
            var callAny = Expression.Call(null, anyMethod, callAsQueryable, anyMethodParameter);
            var newParameters = singleItemPredicate.Parameters.ReplaceExpressionInCollection(predicateSingleItemParameter, lambdaParameter);
            return Expression.Lambda(callAny, newParameters.ToArray());
        }

        private LambdaExpression BuildRangePredicateExpression<TFilterProperty, TEntityProperty>(Type rangeFilterType)
        {
            var rangeFilterGenericArgs = rangeFilterType.GetGenericArguments();
            var filterPropertyParameter = Expression.Parameter(typeof(TFilterProperty));
            var entityParameter = Expression.Parameter(typeof(TEntityProperty));

            // (!filterDate.Exact.HasValue || filterDate.Exact == valueDate))
            var exactValueProperty = TypeUtils.GetGenericTypeMemberInfo((IRangeFilter<int> n) => n.ExactValue, rangeFilterGenericArgs) as PropertyInfo;
            var exactExpression = CreateNullableComparisionPredicate(filterPropertyParameter, entityParameter, exactValueProperty, rangeFilterGenericArgs, ComparisonType.Equal);

            // (!filterDate.MaxValue.HasValue || filterDate.MaxValue >= valueDate) 
            var maxValueProperty = TypeUtils.GetGenericTypeMemberInfo((IRangeFilter<int> n) => n.MaxValue, rangeFilterGenericArgs) as PropertyInfo;
            var maxExpression = CreateNullableComparisionPredicate(filterPropertyParameter, entityParameter, maxValueProperty, rangeFilterGenericArgs, ComparisonType.GreaterThanOrEqual);

            // (!filterDate.MinValue.HasValue || filterDate.MinValue < valueDate))
            var minValueProperty = TypeUtils.GetGenericTypeMemberInfo((IRangeFilter<int> n) => n.MinValue, rangeFilterGenericArgs) as PropertyInfo;
            var minExpression = CreateNullableComparisionPredicate(filterPropertyParameter, entityParameter, minValueProperty, rangeFilterGenericArgs, ComparisonType.LessThanOrEqual);

            var combinedExpression = Expression.AndAlso(exactExpression, Expression.AndAlso(maxExpression, minExpression));
            return Expression.Lambda(combinedExpression, filterPropertyParameter, entityParameter) as Expression<Func<TFilterProperty, TEntityProperty, bool>>;
        }

        private Expression CreateNullableComparisionPredicate(ParameterExpression rangeFilterParameter, ParameterExpression entityPropertyParameter, PropertyInfo subPropertyInfo, Type[] rangeFilterGenericArgs, ComparisonType compartionType)
        {
            var filterSubProperty = Expression.Property(rangeFilterParameter, subPropertyInfo);
            var hasValueProperty = TypeUtils.GetGenericTypeMemberInfo((Nullable<int> n) => n.HasValue, rangeFilterGenericArgs) as PropertyInfo;
            var isNullExpression = Expression.Not(Expression.Property(filterSubProperty, hasValueProperty));

            var predicate = BuildPredicateExpression(subPropertyInfo.PropertyType, entityPropertyParameter.Type, compartionType);
            predicate = predicate.ReplaceExpression(predicate.Parameters[0], filterSubProperty);
            predicate = predicate.ReplaceExpression(predicate.Parameters[1], entityPropertyParameter);

            // (!HasValue) || {predicate}
            return Expression.OrElse(isNullExpression, predicate.Body);
        }

        #endregion
        
    }
}
