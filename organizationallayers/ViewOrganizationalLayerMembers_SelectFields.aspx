<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewOrganizationalLayerMembers_SelectFields.aspx.cs"
    Inherits="organizationalLayers_ViewOrganizationalLayerMembers_SelectFields" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
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
    <%=targetOrganizationalLayer.Name %>
    Members
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="organizationalLayer" style="margin-top: 10px">
        <div class="organizationalLayerHeaderTitle">
            <h2>
                <asp:Literal ID="lSearchCriteria" runat="server">
                Search Criteria</asp:Literal></h2>
        </div>
        <div class="organizationalLayerContent">
            <asp:Literal ID="PageText" runat="server">
            Use the fields below to find the members you are looking for.</asp:Literal>
            <p>
            </p>
            <uc1:CustomFieldSet ID="cfsSearchCriteria" runat="server" SuppressValidation="true" />
            <p>
            </p>
        </div>
    </div>
    <div class="organizationalLayer" style="margin-top: 10px">
        <div class="organizationalLayerHeaderTitle">
            <h2>
                <asp:Literal ID="lOutputFields" runat="server">Select Output Fields</asp:Literal></h2>
        </div>
        <div class="organizationalLayerContent">
            <asp:Literal ID="lAvailableFields" runat="server">Below are a list of available fields that can be included in your search. Select
            them and click <b>Search</b> to continue.</asp:Literal>
            <p>
                &nbsp;</p>
            <cc1:DualListBox runat="server" ID="dlbOutputFields" />
        </div>
    </div>
    <div class="organizationalLayer" style="margin-top: 10px">
        <div class="organizationalLayerHeaderTitle">
            <h2>
                <asp:Literal ID="lOutputFormat" runat="server">Output Format</asp:Literal></h2>
        </div>
        <div class="organizationalLayerContent">
            <asp:RadioButtonList runat="server" ID="rblOutputFormat">
                <asp:ListItem Text="To My Screen" Value="screen" Selected="True" />
                <asp:ListItem Text="Excel" Value="download" />
            </asp:RadioButtonList>
            <div style="text-align: center; padding-top: 20px">
                <asp:Button runat="server" ID="Button2" Text="Execute Search" OnClick="btnSearch_Click" /></div>
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
