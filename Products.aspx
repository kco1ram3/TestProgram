<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Products.aspx.vb" Inherits="TestProgram.Products" EnableEventValidation="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="Scripts/jquery-1.8.0.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.validate.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.validate.unobtrusive.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" ShowFooter="True" ShowHeaderWhenEmpty="True" 
                OnRowEditing="gvProducts_RowEditing" 
                OnRowCancelingEdit="gvProducts_RowCancelingEdit" 
                OnRowCommand="gvProducts_RowCommand" 
                OnRowDeleting="gvProducts_RowDeleting" 
                OnRowUpdating="gvProducts_RowUpdating">
                <Columns>
                    <asp:TemplateField HeaderText="No.">
                        <ItemTemplate>
                            <asp:Label ID="lblNo" runat="server" Text="<%# Container.DataItemIndex + 1 %>" Visible='<%# IIf(IsDBNull(Eval("Id")), False, True) %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="50px" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Code">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditCode" runat="server" Text='<%# Eval("Code")%>'></asp:TextBox>
                            <asp:Panel ID="pnReqEditCode" runat="server">
                                <asp:RequiredFieldValidator ID="reqEditCode" runat="server" 
                                    ErrorMessage="Please enter code" 
                                    ValidationGroup="editProduct" 
                                    ControlToValidate="txtEditCode" 
                                    Display="Dynamic"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>
                            </asp:Panel>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtAddCode" runat="server"></asp:TextBox>
                            <asp:Panel ID="pnReqAddCode" runat="server">
                                <asp:RequiredFieldValidator ID="reqAddCode" runat="server" 
                                    ErrorMessage="Please enter code" 
                                    ValidationGroup="addProduct" 
                                    ControlToValidate="txtAddCode" 
                                    Display="Dynamic"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>
                            </asp:Panel>
                        </FooterTemplate>
                        <ItemTemplate>
                            <%# Eval("Code")%>
                        </ItemTemplate>
                        <FooterStyle VerticalAlign="Top" />
                        <ItemStyle VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditName" runat="server" Text='<%# Eval("Name")%>'></asp:TextBox>
                            <asp:Panel ID="pnReqEditName" runat="server">
                                <asp:RequiredFieldValidator ID="reqEditName" runat="server" 
                                    ErrorMessage="Please enter name" 
                                    ValidationGroup="editProduct" 
                                    ControlToValidate="txtEditName" 
                                    Display="Dynamic"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>
                            </asp:Panel>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtAddName" runat="server"></asp:TextBox>
                            <asp:Panel ID="pnReqAddName" runat="server">
                                <asp:RequiredFieldValidator ID="reqAddName" runat="server" 
                                    ErrorMessage="Please enter name" 
                                    ValidationGroup="addProduct" 
                                    ControlToValidate="txtAddName" 
                                    Display="Dynamic"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>
                            </asp:Panel>
                        </FooterTemplate>
                        <ItemTemplate>
                            <%# Eval("Name")%>
                        </ItemTemplate>
                        <FooterStyle VerticalAlign="Top" />
                        <ItemStyle VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditPrice" runat="server" Text='<%# Eval("Price")%>'></asp:TextBox>
                            <asp:Panel ID="pnReqEditPrice" runat="server">
                                <asp:RequiredFieldValidator ID="reqEditPrice" runat="server" 
                                    ErrorMessage="Please enter price" 
                                    ValidationGroup="editProduct" 
                                    ControlToValidate="txtEditPrice" 
                                    Display="Dynamic"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="regEditPrice" runat="server"
                                    ErrorMessage="Please enter only numeric" 
                                    ValidationGroup="editProduct"
                                    ControlToValidate="txtEditPrice"
                                    Display="Dynamic" 
                                    ForeColor="Red"
                                    ValidationExpression="^\d+(\.\d\d)?$">
                                </asp:RegularExpressionValidator>
                            </asp:Panel>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtAddPrice" runat="server"></asp:TextBox>
                            <asp:Panel ID="pnReqAddPrice" runat="server">
                                <asp:RequiredFieldValidator ID="reqAddPrice" runat="server" 
                                    ErrorMessage="Please enter price" 
                                    ValidationGroup="addProduct" 
                                    ControlToValidate="txtAddPrice" 
                                    Display="Dynamic"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="regAddPrice" runat="server"
                                    ErrorMessage="Please enter only numeric" 
                                    ValidationGroup="addProduct"
                                    ControlToValidate="txtAddPrice"
                                    Display="Dynamic" 
                                    ForeColor="Red"
                                    ValidationExpression="^\d+(\.\d\d)?$">
                                </asp:RegularExpressionValidator>
                            </asp:Panel>
                        </FooterTemplate>
                        <ItemTemplate>
                            <%# Eval("Price")%>
                        </ItemTemplate>
                        <FooterStyle HorizontalAlign="Right" VerticalAlign="Top" />
                        <ItemStyle HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Edit">
                        
                        <FooterTemplate>
                            <asp:Button ID="btnAdd" runat="server" Text="Add" CommandName="Add" ValidationGroup="addProduct" />
                        </FooterTemplate>
                        
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="Edit" Visible='<%# IIf(IsDBNull(Eval("Id")), False, True) %>' />
                        </ItemTemplate>

                        <EditItemTemplate>
                            <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="Update" ValidationGroup="editProduct" CommandArgument='<%# Eval("Id")%>' />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" />
                        </EditItemTemplate>
                        
                        <FooterStyle HorizontalAlign="Center" VerticalAlign="Top" />
                        
                        <ItemStyle HorizontalAlign="Center" Width="100px" Wrap="False" VerticalAlign="Top" />
                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Delete">
                        
                        <ItemTemplate>
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Delete" 
                                Visible='<%# IIf(IsDBNull(Eval("Id")), False, True) %>' 
                                CommandArgument='<%# Eval("Id")%>' 
                                OnClientClick="return confirm('Confirm Delete ?');" />
                        </ItemTemplate>

                        <EditItemTemplate>
                            
                        </EditItemTemplate>
                        
                        <ItemStyle HorizontalAlign="Center" Width="100px" VerticalAlign="Top" />
                        
                    </asp:TemplateField>
                    
                </Columns>

            </asp:GridView>
        </div>
        <%--<asp:ScriptManager ID="ScriptManager1" runat="server" ></asp:ScriptManager>--%>
    </form>
</body>
</html>
