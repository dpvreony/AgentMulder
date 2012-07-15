using Autofac;
using JetBrains.ReSharper.TestFramework;

namespace AgentMulder.ReSharper.Tests.AuotfacVB.Helpers
{
    public class TestAutofacVBAttribute : TestReferencesAttribute
    {
        public override string[] GetReferences()
        {
            return new[]
            {
                typeof(ContainerBuilder).Assembly.Location
            };
        }
    }
}