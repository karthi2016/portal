<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ManageOrganizationalLayerCommittees.aspx.cs" Inherits="organizationalLayers_ManageOrganizationalLayerCommittees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="/organizationalLayers/ViewOrganizationalLayer.aspx?contextID=<%=targetOrganizationalLayer.ID %>"><%=targetOrganizationalLayer.Name%> ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
       Manage <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="organizationalLayer" style="margin-top: 10px">
        <div class="organizationalLayerHeaderTitle">
            <h2>
                <%= targetOrganizationalLayer.Name %>
                 <asp:Literal ID="lTitleCommittees" runat="server">Committees</asp:Literal></h2>
        </div>
        <div class="organizationalLayerContent">
            <asp:GridView ID="gvCommittees" runat="server" GridLines="None" AutoGenerateColumns="false"  OnRowCommand="gvCommittees_RowCommand" EmptyDataText="No committees found." >
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
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>' CommandName="deletecommittee" Text="(delete)" OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="organizationalLayer" style="margin-top: 10px">
        <div class="organizationalLayerHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="organizationalLayerContent">
            <ul>
                <li><a href="ViewOrganizationalLayer.aspx?contextID=<%=ContextID %>">Back to <%=GetSearchResult( drTargetOrganizationalLayerType, "Name", null ) %></a></li>
                <li>
                    <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
