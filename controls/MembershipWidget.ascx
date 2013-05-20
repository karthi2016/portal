<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MembershipWidget.ascx.cs"
    Inherits="controls_MembershipWidget" %>
<div class="sectCont" runat="server" id="divMembership">
    <div class="sectHeaderTitle hIconClipboard">
        <h2>
            My Membership</h2>
    </div>
    <p />
    <table>
        <tr>
            <td class="columnHeader">
                Status:
            </td>
            <td>
                <asp:Label ID="lblMembershipStatus" runat="server" Text='<%# MembershipStatusLabelText  %>' />
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                Member Since:
            </td>
            <td>
                <asp:Label ID="lblMembershipJoinDate" runat="server" Text='<%# JoinDateLabelText  %>' />
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                Expiration:
            </td>
            <td>
                <asp:Label ID="lblMembershipExpiration" ForeColor="Green" runat="server" Text='<%# ExpirationLabelText  %>' />
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                Type:
            </td>
            <td>
                <asp:Label ID="lblMembershipMembershipType" runat="server" Text='<%# MembershipTypeLabelText  %>' />
            </td>
        </tr>
        <tr id="trChapter" runat="server" visible="false">
            <td class="columnHeader">
                Chapter:
            </td>
            <td>
                <asp:HyperLink ID="hlChapter" NavigateUrl='<%# PrimaryChapterNavigateUrl %>' Text='<%# PrimaryChapterLabelText  %>' runat="server" />
            </td>
        </tr>
    </table>
    <ul style="margin-left: -20px">
        <li id="liJoinRenew" runat="server">
            <asp:HyperLink ID="hlPurchaseMembership" runat="server" Text="Join/Renew My Membership" /></li>
        <li id="liViewMembership" runat="server" visible="false">
            <asp:HyperLink ID="hlViewMembership" NavigateUrl="~/membership/ViewMembership.aspx"
                runat="server" Text="View My Primary Membership" /></li>
        <li id="liViewAllMemberships" runat="server" visible="false">
            <asp:HyperLink ID="hlViewAllMemberships" runat="server" Text="View All Memberships" /></li>
        <li id="liSearchMembershipDirectory" runat="server">
            <asp:HyperLink ID="hlSearchMembershipDirectory" NavigateUrl="~/directory/SearchDirectory_Criteria.aspx"
                runat="server" Text="Search Membership Directory" /></li>
        <asp:Repeater ID="rptChapterMembership" runat="server">
            <ItemTemplate>
                <li><a href="/chapters/ViewChapter.aspx?contextID=<%#((System.Data.DataRowView)Container.DataItem)["Chapter.ID"] %>">
                    View
                    <%# ((System.Data.DataRowView)Container.DataItem)["Chapter.Name"]%>
                    Chapter</a></li>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptChapterLeadership" runat="server">
            <ItemTemplate>
                <li><a href="/chapters/ViewChapter.aspx?contextID=<%#((System.Data.DataRowView)Container.DataItem)["Chapter.ID"] %>">
                    View
                    <%# ((System.Data.DataRowView)Container.DataItem)["Chapter.Name"]%>
                    Chapter</a></li>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptSectionMembership" runat="server">
            <ItemTemplate>
                <li><a href="/sections/ViewSection.aspx?contextID=<%#((System.Data.DataRowView)Container.DataItem)["Section.ID"] %>">
                    View
                    <%# ((System.Data.DataRowView)Container.DataItem)["Section.Name"]%>
                    Section</a></li>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptSectionLeadership" runat="server">
            <ItemTemplate>
                <li><a href="/sections/ViewSection.aspx?contextID=<%#((System.Data.DataRowView)Container.DataItem)["Section.ID"] %>">
                    View
                    <%# ((System.Data.DataRowView)Container.DataItem)["Section.Name"]%>
                    Section</a></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <div class="clearBothNoSPC">
    </div>
</div>
