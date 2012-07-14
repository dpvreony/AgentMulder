using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.VB.Tree;
using JetBrains.ReSharper.Psi.Resolve.Managed;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Patterns.Helpers
{
    public class ReturnTypeCollector : TreeNodeVisitor, IRecursiveElementProcessor
    {
        private readonly List<IExpressionType> expressionTypes = new List<IExpressionType>();
        private readonly IResolveContext resolveContext;

        public IEnumerable<IExpressionType> CollectedTypes
        {
            get { return expressionTypes; }
        }

        public bool ProcessingIsFinished
        {
            get { return false; }
        }

        public ReturnTypeCollector(IResolveContext resolveContext)
        {
            this.resolveContext = resolveContext;
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return !(element is IReturnStatement);
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            var VBTreeNode = element as IVBTreeNode;
            if (VBTreeNode != null)
                VBTreeNode.Accept(this);
            else
                VisitNode(element);
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        public override void VisitReturnStatement(IReturnStatement returnStatementParam)
        {
            IVBExpression VBExpression = returnStatementParam.Expression;
            expressionTypes.Add(VBExpression != null
                                    ? VBExpression.GetExpressionType(resolveContext)
                                    : returnStatementParam.GetPsiModule().GetPredefinedType().Void);

            base.VisitReturnStatement(returnStatementParam);
        }
    }
}