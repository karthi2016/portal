<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="CrossSellItems.aspx.cs" Inherits="orders_CrossSellItems" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Additional Items You Might Need
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server">
        Based on what you've ordered, we thought there might be a few other items you'd be interested in:
    </asp:Literal>
    <table style="width: 800px; margin-top: 20px">
        <tr class="rowHead">
            <th>
            </th>
            <th style="text-align: left">
                <asp:Literal ID="lQtyHeader" runat="Server">Qty</asp:Literal>
            </th>
            <th style="text-align: left">
                <asp:Literal ID="lProduct" runat="Server">Product</asp:Literal>
            </th>
            <th style="text-align: left">
                <asp:Literal ID="lPrice" runat="Server">Price</asp:Literal>
            </th>
            <th>
            </th>
        </tr>
        <asp:Repeater ID="rptItems" runat="server" OnItemDataBound="rptItems_ItemDataBound" OnItemCommand="rptItems_Command">
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Image ID="imgProduct" runat="server" Visible="false" />
                    </td>
                    <td style="width: 70px">
                        <asp:TextBox ID="tbQuantity" runat="server" Text="1" Width="40px" />
                    </td>
                    <td>
                    <ASP:HiddenField ID="hfProductID" runat="server" />
                    <ASP:HiddenField ID="hfPrice" runat="server" />
                        <asp:Literal ID="lProductName" runat="server" Text="Product Name" />
                    </td>
                    <td>
                        <asp:Label ID="lblProductPrice" runat="server" />
                    </td>
                    <td style="width: 90px">
                        <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    <br />
    <asp:Panel ID="pnlItems" runat="server" Visible="false">
    <h2>Items You've Added</h2>
    Here are the items you've added so far:
       <asp:GridView ID="gvShoppingCart" runat="server" GridLines="None" Width="800px" AutoGenerateColumns="false"
            OnRowDataBound="gvShoppingCart_RowDataBound" OnRowCommand="gvShoppingCart_OnRowCommand"
            HeaderStyle-HorizontalAlign="Left">
            <Columns>
                
                <asp:TemplateField HeaderText="Product" HeaderStyle-Width="500px" HeaderStyle-HorizontalAlign="Left"
                    ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>                        <asp:Label ID="lblProductName" runat="server" />
                      
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Quantity" HeaderStyle-HorizontalAlign="Left" DataField="Quantity"
                    DataFormatString="{0:F2}" />
                <asp:BoundField HeaderText="Unit Price" HeaderStyle-HorizontalAlign="Left" DataField="UnitPrice"
                    DataFormatString="{0:C}" />
                <asp:BoundField HeaderText="Total" HeaderStyle-HorizontalAlign="Left" DataField="Total"
                    DataFormatString="{0:C}" />
                <asp:ButtonField ButtonType="Button" Text="Remove" CommandName="Delete" ItemStyle-HorizontalAlign="Right"
                   />
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <hr />
    <div align="right">
        <asp:Button ID="btnContinue" runat="server" Text="Continue Order" OnClick="btnContinue_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
