using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFilter;
using NUnit.Framework;
using System.Reflection;
using System.Collections;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class TypeUtilsTests
    {
        #region GetItemTypeIfIEnumerable Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetItemTypeIfIEnumerable_TypeIsNull_Exception()
        {
            TypeUtils.GetItemTypeIfIEnumerable(null);
        }

        [Test]
        public void GetItemTypeIfIEnumerable_TypeIsNotEnumerable_ReturnsNull()
        {
            var result1 = TypeUtils.GetItemTypeIfIEnumerable(typeof(object));
            Assert.IsNull(result1);

            var result2 = TypeUtils.GetItemTypeIfIEnumerable(typeof(TypeUtilsTests));
            Assert.IsNull(result2);
        }

        [Test]
        public void GetItemTypeIfIEnumerable_TypeIsEnumerable_ReturnsItemType()
        {
            var result1 = TypeUtils.GetItemTypeIfIEnumerable(typeof(List<int>));
            Assert.AreEqual(typeof(int), result1);

            var result2 = TypeUtils.GetItemTypeIfIEnumerable(typeof(IEnumerable<string>));
            Assert.AreEqual(typeof(string), result2);

            var result3 = TypeUtils.GetItemTypeIfIEnumerable(typeof(object[]));
            Assert.AreEqual(typeof(object), result3);
        }

        [Test]
        public void GetItemTypeIfIEnumerable_TypeIsString_ReturnsNull()
        {
            var result = TypeUtils.GetItemTypeIfIEnumerable(typeof(string));
            Assert.IsNull(result);
        }

        #endregion

        #region GetItemTypeIfICollection Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetItemTypeIfICollection_TypeIsNull_Exception()
        {
            TypeUtils.GetItemTypeIfICollection(null);
        }

        [Test]
        public void GetItemTypeIfICollection_TypeIsNotCollection_ReturnsNull()
        {
            var result1 = TypeUtils.GetItemTypeIfICollection(typeof(object));
            Assert.IsNull(result1);

            var result2 = TypeUtils.GetItemTypeIfICollection(typeof(TypeUtilsTests));
            Assert.IsNull(result2);

            var result3 = TypeUtils.GetItemTypeIfICollection(typeof(IEnumerable<int>));
            Assert.IsNull(result3);
        }

        [Test]
        public void GetItemTypeIfICollection_TypeIsEnumerable_ReturnsItemType()
        {
            var result1 = TypeUtils.GetItemTypeIfICollection(typeof(List<int>));
            Assert.AreEqual(typeof(int), result1);

            var result2 = TypeUtils.GetItemTypeIfICollection(typeof(ICollection<string>));
            Assert.AreEqual(typeof(string), result2);

            var result3 = TypeUtils.GetItemTypeIfICollection(typeof(object[]));
            Assert.AreEqual(typeof(object), result3);
        }

        #endregion

        #region GetNotNullableType Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNotNullableType_TypeIsNull_Exception()
        {
            TypeUtils.GetNotNullableType(null);
        }

        [Test]
        public void GetNotNullableType_TypeNotInheriteFromNullable_ReturnSameType()
        {
            var result1 = TypeUtils.GetNotNullableType(typeof(object));
            Assert.AreEqual(typeof(object), result1);

            var result2 = TypeUtils.GetNotNullableType(typeof(TypeUtilsTests));
            Assert.AreEqual(typeof(TypeUtilsTests), result2);

            var result3 = TypeUtils.GetNotNullableType(typeof(int));
            Assert.AreEqual(typeof(int), result3); 
        }

        [Test]
        public void GetNotNullableType_TypeInheriteFromNullable_ReturnNotNullableType()
        {
            var result1 = TypeUtils.GetNotNullableType(typeof(Nullable<int>));
            Assert.AreEqual(typeof(int), result1);

            var result2 = TypeUtils.GetNotNullableType(typeof(Nullable<DateTime>));
            Assert.AreEqual(typeof(DateTime), result2);
        }

        #endregion

        #region GetNullableType Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNullableType_TypeIsNull_Exception()
        {
            TypeUtils.GetNullableType(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNullableType_TypeIsNotStruct_Exception()
        {
            TypeUtils.GetNullableType(typeof(TypeUtilsTests));
        }

        [Test]
        public void GetNullableType_TypeIsStruct_ReturnNullableTypeOfType()
        {
            var result1 = TypeUtils.GetNullableType(typeof(int));
            Assert.AreEqual(typeof(Nullable<int>), result1);

            var result2 = TypeUtils.GetNullableType(typeof(DateTime));
            Assert.AreEqual(typeof(Nullable<DateTime>), result2);
        }

        [Test]
        public void GetNullableType_TypeInheriteFromNullable_ReturnSameType()
        {
            var result1 = TypeUtils.GetNullableType(typeof(Nullable<int>));
            Assert.AreEqual(typeof(Nullable<int>), result1);

            var result2 = TypeUtils.GetNullableType(typeof(Nullable<DateTime>));
            Assert.AreEqual(typeof(Nullable<DateTime>), result2);
        }

        #endregion

        #region GetInheritanceDefinition Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetInheritanceDefinition_TypeIsNull_Exception()
        {
            TypeUtils.GetInheritanceDefinition(null, typeof(object));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetInheritanceDefinition_BaseTypeIsNull_Exception()
        {
            TypeUtils.GetInheritanceDefinition(typeof(object), null);
        }

        [Test]
        public void GetInheritanceDefinition_TypeAndBaseTypeAreTheSameType_ReturnsTheType()
        {
            var result1 = TypeUtils.GetInheritanceDefinition(typeof(object), typeof(object));
            Assert.AreEqual(typeof(object), result1);

            var result2 = TypeUtils.GetInheritanceDefinition(typeof(DateTime), typeof(DateTime));
            Assert.AreEqual(typeof(DateTime), result2);

            var result3 = TypeUtils.GetInheritanceDefinition(typeof(TypeUtilsTests), typeof(TypeUtilsTests));
            Assert.AreEqual(typeof(TypeUtilsTests), result3);

            var result4 = TypeUtils.GetInheritanceDefinition(typeof(IEnumerable<int>), typeof(IEnumerable<int>));
            Assert.AreEqual(typeof(IEnumerable<int>), result4);
        }

        [Test]
        public void GetInheritanceDefinition_TypeNotInheriteFromBaseType_ReturnsNull()
        {
            var result1 = TypeUtils.GetInheritanceDefinition(typeof(int), typeof(long));
            Assert.IsNull(result1);

            var result2 = TypeUtils.GetInheritanceDefinition(typeof(TypeUtilsTests), typeof(DateTime));
            Assert.IsNull(result2);

            var result3 = TypeUtils.GetInheritanceDefinition(typeof(TypeUtilsTests), typeof(IEnumerable<int>));
            Assert.IsNull(result3);

            var result4 = TypeUtils.GetInheritanceDefinition(typeof(IEnumerable<int>), typeof(List<int>));
            Assert.IsNull(result4);
        }

        [Test]
        public void GetInheritanceDefinition_TypeInheriteFromBaseType_ReturnsBaseType()
        {
            var result1 = TypeUtils.GetInheritanceDefinition(typeof(int), typeof(object));
            Assert.AreEqual(typeof(object), result1);

            var result2 = TypeUtils.GetInheritanceDefinition(typeof(DateTime), typeof(IComparable<DateTime>));
            Assert.AreEqual(typeof(IComparable<DateTime>), result2);

            var result3 = TypeUtils.GetInheritanceDefinition(typeof(EnvironmentVariableTarget), typeof(Enum));
            Assert.AreEqual(typeof(Enum), result3);

            var result4 = TypeUtils.GetInheritanceDefinition(typeof(List<int>), typeof(IEnumerable<int>));
            Assert.AreEqual(typeof(IEnumerable<int>), result4);
        }

        [Test]
        public void GetInheritanceDefinition_TypeInheriteFromBaseTypeAndBaseTypeIsGenericTypeDefinition_ReturnsFullBaseType()
        {
            var result1 = TypeUtils.GetInheritanceDefinition(typeof(DateRange), typeof(RangeFilter<>));
            Assert.AreEqual(typeof(RangeFilter<DateTime>), result1);

            var result2 = TypeUtils.GetInheritanceDefinition(typeof(DateTime), typeof(IComparable<>));
            Assert.AreEqual(typeof(IComparable<DateTime>), result2);

            var result3 = TypeUtils.GetInheritanceDefinition(typeof(List<int>), typeof(IEnumerable<>));
            Assert.AreEqual(typeof(IEnumerable<int>), result3);
        }

        #endregion

        #region IsInheritsFrom Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsInheritingFrom_TypeIsNull_Exception()
        {
            TypeUtils.IsInheritsFrom(null, typeof(object));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsInheritingFrom_BaseTypeIsNull_Exception()
        {
            TypeUtils.IsInheritsFrom(typeof(object), null);
        }

        [Test]
        public void IsInheritingFrom_TypeAndBaseTypeAreTheSameType_ReturnsTrue()
        {
            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(object), typeof(object)));

            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(DateTime), typeof(DateTime)));

            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(TypeUtilsTests), typeof(TypeUtilsTests)));

            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(IEnumerable<int>), typeof(IEnumerable<int>)));
        }

        [Test]
        public void IsInheritingFrom_TypeNotInheriteFromBaseType_ReturnsFalse()
        {
            Assert.IsFalse(TypeUtils.IsInheritsFrom(typeof(int), typeof(long)));

            Assert.IsFalse(TypeUtils.IsInheritsFrom(typeof(TypeUtilsTests), typeof(DateTime)));

            Assert.IsFalse(TypeUtils.IsInheritsFrom(typeof(TypeUtilsTests), typeof(IEnumerable<int>)));

            Assert.IsFalse(TypeUtils.IsInheritsFrom(typeof(IEnumerable<int>), typeof(List<int>)));
        }

        [Test]
        public void IsInheritingFrom_TypeInheriteFromBaseType_ReturnsTrue()
        {
            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(int), typeof(object)));

            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(DateTime), typeof(IComparable<DateTime>)));

            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(EnvironmentVariableTarget), typeof(Enum)));

            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(List<int>), typeof(IEnumerable<int>)));
        }

        [Test]
        public void IsInheritingFrom_TypeInheriteFromBaseTypeAndBaseTypeIsGenericTypeDefinition_ReturnsFullBaseType()
        {
            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(DateRange), typeof(RangeFilter<>)));

            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(DateTime), typeof(IComparable<>)));

            Assert.IsTrue(TypeUtils.IsInheritsFrom(typeof(List<int>), typeof(IEnumerable<>)));
        }

        #endregion

        #region GetMethodInfo<T> Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMethodInfoOfT_ExpressionIsNull_Exception()
        {
            TypeUtils.GetMethodInfo<int>(null);
        }

        [Test]
        public void GetMethodInfoOfT_ExpressionAccessProperty_ReturnNull()
        {
            var exprParam = Expression.Parameter(typeof(List<int>), "list");
            var expression = Expression.Lambda<Action<List<int>>>(Expression.Property(exprParam, "Count"), exprParam);

            Assert.IsNull(TypeUtils.GetMethodInfo<List<int>>(expression));
        }

        [Test]
        public void GetMethodInfoOfT_ExpressionCallMethod_DoNotExecuteTheExpression()
        {
            TypeUtils.GetMethodInfo<object>(o => Assert.Fail());
        }

        [Test]
        public void GetMethodInfoOfT_ExpressionCallMethod_ReturnsTheMethod()
        {
            var result1 = TypeUtils.GetMethodInfo<IQueryable<DateTime>>(q => q.All<DateTime>(null));
            Assert.AreEqual(typeof(Queryable).GetMethod("All").MakeGenericMethod(typeof(DateTime)),
                            result1);

            var result2 = TypeUtils.GetMethodInfo<List<string>>(l => l.Add(""));
            Assert.AreEqual(typeof(List<string>).GetMethod("Add"), result2);

            var result3 = TypeUtils.GetMethodInfo<List<int>>(l => l.BinarySearch(0, null));
            Assert.AreEqual(typeof(List<int>).GetMethod("BinarySearch", new Type[] { typeof(int), typeof(IComparer<int>) }),
                            result3);

            var result4 = TypeUtils.GetMethodInfo<TypeUtilsTests>(t => t.GetMethodInfoOfT_ExpressionCallMethod_ReturnsTheMethod());
            Assert.AreEqual(MethodBase.GetCurrentMethod(), result4);
        }

        #endregion

        #region GetMethodInfo Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMethodInfo_ExpressionIsNull_Exception()
        {
            TypeUtils.GetMethodInfo(null);
        }

        [Test]
        public void GetMethodInfo_ExpressionAccessProperty_ReturnNull()
        {
            var expression = Expression.Lambda<Action>(Expression.Property(Expression.Constant(new List<int>()), "Count"));

            Assert.IsNull(TypeUtils.GetMethodInfo(expression));
        }

        [Test]
        public void GetMethodInfo_ExpressionCallMethod_DoNotExecuteTheExpression()
        {
            TypeUtils.GetMethodInfo(() => Assert.Fail());
        }
        
        [Test]
        public void GetMethodInfo_ExpressionCallMethod_ReturnsTheMethod()
        {
            var result1 = TypeUtils.GetMethodInfo(() => Queryable.All<DateTime>(null, null));
            Assert.AreEqual(typeof(Queryable).GetMethod("All").MakeGenericMethod(typeof(DateTime)), 
                            result1);

            var result2 = TypeUtils.GetMethodInfo(() => new List<string>().Add(""));
            Assert.AreEqual(typeof(List<string>).GetMethod("Add"), result2);

            var result3 = TypeUtils.GetMethodInfo(() => new List<int>().BinarySearch(0, null));
            Assert.AreEqual(typeof(List<int>).GetMethod("BinarySearch", new Type[] { typeof(int), typeof(IComparer<int>) }), 
                            result3);

            var result4 = TypeUtils.GetMethodInfo(() => GetMethodInfo_ExpressionCallMethod_ReturnsTheMethod());
            Assert.AreEqual(MethodBase.GetCurrentMethod(), result4);
        }

        #endregion

        #region GetGenericTypeMethodInfo<T> Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetGenericTypeMethodInfo_ExpressionIsNull_Exception()
        {
            TypeUtils.GetGenericTypeMethodInfo<List<int>>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetGenericTypeMethodInfo_ArgumentTypeIsNotGeneric_Exception()
        {
            TypeUtils.GetGenericTypeMethodInfo<int>(i => i.CompareTo(5));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetGenericTypeMethodInfo_NotEnoughGenericTypeArguments_Exception()
        {
            TypeUtils.GetGenericTypeMethodInfo<Dictionary<int, string>>(d => d.Add(1, ""), typeof(DateTime));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetGenericTypeMethodInfo_TooMuchGenericTypeArguments_Exception()
        {
            TypeUtils.GetGenericTypeMethodInfo<List<int>>(l => l.Contains(5), typeof(string), typeof(bool));
        }

        [Test]
        public void GetGenericTypeMethodInfo_ExpressionAccessProperty_ReturnsNull()
        {
            var exprParam = Expression.Parameter(typeof(List<int>), "list");
            var expression = Expression.Lambda<Action<List<int>>>(Expression.Property(exprParam, "Count"), exprParam);

            Assert.IsNull(TypeUtils.GetGenericTypeMethodInfo<List<int>>(expression, typeof(string)));
        }

        [Test]
        public void GetGenericTypeMethodInfo_ArgumentTypeIsGenericAndFitsGenericArguments_DoNotExecuteTheExpression()
        {
            TypeUtils.GetGenericTypeMethodInfo<List<int>>(l => Assert.Fail(), typeof(string));
        }

        [Test]
        public void GetGenericTypeMethodInfo_ArgumentTypeIsGenericAndFitsGenericArguments_ReturnsMethodOfNewGenericType()
        {
            var result1 = TypeUtils.GetGenericTypeMethodInfo<List<int>>(l => l.Contains(5), typeof(string));
            Assert.AreEqual(typeof(List<string>).GetMethod("Contains"), result1);

            var result2 = TypeUtils.GetGenericTypeMethodInfo<Dictionary<int, string>>(d => d.Add(1, ""), typeof(DateTime), typeof(DateTime));
            Assert.AreEqual(typeof(Dictionary<DateTime, DateTime>).GetMethod("Add"), result2);

            var result3 = TypeUtils.GetGenericTypeMethodInfo<IComparable<object>>(c => c.CompareTo(null), typeof(DateTime));
            Assert.AreEqual(typeof(IComparable<DateTime>).GetMethod("CompareTo"), result3);
        }

        #endregion

        #region GetMemberInfo<T, TMember> Tests

        private int _testField = 0;
        private string TestProperty
        {
            get
            {
                Assert.Fail();
                return null;
            }
            set
            {
                Assert.Fail();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMemberInfo_ExpressionIsNull_Exception()
        {
            TypeUtils.GetMemberInfo<object, int>(null);
        }

        [Test]
        public void GetMemberInfo_ExpressionIsOk_DoNotExecuteTheExpression()
        {
            TypeUtils.GetMemberInfo((object o) => this.TestProperty);
        }

        [Test]
        public void GetMemberInfo_ExpressionIsMethodCall_ReturnsNull()
        {
            Assert.IsNull(TypeUtils.GetMemberInfo<int, int>(i => i.CompareTo(1)));
        }

        [Test]
        public void GetMemberInfo_ExpressionIsConvertOfMethodCall_ReturnsNull()
        {
            Assert.IsNull(TypeUtils.GetMemberInfo<int, object>(i => (object)i.CompareTo(1)));
        }

        [Test]
        public void GetMemberInfo_ExpressionIsPropertyAccess_ReturnsTheProperty()
        {
            var result1 = TypeUtils.GetMemberInfo((List<int> l) => l.Count);
            Assert.AreEqual(typeof(List<int>).GetProperty("Count"), result1);

            var result2 = TypeUtils.GetMemberInfo((IDictionary<int, string> d) => d.Keys);
            Assert.AreEqual(typeof(IDictionary<int, string>).GetProperty("Keys"), result2);

            var result3 = TypeUtils.GetMemberInfo((TypeUtilsTests t) => t.TestProperty);
            Assert.AreEqual(typeof(TypeUtilsTests).GetProperty("TestProperty", BindingFlags.Instance | BindingFlags.NonPublic), 
                            result3);
        }

        [Test]
        public void GetMemberInfo_ExpressionIsConvertOfPropertyAccess_ReturnsTheProperty()
        {
            var result1 = TypeUtils.GetMemberInfo((List<int> l) => (object)l.Count);
            Assert.AreEqual(typeof(List<int>).GetProperty("Count"), result1);

            var result2 = TypeUtils.GetMemberInfo((IDictionary<int, string> d) => (IEnumerable)d.Keys);
            Assert.AreEqual(typeof(IDictionary<int, string>).GetProperty("Keys"), result2);

            var result3 = TypeUtils.GetMemberInfo((TypeUtilsTests t) => (ICloneable)t.TestProperty);
            Assert.AreEqual(typeof(TypeUtilsTests).GetProperty("TestProperty", BindingFlags.Instance | BindingFlags.NonPublic), 
                            result3);
        }

        [Test]
        public void GetMemberInfo_ExpressionReturnsPropertyNotFromItsArg_ReturnsTheProperty()
        {
            var result1 = TypeUtils.GetMemberInfo((object o) => new List<int>().Count);
            Assert.AreEqual(typeof(List<int>).GetProperty("Count"), result1);

            var result2 = TypeUtils.GetMemberInfo((string s) => this.TestProperty);
            Assert.AreEqual(typeof(TypeUtilsTests).GetProperty("TestProperty", BindingFlags.Instance | BindingFlags.NonPublic), 
                            result2);
        }

        [Test]
        public void GetMemberInfo_ExpressionIsFieldAccess_ReturnsTheField()
        {
            var result = TypeUtils.GetMemberInfo((TypeUtilsTests t) => t._testField);
            Assert.AreEqual(typeof(TypeUtilsTests).GetField("_testField", BindingFlags.Instance | BindingFlags.NonPublic), 
                            result);
        }

        [Test]
        public void GetMemberInfo_ExpressionIsConvertOfFieldAccess_ReturnsTheField()
        {
            var result = TypeUtils.GetMemberInfo((TypeUtilsTests t) => (IFormattable)t._testField);
            Assert.AreEqual(typeof(TypeUtilsTests).GetField("_testField", BindingFlags.Instance | BindingFlags.NonPublic), 
                            result);
        }

        [Test]
        public void GetMemberInfo_ExpressionReturnsFieldNotFromItsArg_ReturnsTheField()
        {
            var result = TypeUtils.GetMemberInfo((string s) => this._testField);
            Assert.AreEqual(typeof(TypeUtilsTests).GetField("_testField", BindingFlags.Instance | BindingFlags.NonPublic), 
                            result);
        }

        #endregion

        #region GetGenericTypeMemberInfo Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetGenericTypeMemberInfo_ExpressionIsNull_Exception()
        {
            TypeUtils.GetGenericTypeMemberInfo<List<int>, int>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetGenericTypeMemberInfo_ArgumentTypeIsNotGeneric_Exception()
        {
            TypeUtils.GetGenericTypeMemberInfo<TypeUtilsTests, string>(t => t.TestProperty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetGenericTypeMemberInfo_NotEnoughGenericTypeArguments_Exception()
        {
            TypeUtils.GetGenericTypeMemberInfo((Dictionary<int, string> d) => d.Keys, typeof(DateTime));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetGenericTypeMemberInfo_TooMuchGenericTypeArguments_Exception()
        {
            TypeUtils.GetGenericTypeMemberInfo((List<int> l) => l.Count, typeof(string), typeof(bool));
        }

        [Test]
        public void GetGenericTypeMemberInfo_ExpressionIsMethodCall_ReturnsNull()
        {
            Assert.IsNull(TypeUtils.GetGenericTypeMemberInfo((List<int> l) => l.Contains(0), typeof(string)));
        }

        [Test]
        public void GetGenericTypeMemberInfo_ArgumentTypeIsGenericAndFitsGenericArguments_ReturnsMemberOfNewGenericType()
        {
            var result1 = TypeUtils.GetGenericTypeMemberInfo((List<int> l) => l.Count, typeof(string));
            Assert.AreEqual(typeof(List<string>).GetProperty("Count"), result1);

            var result2 = TypeUtils.GetGenericTypeMemberInfo((Dictionary<int, string> d) => d.Keys, typeof(DateTime), typeof(TypeUtilsTests));
            Assert.AreEqual(typeof(Dictionary<DateTime, TypeUtilsTests>).GetProperty("Keys"), result2);

            var result3 = TypeUtils.GetGenericTypeMemberInfo((IEnumerator<object> e) => e.Current, typeof(DateTime));
            Assert.AreEqual(typeof(IEnumerator<DateTime>).GetProperty("Current"), result3);
        }

        #endregion
    }
}
