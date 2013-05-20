<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewMyVolunteerProfile.aspx.cs" Inherits="volunteers_ViewMyVolunteerProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View My Volunteer Profile
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <table>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lName" runat="Server">Name:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult( drSearchResults, "Individual.Name") %>
            </td>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lVolunteerTypes" runat="Server">Volunteer Type(s):</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblVolunteerTypes" runat="server" />
            </td>
            <td colspan="2">
            </td>
        </tr>
        <tr id="trSponsor" runat="server" visible="false">
            <td class="columnHeader">
                <asp:Literal ID="Literal2" runat="Server">Sponsor:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult( drSearchResults, "Sponsor.Name") %>
            </td>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lEmergContactName" runat="Server">Emergency Contact Name:</asp:Literal>
            </td>
            <td>
                <%= targetVolunteer.EmergencyContactName  %>
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lEmergContactPhone" runat="Server">Emergency Contact Phone:</asp:Literal>
            </td>
            <td>
                <%= targetVolunteer.EmergencyContactPhone  %>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lNumAssignments" runat="Server"># of Assignments:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult(drSearchResults, "NumberOfAssignments")%>
            </td>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lTotalHoursWorked" runat="Server">Total Hours Worked:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult(drSearchResults, "TotalHoursWorked")%>
            </td>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="Literal1" runat="Server">Last Assignment:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult(drSearchResults, "LastAssignment.JobOccurrence.Job.Name")%>
                on
                <%= GetSearchResult(drSearchResults, "LastAssignment.JobOccurrence.StartDateTime", "d")%>
            </td>
            <td colspan="2">
            </td>
        </tr>
    </table>
    <div id="divUnavailability" runat="server" visible="false">
        <h2>
            Unavailability</h2>
        <table>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lUnavail" runat="server">Unavailable:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblUnavailability" runat="server" />
                </td>
            </tr>
            <tr id="trUnavailabilityComments" runat="server" visible="false">
                <td colspan="2">
                    <asp:Literal ID="lUnavailComments" runat="server"> <b><U>Comments</U></b><br /></asp:Literal>
                    <asp:Label ID="lblAvailabilityComment" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul style="float: right; padding-right: 50px">
            <asp:Repeater ID="rptTraitTypes" runat="server" OnItemDataBound="rptTraitTypes_OnItemDataBound">
                <ItemTemplate>
                    <asp:HyperLink ID="hlViewTraits" runat="server" NavigateUrl="/volunteers/ViewMyTraits.aspx?contextID=" />
                </ItemTemplate>
            </asp:Repeater>
        </ul>
        <ul>
            <asp:HyperLink ID="hlUpdateProfile" NavigateUrl="UpdateMyVolunteerProfile.aspx?contextID="
                runat="server"><li>Update My Profile</li></asp:HyperLink>
            <asp:HyperLink ID="hlViewOpenJobs" NavigateUrl="ViewOpenJobs.aspx?contextID=" runat="server"><li>View Open Jobs</li></asp:HyperLink>
            <asp:HyperLink ID="hlViewMyJobHistory" NavigateUrl="ViewMyJobAssignments.aspx?contextID=" runat="server"><li>View My Past Assignments</li></asp:HyperLink>
            <asp:HyperLink ID="hlSubmitTimesheet" NavigateUrl="SubmitTimesheet.aspx?contextID=" runat="server"><li>Submit a Timesheet</li></asp:HyperLink>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
