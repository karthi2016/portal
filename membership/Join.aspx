<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="Join.aspx.cs" Inherits="membership_Join" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Multiple Identities Found
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="sectionHeaderTitle">
        <asp:Literal ID="lYouAreTiedToMultipleOrgs" runat="server">
        You are tied to 1 or more other organization(s). Are joining/renewing for yourself as an
        individual member, or on behalf of another organization?
        </asp:Literal>
    </div>
    <div class="sectionContent">
        <asp:RadioButtonList runat="server" ID="rblEntity" AppendDataBoundItems="true" DataTextFormatString="I am joining/renewing on behalf of <b>{0}</b>" DataTextField="Name" DataValueField="ID" />
    </div>
    <p></p>
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Continue" OnClick="btnContinue_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
