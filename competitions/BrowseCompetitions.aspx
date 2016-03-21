<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="BrowseCompetitions.aspx.cs" Inherits="committees_BrowseCompetitions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Browse Open Competitions
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><asp:Literal ID="lOpenCompetitions" runat="server">
            Open Competitions</asp:Literal>
            </h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvCompetitions" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="There are no open competitions to view.">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="OpenDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Open Date" />
                    <asp:BoundField DataField="CloseDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Close Date" />
                    <asp:BoundField DataField="TimeRemaining" HeaderStyle-HorizontalAlign="Left" HeaderText="Time Before Close" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\competitions\ViewCompetition.aspx?contextID={0}" DataNavigateUrlFields="ID" Text="(view)" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
      <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
