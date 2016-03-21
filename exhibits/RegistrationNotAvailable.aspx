<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="RegistrationNotAvailable.aspx.cs" Inherits="exhibits_RegistrationNotAvailable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">

</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">

   <asp:Literal ID="PageText" runat="server" >
   Unfortunately, registration is currently unavailable at this time for the exhibitor listed below.
   </asp:Literal>
   <br />
   <p>
   <asp:Literal ID="lExhibitor" runat="server"><B>Exhibitor: </B></asp:Literal> <asp:Label ID="lblExhibitorName" runat="server" />
   </p>
   <br />
   <a href="ViewShow.aspx?contextID=<%=targetShow.ID %>">Go Back to <%=targetShow.Name%> home page </a>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

