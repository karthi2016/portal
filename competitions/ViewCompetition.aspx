<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewCompetition.aspx.cs" Inherits="events_ViewCompetition" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Competition
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <asp:Label runat="server" ID="lblExistingEntries" CssClass="redHighlight" Visible="false" /><br />
    <asp:Label runat="server" ID="lblDraftEntries" CssClass="redHighlight" Visible="false" /> <asp:HyperLink runat="server" ID="hlViewMyCompetitionEntries" Text="Click here to view." NavigateUrl="~/competitions/ViewMyCompetitionEntries.aspx" Visible="false" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><%=targetCompetition.Name%></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">Open:</td>
                    <td><%=targetCompetition.OpenDate.ToString("F") %> - <%=targetCompetition.CloseDate.ToString("F")%></td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
            <asp:Literal ID="lDescription" runat="server">
                Description
                </asp:Literal>
                </h2>
        </div>
        <div class="sectionContent">
            <%=targetCompetition.Description ?? "No description has been provided."%>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
            <asp:Literal ID="lTasks" runat="server">
                Competition Tasks
                </asp:Literal>
                </h2>
        </div>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <li runat="server" id="liEnterCompetition"><a href="ApplyToCompetition.aspx?contextID=<%=targetCompetition.ID %>">  <asp:Literal ID="lApplyNow" runat="server">Apply Now!</asp:Literal></a></li>
                <li runat="server" id="liJudgeEntries"><a href="JudgeEntries.aspx?contextID=<%=judgingTeamID %>"><asp:Literal ID="lJudgeEntries" runat="server">Judge Entries</asp:Literal></a></li>
                 <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
