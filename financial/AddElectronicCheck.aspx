<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="AddElectronicCheck.aspx.cs" Inherits="financial_AddElectronicCheck" %>

<%@ Register TagPrefix="cc1" Namespace="MemberSuite.SDK.Web.Controls" Assembly="MemberSuite.SDK.Web" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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
                <asp:TextBox ID="tbRoutingNumber" runat="server" />
                <asp:RegularExpressionValidator ID="revRoutingNumber" ValidationExpression="^((0[0-9])|(1[0-2])|(2[1-9])|(3[0-2])|(6[1-9])|(7[0-2])|80)([0-9]{7})$"
                    ControlToValidate="tbRoutingNumber" Display="None" ErrorMessage="Please enter a valid nine digit routing number"
                    runat="server" />
                <asp:RequiredFieldValidator ID="rfvRoutingNumber" runat="server" ControlToValidate="tbRoutingNumber"
                    Display="None" ErrorMessage="You have not entered a routing number." />
            </td>
            <td>
                <asp:TextBox ID="tbBankAccountNumber" runat="server" />
                <asp:RequiredFieldValidator ID="rfvBankAccountNumber" runat="server" ControlToValidate="tbBankAccountNumber"
                    Display="None" ErrorMessage="You have not entered a bank account number." />
            </td>
        </tr>
        <tr>
            <td style="width: 50px">
            </td>
            <td>
                Confirm Bank Account Number <span class="requiredField">*</span>
            </td>
        </tr>
        <tr>
            <td style="width: 50px">
            </td>
            <td>
                <asp:TextBox ID="tbBankAccountNumberConfirm" runat="server" />
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
        <asp:Button ID="btnContinue" runat="server" Text="Save this Account" OnClick="btnSaveCard_Click" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel " OnClick="lbCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
