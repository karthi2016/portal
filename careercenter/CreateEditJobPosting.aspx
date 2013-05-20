<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditJobPosting.aspx.cs" Inherits="careercenter_CreateEditJobPosting" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Create Job Posting
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Label runat="server" ID="lblJobPostingsAvailable" CssClass="redHighlight" Visible="false" />
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                       <ASP:Literal ID="lTitle" runat="server"> Title: <span class="redHighlight">*</span></ASP:Literal>
                    </td>
                    <td>
                        <asp:TextBox ID="tbName" runat="server" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" ControlToValidate="tbName"
                            Display="None" ErrorMessage="Please specify a title." />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <ASP:Literal ID="lEmailToSend" runat="server">Email to Send Resumes: <span class="redHighlight">*</span></ASP:Literal>
                    </td>
                    <td>
                        <asp:TextBox ID="tbResumeEmail" runat="server" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvResumeEmail" ControlToValidate="tbResumeEmail"
                            Display="None" ErrorMessage="Please specify an email to use when sending resumes." />
                        <asp:RegularExpressionValidator runat="server" ID="revResumeEmail" Display="None"
                            ErrorMessage="Please specify a valid email to use when sending resumes." ControlToValidate="tbResumeEmail"
                            ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <ASP:Literal ID="lCompanyName" runat="server">Company Name:</ASP:Literal>
                    </td>
                    <td>
                        <asp:TextBox ID="tbCompanyName" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <ASP:Literal ID="lDivision" runat="server">Division:</ASP:Literal>
                    </td>
                    <td>
                        <asp:TextBox ID="tbDivision" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <ASP:Literal ID="lInternalReferenceCode" runat="server">Internal Reference Code:</ASP:Literal>
                    </td>
                    <td>
                        <asp:TextBox ID="tbInternalReferenceCode" runat="server" />
                    </td>
                </tr>
                <tr runat="server" id="trLocation">
                    <td class="columnHeader">
                        <ASP:Literal ID="lLocation" runat="server">Location:</ASP:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlLocation" DataTextField="Name" DataValueField="ID"
                            AppendDataBoundItems="true">
                            <asp:ListItem Text="--Unspecified--" Value="" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="trCategories">
                    <td class="columnHeader">
                        <ASP:Literal ID="lCategories" runat="server">Categories:</ASP:Literal>
                    </td>
                    <td>
                        <cc1:DualListBox runat="server" ID="dlbCategories" Width="300" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                       <ASP:Literal ID="lPostOn" runat="server">Post On:</ASP:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpPostOn" />
                    </td>
                </tr>
            </table>
            <p>
            </p>
            <h2><ASP:Literal ID="lBody" runat="server">Job Posting Body</ASP:Literal></h2>
            <telerik:RadEditor NewLineBr="false" runat="server" ID="reBody" ToolsFile="~/controls/telerik/ToolsFileDeluxe.xml" />
        </div>
        <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" />
        <div class="sectionContent">
            <div align="center" style="padding-top: 20px">
                <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Continue" runat="server" />
                <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server"
                    CausesValidation="false" />
            </div>
        </div>
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
