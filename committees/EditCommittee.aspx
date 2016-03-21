<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="EditCommittee.aspx.cs" Inherits="committees_EditCommittee" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlEventOwner" Visible="false" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Edit <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
       <span class="requiredField">*</span> - indicates a required field.
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
        <h2>
        <asp:Literal ID="lBasicInfo" runat="server">
        Basic Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
           <table>
                <tr>
                    <td class="columnHeader"><asp:Literal ID="Literal1" runat="server">Name: <span class="requiredField">*</span>
                    </asp:Literal>
                    </td> 
                    <td><asp:TextBox ID="tbName" runat="server" />
                    <asp:RequiredFieldValidator runat="server" ID="rfvName" Display="None" ErrorMessage="Please specify a value for Name." ControlToValidate="tbName" />
                    </td>
                </tr>
           </table>
        </div>
    </div>
      <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lDescription" runat="server">Description</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <telerik:RadEditor runat="server" ID="reDescription" ToolsFile="~/controls/telerik/ToolsFileDeluxe.xml" />
        </div>
    </div>
        <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
