<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="RegisterForShow.aspx.cs" Inherits="exhibits_RegisterForShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server"><a href="/exhibits/ViewShow.aspx?contextID=<%=targetShow.ID %>">
        <%=targetShow.Name%>
      >  </a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">

<%=targetShow.Name%> Registration
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
 
 <asp:Literal ID="PageText" runat="server" />
  
    <div class="sectionHeaderTitle">
        <asp:Literal ID="lYouAreTiedToMultipleOrgs" runat="server">
        You are tied to 1 or more other organization(s). Are registering for yourself as an
        individual, or on behalf of another organization?
        </asp:Literal>
    </div>
    <div class="sectionContent">
        <asp:RadioButtonList runat="server" ID="rblEntity" AppendDataBoundItems="true" DataTextFormatString="I am registering on behalf of <b>{0}</b>" DataTextField="Name" DataValueField="ID" />
    </div>
    <p></p>
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Continue" OnClick="btnContinue_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

