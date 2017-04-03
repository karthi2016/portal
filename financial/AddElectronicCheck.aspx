<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="AddElectronicCheck.aspx.cs" Inherits="financial_AddElectronicCheck" %>

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
    Save a Checking Account to Your Account
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div id="ng-app" ng-cloak ng-app="msPayments" ng-controller="PaymentController as payments" ng-init="payments.paymentECheck=true">
        <ng-form name="payments.bankForm">
            <asp:Literal ID="lPageText" runat="server">Enter the checking account details below.
            <br />
            </asp:Literal>
            <br />
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
                        <asp:TextBox ID="tbRoutingNumber" runat="server" autocomplete="off" CssClass="rtn-number" ng-model="payments.bankInfo.rtnNumber" ng-pattern="/^[0-9]{9}$/"/>
                        <asp:RegularExpressionValidator ID="revRoutingNumber" ValidationExpression="^((0[0-9])|(1[0-2])|(2[1-9])|(3[0-2])|(6[1-9])|(7[0-2])|80)([0-9]{7})$"
                            ControlToValidate="tbRoutingNumber" Display="None" ErrorMessage="Please enter a valid nine digit routing number"
                            runat="server" />
                        <asp:RequiredFieldValidator ID="rfvRoutingNumber" runat="server" ControlToValidate="tbRoutingNumber"
                            Display="None" ErrorMessage="You have not entered a routing number." />
                        <asp:HiddenField runat="server" ID="hfOrderBillToId" />
                        <div id="dvPriorityData" runat="server" class="pp-config" style="display: none;" ms-payment-config="payments.config"/>
                    </td>
                    <td>
                        <asp:TextBox ID="tbBankAccountNumber" runat="server" autocomplete="off" CssClass="ba-number" ng-class="{hidden: payments.accountIsTokenized}" ng-model="payments.bankInfo.account" ng-pattern="/^[0-9]{4,17}$/"/>
                        <asp:RequiredFieldValidator ID="rfvBankAccountNumber" runat="server" ControlToValidate="tbBankAccountNumber"
                            Display="None" ErrorMessage="You have not entered a bank account number." />
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
                        <asp:TextBox ID="tbBankAccountNumberConfirm" runat="server" CssClass="ba-number-confirm" ng-model="payments.bankInfo.accountConfirm" ms-match="payments.bankInfo.account"/>
                        <asp:RequiredFieldValidator ID="rfvBankAccountNumberConfirm" runat="server" ControlToValidate="tbBankAccountNumberConfirm"
                            Display="None" ErrorMessage="You must re-enter your bank account number for confirmation." />
                    </td>
                </tr>
            </table>
            <hr style="width: 100%" />
            <div style="text-align: center">
                <asp:Button ID="btnContinue" runat="server" Text="Save this Account" OnClick="btnSaveCard_Click" CssClass="save-token none"/>
                <input type="button" value="Save this Account" ms-submit-button="payments.process"/>
                or
                <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel " OnClick="lbCancel_Click" CausesValidation="False" />
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
