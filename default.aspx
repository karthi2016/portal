<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="Home" %>

<%@ Import Namespace="MemberSuite.SDK.Types" %>
<%@ Register Src="homepagecontrols/Membership.ascx" TagName="Membership" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/MyProfile.ascx" TagName="MyProfile" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/Committees.ascx" TagName="Committees" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/MyAccount.ascx" TagName="MyAccount" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/Events.ascx" TagName="Events" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/ContinuingEducation.ascx" TagName="ContinuingEducation"
    TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/Fundraising.ascx" TagName="Fundraising" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/Competitions.ascx" TagName="Competitions" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/CareerCenter.ascx" TagName="CareerCenter" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/Subscriptions.ascx" TagName="Subscriptions" TagPrefix="uc" %>
<%@ Register Src="homepagecontrols/Discussions.ascx" TagName="Discussions" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal ID="lHomePageTitle" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <div id="divDeclinedPayments" visible="false" runat="server" style="color: red; padding-bottom: 10px">
        <b>ALERT: One or more of your deferred billing payments have been declined! Please click
            <a href="/financial/RectifySuspendedBillingSchedules.aspx">here</a> to update payment
            information so we can bring your account current. </b>
    </div>
    <!-- / site content header -->
    <!-- page content -->
    <div id="colLeft">
        <asp:PlaceHolder ID="phLeftControls" runat="server" />
        <uc:MyProfile ID="ucMyProfile1" runat="server" />
        <uc:Membership ID="ucMembership" runat="server" />
        <uc:Committees ID="ucCommittees" runat="server" />
        <uc:Subscriptions ID="ucSubscriptions" runat="server" />
    </div>
    <div id="colRight">
        <div id="colRightContent">
            <asp:PlaceHolder ID="phRightControls" runat="server" />
            <uc:MyAccount ID="ucMyAccount" runat="server" />          
             <uc:Events ID="ucEvents" runat="server" />          
            <uc:Fundraising ID="ucFundraising" runat="server" />
            <uc:ContinuingEducation ID="ucCEU" runat="server" />
            <uc:Competitions ID="ucCompetitions" runat="server" />
            <uc:CareerCenter ID="ucCareerCenter" runat="server" />
            <uc:Discussions ID="ucDiscussions" runat="server" />
        </div>
    </div>
    <!-- / page content -->
    <div class="clearBothNoSPC">
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
