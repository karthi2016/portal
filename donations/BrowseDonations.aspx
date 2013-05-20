<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="BrowseDonations.aspx.cs" Inherits="events_BrowseDonations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Browse Donations
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><asp:Literal ID="lDonations" runat="server">Donations</asp:Literal></h2>
        </div>
        <asp:Literal ID="PageText" runat="server"/>
        <div class="sectionContent">
            <asp:GridView ID="gvDonations" runat="server" GridLines="None" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="Date" HeaderStyle-HorizontalAlign="Left" HeaderText="Date" />
                    <asp:BoundField DataField="Type" HeaderStyle-HorizontalAlign="Left" HeaderText="Type" />
                    <asp:BoundField DataField="Fund.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Fund" />                    
                    <asp:BoundField DataField="Amount" DataFormatString="{0:C}" HeaderStyle-HorizontalAlign="Left" HeaderText="Amount" />
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
