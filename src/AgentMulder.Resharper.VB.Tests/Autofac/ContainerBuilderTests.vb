Imports AgentMulder.Resharper.Domain.Containers
Imports AgentMulder.Containers.Autofac
Imports NUnit.Framework
Imports JetBrains.ReSharper.Psi.Tree
Imports JetBrains.ReSharper.Psi.VB.Tree
Imports JetBrains.ReSharper.Psi

Namespace Autofac
    Public Class ContainerBuilderTests
        Inherits ComponentRegistrationsTestBase

        Protected Overrides ReadOnly Property RelativeTestDataPath As String
            Get
                Return "Autofac"
            End Get
        End Property

        Protected Overrides ReadOnly Property ContainerInfo As IContainerInfo
            Get
                Return New AutofacContainerInfo()
            End Get
        End Property

        <TestCase("RegisterTypeGeneric", New String() {"CommonImpl1.vb"})>
        Public Sub DoTest1(testName As String, fileNames As String())
            RunTest(testName, Function(registrations)
                                  Dim codefiles() As IVBFile = fileNames.Select(Function(x) GetCodeFile(x)).ToArray()
                                  Assert.AreEqual(codefiles.Length, registrations.Count())
                                  For Each codeFile In codefiles
                                      codeFile.ProcessChildren(Of ITypeDeclaration)(Function(declaration)
                                                                                        Assert.That(registrations.Any(Function(r) r.Registration.IsSatisfiedBy(declaration.DeclaredElement)))
                                                                                    End Function)
                                  Next
                              End Function)
        End Sub

    End Class
End Namespace