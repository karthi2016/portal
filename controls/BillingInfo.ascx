<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BillingInfo.ascx.cs" Inherits="controls_BillingInfo" %>
<%@ Register TagPrefix="cc1" Namespace="MemberSuite.SDK.Web.Controls" Assembly="MemberSuite.SDK.Web" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Literal ID="lMessages" runat="server" />
<asp:CustomValidator ID="cvBillingValidator" runat="server" OnServerValidate="cvBillingValidator_OnServerValidate"
    Display="None" />
<div style="background-color: #EFEFEF; padding: 10px" id="divSavedPaymentOptions"
    runat="server">
    <h3>
        <asp:Literal ID="lSavedOptions" runat="server">Use Your Saved Payment Options</asp:Literal>
    </h3>
    <div id="divWithGridViewOrRepeater">
        <asp:Repeater ID="rptSavedPaymentOptions" runat="server" OnItemDataBound="rptSavedPaymentOptions_OnItemDataBound">
            <ItemTemplate>
                <asp:RadioButton ID="rbSavedPaymentMethod" GroupName="PaymentType" runat="server" />
                <asp:HiddenField ID="hfPaymentMethodID" runat="server" />
                <br />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <asp:Literal ID="lNoOptions" runat="server">Currently, you have no saved payment methods. When you check out, you
        can save your payment information on your account so you do not have to re-enter it again.</asp:Literal>
</div>
<div style="vertical-align: top; padding-top: 20px;">
    <ng-form name="payments.cardForm">
        <div id="divNewCreditCard" runat="server">
            <h3>
                <asp:RadioButton ID="rbNewCard" runat="server" Text="Pay With a New Credit/Debit Card"
                    GroupName="PaymentType" ms-on-radio-change="payments.setPaymentMethod({text: 'credit card'})"/>
            </h3>
            <div id="divNewCreditCardPanel" runat="server" style="border: 1px solid #C0C0C0;
                padding: 5px; margin-bottom: 20px; width: 450px" ng-show="payments.paymentCreditCard">
                <table style="width: 400px">
                <tr>
                    <td rowspan="4" style="width: 120px">
                        <img src="/images/creditcards.gif" />
                    </td>
                    <td style="width: 100px">
                        Card Number: <span class="requiredField">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="tbCardNumber" CssClass="inputText  cc-number" runat="server" autocomplete="off" 
                            ng-class="{hidden: payments.accountIsTokenized}" ng-model="payments.cardInfo.account" ng-pattern="/^[0-9]{13,19}$/"/>
                        <asp:RequiredFieldValidator ID="rfvCreditCardNumber" runat="server" ControlToValidate="tbCardNumber"
                            Enabled="false" Display="None" ErrorMessage="You have not entered a credit card number."
                            EnableClientScript="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Name on Card:<span class="requiredField">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="tbNameOnCard" runat="server" autocomplete="off" ng-pattern="/^(?!\s*$).+/"/>
                        <asp:RequiredFieldValidator ID="rfvNameOnCard" runat="server" ControlToValidate="tbNameOnCard"
                            Display="None" ErrorMessage="You have not entered a name for the credit card."
                            Enabled="false" EnableClientScript="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Security Code:<span class="requiredField">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="tbCCV" Width="50px" runat="server" autocomplete="off" ng-model="payments.cardInfo.cvv"/>
                        <asp:RequiredFieldValidator ID="rfvSecurityCode" runat="server" ControlToValidate="tbCCV"
                            Display="None" ErrorMessage="You have not entered a security code for the credit card."
                            Enabled="false" EnableClientScript="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Expiration Date:<span class="requiredField">*</span>
                    </td>
                    <td>
                        <cc1:MonthYearPicker ID="myExpiration" runat="server" ms-month-year="payments.setExpiration"/>
                    </td>
                </tr>
            </table>
                <br />
                <asp:CheckBox Style="font-size: 11px" ID="cbSaveCreditCard" Checked="true" runat="server"
                    Text="Save this credit card so that I can use it for future orders" />
            </div>
        </div>
    </ng-form>
    <ng-form name="payments.bankForm">
        <div id="divNewChecking" runat="server">
        <h3>
            <asp:RadioButton ID="rbNewChecking" runat="server" Text="Pay With Your Bank Account"
                GroupName="PaymentType" ms-on-radio-change="payments.setPaymentMethod({text: 'electronic check'})"/></h3>
        <div id="divNewCheckingPanel" runat="server" style="border: 1px solid #C0C0C0; padding: 5px;
            margin-bottom: 20px; width: 450px" ng-show="payments.paymentECheck">
            <img src="/images/check_example.gif" />
            <table style="width: 400px">
                <tr>
                    <td style="width: 250px">
                        Routing Account Number <span class="requiredField">*</span>
                    </td>
                    <td>
                        Bank Account Number <span class="requiredField">*</span>
                    </td>
                </tr>
                <tr>
                    <td style="width: 50px">
                        <asp:TextBox ID="tbRoutingNumber" runat="server" CssClass="inputText rtn-number" autocomplete="off" ng-model="payments.bankInfo.rtnNumber" ng-pattern="/^[0-9]{9}$/"/>
                        <asp:RegularExpressionValidator ID="revRoutingNumber" ValidationExpression="^((0[0-9])|(1[0-2])|(2[1-9])|(3[0-2])|(6[1-9])|(7[0-2])|80)([0-9]{7})$"
                            ControlToValidate="tbRoutingNumber" Display="None" ErrorMessage="Please enter a valid nine digit routing number"
                            EnableClientScript="false" Enabled="false" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvRoutingNumber" runat="server" ControlToValidate="tbRoutingNumber"
                            Display="None" ErrorMessage="You have not entered a routing number." EnableClientScript="false" />
                    </td>
                    <td>
                        <asp:TextBox ID="tbBankAccountNumber" runat="server" CssClass="inputText ba-number" autocomplete="off" ng-class="{hidden: payments.accountIsTokenized}" ng-model="payments.bankInfo.account" ng-pattern="/^[0-9]{4,17}$/"/>
                        <asp:RequiredFieldValidator ID="rfvBankAccountNumber" runat="server" ControlToValidate="tbBankAccountNumber"
                            Display="None" ErrorMessage="You have not entered a bank account number." EnableClientScript="false" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 50px">
                        Bank Account Type <span class="requiredField">*</span>
                    </td>
                    <td>
                        Confirm Bank Account Number <span class="requiredField">*</span>
                    </td>
                </tr>
                <tr>
                    <td style="width: 50px">
                        <asp:DropDownList runat="server" ID="rblBankAccountType" CssClass="ba-type" ng-model="payments.bankInfo.type">
                            <asp:ListItem Text="Checking" Value="Checking" Selected="True" />
                            <asp:ListItem Text="Savings" Value="Savings" />
                        </asp:DropDownList>                        
                    </td>
                    <td>
                        <asp:TextBox ID="tbBankAccountNumberConfirm" runat="server" CssClass="inputText ba-number-confirm" autocomplete="off" ng-model="payments.bankInfo.accountConfirm" ms-match="payments.bankInfo.account"/>                       
                        <asp:RequiredFieldValidator ID="rfvBankAccountNumberConfirm" runat="server" ControlToValidate="tbBankAccountNumberConfirm"
                            Display="None" ErrorMessage="You must re-enter your bank account number for confirmation."
                            EnableClientScript="false" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:CheckBox Style="font-size: 11px" ID="cbSaveCheckingAccount" Checked="true" runat="server"
                Text="Save this checking account so that I can use it for future orders" />
        </div>
    </div>
    </ng-form>
        <div id="divPayrollDeduction" runat="server">
            <h3>
                <asp:RadioButton ID="rbPayrollDeduction" runat="server" Text="Pay With Payroll Deduction"
                    GroupName="PaymentType" ms-on-radio-change="payments.setPaymentMethod({text: 'payroll deduction'})"/></h3>
            <div id="divPayrollDeductionPanel" runat="server" style="border: 1px solid #C0C0C0;
                padding: 5px; margin-bottom: 20px; width: 450px" ng-show="payments.paymentPayrollDeduction">
                <asp:Literal ID="lPayrollDeductionWaiver" runat="server">
                 By selecting this option, I am authorizing the deduction of the specific amount from my payroll.
                 If for any reason, except
death, employment is terminated, any amount still owing under this authorization shall be
deducted from the last payment due. Death of the member shall revoke this authorization, and
no further deduction shall be made.
                </asp:Literal>
            </div>
        </div>
        <div id="divBillMeLater" runat="server">
            <h3>
                <asp:RadioButton ID="rbBillMeLater" runat="server" Text="Send Me an Invoice" GroupName="PaymentType" ms-on-radio-change="payments.setPaymentMethod({text: 'bill me later'})"/></h3>
            <div id="divBillMeLaterPanel" runat="server" style="border: 1px solid #C0C0C0; padding: 5px;
                margin-bottom: 20px; width: 450px" ng-show="payments.paymentBillMeLater">
                <table style="width: 400px">
                    <tr>
                        <td style="width: 250px">
                            <asp:Literal ID="lPO" runat="server" Text="Reference/Purchase Order #: "/>
                        </td>
                        <td>
                            <asp:TextBox ID="tbReferenceNumber" runat="server" autocomplete="off" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>    
</div>
<div id="divBillingAddress" runat="server" style="margin-top: 20px" ng-show="payments.showBillingAddress">
    <div class="sectHeaderTitle">
        <h2>Which Billing Address Should We Use?
        </h2>
    </div>
    <asp:Literal ID="lBillingAddressRequired" runat="server">
    The payment method you have selected requires a billing address. Please select from a list below, or enter a new address.
    </asp:Literal>
    <div id="billingAddresses">
        <table style="margin-top: 20px;" cellpadding="5">
            <asp:Repeater ID="rptBillingAddress" runat="server" OnItemDataBound="rptBillingAddress_OnItemDataBound">
                <ItemTemplate>
                    <tr style="vertical-align: top; margin-top: 20px">
                        <td style="width: 30px">
                            <asp:RadioButton ID="rbAddress" runat="server"/>
                        </td>
                        <td>
                            <asp:Literal ID="lAddress" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr style="vertical-align: top; margin-top: 20px">
                <td style="width: 30px">
                    <asp:RadioButton ID="rbNewBillingAddress" GroupName="BillingAddress" runat="server"/>
                </td>
                <td>Enter a new address:
                    <div id="newBillingAddress" runat="server">
                        <cc1:AddressControl ID="acBillingAddress" IsRequired="False" EnableValidation="True" runat="server" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">

        jQuery(document).ready(function ($) {
            var rbNewCard = document.getElementById('<%= rbNewCard.ClientID %>');
            if (rbNewCard) $("#divWithGridViewOrRepeater input:radio").attr("name", rbNewCard.name);

            var rbNewBillingAddress = document.getElementById('<%= rbNewBillingAddress.ClientID %>');
            if (rbNewBillingAddress) $("#billingAddresses input:radio").attr("name", rbNewBillingAddress.name);

        });

    </script>
</telerik:RadCodeBlock>
