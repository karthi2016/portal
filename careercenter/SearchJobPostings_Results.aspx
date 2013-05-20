<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="SearchJobPostings_Results.aspx.cs" Inherits="careercenter_SearchJobPostings_Results" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="/careercenter/SearchJobPostings_Criteria.aspx">Search Job Postings ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Search Job Posting Results
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <center><asp:Label runat="server" ID="lblSearchResultCount" CssClass="columnHeader" /></center>
            <p></p>
            <asp:GridView ID="gvJobPostings" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no job postings available to view.">
                <Columns>
                    <asp:BoundField DataField="CompanyName" HeaderStyle-HorizontalAlign="Left" HeaderText="Company" />
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Location.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Location" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\careercenter\ViewJobPosting.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(view)" />
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
          <li>
                <asp:LinkButton runat="server" ID="lbNewSearch" OnClick="lbNewSearch_Click" Text="New Search" /></li>
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>

   
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
