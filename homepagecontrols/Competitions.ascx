<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Competitions.ascx.cs"
    Inherits="homepagecontrols_Competitions" %>
<%@ Import Namespace="MemberSuite.SDK.Types" %>
<div class="sectCont" runat="server" id="divCompetitions">
    <div class="sectHeaderTitle hIconCommittees">
        <h2>
            <asp:Literal ID="Widget_Awards_Title" runat="server">Awards & Competitions</asp:Literal>
        </h2>
    </div>
    <table>
        <tr ID="Widget_Awards_LastCompEntry_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_Awards_LastCompEntry" runat="server">Last Competition Entry:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblLastCompetitionEntry" runat="server">No Records Found.</asp:Label>
            </td>
        </tr>
        <tr ID="Widget_Awards_NumberOfEntries_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_Awards_NumberOfEntries" runat="server">Number of Entries You’ve Submitted:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblCompetitionEntryCount" runat="server" />
            </td>
        </tr>
    </table>
    <ul style="margin-left: -20px">
        <asp:HyperLink ID="Widget_Awards_hlViewOpenCompetitions" runat="server" NavigateUrl="/competitions/BrowseCompetitions.aspx"><li>View Open Competitions</li></asp:HyperLink>
        <asp:HyperLink ID="Widget_Awards_hlViewMyEntries" runat="server" NavigateUrl="/competitions/ViewMyCompetitionEntries.aspx"><li>View My Competition Entries</li></asp:HyperLink>
        <asp:Repeater ID="rptJudgingTeams" runat="server">
            <ItemTemplate>
                <li><a href="/competitions/JudgeEntries.aspx?contextID=<%#((System.Data.DataRowView)Container.DataItem)["Team.ID"]  %>">
                    Judge Entries for
                    <%# ((System.Data.DataRowView)Container.DataItem)["Team.Competition.Name"]%>
                </a></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
</div>
