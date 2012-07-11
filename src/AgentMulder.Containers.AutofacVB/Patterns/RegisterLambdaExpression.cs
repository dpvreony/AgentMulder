using System.Collections.Generic;
using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.VB.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    internal sealed class RegisterLambdaExpression : RegistrationPatternBase
    {
        private static readonly IStructuralSearchPattern pattern = 
            new VBStructuralSearchPattern("$builder$.Register(Function($args$) $expression$)",
                new ExpressionPlaceholder("builder", "Global.Autofac.ContainerBuilder", true),
                new ArgumentPlaceholder("args", -1, -1),
                new ExpressionPlaceholder("expression"));

        public RegisterLambdaExpression()
            : base(pattern)
        {
        }

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            IStructuralMatchResult match = Match(registrationRootElement);

            if (match.Matched)
            {
                var expression = match.GetMatchedElement<IObjectCreationExpression>("expression");
                if (expression != null && expression.TypeReference != null)
                {
                    IResolveResult resolveResult = expression.TypeReference.Resolve().Result;
                    var @class = resolveResult.DeclaredElement as IClass;
                    if (@class != null)
                    {
                        yield return new ComponentRegistration(registrationRootElement, @class, @class);
                    }
                }
            }
        }
    }
}