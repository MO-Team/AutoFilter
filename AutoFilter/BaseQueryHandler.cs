using System;
using System.Collections.Generic;
using System.Linq;
using AutoFilter.Interfaces;
using System.Linq.Expressions;

namespace AutoFilter
{
    /// <summary>
    /// Provides functionality to create and execute an <see cref="IQueryable{T}"/> or an <see cref="Expression"/> via a specified <see cref="IQueryProvider"/>.
    /// </summary>
    /// <typeparam name="TBaseEntity">The base class or interface which all result classes inherit from.</typeparam>
    public abstract class BaseQueryHandler<TBaseEntity> : IQueryHandler<TBaseEntity>
        where TBaseEntity : class
    {
        #region IQueryHandler Members

        /// <summary>
        /// Create new instance of IQueryable of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <returns>A new instance of IQueryable with empty expression.</returns>
        /// <remarks>
        /// The returned IEnumerable will not be populated with actual values. The actual values will be generated on 
        /// the first access (like any NHibernate query).
        /// </remarks>
        public abstract IQueryable<T> CreateFilter<T>()
            where T : TBaseEntity;

        /// <summary>
        /// Executes an IQueryable and return the execution results.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <param name="query">The IQueryable to execute.</param>
        /// <returns>An IEnumerable of <typeparamref name="T"/> that contains the results from execution.</returns>
        /// <remarks>
        /// The returned IEnumerable will not be populated with actual values. The actual values will be generated on 
        /// the first access (like any NHibernate query).
        /// </remarks>
        public IEnumerable<T> Execute<T>(IQueryable<T> query)
            where T : TBaseEntity
        {
            Ensure.ArgumentNotNull(() => query);

            if (query.Expression == null)
                throw new ArgumentException("The query is not valid");

            return Execute<T>(query.Expression);
        }

        /// <summary>
        /// Executes an IQueryable and return the execution results.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <param name="queryBuildAction">A function that uses the given IQueryable for Linq query or method chain.</param>
        /// <returns>An IEnumerable of <typeparamref name="T"/> that contains the results from execution.</returns>
        /// <remarks>
        /// The returned IEnumerable will not be populated with actual values. The actual values will be generated on 
        /// the first access (like any NHibernate query).
        /// </remarks>
        public IEnumerable<T> Execute<T>(Func<IQueryable<T>, IQueryable<T>> queryBuildAction)
            where T : TBaseEntity
        {
            Ensure.ArgumentNotNull(() => queryBuildAction);

            var query = CreateFilter<T>();
            query = queryBuildAction(query);

            if (query == null)
                throw new ArgumentException("The queryBuildAction returned null");

            return Execute<T>(query.Expression);
        }

        /// <summary>
        /// Executes a Where predicate expression and return the execution results.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <param name="predicate">The Where predicate expression.</param>
        /// <returns>An IEnumerable of <typeparamref name="T"/> that contains the results from execution.</returns>
        /// <remarks>
        /// The returned IEnumerable will not be populated with actual values. The actual values will be generated on 
        /// the first access (like any NHibernate query).
        /// </remarks>
        public IEnumerable<T> Execute<T>(Expression<Func<T, bool>> predicate)
            where T : TBaseEntity
        {
            var queryable = CreateFilter<T>();

            if (predicate != null)
                queryable = queryable.Where(predicate);

            return Execute<T>(queryable.Expression);
        }

        /// <summary>
        /// Executes a complete Expression Tree and return the execution results.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <param name="expression">The complete expression to execute.</param>
        /// <returns>An IEnumerable of <typeparamref name="T"/> that contains the results from execution.</returns>
        /// <remarks>
        /// The returned IEnumerable will not be populated with actual values. The actual values will be generated on 
        /// the first access (like any NHibernate query).
        /// </remarks>
        public IEnumerable<T> Execute<T>(Expression expression)
            where T : TBaseEntity
        {
            Ensure.ArgumentNotNull(() => expression);

            var provider = GetQueryProvider();
            return provider.Execute<IEnumerable<T>>(expression);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Should return the instance of <see cref="IQueryProvider"/> that will be used to execute the Linq expressions.
        /// </summary>
        /// <returns>An instance the current <see cref="IQueryProvider"/>.</returns>
        protected abstract IQueryProvider GetQueryProvider();

        #endregion
    }
}