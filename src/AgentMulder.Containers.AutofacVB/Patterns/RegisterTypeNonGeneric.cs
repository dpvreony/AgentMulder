using System.Collections.Generic;
using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.VB.Tree;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public class RegisterTypeNonGeneric : ComponentImplementationPatternBase
    {
        private static readonly IStructuralSearchPattern pattern = 
            new VBStructuralSearchPattern("$builder$.RegisterType($argument$)",
                new ExpressionPlaceholder("builder", "Global.Autofac.ContainerBuilder", true),
                new ArgumentPlaceholder("argument"));

        public RegisterTypeNonGeneric()
            : base(pattern, "argument")
        {
        }

        public override IEnumerable<IComponentRegistration>  GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            IStructuralMatchResult match = Match(registrationRootElement);

            if (match.Matched)
            {
                var argument = match.GetMatchedElement(ElementName) as IVBArgument;
                if (argument == null)
                {
                    yield break;
                }

                // match typeof() expressions
                var typeOfExpression = argument.GetExpressionType() as IExpressionType;
                if (typeOfExpression != null)
                {
                    var typeElement = (IDeclaredType)typeOfExpression.ToIType();

                    yield return new ComponentRegistration(registrationRootElement, typeElement.GetTypeElement());
                }
            }
        }
    }
}