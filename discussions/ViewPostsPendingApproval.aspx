<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="ViewPostsPendingApproval.aspx.cs" Inherits="discussions_ViewPostsPendingApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="<%=string.Format(@"\discussions\ViewDiscussionBoard.aspx?contextID={0}", TargetDiscussionBoard.ID) %>">
        <%= TargetDiscussionBoard.Name %></a> <a href="<%= targetForum != null ? string.Format(@"\discussions\ViewForum.aspx?contextID={0}", targetForum.ID) : "" %>">
            <%= targetForum != null ? "> " + targetForum.Name : "" %></a> <a href="<%= targetDiscussionTopic != null ? string.Format(@"\discussions\ViewDiscussionTopic.aspx?contextID={0}", targetDiscussionTopic.ID) : "" %>">
                <%= targetDiscussionTopic != null ? "> " + targetDiscussionTopic.Name : ""%></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Posts Pending Approval
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent" style="margin-top: 10px">
            <asp:Repeater runat="server" ID="rptPosts" OnItemDataBound="rptPosts_ItemDataBound" OnItemCommand="rptPosts_OnItemCommand">
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
                                <asp:LinkButton runat="server" ID="lbApprove" Text="(approve post)" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID") %>' CommandName="approve" />
                                <asp:LinkButton runat="server" ID="lbReject" Text="(reject post)" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID") %>' CommandName="reject" />
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
                <asp:Literal ID="lDiscussionTopicTasks" runat="server">Tasks</asp:Literal></h2>
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
