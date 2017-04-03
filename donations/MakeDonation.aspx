<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="MakeDonation.aspx.cs" Inherits="donations_MakeDonation" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .none {
            display: none;
        }

        .hidden {
            visibility: hidden;
        }

        input.ng-invalid {
            border-color: red;
        }

        [ng\:cloak], [ng-cloak], [data-ng-cloak], [x-ng-cloak], .ng-cloak, .x-ng-cloak {
            display: none !important;
        }
    </style>
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
    <!-- ms-recompile custom directive is required because we need to recompile whole controller upon partial update of UpdatePanel -->
    <div id="ng-app" ng-cloak ng-app="msPayments" ng-controller="PaymentController as payments" ng-init="payments.paymentCreditCard=true" ms-recompile> 
        <ng-form name="payments.cardForm">
            <asp:Literal ID="PageText" runat="server" />
            <asp:UpdatePanel ID="upDonationForm" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="section" style="margin-top: 10px">
                    <div class="sectionHeaderTitle">
                        <h2>
                            <asp:Literal ID="lWhereToDonate" runat="server">Where would you like to donate?</asp:Literal></h2>
                    </div>
                    <div class="sectionContent">
                        <asp:Label ID="lblNoFunds" runat="server" ForeColor="Red" Text="Unfortunately, there are no fundraising products available at this time. Please check back later to make a donation." />

                        <asp:RadioButtonList runat="server" ID="rblProducts" DataTextField="Name" AutoPostBack="True" DataValueField="ID" OnSelectedIndexChanged="rblProducts_SelectedIndexChanged" CssClass="rbl-products" />
                        <asp:RequiredFieldValidator ID="rfvProducts" runat="server" ControlToValidate="rblProducts" Display="None" ErrorMessage="Please specify where to donate." />
                    </div>
                </div>
                <div class="section" style="margin-top: 10px">
                    <div class="sectionHeaderTitle">
                        <h2>
                            <asp:Literal ID="lPersonalInfo" runat="server">Personal Information</asp:Literal></h2>
                    </div>
                    <div class="sectionContent">
                        <table>
                            <tr>
                                <td valign="top">
                                    <h3>
                                        <asp:Literal ID="lContactInfo" runat="server">Contact Information</asp:Literal></h3>
                                    <table>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Literal ID="lFirstName" runat="server">First Name:</asp:Literal></td>
                                            <td>
                                                <asp:TextBox ID="tbFirstName" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Literal ID="lLastName" runat="server">Last Name:</asp:Literal></td>
                                            <td>
                                                <asp:TextBox ID="tbLastName" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Literal ID="lEmailAddress" runat="server">Email Address:</asp:Literal></td>
                                            <td>
                                                <asp:TextBox ID="tbEmailAddress" runat="server" /></td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <h3>
                                        <asp:Literal ID="lBillingAddress" runat="server">Billing Address</asp:Literal></h3>
                                    <cc1:AddressControl ID="acBillingAddress" CssClass="columnHeader" IsRequired="True" EnableValidation="True"
                                        runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>                
                    <div class="section" id="divPayment" runat="server">
                        <div class="sectionHeaderTitle">
                            <h2>
                                <asp:Literal ID="lDonation" runat="server">Donation</asp:Literal></h2>
                        </div>
                        <div class="sectionContent">
                            <table style="width: 100%">
                                <tr valign="top">
                                <td>
                                    <h3>
                                        <asp:Literal ID="lAmount" runat="server">Amount</asp:Literal></h3>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:RadioButton runat="server" GroupName="rbAmount" ID="rbAmount25" Text="$25" /></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:RadioButton runat="server" GroupName="rbAmount" ID="rbAmount100" Text="$100" /></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:RadioButton runat="server" GroupName="rbAmount" ID="rbAmount500" Text="$500" /></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:RadioButton runat="server" GroupName="rbAmount" ID="rbAmountOther" Text="Other:" />
                                                <asp:TextBox runat="server" ID="tbAmountOther" Width="60" />
                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbAmountOther" ID="revAmountOther" ValidationExpression="^\d+(\.\d\d)?$" Display="None" ErrorMessage="Please specify a valid amount to donate." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <div id="divCreditCard">
                                        <h3>
                                            <asp:Literal ID="lCreditCardInfoHeader" runat="server">Credit Card Information</asp:Literal></h3>
                                        <table style="width: 500px">
                                            <tr>
                                                <td class="columnHeader">
                                                    <asp:Literal ID="lNameOnCard" runat="server">Name on Card: </asp:Literal><span id="spnNameRequired" class="requiredField" runat="server">*</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbName" runat="server" autocomplete="off" />
                                                    <asp:RequiredFieldValidator ID="rfvCCNameOnCard" runat="server" ControlToValidate="tbName"
                                                        Display="None" ErrorMessage="You have not entered the name on your credit card." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="columnHeader">
                                                    <asp:Literal ID="lCreditCardNumber" runat="server">Credit Card Number: </asp:Literal><span id="spnCreditCardRequired" class="requiredField" runat="server">*</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbCreditCardNumber" runat="server" autocomplete="off" MaxLength="16" CssClass="cc-number" 
                                                        ng-class="{hidden: payments.accountIsTokenized}" ng-model="payments.cardInfo.account" ng-pattern="/^[0-9]{13,19}$/"/>
                                                    <asp:RequiredFieldValidator ID="rfvCreditCardNumber" runat="server" ControlToValidate="tbCreditCardNumber"
                                                        Display="None" ErrorMessage="You have not entered your credit card number." />
                                                
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="columnHeader">
                                                    <asp:Literal ID="lCreditCardCVV" runat="server">Credit Card Security Code (CVV): </asp:Literal><span id="spnCVVRequired" class="requiredField" runat="server">*</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbCCV" runat="server" autocomplete="off" ng-model="payments.cardInfo.cvv"/>
                                                    <asp:RequiredFieldValidator ID="rfvCardSecurity" runat="server" ControlToValidate="tbCCV"
                                                        Display="None" ErrorMessage="You have not entered the security code on the back of your card." />
                                                    <asp:RegularExpressionValidator ControlToValidate="tbCCV" runat="server" ID="revCCV" Enabled="false" Display="None" 
                                                        ValidationExpression="(^\d{3,4}$)"
                                                        ErrorMessage="You have not entered a valid value for CCV Code." SetFocusOnError="true" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="columnHeader">
                                                    <asp:Literal ID="lCreditCardExp" runat="server">Credit Card Expiration: </asp:Literal><span id="spnExpirationRequired" class="requiredField" runat="server">*</span>
                                                </td>
                                                <td>
                                                    <cc1:MonthYearPicker ID="myExpiration" runat="server" CssClass="monthYearPicker" ms-month-year="payments.setExpiration"/>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <asp:HiddenField runat="server" ID="hfOrderBillToId" />
                                    <div id="dvPriorityData" runat="server" class="pp-config" style="display:none;" ms-payment-config/>       
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
        </ng-form>
        <div class="sectionContent">
            <div id="divSaveContact" align="center" style="padding-top: 20px" runat="server">
                <asp:Button runat="server" OnClick="btnSaveContact_Click" Text="Save Contact" />
            </div>
            <div id="divDonate" align="center" style="padding-top: 20px" runat="server" visible="False">
                <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Donate" runat="server" Width="80" Height="50" CssClass="save-token none" />
                <input type="button" value="Donate" ms-submit-button="payments.process" />

                <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server" CausesValidation="false" />
                <div class="clearBothNoSPC" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/bluepay.v3.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.5.7/angular.min.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.module.js"></script>    
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.recompile.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.monthyear.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.submit.button.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.radiobutton.bind.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.radiobutton.click.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.config.value.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.config.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.link.button.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.focus.directive.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.controller.v3.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.priorityPayments.v2.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.bluePay.v3.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.paymentService.factory.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.priorityPaymentsTokenizer.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.cardType.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.errorReporter.service.js"></script>
    <script type="text/javascript" src="https://cdn.membersuite.com/console/js/paymentProviderScripts/payment.logger.service.js"></script>
</asp:Content>
