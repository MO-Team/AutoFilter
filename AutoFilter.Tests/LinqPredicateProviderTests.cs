using System;
using System.Collections.Generic;
using AutoFilter;
using AutoFilter.Interfaces;
using NUnit.Framework;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class LinqPredicateProviderTests
    {
        #region Properties

        LinqPredicateProvider LinqPredicateProvider { get; set; }

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void TestSetUp()
        {
            LinqPredicateProvider = new LinqPredicateProvider();
        }

        #endregion

        #region CanBuildPredicateExpression Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanBuildPredicateExpression_FilterFieldTypeIsNull_Exception()
        {
            LinqPredicateProvider.CanBuildPredicateExpression(null, typeof(string), ComparisonType.Equal);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanBuildPredicateExpression_EntityFieldTypeIsNull_Exception()
        {
            LinqPredicateProvider.CanBuildPredicateExpression(typeof(int), null, ComparisonType.Equal);
        }

        [Test]
        public void CanBuildPredicateExpression_WithSamePrimitiveTypesAndAnyComparisonType_ReturnsTrue()
        {
            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(bool), 
                                                                            typeof(bool), 
                                                                            ComparisonType.Equal));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(long),
                                                                            typeof(long),
                                                                            ComparisonType.GreaterThan));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(uint),
                                                                            typeof(uint),
                                                                            ComparisonType.GreaterThanOrEqual));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(int),
                                                                            typeof(int),
                                                                            ComparisonType.LessThan));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(ulong),
                                                                            typeof(ulong),
                                                                            ComparisonType.LessThanOrEqual));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreDecimalAndAnyComparisonType_ReturnsTrue()
        {
            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(decimal),
                                                                            typeof(decimal),
                                                                            ComparisonType.Equal));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(decimal),
                                                                            typeof(decimal),
                                                                            ComparisonType.GreaterThan));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(decimal),
                                                                            typeof(decimal),
                                                                            ComparisonType.GreaterThanOrEqual));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(decimal),
                                                                            typeof(decimal),
                                                                            ComparisonType.LessThan));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(decimal),
                                                                            typeof(decimal),
                                                                            ComparisonType.LessThanOrEqual));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreDateTimeAndAnyComparisonType_ReturnsTrue()
        {
            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(DateTime),
                                                                            typeof(DateTime),
                                                                            ComparisonType.Equal));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(DateTime),
                                                                            typeof(DateTime),
                                                                            ComparisonType.GreaterThan));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(DateTime),
                                                                            typeof(DateTime),
                                                                            ComparisonType.GreaterThanOrEqual));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(DateTime),
                                                                            typeof(DateTime),
                                                                            ComparisonType.LessThan));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(DateTime),
                                                                            typeof(DateTime),
                                                                            ComparisonType.LessThanOrEqual));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreStringAndComparisonTypeIsEqual_ReturnsTrue()
        {
            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(string),
                                                                            typeof(string),
                                                                            ComparisonType.Equal));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreEnumAndComparisonTypeIsEqual_ReturnsTrue()
        {
            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(ComparisonType),
                                                                            typeof(ComparisonType),
                                                                            ComparisonType.Equal));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreBooleanAndComparisonTypeIsNotEqual_ReturnsFalse()
        {
            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(bool),
                                                                             typeof(bool),
                                                                             ComparisonType.GreaterThan));

            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(bool),
                                                                             typeof(bool),
                                                                             ComparisonType.LessThanOrEqual));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreStringAndComparisonTypeIsNotEqual_ReturnsFalse()
        {
            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(string),
                                                                             typeof(string),
                                                                             ComparisonType.GreaterThanOrEqual));

            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(string),
                                                                             typeof(string),
                                                                             ComparisonType.LessThan));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreEnumAndComparisonTypeIsNotEqual_ReturnsFalse()
        {
            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(ComparisonType),
                                                                             typeof(ComparisonType),
                                                                             ComparisonType.GreaterThanOrEqual));

            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(ComparisonType),
                                                                             typeof(ComparisonType),
                                                                             ComparisonType.LessThanOrEqual));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreNullableOfTheSameType_ReturnsTrue()
        {
            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(Nullable<bool>),
                                                                            typeof(bool),
                                                                            ComparisonType.Equal));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(int),
                                                                            typeof(Nullable<int>),
                                                                            ComparisonType.LessThanOrEqual));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(Nullable<DateTime>),
                                                                            typeof(Nullable<DateTime>),
                                                                            ComparisonType.GreaterThan));
        }

        [Test]
        public void CanBuildPredicateExpression_TypesAreICollectionOfTheSameType_ReturnsTrue()
        {
            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(ICollection<bool>),
                                                                            typeof(bool),
                                                                            ComparisonType.Equal));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(int),
                                                                            typeof(ICollection<int>),
                                                                            ComparisonType.LessThanOrEqual));

            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(ICollection<long>),
                                                                            typeof(ICollection<long>),
                                                                            ComparisonType.Equal));
        }

        [Test]
        public void CanBuildPredicateExpression_FilterFieldIsICollectionAndComparisonTypeIsNotEqual_ReturnsFalse()
        {
            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(ICollection<bool>),
                                                                             typeof(bool),
                                                                             ComparisonType.GreaterThan));

            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(ICollection<long>),
                                                                             typeof(ICollection<long>),
                                                                             ComparisonType.LessThanOrEqual));
        }

        [Test]
        public void CanBuildPredicateExpression_FilterFieldIsRangFilterWithSameEntityType_ReturnsTrue()
        {
            Assert.IsTrue(LinqPredicateProvider.CanBuildPredicateExpression(typeof(IRangeFilter<DateTime>),
                                                                             typeof(DateTime),
                                                                             ComparisonType.Equal));
        }

        [Test]
        public void CanBuildPredicateExpression_FilterFieldIsRangFilterWithDifferentEntityType_ReturnsTrue()
        {
            Assert.IsFalse(LinqPredicateProvider.CanBuildPredicateExpression(typeof(IRangeFilter<DateTime>),
                                                                             typeof(string),
                                                                             ComparisonType.Equal));
        }

        #endregion

        #region BuildPredicateExpression Tests

        [Test]
        public void BuildPredicateExpression_WithSamePrimitiveTypesAndAnyComparisonType_ReturnsPredicate()
        {
            var result1 = LinqPredicateProvider.BuildPredicateExpression<bool, bool>(ComparisonType.Equal);
            Assert.NotNull(result1);
            Assert.AreEqual(true, result1.Compile().DynamicInvoke(false, false));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(true, false));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(false, true));

            var result2 = LinqPredicateProvider.BuildPredicateExpression<long, long>(ComparisonType.GreaterThan);
            Assert.NotNull(result2);
            Assert.AreEqual(true, result2.Compile().DynamicInvoke(1234, 123));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(123, 123));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(123, 1234));

            var result3 = LinqPredicateProvider.BuildPredicateExpression<int, int>(ComparisonType.GreaterThanOrEqual);
            Assert.NotNull(result3);
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(1234, 123));
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(123, 123));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(123, 1234));

            var result4 = LinqPredicateProvider.BuildPredicateExpression<uint, uint>(ComparisonType.LessThan);
            Assert.NotNull(result4);
            Assert.AreEqual(false, result4.Compile().DynamicInvoke((uint)1234, (uint)123));
            Assert.AreEqual(false, result4.Compile().DynamicInvoke((uint)123, (uint)123));
            Assert.AreEqual(true, result4.Compile().DynamicInvoke((uint)123, (uint)1234));

            var result5 = LinqPredicateProvider.BuildPredicateExpression<ulong, ulong>(ComparisonType.LessThanOrEqual);
            Assert.NotNull(result5);
            Assert.AreEqual(false, result5.Compile().DynamicInvoke((ulong)1234, (ulong)123));
            Assert.AreEqual(true, result5.Compile().DynamicInvoke((ulong)123, (ulong)123));
            Assert.AreEqual(true, result5.Compile().DynamicInvoke((ulong)123, (ulong)1234));
        }

        [Test]
        public void BuildPredicateExpression_TypesAreDecimalAndAnyComparisonType_ReturnsPredicate()
        {
            var result1 = LinqPredicateProvider.BuildPredicateExpression<decimal, decimal>(ComparisonType.Equal);
            Assert.NotNull(result1);
            Assert.AreEqual(true, result1.Compile().DynamicInvoke(123m, 123m));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(123m, 1234m));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(1234m, 123m));

            var result2 = LinqPredicateProvider.BuildPredicateExpression<decimal, decimal>(ComparisonType.GreaterThan);
            Assert.NotNull(result2);
            Assert.AreEqual(true, result2.Compile().DynamicInvoke(1234m, 123m));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(123m, 123m));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(123m, 1234m));

            var result3 = LinqPredicateProvider.BuildPredicateExpression<decimal, decimal>(ComparisonType.GreaterThanOrEqual);
            Assert.NotNull(result3);
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(1234m, 123m));
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(123m, 123m));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(123m, 1234m));

            var result4 = LinqPredicateProvider.BuildPredicateExpression<decimal, decimal>(ComparisonType.LessThan);
            Assert.NotNull(result4);
            Assert.AreEqual(false, result4.Compile().DynamicInvoke(1234m, 123m));
            Assert.AreEqual(false, result4.Compile().DynamicInvoke(123m, 123m));
            Assert.AreEqual(true, result4.Compile().DynamicInvoke(123m, 1234m));

            var result5 = LinqPredicateProvider.BuildPredicateExpression<decimal, decimal>(ComparisonType.LessThanOrEqual);
            Assert.NotNull(result5);
            Assert.AreEqual(false, result5.Compile().DynamicInvoke(1234m, 123m));
            Assert.AreEqual(true, result5.Compile().DynamicInvoke(123m, 123m));
            Assert.AreEqual(true, result5.Compile().DynamicInvoke(123m, 1234m));
        }

        [Test]
        public void BuildPredicateExpression_TypesAreDateTimeAndAnyComparisonType_ReturnsPredicate()
        {
            var result1 = LinqPredicateProvider.BuildPredicateExpression<DateTime, DateTime>(ComparisonType.Equal);
            Assert.NotNull(result1);
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(DateTime.Today.AddDays(1), DateTime.Today));
            Assert.AreEqual(true, result1.Compile().DynamicInvoke(DateTime.Today, DateTime.Today));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(DateTime.Today, DateTime.Today.AddDays(1)));

            var result2 = LinqPredicateProvider.BuildPredicateExpression<DateTime, DateTime>(ComparisonType.GreaterThan);
            Assert.NotNull(result2);
            Assert.AreEqual(true, result2.Compile().DynamicInvoke(DateTime.Today.AddDays(1), DateTime.Today));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(DateTime.Today, DateTime.Today));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(DateTime.Today, DateTime.Today.AddDays(1)));

            var result3 = LinqPredicateProvider.BuildPredicateExpression<DateTime, DateTime>(ComparisonType.GreaterThanOrEqual);
            Assert.NotNull(result3);
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(DateTime.Today.AddDays(1), DateTime.Today));
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(DateTime.Today, DateTime.Today));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(DateTime.Today, DateTime.Today.AddDays(1)));

            var result4 = LinqPredicateProvider.BuildPredicateExpression<DateTime, DateTime>(ComparisonType.LessThan);
            Assert.NotNull(result4);
            Assert.AreEqual(false, result4.Compile().DynamicInvoke(DateTime.Today.AddDays(1), DateTime.Today));
            Assert.AreEqual(false, result4.Compile().DynamicInvoke(DateTime.Today, DateTime.Today));
            Assert.AreEqual(true, result4.Compile().DynamicInvoke(DateTime.Today, DateTime.Today.AddDays(1)));

            var result5 = LinqPredicateProvider.BuildPredicateExpression<DateTime, DateTime>(ComparisonType.LessThanOrEqual);
            Assert.NotNull(result5);
            Assert.AreEqual(false, result5.Compile().DynamicInvoke(DateTime.Today.AddDays(1), DateTime.Today));
            Assert.AreEqual(true, result5.Compile().DynamicInvoke(DateTime.Today, DateTime.Today));
            Assert.AreEqual(true, result5.Compile().DynamicInvoke(DateTime.Today, DateTime.Today.AddDays(1)));
        }

        [Test]
        public void BuildPredicateExpression_TypesAreStringAndComparisonTypeIsEqual_ReturnsPredicate()
        {
            var result = LinqPredicateProvider.BuildPredicateExpression<string, string>(ComparisonType.Equal);
            Assert.NotNull(result);
            Assert.AreEqual(true, result.Compile().DynamicInvoke("Doron", "Doron"));
            Assert.AreEqual(false, result.Compile().DynamicInvoke("doron", "doron5"));
            Assert.AreEqual(false, result.Compile().DynamicInvoke("doron", "DORON"));
        }

        [Test]
        public void BuildPredicateExpression_TypesAreEnumAndComparisonTypeIsEqual_ReturnsPredicate()
        {
            var result = LinqPredicateProvider.BuildPredicateExpression<ComparisonType, ComparisonType>(ComparisonType.Equal);
            Assert.NotNull(result);
            Assert.AreEqual(true, result.Compile().DynamicInvoke(ComparisonType.Equal, ComparisonType.Equal));
            Assert.AreEqual(true, result.Compile().DynamicInvoke(ComparisonType.GreaterThanOrEqual, ComparisonType.GreaterThanOrEqual));
            Assert.AreEqual(false, result.Compile().DynamicInvoke(ComparisonType.Equal, ComparisonType.GreaterThan));
            Assert.AreEqual(false, result.Compile().DynamicInvoke(ComparisonType.LessThan, ComparisonType.LessThanOrEqual));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildPredicateExpression_TypesAreBooleanAndComparisonTypeIsNotEqual_Exception()
        {
            LinqPredicateProvider.BuildPredicateExpression<bool, bool>(ComparisonType.GreaterThan);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildPredicateExpression_TypesAreStringAndComparisonTypeIsNotEqual_Exception()
        {
            LinqPredicateProvider.BuildPredicateExpression<string, string>(ComparisonType.GreaterThanOrEqual);

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildPredicateExpression_TypesAreEnumAndComparisonTypeIsNotEqual_Exception()
        {
            LinqPredicateProvider.BuildPredicateExpression<ComparisonType, ComparisonType>(ComparisonType.LessThanOrEqual);
        }

        [Test]
        public void BuildPredicateExpression_TypesAreNullableOfTheSameType_ReturnsPredicate()
        {
            var result1 = LinqPredicateProvider.BuildPredicateExpression<Nullable<bool>, bool>(ComparisonType.Equal);
            Assert.NotNull(result1);
            Assert.AreEqual(true, result1.Compile().DynamicInvoke(false, false));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(true, false));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(false, true));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(null, false));

            var result2 = LinqPredicateProvider.BuildPredicateExpression<long, Nullable<long>>(ComparisonType.GreaterThan);
            Assert.NotNull(result2);
            Assert.AreEqual(true, result2.Compile().DynamicInvoke((long)1234, (long)123));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke((long)123, (long)123));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke((long)123, (long)1234));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke((long)123, null));

            var result3 = LinqPredicateProvider.BuildPredicateExpression<Nullable<int>, Nullable<int>>(ComparisonType.Equal);
            Assert.NotNull(result3);
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(null, 123));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(123, null));
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(null, null));
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(123, 123));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(1234, 123));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(123, 1234));
        }

        [Test]
        public void BuildPredicateExpression_TypesAreICollectionOfTheSameType_ReturnsPredicate()
        {
            var result1 = LinqPredicateProvider.BuildPredicateExpression<ICollection<string>, string>(ComparisonType.Equal);
            Assert.NotNull(result1);
            Assert.AreEqual(true, result1.Compile().DynamicInvoke(new string[] { "Doron" }, "Doron"));
            Assert.AreEqual(true, result1.Compile().DynamicInvoke(new string[] { "Doron1", "Doron2", "Doron33" }, "Doron33"));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(new string[] { }, "Doron"));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(new string[] { "Doron1" }, "Doron"));
            Assert.AreEqual(false, result1.Compile().DynamicInvoke(new string[] { "Doron1", "Doron2", "Doron33" }, "Doron"));

            var result2 = LinqPredicateProvider.BuildPredicateExpression<int, ICollection<int>>(ComparisonType.LessThan);
            Assert.NotNull(result2);
            Assert.AreEqual(true, result2.Compile().DynamicInvoke(0, new int[] { 1 }));
            Assert.AreEqual(true, result2.Compile().DynamicInvoke(0, new int[] { -1, 3, 0 }));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(0, new int[] { }));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(0, new int[] { 0 }));
            Assert.AreEqual(false, result2.Compile().DynamicInvoke(0, new int[] { -1, -5, 0 }));

            var result3 = LinqPredicateProvider.BuildPredicateExpression<ICollection<string>, ICollection<string>>(ComparisonType.Equal);
            Assert.NotNull(result3);
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(new string[] { "Doron" }, new string[] { "Doron" }));
            Assert.AreEqual(true, result3.Compile().DynamicInvoke(new string[] { "Doron1", "Doron2", "Doron33" }, new string[] { "Doron11", "Doron22", "Doron33" }));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(new string[] { }, new string[] { }));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(new string[] { "Doron1", }, new string[] { "Yaacobi" }));
            Assert.AreEqual(false, result3.Compile().DynamicInvoke(new string[] { "Doron1", "Doron2", "Doron33" }, new string[] { "Doron123", "Doron124", "Doron125" }));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildPredicateExpression_FilterFieldIsICollectionAndComparisonTypeIsNotEqual_Exception()
        {
            LinqPredicateProvider.BuildPredicateExpression<ICollection<bool>, bool>(ComparisonType.GreaterThan);
        }

        #endregion
    }
}
