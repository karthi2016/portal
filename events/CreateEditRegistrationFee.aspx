<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditRegistrationFee.aspx.cs" Inherits="events_CreateEditRegistrationFee" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlEventOwner" Visible="false" />
    <a href="/events/CreateEditEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name %>
        ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Label runat="server" ID="lblTitleAction" />
    Event Registration Fee
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <asp:Literal ID="lBasicInfo" runat="Server">Basic Information</asp:Literal> </h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lEvent" runat="Server">Event:</asp:Literal>
                    </td>
                    <td colspan="3" align="left">
                        <asp:Label runat="server" ID="lblEventName"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lName" runat="Server">Name:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td colspan="3">
                        <asp:TextBox runat="server" ID="tbName" Width="450" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" ControlToValidate="tbName"
                            Display="None" ErrorMessage="Please specify a Name." />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lRegularRegPrice" runat="Server">Regular Reg. Price:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbRegularPrice" Width="75" />
                        USD
                        <asp:RequiredFieldValidator runat="server" ID="rfvRegularPrice" ControlToValidate="tbRegularPrice"
                            Display="None" ErrorMessage="Please specify a Regular Reg. Price." />
                        <asp:CompareValidator runat="server" ID="cvRegularPrice" ControlToValidate="tbRegularPrice"
                            Display="None" ErrorMessage="Please specify a valid number for Regular Reg. Price."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvRegularPriceType" ControlToValidate="tbRegularPrice"
                            Display="None" ErrorMessage="Please specify a valid number for Regular Reg. Price."
                            Operator="DataTypeCheck" Type="Currency" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lPreRegPrice" runat="Server">Pre-Reg. Price:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbPreRegPrice" Width="75" />
                        USD
                        <asp:CompareValidator runat="server" ID="cvPreRegPrice" ControlToValidate="tbPreRegPrice"
                            Display="None" ErrorMessage="Please specify a valid number for Pre-Reg. Price."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvPreRegPriceType" ControlToValidate="tbPreRegPrice"
                            Display="None" ErrorMessage="Please specify a valid number for Pre-Reg. Price."
                            Operator="DataTypeCheck" Type="Currency" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDisplayOrder" runat="Server">Display Order:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbDisplayOrder" Width="75" />
                        <asp:CompareValidator runat="server" ID="cvDisplayOrder" ControlToValidate="tbDisplayOrder"
                            Display="None" ErrorMessage="Please specify a valid number for Display Order."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvDisplayOrderType" ControlToValidate="tbDisplayOrder"
                            Display="None" ErrorMessage="Please specify a valid number for Display Order."
                            Operator="DataTypeCheck" Type="Integer" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lEarlyRegPrice" runat="Server">Early Reg. Price:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbEarlyRegPrice" Width="75" />
                        USD
                        <asp:CompareValidator runat="server" ID="cvEarlyRegPrice" ControlToValidate="tbEarlyRegPrice"
                            Display="None" ErrorMessage="Please specify a valid number for Early Reg. Price."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvEarlyRegPriceType" ControlToValidate="tbEarlyRegPrice"
                            Display="None" ErrorMessage="Please specify a valid number for Early Reg. Price."
                            Operator="DataTypeCheck" Type="Currency" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lCode" runat="Server">Code:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbCode" Width="75" />
                    </td>
                    <td class="columnHeader">
                     <asp:Literal ID="lLateRegPrice" runat="Server">Late Reg. Price:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbLateRegPrice" Width="75" />
                        USD
                        <asp:CompareValidator runat="server" ID="cvLateRegPrice" ControlToValidate="tbLateRegPrice"
                            Display="None" ErrorMessage="Please specify a valid number for Late Reg. Price."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvLateRegPriceType" ControlToValidate="tbLateRegPrice"
                            Display="None" ErrorMessage="Please specify a valid number for Late Reg. Price."
                            Operator="DataTypeCheck" Type="Currency" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lRegistrationInformation" runat="Server">Registration Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        
                    </td>
                    <td>
                       
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lConfirmationEmail" runat="Server">Confirmation Email:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlConfirmationEmail" DataTextField="Name" DataValueField="ID" AppendDataBoundItems="true">
                            <asp:ListItem Text="No confirmation email"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lRequiresApproval" runat="Server">Requires Approval?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkRequiresApproval" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lRegistrantClass" runat="Server">Registrant Class:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlRegistrantClass" DataTextField="Name" DataValueField="ID"
                            AppendDataBoundItems="true">
                            <asp:ListItem Text="---- Select ----" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lGuestReg" runat="Server">Is Guest Registration?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsGuestRegistration" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lRegistrantCategory" runat="Server">Registrant Category:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlRegistrantCategory" DataTextField="Name"
                            DataValueField="ID" AppendDataBoundItems="true">
                            <asp:ListItem Text="---- Select ----" />
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lDescription" runat="Server">Description</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <telerik:RadEditor runat="server" ID="reDescription" ToolsFile="~/controls/telerik/ToolsFileDeluxe.xml" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lWebSettings" runat="Server">Web Settings</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lSellOnline" runat="Server">Sell Online?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkSellOnline" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lSellFrom" runat="Server">Sell From:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpSellFrom" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lAllowPayLater" runat="Server">Allow Customers To Pay Later?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkAllowCustomersToPayLater" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lSellUntil" runat="Server">Sell Until:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpSellUntil" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDisplayPriceAs" runat="Server">Display Price As:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbDisplayPriceAs" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lNewUntil" runat="Server">New Until:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpNewUntil" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="sectionContent">
        <center>
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" />
            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"  CausesValidation="false" />
        </center>
    </div>
     <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
        <li><a href="/events/CreateEditEvent.aspx?contextID=<%=targetEvent.ID %>">Back to Edit
                    <%=targetEvent.Name %>
                    Event</a></li>
                <li runat="server" id="liEventOwnerTask" visible="false">
                    <asp:HyperLink runat="server" ID="hlEventOwnerTask" /></li>
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
