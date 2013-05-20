<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewMySubscriptions.aspx.cs" Inherits="subscriptions_ViewMySubscriptions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View My Subscriptions
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <asp:GridView ID="gvSubscriptions" runat="server" EmptyDataText="You currently do not have any active subscriptions."
        GridLines="None" AutoGenerateColumns="false" HeaderStyle-HorizontalAlign="Left">
        <Columns>
            <asp:BoundField DataField="Publication.Name" HeaderText="Publication" />
            <asp:BoundField DataField="StartDate" DataFormatString="{0:d}" HeaderText="Start Date" />
            <asp:BoundField DataField="ExpirationDate" DataFormatString="{0:d}"  HeaderText="Expiration Date" />
            <asp:BoundField DataField="IsActive" HeaderText="Active?" />
            <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="ViewSubscription.aspx?contextID={0}" Text="(view)" />
        </Columns>
    </asp:GridView>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <asp:HyperLink ID="hlSubsribe" NavigateUrl="~/subscriptions/Subscribe.aspx" runat="server"><li>Subscribe to a Publication</li></asp:HyperLink>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
