<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="MakePayment2.aspx.cs" Inherits="financial_MakePayment2" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="bi" TagName="BillingInfo" Src="~/controls/BillingInfo.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .none {
            display: none;
        }
    </style>
    
    <script type="text/javascript" src="/js/priorityPaymentsScript/payment-processor-jquery.js"></script>
    
    <script type="text/javascript" src="/js/priorityPaymentsScript/membersuite.payment-processor.API.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/priorityPayment.logger.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/cardType-util.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/priorityPayment.ajaxAPI.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/membersuite.payment-processor-1.0.js"></script>
    
<%--    <script type="text/javascript" src="/js/priorityPaymentsScript/membersuite.payment-processor.min.js"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Enter Payment Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <span style="font-family: Arial, Helvetica, sans-serif; font-size: 16px; font-weight: bold">
    <ASP:Literal ID="lAmountDue" runat="server">Amount Due:</ASP:Literal> <ASP:Label ID="lblAmountDue" ForeColor=Green runat="server" Text="$0.00"/>
    </span>
    <div class="sectHeaderTitle" style="margin-top: 20px">
        <h2>
            <asp:Literal ID="lShippingInfo" runat="Server">How would you like to pay?</asp:Literal></h2>
    </div>
    <asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true"
        Font-Bold="true" ForeColor="red" ShowMessageBox="false" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    <asp:Literal ID="PageText" runat="server">
    </asp:Literal>
    <asp:Literal ID="lMessages" runat="server" />
    <bi:BillingInfo ID="BillingInfoWidget" runat="server" />
    <asp:HiddenField runat="server" ID="hfOrderBillToId"/>    
    <div id="dvPriorityData" runat="server" class="pp-config" style="display:none;"/>
    <hr style="width: 100%" />
    
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" runat="server" Text="Process Payment" OnClick="btnContinue_Click" CssClass="save-token none"/>
        <input onclick="javascript: requestToken();" type="button" value="Process Payment" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel This Payment" OnClick="lbCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript">
        function requestToken() {
            var config = JSON.parse($('.pp-config').text());
            var isPP = config.IsPreferredConfigured;

            var saveBtnId = '.save-token';
            var savedPaymentIsSelected = false;

            $('#divWithGridViewOrRepeater input').each(function () {
                if (savedPaymentIsSelected) return;
                savedPaymentIsSelected = $(this).prop('checked');
            });

            if (savedPaymentIsSelected) {
                $(saveBtnId).trigger('click');
                return false;
            };

            var hasCCNum = $('[id$="tbCardNumber"]').val() != '';
            var hasBANum = $('[id$="tbBankAccountNumber]').val() != '';

            if (!(hasCCNum || hasBANum) || !isPP) {
                $(saveBtnId).trigger('click');
                return false;
            };

            if (!config.State) {
                config.State = 'No State/Province';
            }

            var id = $('[id$="hfOrderBillToId"]').val();

            if (hasCCNum) {
                var $cardNumberElem = $('.cc-number');
                var $expiryMonthElem = $('.mypMonth');
                var $expiryYearElem = $('.mypYear');

            var parms = {
                ppConfig: config,
                msConfig: {
                    guid: id,
                    $cardNumberElem: $cardNumberElem,
                    $expiryMonthElem: $expiryMonthElem,
                    $expiryYearElem: $expiryYearElem,
                    saveBtnId: saveBtnId
                }
            }

                membersuite.paymentProcessor.init(parms);
            } else if (hasBANum) {
                var $bankAccElement = $('.ba-number');
                var $bankAccElementConfirm = $('.ba-number-confirm');
                var $rtnNumElement = $('.rtn-number');
                var $bankAccTypeElement = $('.ba-type');

                if ($bankAccElementConfirm.length > 0 && ($bankAccElement.val() != $bankAccElementConfirm.val())) {
                    $(saveBtnId).trigger('click');

                    return false;
                }

                var parms = {
                    ppConfig: config,
                    msConfig: {
                        guid: id,
                        $bankAccElement: $bankAccElement,
                        $rtnNumElement: $rtnNumElement,
                        $bankAccTypeElement: $bankAccTypeElement,
                        $bankAccElementConfirm: $bankAccElementConfirm,
                        saveBtnId: saveBtnId                        
                    }
                }

                membersuite.paymentProcessor.init(parms);
            }

            return false;
        };
    </script>
</asp:Content>
