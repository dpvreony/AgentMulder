using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgentMulder.ReSharper.Domain.Containers;
using AgentMulder.ReSharper.Plugin.Components;
using JetBrains.Application.Components;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.VB;
using JetBrains.ReSharper.Psi.VB.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests
{
    [TestFixture]
    [TestFileExtension(VBProjectFileType.VB_EXTENSION)]
    public abstract class ComponentRegistrationsTestBaseVB : BaseTestWithSingleProject
    {
        protected abstract IContainerInfo ContainerInfo { get; }

        protected virtual string RelativeTypesPath
        {
            get { return "..\\Types"; }
        }

        public override ProjectLanguage DefaultProjectLanguage
        {
            get { return ProjectLanguage.VBASIC; }
        }

        protected void RunTest(string testName, Action<IEnumerable<RegistrationInfo>> action)
        {
            string fileName = testName + ".vb";
            var dataPath = new DirectoryInfo(Path.Combine(SolutionItemsBasePath, RelativeTypesPath));
            var fileSet = dataPath.GetFiles("*.vb").SelectNotNull(fileInfo => Path.Combine(RelativeTypesPath, fileInfo.Name)).Concat(new[] { fileName });

            WithSingleProject(fileSet, (lifetime, project) => RunGuarded(() =>
            {
                var searchDomainFactory = ShellInstance.GetComponent<SearchDomainFactory>();
                var patternSearcher = new PatternSearcher(searchDomainFactory);
                var solutionnAnalyzer = new SolutionAnalyzer(patternSearcher);
                solutionnAnalyzer.AddContainer(ContainerInfo);

                var componentRegistrations = solutionnAnalyzer.Analyze();

                action(componentRegistrations);
            }));
        }

        protected IVBFile GetCodeFile(string fileName)
        {
            PsiManager manager = PsiManager.GetInstance(Solution);
            IProjectFile projectFile = Project.GetAllProjectFiles(file => file.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (projectFile == null)
                return null;

            var vbFile = manager.GetPsiFile(projectFile.ToSourceFile(), VBLanguage.Instance) as IVBFile;
            if (vbFile == null || string.IsNullOrEmpty(vbFile.GetText()))
            {
                Assert.Fail("Unable to open the file '{0}', or the file is empty", fileName);
            }

            return vbFile;
        }
    }
}