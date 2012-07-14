Imports Autofac
Imports TestApplicationVB.Types

Namespace Autofac
    Public Class RegisterTypeGeneric
        Inherits [Module]

        Protected Overrides Sub Load(builder As ContainerBuilder)
            builder.RegisterType(Of CommonImpl1)()
            builder.Register(Of CommonImpl1)(Function(c, p)
                                                 Dim i As String
                                             End Function)
        End Sub

    End Class
End Namespace