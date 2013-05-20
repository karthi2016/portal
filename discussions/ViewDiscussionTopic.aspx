<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="ViewDiscussionTopic.aspx.cs" Inherits="discussions_ViewDiscussionTopic" %>

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
    Posts in Topic:
    <%= targetDiscussionTopic.Name %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent" style="margin-top: 10px">
            <div style="text-align: right">
                <asp:Label runat="server" ID="lblSearch" Text="Search this Topic:" CssClass="columnHeader" />
                <asp:TextBox runat="server" ID="tbSearchKeywords"></asp:TextBox><asp:Button runat="server"
                    ID="btnSearchGo" Text="Go" OnClick="btnSearchGo_Click" />
            </div>
            <asp:Repeater runat="server" ID="rptTopics" OnItemDataBound="rptTopics_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table>
                        <tr>
                            <th style="text-align: left" colspan="2">
                                <asp:Label runat="server" ID="lblCreatedDate" Text='<%# FormatDate(DataBinder.Eval(Container.DataItem,"CreatedDate")) %>'></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="border-left-color: whitesmoke; border-right-color: whitesmoke; border-left-style: solid;
                                border-right-style: solid; border-width: thin; vertical-align: top; padding: 5px;
                                width: 20%">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label runat="server" ID="lblPostedBy" Text='<%# DataBinder.Eval(Container.DataItem,"PostedBy_Name") %>' />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trProfileImage">
                                        <td>
                                            <div id="accountProfileImg">
                                                <asp:Image ID="profileImg" Width="75" runat="server" BorderWidth="2px" BorderColor="Whitesmoke"
                                                    ImageUrl="~/Images/noimage.gif" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label runat="server" ID="lblPosts" Text='<%# string.Format("Posts: {0}", DataBinder.Eval(Container.DataItem,"PostedBy_Discussions_DiscussionPosts")) %>' />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="border-left-color: whitesmoke; border-right-color: whitesmoke; border-left-style: solid;
                                border-right-style: solid; border-width: thin; vertical-align: top; padding: 5px;">
                                <table>
                                    <tr runat="server" id="trMessageTitle" style="border-bottom: solid; border-bottom-width: 1px;">
                                        <td class="columnHeader">
                                            <asp:Label runat="server" ID="lblTitle" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-top: 5px">
                                            <asp:Label runat="server" ID="lblPost" Text='<%# DataBinder.Eval(Container.DataItem,"Post") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="text-align: right;">
                            <td style="border-left-color: whitesmoke; border-right-color: whitesmoke; border-bottom-color: whitesmoke;
                                border-left-style: solid; border-right-style: solid; border-bottom-style: solid;
                                border-width: thin; vertical-align: top;">
                                &nbsp;
                            </td>
                            <td style="border-left-color: whitesmoke; border-right-color: whitesmoke; border-bottom-color: whitesmoke;
                                border-left-style: solid; border-right-style: solid; border-bottom-style: solid;
                                border-width: thin; vertical-align: top;">
                                <asp:HyperLink runat="server" ID="hlRemovePost" Text="(remove post)" NavigateUrl='<%# string.Format(@"~\discussions\CreateEditDiscussionPost.aspx?contextID={0}&action=remove", DataBinder.Eval(Container.DataItem,"ID") ) %>' />
                                <asp:HyperLink runat="server" ID="hlEditPost" Text="(edit post)" NavigateUrl='<%# string.Format(@"~\discussions\CreateEditDiscussionPost.aspx?contextID={0}", DataBinder.Eval(Container.DataItem,"ID") ) %>' />
                                <asp:HyperLink runat="server" ID="hlReplyToPost" Text="(reply to this post)" NavigateUrl='<%# string.Format(@"~\discussions\CreateEditDiscussionPost.aspx?contextID={0}&replyToID={1}", targetDiscussionTopic.ID, DataBinder.Eval(Container.DataItem,"ID") ) %>' />
                            </td>
                        </tr>
                    </table>
                    <br />
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Label runat="server" ID="lblNoPostsFound" CssClass="columnHeader">No Posts Found</asp:Label>
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
                <asp:Literal ID="lDiscussionTopicTasks" runat="server">Discussion Topic Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <li><a href="/discussions/CreateEditDiscussionPost.aspx?contextID=<%=targetDiscussionTopic.ID %>">
                    <asp:Literal ID="Literal1" runat="server">Post Reply</asp:Literal></a></li>
                <asp:HyperLink runat="server" ID="hlPostsPendingApproval"
                            NavigateUrl="~\discussions\ViewPostsPendingApproval.aspx?contextID="><li>View Posts Pending Approval</li></asp:HyperLink>
                <li><asp:LinkButton runat="server" ID="lbSubscribeUnsubscribe" OnClick="lbSubscribeUnsubscribe_Click" Text="Suscribe to this Topic"></asp:LinkButton></li>
                <li><a href="/">
                    <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
