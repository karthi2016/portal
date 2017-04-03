<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EnterBillingInfo.aspx.cs" Inherits="orders_EnterBillingInfo" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="bi" TagName="BillingInfo" Src="~/controls/BillingInfo.ascx" %>
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Enter Billing Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div id="ng-app" ng-cloak ng-app="msPayments" ng-controller="PaymentController as payments">
        <span style="font-family: Arial, Helvetica, sans-serif; font-size: 16px; font-weight: bold">
            <asp:Literal ID="lAmountDue" runat="server">Amount Due:</asp:Literal>
            <asp:Label ID="lblAmountDue" CssClass="hlteMon" runat="server" Text="$0.00" />
        </span>
        <div class="sectHeaderTitle" style="margin-top: 20px">
            <h2>
                <asp:Literal ID="lShippingInfo" runat="Server">How would you like to pay?</asp:Literal></h2>
        </div>
        <asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true"
            Font-Bold="true" ForeColor="red" ShowMessageBox="false" HeaderText="We were unable to continue for the following reasons:"
            runat="server" />
        <asp:CustomValidator runat="server" ID="cvCustom" />
        <asp:Literal ID="PageText" runat="server">
        </asp:Literal>
        <asp:Literal ID="lMessages" runat="server" />
        <bi:BillingInfo ID="BillingInfoWidget" runat="server" />
        <asp:HiddenField runat="server" ID="hfOrderBillToId"/>            
        <div id="dvPriorityData" runat="server" class="pp-config" style="display:none;" ms-payment-config/>       
        <hr style="width: 100%" />
        <div style="padding-top: 20px" id="divDiscountPromoCode" runat="server">
            <div class="sectHeaderTitle">
                <h2>
                    Apply Discount/Promo Code
                </h2>
            </div>
            <table>
                <tr>
                    <td colspan="3">
                        <asp:Literal ID="lApplyDiscountPromoCode" runat="server">Apply discount/promo code:</asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="width: 50px">
                        Code:
                    </td>
                    <td>
                        <asp:TextBox ID="tbPromoCode" runat="server" Width="80px" />
                        <asp:RequiredFieldValidator ID="rfvPromoCode" runat="server" ControlToValidate="tbPromoCode"
                            ValidationGroup="DiscountCode" ErrorMessage="Please enter a discount code." Display="None" />
                        <asp:Button ID="btnApplyCoupon" ValidationGroup="DiscountCode" OnClick="btnApplyDiscountCode_Click"
                            CommandName="ApplyDiscount" runat="server" Text="Apply Code" />
                    </td>
                </tr>
            </table>
        </div>
        <hr style="width: 100%" />
        <div style="text-align: center">
            <asp:Button ID="btnContinue" runat="server" Text="Continue" OnClick="btnContinue_Click" CssClass="save-token none"/>
            <input type="button" value="Continue" ms-submit-button="payments.process"/>
            or
            <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel Your Order" CausesValidation="false"
                OnClick="lbCancel_Click" />
        </div>
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
