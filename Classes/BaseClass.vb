Public Class BaseClass
    Protected ConnectionString As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
    Public Enum DatabaseOperator
        Equal
        NotEqual
        GreaterThan
        GreaterThanOrEqual
        LessThan
        LessThanOrEqual
    End Enum
    Public Enum SqlCommandType
        [SELECT]
        [INSERT]
        [UPDATE]
        [DELETE]
        [TRUNCATE]
    End Enum
End Class
