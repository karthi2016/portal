<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="AddSavedPaymentMethod.aspx.cs" Inherits="financial_AddSavedPaymentMethod" %>
<%@ Register TagPrefix="bi" TagName="BillingInfo" Src="~/controls/BillingInfo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .none {
            display: none;
        }
    </style>
    <script type="text/javascript" src="/js/priorityPaymentsScript/jquery-2.1.3.min.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/membersuite.payment-processor.min.js"></script>    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
    Update Payment Method - <%=targetObject.Name %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
     <asp:Literal ID="PageText" runat="server" >
         
         You can update the payment method by using one of the options below:
     </asp:Literal>
     
     <asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true"
        Font-Bold="true" ForeColor="red" ShowMessageBox="false" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    
    <bi:BillingInfo ID="BillingInfoWidget" runat="server" />    
    <asp:HiddenField runat="server" ID="hfOrderBillToId"/>    
    <div id="dvPriorityData" runat="server" class="pp-config" style="display:none;"/>
    <hr style="width: 100%" />
  
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" runat="server" Text="Save Changes" OnClick="btnContinue_Click" CssClass="save-token none"/>
        <input onclick="javascript: requestToken()" type="button" value="Save Changes" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel the Update" CausesValidation="false"
            OnClick="lbCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
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

            if (!hasCCNum || !isPP) {
                $(saveBtnId).trigger('click');
                return false;
            };

            var $cardNumberElem = $('.cc-number');
            var $expiryMonthElem = $('.mypMonth');
            var $expiryYearElem = $('.mypYear');
            var id = $('[id$="hfOrderBillToId"').val();

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
            return false;
        };
    </script>
</asp:Content>

