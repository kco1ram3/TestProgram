Imports System.Data.SqlClient

Public Class ProductContextSession
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

    Private ReadOnly Property ProductsDataTable() As DataTable
        Get
            Dim dtProducts As New DataTable
            'dtProducts.Columns.Add("Id", GetType(Integer))
            'dtProducts.Columns.Add("Code")
            'dtProducts.Columns.Add("Name")
            'dtProducts.Columns.Add("Price", GetType(Double))

            For Each c As String In [Enum].GetNames(GetType(Column))
                Dim pro As Reflection.PropertyInfo = [GetType].GetProperties.FirstOrDefault(Function(info)
                                                                                                Return info.Name = c.ToString
                                                                                            End Function)
                If Not IsNothing(pro) Then
                    dtProducts.Columns.Add(c.ToString, pro.PropertyType)
                End If
            Next


            Dim ps As List(Of Product) = Products
            If Not IsNothing(Products) AndAlso Products.Count > 0 Then
                For Each item As Product In Products
                    Dim dr As DataRow
                    dr = dtProducts.NewRow
                    'dr("Id") = item.Id
                    'dr("Code") = item.Code
                    'dr("Name") = item.Name
                    'dr("Price") = item.Price

                    For Each c As String In [Enum].GetNames(GetType(Column))
                        Dim pro As Reflection.PropertyInfo = item.GetType.GetProperties.FirstOrDefault(Function(info)
                                                                                                           Return info.Name = c.ToString
                                                                                                       End Function)
                        If Not IsNothing(pro) Then
                            dr(c.ToString) = pro.GetValue(item)
                        End If
                    Next
                    dtProducts.Rows.Add(dr)
                Next
            End If

            Return dtProducts
        End Get
    End Property

    Private Property Products() As List(Of Product)
        Get
            If IsNothing(HttpContext.Current.Session(TableName)) Then
                Return New List(Of Product)
            Else
                Return HttpContext.Current.Session(TableName)
            End If
        End Get
        Set(value As List(Of Product))
            HttpContext.Current.Session(TableName) = value
        End Set
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
        _products = New DataTable
        _products = ProductsDataTable.Copy
        Dim whereCondition As String = GetSearchCondition()
        If _products.Rows.Count > 0 And whereCondition <> "" Then
            Dim dvFilter As DataView = _products.Copy.AsDataView
            dvFilter.RowFilter = whereCondition
            _products.Rows.Clear()
            If dvFilter.Count > 0 Then
                _products = dvFilter.ToTable()
            End If
        End If
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

#End Region

#Region "Load"
    Public Function LoadAll() As DataTable
        Return ProductsDataTable
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
        Dim p As List(Of Product) = Products

        If Id > 0 Then
            Dim index As Integer = p.FindIndex(Function(item As Product)
                                                   Return item.Id = Product.Id
                                               End Function)
            If index <> -1 Then
                p.Item(index).Code = Code
                p.Item(index).Name = Name
                p.Item(index).Price = Price
            Else
                Throw New Exception("Product not exist !!!")
            End If
        Else
            Dim item As New Product
            With item
                .Id = IIf(p.Count = 0, 1, p?.LastOrDefault?.Id + 1)
                .Code = Code
                .Name = Name
                .Price = Price
            End With
            p.Add(item)
        End If
        Products = p

        Return True
    End Function
#End Region

#Region "Delete"
    Public Function DeleteByPrimaryKey(ByVal _id As Integer) As Boolean
        Dim p As List(Of Product) = Products
        If _id <> -1 Then
            Dim index As Integer = p.FindIndex(Function(item As Product)
                                                   Return item.Id = _id
                                               End Function)
            p.RemoveAt(index)
        Else
            p.Clear()
        End If
        Products = p
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
