<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ManageResumes.aspx.cs" Inherits="careercenter_ManageResumes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Manage My Resumes
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
              <asp:Literal ID="lMyResumesHeader" runat="server" >My Resumes</asp:Literal>  </h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvResumes" runat="server" GridLines="None" AutoGenerateColumns="false" OnRowDataBound="gvResumes_RowDataBound"
                OnRowCommand="gvResumes_RowCommand" EmptyDataText="No resumes on file.">
                <Columns>
                    <asp:BoundField DataField="LocalID" HeaderStyle-HorizontalAlign="Left" HeaderText="ID" />
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                     <asp:BoundField DataField="IsActive" HeaderStyle-HorizontalAlign="Left" HeaderText="Active?" />
                    <asp:BoundField DataField="LastModifiedDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Last Modified" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink runat="server" ID="hlViewResume" Text="(view)"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/careercenter/UploadResume.aspx?contextID={0}"
                        Text="(edit)" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deleteresume" Text="(delete)" OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
     <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
        <li id="liCreateResume" runat="server" />
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
