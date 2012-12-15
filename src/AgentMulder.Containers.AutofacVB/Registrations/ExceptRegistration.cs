using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AutofacVB.Registrations
{
    internal sealed class ExceptRegistration : BasedOnRegistrationBase
    {
        public ExceptRegistration(ITreeNode registrationRootElement, ITypeElement exceptElement)
            : base(registrationRootElement)
        {
            AddFilter(typeElement => !typeElement.Equals(exceptElement));
        }
    }
}