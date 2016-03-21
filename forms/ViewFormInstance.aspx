<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ViewFormInstance.aspx.cs" Inherits="forms_ViewFormInstance" %>
<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
<a href="ManageFormInstances.aspx?contextID=<%=targetForm.ID %>"> <%= targetFormManifest.ManageLink  %></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
    View <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<asp:Literal ID="lPageText" runat="server" />
<%=targetForm.ViewInstructions  %>

 <uc1:CustomFieldSet ID="CustomFieldSet1" EditMode="false" runat="server" />

 <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <asp:HyperLink ID="hlEditInstance" runat="server" NavigateUrl="EditFormInstance.aspx?contextID=" />
                <asp:LinkButton ID="lbDelete" runat="server" OnClick="lbDelete_Click"><li>Delete this Record</li></asp:LinkButton>
                <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="ManageFormInstances.aspx?contextID=" />
                <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>


</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

