<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewScores.aspx.cs" Inherits="competitions_ViewScores"
    MasterPageFile="~/App_Master/GeneralPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <ASP:Literal ID="lScoresSummary" runat="Server">
                Scores Summary</ASP:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader" width="150px">
                        <ASP:Literal ID="lCombined" runat="Server">Combined Score:</ASP:Literal>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblCombinedScore" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <ASP:Literal ID="lAverage" runat="Server">Average Score:</ASP:Literal>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblAverageScore" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <ASP:Literal ID="lSpread" runat="Server">Spread:</ASP:Literal>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblSpread" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <ASP:Literal ID="lScores" runat="Server">Scores</ASP:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvScoringSummary" runat="server" GridLines="Both" AutoGenerateColumns="false" OnRowDataBound="gvScoringSummary_RowDataBound" ShowFooter="true" FooterStyle-CssClass="columnHeader">
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
            <li><asp:HyperLink ID="hlReviewEntries" runat="server">Review Entries</asp:HyperLink>
            </li>
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
