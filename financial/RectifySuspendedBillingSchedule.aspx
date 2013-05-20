<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="RectifySuspendedBillingSchedule.aspx.cs" Inherits="financial_RectifySuspendedBillingSchedule" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="AccountHistory.aspx">Account History</a> &gt; <a href="ViewOrder.aspx?contextID=<%=GetSearchResult( targetSchedule,"Order", null) %>">
        View Order</a> &gt;
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Update Credit Card Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <table style="width: 450px; padding-bottom: 20px">
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lOrderNum" runat="server" >Order #</asp:Literal>
            </td>
            <td>
                <asp:HyperLink ID="hlOrderNo" runat="server" NavigateUrl="/financial/ViewOrder.aspx?contextID="
                    Text="21032" />
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lDate" runat="server" >Date</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblOrderDate" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lAmount" runat="server" >Amount Remaining to be Billed:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblAmountRemaining" runat="server" />
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lOrderTotal" runat="server" >Order Total:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblOrderTotal" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <p>
        <asp:Literal ID="lYourCreditCardHasBeenDeclined" runat="server" >Your credit card has been declined, which has suspended this order. Please update
        your billing information to resume billing.</asp:Literal>
    </p>
    <div class="section" id="divPayment" runat="server">
        <div class="sectionHeaderTitle">
            <h2>
               <asp:Literal ID="lPayment" runat="server" >Payment Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table style="width: 100%">
                <tr valign="top">
                    <td>
                        <div id="divCreditCard">
                            <h3>
                                <asp:Literal ID="lCreditCardInfo" runat="server" >Credit Card Information</asp:Literal></h3>
                            <table style="width: 500px">
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lCreditCardNumber" runat="server" >Credit Card Number:</asp:Literal> <span id="spnCreditCardRequired" class="requiredField">*</span>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbCreditCardNumber" runat="server" />
                                        <asp:RequiredFieldValidator ID="rfvCreditCardNumber" runat="server" ControlToValidate="tbCreditCardNumber"
                                            Display="None" ErrorMessage="You have not entered your credit card number." />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lCreditCardExpiration" runat="server" >Credit Card Expiration:</asp:Literal> <span id="spnExpirationRequired" class="requiredField">*</span>
                                    </td>
                                    <td>
                                        <cc1:MonthYearPicker ID="myExpiration" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td>
                        <h3>
                            <asp:Literal ID="lBillingAddress" runat="server" >Billing Address</asp:Literal></h3>
                        <cc1:AddressControl ID="acBillingAddress" IsRequired="true" EnableValidation="False"
                            runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <hr />
    <div class="sectionContent">
        <div align="center" style="padding-top: 20px">
            <asp:Button ID="btnUpdatePaymentInfo" Text="Update Payment Info" runat="server" OnClick="btnUpdatePaymentInfo_Click" />
            <asp:Button ID="btnCancel" Text="Cancel" runat="server" OnClick="btnCancel_Click"
                CausesValidation="false" />
            <div class="clearBothNoSPC">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
