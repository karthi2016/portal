<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewAbstract.aspx.cs" Inherits="events_ViewAbstract" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewEvent.aspx?contextID=<%= targetAbstract.Event%>">
        <%=GetSearchResult( targetAbstractRow , "Event.Name")%></a> &gt; <a href="ViewAbstracts.aspx?contextID=<%= targetAbstract.Event%>">
            View Abstracts</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <table style="width: 500px">
        <tr>
            <td class="columnHeader">
               <asp:Literal ID="lName" runat="server">Name:</asp:Literal>
            </td>
            <td>
                <%=targetAbstract.Name%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lStatus" runat="server">Status:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult( targetAbstractRow , "Status.Name")%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader" colspan="2">
                <br />
                <asp:Literal ID="lAbstractSummary" runat="server">Abstract Summary/Description:</asp:Literal>
                <br />
                <%=targetAbstract.Description == null ? "" : targetAbstract.Description.Replace("\r", "<BR/>")%>
            </td>
        </tr>
        <tr id="trSessionTracksConfirm" runat="server">
            <td colspan="2" class="columnHeader">
                <br />
                <asp:Literal ID="lTracks" runat="server">Tracks</asp:Literal>
                <br />
                <asp:Label ID="lblTracks" runat="server" />
            </td>
        </tr>
    </table>
    <h2>
        <asp:Literal ID="lPresenterInfo" runat="server">Presenter Information</asp:Literal></h2>
    <table style="width: 500px">
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lPresenterName" runat="server">Presenter Name:</asp:Literal>
            </td>
            <td>
                <%=targetAbstract.PresenterName%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lPresenterEmail" runat="server">Presenter E-Mail:</asp:Literal>
            </td>
            <td>
                <%=targetAbstract.PresenterEmailAddress%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lPresenterContactNum" runat="server">Presenter Contact #:</asp:Literal>
            </td>
            <td>
                <%=targetAbstract.PresenterPhoneNumber%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader" colspan="2">
                <br />
                <asp:Literal ID="lPresenterBio" runat="server">Presenter Bio:</asp:Literal>
                <br />
                <%=targetAbstract.PresenterBiography.Replace( "\r","<BR/>")%>
            </td>
        </tr>
    </table>
    <hr width="100%" />
    <div runat="server" id="divAdditionalInfo">
        <table width="100%">
            <tr>
                <td align=left>
                    <uc1:CustomFieldSet ID="cfsAbstractCustomFields" EditMode="false" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <hr />
    <div style="text-align: center">
        <asp:Button ID="Button1" runat="server" CausesValidation="false" Text="Back to My Abstracts"
            OnClick="btnMyAbstracts_Click" />
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Back to Event"
            OnClick="btnCancel_Click" />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
