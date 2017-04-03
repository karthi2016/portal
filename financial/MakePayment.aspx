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
        function setTextBoxValue(cb, textBoxID, valueToSet, optionalPanel, disablePartialPayments) {

            if (cb == null) return;
            var tb = document.getElementById(textBoxID);

            if (tb == null) return;

            if (!cb.checked) {
                tb.disabled = true;
                tb.value = '0.00';

                if (optionalPanel && optionalPanel.length > 0) {
                    $('#' + optionalPanel).hide();
                } else {
                    $(tb).attr('class', 'disabledInput');
                    $(tb).focus(function (o){blur();});
                }
            }
            else {
                tb.disabled = disablePartialPayments == 'True';
                tb.value = valueToSet;

                if (optionalPanel && optionalPanel.length > 0) {
                    $('#' + optionalPanel).show();
                } else {
                    $(tb).attr('class', '');
                    $(tb).attr('onfocus', '').off('focus');
                }
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

                <asp:Literal ID="lOptionalScript" runat="server" />;

                if(result < 0)
                    result = 0.00;
                return result;
            }
            
            function onTotalChange(field)
            {
                var amount = parseFloat(field.value);

                if (isNaN(amount)) {
                    amount = 0;
                }

                field.value = amount.toFixed(2);

                updateTotals();
            }

        function updateSummary() {
            var tbAmount = document.getElementById('<%= tbAmount.ClientID %>');
            var totalApplied = getTotalAmounts();
            var totalAmount = parseFloat(tbAmount.value);

            if (isNaN(totalAmount))
                totalAmount = 0;

            var amountToCredit = totalAmount - totalApplied;

            var tbAmountToApply = document.getElementById('tbAmountToApply');
            var tbAmountToCredit = document.getElementById('tbAmountToCredit');

            $(tbAmountToApply).text(parseFloat(totalApplied).toFixed(2));
            $(tbAmountToCredit).text(parseFloat(amountToCredit).toFixed(2));
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Make a Payment
    <asp:Label runat="server" ID="PageTitleExtension"></asp:Label>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server">Please check all Invoices that you would like to pay in full.</asp:Literal>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lOpenInvoices" runat="server">Open Invoices</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <asp:Repeater ID="rptInvoices" runat="server" OnItemDataBound="rptInvoices_OnRowDataBound">
                    <HeaderTemplate>
                        <tr>
                            <th style="text-align: left; width: 30px">Pay</th>
                            <th style="text-align: left;">Name</th>
                            <th style="text-align: right;">Invoice Date</th>
                            <th style="text-align: right;">Balance Due</th>
                            <th style="text-align: right; width: 120px;">Amount To Pay</th>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbUse" runat="server" />
                            </td>
                            <td>
                                <asp:HyperLink runat="server" ID="hlInvoice"></asp:HyperLink>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="lDate"></asp:Literal>
                            </td>
                            <td style="text-align: right;">
                                <asp:Literal runat="server" ID="lBalanceDue"></asp:Literal>
                                <asp:HiddenField ID="hiddenBalance" runat="server"/>
                            </td>
                            <td style="text-align: right;">
                                $<asp:TextBox runat="server" ID="tbAmountToPay" Width="60" Enabled="false" onfocus="blur();" CssClass="disabledInput" style="text-align: right;" />
                            </td>
                        </tr>
                        <tr runat="server" id="trOptional" style="display: none;">
                            <td></td>
                            <td colspan="3"><asp:Label runat="server" ID="lOptLabel"></asp:Label> (<asp:Literal runat="server" ID="lOptionalDescr"></asp:Literal>)*</td>
                            <td style="text-align: right;">
                                $<asp:TextBox runat="server" ID="tbOptionalAmount" Width="60" style="text-align: right;" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td>No invoices found.</td>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
                <tr runat="server" ID="trOptionalMessage" Visible="False">
                    <td colspan="5">
                        <asp:Literal runat="server" ID="lOptionalMessage">
                            <p>&nbsp;</p><p>* You can change the amount that you would like to pay on an optional line item by entering a new amount. If you would like to remove this line item, you can do so by entering $0.00 in the Amount To Pay field.</p>
                        </asp:Literal>
                        <asp:Literal runat="server" ID="lOptionalLabel" Visible="False">Optional Amount</asp:Literal>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="secCredits">
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
                    <%--
                    <asp:BoundField DataField="UseThrough" HeaderStyle-HorizontalAlign="Left" HeaderText="Expires"
                        DataFormatString="{0:d}" />
                        --%>
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
                        $<asp:TextBox ID="tbAmount" Width="60" onfocus="blur();" CssClass="disabledInput" runat="server" style="text-align: right;" />
                        <span class="redHighlight">*</span>
                        <asp:CompareValidator ID="valRequired" runat="server" ControlToValidate="tbAmount"
                            ValueToCompare="-1" Type="Currency" Operator="GreaterThan" ErrorMessage="Please specify a valid total to charge"
                            Display="None">                    
                        </asp:CompareValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbAmount"
                            Display="None" ErrorMessage="You must enter a payment amount. Click on the checkboxes to select the invoices you'd like to pay." />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <b><span class="hlteMon">$<span id="tbAmountToApply">0.00</span></span></b>
                        <asp:Literal ID="lWillBeAppliedToInvoices" runat="server">WILL BE APPLIED TO INVOICES</asp:Literal>
                    </td>
                </tr>
                <tr style="display: none">
                    <td align="center">
                        <b><span class="hlteMon">$<span id="tbAmountToCredit">0.00</span></span></b>
                        <asp:Literal ID="lWillBeAddedAsACredit" runat="server">WILL BE ADDED AS A CREDIT</asp:Literal>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Continue to Billing Information"
             runat="server" style="width:250px" />
                or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel Your Payment" OnClick="btnCancel_Click" CausesValidation="false" />
     
        <div class="clearBothNoSPC">
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
