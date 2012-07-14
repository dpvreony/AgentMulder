using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using AgentMulder.ReSharper.Domain.Containers;

namespace AgentMulder.Containers.AutofacVB
{
    [Export(typeof(IContainerInfo))]
    public class AutofacVBContainerInfo : ContainerInfoBase
    {
        public override string ContainerDisplayName
        {
            get { return "AutofacVB"; }
        }

        protected override ComposablePartCatalog GetComponentCatalog()
        {
            return new AssemblyCatalog(Assembly.GetExecutingAssembly());
        }
    }
}