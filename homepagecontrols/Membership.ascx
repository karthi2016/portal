<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Membership.ascx.cs" Inherits="homepagecontrols_Membership" %>
<!-- my membership -->
<div class="sectCont" runat="server" id="divMembership">
    <div class="sectHeaderTitle hIconClipboard">
        <h2>
            <asp:Literal ID="Widget_MyMembership_Title" runat="Server">My Membership</asp:Literal></h2>
    </div>
    <p />
    <table>
        <tr ID="Widget_MyMembership_Status_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_MyMembership_Status" runat="Server">Status:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblMembershipStatus" runat="server">-</asp:Label>
            </td>
        </tr>
        <tr ID="Widget_MyMembership_MemberSince_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_MyMembership_MemberSince" runat="Server">Member Since:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblMembershipJoinDate" runat="server">Not a Member</asp:Label>
            </td>
        </tr>
        <tr ID="Widget_MyMembership_Expiration_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_MyMembership_Expiration" runat="Server">Expiration:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblMembershipExpiration" CssClass="hlte" runat="server">-</asp:Label>
            </td>
        </tr>
        <tr ID="Widget_MyMembership_Type_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_MyMembership_Type" runat="Server">Type:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblMembershipMembershipType" runat="server">-</asp:Label>
            </td>
        </tr>
        <tr id="trChapter" runat="server" visible="false">
            <td class="columnHeader">
                <asp:Literal ID="Widget_MyMembership_Chapter" runat="Server">Chapter:</asp:Literal>
            </td>
            <td>
                <asp:HyperLink ID="hlChapter" NavigateUrl="~/chapters/ViewChapter.aspx" runat="server">-</asp:HyperLink>
            </td>
        </tr>
        <asp:Repeater ID="rptPrimaryOrganizationalLayers" runat="server">
            <ItemTemplate>
                <tr>
                    <td class="columnHeader">
                        <%# ((System.Data.DataRowView)Container.DataItem)["Type.Name"]%>:
                    </td>
                    <td>
                        <a href="/organizationallayers/ViewOrganizationalLayer.aspx?contextID=<%# ((System.Data.DataRowView)Container.DataItem)["ID"]%>"><%# ((System.Data.DataRowView)Container.DataItem)["Name"]%></a>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    <ul style="margin-left: -20px">
      <asp:Repeater ID="rptMemOrgs" runat="server">
            <ItemTemplate>
                <li><a href="/membership/Join.aspx?contextID=<%#((System.Data.DataRowView)Container.DataItem)["ID"] %>">
                    Join/Renew
                    <%# ((System.Data.DataRowView)Container.DataItem)["Name"]%>
                </a></li>
            </ItemTemplate>
        </asp:Repeater>
        <%-- <li id="liJoinRenew" runat="server">
            <asp:HyperLink ID="hlPurchaseMembership" runat="server" Text="Join/Renew My Membership" /></li>--%>
        
          <%--    <li id="liViewMembership" runat="server" visible="false">
            <asp:HyperLink ID="hlViewMembership" NavigateUrl="~/membership/ViewMembership.aspx"
                runat="server" Text="View My Primary Membership" /></li>--%>
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
        <asp:Repeater ID="rptOrganizationalLayerMembership" runat="server">
            <ItemTemplate>
                <li><a href="/organizationallayers/ViewOrganizationalLayer.aspx?contextID=<%#((System.Data.DataRowView)Container.DataItem)["ID"] %>">
                    View
                    <%# ((System.Data.DataRowView)Container.DataItem)["Name"]%>
                    <%# ((System.Data.DataRowView)Container.DataItem)["Type.Name"]%></a></li>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptOrganizationalLayerLeadership" runat="server">
            <ItemTemplate>
                <li><a href="/organizationallayers/ViewOrganizationalLayer.aspx?contextID=<%#((System.Data.DataRowView)Container.DataItem)["OrganizationalLayer.ID"] %>">
                    View
                    <%# ((System.Data.DataRowView)Container.DataItem)["OrganizationalLayer.Name"]%>
                    <%# ((System.Data.DataRowView)Container.DataItem)["OrganizationalLayer.Type.Name"]%></a></li>
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
          <li id="ViewMemberships" runat="server">My Memberships
            <ol>
                <asp:Repeater ID="rptMemOrgsView" runat="server">
                    <ItemTemplate>
                        <li><a href="/membership/ViewMembership.aspx?contextID=<%#Eval("Key") %>">View
                            <%#Eval("Value") %>
                        </a></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ol>
        </li>
    </ul>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
    <div class="clearBothNoSPC">
    </div>
</div>
