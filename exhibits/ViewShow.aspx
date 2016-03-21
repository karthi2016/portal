<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ViewShow.aspx.cs" Inherits="exhibits_ViewShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
<asp:HyperLink ID="hlEvent1" runat="server" Visible="true" NavigateUrl="/events/ViewEvent.aspx?contextID=" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
  <asp:Literal ID="PageText" runat="server" />
  
  <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lDescription" runat="server">Description</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <%=targetShow.Description ?? "No description has been provided."%>
             
        </div>
    </div>
     <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lEventTasks" runat="server">Exhibit Show Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <asp:HyperLink runat="server" ID="hlRegistration" NavigateUrl="~/exhibits/RegisterForShow.aspx?contextID="><LI>Register for this Show!</LI></asp:HyperLink>
                <asp:Repeater ID="rptVisibleExhibitorRecords" runat="server">
                <ItemTemplate>
                <li><a href='ViewExhibitor.aspx?contextID=<%#DataBinder.Eval( Container.DataItem,"Value") %>'>View My Exhibitor Information - <%#DataBinder.Eval( Container.DataItem,"Name") %></a></li>
                </ItemTemplate>
                </asp:Repeater>
                <asp:HyperLink runat="server" ID="hlExhibitorList" NavigateUrl="ViewExhibitorListing.aspx?contextID="><LI>View Exhibitor Listing</LI></asp:HyperLink>
                <asp:HyperLink runat="server" ID="hlDownloadShowFloor"><LI>Download Show Floor Layout</LI></asp:HyperLink>
                <li><a href="../events/BrowseEvents.aspx">
                    <asp:Literal ID="lViewUpcomingEvents" runat="server">View All Upcoming Events</asp:Literal></a></li>
                    <li id="liEvent" runat="Server" visible="false" >
                    <asp:HyperLink id="hlEvent2" runat="server" NavigateUrl="/events/ViewEvent.aspx?contextID=" />
                    </li>
                <li><a href="/">
                    <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

