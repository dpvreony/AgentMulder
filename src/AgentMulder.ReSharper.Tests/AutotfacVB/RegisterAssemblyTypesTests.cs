using System.Linq;
using AgentMulder.Containers.Autofac;
using AgentMulder.Containers.AutofacVB;
using AgentMulder.ReSharper.Domain.Containers;
using AgentMulder.ReSharper.Tests.AuotfacVB.Helpers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB.Tree;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests.AuotfacVB
{
    [TestAutofacVB]
    public class RegisterAssemblyTypesTests : ComponentRegistrationsTestBaseVB
    {
        protected override string RelativeTestDataPath
        {
            get { return @"VB\Autofac\RegisterAssemblyTypesTests"; }
        }

        protected override string RelativeTypesPath
        {
            get { return @"..\..\VB\Types"; }
        }

        protected override IContainerInfo ContainerInfo
        {
            get { return new AutofacVBContainerInfo(); }
        }

        [TestCase("NoAssembly")]
        public void Sanity(string testName)
        {
            RunTest(testName, registrations => Assert.AreEqual(0, registrations.Count()));
        }

        [TestCase("FromThisAssemblyModuleProperty", new[] { "Foo.vb", "Bar.vb", "Baz.vb", "CommonImpl1.vb" })]
        [TestCase("FromGetExecutingAssembly", new[] { "Foo.vb", "Bar.vb", "Baz.vb", "CommonImpl1.vb" })]
        [TestCase("FromAssemblyTypeOf", new[] { "Foo.vb", "Bar.vb", "Baz.vb", "CommonImpl1.vb" })]
        [TestCase("AllThreeTogether", new[] { "Foo.vb", "Bar.vb", "Baz.vb", "CommonImpl1.vb" })]
        [TestCase("AsGeneric1", new[] { "CommonImpl1.vb" })]
        [TestCase("AsGeneric2", new[] { "CommonImpl12.vb" })]
        [TestCase("AsNonGeneric1", new[] { "CommonImpl1.vb" })]
        [TestCase("AsNonGeneric2", new[] { "CommonImpl12.vb" })]
        [TestCase("AsImplementedInterfaces", new[] { "Foo.vb", "Bar.vb", "Baz.vb", "CommonImpl1.vb" })]
        [TestCase("AssignableToGeneric1", new[] { "StandardCard.vb", "GoldCard.vb" })]
        [TestCase("AssignableToGeneric2", new[] { "CommonImpl12.vb" })]
        [TestCase("AssignableToNonGeneric1", new[] { "StandardCard.vb", "GoldCard.vb" })]
        [TestCase("AssignableToNonGeneric2", new[] { "CommonImpl12.vb" })]
        [TestCase("Except1", new[] { "StandardCard.vb" })]
        [TestCase("Except2", new[] { "PlatinumCard.vb" })]
        [TestCase("ExceptWithArgument", new[] { "StandardCard.vb" })]
        [TestCase("InNamespaceOfType", new[] { "InSomeNamespace.vb", "InSomeOtherNamespace.vb" })]
        [TestCase("InNamespaceString", new[] { "InSomeNamespace.vb", "InSomeOtherNamespace.vb" })]
        public void DoTest(string testName, string[] fileNames)
        {
            RunTest(testName, registrations =>
            {
                IVBFile[] codeFiles = fileNames.Select(GetCodeFile).ToArray();

                CollectionAssert.IsNotEmpty(registrations);
                foreach (var codeFile in codeFiles)
                {
                    codeFile.ProcessChildren<ITypeDeclaration>(declaration =>
                        Assert.That(registrations.Any((r => r.Registration.IsSatisfiedBy(declaration.DeclaredElement)))));
                }
            });
        }

        [TestCase("AsGeneric1", new[] { "Foo.vb" })]
        [TestCase("AsGeneric2", new[] { "CommonImpl1.vb" })]
        [TestCase("AsNonGeneric1", new[] { "Foo.vb" })]
        [TestCase("AsNonGeneric2", new[] { "CommonImpl1.vb" })]
        [TestCase("AsImplementedInterfaces", new[] { "GoldCard.vb" })]
        [TestCase("AssignableToGeneric1", new[] { "Foo.vb" })]
        [TestCase("AssignableToGeneric1", new[] { "CommonImpl.vb" })]
        [TestCase("AssignableToNonGeneric1", new[] { "Foo.vb" })]
        [TestCase("AssignableToNonGeneric2", new[] { "CommonImpl.vb" })]
        [TestCase("Except1", new[] { "GoldCard.vb" })]
        [TestCase("Except2", new[] { "GoldCard.vb", "StandardCard.vb" })]
        [TestCase("ExceptWithArgument", new[] { "GoldCard.vb" })]
        [TestCase("InNamespaceOfType", new[] { "Foo.vb" })]
        [TestCase("InNamespaceString", new[] { "Foo.vb" })]
        public void ExcludeTest(string testName, string[] fileNamesToExclude)
        {
            RunTest(testName, registrations =>
            {
                IVBFile[] codeFiles = fileNamesToExclude.Select(GetCodeFile).ToArray();

                CollectionAssert.IsNotEmpty(registrations);
                foreach (var codeFile in codeFiles)
                {
                    codeFile.ProcessChildren<ITypeDeclaration>(declaration =>
                        Assert.That(registrations.All((r => !r.Registration.IsSatisfiedBy(declaration.DeclaredElement)))));
                }
            });
        }
    }
}