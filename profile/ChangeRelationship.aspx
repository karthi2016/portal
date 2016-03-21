<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChangeRelationship.aspx.cs"
    Inherits="profile_ChangeRelationship" MasterPageFile="~/App_Master/GeneralPage.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Change Relationship between <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lRelationshipInfo" runat="server">Relationship Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lRelationshipType" runat="server">Relationship Type:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlRelationshipType" runat="server" DataTextField="Name" DataValueField="ID">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lStartDate" runat="server">Start Date:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpStartDate" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lEndDate" runat="server">End Date:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpEndDate" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lNotes" runat="server">Notes/Description:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbDescription" Columns="80" Rows="5" TextMode="MultiLine" />
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <div style="text-align: center">
            <asp:Button ID="btnSave" Text="Save" runat="server" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" Text="Cancel" runat="server" OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
