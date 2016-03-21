<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="ViewForum.aspx.cs" Inherits="discussions_ViewForum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="<%=string.Format(@"\discussions\ViewDiscussionBoard.aspx?contextID={0}", TargetDiscussionBoard.ID) %>">
        <%= TargetDiscussionBoard.Name %></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Topics in Forum: <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent" style="margin-top: 10px">
            <div style="text-align: right">
                <asp:Label runat="server" ID="lblSearch" Text="Search this Forum:" CssClass="columnHeader" />
                <asp:TextBox runat="server" ID="tbSearchKeywords"></asp:TextBox><asp:Button runat="server"
                    ID="btnSearchGo" Text="Go" OnClick="btnSearchGo_Click" />
            </div>
            <table>
                <tr>
                    <th style="width: 70%">
                        Topic
                    </th>
                    <th style="width: 20%;">
                        Last Post
                    </th>
                    <th style="width: 10%;">
                        Posts
                    </th>
                </tr>
                <tr runat="server" id="trPostsPendingApproval">
                    <td style="border: whitesmoke; border-style: solid; border-width: thin; vertical-align: top;
                        padding: 5px;">
                        <asp:HyperLink runat="server" ID="hlPostsPendingApproval" Text="Posts Pending Approval"
                            NavigateUrl="~\discussions\ViewPostsPendingApproval.aspx?contextID=" /><br />
                    </td>
                    <td style="border: whitesmoke; border-style: solid; border-width: thin; vertical-align: top;
                        padding: 5px; text-align: right">
                        <asp:Label runat="server" ID="lblLastPostDate"></asp:Label>
                        <asp:Label runat="server" ID="lblBy">by </asp:Label><asp:Label runat="server" ID="lblLastPostPostedBy" /><br />
                    </td>
                    <td style="border: whitesmoke; border-style: solid; border-width: thin; text-align: center;
                        padding: 5px;">
                        <asp:Label runat="server" ID="lblPosts"><%= numberOfPostsPendingApproval %></asp:Label>
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="rptTopics" OnItemDataBound="rptTopics_ItemDataBound">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="border: whitesmoke; border-style: solid; border-width: thin; vertical-align: top;
                                padding: 5px;">
                                <asp:HyperLink runat="server" ID="hlTopicName" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>'
                                    NavigateUrl='<%# string.Format(@"~\discussions\ViewDiscussionTopic.aspx?contextID={0}", DataBinder.Eval(Container.DataItem, "ID"))%>' />
                                <asp:HyperLink runat="server" ID="hlEditDiscussionTopic" NavigateUrl='<%# string.Format(@"~\discussions\CreateEditDiscussionTopic.aspx?contextID={0}", DataBinder.Eval(Container.DataItem, "ID"))%>'
                                    Text="(edit)"></asp:HyperLink><br />
                                <asp:Label runat="server" ID="lblTopicPostedBy" Text='<%# DataBinder.Eval(Container.DataItem,"PostedBy_Name") %>' />
                            </td>
                            <td style="border: whitesmoke; border-style: solid; border-width: thin; vertical-align: top;
                                padding: 5px; text-align: right">
                                <asp:Label runat="server" ID="lblLastPostDate" Text='<%# FormatDate(DataBinder.Eval(Container.DataItem,"LastDiscussionPost_CreatedDate")) %>'></asp:Label><br />
                                <asp:Label runat="server" ID="lblBy">by </asp:Label><asp:Label runat="server" ID="lblLastPostPostedBy"
                                    Text='<%# DataBinder.Eval(Container.DataItem,"LastDiscussionPost_PostedBy_Name") %>' />
                            </td>
                            <td style="border: whitesmoke; border-style: solid; border-width: thin; text-align: center;
                                padding: 5px;">
                                <asp:Label runat="server" ID="lblPosts" Text='<%# DataBinder.Eval(Container.DataItem,"DiscussionPost_Count") %>'></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:Repeater>
            </table>
            <asp:Label runat="server" ID="lblNoTopicsFound" CssClass="columnHeader">No Topics Found</asp:Label>
            <br />
            <table width="90%">
                <tr>
                    <td style="text-align: left; width: 33%">
                        <asp:HyperLink ID="hlFirstPage" runat="server">&lt;&lt;&lt;&lt; First Page</asp:HyperLink>
                        <asp:HyperLink ID="hlPrevPage" runat="server">&lt;&lt;Previous Page</asp:HyperLink>
                    </td>
                    <td style="text-align: center; width: 33%">
                        Page
                        <asp:Literal ID="lCurrentPage" runat="server" />
                        of
                        <asp:Literal ID="lNumPages" runat="server" />
                    </td>
                    <td style="text-align: right; width: 33%">
                        <asp:HyperLink ID="hlNextPage" runat="server">Next Page &gt;&gt;</asp:HyperLink>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lDiscussionForumTasks" runat="server">Discussion Forum Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <li><a href="/discussions/CreateEditDiscussionTopic.aspx?contextID=<%=targetForum.ID %>">
                    <asp:Literal ID="Literal1" runat="server">New Topic</asp:Literal></a></li>
                <li><a href="/">
                    <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
