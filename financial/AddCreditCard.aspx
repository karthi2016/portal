<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="AddCreditCard.aspx.cs" Inherits="financial_AddCreditCard" %>

<%@ Register TagPrefix="cc1" Namespace="MemberSuite.SDK.Web.Controls" Assembly="MemberSuite.SDK.Web" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .none {
            display: none;
        }
        .hidden {
            visibility: hidden
        }
        input.ng-invalid {
            border-color: red
        }
        [ng\:cloak], [ng-cloak], [data-ng-cloak], [x-ng-cloak], .ng-cloak, .x-ng-cloak {
            display: none !important;
        }

        input.masked-cc-number {
            -webkit-text-security: disc;
            color: white;
        }
    </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Save a Credit Card to Your Account
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div id="ng-app" ng-cloak ng-app="msPayments" ng-controller="PaymentController as payments" ng-init="payments.paymentCreditCard=true">
        <ng-form name="payments.cardForm">
            <asp:Literal ID="lPageText" runat="server">Enter the credit card details below.
            </asp:Literal>
            <table style="width: 400px">
                <tr>
                    <td rowspan="4" style="width: 120px">
                        <img src="/images/creditcards.gif" />
                    </td>
                    <td style="width: 100px">Card Number: <span class="requiredField">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="tbCardNumber" CssClass="inputText cc-number" runat="server" autocomplete="off" 
                            ng-class="{hidden: payments.accountIsTokenized}" ng-model="payments.cardInfo.account" ng-pattern="/^[0-9]{13,19}$/"/>
                        <asp:RequiredFieldValidator ID="rfvCreditCardNumber" runat="server" ControlToValidate="tbCardNumber"
                            Display="None" ErrorMessage="You have not entered a credit card number." />
                        <asp:HiddenField runat="server" ID="hfOrderBillToId" />
                        <div id="dvPriorityData" runat="server" class="pp-config" style="display: none;" ms-payment-config/>
                    </td>
                </tr>
                <tr>
                    <td>Name on Card:<span class="requiredField">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="tbNameOnCard" runat="server" autocomplete="off" />
                        <asp:RequiredFieldValidator ID="rfvNameOnCard" runat="server" ControlToValidate="tbNameOnCard"
                            Display="None" ErrorMessage="You have not entered a name for the credit card." />
                    </td>
                </tr>
                <tr>
                    <td>Security Code:<span class="requiredField">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="tbCCV" Width="50px" runat="server" autocomplete="off" ng-model="payments.cardInfo.cvv"/>
                        <asp:RequiredFieldValidator ID="rfvSecurityCode" runat="server" ControlToValidate="tbCCV"
                            Display="None" ErrorMessage="You have not entered a security code for the credit card." />
                    </td>
                </tr>
                <tr>
                    <td>Expiration Date:<span class="requiredField">*</span>
                    </td>
                    <td>
                        <cc1:MonthYearPicker ID="myExpiration" runat="server" CssClass="monthYearPicker" ms-month-year="payments.setExpiration"/>
                    </td>
                </tr>
            </table>
            <hr style="width: 100%" />
            <div style="text-align: center">
                <asp:Button ID="btnContinue" runat="server" Text="Save this Card" OnClick="btnSaveCard_Click" CssClass="save-token none" />
                <input type="button" value="Save this Card" ms-submit-button="payments.process"/>
                or
                <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel " OnClick="lbCancel_Click" CausesValidation="false" />
            </div>
        </ng-form>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/bluepay.v3.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.7/angular.min.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.module.js"></script>    
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.recompile.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.monthyear.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.submit.button.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.radiobutton.bind.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.radiobutton.click.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.config.value.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.config.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.link.button.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.focus.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.controller.v3.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.priorityPayments.v2.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.bluePay.v3.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.paymentService.factory.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.priorityPaymentsTokenizer.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.cardType.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.errorReporter.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.logger.service.js"></script>
</asp:Content>
