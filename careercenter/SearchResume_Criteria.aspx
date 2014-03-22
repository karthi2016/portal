<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="SearchResume_Criteria.aspx.cs" Inherits="careercenter_SearchResume_Criteria" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Search Resume Bank
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lSearchFields" runat="server">Search Fields</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Literal ID="PageText" runat="server">
            Use the fields below to find the record you are looking for.
            </asp:Literal>
            <p>
            </p>
            <table>
                <tr>
                    <td class="columnHeader">
                        Keywords:
                    </td>
                    <td>
                        <asp:TextBox ID="tbKeywords" runat="server"/>
                    </td>
                </tr>
            </table>
            <uc1:CustomFieldSet ID="cfsSearchCriteria" runat="server" SuppressValidation="true" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lOutputFormat" runat="server"> Output Format </asp:Literal>
            </h2>
        </div>
        <div class="sectionContent">
            <asp:RadioButtonList runat="server" ID="rblOutputFormat">
                <asp:ListItem Text="To My Screen" Value="screen" Selected="True" />
                <asp:ListItem Text="To a compressed (zip) file" Value="zip" />
            </asp:RadioButtonList>
            <div style="text-align: center; padding-top: 20px">
                <asp:Button runat="server" ID="btnSearch" Text="Execute Search" OnClick="btnSearch_Click" /></div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
