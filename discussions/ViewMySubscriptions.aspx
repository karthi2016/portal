<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewMySubscriptions.aspx.cs" Inherits="discussions_ViewMySubscriptions" MasterPageFile="~/App_Master/GeneralPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View My Topic Subscriptions
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
                <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lMySubscriptions" runat="server">
            My Subscriptions</asp:Literal></h2>
        </div>
        <div class="sectionContent" style="margin-top: 10px">
            <asp:GridView ID="gvSubscriptions" runat="server" GridLines="None" AutoGenerateColumns="false"  OnRowCommand="gvSubscriptions_RowCommand" EmptyDataText="No Subscriptions Found.">
                <Columns>
                    <asp:BoundField DataField="Topic.Forum.DiscussionBoard.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Discussion Board" />
                    <asp:BoundField DataField="Topic.Forum.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Forum" />
                    <asp:BoundField DataField="Topic.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Topic" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnUnsubscribe" CommandArgument='<%# DataBinder.GetPropertyValue( Container.DataItem, "ID") %>' CommandName="unsubscribe" Text="(unsubscribe)" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
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
