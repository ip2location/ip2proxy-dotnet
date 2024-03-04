Public Class Ref(Of T)
    Private _t As T
    Public Sub New()
    End Sub

    Public Sub New(value As T)
        _t = value
    End Sub

    Public Property Value As T
        Get
            Return _t
        End Get
        Set(value As T)
            _t = value
        End Set
    End Property

End Class