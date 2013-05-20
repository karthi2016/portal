<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ManageSectionEvents.aspx.cs" Inherits="sections_ManageSectionEvents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="/sections/ViewSection.aspx?contextID=<%=targetSection.ID %>"><%=targetSection.Name%> ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Manage Section Events
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <%= targetSection.Name %>
                 <asp:Literal ID="lTitleEvents" runat="server">Events</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvEvents" runat="server" GridLines="None" AutoGenerateColumns="false"  EmptyDataText="No events found." >
                <Columns>
                    <asp:BoundField DataField="StartDate" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="Start Date" />
                    <asp:BoundField DataField="EndDate" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="End Date" />
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\events\ViewEvent.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(view)" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\events\CreateEditEvent.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(edit)" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
            <li><a href="/events/CreateEditEvent.aspx?contextID=<%=targetSection.ID %>">Create New Event</a></li>
                <li><a href="ViewSection.aspx?contextID=<%=ContextID %>">Back to Section</a></li>
                 <li>
                    <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
