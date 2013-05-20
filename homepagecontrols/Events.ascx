<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Events.ascx.cs" Inherits="homepagecontrols_Events" %>
<div class="sectCont" runat="server" id="divEvents">
    <div class="sectHeaderTitle hIconCal">
        <h2>
            <asp:Literal ID="Widget_Events_Title" runat="server">Events</asp:Literal>
        </h2>
    </div>
    <table>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="Widget_Events_LastRegistration" runat="server">Last Registration:</asp:Literal>
            </td>
            <td>
                <asp:HyperLink ID="lbLastRegistration" Enabled="false" runat="server" Text="No registration found" />
            </td>
        </tr>
    </table>
    <ul style="margin-left: -20px">
        <asp:Repeater ID="rptFeaturedEvents" runat="server">
            <ItemTemplate>
                <li><a href="/events/ViewEvent.aspx?contextID=<%# DataBinder.Eval( Container.DataItem, "ID") %>">
                    <%# DataBinder.Eval( Container.DataItem, "Name") %></a></li>
            </ItemTemplate>
        </asp:Repeater>
        <asp:HyperLink ID="Widget_Events_hlBrowseEvents" runat="server" NavigateUrl="/events/BrowseEvents.aspx"> <li>Browse Events</li></asp:HyperLink>
        <asp:HyperLink ID="Widget_Events_ViewEventRegistrations" runat="server" NavigateUrl="/events/ViewMyRegistrations.aspx"> <li>View My Event Registrations</li></asp:HyperLink>
    </ul>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
</div>
