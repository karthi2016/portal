<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditPromoCode.aspx.cs" Inherits="events_CreateEditPromoCode" %>

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
    Event Discount / Promo Code
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
            <asp:Literal ID="lBasicInfo" runat="server">
                Basic Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lEvent" runat="server">Event:</asp:Literal>
                    </td>
                    <td colspan="3" align="left">
                        <asp:Label runat="server" ID="lblEventName"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lCode" runat="server">Code:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbCode" Width="75" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvCode" Display="None" ErrorMessage="Please specify a value for Code."
                            ControlToValidate="tbCode" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lAmount" runat="server">Amount: </asp:Literal><span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbAmount" Width="75" />
                        USD
                        <asp:RequiredFieldValidator runat="server" ID="rfvAmount" Display="None" ErrorMessage="Please specify a value for Amount."
                            ControlToValidate="tbAmount" />
                        <asp:CompareValidator runat="server" ID="cvAmount" ControlToValidate="tbAmount" Display="None"
                            ErrorMessage="Please specify a valid number for Amount." Operator="GreaterThanEqual"
                            ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvAmountType" ControlToValidate="tbAmount"
                            Display="None" ErrorMessage="Please specify a valid number for Amount." Operator="DataTypeCheck"
                            Type="Currency" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lName" runat="server">Name: </asp:Literal><span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbName" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" Display="None" ErrorMessage="Please specify a value for Name."
                            ControlToValidate="tbName" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lPercentage" runat="server">Percentage: </asp:Literal><span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbPercentage" Width="75" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvPercentage" Display="None" ErrorMessage="Please specify a value for Percentage."
                            ControlToValidate="tbPercentage" />
                        <asp:CompareValidator runat="server" ID="cfPercentage" ControlToValidate="tbAmount"
                            Display="None" ErrorMessage="Please specify a valid number for Percentage." Operator="GreaterThanEqual"
                            ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cfPercentageType" ControlToValidate="tbAmount"
                            Display="None" ErrorMessage="Please specify a valid number for Percentage." Operator="DataTypeCheck"
                            Type="Currency" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDiscountProduct" runat="server">Discount Product:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlDiscountProduct" DataTextField="Text" DataValueField="Value"
                            AppendDataBoundItems="true">
                            <asp:ListItem Text="---- Select ----" />
                        </asp:DropDownList>
                        <asp:CompareValidator runat="server" ID="cvDiscountProduct" ControlToValidate="ddlDiscountProduct"
                            Display="None" ErrorMessage="Please specify a Discount Product." Operator="NotEqual"
                            ValueToCompare="0" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lValidFrom" runat="server">Valid From:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpValidFrom" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lIsActive" runat="server">Is Active?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsActive" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lValidUntil" runat="server">Valid Until:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpValidUntil" />
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
            <asp:TextBox runat="server" ID="tbDescription" Columns="130" Rows="5" TextMode="MultiLine" />
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
