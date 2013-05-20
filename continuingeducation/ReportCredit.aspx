<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ReportCredit.aspx.cs" Inherits="continuingeducation_ReportCredit" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Self-Report CEU Credits
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <p>
        <asp:Literal ID="PageText" runat="server">
       
        Use this page to report (or edit) CEU credits. After you report these credits, they
        will need to be verified by the association staff. Once verified, CEU credits <i>cannot</i>
        be edited or deleted.
  
    
        </asp:Literal>
        <p />
        <span class="requiredField">*</span> - indicates a required field.
    </p>
    <table style="width: 100%">
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lCreditType" runat="server">
                Credit Type:  </asp:Literal>
                <span class="requiredField">*</span>
            </td>
            <td>
                <asp:DropDownList ID="ddlType" runat="server" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter a type of credit"
                    ControlToValidate="ddlType" Display="None" />
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lDateReceived" runat="server">Date Received: </asp:Literal><span
                    class="requiredField">*</span>
            </td>
            <td>
                <telerik:RadDatePicker ID="dpDate" runat="server">
                </telerik:RadDatePicker>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Please enter a date"
                    ControlToValidate="dpDate" Display="None" />
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lQuantity" runat="server">Quantity: </asp:Literal>
                <span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbQuantity" runat="server" Width="40px" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please enter the number of credits received"
                    ControlToValidate="tbQuantity" Display="None" />
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="tbQuantity"
                    ValueToCompare="0" Display="None" ErrorMessage="Please enter a valid number greater than 0."
                    Operator="GreaterThan" Type="Double" />
            </td>
            <td class="columnHeader">
            </td>
            <td>
            </td>
        </tr>
    </table>
    <br />
    <asp:Literal ID="lNotesDescription" runat="server"> <b>Notes/Description:</b>  </asp:Literal>
    <br />
    <asp:TextBox ID="tbNotes" runat="server" TextMode="MultiLine" Rows="7" Columns="100" />
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Create" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
