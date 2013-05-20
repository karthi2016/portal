<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="SearchResume_ViewDetails.aspx.cs" Inherits="careercenter_SearchResume_ViewDetails" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Resume Search Details
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div runat="server" id="divResumeDetailsTitle" class="sectionHeaderTitle">
            <h2>
            <asp:Literal ID="lResumeDetails" runat="server">
                Resume Details</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Repeater runat="server" ID="rptResumeFields">
                <HeaderTemplate>
                    <table>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="columnHeader">
                            <asp:Label runat="server" ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem,"FieldName") %>' />
                        </td>
                        <td>
                            <asp:Label runat="server" ID="Label1" Text='<%# DataBinder.Eval(Container.DataItem,"FieldValue") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table></FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div runat="server" id="divTextualRepresentation" class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lTextRepresentation" runat="server">Textual Representation</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Literal runat="server" ID="litResumeText" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li><asp:HyperLink runat="server" ID="hlDownloadResume">Download Resume</asp:HyperLink></li>
            <li><a href="/careercenter/SearchResume_Results.aspx"> <asp:Literal ID="lBackToResults" runat="server">Back to Search Resume Results</asp:Literal></a></li>
            <li><a href="/"> <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
