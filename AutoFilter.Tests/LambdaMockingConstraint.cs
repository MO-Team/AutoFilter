using System;
using AutoFilter;
using Rhino.Mocks.Constraints;
using System.Linq.Expressions;

namespace AutoFilter.Tests
{
    public class LambdaMockingConstraint<TClass, TMember> : AbstractConstraint
    {
        private Expression<Func<TClass, TMember>> Lambda { get; set; }

        public LambdaMockingConstraint(Expression<Func<TClass, TMember>> lambda)
        {
            Lambda = lambda;
        }

        public override bool Eval(object obj)
        { 
            var expression = obj as Expression<Func<TClass, TMember>>;
            return GetPropertyMapKey(expression) == GetPropertyMapKey(Lambda);
        }

        public override string Message
        {
            get { return "Not done"; }
        }

        private string GetPropertyMapKey(LambdaExpression filterPropertyLambda)
        {
            var lambdaParameter = filterPropertyLambda.Parameters[0];
            var paramWithConstName = Expression.Parameter(lambdaParameter.Type, "x");
            filterPropertyLambda = filterPropertyLambda.ReplaceExpression(lambdaParameter, paramWithConstName);

            return filterPropertyLambda.ToString();
        }
    }
}
