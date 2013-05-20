<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="SearchEventRegistrations_Results.aspx.cs" Inherits="careercenter_SearchEventRegistrations_Results" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlEventOwner" Visible="false" />
    <a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name %>
        ></a> <a href="/events/SearchEventRegistrations_Criteria.aspx?contextID=<%=targetEvent.ID %>">
            Search Event Registrations ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Search
    <%=targetEvent.Name %>
    Registration Results
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <center>
                <asp:Label runat="server" ID="lblSearchResultCount" CssClass="columnHeader" /></center>
            <p>
            </p>
            <asp:GridView ID="gvEventRegistrations" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no event registrations available to view.">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Fee.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Fee" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\events\EditEventRegistration.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(edit)" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\events\ViewEventRegistration.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(view)" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li>
                <asp:LinkButton runat="server" ID="lbNewSearch" OnClick="lbNewSearch_Click" Text="New Search" /></li>
            <li><a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">Back to View
                <%=targetEvent.Name %>
                Event</a></li>
            <li runat="server" id="liEventOwnerTask" visible="false">
                <asp:HyperLink runat="server" ID="hlEventOwnerTask" /></li>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
