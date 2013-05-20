<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="MakePayment.aspx.cs" Inherits="financial_MakePayment" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript">
            
            function initialize()
            {
            <asp:Literal ID="lStartupScript" runat="server" />;    
            updateSummary();     
            }
            function setTextBoxValue(cb, textBoxID, valueToSet) {

                if (cb == null) return;
                var tb = document.getElementById(textBoxID);

                if (tb == null) return;

                if (!cb.checked) {
                    tb.disabled = true;
                    tb.value = '0.00';
                }
                else {
                    tb.disabled = false;
                    tb.value = valueToSet;
                }

                updateTotals();
            }

            function updateTotals() {
                var tbAmount = document.getElementById('<%= tbAmount.ClientID %>');
                if (tbAmount == null) return;

                var total = getTotalAmounts().toFixed( 2 );
                 if(total < 0)
                  total = 0.00;
                 tbAmount.value =  total;
                 updateSummary();
            }
            
            function getTotalAmounts(){
            // the zero is important since we add plus signs, it's NOT a typo!
              
                var result =  0 <asp:Literal ID="lTotalScript" runat="server" />;
                if(result < 0)
                        result = 0.00;
                return result;
            }
            
                    
            function onTotalChange()
            {
                updateTotals();
            }

            function updateSummary()
            {
                var tbAmount = document.getElementById('<%= tbAmount.ClientID %>');
                var totalApplied = getTotalAmounts();
                var totalAmount = parseFloat( tbAmount.value );
                
                if ( isNaN( totalAmount) )
                     totalAmount = 0;
                
                var amountToApply = totalApplied;
                var amountToCredit = totalAmount - totalApplied;
                
                var tbAmountToApply = document.getElementById('tbAmountToApply');
                var tbAmountToCredit = document.getElementById('tbAmountToCredit');
                
             
                tbAmountToApply.innerText = parseFloat( totalApplied ).toFixed(2);
                tbAmountToCredit.innerText= parseFloat( amountToCredit ).toFixed(2);
            
                togglePaymentOptions();
            }

  
            function togglePaymentOptions()
            {
                var rfvCreditCard = document.getElementById( '<%=rfvCreditCardNumber.ClientID %>');
                var tbCreditCard = document.getElementById( '<%=tbCreditCardNumber.ClientID %>');
                var spnCreditCardRequired = document.getElementById("spnCreditCardRequired");

                var rfvName = document.getElementById( '<%=rfvCCNameOnCard.ClientID %>');
                var tbName = document.getElementById( '<%=tbName.ClientID %>');
                var spnNameRequired = document.getElementById("spnNameRequired");
                
                var rfvSecurityCode = document.getElementById( '<%=rfvCardSecurity.ClientID %>');
                var tbSecurityCode = document.getElementById( '<%=tbCVV.ClientID %>');
                var spnCVVRequired = document.getElementById("spnCVVRequired");
                
                var mdpExpiration = document.getElementById( '<%=myExpiration.ClientID %>');
                var spnExpirationRequired = document.getElementById("spnExpirationRequired");
                
                var tbAmount = document.getElementById('<%= tbAmount.ClientID %>');
                var totalAmount = parseFloat( tbAmount.value );

                if ( isNaN( totalAmount) )
                     totalAmount = 0;

                var enable = false;
                var visibility = "hidden";

                if(totalAmount > 0)
                {  
                    enable = true;   
                    visibility = "visible";
                }
                

                tbCreditCard.disabled = !enable;
                tbName.disabled = !enable;
                tbSecurityCode.disabled = !enable;
                mdpExpiration.disabled = !enable;  

                spnCreditCardRequired.style.visibility = visibility;
                spnNameRequired.style.visibility = visibility;
                spnCVVRequired.style.visibility = visibility;
                spnExpirationRequired.style.visibility = visibility;

                ValidatorEnable(rfvCreditCard,enable);
                ValidatorEnable(rfvName,enable);
                ValidatorEnable(rfvSecurityCode,enable);
            }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Make a Payment
    <% if (CurrentEntity.ID != targetEntity.ID) Response.Write(string.Format("for {0}", targetEntity.Name)); %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
   
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lOpenInvoices" runat="server">Open Invoices</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvInvoices" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No invoices found." OnRowDataBound="gvInvoices_OnRowDataBound">
                <Columns>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Pay">
                        <ItemTemplate>
                            <asp:CheckBox ID="cbUse" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\financial\ViewInvoice.aspx?contextID={0}"
                        HeaderStyle-HorizontalAlign="Left" HeaderText="Name" DataNavigateUrlFields="ID"
                        DataTextField="Name" />
                    <asp:BoundField DataField="Date" HeaderStyle-HorizontalAlign="Left" HeaderText="Date"
                        DataFormatString="{0:d}" />
                    <asp:BoundField DataField="BalanceDue" HeaderStyle-HorizontalAlign="Left" HeaderText="Balance Due"
                        DataFormatString="{0:C}" />
                    <asp:TemplateField HeaderText="Amount To Pay" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            $<asp:TextBox ID="tbAmountToPay" Width="60" Enabled="false" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lAvailableCredits" runat="server">Available Credits</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvCredits" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No available credits found." OnRowDataBound="gvCredits_OnRowDataBound">
                <Columns>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Use">
                        <ItemTemplate>
                            <asp:CheckBox ID="cbUse" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\financial\ViewInvoice.aspx?contextID={0}"
                        HeaderStyle-HorizontalAlign="Left" HeaderText="Name" DataNavigateUrlFields="ID"
                        DataTextField="Name" />
                    <asp:BoundField DataField="Date" HeaderStyle-HorizontalAlign="Left" HeaderText="Date"
                        DataFormatString="{0:d}" />
                    <asp:BoundField DataField="UseThrough" HeaderStyle-HorizontalAlign="Left" HeaderText="Expires"
                        DataFormatString="{0:d}" />
                    <asp:BoundField DataField="AmountAvailable" HeaderStyle-HorizontalAlign="Left" HeaderText="Available"
                        DataFormatString="{0:C}" />
                    <asp:TemplateField HeaderText="Amount To Use" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            $<asp:TextBox ID="tbAmountToUse" Width="60" Enabled="false" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lPaymentSummary" runat="server">Payment Summary</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td align="center" class="columnHeader">
                        <asp:Literal ID="lTotalToCharge" runat="server">TOTAL TO CHARGE:</asp:Literal>
                        $<asp:TextBox ID="tbAmount" Width="60" runat="server" onchange="updateSummary();" />
                        <font class="redHighlight">*</font>
                        <asp:CompareValidator ID="valRequired" runat="server" ControlToValidate="tbAmount"
                            ValueToCompare="-1" Type="Currency" Operator="GreaterThan" ErrorMessage="Please specify a valid total to charge"
                            Display="None">                    
                        </asp:CompareValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbAmount" Display="None" ErrorMessage="You must enter a payment amount. Click on the checkboxes to select the invoices you'd like to pay." />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <b><font color="green">$<span id="tbAmountToApply">0.00</span></font></b>
                        <asp:Literal ID="lWillBeAppliedToInvoices" runat="server">WILL BE APPLIED TO INVOICES</asp:Literal>
                    </td>
                </tr>
                <tr style="display: none">
                    <td align="center">
                        <b><font color="green">$<span id="tbAmountToCredit">0.00</span></font></b>
                        <asp:Literal ID="lWillBeAddedAsACredit" runat="server">WILL BE ADDED AS A CREDIT</asp:Literal>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" id="divPayment" runat="server">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lPaymentInformation" runat="server">Payment Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table style="width: 100%">
                <tr valign="top">
                    <td>
                        <div id="divCreditCard">
                            <h3>
                                <asp:Literal ID="lCreditCardInformation" runat="server">Credit Card Information</asp:Literal></h3>
                            <table style="width: 500px">
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lNameOnCard" runat="server">Name on Card:</asp:Literal>
                                        <span id="spnNameRequired" class="requiredField">*</span>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbName" runat="server" />
                                        <asp:RequiredFieldValidator ID="rfvCCNameOnCard" runat="server" ControlToValidate="tbName"
                                            Display="None" ErrorMessage="You have not entered the name on your credit card." />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lCreditCardNumber" runat="server">Credit Card Number: </asp:Literal><span
                                            id="spnCreditCardRequired" class="requiredField">*</span>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbCreditCardNumber" runat="server" />
                                        <asp:RequiredFieldValidator ID="rfvCreditCardNumber" runat="server" ControlToValidate="tbCreditCardNumber"
                                            Display="None" ErrorMessage="You have not entered your credit card number." />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lCVV" runat="server">Credit Card Security Code (CVV):</asp:Literal>
                                        <span id="spnCVVRequired" class="requiredField">*</span>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbCVV" runat="server" />
                                        <asp:RequiredFieldValidator ID="rfvCardSecurity" runat="server" ControlToValidate="tbCVV"
                                            Display="None" ErrorMessage="You have not entered the security code on the back of your card." />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lCreditCardExp" runat="server">Credit Card Expiration:</asp:Literal>
                                        <span id="spnExpirationRequired" class="requiredField">*</span>
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
                            <asp:Literal ID="lBillingAddress" runat="server">Billing Address</asp:Literal></h3>
                        <cc1:AddressControl ID="acBillingAddress" IsRequired="true" EnableValidation="False"
                            runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="sectionContent">
        <div align="center" style="padding-top: 20px">
            <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Continue" runat="server" />
            <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" CausesValidation="false"
                runat="server" />
            <div class="clearBothNoSPC">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
