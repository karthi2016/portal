<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="ForgotPassword.aspx.cs" Inherits="profile_ForgotPassword" %>

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
    Forgot Password
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <div id="colLeft">
        <div class="sectCont">
            <p><asp:Literal ID="PageText" runat="server">
                In order to locate your information please enter your login ID or e-mail address
                below.</asp:Literal></p>
            <table>
                <tr>
                    <td colspan="2">
                    <asp:Label ID="lblNotFound" CssClass="redHighlight" runat="server" Visible="false">The specified login ID or e-mail address could not be found.</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lLoginID" runat="server">Login ID/E-mail Address:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox ID="tbLoginID" Style="width: 160px" runat="server" TabIndex="10" /><font
                            color="red">*</font>
                        <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ErrorMessage="Please enter your login ID"
                            ControlToValidate="tbLoginID" Display="None" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button runat="server" ID="btnContinue" OnClick="btnContinue_Click" TabIndex="40"
                            CausesValidation="true" Text="Continue" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
