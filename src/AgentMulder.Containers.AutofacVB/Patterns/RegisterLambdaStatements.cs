using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AgentMulder.Containers.AutofacVB.Patterns.Helpers;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.VB.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve.Managed;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    internal sealed class RegisterLambdaStatements : RegistrationPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new VBStructuralSearchPattern("$builder$.Register(Function($args$)  $statements$ End Function)",
                new ExpressionPlaceholder("builder", "Global.Autofac.ContainerBuilder", true),
                new ArgumentPlaceholder("args", -1, -1),
                new StatementPlaceholder("statements", -1, -1));

        public RegisterLambdaStatements()
            : base(pattern)
        {
        }

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            IStructuralMatchResult match = Match(registrationRootElement);

            if (match.Matched)
            {
                var statements = match.GetMatchedElementList("statements").Cast<IVBStatement>();

                var collectedTypes = statements.SelectMany(statement =>
                {
                    var returnTypeCollector = new ReturnTypeCollector(new UniversalContext(statement.GetPsiModule()));
                    statement.ProcessThisAndDescendants(returnTypeCollector);
                    return returnTypeCollector.CollectedTypes;
                });

                foreach (var type in collectedTypes)
                {
                    var declaredType = type as IDeclaredType;
                    if (declaredType != null)
                    {
                        var typeElement = declaredType.GetTypeElement();
                        if (typeElement != null)
                        {
                            yield return new ElementBasedOnRegistration(registrationRootElement, typeElement);
                        }
                    }
                }
            }
        }
    }
}