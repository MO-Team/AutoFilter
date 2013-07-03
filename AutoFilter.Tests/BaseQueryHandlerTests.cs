using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFilter;
using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace AutoFilter.Tests
{
    public class ForTestsBaseQueryHandler<TBaseEntity> : BaseQueryHandler<TBaseEntity>
        where TBaseEntity : class
    {
        public IQueryProvider MockQueryProvider { get; set; }

        public override IQueryable<T> CreateFilter<T>()
        {
            return new List<T>().AsQueryable();
        }

        protected override IQueryProvider GetQueryProvider()
        {
            return MockQueryProvider;
        }        
    }

    [TestFixture]
    public class BaseQueryHandlerTests
    {
        #region Properties

        BaseQueryHandler<User> MockedQueryHandler { get; set; }
        IQueryProvider MockedQueryProvider { get; set; }

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void TestSetUp()
        {
            MockedQueryProvider = MockRepository.GenerateStub<IQueryProvider>();
            MockedQueryHandler = new ForTestsBaseQueryHandler<User>() { MockQueryProvider = MockedQueryProvider};
        }

        #endregion

        #region ExecuteQuery Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteQuery_WithNullIQueryable_Exception()
        {
            MockedQueryHandler.Execute<User>((IQueryable<User>)null);
        }

        [Test]
        public void ExecuteQuery_WithIQueryable_CallProviderWithExpression()
        {
            var queryable = new List<User>().AsQueryable();
            var result = new List<User> { new User() { FirstName = "Doron", LastName = "Yaacoby" } };
            MockedQueryProvider.Expect(p => p.Execute<IEnumerable<User>>(queryable.Expression))
                .Return(result);

            var actualResult = MockedQueryHandler.Execute<User>(queryable);

            Assert.AreEqual(result, actualResult);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteQuery_WithNullLambdaFunc_Exception()
        {
            MockedQueryHandler.Execute<User>((Func<IQueryable<User>, IQueryable<User>>)null);
        }

        [Test]
        public void ExecuteQuery_WithLambda_CallLambdaWithValidParameter() 
        {
            bool wasLambdaCall = false;

            var actualResult = MockedQueryHandler.Execute<User>(q =>
                                                                     {
                                                                         wasLambdaCall = true;
                                                                         Assert.NotNull(q);
                                                                         return q;
                                                                     });

            Assert.IsTrue(wasLambdaCall);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteQuery_WithLambdaThatReturnsNull_Exception()
        {
            var actualResult = MockedQueryHandler.Execute<User>(q => null);
        }

        [Test]
        public void ExecuteQuery_WithLambdaThatReturnsIQueryable_CallProviderWithExpression()
        {
            var queryable = new List<User>().AsQueryable();
            var result = new List<User> { new User() { FirstName = "Doron", LastName = "Yaacoby" } };
            MockedQueryProvider.Expect(p => p.Execute<IEnumerable<User>>(queryable.Expression))
                .Return(result);

            var actualResult = MockedQueryHandler.Execute<User>(q => queryable);

            Assert.AreEqual(result, actualResult);
        }

        [Test]
        public void ExecuteQuery_WithLambdaExpression_CallProviderWithExpression()
        {
            Expression<Func<User, bool>> expression = u => u.FirstName == "Doron";
            var result = new List<User> { new User() { FirstName = "Doron", LastName = "Yaacoby" } };
            MockedQueryProvider.Expect(p => p.Execute<IEnumerable<User>>(expression))
                .Constraints(Is.Matching<Expression>(e => e is MethodCallExpression &&  
                                                          ((MethodCallExpression)e).Method.Name == "Where" && 
                                                          ((MethodCallExpression)e).Arguments[1] is UnaryExpression &&
                                                          ((UnaryExpression)((MethodCallExpression)e).Arguments[1]).Operand == expression))
                .Return(result);

            var actualResult = MockedQueryHandler.Execute<User>(expression);

            Assert.AreEqual(result, actualResult);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteQuery_WithNullExpression_Exception()
        {
            MockedQueryHandler.Execute<User>((Expression)null);
        }

        [Test]
        public void ExecuteQuery_WithExpression_CallProviderWithExpression()
        {
            var expression = new List<User>().AsQueryable().Where(u => u.FirstName == "Doron").Expression;
            var result = new List<User> { new User() { FirstName = "Doron", LastName = "Yaacoby" } };
            MockedQueryProvider.Expect(p => p.Execute<IEnumerable<User>>(expression))
                .Return(result);

            var actualResult = MockedQueryHandler.Execute<User>(expression);

            Assert.AreEqual(result, actualResult);
        }

        #endregion
    }

    public class User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string EMail { get; set; }
    }
}
