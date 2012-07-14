Imports Autofac

Namespace Autofac
    Public Class RegisterTypeGeneric
        Inherits [Module]

        Protected Overrides Sub Load(builder As ContainerBuilder)
            builder.RegisterType(Of CommonImpl1)()
        End Sub

    End Class

    Public Class CommonImpl1
    End Class
End Namespace