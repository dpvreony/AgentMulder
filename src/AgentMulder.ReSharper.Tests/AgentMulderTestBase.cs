using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AgentMulder.ReSharper.Domain.Containers;
using AgentMulder.ReSharper.Plugin.Components;
using AgentMulder.ReSharper.Plugin.Daemon;
using JetBrains.Application;
using JetBrains.DocumentManagers;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;
using FluentAssertions;

namespace AgentMulder.ReSharper.Tests
{
    [TestFixture]
    public abstract class AgentMulderTestBase : BaseTestWithSingleProject
    {
        private static readonly Regex patternCountRegex = new Regex(@"// Patterns: (?<patterns>\d+)");
        private static readonly Regex matchesRegex      = new Regex(@"// Matches: (?<files>.*?)\r?\n");
        private static readonly Regex notMatchesRegex   = new Regex(@"// NotMatches: (?<files>.*?)\r?\n");

        protected abstract IContainerInfo ContainerInfo { get; }

        protected void RunTest(string fileName, Action<IPatternManager> action)
        {
            var typesPath = new DirectoryInfo(Path.Combine(BaseTestDataPath.FullPath, "Types"));
            var fileSet = typesPath.GetFiles("*" + Extension)
                                   .SelectNotNull(fs => fs.FullName)
                                   .Concat(new[] { Path.Combine(SolutionItemsBasePath, fileName) });

            WithSingleProject(fileSet, (lifetime, project) => RunGuarded(() =>
            {
                var solutionAnalyzer = Solution.GetComponent<SolutionAnalyzer>();
                solutionAnalyzer.AddContainer(ContainerInfo);

                var patternManager = Solution.GetComponent<IPatternManager>();

                action(patternManager);
            }));
        }

        protected ICSharpFile GetCodeFile(string fileName)
        {
            IProjectFile projectFile = Project.GetAllProjectFiles(file => file.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (projectFile == null)
                return null;

            IPsiSourceFile psiSourceFile = projectFile.ToSourceFile();
            if (psiSourceFile == null)
                return null;

#if SDK70
            var cSharpFile = psiSourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance) as ICSharpFile;
#else
            var cSharpFile = psiSourceFile.GetPsiFile(CSharpLanguage.Instance) as ICSharpFile;
#endif

            cSharpFile.AssertIsValid();
            
            return cSharpFile;
        }

// ReSharper disable MemberCanBePrivate.Global
        protected IEnumerable TestCases 
// ReSharper restore MemberCanBePrivate.Global
        {
            get
            {
                var testCasesDirectory = new DirectoryInfo(SolutionItemsBasePath);
                var testCases = testCasesDirectory.GetFiles("*" + Extension).Select(info => new TestCaseData(info.Name)).ToList();
                return testCases;
            }
        }

        [TestCaseSource("TestCases")]
        public void Test(string fileName)
        {
            RunTest(fileName, patternManager =>
            {
                ICSharpFile cSharpFile = GetCodeFile(fileName);
                var testData = GetTestData(cSharpFile);

                var patterns = patternManager.GetRegistrationsForFile(cSharpFile.GetSourceFile()).ToList();

                patterns.Count.Should().Be(testData.Item1, 
                    "Mismatched number of expected registrations. Make sure the '// Patterns:' comment is correct");

                if (testData.Item1 > 0)
                {
                    IEnumerable<ICSharpFile> codeFiles = testData.Item2.SelectNotNull(GetCodeFile);
                    foreach (ICSharpFile codeFile in codeFiles)
                    {
                         codeFile.ProcessChildren<ITypeDeclaration>(declaration =>
                             patterns.Should().Contain(r => r.Registration.IsSatisfiedBy(declaration.DeclaredElement),
                             "Of {0} registrations, at least one should match '{1}'", patterns.Count, declaration.CLRName));
                    }
                    codeFiles = testData.Item3.SelectNotNull(GetCodeFile);
                    foreach (ICSharpFile codeFile in codeFiles)
                    {
                         codeFile.ProcessChildren<ITypeDeclaration>(declaration =>
                             patterns.Should().NotContain(r => r.Registration.IsSatisfiedBy(declaration.DeclaredElement),
                             "Of {0} registrations, none should match '{1}'", patterns.Count, declaration.CLRName));
                    }
                }
            });
        }

        private static Tuple<int, string[], string[]> GetTestData(ICSharpFile cSharpFile)
        {
            string code = cSharpFile.GetText();
            var match = patternCountRegex.Match(code); 
            if (!match.Success)
            {
                Assert.Fail("Unable to find number of patterns. Make sure the '// Patterns:' comment is correct");
            }
            
            int count = Convert.ToInt32(match.Groups["patterns"].Value);

            if (count == 0)
            {
                return Tuple.Create(0, EmptyArray<string>.Instance, EmptyArray<string>.Instance);
            }

            match = matchesRegex.Match(code);
            if (!match.Success)
            {
                Assert.Fail("Unable to find matched files. Make sure the '// Matched:' comment is correct");
            }
            
            string[] matches = match.Groups["files"].Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            match = notMatchesRegex.Match(code);
            if (!match.Success)
            {
                Assert.Fail("Unable to find not-matched files. Make sure the '// NotMatched:' comment is correct");
            }
            string[] notMatches = match.Groups["files"].Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            
            return Tuple.Create(count, matches, notMatches);
        }
    }
}