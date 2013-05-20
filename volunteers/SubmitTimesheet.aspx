<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="SubmitTimesheet.aspx.cs" Inherits="volunteers_SubmitTimesheet" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
<a href="ViewMyVolunteerProfile.aspx?contextID=<%=targetVolunteer.ID %>">
       My Volunteer Profile</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
Submit Timesheet
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<asp:Literal id="lPageText" runat="server">This function has been disabled by your administrator.</asp:Literal>
<p>
</p>
  <hr />
    <div align=center>
    <asp:Button ID="btnGoHome" Text="Back to My Volunteer Profile" runat="server" OnClick="btnGoHome_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

