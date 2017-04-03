<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ViewMyRegistrations.aspx.cs" Inherits="events_ViewMyRegistrations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    My Event Registrations
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        
        <div class="sectionContent">
            <asp:GridView ID="gvEvents" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataRowStyle-ForeColor="Red" EmptyDataText="You have no event registrations.">
                <Columns>
                    <asp:BoundField DataField="Event.Name" HeaderText="Event" />                    
                    <asp:BoundField DataField="Event.StartDate" DataFormatString="{0:d}" HeaderStyle-HorizontalAlign="Left" HeaderText="Event Date" />
                    <asp:BoundField DataField="CreatedDate" HeaderText="Registration Date" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\events\Register_CreateRegistration.aspx?contextID={0}" DataNavigateUrlFields="ID" Text="(change sessions)" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\events\ViewEventRegistration.aspx?contextID={0}" DataNavigateUrlFields="ID" Text="(view)" />
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
