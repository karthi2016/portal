<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EmailLoginInformation.aspx.cs" Inherits="profile_EmailLoginInformation" %>

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
    Create Account
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server">
<div class="sectHeaderTitle"><h2>You're already in our system!</h2></div>
<p>Based on the information you've entered, it looks like you're already in our system. </p>
    </asp:Literal>
    <div class="sectHeaderTitle">
        <h2>
            <asp:Literal ID="lWhatNext" runat="server">What do I do next?</asp:Literal></h2>
    </div>
    <p>
        A link has already been sent to the email address on file for
        <%=targetPortalUser.EmailAddress%>
        with instructions on how to reset your password. <font class="redHighlight">Do NOT proceed
            with a different email address if
            this email
            belongs to you.</font> 
            <asp:Literal ID="lCreateDupe" runat="server">
            Doing so will create a duplicate record. Please allow
        up to 5 minutes for the email to arrive. If you do not receive it in 5 minutes,
        please make sure you spam filters have not caught it, or that it has not been inadvertently
        rerouted to your Junk Mail folder. When you receive the link, you will be able to
        login to perform the operation you were attempting.</asp:Literal>
    </p>
    <div class="sectHeaderTitle">
        <h2>
            What if I don't have access to the
            <%=targetPortalUser.EmailAddress%>
            mailbox?</h2>
    </div>
    <p>
        If you no longer have access to the
        <%=targetPortalUser.EmailAddress%>
        mailbox, please contact us using the contact page with your name and phone number
        and we can reset your password.
    </p>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
