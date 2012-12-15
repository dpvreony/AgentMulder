Imports JetBrains.Application
Imports NUnit.Framework
Imports System.Reflection
Imports JetBrains.Threading

<SetUpFixture()>
Public Class TestEnvironmentAssembly
    Inherits ReSharperTestEnvironmentAssembly

    Private Shared Function GetAssembliesToLoad() As IEnumerable(Of Assembly)
        Return Assembly.GetExecutingAssembly()
    End Function

    Public Overrides Sub SetUp()
        MyBase.SetUp()
        ReentrancyGuard.Current.Execute("LoadAssemblies", Function()

                                                          End Function)
        Shell.Instance.GetComponent(Of AssemblyManager)().LoadAssemblies(GetType().Name, GetAssembliesToLoad())
    End Sub

    Public Overrides Sub TearDown()

    ReentrancyGuard.Current.Execute("UnloadAssemblies",function() Shell.Instance.GetComponent(of assemblyManager)().UnloadAssemblies(GetType().Name, GetAssembliesToLoad()))
        MyBase.TearDown()
    End Sub
End Class