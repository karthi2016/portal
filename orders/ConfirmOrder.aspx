<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ConfirmOrder.aspx.cs" Inherits="orders_ConfirmOrder" %>

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
    <asp:Literal ID="PageText" runat="server">
    <b><span class="requiredField">YOUR ORDER HAS NOT BEEN SUBMITTED YET! Please review the
        information below and select <b>Place Order</b> to submit your order.</span></b>
    </asp:Literal>
    <div class="section" style="margin-top: 10px">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lItemsInYourOrder" runat="server">Items In Your Order</asp:Literal></h2>
        </div>
        <asp:Label ID="lblShoppingCartEmpty" runat="server">There are no items in your shopping cart.</asp:Label>
        <div id="divMissingDemographics" runat="server" style="color: Red; vertical-align: middle">
            <img alt="Warning" height="20" src="/images/icons/warning.png" />
            <asp:Literal ID="lMissingDemographics" runat="server">
        One or more items in your order require additional information. Click the <b>(edit)</b> link next to the item(s)
        to add the information.
            </asp:Literal>
        </div>
        <!-- Per MS-2148 - we're removing the type column -->
        <asp:GridView ID="gvShoppingCart" runat="server" GridLines="None" Width="800px" AutoGenerateColumns="false"
            OnRowDataBound="gvShoppingCart_RowDataBound" OnRowCommand="gvShoppingCart_OnRowCommand"
            HeaderStyle-HorizontalAlign="Left">
            <Columns>
                <asp:TemplateField HeaderText="Qty" ItemStyle-HorizontalAlign="Center" Visible="false">
                    <ItemTemplate>
                        <asp:TextBox ID="tbQuantity" runat="server" Text="0" Width="30" />
                        <asp:CompareValidator ID="cvQuantity" runat="server" ControlToValidate="tbQuantity"
                            Operator="GreaterThanEqual" ValueToCompare="0" ErrorMessage="One or more item quantities are invalid."
                            Type="Integer" Display="None" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="lblProductType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Product" HeaderStyle-Width="500px" HeaderStyle-HorizontalAlign="Left"
                    ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Image ID="imgWarning" runat="server" ImageUrl="/images/icons/warning.png" Height="20"
                            Visible="false" />
                        <asp:Label ID="lblProductName" runat="server" />
                        <asp:LinkButton ID="lbEdit" CommandName="Edit" CausesValidation="false" runat="server">(edit)</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Quantity" HeaderStyle-HorizontalAlign="Left" DataField="Quantity"
                    DataFormatString="{0:F2}" />
                <asp:BoundField HeaderText="Unit Price" HeaderStyle-HorizontalAlign="Left" DataField="UnitPrice"
                    DataFormatString="{0:C}" />
                <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="lblProductPrice" CssClass="price" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:ButtonField ButtonType="Button" Text="Remove" CommandName="Delete" ItemStyle-HorizontalAlign="Right"
                    Visible="false" />
            </Columns>
        </asp:GridView>
        <hr style="width: 100%" />
        <div style="float: right; padding-right: 60px; margin-bottom: 50px">
            <table style="width: 200px;">
                <tr id="trDiscounts" runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lDiscounts" runat="server">Discounts:</asp:Literal>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="lblDiscounts" runat="server" CssClass="price" Text="$0.00" />
                    </td>
                </tr>
                <tr id="trShipping" runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lShipping" runat="server">Shipping:</asp:Literal>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="lblShipping" runat="server" CssClass="price" Text="$0.00" />
                    </td>
                </tr>
                <tr id="trTaxes" runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lTasks" runat="server">Taxes:</asp:Literal>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="lblTaxes" runat="server" CssClass="price" Text="$0.00" />
                    </td>
                </tr>
                <tr id="trTotal" runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lTotal" runat="server">Total:</asp:Literal>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="lblTotal" runat="server" CssClass="price" Text="$0.00" />
                    </td>
                </tr>
                <tr id="trTotalDueNow" runat="server" visible="false">
                    <td class="columnHeader">
                        <asp:Literal ID="lTotalDueNow" runat="server">Total Due Now:</asp:Literal>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="lblTotalDueNow" runat="server" CssClass="price" Text="$0.00" />
                    </td>
                </tr>
            </table>
            <asp:HyperLink ID="hlChangeRemoveAdditionalItems" NavigateUrl="/orders/CrossSellItems.aspx"
                runat="server" Text="Change/Remove Items You've Added" Visible="false" />
        </div>
        <div style="min-height: 50px; padding-top: 20px">
            <table>
                <tr style="vertical-align: top">
                    <td style="width: 50%">
                        <div id="divBilling" runat="Server">
                            <h3>
                                <asp:Literal ID="lBillingInfo" runat="server">Billing Information</asp:Literal></h3>
                            <hr />
                            <table>
                                <tr>
                                    <td class="columnHeader" style="width: 150px">
                                        <asp:Literal ID="lPaymentMethod" runat="server">Payment Method:</asp:Literal>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Label ID="lblPaymentMethod" runat="server">AMEX ending in x43433</asp:Label>
                                        <asp:HyperLink ID="hlChangeBilling" runat="server" NavigateUrl="~/orders/EnterBillingInfo.aspx">(change)</asp:HyperLink>
                                        <br />
                                        <span style="font-size: 10px" id="spanSaving" runat="server" ><i>
                                        <asp:Literal ID="lSavingInfo" runat="server" Visible="false">
                                            <span style="color:green">We will be securely saving this payment information so you can use it in the future.</span>
                                        </asp:Literal>
                                        
                                        </i>
                                        </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="padding-top: 10px">
                                        <asp:Literal ID="lBillingAddress" runat="server"><strong><u>Billing Address</u></strong></asp:Literal>
                                        <br />
                                        <asp:Label ID="lblBillingAddress" runat="server">n/a</asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td style="width: 20px">
                    </td>
                    <td>
                        <div id="divShipping" runat="server" visible="false">
                            <h3>
                                <asp:Literal ID="lShippingInfo" runat="server">Shipping Information</asp:Literal></h3>
                            <hr />
                            <table style="width: 300px">
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lShippingMethodHeader" runat="server">Shipping Method:</asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="lShippingMethod" runat="server">n/a</asp:Literal>
                                        <asp:HyperLink ID="hlChangeShippingMethod" runat="server" NavigateUrl="~/orders/EnterShippingInformation.aspx">(change)</asp:HyperLink>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="padding-top: 10px">
                                        <asp:Literal ID="lShipToHeader" runat="server"><strong><u>Ship To</u></strong></asp:Literal>
                                        <br />
                                        <asp:Literal ID="lShipTo" runat="server">n/a</asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divFutureBillings" runat="server" visible="false">
            <h2>
                <asp:Literal ID="lFutureBillingInfo" runat="server">Future Billing Information</asp:Literal></h2>
            <asp:Literal ID="lOneOrMoreItemsRequireInstallments" runat="server">One or more of the items you have ordered will be billed in future installments.
            The <b>Total Due Now</b> is the amount that you will be charged for when you complete
            your order. You will be billed on the following dates:
            <br />
            <br /></asp:Literal>
            <asp:DataList ID="dlFutureBillings" runat="server" RepeatColumns="3" RepeatDirection="Vertical"
                RepeatLayout="Table">
                <ItemTemplate>
                    <b>
                        <%#DataBinder.Eval( Container.DataItem,"Date", "{0:d}") %></b> - <span class="price">
                            <%#DataBinder.Eval( Container.DataItem,"Amount", "{0:C}") %>
                        </span>
                </ItemTemplate>
            </asp:DataList>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" id="divExhibitorConfirmation" runat="server"
        visible="false">
        <div class="sectHeaderTitle">
            <h2>
                Exhibitor Confirmation</h2>
        </div>
        <div id="divExhibitorConfirmation_BoothPreferences" runat="server" visible="false">
            <b>Booth Preferences:</b> &nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblExhbitor_BoothPreferences" runat="server" />
            <br />
            &nbsp;
        </div>
        <h3>
            Special Requests</h3>
        <asp:Label ID="lblExhibitorSpecialRequests" runat="server">None.</asp:Label>
    </div>
    <asp:Label ID="lblContinueShoppingInstructions" runat="server">
    You can go ahead and complete your order now, or leave these items and your cart
    and continue shopping.
    </asp:Label>
    <p />
      <div class="sectHeaderTitle">
            <h2>
               Notes/Comments</h2>
        </div>
        Add any notes or special instructions to this order.
    <asp:TextBox ID="tbNotesComments" runat="server" TextMode="MultiLine" Rows=5 Width=600px/>
    <hr style="width: 100%" />
    <div style="text-align: center">
         
        <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" OnClick="btnPlaceOrder_Click" />
        <asp:PlaceHolder runat="server" ID="CancelOrderWrapper">
           or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel Your Order" CausesValidation="false"
            OnClick="btnContinueShopping_Click" />
        </asp:PlaceHolder>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
