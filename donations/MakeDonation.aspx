<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="MakeDonation.aspx.cs" Inherits="donations_MakeDonation" %>


<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Make a Donation
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
  <asp:Literal ID="PageText" runat="server"/>
    <asp:UpdatePanel ID="upDonationForm" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="section" style="margin-top: 10px">
            <div class="sectionHeaderTitle">
                <h2>  <asp:Literal ID="lWhereToDonate" runat="server">Where would you like to donate?</asp:Literal></h2>
            </div>
            <div class="sectionContent">
            <asp:Label ID="lblNoFunds" runat="server" ForeColor=Red Text="Unfortunately, there are no fundraising products available at this time. Please check back later to make a donation." />

                <asp:RadioButtonList runat="server" ID="rblProducts" DataTextField="Name" AutoPostBack="true" DataValueField="ID" OnSelectedIndexChanged="rblProducts_SelectedIndexChanged" />
                <asp:RequiredFieldValidator ID="rfvProducts" runat="server" ControlToValidate="rblProducts" Display="None" ErrorMessage="Please specify where to donate." />
            </div>
        </div>
        <div class="section" style="margin-top: 10px">
            <div class="sectionHeaderTitle">
                <h2><asp:Literal ID="lPersonalInfo" runat="server">Personal Information</asp:Literal></h2>
            </div>
            <div class="sectionContent">
                <table>
                    <tr>
                        <td valign="top">
                        <h3><asp:Literal ID="lContactInfo" runat="server">Contact Information</asp:Literal></h3>
                            <table>
                                <tr>
                                    <td class="columnHeader"><asp:Literal ID="lFirstName" runat="server">First Name:</asp:Literal></td>
                                    <td><asp:TextBox ID="tbFirstName" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="columnHeader"><asp:Literal ID="lLastName" runat="server">Last Name:</asp:Literal></td>
                                    <td><asp:TextBox ID="tbLastName" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="columnHeader"><asp:Literal ID="lEmailAddress" runat="server">Email Address:</asp:Literal></td>
                                    <td><asp:TextBox ID="tbEmailAddress" runat="server" /></td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <h3><asp:Literal ID="lBillingAddress" runat="server">Billing Address</asp:Literal></h3>
                            <cc1:AddressControl ID="acBillingAddress" CssClass="columnHeader" IsRequired="true" EnableValidation="False"
                                runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="section" id="divPayment" runat="server">
            <div class="sectionHeaderTitle">
                <h2><asp:Literal ID="lDonation" runat="server">Donation</asp:Literal></h2>
            </div>
            <div class="sectionContent">
            <table style="width: 100%">
                <tr valign="top">
                    <td>
                        <h3><asp:Literal ID="lAmount" runat="server">Amount</asp:Literal></h3>
                        <table>
                            <tr>
                                <td><asp:RadioButton runat="server" GroupName="rbAmount" ID="rbAmount25" Text="$25" /></td>
                            </tr>
                            <tr>
                                <td><asp:RadioButton runat="server" GroupName="rbAmount" ID="rbAmount100" Text="$100" /></td>
                            </tr>
                            <tr>
                                <td><asp:RadioButton runat="server" GroupName="rbAmount" ID="rbAmount500" Text="$500" /></td>
                            </tr>
                            <tr>
                                <td><asp:RadioButton runat="server" GroupName="rbAmount" ID="rbAmountOther" Text="Other:" /> <asp:TextBox runat="server" ID="tbAmountOther" Width="60" /> <asp:RegularExpressionValidator runat="server" ControlToValidate="tbAmountOther" ID="revAmountOther" ValidationExpression="^\d+(\.\d\d)?$" Display="None" ErrorMessage="Please specify a valid amount to donate." /> </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <div id="divCreditCard"  >
                            <h3><asp:Literal ID="lCreditCardInfoHeader" runat="server">Credit Card Information</asp:Literal></h3>
                            <table style="width: 500px">
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lNameOnCard" runat="server">Name on Card: <span id="spnNameRequired" class="requiredField">*</span>
                                        </asp:Literal>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbName" runat="server" autocomplete="off" />
                                        <asp:RequiredFieldValidator ID="rfvCCNameOnCard" runat="server" ControlToValidate="tbName"
                                            Display="None" ErrorMessage="You have not entered the name on your credit card." />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lCreditCardNumber" runat="server">Credit Card Number: <span id="spnCreditCardRequired" class="requiredField">*</span>
                                        </asp:Literal>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbCreditCardNumber" runat="server" autocomplete="off" />
                                        <asp:RequiredFieldValidator ID="rfvCreditCardNumber" runat="server" ControlToValidate="tbCreditCardNumber"
                                            Display="None" ErrorMessage="You have not entered your credit card number." />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lCreditCardCVV" runat="server">Credit Card Security Code (CVV): <span id="spnCVVRequired" class="requiredField">*</span>
                                        </asp:Literal>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbCVV" runat="server" autocomplete="off" />
                                        <asp:RequiredFieldValidator ID="rfvCardSecurity" runat="server" ControlToValidate="tbCVV"
                                            Display="None" ErrorMessage="You have not entered the security code on the back of your card." />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lCreditCardExp" runat="server">Credit Card Expiration: <span id="spnExpirationRequired" class="requiredField">*</span>
                                        </asp:Literal>
                                    </td>
                                    <td>
                                        <cc1:MonthYearPicker ID="myExpiration" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="rblProducts" EventName="SelectedIndexChanged" />
    </Triggers>
    </asp:UpdatePanel>
            <div class="sectionContent">
                <div align="center" style="padding-top: 20px">
                    <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Donate" runat="server" Width="80" Height="50" />
                    <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server" CausesValidation="false" />
                    <div class="clearBothNoSPC">
                    </div>
                </div>
        </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
