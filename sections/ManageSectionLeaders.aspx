<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ManageSectionLeaders.aspx.cs" Inherits="sections_ManageSectionLeaders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Manage Section Leaders
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <%= targetSection.Name %>
                <asp:Literal ID="lTitleLeaders" runat="server">Leaders</asp:Literal></h2>
        </div>
        <asp:Literal ID="PageText" runat="server" />
        <div class="sectionContent">
            <asp:GridView ID="gvLeaders" runat="server" GridLines="None" AutoGenerateColumns="false"
                OnRowCommand="gvLeaders_RowCommand" EmptyDataText="No records found.">
                <Columns>
                    <asp:BoundField DataField="Individual.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# DataBinder.GetPropertyValue( Container.DataItem,"Individual.ID") %>'
                                CommandName="editleader" Text="(edit)" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# DataBinder.GetPropertyValue( Container.DataItem, "Individual.ID") %>'
                                CommandName="deleteleader" Text="(delete)" OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li><a href="CreateSectionLeader.aspx?contextID=<%=ContextID %>">
                    <asp:Literal ID="lAddSectionLeader" runat="Server">Add a Section Leader</asp:Literal></a></li>
                <li><a href="ViewSection.aspx?contextID=<%=ContextID %>">
                    <asp:Literal ID="lBackToSection" runat="Server">Back to Section</asp:Literal></a></li>
               <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
