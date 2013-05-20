<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditInformationLink.aspx.cs" Inherits="events_CreateEditInformationLink" %>

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
    Supplemental Information Link
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
                        <asp:Literal ID="lName" runat="Server">Name: </asp:Literal><span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbName" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" Display="None" ErrorMessage="Please specify a value for Name."
                            ControlToValidate="tbName" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lIsActive" runat="Server">Is Active?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsActive" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lCode" runat="Server">Code:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbCode" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lDisplayOrder" runat="Server">Display Order:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbDisplayOrder" Width="75" />
                        <asp:RequiredFieldValidator ID="rfvDisplayOrder" runat="server" Display="None" ErrorMessage="Please specify a value for Display Order."
                            ControlToValidate="tbDisplayOrder" />
                        <asp:CompareValidator runat="server" ID="cvDisplayOrder" ControlToValidate="tbDisplayOrder"
                            Display="None" ErrorMessage="Please specify a valid number for Display Order."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvDisplayOrderType" ControlToValidate="tbDisplayOrder"
                            Display="None" ErrorMessage="Please specify a valid number for Display Order."
                            Operator="DataTypeCheck" Type="Integer" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lHTML" runat="Server">HTML</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <telerik:RadEditor NewLineBr="false" runat="server" ID="reHtml" ToolsFile="~/controls/telerik/ToolsFileDeluxe.xml" />
        </div>
    </div>
    <div class="sectionContent">
        <center>
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" />
            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" CausesValidation="false" />
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
