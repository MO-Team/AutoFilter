using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace AutoFilter
{
    /// <summary>
    /// provides different reflection helper methods
    /// </summary>
    public static class TypeUtils
    {
        internal static Type GetItemTypeIfIEnumerable(this Type type)
        {
            Ensure.ArgumentNotNull(() => type);

            if (type == typeof(string))  //because String is enumerable
                return null;

            var iEnumerableDefinition = GetInheritanceDefinition(type, typeof(IEnumerable<>));
            if (iEnumerableDefinition == null)
                return null;

            return iEnumerableDefinition.GetGenericArguments()[0];
        }

        internal static Type GetItemTypeIfICollection(this Type type)
        {
            Ensure.ArgumentNotNull(() => type);

            var iEnumerableDefinition = GetInheritanceDefinition(type, typeof(ICollection<>));
            if (iEnumerableDefinition == null)
                return null;

            return iEnumerableDefinition.GetGenericArguments()[0];
        }

        internal static Type GetNotNullableType(this Type type)
        {
            Ensure.ArgumentNotNull(() => type);

            var nullableDefinition = GetInheritanceDefinition(type, typeof(Nullable<>));
            if (nullableDefinition == null)
                return type;

            return nullableDefinition.GetGenericArguments()[0];
        }

        internal static Type GetNullableType(this Type type)
        {
            Ensure.ArgumentNotNull(() => type);

            if (type.IsInheritsFrom(typeof(Nullable<>)))
                return type;

            return typeof(Nullable<>).MakeGenericType(type);
        }

        internal static Type GetInheritanceDefinition(this Type type, Type baseType)
        {
            Ensure.ArgumentNotNull(() => type);
            Ensure.ArgumentNotNull(() => baseType);

            Func<Type, bool> predicate =
                t => t == baseType || (t.IsGenericType && baseType.IsGenericTypeDefinition &&
                                       t.GetGenericTypeDefinition() == baseType);

            if (predicate(type))
                return type;

            if (baseType.IsInterface)
            {
                return type.GetInterfaces().FirstOrDefault(predicate);
            }
            if (type.BaseType != null)
            {
                return GetInheritanceDefinition(type.BaseType, baseType);
            }

            return null;
        }

        /// <summary>
        /// Check if type Inherits from a given base type
        /// </summary>
        /// <param name="type">type to check</param>
        /// <param name="baseType">base type</param>
        /// <returns></returns>
        public static bool IsInheritsFrom(this Type type, Type baseType)
        {
            Ensure.ArgumentNotNull(() => type);
            Ensure.ArgumentNotNull(() => baseType);

            return GetInheritanceDefinition(type, baseType) != null;
        }

        /// <summary>
        /// return method info for given method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodInLambda">method call expression</param>
        /// <returns>method information</returns>
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> methodInLambda)
        {
            Ensure.ArgumentNotNull(() => methodInLambda);

            var methodCall = methodInLambda.Body as MethodCallExpression;
            if (methodCall != null)
            {
                return methodCall.Method;
            }

            return null;
        }

        /// <summary>
        /// return method info for given method
        /// </summary>
        /// <param name="methodInLambda">method call expression</param>
        /// <returns>method information</returns>
        public static MethodInfo GetMethodInfo(Expression<Action> methodInLambda)
        {
            Ensure.ArgumentNotNull(() => methodInLambda);

            var lambdaWithParameter = Expression.Lambda<Action<object>>(methodInLambda.Body, Expression.Parameter(typeof(object),null));
            return GetMethodInfo(lambdaWithParameter);
        }

        /// <summary>
        /// return method info for given generic method 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodInLambda">method call expression</param>
        /// <param name="genericTypeArguments">generic arguments for method</param>
        /// <returns>method info for method with generic arguments</returns>
        public static MethodInfo GetGenericTypeMethodInfo<T>(Expression<Action<T>> methodInLambda, params Type[] genericTypeArguments)
        {
            Ensure.ArgumentNotNull(() => methodInLambda);
            Ensure.ArgumentNotNull(() => genericTypeArguments);

            if (!typeof(T).IsGenericType)
                throw new ArgumentException(@"The Lambda Expression parameter type is not a generic type", "methodInLambda");

            var method = GetMethodInfo(methodInLambda);
            if (method == null)
                return null;

            var genericTypeDefinition = methodInLambda.Parameters[0].Type.GetGenericTypeDefinition();
            return genericTypeDefinition.MakeGenericType(genericTypeArguments).GetMethod(method.Name);
        }

        /// <summary>
        /// return member info for given member 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMember">member type</typeparam>
        /// <param name="memberInLambda">member call expression</param>
        /// <returns>member info for member</returns>
        public static MemberInfo GetMemberInfo<T, TMember>(Expression<Func<T, TMember>> memberInLambda)
        {
            Ensure.ArgumentNotNull(() => memberInLambda);

            switch (memberInLambda.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)memberInLambda.Body).Member;

                case ExpressionType.Convert:
                    var memberExpression = ((UnaryExpression)memberInLambda.Body).Operand as MemberExpression;
                    if (memberExpression != null)
                        return memberExpression.Member;
                    break;
            }

            return null;
        }

        /// <summary>
        /// return member info for given generic member 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="memberInLambda">member call expression</param>
        /// <param name="genericTypeArguments">generic arguments for method</param>
        /// <returns>member info for member with generic arguments</returns>
        public static MemberInfo GetGenericTypeMemberInfo<T, TReturn>(Expression<Func<T, TReturn>> memberInLambda, params Type[] genericTypeArguments)
        {
            Ensure.ArgumentNotNull(() => memberInLambda);
            Ensure.ArgumentNotNull(() => genericTypeArguments);

            if (!memberInLambda.Parameters[0].Type.IsGenericType)
                throw new ArgumentException(@"The Lambda Expression parameter type is not a generic type", "memberInLambda");

            var member = GetMemberInfo(memberInLambda);
            if (member == null)
                return null;

            var genericTypeDefinition = memberInLambda.Parameters[0].Type.GetGenericTypeDefinition();
            return genericTypeDefinition.MakeGenericType(genericTypeArguments).GetMember(member.Name).FirstOrDefault();
        }
    }
}
