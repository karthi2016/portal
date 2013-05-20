<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ResetPassword.aspx.cs" Inherits="profile_ResetPassword" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="Server">
    Reset Password
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
 <div id="colLeft">
    <div class="sectCont">
        <p><asp:Literal ID="PageText" runat="server">In order to complete the password reset please enter your new password below.</asp:Literal></p>

        <table>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lNewPassword" runat="server">New Password:</asp:Literal>
                </td>
                <td>
                    <asp:TextBox ID="tbPassword" Style="width: 160px" TextMode="Password" runat="server"  TabIndex="20" /><font
                        color="red">*</font>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Please enter a new password"
                        ControlToValidate="tbPassword" Display="None" />
                </td>
            </tr>
                        <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lConfirmNewPassword" runat="server">Confirm New Password:</asp:Literal>
                </td>
                <td>
                    <asp:TextBox ID="tbConfirmPassword" Style="width: 160px" TextMode="Password" runat="server" TabIndex="30" /><font
                        color="red">*</font>
                    <asp:CompareValidator ID="cvPasswordConfirm" runat="server" ErrorMessage="Password and confirmation password do not match" ControlToValidate="tbConfirmPassword" ControlToCompare="tbPassword" Display="None" />                
                </td>
            </tr>
            <tr>
            <td colspan="2" align="center"><asp:Button runat="server" ID="btnContinue" OnClick="btnContinue_Click" TabIndex="40" CausesValidation="true" Text="Continue" /></td>
            </tr>
        </table>
    </div>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
