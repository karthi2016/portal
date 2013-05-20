<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="RequestRefund.aspx.cs" Inherits="financial_RequestRefund" %>

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
                var spn = document.getElementById('spnTotal');
                if (spn == null) return;

                var total = getTotalAmounts().toFixed( 2 );
                 if(total < 0)
                  total = 0.00;
                 spn.innerHTML =  total;
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
           
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Request a Refund
    <% if (CurrentEntity.ID != targetEntity.ID) Response.Write(string.Format("for {0}", targetEntity.Name)); %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lAvailableCredits" runat="server" >Available Credits</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvCredits" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no available credits to be refunded." OnRowDataBound="gvCredits_OnRowDataBound">
                <Columns>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Refund">
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
                    <asp:TemplateField HeaderText="Amount To Refund" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            $<asp:TextBox ID="tbAmountToUse" Width="60" Enabled="false" runat="server" />
                            <asp:CompareValidator ID="valAmount" runat="server" ControlToValidate="tbAmountToUse"
                                Type="Currency" Operator="LessThanEqual" ValueToCompare='<%# Bind("AmountAvailable", "{0:F}") %>'
                                ErrorMessage='<%# "Please specify a valid amount to refund for " + Eval("Name") %>'
                                Display="None" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div style="text-align: right">
        <asp:Literal ID="lTotalRefund" runat="server" ><b>TOTAL TO REFUND: </b></asp:Literal>$<span id="spnTotal">0.00</span>
    </div>
    <div class="sectionContent">
        <div align="center" style="padding-top: 20px">
            <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Continue" runat="server" />
            <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server" />
            <div class="clearBothNoSPC">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
