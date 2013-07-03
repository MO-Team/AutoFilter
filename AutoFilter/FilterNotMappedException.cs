using System;
using System.Runtime.Serialization;

namespace AutoFilter
{

    /// <summary>
    /// Thrown when attempting to build predicate using an unmapped filter
    /// </summary>	
    [Serializable]
    public class FilterNotMappedException : Exception
    {
        private const string ErrorMessageFormat =
            "Filter {0} is not mapped to {1} in the current ";

        ///<summary>		
        /// Initializes <see cref="FilterNotMappedException"/> class with an error messages regarding the invalid key.
        ///</summary>
        ///<param name="filterType"></param>
        ///<param name="entityType"></param>
        public FilterNotMappedException(Type filterType,Type entityType)
            : base(String.Format(ErrorMessageFormat, filterType.FullName, entityType.FullName))
        {
        }

        ///<summary>		
        /// Initializes <see cref="FilterNotMappedException"/> class with an error messages regarding the invalid key,
        /// and some more information.
        ///</summary>
        /// <param name="key">The invalid key.</param>
        /// <param name="moreInformation">More information to appear in the error message.</param>
        public FilterNotMappedException(string key, string moreInformation)
            : base(String.Format(ErrorMessageFormat, key, moreInformation))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterNotMappedException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="key">The error message string.</param>
        /// <param name="moreInformation">More information to appear in the error message.</param>
        /// <param name="inner">The inner exception reference.</param>
        public FilterNotMappedException(string key, string moreInformation, Exception inner)
            : base(String.Format(ErrorMessageFormat, key, moreInformation), inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterNotMappedException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data. </param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected FilterNotMappedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
