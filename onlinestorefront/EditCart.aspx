<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EditCart.aspx.cs" Inherits="onlinestorefront_EditCart" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Confirm Your Order
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <div class="sectHeaderTitle">
                <h2>
                    <asp:Literal ID="lItemsInYourCart" runat="server">Items In Your Cart</asp:Literal></h2>
            </div>
            <asp:Label ID="lblShoppingCartEmpty" runat="server">There are no items in your shopping cart.</asp:Label>
            <asp:GridView ID="gvShoppingCart" runat="server" GridLines="None" Width="800px" AutoGenerateColumns="false"
                OnRowDataBound="gvShoppingCart_RowDataBound" HeaderStyle-HorizontalAlign="Left"
                OnRowCommand="gvShoppingCart_RowCommand" ShowFooter="true" OnRowDeleting="gvShoppingCart_RowDeleting">
                <Columns>
                    <asp:TemplateField HeaderText="Product" HeaderStyle-Width="500px" HeaderStyle-HorizontalAlign="Left"
                        ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="lblProductName" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:TextBox ID="tbQuantity" runat="server" Text="0" Width="30" />
                            <asp:CompareValidator ID="cvQuantity" runat="server" ControlToValidate="tbQuantity"
                                Operator="GreaterThanEqual" ValueToCompare="0" ErrorMessage="One or more item quantities are invalid."
                                Type="Integer" Display="None" />
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Button runat="server" ID="btnUpdate" Text="Update" />
                        </FooterTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Unit Price" HeaderStyle-HorizontalAlign="Left" DataField="UnitPrice"
                        DataFormatString="{0:C}" />
                    <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:Label ID="lblProductPrice" CssClass="price" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:ButtonField ButtonType="Image" Text="Remove" ImageUrl="~/Images/b_shopping_cart_remove.gif"
                        CommandName="Delete" ItemStyle-HorizontalAlign="Right" />
                </Columns>
            </asp:GridView>
        </div>
        <br />
        <asp:Label ID="lblContinueShoppingInstructions" runat="server">
    You can go ahead and checkout now, or leave these items and your cart
    and continue shopping.
        </asp:Label>
        <p />
        <hr style="width: 100%" />
        <div style="text-align: center">
            <asp:Button ID="btnContinue" CausesValidation="false" OnClick="btnContinueShopping_Click"
                runat="server" Text="Continue Shopping" />
                            <asp:Button ID="btnClear" CausesValidation="false" OnClick="btnClear_Click"
                runat="server" Text="Clear Cart" />
            <asp:Button ID="btnCheckout" runat="server" Text="Checkout" OnClick="btnCheckout_Click" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
