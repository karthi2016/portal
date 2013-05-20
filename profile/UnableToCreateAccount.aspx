<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="UnableToCreateAccount.aspx.cs" Inherits="profile_UnableToCreateAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="Server">
    Create Account
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="sectHeaderTitle">
        <h2>
             <asp:Literal ID="lWeAreSorry" runat="server">We're Sorry!</asp:Literal></h2>
    </div>
    <p><asp:Literal ID="PageText" runat="server">
        Based on the information you've entered and information already in our system we
        are unable to automatically retrieve your account. Please contact us using the contact
        page with your name and phone number so that we can reset your information.</asp:Literal>
    </p>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
