<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchDiscussionContents_Results.aspx.cs"
    Inherits="discussions_SearchDiscussionContents_Results" MasterPageFile="~/App_Master/GeneralPage.master" %>

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
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent" style="margin-top: 10px">
            <table>
                <tr>
                    <th style="width: 50%">
                        Name
                    </th>
                    <th style="width: 10%;">
                        Content Type
                    </th>
                    <th style="width: 30%;">
                        Last Post
                    </th>
                    <th style="width: 10%;">
                        Posts
                    </th>
                </tr>
                <asp:Repeater runat="server" ID="rptSearchResults" OnItemDataBound="rptSearchResults_ItemDataBound">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="border: whitesmoke; border-style: solid; border-width: thin; vertical-align: top;
                                padding: 5px;">
                                <asp:HyperLink runat="server" ID="hlTitle" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>' />
                            </td>
                            <td style="border: whitesmoke; border-style: solid; border-width: thin; vertical-align: top;
                                padding: 5px;">
                                <asp:Label runat="server" ID="hlContentType" Text='<%# DataBinder.Eval(Container.DataItem,"ContentType") %>' />
                            </td>
                            <td style="border: whitesmoke; border-style: solid; border-width: thin; vertical-align: top;
                                padding: 5px;">
                                <asp:HyperLink runat="server" ID="hlLastPost" Text='<%# DataBinder.Eval(Container.DataItem,"LastDiscussionPost_Name") %>'
                                    NavigateUrl='<%# string.Format(@"~\discussions\ViewDiscussionTopic.aspx?contextID={0}", DataBinder.Eval(Container.DataItem, "LastDiscussionPost_Topic"))%>' /><br />
                                <asp:Label runat="server" ID="lblBy">by </asp:Label><asp:Label runat="server" ID="lblLastPostPostedBy"
                                    Text='<%# DataBinder.Eval(Container.DataItem,"LastDiscussionPost_PostedBy_Name") %>' /><br />
                                <asp:Label runat="server" ID="lblLastPostDate" Text='<%# FormatDate(DataBinder.Eval(Container.DataItem,"LastDiscussionPost_CreatedDate")) %>'></asp:Label>
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
            <asp:Label runat="server" ID="lblNoRecordsFound" CssClass="columnHeader">No Records Found</asp:Label>
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
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
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
