<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="BrowseMerchandise.aspx.cs" Inherits="onlinestorefront_BrowseMerchandise" %>

<%@ Import Namespace="System.Data" %>

<%@ Import Namespace="MemberSuite.SDK.Types" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
    <link rel="stylesheet" type="text/css" href="/images/onlinestorefront.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<asp:HyperLink runat="server" ID="hlAllMerchandise" NavigateUrl="~/onlinestorefront/BrowseMerchandise.aspx" Text="All Merchandise" /> <asp:HyperLink runat="server" ID="hlCategory" /> 
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Browse Merchandise
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
    <table>
        <tr>
            <td align="right">
                <ul class="buttonList">
                    <li><a class="iconBtn iconBtnCart" href="/onlinestorefront/EditCart.aspx"><span><asp:Literal ID="lViewShoppingCart" runat="server">View
                        Shopping Cart</asp:Literal> (<%= MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.Count()%>)</span></a></li>
                </ul>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server"/>
    <!-- left column -->
    <div id="colLeftLrg">
        <div class="sectContLrg productListing">
            <asp:Label runat="server" CssClass="columnHeader" ID="lblNoProducts" Visible="false" />
            <asp:Repeater ID="rptMerchandise" runat="server" OnItemCommand="rptMerchandise_ItemCommand" OnItemDataBound="rptMerchandise_ItemDataBound">
                <ItemTemplate>
                    <div class="productListItem">
                        <asp:ImageButton ID="imgProduct" ImageUrl="~/Images/noimage.gif" Height="100" runat="server" CommandName="gotodetails" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>' />
                               <div class="productListItemInfo">
                            <h1>
                                <asp:LinkButton ID="lbName" runat="server" Text='<%# ((DataRowView)Container.DataItem)["Name"] %>' CommandName="gotodetails" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>' /></h1>
                            <div class="clearBothNoSPC">
                            </div>
                            <p>
                                <asp:Literal ID="litDescription" runat="server" Text='<%# ((DataRowView)Container.DataItem)["ShortDescription"] %>' /></p>
                            <div class="productListingPrice">
                                <span class="columnHeader">Price:</span> <strong class="redHighlight"><asp:Literal ID="litPrice" runat="server" /></strong>
                                    <asp:Literal ID="litMemberPrice" Visible='<%# ((DataRowView)Container.DataItem)["MemberPrice"] != DBNull.Value  %>' runat="server" Text='<%# string.Format("<br><span class=\"columnHeader\">Member Price:</span>  <strong class=\"redHighlight\">{0:C}</strong>", ((DataRowView)Container.DataItem)["MemberPrice"]) %>' />
                            </div>
                            <asp:LinkButton runat="server" ID="lbAddToCart" CssClass="uBtn floatLeft" CommandName="addtocart" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>'><span>Add To Cart</span></asp:LinkButton>
                            <div class="clearBothNoSPC">
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <div class="productListItem last">
                        <div class="productListItemContent">
                            <asp:ImageButton ID="imgProduct" ImageUrl="~/Images/noimage.gif"  Height="100" runat="server" CommandName="gotodetails" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>' />
                            <div class="productListItemInfo">
                                <h1>
                                    <asp:LinkButton ID="lbName" runat="server" Text='<%# ((DataRowView)Container.DataItem)["Name"] %>' CommandName="gotodetails" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>' /></h1>
                                <div class="clearBothNoSPC">
                                </div>
                                <p>
                                    <asp:Literal ID="litDescription" runat="server" Text='<%# ((DataRowView)Container.DataItem)["ShortDescription"] %>' /></p>
                                <div class="productListingPrice">
                                <span class="columnHeader">Price:</span> <strong class="redHighlight"><asp:Literal ID="litPrice" runat="server" /></strong>
                                        <asp:Literal ID="litMemberPrice" Visible='<%# ((DataRowView)Container.DataItem)["MemberPrice"] != DBNull.Value  %>' runat="server" Text='<%# string.Format("<br><span class=\"columnHeader\">Member Price:</span>  <strong class=\"redHighlight\">{0:C}</strong>", ((DataRowView)Container.DataItem)["MemberPrice"]) %>' />
                                </div>
                                <asp:LinkButton runat="server" ID="lbAddToCart" CssClass="uBtn floatLeft" CommandName="addtocart" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>'><span>Add To Cart</span></asp:LinkButton>
                                <div class="clearBothNoSPC">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="clearBothNoSPC">
                    </div>
                    <div class="productListingBreak">
                    </div>
                </AlternatingItemTemplate>
            </asp:Repeater>
            <div class="clearBothNoSPC">
            </div>
            <div runat="server" id="divLastLine" class="productListingBreak">
            </div>
        </div>
    </div>
    <!-- / left column -->
    <!-- right column -->
    <div id="colRightSml">
        <!-- side navigation
    -->
    <asp:PlaceHolder ID="phCategories" runat="server">
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
        </div></asp:PlaceHolder>
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
