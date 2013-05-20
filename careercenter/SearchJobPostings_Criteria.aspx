<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="SearchJobPostings_Criteria.aspx.cs" Inherits="careercenter_SearchJobPostings_Criteria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Search Job Postings
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSearchFields" runat="server">
            Search Fields
                </asp:Literal>
            </h2>
        </div>
        <asp:Literal ID="PageText" runat="server">
        Click <B>Search</B> to see all jobs, or enter keywords to target your search to match specific words or phrases.
        </asp:Literal>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lKeyWords" runat="server">Keywords:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbKeywords" />
                    </td>
                </tr>
            </table>
            <p>
            </p>
            <div align="center">
                <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="Search" /></div>
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
