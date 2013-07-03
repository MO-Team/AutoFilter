using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Provides functionality to create and execute an <see cref="IQueryable{T}"/> or an <see cref="Expression"/>.
    /// </summary>
    /// <typeparam name="TBaseEntity">The base class or interface which all result classes inherit from.</typeparam>
    public interface IQueryHandler<in TBaseEntity>
        where TBaseEntity : class
    {
        /// <summary>
        /// Create new instance of IQueryable of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <returns>A new instance of IQueryable.</returns>
        IQueryable<T> CreateFilter<T>()
            where T : TBaseEntity;

        /// <summary>
        /// Executes an IQueryable and return the execution results.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <param name="query">The IQueryable to execute.</param>
        /// <returns>An IEnumerable of <typeparamref name="T"/> that contains the results from execution.</returns>
        IEnumerable<T> Execute<T>(IQueryable<T> query)
            where T : TBaseEntity;

        /// <summary>
        /// Executes an IQueryable and return the execution results.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <param name="queryBuildAction">A function that uses the given IQueryable for Linq query or method chain.</param>
        /// <returns>An IEnumerable of <typeparamref name="T"/> that contains the results from execution.</returns>
        IEnumerable<T> Execute<T>(Func<IQueryable<T>, IQueryable<T>> queryBuildAction)
            where T : TBaseEntity;

        /// <summary>
        /// Executes a Where predicate expression and return the execution results.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <param name="predicate">The Where predicate expression.</param>
        /// <returns>An IEnumerable of <typeparamref name="T"/> that contains the results from execution.</returns>
        IEnumerable<T> Execute<T>(Expression<Func<T, bool>> predicate)
            where T : TBaseEntity;

        /// <summary>
        /// Executes a complete Expression Tree and return the execution results.
        /// </summary>
        /// <typeparam name="T">The type of the entity to query.</typeparam>
        /// <param name="expression">The complete expression to execute.</param>
        /// <returns>An IEnumerable of <typeparamref name="T"/> that contains the results from execution.</returns>
        IEnumerable<T> Execute<T>(Expression expression)
            where T : TBaseEntity;
    }
}
