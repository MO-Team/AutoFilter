using System;
using System.Collections.Generic;
using System.Linq;
using AutoFilter.Interfaces;

namespace AutoFilter
{
    ///<summary>
    /// Allows loading filters from ICreateFilter
    ///</summary>
    /// <example><![CDATA[
    /// If we have the classes:
    ///     CreateUserFilter : ICreateFilter    >>  Create filter between UserQuery and User
    ///     CreateRoleFilter : ICreateFilter    >>  Create filter between RoleQuery and Role
    /// InitializeFilter.On(FilterConfiguration).AddFromAssemblyOf<CreateUserFilter>().Apply();
    /// This will create all filters defined by ICreateFilter, in this case: User, Role.
    /// ]]>
    /// </example>
    public class InitializeFilter : IInitializeFilter
    {
        #region Properties

        private readonly IFilterConfiguration _config;
        private readonly ICollection<Type> _filterInitializers;
        private readonly ICollection<Type> _excludedInitializers;

        #endregion

        private InitializeFilter(IFilterConfiguration config)
        {
            _config = config;
            _filterInitializers = new List<Type>();
            _excludedInitializers = new List<Type>();
        }

        ///<summary>
        /// Defines filters on a given filter configuration
        ///</summary>
        ///<param name="config">Filter configuration, usualy FilterConfiguration</param>
        ///<returns></returns>
        public static IRegisterFilter On(IFilterConfiguration config)
        {
            return new InitializeFilter(config);
        }

        ///<summary>
        /// Add ICreateFilter type
        ///</summary>
        ///<param name="type">ICreateFilter type</param>
        ///<returns></returns>
        public IInitializeFilter Add(Type type)
        {
            if (!_filterInitializers.Contains(type))
            {
                _filterInitializers.Add(type);
            }
            return this;
        }

        ///<summary>
        /// Add ICreateFilter type
        ///</summary>
        ///<typeparam name="T">ICreateFilter type</typeparam>
        ///<returns></returns>
        public IInitializeFilter Add<T>()
            where T : ICreateFilter
        {
            return Add(typeof (T));
        }

        ///<summary>
        /// Exclude ICreateFilter type
        ///</summary>
        ///<param name="type">ICreateFilter type</param>
        ///<returns></returns>
        public IInitializeFilter Exclude(Type type)
        {
            if (!_excludedInitializers.Contains(type))
            {
                _excludedInitializers.Add(type);
            }
            return this;
        }

        ///<summary>
        /// Exclude ICreateFilter type
        ///</summary>
        ///<typeparam name="T">ICreateFilter type</typeparam>
        ///<returns></returns>
        public IInitializeFilter Exclude<T>()
            where T : ICreateFilter
        {
            return Exclude(typeof (T));
        }

        ///<summary>
        /// Add all ICreateFilter types from the assembly
        ///</summary>
        ///<param name="type">ICreateFilter type</param>
        ///<returns></returns>
        public IInitializeFilter AddFromAssemblyOf(Type type)
        {
            var types = from t in type.Assembly.GetTypes()
                        where typeof (ICreateFilter).IsAssignableFrom(t) &&
                              t.IsClass && !t.IsAbstract
                        select t;

            foreach (var filterInitializer in types)
            {
                Add(filterInitializer);
            }
            return this;
        }

        ///<summary>
        /// Add all ICreateFilter types from the assembly
        ///</summary>
        ///<typeparam name="T">ICreateFilter type</typeparam>
        ///<returns></returns>
        public IInitializeFilter AddFromAssemblyOf<T>()
            where T : ICreateFilter
        {
            return AddFromAssemblyOf(typeof (T));
        }

        ///<summary>
        /// Apply all assigned filters
        ///</summary>
        public IFilterConfigurationValidator Apply()
        {
            var filterInitializers = _filterInitializers.Where(t => // Isn't excluded
                                                               !_excludedInitializers.Contains(t) &&
                                                                    // Non generic
                                                               !t.IsGenericTypeDefinition &&
                                                                    // A non-abstract class
                                                               t.IsClass && !t.IsAbstract &&
                                                                    // Has a default constructor
                                                               t.GetConstructor(Type.EmptyTypes) != null);

            var instances = new List<ICreateFilter>(
                filterInitializers.Select(Activator.CreateInstance)
                .OfType<ICreateFilter>());

            instances.ForEach(x => x.CreateFilter(_config));

            return _config;
        }
    }
}