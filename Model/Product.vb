Public Class Product
    Inherits BaseClass

    Public Enum Column
        Id
        Code
        Name
        Price
    End Enum

    Public Property Id() As Integer
    Public Property Code() As String
    Public Property Name() As String
    Public Property Price() As Double
End Class
