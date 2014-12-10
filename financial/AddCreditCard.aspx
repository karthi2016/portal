<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="AddCreditCard.aspx.cs" Inherits="financial_AddCreditCard" %>
<%@ Register TagPrefix="cc1" Namespace="MemberSuite.SDK.Web.Controls" Assembly="MemberSuite.SDK.Web" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Save a Credit Card to Your Account
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server">Enter the credit card details below.
    </asp:Literal>
    <table style="width: 400px">
        <tr>
            <td rowspan="4" style="width: 120px">
                <img src="/images/creditcards.gif" />
            </td>
            <td style="width: 100px">
                Card Number: <span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbCardNumber" CssClass="inputText" runat="server" autocomplete="off" />
                <asp:RequiredFieldValidator ID="rfvCreditCardNumber" runat="server" ControlToValidate="tbCardNumber"
                      Display="None" ErrorMessage="You have not entered a credit card number."
                    />
            </td>
        </tr>
        <tr>
            <td>
                Name on Card:<span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbNameOnCard" runat="server" autocomplete="off" />
                <asp:RequiredFieldValidator ID="rfvNameOnCard" runat="server" ControlToValidate="tbNameOnCard"
                    Display="None" ErrorMessage="You have not entered a name for the credit card."
                      />
            </td>
        </tr>
        <tr>
            <td>
                Security Code:<span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbCCV" Width="50px" runat="server" autocomplete="off" />
                <asp:RequiredFieldValidator ID="rfvSecurityCode" runat="server" ControlToValidate="tbCCV"
                    Display="None" ErrorMessage="You have not entered a security code for the credit card."
                     />
            </td>
        </tr>
        <tr>
            <td>
                Expiration Date:<span class="requiredField">*</span>
            </td>
            <td>
                <cc1:monthyearpicker id="myExpiration" runat="server" />
            </td>
        </tr>
    </table>
     <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" runat="server" Text="Save this Card" OnClick="btnSaveCard_Click" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel " OnClick="lbCancel_Click" CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
