<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewChapter.aspx.cs" Inherits="chapters_ViewChapter" %>

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
    <%=GetSearchResult( drTargetChapter, "Name", null ) %>
    Chapter
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server"></asp:Literal>
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lChapterInfo" runat="server">Chapter Information</asp:Literal>  </h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td width="50%" valign="top">
                        <table>
                            <tr>
                                <td class="columnHeader">
                                     <asp:Literal ID="lChapterID" runat="server">Chapter ID:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult( drTargetChapter, "LocalID", null ) %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                     <asp:Literal ID="lChapterName" runat="server">Chapter Name:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult( drTargetChapter, "Name", null ) %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                     <asp:Literal ID="lActiveMemberCount" runat="server">Active Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <asp:HyperLink runat="server" ID="hlActiveMembers" Visible="false">
                                        <%=GetSearchResult( drTargetChapter, "ActiveMemberCount", null ) %>
                                    </asp:HyperLink>
                                    <asp:Label ID="lblActiveMembers" runat="server">
                                        <%=GetSearchResult( drTargetChapter, "ActiveMemberCount", null ) %>
                                    </asp:Label>
                                </td>
                            </tr>
                            <tr runat="server" id="trExpiredMembers" visible="false">
                                <td class="columnHeader">
                                     <asp:Literal ID="lExpiredMemberCount" runat="server">Expired Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <a href="/chapters/ViewChapterMembers_SelectFields.aspx?contextID=<%=GetSearchResult( drTargetChapter, "ID", null ) %>&continue=true&filter=expired">
                                        <%=getExpiredMemberCount() %>
                                    </a>
                                </td>
                            </tr>
                            <tr runat="server" id="trTotalMembers" visible="false">
                                <td class="columnHeader">
                                     <asp:Literal ID="lTotalMemberCount" runat="server">Total Member Count:</asp:Literal>
                                </td>
                                <td>
                                    <a href="/chapters/ViewChapterMembers_SelectFields.aspx?contextID=<%=GetSearchResult( drTargetChapter, "ID", null ) %>&continue=true">
                                        <%=GetSearchResult( drTargetChapter, "TotalMemberCount", null ) %>
                                    </a>
                                </td>
                            </tr>
                            <tr runat="server" id="trChapterBalance" visible="false">
                                <td class="columnHeader">
                                     <asp:Literal ID="lChapterBalance" runat="server">Chapter Balance:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult(drLinkedOrganization, "Invoices_OpenInvoiceBalance", "C")%>
                                </td>
                            </tr>
                            <tr id="trLayer" runat="server">
                                <td class="columnHeader">
                                    <%=GetSearchResult( drTargetChapter, "Layer.Type.Name", null ) %>:
                                </td>
                                <td>
                                    <a href="/organizationallayers/ViewOrganizationalLayer.aspx?contextID=<%=GetSearchResult( drTargetChapter, "Layer", null ) %>">
                                        <%=GetSearchResult( drTargetChapter, "Layer.Name", null ) %></a>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <h3>
                             <asp:Literal ID="lAddressHeader" runat="server">Address</asp:Literal></h3>
                        <table>
                            <tr>
                                <td class="columnHeader">
                                     <asp:Literal ID="lAddressLine1" runat="server">Line 1:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult(drLinkedOrganization, "_Preferred_Address_Line1", null)%>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                     <asp:Literal ID="lAddressLine2" runat="server">Line 2:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult(drLinkedOrganization, "_Preferred_Address_Line2", null)%>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                     <asp:Literal ID="lAddressCity" runat="server">City:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult(drLinkedOrganization, "_Preferred_Address_City", null)%>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                     <asp:Literal ID="lAddressState" runat="server">State:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult(drLinkedOrganization, "_Preferred_Address_State", null)%>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                     <asp:Literal ID="lAddressPostalCode" runat="server">Postal Code:</asp:Literal>
                                </td>
                                <td>
                                    <%=GetSearchResult(drLinkedOrganization, "_Preferred_Address_PostalCode", null)%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <%=GetSearchResult( drTargetChapter, "Description", null ) %>
    <div class="section" id="divOtherInformation" runat="server">
        <div class="sectionContent">
            <uc1:CustomFieldSet ID="cfsChapterFields" runat="server" EditMode="false" />
        </div>
    </div>
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lChapterCommittees" runat="server">Chapter Committees</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvChapterCommittees" runat="server" GridLines="None" AutoGenerateColumns="false"
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
                 <asp:Literal ID="lChapterEvents" runat="server">Chapter Events</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvChapterEvents" runat="server" GridLines="None" AutoGenerateColumns="false"
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
    <div class="section" id="divChapterLeaderTasks" runat="server" visible="false">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lChapterLeaderTasks" runat="server">Chapter Leader Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:BulletedList ID="blLeaderTasks" runat="server" DisplayMode="LinkButton" OnClick="blLeaderTasks_Click" />
        </div>
    </div>
    <div class="section">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lChapterTasks" runat="server">Chapter Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <asp:HyperLink ID="hlDiscussionBoard" runat="server" NavigateUrl="/discussions/ViewDiscussionBoard.aspx?contextID=" ><li>View Discussion Board</li></asp:HyperLink>
                <asp:HyperLink ID="hlViewDocuments" runat="server" NavigateUrl="/chapters/ViewMembershipDocuments.aspx?contextID=" ><li>View Chapter Documents</li></asp:HyperLink>
                <li><a href="/"> <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
