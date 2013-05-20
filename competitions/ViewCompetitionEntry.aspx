<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ViewCompetitionEntry.aspx.cs" Inherits="competitions_ViewCompetitionEntry" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Competition Entry for <%=targetCompetition.Name %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><%=targetCompetitionEntry.Name %></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">  <asp:Literal ID="lEntryID" runat="server">Entry ID:</asp:Literal></td>
                    <td><%=GetSearchResult(drCompetitionEntry, "LocalID")%></td>
                </tr>
                <tr>
                    <td class="columnHeader">Entry Name:</td>
                    <td><%=GetSearchResult(drCompetitionEntry, "Name")%></td>
                </tr>
                <tr>
                    <td class="columnHeader">Entrant:</td>
                    <td><%=GetSearchResult(drCompetitionEntry, "Entrant.Name")%></td>
                </tr>
                <tr>
                    <td class="columnHeader">Entrant Organization:</td>
                    <td><%=GetSearchResult(drCompetitionEntry, "Individual.PrimaryOrganization.Name")%></td>
                </tr>
                <tr>
                    <td class="columnHeader">Organization City:</td>
                    <td><%=GetSearchResult(drCompetitionEntry, "Individual.PrimaryOrganization._Preferred_Address_City")%></td>
                </tr>
                <tr>
                    <td class="columnHeader">Organization State:</td>
                    <td><%=GetSearchResult(drCompetitionEntry, "Individual.PrimaryOrganization._Preferred_Address_State")%></td>
                </tr>
                <tr>
                    <td class="columnHeader">  <asp:Literal ID="lEntryStatus" runat="server">Status:</asp:Literal></td>
                    <td><%=targetEntryStatus != null ? targetEntryStatus.Name : "Unassigned" %></td>
                </tr>
                <tr>
                    <td class="columnHeader">  <asp:Literal ID="lSubmittedOn" runat="server">Submitted On:</asp:Literal></td>
                    <td><%=GetSearchResult(drCompetitionEntry, "DateSubmitted") %></td>
                </tr>
                <tr>
                    <td class="columnHeader">Score:</td>
                    <td><%=totalScore%></td>
                </tr>
                <tr>
                    <td class="columnHeader">Average Score:</td>
                    <td><%=averageScore%></td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>  <asp:Literal ID="lRegistrationQuestions" runat="server">
                Registration Questions</asp:Literal>
                </h2>
        </div>
        <div class="sectionContent">
            <uc1:CustomFieldSet ID="cfsEntryFields" EditMode="false" runat="server" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>  <asp:Literal ID="lTasks" runat="server">
                Tasks</asp:Literal>
                </h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li><a href="/competitions/ViewMyCompetitionEntries.aspx"><asp:Literal ID="lBackToMyEntries" runat="server">Back to My Competition Entries</asp:Literal></a></li>
                <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
