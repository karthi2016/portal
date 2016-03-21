<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="BrowseChapters.aspx.cs" Inherits="chapters_BrowseChapters" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlOrganizationalLayer"  />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Browse <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal> Chapters
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">

<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><%= targetOrganizationalLayer != null ? string.Format("{0} ",targetOrganizationalLayer.Name) : ""  %>Chapters</h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvChapters" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="There are no chapters to view.">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" ItemStyle-VerticalAlign="Top" />
                    <asp:TemplateField ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Literal runat="server" ID="litChapters" Text='<%# string.Format("<p>{0}</p>", DataBinder.GetPropertyValue(Container.DataItem, "LinkedOrganization._Preferred_Address")) %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField ItemStyle-VerticalAlign="Top" DataNavigateUrlFormatString="~\chapters\ViewChapter.aspx?contextID={0}" DataNavigateUrlFields="ID" Text="(view)" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
      <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li runat="server" ID="liOrganizationalLayer">
                    <asp:HyperLink runat="server" ID="hlOrganizationalLayerTask" />
                </li>
                 <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
