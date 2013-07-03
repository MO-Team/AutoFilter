using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoFilter;
using AutoFilter.Interfaces;
using NUnit.Framework;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class FilterPropertyMapTests
    {
        private readonly IFilterDefinition<TestCatalogFilter, ProductMetadata> _definition = new FilterExpressionBuilder<TestCatalogFilter, ProductMetadata>();

        #region Ctor Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullFilterProp_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, DateRange, DateTime?>(null, sp => sp.DataInsertionDate, _definition);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullEntityProp_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, DateRange, DateTime?>(f => f.DataInsertionDate, null, _definition);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullFilterDefinition_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, DateRange, DateTime?>(f => f.DataInsertionDate, sp => sp.DataInsertionDate,null);
        }

        [Test]
        public void Ctor_NullPredicatAndWithoutFilterByNull_CreateFilterPropertyMapWithoutPredicat()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);
            
            Assert.That(map.Predicate, Is.Null);
            Assert.That(map.IgnoreNullValues, Is.True);
        }

        [Test]
        public void Ctor_WithPredicat_CreateFilterPropertyMapWithoutPredicat()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition,
                                                                                               (s1, s2) => s2.Contains(s1),false);
            Assert.That(map.Predicate, Is.Not.Null);
            Assert.That(map.IgnoreNullValues, Is.False);
        }

        [Test]
        public void Ctor_WithPredicatAndFilterByNull_CreateFilterPropertyMapWithoutPredicat()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition,
                                                                                               (s1, s2) => s2.Contains(s1), false);
            Assert.That(map.Predicate, Is.Not.Null);
            Assert.That(map.IgnoreNullValues, Is.False);
        }

        #endregion

        #region FilterProperty Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetFilterProperty_WithNull_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, DateRange, DateTime?>(f => f.DataInsertionDate, sp => sp.DataInsertionDate, _definition);

            map.FilterProperty = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFilterProperty_WithLambdaThatNotAccessMember_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);

            map.FilterProperty = sp => sp.ToString();
        }

        [Test]
        public void SetFilterProperty_LambdaIsValid_SetNewValue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);
            Expression<Func<TestCatalogFilter, string>> newLambda = f => f.ProductName;

            map.FilterProperty = newLambda;

            Assert.AreEqual(newLambda, map.FilterProperty);
        }

        #endregion

        #region EntityProperty Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetEntityProperty_WithNull_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, DateRange, DateTime?>(f => f.DataInsertionDate, sp => sp.DataInsertionDate, _definition);

            map.EntityProperty = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetEntityProperty_WithLambdaThatNotAccessMember_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);

            map.EntityProperty = sp => sp.ToString();
        }

        [Test]
        public void SetEntityProperty_LambdaIsValid_SetNewValue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);
            Expression<Func<ProductMetadata, string>> newLambda = f => f.ProductName;

            map.EntityProperty = newLambda;

            Assert.AreEqual(newLambda, map.EntityProperty);
        }

        #endregion

        #region CanBuildConditionExpression Tests

        [Test]
        public void CanBuildConditionExpression_FilterIsNull_ReturnsFalse()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition,
                                                                                           (fName, spName) => fName == spName);

            Assert.IsFalse(map.CanBuildConditionExpression(null));
        }

        [Test]
        public void CanBuildConditionExpression_PredicateIsNull_ReturnsFalse()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);
            map.Predicate = null;

            Assert.IsFalse(map.CanBuildConditionExpression(new TestCatalogFilter()));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsNull_ReturnsFalse()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName,_definition,
                                                                                           (fName, spName) => fName == spName);
            var filter = new TestCatalogFilter() { ProductName = null };

            Assert.IsFalse(map.CanBuildConditionExpression(filter));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsNullFilterByNull_ReturnsTrue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition,
                                                                                           (fName, spName) => fName == spName, false);
            var filter = new TestCatalogFilter() { ProductName = null };

            Assert.IsTrue(map.CanBuildConditionExpression(filter));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsNullCollection_ReturnsFalse()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName,_definition,
                                                                                                        (fName, spName) => fName.Contains(spName));
            var filter = new TestCatalogFilter() { EditionName = null };

            Assert.IsFalse(map.CanBuildConditionExpression(filter));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsNullCollectionFilterByNull_ReturnsTrue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName, _definition,
                                                                                                        (fName, spName) => fName.Contains(spName),false);
            var filter = new TestCatalogFilter() { EditionName = null };

            Assert.IsTrue(map.CanBuildConditionExpression(filter));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsEmpltyCollection_ReturnsFalse()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName,_definition,
                                                                                                        (fName, spName) => fName.Contains(spName));
            var filter = new TestCatalogFilter() { EditionName = new List<string>() };

            Assert.IsFalse(map.CanBuildConditionExpression(filter));
        }
        [Test]
        public void CanBuildConditionExpression_PropertyValueIsEmptyCollectionFilterByNull_ReturnsTrue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName, _definition,
                                                                                                        (fName, spName) => fName.Contains(spName),false);
            var filter = new TestCatalogFilter() { EditionName = new List<string>() };

            Assert.IsTrue(map.CanBuildConditionExpression(filter));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsNotNull_ReturnsTrue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName,_definition,
                                                                                           (fName, spName) => fName == spName);
            var filter = new TestCatalogFilter() { ProductName = "Doron" };

            Assert.IsTrue(map.CanBuildConditionExpression(filter));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsNotNullFilterByNull_ReturnsTrue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition,
                                                                                           (fName, spName) => fName == spName, false);
            var filter = new TestCatalogFilter() { ProductName = "Doron" };

            Assert.IsTrue(map.CanBuildConditionExpression(filter));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsCollectionWithOneValue_ReturnsTrue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName,_definition,
                                                                                                        (fName, spName) => fName.Contains(spName));
            var filter = new TestCatalogFilter() { EditionName = new List<string>() { "Doron" } };

            Assert.IsTrue(map.CanBuildConditionExpression(filter));
        }

        [Test]
        public void CanBuildConditionExpression_PropertyValueIsCollectionWithValues_ReturnsTrue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName,_definition,
                                                                                                        (fName, spName) => fName.Contains(spName));
            var filter = new TestCatalogFilter() { EditionName = new List<string>() { "Doron", "Boo" } };

            Assert.IsTrue(map.CanBuildConditionExpression(filter));
        }

        #endregion

        #region BuildConditionExpression Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildConditionExpression_FilterIsNull_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName,_definition,
                                                                                           (fName, spName) => fName == spName);

            map.BuildConditionExpression(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildConditionExpression_PredicateIsNull_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);
            map.Predicate = null;

            map.BuildConditionExpression(new TestCatalogFilter());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildConditionExpression_PropertyValueIsNull_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName,_definition,
                                                                                           (fName, spName) => fName == spName);
            var filter = new TestCatalogFilter() { ProductName = null };

            map.BuildConditionExpression(filter);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildConditionExpression_PropertyValueIsNullCollection_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName,_definition,
                                                                                                        (fName, spName) => fName.Contains(spName));
            var filter = new TestCatalogFilter() { EditionName = null };

            map.BuildConditionExpression(filter);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildConditionExpression_PropertyValueIsEmpltyCollection_Exception()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName,_definition,
                                                                                                        (fName, spName) => fName.Contains(spName));
            var filter = new TestCatalogFilter() { EditionName = new List<string>() };

            map.BuildConditionExpression(filter);
        }

        [Test]
        public void BuildConditionExpression_PropertyValueIsNullFilterNull_ReturnsConditionExpression()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName,_definition,
                                                                                           (fName, spName) => fName == spName,false);
            var filter = new TestCatalogFilter();

            var conditionExpr = map.BuildConditionExpression(filter);

            Assert.NotNull(conditionExpr);
            Assert.AreEqual(true, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = null }));
            Assert.AreEqual(false, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = "Boo" }));
        }

        [Test]
        public void BuildConditionExpression_PropertyValueIsNullCollectionFilterNull_ReturnsConditionExpression()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName, _definition,
                                                                                                        (fName, spName) => fName.Contains(spName),false);
            var filter = new TestCatalogFilter() { EditionName = null };

            var conditionExpr = map.BuildConditionExpression(filter);

            Assert.AreEqual(true, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = null }));
            Assert.AreEqual(false, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = "Boo" }));
        }

        [Test]
        public void BuildConditionExpression_PropertyValueIsEmptyCollectionFilterNull_ReturnsAlwaysFalse()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName, _definition,
                                                                                                        (fName, spName) => fName.Contains(spName), false);
            var filter = new TestCatalogFilter() {  EditionName = new List<string>() };

            var conditionExpr = map.BuildConditionExpression(filter);

            Assert.AreEqual(true, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = null }));
            Assert.AreEqual(false, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = "Boo" }));
        }

        [Test]
        public void BuildConditionExpression_PropertyValueIsCollectionWithOneValue_ReturnsConditionExpression()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName,_definition,
                                                                                                        (fName, spName) => fName.Contains(spName));
            var filter = new TestCatalogFilter() { EditionName = new List<string>() { "Doron" } };

            var conditionExpr = map.BuildConditionExpression(filter);

            Assert.NotNull(conditionExpr);
            Assert.AreEqual(true, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = "Doron" }));
            Assert.AreEqual(false, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = "Boo" }));
        }

        [Test]
        public void BuildConditionExpression_PropertyValueIsCollectionWithValues_ReturnsConditionExpression()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, ICollection<string>, string>(f => f.EditionName, sp => sp.ProductName,_definition,
                                                                                                        (fName, spName) => fName.Contains(spName));
            var filter = new TestCatalogFilter() { EditionName = new List<string>() { "Doron", "Boo" } };

            var conditionExpr = map.BuildConditionExpression(filter);

            Assert.NotNull(conditionExpr);
            Assert.AreEqual(true, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = "Doron" }));
            Assert.AreEqual(true, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = "Boo" }));
            Assert.AreEqual(false, conditionExpr.Compile().DynamicInvoke(new ProductMetadata() { ProductName = "MyProductMetadata" }));
        }

        #endregion

        #region UsePredicate Tests

        [Test]
        public void UsePredicate_PredicateIsNull_SetNull()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);

            map.UsePredicate((Expression<Func<string,string,bool>>)null);

            Assert.IsNull(map.Predicate);
        }

        [Test]
        public void UsePredicate_PredicateIsNotNull_SetNewValue()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);
            Expression<Func<string, string, bool>> newPredicate = (fName, spName) => fName == spName;

            map.UsePredicate(newPredicate);

            Assert.AreEqual(newPredicate, map.Predicate);
        }

        #endregion

        #region Ignore Tests

        [Test]
        public void Ignore_Call_SetPredicateToNull()
        {
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition);
            map.Predicate = (s1, s2) => s1.Contains(s2);

            map.Ignore();

            Assert.IsNull(map.Predicate);
        }

        #endregion

        #region WhenNull
        [Test]
        public void WhenNull_IgnoreProperty_FilterNullFalse()
        {
            //Arrange
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition,
                                                                                               (s1, s2) => s2.Contains(s1),false);
            //Act
            map.WhenNull().IgnoreProperty();

            //Assert
            Assert.That(map.IgnoreNullValues, Is.True);
        }
        [Test]
        public void WhenNull_FilterByNull_FilterNullTrue()
        {
            //Arrange
            var map = new FilterPropertyMap<TestCatalogFilter, ProductMetadata, string, string>(f => f.ProductName, sp => sp.ProductName, _definition,
                                                                                               (s1, s2) => s2.Contains(s1));
            //Act
            map.WhenNull().FilterByNull();

            //Assert
            Assert.That(map.IgnoreNullValues, Is.False);
        }
        #endregion
    }
}
