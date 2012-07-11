using System.Linq;
using AgentMulder.Containers.Autofac;
using AgentMulder.ReSharper.Domain.Containers;
using AgentMulder.ReSharper.Tests.AuotfacVB.Helpers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB.Tree;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests.AuotfacVB
{
    [TestAutofacVB]
    public class ContainerBuilderTests : ComponentRegistrationsTestBaseVB
    {
        protected override string RelativeTestDataPath
        {
            get { return @"VB\Autofac"; }
        }

        protected override IContainerInfo ContainerInfo
        {
            get { return new AutofacContainerInfo(); }
        }

        [TestCase("RegisterTypeGeneric", new[] { "CommonImpl1.vb" })]
        [TestCase("RegisterTypeNonGeneric", new[] { "CommonImpl1.vb" })]
        [TestCase("RegisterWithLambda", new[] { "Foo.vb" })]
        [TestCase("RegisterWithLambdaTakesDependency", new[] { "TakesDependency.vb" })]
        [TestCase("RegisterWithLambdaInitializer", new[] { "FooBar.vb" })]
        [TestCase("RegisterComplex", new[] { "GoldCard.cs", "StandardCard.vb" })]
        [TestCase("RegisterComplexWithVariable", new[] { "GoldCard.vb" })]
        public void DoTest(string testName, string[] fileNames)
        {
            RunTest(testName, registrations =>
            {
                IVBFile[] codeFiles = fileNames.Select(GetCodeFile).ToArray();

                Assert.AreEqual(codeFiles.Length, registrations.Count());
                foreach (var codeFile in codeFiles)
                {
                    codeFile.ProcessChildren<ITypeDeclaration>(declaration =>
                        Assert.That(registrations.Any((r => r.Registration.IsSatisfiedBy(declaration.DeclaredElement)))));
                }
            });
        }
    }
}