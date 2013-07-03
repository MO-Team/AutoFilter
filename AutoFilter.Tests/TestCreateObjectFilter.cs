using System;
using AutoFilter.Interfaces;

namespace AutoFilter.Tests
{
    public class TestCreateObjectFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestObjectFilter, TestObjectFilterQuery>();
            TimesPerformed++;
        }
    }

    public class TestCreateSecondObjectFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestObjectFilter, TestObjectFilterQuery>();
            TimesPerformed++;
        }
    }

    public class TestCreateCatalogFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();
            TimesPerformed++;
        }
    }

    public class TestCreateEmptyFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            TimesPerformed++;
        }
    }

    public class TestCreateDoubleFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestObjectFilter, TestObjectFilterQuery>();
            filterConfiguration.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();
            TimesPerformed++;
        }
    }

    public class TestCreateUserFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestUserFilter, TestUserFilterQuery>();
            TimesPerformed++;
        }
    }

    public class TestCreateMixFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestUserFilter, TestUserFilterQuery>();
            filterConfiguration.CreateFilter<TestUserFilter, TestObjectFilterQuery>();
            TimesPerformed++;
        }
    }

    public abstract class TestCreateAbstractUserFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestUserFilter, TestUserFilterQuery>();
            TimesPerformed++;
        }
    }

    public abstract class TestCreateGenericUserFilter<T> : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestUserFilter, TestUserFilterQuery>();
            TimesPerformed++;
        }
    }

    public abstract class TestCreateNoDefaultCtorUserFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public TestCreateNoDefaultCtorUserFilter(int dummy)
        {
            
        }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestUserFilter, TestUserFilterQuery>();
            TimesPerformed++;
        }
    }

    public struct TestCreateStructUserFilter : ICreateFilter
    {
        public static int TimesPerformed { get; set; }

        public void CreateFilter(IFilterConfiguration filterConfiguration)
        {
            filterConfiguration.CreateFilter<TestUserFilter, TestUserFilterQuery>();
            TimesPerformed++;
        }
    }

    public class QueryHandlerTestClass
    {

        public virtual int IdAtSource { get; set; }

        public virtual string ProductName { get; set; }
        public virtual DateTime DataInsertionDate { get; set; }
    }
}