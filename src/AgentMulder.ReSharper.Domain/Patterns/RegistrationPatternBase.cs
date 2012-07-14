using System;
using System.Collections.Generic;
using System.Linq;
using AgentMulder.ReSharper.Domain.Registrations;
using AgentMulder.ReSharper.Domain.Utils;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.ReSharper.Domain.Patterns
{
    public abstract class RegistrationPatternBase : IRegistrationPattern
    {
        private readonly IStructuralMatcher matcher;

        public IStructuralMatcher Matcher
        {
            get { return matcher; }
        }

        protected RegistrationPatternBase(IStructuralSearchPattern pattern)
        {
            matcher = pattern.CreateMatcher();
            if (pattern.Check() != null)
            {
                throw new Exception(pattern.SearchPattern);
            }
        }

        private IInvocationExpression GetMatchedExpression(ITreeNode element)
        {
            var invocationExpression = element as IInvocationExpression;
            if (invocationExpression == null)
                return null;

            return invocationExpression.GetAllExpressions().FirstOrDefault(expression => matcher.QuickMatch(expression));
        }

        private IEnumerable<IInvocationExpression> GetAllMatchedExpressions(ITreeNode element)
        {
            var invocationExpression = element as IInvocationExpression;
            if (invocationExpression == null)
                return null;

            return invocationExpression.GetAllExpressions().Where(expression => matcher.QuickMatch(expression));
        }

        protected IStructuralMatchResult Match(ITreeNode treeNode)
        {
            IInvocationExpression expression = GetMatchedExpression(treeNode);
            if (expression == null)
            {
                return matcher.Match(treeNode);
            }

            return matcher.Match(expression);
        }

        protected IEnumerable<IStructuralMatchResult> MatchMany(ITreeNode treeNode)
        {
            return GetAllMatchedExpressions(treeNode).Select(expression => matcher.Match(expression));
        }


        public abstract IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement);
    }
}