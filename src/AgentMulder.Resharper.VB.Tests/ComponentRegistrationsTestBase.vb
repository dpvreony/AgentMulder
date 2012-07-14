Imports System.IO
Imports AgentMulder.Resharper.Domain.Containers
Imports AgentMulder.ReSharper.Plugin.Components
Imports JetBrains.Application.Components
Imports NUnit.Framework
Imports JetBrains.ReSharper.TestFramework
Imports JetBrains.ReSharper.Psi.Search
Imports JetBrains.Util
Imports JetBrains.ReSharper.Psi
Imports JetBrains.ReSharper.Psi.VB.Tree
Imports JetBrains.ReSharper.Psi.VB
Imports JetBrains.ProjectModel

<TestFixture()>
    Public MustInherit Class ComponentRegistrationsTestBase
    Inherits BaseTestWithSingleProject

    Private _containerInfo As IContainerInfo

    Protected Overridable ReadOnly Property ContainerInfo As IContainerInfo
        Get
            Return _containerInfo
        End Get
    End Property

    Protected Overridable ReadOnly Property RelativeTypesPath As String
        Get
            Return "..\\Types"
        End Get
    End Property

    Protected Sub RunTest(testName As String, action As Action(Of IEnumerable(Of RegistrationInfo)))
        Dim fileName = testName + Extension
        Dim dataPath = New DirectoryInfo(Path.Combine(SolutionItemsBasePath, RelativeTypesPath))
        Dim fileSet = dataPath.GetFiles("*.vb").SelectNotNull(Function(fileInfo) Path.Combine(RelativeTypesPath, fileInfo.Name)).Concat(New IContainerInfo() {fileName})

        WithSingleProject(fileSet, Sub(lifetime, project1) RunGuarded(
            Function()
                Dim searchDomainFactory = ShellInstance.GetComponent(Of SearchDomainFactory)()
                Dim patternSearcher = New PatternSearcher(searchDomainFactory)
                Dim solutionnAnalyzer = New SolutionAnalyzer(patternSearcher)
                solutionnAnalyzer.AddContainer(ContainerInfo)

                Dim componentRegistrations = solutionnAnalyzer.Analyze()

                action(componentRegistrations)
            End Function))
    End Sub

    Protected Function GetCodeFile(fileName As String) As IVBFile
        Dim manager = PsiManager.GetInstance(Solution)
        Dim projectFile = Project.GetAllProjectFiles(Function(file) file.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()
        If projectFile Is Nothing Then Return Nothing

        Dim cSharpFile = manager.GetPsiFile(projectFile.ToSourceFile(), VBLanguage.Instance)
        If cSharpFile Is Nothing OrElse String.IsNullOrEmpty(cSharpFile.GetText()) Then
            Assert.Fail("Unable to open the file '{0}', or the file is empty", fileName)
        End If

        Return cSharpFile
    End Function
End Class
