<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="RectifySuspendedBillingSchedule.aspx.cs" Inherits="financial_RectifySuspendedBillingSchedule" %>

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
    <a href="AccountHistory.aspx">Account History</a> &gt; <a href="ViewOrder.aspx?contextID=<%=GetSearchResult( targetSchedule,"Order", null) %>">
        View Order</a> &gt;
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Update Electronic Payment Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server" />
    <div id="ng-app" ng-cloak ng-app="msPayments" ng-controller="PaymentController as payments">
        <asp:ValidationSummary ID="vsSummary2" ForeColor="Red" Font-Bold="true" DisplayMode="BulletList"  
            ShowSummary="true" HeaderText="We were unable to continue for the following reasons:"
            runat="server" />
        <table style="width: 450px; padding-bottom: 20px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lOrderNum" runat="server" >Order #</asp:Literal>
                </td>
                <td>
                    <asp:HyperLink ID="hlOrderNo" runat="server" NavigateUrl="/financial/ViewOrder.aspx?contextID="
                        Text="21032" />
                </td>
                <td class="columnHeader">
                    <asp:Literal ID="lDate" runat="server" >Date</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblOrderDate" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lAmount" runat="server" >Amount Remaining to be Billed:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblAmountRemaining" runat="server" />
                </td>
                <td class="columnHeader">
                    <asp:Literal ID="lOrderTotal" runat="server" >Order Total:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblOrderTotal" runat="server" />
                </td>
            </tr>
        </table>
        <br />     
        <bi:BillingInfo ID="BillingInfoWidget" runat="server" />
        <asp:HiddenField runat="server" ID="hfOrderBillToId"/>    
        <div id="dvPriorityData" runat="server" class="pp-config" style="display:none;" ms-payment-config/>
        <hr />
        <div class="sectionContent">
            <div align="center" style="padding-top: 20px">
                <asp:Button ID="btnUpdatePaymentInfo" Text="Update Payment Info" runat="server" OnClick="btnUpdatePaymentInfo_Click" CssClass="save-token none"/>
                <input type="button" value="Update Payment Info" ms-submit-button="payments.process"/>
                <asp:Button ID="btnCancel" Text="Cancel" runat="server" OnClick="btnCancel_Click"
                    CausesValidation="false" />
                <div class="clearBothNoSPC">
                </div>
            </div>
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
