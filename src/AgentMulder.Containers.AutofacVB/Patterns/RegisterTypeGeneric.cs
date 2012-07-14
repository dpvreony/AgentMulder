using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    internal sealed class RegisterTypeGeneric : ComponentImplementationPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new VBStructuralSearchPattern("$builder$.RegisterType(Of $type$)()",
                new ExpressionPlaceholder("builder", "Global.Autofac.ContainerBuilder"),
                new TypePlaceholder("type"));

        public RegisterTypeGeneric()
            : base(pattern, "type")
        {
        }

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            foreach (var registration in base.GetComponentRegistrations(registrationRootElement).Cast<ComponentRegistration>())
            {
                registration.Implementation = registration.ServiceType;

                yield return registration;
            }
        }
    }
}