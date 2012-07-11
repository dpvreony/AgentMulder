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
    internal sealed class InNamespaceString : MultipleMatchBasedOnPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new VBStructuralSearchPattern("$builder$.InNamespace($argument$)",
                new ExpressionPlaceholder("builder",
                    "Global.Autofac.Builder.IRegistrationBuilde(Of object,Global.Autofac.Features.Scanning.ScanningActivatorData,Global.Autofac.Builder.DynamicRegistrationStyle)", false),
                new ArgumentPlaceholder("argument"));

        public InNamespaceString()
            : base(pattern)
        {
        }

        protected override IEnumerable<BasedOnRegistrationBase> DoCreateRegistrations(ITreeNode registrationRootElement, IStructuralMatchResult match)
        {
            var argument = match.GetMatchedElement("argument") as IVBArgument;
            if (argument != null)
            {
                INamespace @namespace = ReSharper.Domain.Utils.PsiExtensions.GetNamespaceDeclaration(argument.Expression as IVBExpression);
                if (@namespace != null)
                {
                    yield return new InNamespaceRegistration(registrationRootElement, @namespace, true);
                }
            }
        }
    }
}