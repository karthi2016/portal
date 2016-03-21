<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="AddElectronicCheck.aspx.cs" Inherits="financial_AddElectronicCheck" %>

<%@ Register TagPrefix="cc1" Namespace="MemberSuite.SDK.Web.Controls" Assembly="MemberSuite.SDK.Web" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="/js/priorityPaymentsScript/payment-processor-jquery.js"></script>
    
    <script type="text/javascript" src="/js/priorityPaymentsScript/membersuite.payment-processor.API.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/priorityPayment.logger.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/cardType-util.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/priorityPayment.ajaxAPI.js"></script>
    <script type="text/javascript" src="/js/priorityPaymentsScript/membersuite.payment-processor-1.0.js"></script>    
    
    <style type="text/css">
        .none {
            display: none;
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
                <asp:TextBox ID="tbRoutingNumber" runat="server" autocomplete="off" CssClass="rtn-number"/>
                <asp:RegularExpressionValidator ID="revRoutingNumber" ValidationExpression="^((0[0-9])|(1[0-2])|(2[1-9])|(3[0-2])|(6[1-9])|(7[0-2])|80)([0-9]{7})$"
                    ControlToValidate="tbRoutingNumber" Display="None" ErrorMessage="Please enter a valid nine digit routing number"
                    runat="server" />
                <asp:RequiredFieldValidator ID="rfvRoutingNumber" runat="server" ControlToValidate="tbRoutingNumber"
                    Display="None" ErrorMessage="You have not entered a routing number." />
                <asp:HiddenField runat="server" ID="hfOrderBillToId" />
                <div id="dvPriorityData" runat="server" class="pp-config" style="display: none;" />
            </td>
            <td>
                <asp:TextBox ID="tbBankAccountNumber" runat="server" autocomplete="off" CssClass="ba-number"/>
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
                <asp:DropDownList runat="server" ID="rblBankAccountType" CssClass="ba-type">
                    <asp:ListItem Text="Checking" Value="checking" Selected="True" />
                    <asp:ListItem Text="Savings" Value="savings" />
                </asp:DropDownList>                        
            </td>            
            <td>
                <asp:TextBox ID="tbBankAccountNumberConfirm" runat="server" CssClass="ba-number-confirm"/>
                <asp:CompareValidator ID="cvAccountNumber" ControlToValidate="tbBankAccountNumberConfirm"
                    ControlToCompare="tbBankAccountNumber" Display="None" ErrorMessage="The second bank account number you entered for confirmation does not match the first."
                    runat="server" />
                <asp:RequiredFieldValidator ID="rfvBankAccountNumberConfirm" runat="server" ControlToValidate="tbBankAccountNumberConfirm"
                    Display="None" ErrorMessage="You must re-enter your bank account number for confirmation." />
            </td>
        </tr>
    </table>
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" runat="server" Text="Save this Account" OnClick="btnSaveCard_Click" CssClass="save-token none"/>
        <input onclick="javascript: requestToken();" type="button" value="Save this Account" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel " OnClick="lbCancel_Click" CausesValidation="False" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript">
        function requestToken() {
           
            var config = JSON.parse($('.pp-config').text());
            var isPP = config.IsPreferredConfigured;

            var hasBANum = $('[id$="tbBankAccountNumber"]').val() != '';
            var saveBtnId = '.save-token';

            if (!hasBANum || !isPP) {
                $(saveBtnId).trigger('click');
                return false;
            };

            var $bankAccElement = $('.ba-number');
            var $bankAccElementConfirm = $('.ba-number-confirm');
            var $rtnNumElement = $('.rtn-number');
            var $bankAccTypeElement = $('.ba-type');
            var id = $('[id$="hfOrderBillToId"]').val();

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

            return false;
        };
    </script>    
</asp:Content>
