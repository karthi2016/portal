<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateEditDiscussionTopic.aspx.cs"
    Inherits="discussions_CreateEditDiscussionTopic" MasterPageFile="~/App_Master/GeneralPage.master" %>

<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="<%=string.Format(@"\discussions\ViewDiscussionBoard.aspx?contextID={0}", TargetDiscussionBoard.ID) %>">
        <%= TargetDiscussionBoard.Name %>
        ></a> <a href="<%=string.Format(@"\discussions\ViewForum.aspx?contextID={0}", targetForum.ID) %>">
            <%= targetForum.Name %></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <%= targetForum.Name %>:
    <%= editMode ? "Edit" : "New" %>
    Topic
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lPostNew" runat="server">
            Post New Thread</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lTitle" runat="server">Title: <span class="redHighlight">*</span></asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox ID="tbName" runat="server" Width="300" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" ControlToValidate="tbName"
                            Display="None" ErrorMessage="Please specify a title." />
                    </td>
                </tr>
                <tr runat="server" id="trMessage">
                    <td class="columnHeader">
                        <asp:Literal ID="lMessage" runat="server">Message: <span class="redHighlight">*</span></asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbMessage" TextMode="MultiLine" Columns="100" Rows="20"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="rfvMessage" ControlToValidate="tbMessage"
                            Display="None" ErrorMessage="Please specify a message." />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lAdditionalOptions" runat="server">
            Additional Options</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader" style="width: 33%">
                        <asp:Literal ID="lSubscribe" runat="server">Subscribe to this Topic?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkSubscribe" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="sectionContent">
        <div align="center" style="padding-top: 20px">
            <asp:Button ID="btnPost" OnClick="btnPost_Click" Text="Post" runat="server" />
            <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server"
                CausesValidation="false" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lDiscussionBoardTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <li><a href="/">
                    <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
