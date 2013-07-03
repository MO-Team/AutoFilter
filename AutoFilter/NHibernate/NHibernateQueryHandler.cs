using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Linq;

namespace AutoFilter.NHibernateProvider
{
    /// <summary>
    /// Provides functionality to create and execute an <see cref="IQueryable{T}"/> or an <see cref="Expression"/> via NHibernateQueryProvider/>.
    /// </summary>
    public class NHibernateQueryHandler<TBaseEntity> : BaseQueryHandler<TBaseEntity>
        where TBaseEntity : class
    {
        #region Properties

        /// <summary>
        /// NHibernate session
        /// </summary>
        protected ISession Session { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// NHibernateQueryHandler c'tor
        /// </summary>
        /// <param name="session">NHibernate Session</param>
        public NHibernateQueryHandler(ISession session)
        {
            Ensure.ArgumentNotNull(() => session);

            Session = session;
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// returns query provider for current session
        /// </summary>
        /// <returns>query provider</returns>
        protected override IQueryProvider GetQueryProvider()
        {
            return new NHibernate.Linq.DefaultQueryProvider((ISessionImplementor)Session);
        }

        /// <summary>
        /// creates new query object
        /// </summary>
        /// <typeparam name="T">query type</typeparam>
        /// <returns>session's query object</returns>
        public override IQueryable<T> CreateFilter<T>()
        {
            return Session.Query<T>();
        }
        #endregion
    }
}
