Public Class Products
    Inherits System.Web.UI.Page

#Region "Properties"
    Private ReadOnly Property GetProductContext() As Object
        Get
            If AppSettings.UseSessionForStorageData Then
                Return New ProductContextSession
            Else
                Return New ProductContext
            End If
        End Get
    End Property
#End Region

#Region "Page Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            BindData()
        End If
    End Sub
#End Region

#Region "Functions"
    Protected Sub BindData()
        Dim dtProducts As New DataTable
        'dtProducts.Columns.Add("Id", GetType(Integer))
        'dtProducts.Columns.Add("Code")
        'dtProducts.Columns.Add("Name")
        'dtProducts.Columns.Add("Price", GetType(Double))

        'Test Insert Multi Record
        'For record As Integer = 1 To 10
        '    Dim insert As Object = GetProductContext()
        '    With insert
        '        .Code = "IP-" & ("00000" & record).Substring(("00000" & record).Length - 5)
        '        .Name = "Insert Product No. " & record
        '        .Price = record * 10
        '        .Save()
        '    End With
        'Next

        'Test Update Multi Record
        'Dim no As Integer = 1
        'Dim update As New ProductContext
        'With update
        '    If .Query Then
        '        Do
        '            .Code = "UP-" & ("00000" & no).Substring(("00000" & no).Length - 5)
        '            .Name = "Update Product No. " & no
        '            .Price = no * 10
        '            .Save()
        '            no += 1
        '        Loop While .NextRecord
        '    End If
        'End With

        'Test Delete Multi Record
        'Dim delete As New ProductContext
        'delete.Truncate()
        'delete.DeleteAll()
        'With delete
        '    If .Query Then
        '        Do
        '            .Delete()
        '        Loop While .NextRecord
        '    End If
        'End With

        Dim product As Object = GetProductContext()
        'Dim products As List(Of Product) = product.LoadAll
        'If Not IsNothing(products) AndAlso products.Count > 0 Then
        '    For Each item As Product In products
        '        dtProducts.Rows.Add(item.Id, item.Code, item.Name, item.Price)
        '    Next
        'Else
        '    dtProducts.Rows.Add(dtProducts.NewRow())
        'End If
        dtProducts = product.LoadAll
        If dtProducts.Rows.Count = 0 Then
            dtProducts.Rows.Add(dtProducts.NewRow())
        End If

        gvProducts.DataSource = dtProducts
        gvProducts.DataBind()
    End Sub
#End Region

#Region "GridView Events"
    Protected Sub gvProducts_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvProducts.EditIndex = e.NewEditIndex
        BindData()
    End Sub

    Protected Sub gvProducts_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvProducts.EditIndex = -1
        BindData()
    End Sub

    Protected Sub gvProducts_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Select Case e.CommandName
            Case "Add"
                Dim txtAddCode As TextBox = gvProducts.FooterRow.FindControl("txtAddCode")
                Dim txtAddName As TextBox = gvProducts.FooterRow.FindControl("txtAddName")
                Dim txtAddPrice As TextBox = gvProducts.FooterRow.FindControl("txtAddPrice")

                Try
                    Dim product As Object = GetProductContext()
                    With product
                        .Where.Code = txtAddCode.Text.Trim
                        If .Query Then
                            Response.Write("<script>alert('Product code duplicate !!!');</script>")
                        Else
                            .Code = txtAddCode.Text.Trim
                            .Name = txtAddName.Text.Trim
                            .Price = CDbl(txtAddPrice.Text.Trim)
                            .Save()

                            txtAddCode.Text = ""
                            txtAddName.Text = ""
                            txtAddPrice.Text = ""

                            Response.Write("<script>alert('Added');</script>")
                            BindData()
                        End If
                    End With
                Catch ex As Exception
                    Response.Write("<script>alert('" & ex.Message & "');</script>")
                End Try
            Case "Update"
                Dim id As Integer = e.CommandArgument
                Dim txtEditCode As TextBox = gvProducts.Rows(sender.EditIndex).FindControl("txtEditCode")
                Dim txtEditName As TextBox = gvProducts.Rows(sender.EditIndex).FindControl("txtEditName")
                Dim txtEditPrice As TextBox = gvProducts.Rows(sender.EditIndex).FindControl("txtEditPrice")

                Try
                    Dim product As Object = GetProductContext()
                    With product
                        .Where.Id = id
                        .WhereOperator(TestProgram.Product.Column.Id, BaseClass.DatabaseOperator.NotEqual)
                        .Where.Code = txtEditCode.Text.Trim
                        If .Query Then
                            Response.Write("<script>alert('Product code duplicate !!!');</script>")
                        Else
                            If .LoadByPrimaryKey(id) Then
                                .Code = txtEditCode.Text.Trim
                                .Name = txtEditName.Text.Trim
                                .Price = txtEditPrice.Text.Trim
                                .Save()

                                Response.Write("<script>alert('Updated');</script>")
                            Else
                                Response.Write("<script>alert('Product not exist !!!');</script>")
                            End If
                            gvProducts.EditIndex = -1
                            BindData()
                        End If
                    End With
                Catch ex As Exception
                    Response.Write("<script>alert('" & ex.Message & "');</script>")
                End Try
            Case "Delete"
                Dim id As Integer = e.CommandArgument
                Dim product As Object = GetProductContext()
                If product.LoadByPrimaryKey(id) Then
                    product.Delete()
                    Response.Write("<script>alert('Deleted');</script>")
                Else
                    Response.Write("<script>alert('Product not exist !!!');</script>")
                End If
                BindData()
        End Select
    End Sub

    Protected Sub gvProducts_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

    End Sub

    Protected Sub gvProducts_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)

    End Sub
#End Region

End Class