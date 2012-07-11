using System.Collections.Generic;
using System.Linq;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.VB.Tree;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Patterns.FromAssemblies.BasedOn
{
    internal sealed class AsNonGeneric : MultipleMatchBasedOnPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new VBStructuralSearchPattern("$builder$.As($arguments$)",
                new ExpressionPlaceholder("builder",
                    "Global.Autofac.Builder.IRegistrationBuilder(Of object,Global.Autofac.Features.Scanning.ScanningActivatorData,Global.Autofac.Builder.DynamicRegistrationStyle)", false),
                new ArgumentPlaceholder("arguments", -1, -1));

        public AsNonGeneric()
            : base(pattern)
        {
        }

        protected override IEnumerable<BasedOnRegistrationBase> DoCreateRegistrations(ITreeNode registrationRootElement, IStructuralMatchResult match)
        {
            var arguments = match.GetMatchedElementList("arguments").Cast<IVBArgument>();

            foreach (var argument in arguments)
            {
                // match typeof() expressions
                var typeOfExpression = argument.GetExpressionType() as IExpressionType;
                if (typeOfExpression != null)
                {
                    var argumentType = typeOfExpression.ToIType() as IDeclaredType;
                    if (argumentType != null)
                    {
                        var typeElement = argumentType.GetTypeElement();
                        if (typeElement == null) // can happen if the typeof() expression is empty
                        {
                            yield break;
                        }

                        yield return new ElementBasedOnRegistration(registrationRootElement, typeElement);
                    }
                }
            }
        }
    }
}