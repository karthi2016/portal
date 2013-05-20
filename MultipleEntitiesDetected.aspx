<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="MultipleEntitiesDetected.aspx.cs" Inherits="MultipleEntitiesDetected" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
Multiple Identities Detected
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<p><asp:Literal ID="PageText" runat="server">
It looks like the account you’ve logged into has the right to 
manage more than one customer in our system. Please choose the customer you’d like to login as.  
When you login as this customer, everything you do will be on behalf of this customer - this includes
making payments, viewing invoices, registering for events. You can switch between identities
by using the drop down list at the top of your screen.</asp:Literal>
</p>
<h3> <asp:Literal ID="lSelectAnIdentity" runat="server">Select an identity:</asp:Literal></h3>
<asp:RadioButtonList ID="rblUsers" runat="server"></asp:RadioButtonList>

<hr />

<asp:Button ID="btnContinue" runat="server" Text="Continue" 
        onclick="btnContinue_Click" />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

