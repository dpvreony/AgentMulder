using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests.AuotfacVB
{
    [TestFixture]
    public class TestStructuralPatternRegisterType
    {
        [Test]
        public void IfFindsRegisterType()
        {
            var pattern =
            new VBStructuralSearchPattern("$builder$.RegisterType(Of $type$)()",
               new ExpressionPlaceholder("builder", "Global.Autofac.ContainerBuilder"),
                new TypePlaceholder("type"));
            var matcher = pattern.CreateMatcher();

        }
    }
}
