<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ConfirmOrder.aspx.cs" Inherits="orders_ConfirmOrder" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        function updatePayment() {

            var rbCreditCard = document.getElementById('<%=rbPaymentCreditCard.ClientID %>');
            var rbPayLater = document.getElementById('<%=rbPaymentPayLater.ClientID %>');

            var divPayLater = document.getElementById('divPayLater');
            var divCreditCard = document.getElementById('divCreditCard');

            if (rbCreditCard == null) return;   // it's hidden

            // activate/deactivator client validators
            ValidatorEnable(document.getElementById('<%=rfvCCNameOnCard.ClientID %>'), rbCreditCard.checked);
            ValidatorEnable(document.getElementById('<%=rfvCreditCardNumber.ClientID %>'), rbCreditCard.checked);
            ValidatorEnable(document.getElementById('<%=rfvCardSecurity.ClientID %>'), rbCreditCard.checked);

            // hide them all
            divPayLater.style.display = 'none';
            divCreditCard.style.display = 'none';

            if (rbCreditCard.checked)
                divCreditCard.style.display = '';

            if (rbPayLater.checked)
                divPayLater.style.display = '';

        }
    </script>
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
        <div id="divDiscountPromo" runat="server" style="float: left">
            <asp:Literal ID="lApplyDiscountPromoCode" runat="server">Apply discount/promo code:</asp:Literal>
            <asp:TextBox ID="tbPromoCode" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPromoCode" runat="server" ControlToValidate="tbPromoCode"
                ValidationGroup="DiscountCode" ErrorMessage="Please enter a discount code." Display="None" />
            <asp:Button ID="btnApplyCoupon" ValidationGroup="DiscountCode" OnClick="btnApplyDiscountCode_Click"
                CommandName="ApplyDiscount" runat="server" Text="Apply Code" />
        </div>
        
        <div style="float: right; padding-right: 60px; margin-bottom: 50px">
            <table style="width: 200px;">
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDiscounts" runat="server">Discounts:</asp:Literal>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="lblDiscounts" runat="server" CssClass="price" Text="$0.00" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lShipping" runat="server">Shipping:</asp:Literal>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="lblShipping" runat="server" CssClass="price" Text="$0.00" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lTasks" runat="server">Taxes:</asp:Literal>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="lblTaxes" runat="server" CssClass="price" Text="$0.00" />
                    </td>
                </tr>
                <tr>
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
            <ASP:HyperLink ID="hlChangeRemoveAdditionalItems" NavigateUrl="/orders/CrossSellItems.aspx" runat="server" Text="Change/Remove Items You've Added"  Visible="false" />
        </div>
        <div style="min-height: 50px; padding-top: 20px">
            <div id="divShipping" runat="server" visible="false">
                <h3>
                    <asp:Literal ID="lShippingInfo" runat="server">Shipping Information</asp:Literal></h3>
                <table style="width: 300px">
                    <tr>
                        <td>
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
            <br />&nbsp;
        </div>
        <h3>
            Special Requests</h3>
        <asp:Label ID="lblExhibitorSpecialRequests" runat="server">None.</asp:Label>
    </div>
    <div class="section" id="divPayment" runat="server" visible="false">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lPaymentInfo" runat="server">Payment Information</asp:Literal></h2>
        </div>
        <p>
            <asp:Literal ID="lHowWouldYouLikeToPay" runat="Server">How would you like to pay for this order?</asp:Literal>
        </p>
        <table style="width: 100%">
            <tr>
                <td colspan="2">
                    <asp:RadioButton ID="rbPaymentCreditCard" Checked="true" onclick="updatePayment();"
                        GroupName="Payment" runat="server" Text="Pay with Credit Card" />
                    <asp:RadioButton ID="rbPaymentPayLater" GroupName="Payment" onclick="updatePayment();"
                        runat="server" Text="Bill Me/Pay Later" />
                    <hr style="width: 100%" />
                </td>
            </tr>
            <tr valign="top">
                <td>
                    <div id="divPayLater">
                        <table style="width: 500px">
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lPurchaseOrder" runat="Server">Purchase Order #:</asp:Literal>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbPurchaseOrder" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="divCreditCard">
                        <h3>
                            <asp:Literal ID="lCreditCardInfo" runat="Server">Credit Card Information</asp:Literal></h3>
                        <table style="width: 500px">
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lNameOnCard" runat="Server">Name on Card: </asp:Literal><span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbName" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvCCNameOnCard" runat="server" ControlToValidate="tbName"
                                        Display="None" ErrorMessage="You have not entered the name on your credit card." />
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lCreditCardNum" runat="Server">Credit Card Number:</asp:Literal>
                                    <span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbCreditCardNumber" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvCreditCardNumber" runat="server" ControlToValidate="tbCreditCardNumber"
                                        Display="None" ErrorMessage="You have not entered your credit card number." />
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lCVV" runat="Server">Credit Card Security Code (CVV):</asp:Literal>
                                    <span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbCVV" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvCardSecurity" runat="server" ControlToValidate="tbCVV"
                                        Display="None" ErrorMessage="You have not entered the security code on the back of your card." />
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lCreditCardExp" runat="Server">Credit Card Expiration:</asp:Literal>
                                    <span class="requiredField">*</span>
                                </td>
                                <td>
                                    <cc1:MonthYearPicker ID="myExpiration" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
                <td>
                    <h3>
                        <asp:Literal ID="lBillingAddress" runat="Server">Billing Address</asp:Literal></h3>
                    <cc1:AddressControl ID="acBillingAddress" IsRequired="true" EnableValidation="False"
                        runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <asp:Label ID="lblContinueShoppingInstructions" runat="server">
    You can go ahead and complete your order now, or leave these items and your cart
    and continue shopping.
    </asp:Label>
    <p />
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" CausesValidation="false" OnClick="btnContinueShopping_Click"
            runat="server" Text="Continue Shopping" />
        <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" OnClick="btnPlaceOrder_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
