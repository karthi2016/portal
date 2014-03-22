<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="AddEditExhibitorContact.aspx.cs" Inherits="exhibits_AddEditExhibitorContact" %>

<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewShow.aspx?contextID=<%=targetShow.ID %>">
        <%=targetShow.Name  %></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <%=targetShow.Name%>
    Exhibitor -
    <%=targetExhibitor.Name %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lAddContact" runat="server">Add/Edit an Exhibitor Contact</asp:Literal></h2>
        </div><br />
        <asp:CustomValidator ID="cvContactRestriction" runat="server" Display="Dynamic" ForeColor="Red"
            Font-Bold="true" />
        <table style="width: 600px; margin-top: 20px">
            <tr>
                <td class="columnHeader" style="width: 120px">
                    <asp:Literal ID="lContactType" runat="Server">Contact Type:</asp:Literal>
                    <span class="requiredField">*</span>
                </td>
                <td>
                    <asp:DropDownList ID="ddlType" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlType"
                        Display="None" ErrorMessage="Please select a type." />
                </td>
            </tr>
               <tr>
                <td class="columnHeader" style="width: 120px">
                    <asp:Literal ID="Literal1" runat="Server">Title:</asp:Literal>
                    
                </td>
                <td>
                    <asp:TextBox ID="tbTitle" runat="server" />
                    
                </td>
            </tr>
            <tr>
                <td class="columnHeader" style="width: 120px">
                    <asp:Literal ID="lFirstName" runat="Server">First Name:</asp:Literal>
                    <span class="requiredField">*</span>
                </td>
                <td>
                    <asp:TextBox ID="tbFirstName" runat="server" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbFirstName" Display="None"
                        ErrorMessage="Please enter a first name." />
                </td>
            </tr>
            <tr>
                <td class="columnHeader" style="width: 120px">
                    <asp:Literal ID="lLastName" runat="Server">Last Name:</asp:Literal>
                    <span class="requiredField">*</span>
                </td>
                <td>
                    <asp:TextBox ID="tbLastName" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbLastName"
                        Display="None" ErrorMessage="Please enter a last name." />
                </td>
            </tr>
            <tr>
                <td class="columnHeader" style="width: 120px">
                    <asp:Literal ID="lEmailAddress" runat="Server">Email Address:</asp:Literal>
                    <span class="requiredField">*</span>
                </td>
                <td>
                    <asp:TextBox ID="tbEmail" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbEmail"
                        Display="None" ErrorMessage="Please enter an email address." />
                </td>
            </tr>
            <tr>
                <td class="columnHeader" style="width: 120px">
                    <asp:Literal ID="lWorkPhone" runat="Server">Work Phone:</asp:Literal>
                    <span class="requiredField">*</span>
                </td>
                <td>
                    <asp:TextBox ID="tbWorkPhone" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbWorkPhone"
                        Display="None" ErrorMessage="Please enter a work phone." />
                </td>
            </tr>
            <tr>
                <td class="columnHeader" style="width: 120px">
                    <asp:Literal ID="lMobilePhone" runat="Server">Mobile Phone:</asp:Literal>
                </td>
                <td>
                    <asp:TextBox ID="tbMobilePhone" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <hr />
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnContinue_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
