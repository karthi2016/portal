<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ManageChapterLeaders.aspx.cs" Inherits="chapters_ManageChapterLeaders" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="/chapters/ViewChapter.aspx?contextID=<%=targetChapter.ID %>"><%=targetChapter.Name %> ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Manage Chapter Leaders
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server"></asp:Literal>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <%= targetChapter.Name %>
                Leaders</h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvLeaders" runat="server" GridLines="None" AutoGenerateColumns="false"  OnRowCommand="gvLeaders_RowCommand" EmptyDataText="No records found." OnRowDataBound="gvLeaders_RowDataBound" >
                <Columns>
                    <asp:BoundField DataField="Individual.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# DataBinder.GetPropertyValue( Container.DataItem,"Individual.ID") %>' CommandName="editleader" Text="(edit)" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# DataBinder.GetPropertyValue( Container.DataItem, "Individual.ID") %>' CommandName="deleteleader" Text="(delete)" OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
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
               <li><a href="CreateEditChapterLeader.aspx?contextID=<%=ContextID %>"><ASP:Literal ID="lAddChapterLeader" runat="server">Add a Chapter Leader</ASP:Literal></a></li>
                <li><a href="ViewChapter.aspx?contextID=<%=ContextID %>"><asp:Literal ID="lBackToChapter" runat="server" >Back to Chapter</asp:Literal> </a></li>
                <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>

     
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>

