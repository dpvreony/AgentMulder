using System.Collections.Generic;
using AgentMulder.Containers.AutofacVB.Registrations;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Patterns.FromAssemblies.BasedOn
{
    internal sealed class Except : MultipleMatchBasedOnPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new VBStructuralSearchPattern("$builder$.Except(Of $type$)($arguments$)",
                new ExpressionPlaceholder("builder",
                    "Global.Autofac.Builder.IRegistrationBuilder(Of object,Global.Autofac.Features.Scanning.ScanningActivatorData,Global.Autofac.Builder.DynamicRegistrationStyle)", false),
                new TypePlaceholder("type"),
                new ArgumentPlaceholder("arguments", -1 ,-1));

        public Except()
            : base(pattern)
        {
        }

        protected override IEnumerable<BasedOnRegistrationBase> DoCreateRegistrations(ITreeNode registrationRootElement, IStructuralMatchResult match)
        {
            var matchedType = match.GetMatchedType("type") as IDeclaredType;
            if (matchedType != null)
            {
                ITypeElement typeElement = matchedType.GetTypeElement();
                if (typeElement != null)
                {
                    yield return new ExceptRegistration(registrationRootElement, typeElement);
                }
            }
        }
    }
}