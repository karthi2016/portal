<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ChangePassword.aspx.cs" Inherits="profile_ChangePassword" %>

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
    <asp:Label ID="lblMustChange" runat="server" Font-Bold="true" ForeColor="Green" Visible="false">Welcome! Because this is your first time logging in, we will need you to change your password.</asp:Label>
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <asp:Localize ID="txtChangingPasswordFor" runat="server">Changing Password For:</asp:Localize>
            </td>
            <td>
                <%= ConciergeAPI.CurrentEntity.Name%>
            </td>
        </tr>
        <tr id="trCurrentPassword" runat="server">
            <td>
                 <asp:Localize ID="txtCurrentPassword" runat="server">Current Password:</asp:Localize><span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbCurrentPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbCurrentPassword"
                    Display="None" ErrorMessage="You haven't entered in the current password."></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvPasswordValidator" runat="server" Display="None" OnServerValidate="cvPasswordValidator_Validate"
                    ErrorMessage="The current Password does not match the Password on file."></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td>
                 <asp:Localize ID="txtNewPassword" runat="server">New Password:</asp:Localize><span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbNewPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbNewPassword"
                    Display="None" ErrorMessage="You haven't entered in the new password."></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                 <asp:Localize ID="txtConfirmNewPassword" runat="server">Confirm New Password:</asp:Localize><span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbConfirmNewPassword" runat="server" TextMode="Password" />
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="tbConfirmNewPassword"
                    ControlToValidate="tbNewPassword" Display="None" ErrorMessage="The new passwords do not match."></asp:CompareValidator>
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
