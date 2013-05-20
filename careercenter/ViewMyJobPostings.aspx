<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ViewMyJobPostings.aspx.cs" Inherits="careercenter_ViewMyJobPostings" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View My Job Postings
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lMyJobPostings" runat="server">My Job Postings</asp:Literal></h2>
        </div>
        <asp:Literal ID="PageText" runat="server"></asp:Literal>
        <div class="sectionContent">
            <asp:GridView ID="gvJobPostings" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="No job postings on file.">
                <Columns>
                    <asp:BoundField DataField="LocalID" HeaderStyle-HorizontalAlign="Left" HeaderText="ID" />
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="PostOn" HeaderStyle-HorizontalAlign="Left" HeaderText="Posted" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="ExpirationDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Expires"  DataFormatString="{0:d}" />
                    <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/careercenter/ViewJobPosting.aspx?contextID={0}" Text="(view)" />
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