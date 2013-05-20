<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ConfirmJobPosting.aspx.cs" Inherits="careercenter_ConfirmJobPosting" %>

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
    View Job Posting
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<br />
    <asp:Label runat="server" ID="lblJobPostingsAvailable" CssClass="redHighlight" Font-Bold="true" Text="YOUR JOB POSTING IS NOT YET COMPLETE!  Please confirm the information below then click the Post Job button." />
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <%= targetJobPosting.Name %></h2>
        </div>
        <div class="sectionContent">
            <div align="center">
                <b>
                    <%= targetJobPosting.CompanyName %></b></div>
            <%= targetJobPosting.Body %>
            <p>
            </p>
            <table>
                <tr id="trDivision" runat="server">
                    <td class="columnHeader" style="width: 150px">
                        <ASP:Literal ID="lDivision" runat="server">Division:</ASP:Literal> 
                    </td>
                    <td>
                        <%= targetJobPosting.Division %>
                    </td>
                </tr>
                <tr id="trCode" runat="server">
                    <td class="columnHeader">
                        <ASP:Literal ID="lInternalReferenceCode" runat="server">Internal Reference Code:</ASP:Literal>
                    </td>
                    <td>
                        <%= targetJobPosting.InternalReferenceCode %>
                    </td>
                </tr>
                <tr id="trLocation" runat="server">
                    <td class="columnHeader">
                       <ASP:Literal ID="lLocation" runat="server">Location:</ASP:Literal>
                    </td>
                    <td>
                        <%= targetJobPostingLocation.Name %>
                    </td>
                </tr>
                <tr id="trCategories" runat="server">
                    <td class="columnHeader">
                        <ASP:Literal ID="lCategories" runat="server">Categories:</ASP:Literal>
                    </td>
                    <td>
                        <asp:Label ID="lblCategories" runat="server" />
                    </td>
                </tr>
            </table>
            <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" EditMode="false" />
        </div>
    </div>
    <div class="sectionContent" align="center">
        <asp:Button ID="btnEdit" runat="server" Text="Edit Posting" OnClick="btnEdit_Click" />
        <asp:Button ID="btnPost" runat="server" Text="Post Job" OnClick="btnPost_Click" />
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
