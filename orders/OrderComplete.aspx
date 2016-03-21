<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="OrderComplete.aspx.cs" Inherits="orders_OrderComplete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
    Order #<asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<asp:Literal ID="PageText" runat="server" />
<asp:Label runat="server" ID="lblConfirmationText" />
<asp:Literal ID="lDigitalItems" runat="server">
<p>
Please note that if you purchased any digital items, you may access them by clicking on the <b>My Digital Library</b> link on the home page.
</p>
</asp:Literal>
<ul>
<li><asp:HyperLink ID="hlViewOrder" runat="server">View Order</asp:HyperLink></li>
<li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink></li>
</ul>

</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

