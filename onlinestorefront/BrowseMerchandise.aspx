<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="BrowseMerchandise.aspx.cs" Inherits="onlinestorefront_BrowseMerchandise" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="MemberSuite.SDK.Types" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    
    <title><asp:Literal ID="lCaptionText" runat="server"/>All Merchandise </title>
    <link rel="stylesheet" type="text/css" href="/images/onlinestorefront.css" />
    <style type="text/css">
        .disabled
        {
            color: silver;
        }
        .hint
        {
            color: gray;
            font-size: small;
            text-align: center;
            font-style: italic;
        }
        .searchText
        {
            font-size: small;
            color: #0645AD;
            margin-right: auto;
            margin-left: auto;
            text-decoration: underline;
        }
        .searchText:hover
        {
            cursor: pointer;
            cursor: hand;
            text-decoration: none;
        }
        .pagination
        {
             
            border: solid 1px #AAE !important;
            color: #15B !important;
            padding: 2pt;
            font-weight: bolder;
            
            padding: 0.3em 0.5em;
            margin-right: 5px;
            margin-bottom: 5px;
            width: 80px !important;;
            height: 30px !important;;
            text-align: center;
            -webkit-border-radius: 4px;
            -moz-border-radius: 4px;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.05);
            -moz-box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.05);
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.05);
            vertical-align: middle;
            cursor: pointer;
        }
        
       
        input[type=submit]:disabled {
            background-color: grey;
            color: White !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
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
            <td style="float: right">
                <ul class="buttonList">
                    <li><a class="iconBtn iconBtnCart" href="/onlinestorefront/EditCart.aspx"><span>
                        <asp:Literal ID="lViewShoppingCart" runat="server">View
                        Shopping Cart</asp:Literal>
                        (<%= MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.Count()%>)</span></a></li>
                </ul>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <script type="text/javascript">
        jQuery(document).ready(function ($) {

            if ($(".searchText").attr("display") == "block") {
                showHideSearch($, $(".searchText"));
            }

            $(".searchText").click(function () {
                showHideSearch($, $(this));
            });
        });
        function showHideSearch($, obj) {
            if (obj.text() == "Click Here To Search Products") {
                $("#search").show();
                obj.text("Click Here To Hide Search Products");
            } else {
                obj.text("Click Here To Search Products");
                $("#search").hide();
            }
        }
        function nothing()
        { return false; }
        function enableDisableButton() {
            if (jQuery("#<%=tbProductName.ClientID %>").val() == "" && $("#<%=tbProductDescription.ClientID %>").val() == "") {
                jQuery("#<%=btnFilter.ClientID %>").attr("disabled", "disabled");
                jQuery("#<%=btnFilter.ClientID %>").addClass("disabled");

            } else {
                jQuery("#<%=btnFilter.ClientID %>").removeAttr("disabled");
                jQuery("#<%=btnFilter.ClientID %>").removeClass("disabled");
            }
        }

        function blockUI() {


            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .5,
                    color: '#fff'
                }
            });
        }
        function unblockUI() {
            $.unblockUI();
        }
    </script>
    <asp:Literal ID="PageText" runat="server" />
    <table align="center">
        <tr>
            <td align="center">
                <span class="searchText" runat="server" id="searchText">Click Here To Search Products</span>
            </td>
        </tr>
        <tr>
            <td>
                <table style="width: 100%; border: 1pt solid silver; display: none" id="search">
                    <caption style="font-weight: bolder; border-top: 1pt solid silver; border-left: 1pt solid silver;
                        border-right: 1pt solid silver">
                        Filter products
                    </caption>
                    <tr>
                        <td>
                            <asp:Literal ID="lProductName" runat="server" Text="Name: " />
                        </td>
                        <td>
                            <asp:TextBox ID="tbProductName" runat="server" />
                        </td>
                        <td>
                            <asp:Literal ID="lProductDescription" runat="server" Text="Description: " />
                        </td>
                        <td>
                            <asp:TextBox ID="tbProductDescription" runat="server" Width="300" />
                        </td>
                        <td>
                            <asp:Button ID="btnFilter" runat="server" Text="Search" OnClick="btnFilter_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="text-align: center">
                            <asp:Label ID="lblHint" runat="server" CssClass="hint" Text="If both Name and Description boxes are empty, all merchandise will be retrieved when you click on the Search button." />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <!-- left column -->
    <div id="colLeftLrg">
        <div class="sectContLrg productListing">
           
          
                    <table style="width: 100%; border: 1pt solid silver; margin-left: auto; margin-right: auto" id="tblNavInfo" runat="server">
                    
                        <tr>
                            <td style="font-weight:900">
                                <asp:Literal ID="lUpdateInfo" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="margin-left: auto; margin-right: auto; text-align: center">
                                <asp:Button ID="btnFirst" runat="server" Text="First" CssClass="pagination" OnClick="btnFirst_Click" OnClientClick=" blockUI();" />
                                <asp:Button ID="btnPrevous" runat="server" Text="Previous" CssClass="pagination"
                                    OnClientClick=" blockUI();"  OnClick="btnPrevous_Click" />
                                <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="pagination" OnClick="btnNext_Click" OnClientClick=" blockUI();"/>
                                <asp:Button ID="btnLast" runat="server" Text="Last" CssClass="pagination" OnClick="btnLast_Click"  OnClientClick=" blockUI();"
                                 />
                            </td>
                        </tr>
                    </table>
                    <asp:Repeater ID="rptMerchandise" runat="server" OnItemCommand="rptMerchandise_ItemCommand"
                        OnItemDataBound="rptMerchandise_ItemDataBound" >
                        <ItemTemplate>
                           
                            <div class="productListItem">
                                <asp:ImageButton ID="imgProduct" ImageUrl="~/Images/noimage.gif" Height="100" runat="server"
                                    CommandName="gotodetails" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>' />
                                <div class="productListItemInfo">
                                    <h1>
                                        <asp:LinkButton ID="lbName" runat="server" Text='<%# ((DataRowView)Container.DataItem)["Name"] %>'
                                            CommandName="gotodetails" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>' /></h1>
                                    <div class="clearBothNoSPC">
                                    </div>
                                    <p>
                                        <asp:Literal ID="litDescription" runat="server" Text='<%# ((DataRowView)Container.DataItem)["ShortDescription"] %>' /></p>
                                    <div class="productListingPrice">
                                        <span class="columnHeader">Price:</span> <strong class="redHighlight">
                                            <asp:Literal ID="litPrice" runat="server" /></strong>
                                        <asp:Literal ID="litMemberPrice" Visible='<%# ((DataRowView)Container.DataItem)["MemberPrice"] != DBNull.Value  %>'
                                            runat="server" Text='<%# string.Format("<br><span class=\"columnHeader\">Member Price:</span>  <strong class=\"redHighlight\">{0:C}</strong>", ((DataRowView)Container.DataItem)["MemberPrice"]) %>' />
                                    </div>
                                    <asp:LinkButton runat="server" ID="lbAddToCart" CssClass="uBtn floatLeft" CommandName="addtocart" OnClientClick=" blockUI();"
                                        CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>'><span>Add To Cart</span></asp:LinkButton>
                                    <div class="clearBothNoSPC">
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="productListItem last">
                                <div class="productListItemContent">
                                    <asp:ImageButton ID="imgProduct" ImageUrl="~/Images/noimage.gif" Height="100" runat="server"
                                        CommandName="gotodetails" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>' />
                                    <div class="productListItemInfo">
                                        <h1>
                                            <asp:LinkButton ID="lbName" runat="server" Text='<%# ((DataRowView)Container.DataItem)["Name"] %>'
                                                CommandName="gotodetails" CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>' /></h1>
                                        <div class="clearBothNoSPC">
                                        </div>
                                        <p>
                                            <asp:Literal ID="litDescription" runat="server" Text='<%# ((DataRowView)Container.DataItem)["ShortDescription"] %>' /></p>
                                        <div class="productListingPrice">
                                            <span class="columnHeader">Price:</span> <strong class="redHighlight">
                                                <asp:Literal ID="litPrice" runat="server" /></strong>
                                            <asp:Literal ID="litMemberPrice" Visible='<%# ((DataRowView)Container.DataItem)["MemberPrice"] != DBNull.Value  %>'
                                                runat="server" Text='<%# string.Format("<br><span class=\"columnHeader\">Member Price:</span>  <strong class=\"redHighlight\">{0:C}</strong>", ((DataRowView)Container.DataItem)["MemberPrice"]) %>' />
                                        </div>
                                        <asp:LinkButton runat="server" ID="lbAddToCart" CssClass="uBtn floatLeft" CommandName="addtocart"
                                            CommandArgument='<%# ((DataRowView)Container.DataItem)["ID"] %>'><span>Add To Cart</span></asp:LinkButton>
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
      
            <table>
                <tr>
                    <td>&nbsp;</td>
                </tr>
            </table>
            <table style="width: 100%; border: 1pt solid silver; margin-left: auto; margin-right: auto" id="tblBottomNavInfo" runat="server">
                    
                        <tr>
                            <td style="font-weight:900">
                                <asp:Literal ID="lbUpdateInfo" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="margin-left: auto; margin-right: auto; text-align: center">
                                <asp:Button ID="btnBFirst" runat="server" Text="First" CssClass="pagination" OnClick="btnFirst_Click" OnClientClick=" blockUI();" />
                                <asp:Button ID="btnBPrevous" runat="server" Text="Previous" CssClass="pagination"
                                    OnClientClick=" blockUI();"  OnClick="btnPrevous_Click" />
                                <asp:Button ID="btnBNext" runat="server" Text="Next" CssClass="pagination"   OnClientClick=" blockUI();" OnClick="btnNext_Click" />
                                <asp:Button ID="btnBLast" runat="server" Text="Last" CssClass="pagination" OnClick="btnLast_Click"  OnClientClick=" blockUI();"
                                 />
                            </td>
                        </tr>
                    </table>
            
            <asp:Label runat="server" CssClass="columnHeader" ID="lblNoProducts" Visible="false" />
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
            </div>
        </asp:PlaceHolder>
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
                <asp:HyperLink runat="server" ID="hlCartSubTotal" NavigateUrl="/onlinestorefront/EditCart.aspx"
                    CssClass="cartSubTotal"></asp:HyperLink>
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
