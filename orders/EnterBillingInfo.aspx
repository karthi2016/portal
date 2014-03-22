<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EnterBillingInfo.aspx.cs" Inherits="orders_EnterBillingInfo" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="bi" TagName="BillingInfo" Src="~/controls/BillingInfo.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Enter Billing Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <span style="font-family: Arial, Helvetica, sans-serif; font-size: 16px; font-weight: bold">
        <asp:Literal ID="lAmountDue" runat="server">Amount Due:</asp:Literal>
        <asp:Label ID="lblAmountDue" ForeColor="Green" runat="server" Text="$0.00" />
    </span>
    <div class="sectHeaderTitle" style="margin-top: 20px">
        <h2>
            <asp:Literal ID="lShippingInfo" runat="Server">How would you like to pay?</asp:Literal></h2>
    </div>
    <asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true"
        Font-Bold="true" ForeColor="red" ShowMessageBox="false" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    <asp:Literal ID="PageText" runat="server">
    </asp:Literal>
    <asp:Literal ID="lMessages" runat="server" />
    <bi:BillingInfo ID="BillingInfoWidget" runat="server" />
    <hr style="width: 100%" />
    <div style="padding-top: 20px" id="divDiscountPromoCode" runat="server">
        <div class="sectHeaderTitle">
            <h2>
                Apply Discount/Promo Code
            </h2>
        </div>
        <table>
            <tr>
                <td colspan="3">
                    <asp:Literal ID="lApplyDiscountPromoCode" runat="server">Apply discount/promo code:</asp:Literal>
                </td>
            </tr>
            <tr>
                <td style="width: 50px">
                    Code:
                </td>
                <td>
                    <asp:TextBox ID="tbPromoCode" runat="server" Width="80px" />
                    <asp:RequiredFieldValidator ID="rfvPromoCode" runat="server" ControlToValidate="tbPromoCode"
                        ValidationGroup="DiscountCode" ErrorMessage="Please enter a discount code." Display="None" />
                    <asp:Button ID="btnApplyCoupon" ValidationGroup="DiscountCode" OnClick="btnApplyDiscountCode_Click"
                        CommandName="ApplyDiscount" runat="server" Text="Apply Code" />
                </td>
            </tr>
        </table>
    </div>
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" runat="server" Text="Continue" OnClick="btnContinue_Click" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel Your Order" CausesValidation="false"
            OnClick="lbCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
