<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Discussions.ascx.cs"
    Inherits="homepagecontrols_Discussions" %>
<%@ Import Namespace="MemberSuite.SDK.Types" %>
<div class="sectCont" runat="server" id="divDiscussions">
    <div class="sectHeaderTitle hIconDiscussions">
        <h2>
            <asp:Literal ID="Widget_Discussions_Title" runat="server">Discussions</asp:Literal>
        </h2>
    </div>
    <table>
        <tr ID="Widget_Discussions_LastPost_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_Discussions_LastPost" runat="server">Last Post:</asp:Literal>
            </td>
            <td>
                <asp:HyperLink ID="hlLastPost" runat="server" NavigateUrl="~\discussions\ViewDiscussionTopic.aspx?contextID=">No Records Found.</asp:HyperLink>
            </td>
        </tr>
        <tr ID="Widget_Discussions_LastPostDate_Row" runat="server">
            <td class="columnHeader">
                <asp:Literal ID="Widget_Discussions_LastPostDate" runat="server">Last Post Date:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblLastPostDate" runat="server" />
            </td>
        </tr>
    </table>
    <ul style="margin-left: -20px">
        <asp:HyperLink ID="Widget_Discussions_hlViewDiscussionBoard" runat="server" NavigateUrl="/discussions/ViewDiscussionBoard.aspx"><li>View Discussion Board</li></asp:HyperLink>
        <asp:HyperLink ID="Widget_Discussions_hlViewMySubscriptions" runat="server" NavigateUrl="/discussions/ViewMySubscriptions.aspx"><li>View My Subscriptions</li></asp:HyperLink>
    </ul>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
</div>
