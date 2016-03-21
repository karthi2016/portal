<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewChapterMembers_Results.aspx.cs" Inherits="chapters_ViewChapterMembers_Results" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="/chapters/ViewChapter.aspx?contextID=<%=targetChapter.ID %>"><%=targetChapter.Name %> ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server"></asp:Literal>
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <center><asp:Label runat="server" ID="lblSearchResultCount" CssClass="columnHeader" /></center>
            <p></p>
            <asp:GridView ID="gvMembers" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="No active members were found matching the specified criteria." />
        </div>
    </div>

      <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
              
                <li><a href="ViewChapter.aspx?contextID=<%=ContextID %>"><asp:Literal ID="Literal1" runat="server" >Back to Chapter</asp:Literal> </a></li>
                <li><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>
     
 
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
