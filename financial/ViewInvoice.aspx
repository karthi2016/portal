<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewInvoice.aspx.cs" Inherits="financial_ViewInvoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>View Invoice #
        <%=GetSearchResult( targetInvoice, "LocalID",null ) %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="AccountHistory.aspx">Account History</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Invoice #
    <%=GetSearchResult( targetInvoice, "LocalID",null ) %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div style="float: right">
        <table style="width: 200px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lInvoice" runat="server" >Invoice #</asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult( targetInvoice, "LocalID",null ) %>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lDate" runat="server" >Date</asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult(targetInvoice, "Date", "d")%>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lOrder" runat="server" >Order:</asp:Literal>
                </td>
                <td>
                    <a href='ViewOrder.aspx?contextID=<%=GetSearchResult(targetInvoice, "Order", null)%>'>
                        <%=GetSearchResult(targetInvoice, "Order.Name", null)%></a>
                </td>
            </tr>
           
        </table>
    </div>
    <div style="float: left">
        <table style="width: 500px">
            <tr>
                <td class="columnHeader">
                   <asp:Literal ID="lBillTo" runat="server" > <u>Bill To: </u></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <%=GetSearchResult(targetInvoice, "BillTo.Name", null)%>
                    <br />
                    <%=GetSearchResult(targetInvoice, "BillingAddress", null)%>
                </td>
            </tr>
        </table>
    </div>
    <div style="min-height: 95px">
    </div>
    <h2>
        <asp:Literal ID="lSummaryInformation" runat="server" >Summary Information</asp:Literal></h2>
    <table style="width: 600px">
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lInvoiceTotal" runat="server" >Invoice Total:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetInvoice, "Total", "C")%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lAmountPaid" runat="server" >Amount Paid:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetInvoice, "AmountPaid", "C")%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lBalanceDue" runat="server" >Balance Due:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetInvoice, "BalanceDue", "C")%>
            </td>
        </tr>
    </table>
    <h2>
        <asp:Literal ID="lLineItems" runat="server" >Line Items</asp:Literal></h2>
    
        <asp:GridView ID="gvInvoiceItems" AutoGenerateColumns="false" EmptyDataText="No items in this Invoice."
            GridLines="None" runat="server">
            <Columns>
                <asp:BoundField DataField="Product.Name" HeaderText="Product" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="Description" HeaderText="Desc" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="Quantity" HeaderText="Quantity" DataFormatString="{0:C}"
                    HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:C}"
                    HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" HeaderStyle-HorizontalAlign="Left" />
            </Columns>
        </asp:GridView>
    
    <h2>
        <asp:Literal ID="lPayments" runat="server" >Payments</asp:Literal></h2>
     
        <asp:GridView ID="gvPayments" AutoGenerateColumns="false" EmptyDataText="No payments are tied to this Invoice."
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
