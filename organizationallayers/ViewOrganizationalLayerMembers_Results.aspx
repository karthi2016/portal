<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewOrganizationalLayerMembers_Results.aspx.cs"
    Inherits="organizationalLayers_ViewOrganizationalLayerMembers_Results" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="/organizationalLayers/ViewOrganizationalLayer.aspx?contextID=<%=targetOrganizationalLayer.ID %>">
        <%=targetOrganizationalLayer.Name%>
        ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="organizationalLayer" style="margin-top: 10px">
        <div class="organizationalLayerContent">
            <center>
                <asp:Label runat="server" ID="lblSearchResultCount" CssClass="columnHeader" /></center>
            <p>
            </p>
            <asp:GridView ID="gvMembers" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No active members were found matching the specified criteria." />
        </div>
    </div>
    <div class="organizationalLayer" style="margin-top: 10px">
        <div class="organizationalLayerHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li><a href="/organizationallayers/ViewOrganizationalLayer.aspx?contextID=<%=ContextID %>">
                Back to
                <%=drTargetOrganizationalLayerType["Name"] %></a></li>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink></li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
