Imports Autofac
Imports TestApplicationVB.Types

Namespace Autofac
    Public Class RegisterTypeGeneric
        Inherits [Module]

        Protected Overrides Sub Load(builder As ContainerBuilder)
            builder.RegisterType(Of CommonImpl1)()
        End Sub

    End Class
End Namespace