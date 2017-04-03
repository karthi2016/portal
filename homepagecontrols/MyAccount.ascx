<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAccount.ascx.cs" Inherits="homepagecontrols_MyAccount" %>
<div class="sectCont" runat="server" id="divFinancial">
    <div class="sectHeaderTitle hIconDollar">
        <h2>
            <asp:Literal ID="Widget_MyAccount_Title" runat="Server">My Account</asp:Literal>
        </h2>
    </div>
    <table>
        <tr id="Widget_MyAccount_OutstandingBalance_Row" runat="Server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_MyAccount_OutstandingBalance" runat="Server">Outstanding Balance:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblOutstandingBalance" runat="server" />
            </td>
        </tr>
        <!--
        <tr id="Widget_MyAccount_CreditBalance_Row" runat="Server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_MyAccount_CreditBalance" runat="Server">Credit Balance:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblCreditBalance" runat="server" />
            </td>
        </tr>
        -->
        <tr id="Widget_MyAccount_LastPayment_Row" runat="Server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_MyAccount_LastPayment" runat="Server">Last Payment:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblLastPayment" runat="server">No payments on file.</asp:Label>
            </td>
        </tr>
    </table>
    <p />
    <ul style="margin-left: -20px">
        <asp:HyperLink ID="hlMakeAPayment" runat="server" NavigateUrl="~/financial/MakePayment.aspx"
            Visible="false"><li>Make a Payment</li></asp:HyperLink>
              <asp:HyperLink ID="hlManagePaymentOptions" runat="server" NavigateUrl="~/financial/ManagePaymentOptions.aspx"
             ><li>Manage Saved Payment Options</li></asp:HyperLink>
                <asp:HyperLink ID="hlManageInstallmentPlans" runat="server" NavigateUrl="~/financial/ManageInstallmentPlans.aspx"
             ><li>Manage Installment Plans</li></asp:HyperLink>
        
        <asp:HyperLink ID="hlViewAccountHistory" NavigateUrl="~/financial/AccountHistory.aspx"
            runat="server"><li>View Account History</li></asp:HyperLink>
    </ul>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
</div>
