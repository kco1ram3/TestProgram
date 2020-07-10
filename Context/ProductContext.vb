Imports System.Data.SqlClient

Public Class ProductContext
    Inherits Product

#Region "Variables"
    Private _product As Product
    Private _products As DataTable
    Private TableName As String = "Products"
    Private ConditionsOperator As Hashtable
    Private CurrentRow As Integer = 0
#End Region

#Region "Properties"
    Public Property Product() As Product
        Get
            Return _product
        End Get
        Set(value As Product)
            _product = value
            Id = value.Id
            Code = value.Code
            Name = value.Name
            Price = value.Price
        End Set
    End Property

    Public Property Where() As Product

    Private ReadOnly Property Products() As DataTable
        Get
            WhereClearSearchCondition()
            Query()
            Return _products
        End Get
    End Property
#End Region

#Region "Constructors"
    Public Sub New()
        Where = New Product
        ConditionsOperator = New Hashtable
        For Each c As String In [Enum].GetNames(GetType(Column))
            With ConditionsOperator
                .Add(c, DatabaseOperator.Equal)
            End With
        Next
    End Sub
#End Region

#Region "Query"
    Public Function Query() As Boolean
        CurrentRow = 0
        Dim conn As SqlConnection = New SqlConnection(ConnectionString)
        Dim adapter As New SqlDataAdapter
        conn.Open()
        adapter.SelectCommand = New SqlCommand(GetSqlCommand(SqlCommandType.SELECT), conn)
        _products = New DataTable
        adapter.Fill(_products)
        conn.Close()
        If _products.Rows.Count > 0 Then
            Dim p As New Product
            p.Id = _products.Rows(CurrentRow).Field(Of Integer)(Column.Id.ToString)
            p.Code = _products.Rows(CurrentRow).Field(Of String)(Column.Code.ToString)
            p.Name = _products.Rows(CurrentRow).Field(Of String)(Column.Name.ToString)
            p.Price = _products.Rows(CurrentRow).Field(Of Double)(Column.Price.ToString)
            CurrentRow += 1
            Product = p
            Return True
        Else
            Return False
        End If
    End Function

    Public Function NextRecord() As Boolean
        If _products.Rows.Count > CurrentRow Then
            Dim p As New Product
            p.Id = _products.Rows(CurrentRow).Field(Of Long)(Column.Id.ToString)
            p.Code = _products.Rows(CurrentRow).Field(Of String)(Column.Code.ToString)
            p.Name = _products.Rows(CurrentRow).Field(Of String)(Column.Name.ToString)
            p.Price = _products.Rows(CurrentRow).Field(Of Decimal)(Column.Price.ToString)
            CurrentRow += 1
            Product = p
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetSearchCondition() As String
        Dim listCondition As New List(Of String)
        With Where
            If Not IsNothing(.Id) AndAlso .Id > 0 Then
                listCondition.Add(Column.Id.ToString & " " & GetOperatorSymbol(ConditionsOperator(Column.Id.ToString)) & " " & .Id)
            End If
            If Not IsNothing(.Code) AndAlso .Code.Trim <> "" Then
                listCondition.Add(Column.Code.ToString & " " & GetOperatorSymbol(ConditionsOperator(Column.Code.ToString)) & " '" & .Code.Trim & "'")
            End If
            If Not IsNothing(.Name) AndAlso .Name.Trim <> "" Then
                listCondition.Add(Column.Name.ToString & " " & GetOperatorSymbol(ConditionsOperator(Column.Name.ToString)) & " '" & .Name.Trim & "'")
            End If
            If Not IsNothing(.Price) AndAlso .Price > 0 Then
                listCondition.Add(Column.Price.ToString & " " & GetOperatorSymbol(ConditionsOperator(Column.Price.ToString)) & " " & .Price)
            End If
        End With
        If listCondition.Count > 0 Then
            Return String.Join(" AND ", listCondition)
        End If
        Return ""
    End Function

    Public Sub WhereOperator(ByVal c As Column, ByVal o As DatabaseOperator)
        ConditionsOperator(c.ToString) = o
    End Sub

    Private Function GetOperatorSymbol(ByVal o As DatabaseOperator) As String
        Select Case o
            Case DatabaseOperator.Equal
                Return "="
            Case DatabaseOperator.NotEqual
                Return "<>"
            Case DatabaseOperator.GreaterThan
                Return ">"
            Case DatabaseOperator.GreaterThanOrEqual
                Return ">="
            Case DatabaseOperator.LessThan
                Return "<"
            Case DatabaseOperator.LessThanOrEqual
                Return "<="
            Case Else
                Return ""
        End Select
    End Function

    Public Sub WhereClearSearchCondition()
        Where.Id = Nothing
        Where.Code = Nothing
        Where.Name = Nothing
        Where.Price = Nothing
        For Each c As String In [Enum].GetNames(GetType(Column))
            ConditionsOperator(c) = DatabaseOperator.Equal
        Next
    End Sub

    Private Function GetSqlCommand(ByVal type As SqlCommandType, Optional ByVal _id As Integer = -1) As String
        Dim sql As String = ""
        Select Case type
            Case SqlCommandType.SELECT
                sql = "SELECT "
                Dim columns As String = ""
                For Each c As String In [Enum].GetNames(GetType(Column))
                    columns += "," & c.ToString
                Next
                sql += columns.Substring(1) & " FROM " & TableName
                Dim whereCondition As String = GetSearchCondition()
                If whereCondition <> "" Then
                    sql += " WHERE " & whereCondition
                End If
            Case SqlCommandType.INSERT
                sql = "INSERT INTO " & TableName
                Dim columns As String = ""
                Dim values As String = ""
                For Each c As String In [Enum].GetNames(GetType(Column))
                    If c.ToString = Column.Id.ToString Then
                        Continue For
                    End If
                    columns += "," & c.ToString
                    Dim pro As Reflection.PropertyInfo = [GetType].GetProperties.FirstOrDefault(Function(info)
                                                                                                    Return info.Name = c.ToString
                                                                                                End Function)
                    If Not IsNothing(pro) Then
                        Select Case pro.PropertyType
                            Case GetType(String)
                                values += ",'" & pro.GetValue(Me) & "'"
                            Case GetType(Double)
                                values += "," & pro.GetValue(Me)
                        End Select
                    End If
                Next
                sql += "(" & columns.Substring(1) & ")"
                sql += "VALUES(" & values.Substring(1) & ");"
            Case SqlCommandType.UPDATE
                sql = "UPDATE " & TableName
                Dim columns As String = ""
                For Each c As String In [Enum].GetNames(GetType(Column))
                    If c.ToString = Column.Id.ToString Then
                        Continue For
                    End If
                    columns += "," & c.ToString
                    Dim pro As Reflection.PropertyInfo = [GetType].GetProperties.FirstOrDefault(Function(info)
                                                                                                    Return info.Name = c.ToString
                                                                                                End Function)
                    If Not IsNothing(pro) Then
                        Select Case pro.PropertyType
                            Case GetType(String)
                                columns += "='" & pro.GetValue(Me) & "'"
                            Case GetType(Double)
                                columns += "=" & pro.GetValue(Me)
                        End Select
                    End If
                Next
                sql += " SET " & columns.Substring(1)
                sql += " WHERE " & Column.Id.ToString & " = " & Product.Id & ";"
            Case SqlCommandType.DELETE
                sql = "DELETE FROM " & TableName
                If _id <> -1 Then
                    sql += " WHERE " & Column.Id.ToString & " = " & _id
                End If
            Case SqlCommandType.TRUNCATE
                sql = "TRUNCATE TABLE " & TableName
        End Select
        Return sql
    End Function
#End Region

#Region "Load"
    Public Function LoadAll() As DataTable
        Return Products
    End Function

    Public Function LoadByPrimaryKey(ByVal _id As Integer) As Boolean
        WhereClearSearchCondition()
        Where.Id = _id
        If Query() Then
            Return True
        Else
            Return False
        End If
    End Function
#End Region

#Region "Save"
    Public Function Save() As Boolean
        If Id > 0 Then
            Dim conn As SqlConnection = New SqlConnection(ConnectionString)
            Dim adapter As New SqlDataAdapter
            conn.Open()
            adapter.UpdateCommand = New SqlCommand(GetSqlCommand(SqlCommandType.UPDATE), conn)
            adapter.UpdateCommand.ExecuteNonQuery()
            conn.Close()
        Else
            Dim conn As SqlConnection = New SqlConnection(ConnectionString)
            Dim adapter As New SqlDataAdapter
            conn.Open()
            adapter.InsertCommand = New SqlCommand(GetSqlCommand(SqlCommandType.INSERT), conn)
            adapter.InsertCommand.ExecuteNonQuery()
            conn.Close()
        End If
        Return True
    End Function
#End Region

#Region "Delete"
    Public Function Truncate() As Boolean
        Dim conn As SqlConnection = New SqlConnection(ConnectionString)
        Dim adapter As New SqlDataAdapter
        conn.Open()
        adapter.DeleteCommand = New SqlCommand(GetSqlCommand(SqlCommandType.TRUNCATE), conn)
        adapter.DeleteCommand.ExecuteNonQuery()
        conn.Close()
        Return True
    End Function

    Public Function DeleteByPrimaryKey(ByVal _id As Integer) As Boolean
        Dim conn As SqlConnection = New SqlConnection(ConnectionString)
        Dim adapter As New SqlDataAdapter
        conn.Open()
        adapter.DeleteCommand = New SqlCommand(GetSqlCommand(SqlCommandType.DELETE, _id), conn)
        adapter.DeleteCommand.ExecuteNonQuery()
        conn.Close()
        Return True
    End Function

    Public Function Delete() As Boolean
        Return DeleteByPrimaryKey(Product.Id)
    End Function

    Public Function DeleteAll() As Boolean
        Return DeleteByPrimaryKey(-1)
    End Function
#End Region

End Class
