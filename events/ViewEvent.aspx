<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewEvent.aspx.cs" Inherits="events_ViewEvent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <%=targetEvent.Name%>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lDescription" runat="server">Description</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <%=targetEvent.Description ?? "No description has been provided."%>
            <div align="center">
                <asp:Label ID="lblAbstracts" ForeColor="Red" runat="server" /><br />
                <asp:Label ID="lblRegistrationClosed" ForeColor="Red" runat="server" />
            </div>
            <asp:BulletedList ID="blInformationLinks" DisplayMode="LinkButton" OnClick="blInformationLinks_Click"
                DataTextField="Name" DataValueField="ID" runat="server" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lMyRegistrations" runat="server">My Registrations</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvRegistrations" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No registrations found.">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\events\ViewEventRegistration.aspx?contextID={0}"
                        HeaderStyle-HorizontalAlign="Left" HeaderText="Name" DataNavigateUrlFields="ID"
                        DataTextField="Name" />
                    <asp:BoundField DataField="CreatedDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Registration Date" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lEventTasks" runat="server">Event Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <asp:HyperLink runat="server" ID="hlRegistration"><LI>Register for this Event</LI></asp:HyperLink>
                <asp:Repeater ID="rptGroupRegistration" runat="server">
                    <ItemTemplate>
                        <a href='ManageGroupRegistration.aspx?contextID=<%=targetEvent.ID %>&organizationID=<%#DataBinder.Eval( Container.DataItem, "Value") %>'>
                            <li>Manage Group Registration for <b>
                                <%#DataBinder.Eval( Container.DataItem, "Name") %></b></li></a>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Repeater ID="rptExhibits" runat="server">
                 <ItemTemplate>
                 <li> <a href='/exhibits/ViewShow.aspx?contextID=<%#DataBinder.Eval(Container.DataItem,"ID") %>'>View <%#DataBinder.Eval(Container.DataItem,"Name") %> Home Page  </a>
                 </li> 
                   </ItemTemplate>
                </asp:Repeater>
                <asp:HyperLink runat="server" NavigateUrl="~/events/SubmitAbstract.aspx?contextID="
                    ID="hlSubmitAbstracts" Visible="false"><LI>Submit an Abstract</LI></asp:HyperLink>
                <asp:HyperLink runat="server" ID="hlViewMyAbstracts" NavigateUrl="~/events/ViewAbstracts.aspx?contextID="
                    Visible="false"><LI>View My Abstracts</LI></asp:HyperLink>
                <li runat="server" id="liEditThisEvent" visible="false"><a href="/events/CreateEditEvent.aspx?contextID=<%=targetEvent.ID %>">
                    <asp:Literal ID="lEditThisEvent" runat="server">Edit This Event</asp:Literal></a></li>
                <li runat="server" id="liRegisterSomeoneElse" visible="false"><a href="/events/Register_SelectMember.aspx?contextID=<%=targetEvent.ID %>">
                    <asp:Literal ID="lRegisterSomeone" runat="server">Register Someone Else for this Event</asp:Literal></a></li>
                <li runat="server" id="liSearchEventRegistrations" visible="false"><a href="/events/SearchEventRegistrations_Criteria.aspx?contextID=<%=targetEvent.ID %>">
                    <asp:Literal ID="lSearchRegistrations" runat="server">Search Event Registrations</asp:Literal></a></li>
                <asp:HyperLink ID="hlDiscussionBoard" runat="server" NavigateUrl="/discussions/ViewDiscussionBoard.aspx?contextID="><li>View Discussion Board</li></asp:HyperLink>
                <li><a href="/BrowseEvents.aspx">
                    <asp:Literal ID="lViewUpcomingEvents" runat="server">View All Upcoming Events</asp:Literal></a></li>
                <li><a href="/">
                    <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
