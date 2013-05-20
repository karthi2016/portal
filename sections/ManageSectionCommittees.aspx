<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ManageSectionCommittees.aspx.cs" Inherits="sections_ManageSectionCommittees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="/sections/ViewSection.aspx?contextID=<%=targetSection.ID %>">
        <%=targetSection.Name%>
        ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Manage Section Committees
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <%= targetSection.Name %>
                <asp:Literal ID="lTitleCommittees" runat="server">Committees</asp:Literal>
            </h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvCommittees" runat="server" GridLines="None" AutoGenerateColumns="false"
                OnRowCommand="gvCommittees_RowCommand" EmptyDataText="No committees found.">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="CurrentMemberCount" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="# of Members" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\committees\ViewCommittee.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(view)" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\committees\EditCommittee.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(edit)" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deletecommittee" Text="(delete)" OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server"> Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li><a href="ViewSection.aspx?contextID=<%=ContextID %>">
                    <asp:Literal ID="lBackToSection" runat="server">Back to Section</asp:Literal></a></li>
                <li>
                    <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
