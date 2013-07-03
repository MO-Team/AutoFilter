using System;

namespace AutoFilter.Interfaces
{
    ///<summary>
    /// InitializeFilter interface for registering filters.
    ///</summary>
    public interface IRegisterFilter
    {
        ///<summary>
        /// Add ICreateFilter type
        ///</summary>
        ///<param name="type">ICreateFilter type</param>
        ///<returns></returns>
        IInitializeFilter Add(Type type);

        ///<summary>
        /// Add ICreateFilter type
        ///</summary>
        ///<typeparam name="T">ICreateFilter type</typeparam>
        ///<returns></returns>
        IInitializeFilter Add<T>() where T : ICreateFilter;

        ///<summary>
        /// Exclude ICreateFilter type
        ///</summary>
        ///<param name="type">ICreateFilter type</param>
        ///<returns></returns>
        IInitializeFilter Exclude(Type type);

        ///<summary>
        /// Exclude ICreateFilter type
        ///</summary>
        ///<typeparam name="T">ICreateFilter type</typeparam>
        ///<returns></returns>
        IInitializeFilter Exclude<T>() where T : ICreateFilter;

        ///<summary>
        /// Add all ICreateFilter types from the assembly
        ///</summary>
        ///<param name="type">ICreateFilter type</param>
        ///<returns></returns>
        IInitializeFilter AddFromAssemblyOf(Type type);

        ///<summary>
        /// Add all ICreateFilter types from the assembly
        ///</summary>
        ///<typeparam name="T">ICreateFilter type</typeparam>
        ///<returns></returns>
        IInitializeFilter AddFromAssemblyOf<T>() where T : ICreateFilter;
    }
}