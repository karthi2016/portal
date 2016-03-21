<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewSection.aspx.cs" Inherits="sections_ViewSection" %>

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
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                Section Information</h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td width="50%" valign="top">
                        <table>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lSectionID" runat="server">Section ID:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult( drTargetSection, "LocalID", null ) %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lSectionName" runat="server">Section Name:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult( drTargetSection, "Name", null ) %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lActiveMemberCount" runat="server">Active Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <asp:HyperLink runat="server" ID="hlActiveMembers" Visible="false">
                                        <%=GetSearchResult(drTargetSection, "ActiveMemberCount", null)%>
                                    </asp:HyperLink>
                                    <asp:Label ID="lblActiveMembers" runat="server">
                                        <%=GetSearchResult(drTargetSection, "ActiveMemberCount", null)%>
                                    </asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trExpiredMembers" visible="false">
                                <td class="columnHeader">
                                    <asp:Literal ID="lExpiredMemberCount" runat="server">Expired Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <a href="/sections/ViewSectionMembers_SelectFields.aspx?contextID=<%=GetSearchResult( drTargetSection, "ID", null ) %>&continue=true&filter=expired">
                                        <%=getExpiredMemberCount() %>
                                    </a>
                                </td>
                            </tr>
                            <tr runat="server" id="trTotalMembers" visible="false">
                                <td class="columnHeader">
                                    <asp:Literal ID="lTotalMemberCount" runat="server">Total Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <a href="/sections/ViewSectionMembers_SelectFields.aspx?contextID=<%=GetSearchResult( drTargetSection, "ID", null ) %>&continue=true">
                                        <%=GetSearchResult(drTargetSection, "TotalMemberCount", null)%>
                                    </a>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <h3>
                            <asp:Literal ID="lAddress" runat="server">Address</asp:Literal></h3>
                        <table>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lAddressLine1" runat="server">Line 1:</asp:Literal>
                                </td>
                                <td>
                                    <%=sectionAddress.Line1 %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lAddressLine2" runat="server">Line 2:</asp:Literal>
                                </td>
                                <td>
                                    <%=sectionAddress.Line2 %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lAddressCity" runat="server">City:</asp:Literal>
                                </td>
                                <td>
                                    <%=sectionAddress.City %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lAddressState" runat="server">State:</asp:Literal>
                                </td>
                                <td>
                                    <%=sectionAddress.State %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lAddressPostalCode" runat="server">Postal Code:</asp:Literal>
                                </td>
                                <td>
                                    <%=sectionAddress.PostalCode %>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <%=GetSearchResult( drTargetSection, "Description", null ) %>
    <div class="section" id="divOtherInformation" runat="server">
        <div class="sectionContent">
            <uc1:CustomFieldSet ID="cfsSectionFields" runat="server" EditMode="false" />
        </div>
    </div>
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSectionCommittees" runat="server">Section Committees</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvSectionCommittees" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No committees found.">
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
                <asp:Literal ID="lSectionEvents" runat="server">Section Events</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvSectionEvents" runat="server" GridLines="None" AutoGenerateColumns="false"
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
    <div class="section" id="divSectionLeaderTasks" runat="server" visible="false">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSectionLeaderTasks" runat="server">Section Leader Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:BulletedList ID="blLeaderTasks" runat="server" DisplayMode="LinkButton" OnClick="blLeaderTasks_Click" />
        </div>
    </div>
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSectionTasks" runat="server">Section Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <asp:HyperLink ID="hlDiscussionBoard" runat="server" NavigateUrl="/discussions/ViewDiscussionBoard.aspx?contextID=" ><li>View Discussion Board</li></asp:HyperLink>
                <asp:HyperLink ID="hlViewDocuments" runat="server" NavigateUrl="/chapters/ViewMembershipDocuments.aspx?contextID="><li>View Section Documents</li></asp:HyperLink>
                <li>
                    <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
