using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFilter;
using AutoFilter.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class FilterExpressionBuilderTests
    {
        #region Properties
        private FilterExpressionBuilder<TestCatalogFilter, ProductMetadata> ExpressionBuilder;
        private IFilterConditionExpressionBuilder<TestCatalogFilter, ProductMetadata> NameExpressionBuilderMock;
        private IFilterConditionExpressionBuilder<TestCatalogFilter, ProductMetadata> DateExpressionBuilderMock;
        #endregion

        #region Set Up
        [SetUp]
        public void SetUp()
        {
            ExpressionBuilder = MockRepository.GenerateStub<FilterExpressionBuilder<TestCatalogFilter, ProductMetadata>>();
            ExpressionBuilder.PropertyMappings =
                new Dictionary<string, IFilterConditionExpressionBuilder<TestCatalogFilter, ProductMetadata>>();

            NameExpressionBuilderMock =
                MockRepository.GenerateStub<IFilterConditionExpressionBuilder<TestCatalogFilter, ProductMetadata>>();
            DateExpressionBuilderMock =
                MockRepository.GenerateStub<IFilterConditionExpressionBuilder<TestCatalogFilter, ProductMetadata>>();
        }
        #endregion

        #region BuildExpression Tests

        [Test]
        public void BuildExpression_OnePropertyMapping_CorrectExpressionBuilt()
        {
            // Arrange
            var filter = new TestCatalogFilter
            {
                EditionName = new List<string> { "Orad", "Eyal" }
            };

            NameExpressionBuilderMock.Stub(x => x.CanBuildConditionExpression(filter)).Return(true);
            NameExpressionBuilderMock.Stub(x => x.BuildConditionExpression(filter))
                .Return(sp => filter.EditionName.Contains(sp.ProductName));

            ExpressionBuilder.PropertyMappings.Add(
                "x => x.Name",
                NameExpressionBuilderMock);

            // Act
            var expr = ExpressionBuilder.BuildExpression(filter);

            // Assert
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Orad" }));
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Eyal" }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Dakel" }));
        }

        [Test]
        public void BuildExpression_TwoPropertyMappings_BothPropertiesInResultantExpression()
        {
            // Arrange
            var filter = new TestCatalogFilter
            {
                EditionName = new List<string> { "Orad", "Eyal" },
                DataInsertionDate = new DateRange(DateTime.Today, DateTime.Today.AddDays(2))
            };

            NameExpressionBuilderMock.Stub(x => x.CanBuildConditionExpression(filter)).Return(true);
            NameExpressionBuilderMock.Stub(x => x.BuildConditionExpression(filter))
                .Return(sp => filter.EditionName.Contains(sp.ProductName));

            DateExpressionBuilderMock.Stub(x => x.CanBuildConditionExpression(filter)).Return(true);
            DateExpressionBuilderMock.Stub(x => x.BuildConditionExpression(filter))
                .Return(sp => (filter.DataInsertionDate.MinValue <= sp.DataInsertionDate && filter.DataInsertionDate.MaxValue >= sp.DataInsertionDate));

            ExpressionBuilder.PropertyMappings.Add(
                "x => x.Name",
                NameExpressionBuilderMock);

            ExpressionBuilder.PropertyMappings.Add(
                "x => x.DataInsertionDate",
                DateExpressionBuilderMock);

            // Act
            var expr = ExpressionBuilder.BuildExpression(filter);

            // Assert
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Orad" }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Eyal" }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Dakel" }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { DataInsertionDate = DateTime.Today.AddHours(1) }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { DataInsertionDate = DateTime.Today.AddDays(1) }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { DataInsertionDate = DateTime.Today.AddDays(3) }));
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Orad", DataInsertionDate = DateTime.Today.AddHours(1) }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Dakel", DataInsertionDate = DateTime.Today.AddHours(1) }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Orad", DataInsertionDate = DateTime.Today.AddDays(3) }));
        }

        [Test]
        public void BuildExpression_NoPropertyMappings_HandleEmptyExpressionCalled()
        {
            // Arrange
            var filter = new TestCatalogFilter
            {
                EditionName = new List<string> { "Orad", "Eyal" }
            };

            // Act
            var expr = ExpressionBuilder.BuildExpression(filter);

            // Assert
            ExpressionBuilder.AssertWasCalled(x => x.HandleEmptyExpression());
        }

        [Test]
        public void BuildExpression_NoPropertyMappings_ReturnsTrueExpression()
        {
            // Arrange
            var filter = new TestCatalogFilter
            {
                EditionName = new List<string> { "Orad", "Eyal" }
            };
            filter.DataInsertionDate = null;
            ExpressionBuilder.Expect(x => x.HandleEmptyExpression())
                             .CallOriginalMethod(OriginalCallOptions.NoExpectation);

            // Act
            var expr = ExpressionBuilder.BuildExpression(filter);

            // Assert
            Assert.NotNull(expr);
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new ProductMetadata()));
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new object[] { null }));
        }

        [Test]
        public void BuildExpression_PropertyMappingsHasNullConditionExpressionBuilder_ReturnsTrueExpression()
        {
            // Arrange
            var filter = new TestCatalogFilter
            {
                EditionName = new List<string> { "Orad", "Eyal" }
            };
            filter.DataInsertionDate = null;
            ExpressionBuilder.Expect(x => x.HandleEmptyExpression())
                             .CallOriginalMethod(OriginalCallOptions.NoExpectation);
            ExpressionBuilder.PropertyMappings.Add("x => x.Name", null);

            // Act
            var expr = ExpressionBuilder.BuildExpression(filter);

            // Assert
            Assert.NotNull(expr);
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new ProductMetadata()));
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new object[] { null }));
        }

        [Test]
        public void BuildExpression_CantBuildExpressionForProperty_PropertyNotInResultantExpression()
        {
            // Arrange
            var filter = new TestCatalogFilter
            {
                EditionName = new List<string> { "Orad", "Eyal" },
                DataInsertionDate = new DateRange(DateTime.Today, DateTime.Today.AddDays(2))
            };

            NameExpressionBuilderMock.Stub(x => x.CanBuildConditionExpression(filter)).Return(true);
            NameExpressionBuilderMock.Stub(x => x.BuildConditionExpression(filter))
                .Return(sp => filter.EditionName.Contains(sp.ProductName));

            DateExpressionBuilderMock.Stub(x => x.CanBuildConditionExpression(filter)).Return(false);

            ExpressionBuilder.PropertyMappings.Add(
                "x => x.Name",
                NameExpressionBuilderMock);

            ExpressionBuilder.PropertyMappings.Add(
                "x => x.DataInsertionDate",
                DateExpressionBuilderMock);

            // Act
            var expr = ExpressionBuilder.BuildExpression(filter);

            // Assert
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Orad" }));
            DateExpressionBuilderMock.AssertWasNotCalled(x => x.BuildConditionExpression(filter));
        }

        [Test]
        public void BuildExpression_ConditionExpressionBuiltForPropertyIsNull_PropertyNotInResultantExpression()
        {
            // Arrange
            var filter = new TestCatalogFilter
            {
                EditionName = new List<string> { "Orad", "Eyal" },
                DataInsertionDate = new DateRange(DateTime.Today, DateTime.Today.AddDays(2))
            };

            NameExpressionBuilderMock.Stub(x => x.CanBuildConditionExpression(filter)).Return(true);
            NameExpressionBuilderMock.Stub(x => x.BuildConditionExpression(filter))
                .Return(sp => filter.EditionName.Contains(sp.ProductName));

            DateExpressionBuilderMock.Stub(x => x.CanBuildConditionExpression(filter)).Return(true);
            DateExpressionBuilderMock.Stub(x => x.BuildConditionExpression(filter))
                .Return(null);

            ExpressionBuilder.PropertyMappings.Add(
                "x => x.Name",
                NameExpressionBuilderMock);

            ExpressionBuilder.PropertyMappings.Add(
                "x => x.DataInsertionDate",
                DateExpressionBuilderMock);

            // Act
            var expr = ExpressionBuilder.BuildExpression(filter);

            // Assert
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Orad" }));
        }

        [Test]
        public void BuildExpression_ConditionExpressionBuiltForNullPropertyMarkedAsFilterByNull_ReturnsTrueExpression()
        {
            // Arrange
            var filter = new TestCatalogFilter();

            var productNameExpressionBuilderMock =
                MockRepository.GenerateStub<IFilterConditionExpressionBuilder<TestCatalogFilter, ProductMetadata>>();
            productNameExpressionBuilderMock.Stub(x => x.CanBuildConditionExpression(filter)).Return(true);
            productNameExpressionBuilderMock.Stub(x => x.BuildConditionExpression(filter))
                .Return(sp=> sp.ProductName == filter.ProductName);

           
            ExpressionBuilder.PropertyMappings.Add(
                "x => x.ProductName",
                productNameExpressionBuilderMock);

            // Act
            var expr = ExpressionBuilder.BuildExpression(filter);

            // Assert
            Assert.AreEqual(true, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = null }));
            Assert.AreEqual(false, expr.Compile().DynamicInvoke(new ProductMetadata { ProductName = "Product Name" }));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildExpression_NullFilter_ArgumentNullException()
        {
            // Act
            ExpressionBuilder.BuildExpression(null);
        }

        #endregion

        #region MapPropertiesWithSameName Tests
       

        [Test]
        public void MapPropertiesWithSameName_ClassWithTwoProperties_TwoPropertyMappingsAddedToDictionary()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassTwoProperties, EntityTestClassTwoProperties>();

            Expression<Func<string, string, bool>> testNamePredicateExpression = ((x, y) => true);
            Expression<Func<DateTime?, DateTime?, bool>> testDatePredicateExpression = ((x, y) => true);

            Expression<Func<FilterTestClassTwoProperties, string>> expectedFilterNameExpression =
                FilterTestClassTwoProperties => FilterTestClassTwoProperties.Name;
            Expression<Func<EntityTestClassTwoProperties, string>> expectedEntityNameExpression =
                EntityTestClassTwoProperties => EntityTestClassTwoProperties.Name;

            Expression<Func<FilterTestClassTwoProperties, DateTime?>> expectedFilterDateExpression =
                FilterTestClassTwoProperties => FilterTestClassTwoProperties.Date;
            Expression<Func<EntityTestClassTwoProperties, DateTime?>> expectedEntityDateExpression =
                EntityTestClassTwoProperties => EntityTestClassTwoProperties.Date;


            var predicateProviderStub = MockRepository.GenerateStub<IPredicateProvider>();
            var propertyMapFactoryStub = (IFilterPropertyMapFactory<FilterTestClassTwoProperties, EntityTestClassTwoProperties>)
                MockRepository.GenerateStub(typeof(IFilterPropertyMapFactory<FilterTestClassTwoProperties, EntityTestClassTwoProperties>));

            var namePropertyMapStub =
                MockRepository.GenerateStub
                    <IFilterPropertyMap<FilterTestClassTwoProperties, EntityTestClassTwoProperties, string, string>>();
            var datePropertyMapStub =
                MockRepository.GenerateStub
                    <IFilterPropertyMap<FilterTestClassTwoProperties, EntityTestClassTwoProperties, DateTime?, DateTime?>>();

            predicateProviderStub.Stub(
               x => x.BuildPredicateExpression(typeof(string), typeof(string), ComparisonType.Equal))
               .Return(testNamePredicateExpression);
            predicateProviderStub.Stub(
                x => x.BuildPredicateExpression(typeof(DateTime?), typeof(DateTime?), ComparisonType.Equal))
                .Return(testDatePredicateExpression);

            propertyMapFactoryStub.Stub(
                x =>
                x.CreateFilterPropertyMap(
                    Arg<Expression<Func<FilterTestClassTwoProperties, string>>>.Matches(
                        new LambdaMockingConstraint<FilterTestClassTwoProperties, string>(expectedFilterNameExpression)),
                    Arg<Expression<Func<EntityTestClassTwoProperties, string>>>.Matches(
                        new LambdaMockingConstraint<EntityTestClassTwoProperties, string>(expectedEntityNameExpression)),
                    Arg<Expression<Func<string, string, bool>>>.Is.Equal(testNamePredicateExpression)))
                .Return(namePropertyMapStub);

            propertyMapFactoryStub.Stub(
                x =>
                x.CreateFilterPropertyMap(
                    Arg<Expression<Func<FilterTestClassTwoProperties, DateTime?>>>.Matches(
                        new LambdaMockingConstraint<FilterTestClassTwoProperties, DateTime?>(expectedFilterDateExpression)),
                    Arg<Expression<Func<EntityTestClassTwoProperties, DateTime?>>>.Matches(
                        new LambdaMockingConstraint<EntityTestClassTwoProperties, DateTime?>(expectedEntityDateExpression)),
                    Arg<Expression<Func<DateTime?, DateTime?, bool>>>.Is.Equal(testDatePredicateExpression)))
                .Return(datePropertyMapStub);

            predicateProviderStub.Stub(
                x => x.CanBuildPredicateExpression(typeof(string), typeof(string), ComparisonType.Equal))
                .Return(true);
            predicateProviderStub.Stub(
                x => x.CanBuildPredicateExpression(typeof(DateTime?), typeof(DateTime?), ComparisonType.Equal))
                .Return(true);

            expressionBuilder.PredicateProvider = predicateProviderStub;
            expressionBuilder.PropertyMapFactory = propertyMapFactoryStub;

            // Act
            var resultantPropertyMappings = expressionBuilder.PropertyMappings;

            // Assert
            Assert.That(resultantPropertyMappings.Values.Count == 2);
            Assert.That(resultantPropertyMappings.Values.Contains(namePropertyMapStub));
            Assert.That(resultantPropertyMappings.Values.Contains(datePropertyMapStub));
        }

        [Test]
        public void MapPropertiesWithSameName_FilterAndEntityClassWithNoProperties_EmptyDictionary()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassNoProperties, EntityTestClassNoProperties>();

            // Act
            var resultantPropertyMappings = expressionBuilder.PropertyMappings;

            // Assert
            Assert.That(resultantPropertyMappings.Count, Is.EqualTo(0));
        }

        [Test]
        public void MapPropertiesWithSameName_FilterClassWithNoPropertiesButEntityClassWithProperty_EmptyDictionary()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassNoProperties, EntityTestClassOneProperty>();

            // Act
            var resultantPropertyMappings = expressionBuilder.PropertyMappings;

            // Assert
            Assert.That(resultantPropertyMappings.Count, Is.EqualTo(0));
        }

        [Test]
        public void MapPropertiesWithSameName_FilterClassWithOnePropertyButEntityClassWithNoProperties_EmptyDictionary()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassNoProperties>();

            // Act
            var resultantPropertyMappings = expressionBuilder.PropertyMappings;

            // Assert
            Assert.That(resultantPropertyMappings.Count, Is.EqualTo(0));
        }

        [Test]
        public void MapPropertiesWithSameName_FilterClassWithIndexedProperty_EmptyDictionary()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassIndexedProperty, EntityTestClassIndexedProperty>();

            // Act
            var resultantPropertyMappings = expressionBuilder.PropertyMappings;

            // Assert
            Assert.That(resultantPropertyMappings.Count, Is.EqualTo(0));
        }

        [Test]
        public void MapPropertiesWithSameName_CannotBuildPredicateExpression_BuildPredicateExpressionNotCalledAndPropertyMapAddedToDictionary()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>();

            Expression<Func<FilterTestClassOneProperty, string>> expectedFilterNameExpression =
                FilterTestClassOneProperty => FilterTestClassOneProperty.Name;
            Expression<Func<EntityTestClassOneProperty, string>> expectedEntityNameExpression =
                EntityTestClassOneProperty => EntityTestClassOneProperty.Name;


            var predicateProviderMock = MockRepository.GenerateMock<IPredicateProvider>();
            var propertyMapFactoryStub =
                MockRepository.GenerateStub<IFilterPropertyMapFactory<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            var namePropertyMapStub =
                MockRepository.GenerateStub
                    <IFilterPropertyMap<FilterTestClassOneProperty, EntityTestClassOneProperty, string, string>>();

            propertyMapFactoryStub.Stub(
                x =>
                x.CreateFilterPropertyMap(
                    Arg<Expression<Func<FilterTestClassOneProperty, string>>>.Matches(
                        new LambdaMockingConstraint<FilterTestClassOneProperty, string>(expectedFilterNameExpression)),
                    Arg<Expression<Func<EntityTestClassOneProperty, string>>>.Matches(
                        new LambdaMockingConstraint<EntityTestClassOneProperty, string>(expectedEntityNameExpression)),
                    Arg<Expression<Func<string, string, bool>>>.Is.Equal(null)))
                .Return(namePropertyMapStub);

            predicateProviderMock.Stub(
                x => x.CanBuildPredicateExpression(typeof(string), typeof(string), ComparisonType.Equal))
                .Return(false);

            expressionBuilder.PredicateProvider = predicateProviderMock;
            expressionBuilder.PropertyMapFactory = propertyMapFactoryStub;

            // Act
            var resultantPropertyMappings = expressionBuilder.PropertyMappings;

            // Assert
            predicateProviderMock.AssertWasNotCalled(
                x => x.BuildPredicateExpression(null, null, ComparisonType.Equal),
                o => o.IgnoreArguments());
            Assert.That(resultantPropertyMappings.Values.Count == 1);
            Assert.That(resultantPropertyMappings.Values.Contains(namePropertyMapStub));
        }

        #endregion

        #region Map Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Map_FilterPropertyNull_ArgumentNullException()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>();

            Expression<Func<FilterTestClassOneProperty, string>> expectedFilterNameExpression =
                null;
            Expression<Func<EntityTestClassOneProperty, string>> expectedEntityNameExpression =
                EntityTestClassOneProperty => EntityTestClassOneProperty.Name;

            // Act
            expressionBuilder.Map(
                expectedFilterNameExpression,
                expectedEntityNameExpression,
                ComparisonType.Equal);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Map_EntityPropertyNull_ArgumentNullException()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>();

            Expression<Func<string, string, bool>> testNamePredicateExpression = ((x, y) => true);

            Expression<Func<FilterTestClassOneProperty, string>> expectedFilterNameExpression =
                FilterTestClassOneProperty => FilterTestClassOneProperty.Name;
            Expression<Func<EntityTestClassOneProperty, string>> expectedEntityNameExpression =
                null;

            // Act
            expressionBuilder.Map(
                expectedFilterNameExpression,
                expectedEntityNameExpression,
                ComparisonType.Equal);
        }

        [Test]
        public void Map_FilterPropertyDoesntExistInDictionary_FilterPropertyAddedToDictionaryAndReturned()
        {
            // Arrange
            var namePropertyMapStub =
                MockRepository.GenerateStub<IFilterPropertyMap<FilterTestClassOneProperty, EntityTestClassOneProperty, string, string>>();
            var expressionBuilder = CreateStubbedExpressionBuilder(namePropertyMapStub);

            expressionBuilder.PropertyMappings =
                new Dictionary<string, IFilterConditionExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            // Act
            var resultantMap = expressionBuilder.Map(f => f.Name, e => e.Name, ComparisonType.Equal);

            // Assert
            Assert.That(expressionBuilder.PropertyMappings.Values.Count == 1);
            Assert.That(expressionBuilder.PropertyMappings.Values.Contains(namePropertyMapStub));
            Assert.That(resultantMap, Is.EqualTo(namePropertyMapStub));
        }

        [Test]
        public void Map_FilterPropertyExistsInDictionary_FilterPropertyUpdatedInDictionaryAndReturned()
        {
            // Arrange
            var namePropertyMapStub = 
                MockRepository.GenerateStub<IFilterPropertyMap<FilterTestClassOneProperty, EntityTestClassOneProperty, string, string>>();
            var expressionBuilder = CreateStubbedExpressionBuilder(namePropertyMapStub);

            expressionBuilder.PropertyMappings =
                new Dictionary<string, IFilterConditionExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            expressionBuilder.PropertyMappings.Add("x => x.Name",
                                                   null);

            // Act
            var resultantMap = expressionBuilder.Map(f => f.Name, e => e.Name, ComparisonType.Equal);

            // Assert
            Assert.That(expressionBuilder.PropertyMappings.Values.Count == 1);
            Assert.That(expressionBuilder.PropertyMappings.Values.Contains(namePropertyMapStub));
            Assert.That(resultantMap, Is.EqualTo(namePropertyMapStub));
        }

        [Test]
        public void Map_FilterPropertyWasIgnoredBefore_FilterPropertyUpdatedInDictionaryAndReturned()
        {
            // Arrange
            var namePropertyMapStub =
                MockRepository.GenerateStub<IFilterPropertyMap<FilterTestClassOneProperty, EntityTestClassOneProperty, string, string>>();
            var expressionBuilder = CreateStubbedExpressionBuilder(namePropertyMapStub);

            expressionBuilder.PropertyMappings =
                new Dictionary<string, IFilterConditionExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            // Act
            var resultantMap = expressionBuilder.Ignore(f => f.Name)
                                                .Map(f => f.Name, e => e.Name, ComparisonType.Equal);

            // Assert
            Assert.That(expressionBuilder.PropertyMappings.Values.Count == 1);
            Assert.That(expressionBuilder.PropertyMappings.Values.Contains(namePropertyMapStub));
            Assert.That(resultantMap, Is.EqualTo(namePropertyMapStub));
        }

        [Test]
        public void Map_CannotBuildPredicate_FilterPropertyUpdatedInDictionaryAndReturnedAndBuildPredicateExpressionNotCalled()
        {
            // Arrange
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>();

            expressionBuilder.PropertyMappings =
                new Dictionary<string, IFilterConditionExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            expressionBuilder.PropertyMappings.Add(
                "x => x.Name",
                null);

            Expression<Func<string, string, bool>> testNamePredicateExpression = ((x, y) => true);

            Expression<Func<FilterTestClassOneProperty, string>> expectedFilterNameExpression =
                FilterTestClassOneProperty => FilterTestClassOneProperty.Name;
            Expression<Func<EntityTestClassOneProperty, string>> expectedEntityNameExpression =
                EntityTestClassOneProperty => EntityTestClassOneProperty.Name;

            var predicateProviderMock = MockRepository.GenerateMock<IPredicateProvider>();

            var namePropertyMapStub =
                MockRepository.GenerateStub
                    <IFilterPropertyMap<FilterTestClassOneProperty, EntityTestClassOneProperty, string, string>>();

            var propertyMapFactoryStub =
                MockRepository.GenerateStub < IFilterPropertyMapFactory<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            predicateProviderMock.Stub(
                x => x.CanBuildPredicateExpression<string, string>(ComparisonType.Equal))
                .Return(false);

            propertyMapFactoryStub.Stub(
                x =>
                x.CreateFilterPropertyMap(
                    Arg<Expression<Func<FilterTestClassOneProperty, string>>>.Matches(
                        new LambdaMockingConstraint<FilterTestClassOneProperty, string>(expectedFilterNameExpression)),
                    Arg<Expression<Func<EntityTestClassOneProperty, string>>>.Matches(
                        new LambdaMockingConstraint<EntityTestClassOneProperty, string>(expectedEntityNameExpression)),
                    Arg<Expression<Func<string, string, bool>>>.Is.Null))
                .Return(namePropertyMapStub);

            expressionBuilder.PredicateProvider = predicateProviderMock;
            expressionBuilder.PropertyMapFactory = propertyMapFactoryStub;

            // Act
            var resultantMap = expressionBuilder.Map(
                expectedFilterNameExpression,
                expectedEntityNameExpression,
                ComparisonType.Equal);

            // Assert
            predicateProviderMock.AssertWasNotCalled(
                x => x.BuildPredicateExpression(null, null, ComparisonType.Equal),
                o => o.IgnoreArguments());

            Assert.That(expressionBuilder.PropertyMappings.Values.Count == 1);
            Assert.That(expressionBuilder.PropertyMappings.Values.Contains(namePropertyMapStub));
            Assert.That(resultantMap, Is.EqualTo(namePropertyMapStub));
        }

        [Test]
        public void Map_NoPredicateParameter_MapCalledWithDefaultPredicateParameter()
        {
            // Arrange
            var expressionBuilder =
                MockRepository.GenerateMock<FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            Expression<Func<FilterTestClassOneProperty, string>> expectedFilterNameExpression =
                FilterTestClassOneProperty => FilterTestClassOneProperty.Name;
            Expression<Func<EntityTestClassOneProperty, string>> expectedEntityNameExpression =
                EntityTestClassOneProperty => EntityTestClassOneProperty.Name;

            var namePropertyMapStub =
                MockRepository.GenerateStub<IFilterPropertyMap<FilterTestClassOneProperty, EntityTestClassOneProperty, string, string>>();

            expressionBuilder.Stub(x => x.Map(
                expectedFilterNameExpression,
                expectedEntityNameExpression,
                ComparisonType.Equal))
                .Return(namePropertyMapStub);

            // Act
            var resultantMap = expressionBuilder.Map(
                expectedFilterNameExpression,
                expectedEntityNameExpression);


            // Assert
            expressionBuilder.AssertWasCalled(x => x.Map(
                expectedFilterNameExpression,
                expectedEntityNameExpression,
                ComparisonType.Equal));

            Assert.That(resultantMap, Is.EqualTo(namePropertyMapStub));
        }

        private FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty> CreateStubbedExpressionBuilder(
                                IFilterPropertyMap<FilterTestClassOneProperty, EntityTestClassOneProperty, string, string> resultantPropertyMap)
        {
            var expressionBuilder =
                new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>();

            expressionBuilder.PropertyMappings =
                new Dictionary<string, IFilterConditionExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            Expression<Func<string, string, bool>> testNamePredicateExpression = ((x, y) => true);

            Expression<Func<FilterTestClassOneProperty, string>> expectedFilterNameExpression =
                FilterTestClassOneProperty => FilterTestClassOneProperty.Name;
            Expression<Func<EntityTestClassOneProperty, string>> expectedEntityNameExpression =
                EntityTestClassOneProperty => EntityTestClassOneProperty.Name;

            var predicateProviderStub = MockRepository.GenerateStub<IPredicateProvider>();

            var propertyMapFactoryStub =
                MockRepository.GenerateStub<IFilterPropertyMapFactory<FilterTestClassOneProperty, EntityTestClassOneProperty>>();

            predicateProviderStub.Stub(
                x => x.CanBuildPredicateExpression<string, string>(ComparisonType.Equal))
                .Return(true);

            predicateProviderStub.Stub(
               x => x.BuildPredicateExpression<string, string>(ComparisonType.Equal))
               .Return(testNamePredicateExpression);

            propertyMapFactoryStub.Stub(
                x =>
                x.CreateFilterPropertyMap(
                    Arg<Expression<Func<FilterTestClassOneProperty, string>>>.Matches(
                        new LambdaMockingConstraint<FilterTestClassOneProperty, string>(expectedFilterNameExpression)),
                    Arg<Expression<Func<EntityTestClassOneProperty, string>>>.Matches(
                        new LambdaMockingConstraint<EntityTestClassOneProperty, string>(expectedEntityNameExpression)),
                    Arg<Expression<Func<string, string, bool>>>.Is.Equal(testNamePredicateExpression)))
                .Return(resultantPropertyMap);

            expressionBuilder.PredicateProvider = predicateProviderStub;
            expressionBuilder.PropertyMapFactory = propertyMapFactoryStub;

            return expressionBuilder;
        }

        #endregion

        #region Ignore Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ignore_FilterPropertyNull_ArgumentNullException()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassNoProperties, EntityTestClassNoProperties>();

            // Act
            expressionBuilder.Ignore<int>(null);
        }

        [Test]
        public void Ignore_FilterPropertyDoesntExistInDictionary_FilterPropertyAddedToDictionaryWithNullConditionExpressionBuilder()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>();

            // Act
            expressionBuilder.Ignore(f => f.Name);

            // Assert
            Assert.That(expressionBuilder.PropertyMappings.Count, Is.EqualTo(1));
            Assert.That(expressionBuilder.PropertyMappings.Values.First(), Is.Null);
        }

        [Test]
        public void Ignore_FilterPropertyExistInDictionary_ConditionExpressionBuilderChanegedToNull()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassOneProperty>();
            expressionBuilder.Map(f => f.Name, e => e.Name);

            // Act
            expressionBuilder.Ignore(f => f.Name);

            // Assert
            Assert.That(expressionBuilder.PropertyMappings.Values.Single(), Is.Null);
        }

        #endregion

        #region AssertConfigurationIsValid Tests

        [Test]
        public void AssertConfigurationIsValid_NoProperties_AllOk()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassNoProperties, EntityTestClassNoProperties>();

            // Act
            expressionBuilder.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_AllPropertiesMappedAutomatically_AllOk()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassTwoProperties, EntityTestClassTwoProperties>();

            // Act
            expressionBuilder.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_AllPropertiesMappedManually_AllOk()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassTwoProperties, EntityTestClassTwoProperties>();
            expressionBuilder.Map(f => f.Name, e => e.Name).UsePredicate((f, e) => e.Contains(f))
                             .Map(f => f.Date, e => e.Date).UsePredicate((f, e) => e.Value.Date == f.Value.Date);

            // Act
            expressionBuilder.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_OnePropertyIgnored_AllOk()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassTwoProperties, EntityTestClassOneProperty>();
            expressionBuilder.Ignore(f => f.Date);

            // Act
            expressionBuilder.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_FilterHasUnmappedIndexPreprty_AllOk()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassIndexedProperty, EntityTestClassNoProperties>();

            // Act
            expressionBuilder.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_OnePropertyMissingMapping_FilterPropertyMissingMappingException()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassOneProperty, EntityTestClassNoProperties>();

            // Act
            TestDelegate action = () => expressionBuilder.AssertConfigurationIsValid();

            // Assert
            Assert.That(action, Throws.InstanceOf<FilterPropertyMissingMappingException>()
                                      .And.Message.Contains("Name"));
        }

        [Test]
        public void AssertConfigurationIsValid_MoreThanOnePropertiesMissingMapping_FilterPropertyMissingMappingException()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassTwoProperties, EntityTestClassNoProperties>();

            // Act
            TestDelegate action = () => expressionBuilder.AssertConfigurationIsValid();

            // Assert
            Assert.That(action, Throws.InstanceOf<FilterPropertyMissingMappingException>()
                                      .And.Message.Contains("Name")
                                      .And.Message.Contains("Date"));
        }

        [Test]
        public void AssertConfigurationIsValid_OnePropertyMappedAndOneNotMapped_FilterPropertyMissingMappingException()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassTwoProperties, EntityTestClassOneProperty>();
            expressionBuilder.Map(f => f.Name, e => e.Name);

            // Act
            TestDelegate action = () => expressionBuilder.AssertConfigurationIsValid();

            // Assert
            Assert.That(action, Throws.InstanceOf<FilterPropertyMissingMappingException>()
                                      .And.Message.Not.Contains("Name")
                                      .And.Message.Contains("Date"));
        }

        [Test]
        public void AssertConfigurationIsValid_OnePropertyIgnoredAndOneNotMapped_FilterPropertyMissingMappingException()
        {
            // Arrange
            var expressionBuilder = new FilterExpressionBuilder<FilterTestClassTwoProperties, EntityTestClassNoProperties>();
            expressionBuilder.Ignore(f => f.Name);

            // Act
            TestDelegate action = () => expressionBuilder.AssertConfigurationIsValid();

            // Assert
            Assert.That(action, Throws.InstanceOf<FilterPropertyMissingMappingException>()
                                      .And.Message.Not.Contains("Name")
                                      .And.Message.Contains("Date"));
        }

        #endregion
    }

    public class CatalogFilter
    {
        public string ProductName;
        public DateRange DataInsertionDate;
    }

    public class ProductMetadata
    {
        public string ProductName;
        public DateTime? DataInsertionDate;
    }

    #region Test Classes
    public class FilterTestClassNoProperties
    {

    }

    public class EntityTestClassNoProperties
    {
        public string PublicField;
        public void PublicMethod(){}
        private string PrivateProperty { get; set; }
    }

    public class FilterTestClassOneProperty
    {
        public string Name { get; set; }
    }

    public class EntityTestClassOneProperty
    {
        public string Name { get; set; }
    }

    public class FilterTestClassTwoProperties
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
    }

    public class EntityTestClassTwoProperties
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
    }

    public class FilterTestClassRangeProperty
    {
        public DateRange Date { get; set; }
    }

    public class EntityTestClassRangeProperty
    {
        public DateTime? Date { get; set; }
    }

    public class FilterTestClassIndexedProperty
    {
        public string this[int index]
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

    }

    public class EntityTestClassIndexedProperty
    {
        public string this[int index]
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }
    }
    #endregion
}
