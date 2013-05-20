<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EditControlProperty.aspx.cs" Inherits="admin_EditControlProperty" %>
    
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Edit Portal Control
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <table>
        <tr>
            <td class="columnHeader">
                Page:
            </td>
            <td>
                <%=MultiStepWizards.CustomizePage.PageName  %>
            </td>
        </tr>
         <tr>
            <td class="columnHeader">
                Control:  <span class="requiredField">*</span>
            </td>
            <td>
                <asp:DropDownList ID="ddlAllControls" runat="server" Visible="false">
                </asp:DropDownList>
                <asp:Literal id="lControlName" runat="server" />
            </td>
        </tr>
          <tr>
            <td class="columnHeader">
                Property:  <span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox id="tbProperty" runat="server" />
                <asp:RequiredFieldValidator runat="server" ErrorMessage="Please enter a property name" Display="None" ControlToValidate="tbProperty" />
            </td>
        </tr>
    </table>
    <h2>Text/Property Value:</h2>
       <telerik:RadEditor NewLineBr="false" runat="server" ID="reValue" ToolsFile="~/controls/telerik/ToolsFileDeluxe.xml" />

       <h2>Description/Notes:</h2>
       <asp:TextBox ID="tbNotes" runat="server" TextMode="MultiLine" Rows=5 Columns=100 />

         <hr />
     
            <div align="center" style="padding-top: 20px">
                <asp:Button ID="btnSave" OnClick="btnSave_Click" Text="Save Changes" runat="server" />
                <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" CausesValidation="false" runat="server" />
                <div class="clearBothNoSPC">
                </div>
            </div>
      
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
