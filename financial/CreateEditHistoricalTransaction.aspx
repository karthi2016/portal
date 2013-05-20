<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditHistoricalTransaction.aspx.cs" Inherits="financial_CreateEditHistoricalTransaction" %>

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
    <asp:Label runat="server" ID="lblTitleAction">Create</asp:Label>
    Historical Transaction
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lHistoricalTransaction" runat="server" >Historical Transaction Details</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lName" runat="server" >Name:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbName" Width="400" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                       <asp:Literal ID="lDate" runat="server" >Date:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpDate" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvDate" ControlToValidate="dtpDate"
                            Display="None" ErrorMessage="Please specify a Date." />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lType" runat="server" >Type:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlType" runat="server" DataTextField="Text" DataValueField="Value" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lReferenceNumber" runat="server" >Reference Number:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbReferenceNumber" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lTotal" runat="server" >Total:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbTotal" Width="60" /> USD
                        <asp:CompareValidator runat="server" ID="cvTotal" ControlToValidate="tbTotal" Display="None"
                            ErrorMessage="Please specify a valid number for Total." Operator="GreaterThanEqual"
                            ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvTotalType" ControlToValidate="tbTotal"
                            Display="None" ErrorMessage="Please specify a valid number for Total." Operator="DataTypeCheck"
                            Type="Currency" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lMemo" runat="server" >Memo:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbMemo" Width="620" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lNotes" runat="server" >Notes:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbNotes" TextMode="MultiLine" Columns="100" Rows="5"  />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="sectionContent">
        <center>
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" />
            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"
                CausesValidation="false" />
        </center>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server" >Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li runat="server" id="liDeleteHistoricalTransaction">
                    <asp:LinkButton runat="server" ID="lbDeleteHistoricalTransaction" Text="Delete Historical Transaction"
                        OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;"
                        OnClick="lbDeleteHistoricalTransaction_Click" /></li>
                <li><a href="<%=CompleteUrl %>">Go Back</a></li>
                <li><a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">Back to View
                    <%=targetEvent.Name %>
                    Event</a></li>
                <li runat="server" id="liEventOwnerTask" visible="false">
                    <asp:HyperLink runat="server" ID="hlEventOwnerTask" /></li>
                <li><a href="/"><asp:Literal ID="lGoHome" runat="server" >Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
