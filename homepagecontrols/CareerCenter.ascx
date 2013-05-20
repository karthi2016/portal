<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CareerCenter.ascx.cs"
    Inherits="homepagecontrols_CareerCenter" %>
<%@ Import Namespace="MemberSuite.SDK.Types" %>
<div class="sectCont" runat="server" id="divCompetitions">
    <div class="sectHeaderTitle hIconCommittees">
        <h2>
            <asp:Literal ID="Widget_CareerCenter_Title" runat="server">Career Center</asp:Literal></h2>
    </div>
    <table>
        <tr  ID="Widget_CareerCenter_NumberOfResumes_Row" runat="server">
            <td class="columnHeader">
                 <asp:Literal ID="Widget_CareerCenter_NumberOfResumes" runat="server">Number of Resumes:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblNumberOfResumes" runat="server" />
            </td>
        </tr>
        <tr ID="Widget_CareerCenter_JobsPosted_Row" runat="server">
            <td class="columnHeader">
                 <asp:Literal ID="Widget_CareerCenter_JobsPosted" runat="server">Jobs Posted:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblJobsPosted" runat="server" />
            </td>
        </tr>
    </table>
    <ul style="margin-left: -20px">
        <ASP:HyperLink ID="Widget_CareerCenter_hlManageResumes" runat="server" NavigateUrl="/careercenter/ManageResumes.aspx"><li>Manage My Resumes</li></ASP:HyperLink>
        <li runat="server" id="liPostJob">
            <asp:HyperLink runat="server" ID="hlPostJob" Text="Post a Job" NavigateUrl="~/careercenter/CreateEditJobPosting.aspx" /></li>
        <li runat="server" id="liPurchaseJobPosting"><a href="/careercenter/SelectCareerCenterProduct.aspx">Purchase Job Postings</a></li>
        <li id="liSearchJobPostings" runat="server"><a href="/careercenter/SearchJobPostings_Criteria.aspx">
            Search Job Postings</a> </li>
        <li id="liSearchResumes" runat="server"><a href="/careercenter/SearchResume_Criteria.aspx">
            Search Resumes</a> </li>
        <li><a href="/careercenter/ViewMyJobPostings.aspx">View My Job Postings</a> </li>
    </ul>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
</div>
