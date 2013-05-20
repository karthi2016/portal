<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContinuingEducation.ascx.cs"
    Inherits="homepagecontrols_ContinuingEducation" %>
<div class="sectCont">
    <div class="sectHeaderTitle hIconClipboard">
        <h2>
          <asp:Literal ID="Widgets_CEU_Title" runat="server">My CEU Credits</asp:Literal> </h2>
    </div>
    <p />
    <table>
        <tr ID="Widgets_CEU_YTDCredits_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widgets_CEU_YTDCredits" runat="server">Year-to-date Credits:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblYtdCredits" runat="server">0</asp:Label>
            </td>
        </tr>
        <tr ID="Widgets_CEU_TotalCredits_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widgets_CEU_TotalCredits" runat="server">Total # of Credits:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblNumberOfCredts" runat="server">0</asp:Label>
            </td>
        </tr>
    </table>
    <ul style="margin-left: -20px">
        <ASP:HyperLink ID="Widgets_CEU_hlViewCreditHistory" runat="server" NavigateUrl="/ContinuingEducation/ViewCEUCreditHistory.aspx"><li>View My Credit History</li></ASP:HyperLink>
        <ASP:HyperLink ID="Widgets_CEU_hlMyCertificationHistory" runat="server" NavigateUrl="/ContinuingEducation/ViewCertificationHistory.aspx"><li>View My Certification History</li></ASP:HyperLink>
        <ASP:HyperLink ID="Widgets_CEU_hlReportCEUCredits" runat="server" NavigateUrl="/ContinuingEducation/ReportCredit.aspx"><li>Report CEU Credits</li></ASP:HyperLink>
        <ASP:HyperLink ID="Widgets_CEU_hlReportComponentAttendance" runat="server" NavigateUrl="/ContinuingEducation/ReportComponentAttendance.aspx"><li>Report Component Attendance</li></ASP:HyperLink>
        <ASP:HyperLink ID="Widgets_CEU_hlViewMyComponents" runat="server" NavigateUrl="/ContinuingEducation/ViewMyComponents.aspx"><li>View My Certification Components</li></ASP:HyperLink>
    </ul>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
    <div class="clearBothNoSPC">
    </div>
</div>
