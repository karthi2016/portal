<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="SearchResume_Results.aspx.cs" Inherits="careercenter_SearchResume_Results" %>

 <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="/careercenter/SearchResume_Criteria.aspx">Search Resumes ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
   Resume Bank Search Results
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <center><asp:Label runat="server" ID="lblSearchResultCount" CssClass="columnHeader" /></center>
            <p></p>
            <asp:GridView ID="gvMembers" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="No records found." />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><ASP:Literal ID="lTasks" runat="Server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
            <li><asp:LinkButton runat="server" ID="lbNewSearch" OnClick="lbNewSearch_Click" Text="New Search"/></li>
            <li> <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink></li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
