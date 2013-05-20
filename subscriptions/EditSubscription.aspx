<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EditSubscription.aspx.cs" Inherits="subscriptions_EditSubscription" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewMySubscriptions.aspx">View My Subscriptions</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Modify Subscription
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <table>
        <tr id="trPublication" runat="server">
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lPublication" runat="server">Publication:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblPublication" runat="server" />
            </td>
        </tr>
        <tr id="trOwner" runat="server">
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lOwner" runat="server">Owner:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblOwner" runat="server" />
            </td>
        </tr>
    </table>
    <h2>
        Shipping &amp; Fulfillment</h2>
    <table>
        <tr id="trShipTo" runat="server">
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lShipTo" runat="server">Ship To:</asp:Literal>
            </td>
            <td>
                <asp:TextBox ID="tbShipTo" runat="server" />
                (leave blank to ship to yourself)
            </td>
        </tr>
        <tr id="trShippingAddress" runat="server">
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lShippingAddress" runat="server">Shipping Address:</asp:Literal>
            </td>
            <td>
                <asp:RadioButton ID="rbUseDefault" GroupName="Specify" OnCheckedChanged="rbAddress_Changed" AutoPostBack="true"
                    runat="server" Text="Use the default preferred address on file" />
                <asp:RadioButton ID="rbSpecify"  GroupName="Specify" OnCheckedChanged="rbAddress_Changed" AutoPostBack="true"
                    runat="server" Text="Specify an address" />
            </td>
        </tr>
        <tr id="trSpecifyAddress" runat="server">
            <td colspan="2">
                <asp:Literal ID="lSpecifyAddress" runat="server"><B>Specify Address Below:</B></asp:Literal>
                <br />
                <cc1:AddressControl ID="acAddress" IsRequired="true" runat="server" />
            </td>
        </tr>
        <tr id="trOnHold" runat="server">
            <td class="columnHeader" style="width: 150px" colspan="2">
                <asp:CheckBox ID="cbOnHold" runat="server" Text="I would like to place my subscription on hold." />
            </td>
        </tr>
    </table>
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" OnClick="btnSave_Click" runat="server" Text="Save Changes" />
        or
        <asp:HyperLink ID="hlViewSubscription" NavigateUrl="/subscriptions/ViewSubscription.aspx?contextID="
            runat="server">cancel</asp:HyperLink>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
