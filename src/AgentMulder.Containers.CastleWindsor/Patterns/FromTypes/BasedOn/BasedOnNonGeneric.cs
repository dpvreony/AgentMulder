using System.Collections.Generic;
using System.Linq;
using AgentMulder.Containers.CastleWindsor.Patterns.FromTypes.WithService;
using AgentMulder.ReSharper.Domain;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.CastleWindsor.Patterns.FromTypes.BasedOn
{
    internal sealed class BasedOnNonGeneric : BasedOnRegistrationBasePattern
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$fromDescriptor$.BasedOn($argument$)",
                                              new ExpressionPlaceholder("fromDescriptor", "Castle.MicroKernel.Registration.FromDescriptor", false),
                                              new ArgumentPlaceholder("argument"));

        public BasedOnNonGeneric(params WithServiceRegistrationBasePattern[] withServicePatterns)
            : base(pattern, withServicePatterns)
        {
        }

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode parentElement)
        {
            IStructuralMatchResult match = Match(parentElement);

            if (match.Matched)
            {
                var argument = match.GetMatchedElement("argument") as ICSharpArgument;
                if (argument != null)
                {
                    var typeofExpression = argument.Value as ITypeofExpression;
                    if (typeofExpression != null)
                    {
                        var declaredType = typeofExpression.ArgumentType as IDeclaredType;
                        if (declaredType != null)
                        {
                            declaredType = declaredType.GetScalarType();
                        }
                        if (declaredType != null)
                        {
                            ITypeElement typeElement = declaredType.GetTypeElement();
                            if (typeElement != null)
                            {
                                var withServiceRegistrations = base.GetComponentRegistrations(parentElement).OfType<WithServiceRegistration>();

                                yield return new BasedOnRegistration(match.GetDocumentRange(), typeElement, withServiceRegistrations);
                            }
                        }
                    }
                }
            }
        }
    }
}