<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ViewMyJobAssignments.aspx.cs" Inherits="volunteers_ViewMyJobAssignments" %>
<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
<a href="ViewMyVolunteerProfile.aspx?contextID=<%=targetVolunteer.ID %>">
       My Volunteer Profile</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
View My Volunteer History
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<asp:Literal ID="lPageText" runat='server'> </asp:Literal>
  <div  style="margin-top: 10px">
       
       <asp:GridView ID="gvHistory" runat="server" EmptyDataText="You do not have any assignments."
        GridLines="None" AutoGenerateColumns="false" HeaderStyle-HorizontalAlign="Left">
        <Columns>
            <asp:BoundField DataField="JobOccurrence.Job.Name" HeaderText="Job" />
            <asp:BoundField DataField="StartDateTime" HeaderText="Start" />
            <asp:BoundField DataField="EndDateTime" HeaderText="End" />
            <asp:BoundField DataField="HoursWorked" HeaderText="Hours Reported" />
         <%--  <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="ViewJobAssignment.aspx?contextID={0}" Text="(view)" />--%>
        </Columns>
    </asp:GridView>
       
    </div>
     <hr />
    <div align=center>
    <asp:Button ID="btnGoHome" Text="Back to My Volunteer Profile" runat="server" OnClick="btnGoHome_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

