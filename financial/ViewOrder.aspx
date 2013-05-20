<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewOrder.aspx.cs" Inherits="financial_ViewOrder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>View Order #
        <%=GetSearchResult( targetOrder, "LocalID",null ) %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="AccountHistory.aspx">Account History</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Order #
    <%=GetSearchResult( targetOrder, "LocalID",null ) %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div style="float: right">
        <table style="width: 200px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lOrderNum" runat="server">Order #</asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult( targetOrder, "LocalID",null ) %>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lDate" runat="server">Date</asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult(targetOrder, "Date", "d")%>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lShipDate" runat="server">Ship Date</asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult(targetOrder, "ShipDate", "d")%>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lPurchaseOrder" runat="server">P/O #:</asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult(targetOrder, "PurchaseOrderNumber", null )%>
                </td>
            </tr>
        </table>
    </div>
    <div style="float: left">
        <table style="width: 500px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lBillTo" runat="server"><u>Bill To: </u></asp:Literal>
                </td>
                <td class="columnHeader">
                    <asp:Literal ID="lShipTo" runat="server"><u>Ship To: </u></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <%=GetSearchResult(targetOrder, "BillTo.Name", null)%>
                    <br />
                    <%=GetSearchResult(targetOrder, "BillingAddress", null)%>
                </td>
                <td>
                    <%=GetSearchResult(targetOrder, "ShipTo.Name", null)%>
                    <br />
                    <%=GetSearchResult(targetOrder, "ShippingAddress", null)%>
                </td>
            </tr>
        </table>
    </div>
    <div style="min-height: 95px">
    </div>
    <h2>
        <asp:Literal ID="lSummaryInfo" runat="server">Summary Information</asp:Literal></h2>
    <table style="width: 600px">
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lOrderTotal" runat="server">Order Total:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetOrder, "Total", "C")%>
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lStatus" runat="server">Status:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetOrder, "Status", "C")%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lAmountPaid" runat="server">Amount Paid:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetOrder, "AmountPaid", "C")%>
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lBalanceDue" runat="server">Balance Due:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetOrder, "BalanceDue", "C")%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                Tracking Number:
            </td>
            <td colspan="3">
                <%=GetSearchResult(targetOrder, "TrackingNumber" )%>
            </td>
        </tr>
    </table>
    <div id="divNotes" runat="server">
        <h2>
            Customer Notes</h2>
        <%=GetSearchResult(targetOrder, "CustomerNotes" )%>
    </div>
    <h2>
        <asp:Literal ID="lLineItems" runat="server">Line Items</asp:Literal></h2>
    <asp:GridView ID="gvOrderItems" AutoGenerateColumns="false" EmptyDataText="No items in this order."
        GridLines="None" runat="server">
        <Columns>
            <asp:BoundField DataField="Product.Name" HeaderText="Product" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Description" HeaderText="Desc" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Quantity" HeaderText="Quantity" DataFormatString="{0:C}"
                HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:C}"
                HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-HorizontalAlign="Left" />
        </Columns>
    </asp:GridView>
    <h2>
        <asp:Literal ID="lInvoices" runat="server">Invoices</asp:Literal></h2>
    <asp:GridView ID="gvInvoices" AutoGenerateColumns="false" EmptyDataText="No invoices are tied to this order."
        GridLines="None" runat="server">
        <Columns>
            <asp:BoundField DataField="Date" HeaderText="Date" HeaderStyle-HorizontalAlign="Left"
                DataFormatString="{0:d}" />
            <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Total" HeaderText="Total" HeaderStyle-HorizontalAlign="Left"
                DataFormatString="{0:C}" />
            <asp:BoundField DataField="AmountPaid" HeaderText="Amount Paid" HeaderStyle-HorizontalAlign="Left"
                DataFormatString="{0:C}" />
            <asp:BoundField DataField="BalanceDue" HeaderText="Balance Due" HeaderStyle-HorizontalAlign="Left"
                DataFormatString="{0:C}" />
            <asp:HyperLinkField Text="(view)" DataNavigateUrlFormatString="/financial/ViewInvoice.aspx?contextID={0}"
                DataNavigateUrlFields="ID" />
        </Columns>
    </asp:GridView>
    <h2>
        <asp:Literal ID="lPayments" runat="server">Payments</asp:Literal></h2>
    <div style="padding-top: 10px">
        <asp:GridView ID="gvPayments" AutoGenerateColumns="false" EmptyDataText="No payments are tied to this order."
            GridLines="None" runat="server">
            <Columns>
                <asp:BoundField DataField="Payment.Date" HeaderText="Date" HeaderStyle-HorizontalAlign="Left"
                    DataFormatString="{0:d}" />
                <asp:BoundField DataField="Payment.Name" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="Amount" HeaderText="Total" HeaderStyle-HorizontalAlign="Left"
                    DataFormatString="{0:C}" />
                <asp:HyperLinkField Text="(view)" DataNavigateUrlFormatString="/financial/ViewPayment.aspx?contextID={0}"
                    DataNavigateUrlFields="Payment" />
            </Columns>
        </asp:GridView>
    </div>
    <asp:Panel ID="pnlInstallments" runat="server" Visible="false">
        <h2>
            <asp:Literal ID="lInstallments" runat="server">Installments</asp:Literal></h2>
        <table style="width: 200px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lInstallmentStatus" runat="server">Installment Status:</asp:Literal>
                </td>
                <td>
                    <%= GetSearchResult( targetOrder, "BillingSchedule.Status", null ) %>
                </td>
            </tr>
        </table>
        <asp:Literal ID="lBrokenOutIntoInstallments" runat="server">This order is broken out into installments. The billing schedule is listed below:</asp:Literal>
        <p />
        <asp:GridView ID="gvInstallments" AutoGenerateColumns="false" EmptyDataText="No billing schedule entries are tied to this order."
            GridLines="None" runat="server">
            <Columns>
                <asp:BoundField DataField="Date" HeaderText="Date" HeaderStyle-HorizontalAlign="Left"
                    DataFormatString="{0:d}" />
                <asp:BoundField DataField="Amount" HeaderText="Total" HeaderStyle-HorizontalAlign="Left"
                    DataFormatString="{0:C}" />
                <asp:BoundField DataField="Status" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" />
            </Columns>
        </asp:GridView>
        <div style="text-align: right">
            <asp:Button ID="btnUpdateCreditCardInfo" runat="server" OnClick="btnUpdateCreditCard_Click"
                Text="Update Credit Card Info for this Order" />
        </div>
    </asp:Panel>
    <hr />
    <div style="text-align: center">
        <asp:Button ID="Button1" runat="server" CausesValidation="false" Text="Account History"
            OnClick="btnAccountHistory_Click" />
        <asp:Button ID="btnGoHome" runat="server" CausesValidation="false" Text="Go Home"
            OnClick="btnGoHome_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
