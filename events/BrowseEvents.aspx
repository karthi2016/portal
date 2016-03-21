<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="BrowseEvents.aspx.cs" Inherits="events_BrowseEvents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="BrowseEvents.aspx">Browse Events</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Upcoming Events<asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <asp:Literal ID="lNoUpcomingEvents" runat="server">
    <span style="color:Red">There are no upcoming events.</span>
    </asp:Literal>
    <asp:Repeater ID="rptEvents" runat="server" OnItemDataBound="rptEvents_OnItemDataBound"
         >
        <ItemTemplate>
            <div style="padding-top: 10px; padding-bottom: 50px">
                <h3>
                    <asp:HyperLink ID="hlEventName" runat="server" /></h3>
                <asp:Literal ID="lEventTime" runat="server" /><br />
                <i>
                    <asp:Literal ID="lEventLocation" runat="server" /></i>
                <asp:PlaceHolder ID="phDescription" runat="server" Visible="false">
                    <hr />
                    <asp:Literal ID="lEventDescription" runat="server" />
                    <asp:HyperLink ID="hlMore" runat="server" Text="learn more..." />
                </asp:PlaceHolder>
            </div>
            <hr style="align: center; width: 150px" />
        </ItemTemplate>
    </asp:Repeater>
    <asp:Panel ID="pnlSelectType" runat="server" Visible="false">
        <h2>
            <asp:Literal runat="server" ID="SelectEventHeader">Select an event category:</asp:Literal>
        </h2>
        <ul>
            <asp:Repeater ID="rptEventCategory" runat='server'>
                <ItemTemplate>
                    <li><a href="BrowseEvents.aspx?contextID=<%#DataBinder.Eval(Container.DataItem,"ID") %>">
                    <%#DataBinder.Eval(Container.DataItem,"Name") %></a> <%#DataBinder.Eval(Container.DataItem,"Description") %></li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </asp:Panel>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
