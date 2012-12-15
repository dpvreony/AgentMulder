using System.Collections.Generic;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Patterns.FromAssemblies.BasedOn
{
    internal sealed class AssignableToGeneric : MultipleMatchBasedOnPatternBase
    {
        private static readonly IStructuralSearchPattern pattern = 
            new VBStructuralSearchPattern("$builder$.AssignableTo(Of $service$)()",
                new ExpressionPlaceholder("builder",
                    "Global.Autofac.Builder.IRegistrationBuilder(Of object,Global.Autofac.Features.Scanning.ScanningActivatorData,Global.Autofac.Builder.DynamicRegistrationStyle)", false),
                new TypePlaceholder("service"));

        public AssignableToGeneric()
            : base(pattern)
        {
        }

        protected override IEnumerable<BasedOnRegistrationBase> DoCreateRegistrations(ITreeNode registrationRootElement, IStructuralMatchResult match)
        {
            var matchedType = match.GetMatchedType("service") as IDeclaredType;
            if (matchedType != null)
            {
                ITypeElement typeElement = matchedType.GetTypeElement();
                if (typeElement != null)
                {
                    // todo possible bug here. Investigate wheather As and AssignableTo differ somehow in the result.
                    yield return new ElementBasedOnRegistration(registrationRootElement, typeElement);
                }
            }
        }
    }
}