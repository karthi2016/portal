<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ChangeLoginId.aspx.cs" Inherits="profile_ChangeLoginId" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Change Password
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                 <asp:Literal ID="litCurrentLoginId" runat="server">Current Login ID:</asp:Literal>
            </td>
            <td>
                <%= user.Name%>
            </td>
        </tr>
        <tr>
            <td>
                 <asp:Literal ID="litNewLoginId" runat="server">New Login ID:</asp:Literal><span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbNewLoginId" runat="server" />
                <asp:RequiredFieldValidator ID="rfvNewLoginId" runat="server" ControlToValidate="tbNewLoginId"
                    Display="None" ErrorMessage="You must enter a new login ID."></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
    <p />
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Text="Save Changes" />
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Cancel"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
