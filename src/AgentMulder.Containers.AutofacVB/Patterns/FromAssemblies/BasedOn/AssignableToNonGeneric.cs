using System;
using System.Collections.Generic;
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
    internal sealed class AssignableToNonGeneric : MultipleMatchBasedOnPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new VBStructuralSearchPattern("$builder$.AssignableTo($argument$)",
                new ExpressionPlaceholder("builder",
                    "Global.Autofac.Builder.IRegistrationBuilder(Of object,Global.Autofac.Features.Scanning.ScanningActivatorData,Global.Autofac.Builder.DynamicRegistrationStyle)", false),
                new ArgumentPlaceholder("argument"));

        public AssignableToNonGeneric()
            : base(pattern)
        {
        }

        protected override IEnumerable<BasedOnRegistrationBase> DoCreateRegistrations(ITreeNode registrationRootElement, IStructuralMatchResult match)
        {
            var argument = match.GetMatchedElement("argument") as IVBArgument;
            if (argument == null)
            {
                yield break;
            }

            var typeofExpression = argument.GetExpressionType() as IExpressionType;
            if (typeofExpression != null)
            {
                var declaredType = typeofExpression.ToIType() as IDeclaredType;
                if (declaredType != null)
                {
                    ITypeElement typeElement = declaredType.GetTypeElement();
                    if (typeElement != null)
                    {
                        // todo possible bug: same as in the generic variant. Currently works the same as As<T>.
                        yield return new ElementBasedOnRegistration(registrationRootElement, typeElement);
                    }
                }
            }
        }
    }
}