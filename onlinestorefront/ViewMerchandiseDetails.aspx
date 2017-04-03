<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="ViewMerchandiseDetails.aspx.cs" Inherits="onlinestorefront_ViewMerchandiseDetails" %>

<%@ Import Namespace="System.Data" %>
<%@ Reference Page="~/onlinestorefront/BrowseMerchandise.aspx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
    <link rel="stylesheet" type="text/css" href="/images/onlinestorefront.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlAllMerchandise" NavigateUrl="~/onlinestorefront/BrowseMerchandise.aspx"
        Text="All Merchandise" />
    <asp:HyperLink runat="server" ID="hlCategory" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Browse Merchandise
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
    <table>
        <tr>
            <td align="right">
                <ul class="buttonList">
                    <li><a class="iconBtn iconBtnCart" href="/onlinestorefront/EditCart.aspx"><span>View
                        Shopping Cart (<%= MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.Count()%>)</span></a></li>
                </ul>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server"/>
    <div id="colLeftLrg">
        <!-- left column -->
        <div class="sectContLrg productListing">
            <asp:Image ID="imgMerchandise" runat="server" />
            <div class="productListItemInfo" >
                    <h1>
                         <asp:Label runat="server" ID="lblProductName" /></h1>
                    <div class="clearBothNoSPC">
                    </div>
                    <p>
                        <%= targetMerchandise["Description"] != DBNull.Value && !string.IsNullOrWhiteSpace((string)targetMerchandise["Description"] ) ? targetMerchandise["Description"] : targetMerchandise["ShortDescription"]%></p>
                    <div class="productListingPrice">
                        <span class="columnHeader">Price:</span> <strong class="redHighlight"><asp:Literal ID="litPrice" runat="server" /></strong>
                        <%=  targetMerchandise["MemberPrice"] != DBNull.Value ? string.Format("<br><span class=\"columnHeader\">Member Price:</span>  <strong class=\"redHighlight\">{0:C}</strong>", targetMerchandise["MemberPrice"]) : ""%>
                    </div>
                    <table>
                        <tr>
                            <td>
                                Quantity:
                                <asp:TextBox ID="tbQuantity" runat="server" Text="1" Width="60" />
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="lbAddToCart" CssClass="uBtn floatLeft" OnClick="lbAddToCart_Click" ><span>Add To Cart</span></asp:LinkButton><br />
                                <asp:CompareValidator ID="valRequired" runat="server" ControlToValidate="tbQuantity"
                                    ValueToCompare="0" Type="Integer" Operator="GreaterThan" ErrorMessage="Please specify a valid quantity"
                                    Display="None" />
                            </td>
                        </tr>
                    </table>
              
                <div class="productListingBreak">
                </div>

                <div class="clearBothNoSPC">
                </div>
            </div>
                            <asp:LinkButton runat="server" ID="lbContinueShopping" CssClass="uBtn floatLeft"
                    OnClick="lbContinueShopping_Click"><span>Continue Shopping</span></asp:LinkButton>
        </div>
    </div>
    <div id="colRightSml">
        <!-- side navigation
    -->
        <div id="sideNavWrap">
            <div id="sideNavCont">
                <div id="sideNavContent">
                    <div id="sideNavHeader">
                        <div class="sectHeaderTitle hIconClipboardPencil">
                            <h2>
                                <asp:Literal ID="lCategories" runat="server">Categories</asp:Literal></h2>
                        </div>
                    </div>
                    <div id="sideNavList">
                        <asp:BulletedList runat="server" DisplayMode="LinkButton" ID="blCategories" DataTextField="Name"
                            DataValueField="ID" OnClick="blCategories_Click" />
                    </div>
                </div>
            </div>
        </div>
        <!-- / side navigation -->
        <!-- shopping cart -->
        <div class="sectContSml noBottomPad">
            <div class="sectHeaderTitle hIconCart">
                <h2>
                    <asp:Literal ID="lShoppingCart" runat="server">Shopping Cart</asp:Literal></h2>
            </div>
            <div class="shoppingCartMsg">
                There are <strong class="redHighlight">(<%= MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.Count()%>)</strong>
                items in your shopping cart.</div>
            <div class="shoppingCartContent">
                <asp:HyperLink runat="server" ID="hlCartSubTotal" NavigateUrl="/onlinestorefront/EditCart.aspx" CssClass="cartSubTotal"></asp:HyperLink>
                <asp:LinkButton runat="server" ID="lbCheckout" OnClick="lbCheckout_Click" class="sqBtn"><span>Checkout</span></asp:LinkButton>
                <div runat="server" id="divRecentlyAdded" class="shoppingCartAddedItems">
                    <strong class="redHighlight">Recently Added Item(s)</strong>
                </div>
                <asp:Repeater ID="rptRecentItems" runat="server" OnItemCommand="rptRecentItems_ItemCommand">
                    <ItemTemplate>
                        <!-- cart item-->
                        <div class="shoppingCartItem">
                            <asp:LinkButton runat="server" ID="lbRemove" CssClass="bShoppingCartItemRemove" />
                            <div class="shoppingCartItemThumb">
                                <asp:Image ID="Image1" ImageUrl='<%# string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["ImageServerUri"],
                                        PortalConfiguration.Current.AssociationID, PortalConfiguration.Current.PartitionKey,
                                        ((DataRow)Container.DataItem)["Image"]) %>' Width="36" Height="36" runat="server" />
                            </div>
                            <div class="shoppingCartItemTitle">
                                <asp:Literal ID="litName" runat="server" Text='<%#  ((DataRow)Container.DataItem)["Name"] %>' />
                            </div>
                            <div class="shoppingCartItemQuant">
                                <asp:Literal ID="Literal1" runat="server" Text='<%# string.Format("{0} x {1:C}",((DataRow)Container.DataItem)["Quantity"], ((DataRow)Container.DataItem)["Price"]) %>' /></div>
                        </div>
                        <!-- / cart item -->
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <!-- / shopping cart -->
    </div>
    <!-- / right
    column -->
    <div class="clearBothNoSPC">
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
