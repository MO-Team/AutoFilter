using System;
using System.Linq.Expressions;
using AutoFilter;
using AutoFilter.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class BasePredicateProviderTests
    {
        #region Properties

        BasePredicateProvider MockedPredicateProvider { get; set; }

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void TestSetUp()
        {
            MockedPredicateProvider = MockRepository.GenerateStub<BasePredicateProvider>();
        }

        #endregion

        #region CanBuildPredicateExpression Tests

        [Test]
        public void CanBuildPredicateExpression_Call_CallAbstractOverload()
        {
            MockedPredicateProvider
                .Expect(p => p.CanBuildPredicateExpression(typeof(string), typeof(int), ComparisonType.GreaterThanOrEqual))
                .Return(true);

            var result = MockedPredicateProvider.CanBuildPredicateExpression<string, int>(ComparisonType.GreaterThanOrEqual);

            Assert.IsTrue(result);
        }

        #endregion

        #region BuildPredicateExpression Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildPredicateExpression_FirstTypeIsNull_Exception()
        {
            MockedPredicateProvider.BuildPredicateExpression(null, typeof(int), ComparisonType.GreaterThan);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildPredicateExpression_SecondTypeIsNull_Exception()
        {
            MockedPredicateProvider.BuildPredicateExpression(typeof(int), null, ComparisonType.GreaterThan);
        }

        [Test]
        public void BuildPredicateExpression_ArgsOk_CallAbstractOverload()
        {
            Expression<Func<int, object, bool>> result = (i, o) => false;
            MockedPredicateProvider
                .Expect(p => p.BuildPredicateExpression<int, object>(ComparisonType.LessThan))
                .Return(result);

            var actualResult = MockedPredicateProvider.BuildPredicateExpression(typeof(int), 
                                                                                typeof(object), 
                                                                                ComparisonType.LessThan);

            Assert.AreEqual(result, actualResult);
        }

        #endregion

        #region GetBinaryExpressionType Tests

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetBinaryExpressionType_CallWithInvalidValue_Exception()
        {
            MockedPredicateProvider.GetBinaryExpressionType((ComparisonType)999);
        }

        [Test]
        public void GetBinaryExpressionType_CallWithComparisonTypeValue_ReturnsCorrectExpressionType()
        {
            Assert.AreEqual(ExpressionType.Equal, 
                            MockedPredicateProvider.GetBinaryExpressionType(ComparisonType.Equal));

            Assert.AreEqual(ExpressionType.LessThan, 
                            MockedPredicateProvider.GetBinaryExpressionType(ComparisonType.LessThan));

            Assert.AreEqual(ExpressionType.LessThanOrEqual, 
                            MockedPredicateProvider.GetBinaryExpressionType(ComparisonType.LessThanOrEqual));

            Assert.AreEqual(ExpressionType.GreaterThan, 
                            MockedPredicateProvider.GetBinaryExpressionType(ComparisonType.GreaterThan));

            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, 
                            MockedPredicateProvider.GetBinaryExpressionType(ComparisonType.GreaterThanOrEqual));
        }

        #endregion
    }
}
