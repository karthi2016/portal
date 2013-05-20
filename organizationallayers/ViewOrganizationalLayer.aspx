<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewOrganizationalLayer.aspx.cs" Inherits="chapters_ViewOrganizationalLayer" %>

<%@ Import Namespace="System.Data" %>
<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <%=GetSearchResult( drTargetOrganizationalLayer, "Name", null ) %>
    <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %>
                <asp:Literal ID="lTitleInformation" runat="server">Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td width="50%" valign="top">
                        <table>
                            <tr>
                                <td class="columnHeader">
                                    <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %>
                                    <asp:Literal ID="lID" runat="server">ID:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult( drTargetOrganizationalLayer, "LocalID", null ) %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %>
                                    <asp:Literal ID="lName" runat="server">Name:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult( drTargetOrganizationalLayer, "Name", null ) %>
                                </td>
                            </tr>
                            <tr runat="server" id="trParentLayer">
                                <td class="columnHeader">
                                    <%=GetSearchResult( drTargetOrganizationalLayer, "ParentLayer.Type.Name", null ) %>:
                                </td>
                                <td>
                                    <a href="/organizationallayers/ViewOrganizationalLayer.aspx?contextID=<%=GetSearchResult( drTargetOrganizationalLayer, "ParentLayer", null ) %>&continue=true">
                                        <%=GetSearchResult( drTargetOrganizationalLayer, "ParentLayer.Name", null ) %></a>
                                </td>
                            </tr>
                            <tr runat="server" id="trActiveMembers" visible="false">
                                <td class="columnHeader">
                                    <asp:Literal ID="lActiveMemberCount" runat="server">Active Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <a href="/organizationallayers/ViewOrganizationalLayerMembers_SelectFields.aspx?contextID=<%=GetSearchResult( drTargetOrganizationalLayer, "ID", null ) %>&continue=true&filter=active">
                                        <%=activeMemberCount %></a>
                                </td>
                            </tr>
                            <tr runat="server" id="trExpiredMembers" visible="false">
                                <td class="columnHeader">
                                    <asp:Literal ID="lExpiredMemberCount" runat="server">Expired Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <a href="/organizationallayers/ViewOrganizationalLayerMembers_SelectFields.aspx?contextID=<%=GetSearchResult( drTargetOrganizationalLayer, "ID", null ) %>&continue=true&filter=expired">
                                        <%=memberCount - activeMemberCount %></a>
                                </td>
                            </tr>
                            <tr runat="server" id="trTotalMembers" visible="false">
                                <td class="columnHeader">
                                    <asp:Literal ID="lTotalMemberCount" runat="server">Total Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <a href="/organizationallayers/ViewOrganizationalLayerMembers_SelectFields.aspx?contextID=<%=GetSearchResult( drTargetOrganizationalLayer, "ID", null ) %>&continue=true">
                                        <%=memberCount %></a>
                                </td>
                            </tr>
                            <tr runat="server" id="trChapterCount" visible="false">
                                <td class="columnHeader">
                                    <asp:Literal ID="lChapterCount" runat="server">Chapter Count:</asp:Literal>
                                </td>
                                <td>
                                    <a href="/chapters/BrowseChapters.aspx?contextID=<%=GetSearchResult( drTargetOrganizationalLayer, "ID", null ) %>">
                                        <%=chapterCount %>
                                    </a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <%=GetSearchResult( drTargetOrganizationalLayer, "Description", null ) %>
    <div class="section" id="divOtherInformation" runat="server">
        <div class="sectionContent">
            <uc1:CustomFieldSet ID="cfsOrganizationalLayerFields" runat="server" EditMode="false" />
        </div>
    </div>
    <asp:Repeater ID="rptChildOrganizationalLayers" runat="server" OnItemDataBound="rptChildOrganizationalLayers_ItemDataBound">
        <ItemTemplate>
            <div class="section">
                <div class="sectionHeaderTitle">
                    <h2>
                        <asp:Literal ID="litChildLayerTypeName" runat="server" Text='<%#  (string)Container.DataItem  %>'></asp:Literal>(s)</h2>
                </div>
                <div class="sectionContent">
                    <asp:GridView ID="gvOrganizationalLayers" runat="server" GridLines="None" OnRowDataBound="gvOrganizationalLayers_RowDataBound"
                        AutoGenerateColumns="false" EmptyDataText='<%# string.Format("No {0}(s) found.", (string)Container.DataItem )%>'>
                        <Columns>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Name">
                                <ItemTemplate>
                                    <asp:HyperLink runat="server" ID="hlChildOrganizationalLayer" NavigateUrl='<%# string.Format(@"~\organizationallayers\ViewOrganizationalLayer.aspx?contextID={0}", ((DataRow)Container.DataItem)["ID"]) %>'
                                        Text='<%# ((DataRow)Container.DataItem)["Name"] %>' />
                                    <asp:Label runat="server" ID="lblChildOrganizationalLayer" Text='<%# ((DataRow)Container.DataItem)["Name"] %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %>
                <asp:Literal ID="lSectionCommittees" runat="server">Committees</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvOrganizationalLayerCommittees" runat="server" GridLines="None"
                AutoGenerateColumns="false" EmptyDataText="No committees found.">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\committees\ViewCommittee.aspx?contextID={0}"
                        HeaderStyle-HorizontalAlign="Left" HeaderText="Name" DataNavigateUrlFields="ID"
                        DataTextField="Name" />
                    <asp:BoundField DataField="CurrentMemberCount" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="Current Member Count" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %>
                <asp:Literal ID="lSectionEvents" runat="server">Events</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvOrganizationalLayerEvents" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No upcoming events found.">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\events\ViewEvent.aspx?contextID={0}"
                        HeaderStyle-HorizontalAlign="Left" HeaderText="Name" DataNavigateUrlFields="ID"
                        DataTextField="Name" />
                    <asp:BoundField DataField="StartDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Start Date" />
                    <asp:BoundField DataField="EndDate" HeaderStyle-HorizontalAlign="Left" HeaderText="End Date" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" id="divOrganizationalLayerLeaderTasks" runat="server" visible="false">
        <div class="sectionHeaderTitle">
            <h2>
                <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %>
                <asp:Literal ID="lLeaderTasks" runat="server">Leader Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:BulletedList ID="blLeaderTasks" runat="server" DisplayMode="LinkButton" OnClick="blLeaderTasks_Click" />
        </div>
    </div>
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <asp:HyperLink ID="hlDiscussionBoard" runat="server" NavigateUrl="/discussions/ViewDiscussionBoard.aspx?contextID="><li>View Discussion Board</li></asp:HyperLink>
                <asp:HyperLink ID="hlViewDocuments" runat="server" NavigateUrl="/chapters/ViewMembershipDocuments.aspx?contextID="><li>View Documents</li></asp:HyperLink>
                <li>
                    <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
