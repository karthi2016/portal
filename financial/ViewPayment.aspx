<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewPayment.aspx.cs" Inherits="financial_ViewPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>View Payment #
        <%=GetSearchResult( targetPayment, "LocalID",null ) %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="AccountHistory.aspx">Account History</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Payment #
    <%=GetSearchResult( targetPayment, "LocalID",null ) %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div style="float: right">
        <table style="width: 200px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lPayment" runat="server" >Payment #</asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult( targetPayment, "LocalID",null ) %>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lDate" runat="server" >Date</asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult(targetPayment, "Date", "d")%>
                </td>
            </tr>
        </table>
    </div>
    <div style="float: left">
        <table style="width: 500px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lOwner" runat="server" ><u>Owner: </u></asp:Literal>
                </td>
                <td>
                    <%=GetSearchResult(targetPayment, "Owner.Name", null)%>
                </td>
            </tr>
        </table>
    </div>
    <div style="min-height: 25px">
    </div>
    <h2>
        <asp:Literal ID="lSummaryInfo" runat="server" >Summary Information</asp:Literal></h2>
    <table style="width: 600px">
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lPaymentTotal" runat="server" >Payment Total:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetPayment, "Total", "C")%>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lAmountRefunded" runat="server" >Amount Refunded:</asp:Literal>
            </td>
            <td>
                <%=GetSearchResult(targetPayment, "AmountRefunded", "C")%>
            </td>
        </tr>
    </table>
    <h2>
        <asp:Literal ID="lLineItems" runat="server" >Line Items</asp:Literal></h2>
    <asp:GridView ID="gvPaymentItems" AutoGenerateColumns="false" EmptyDataText="No items in this Payment."
        GridLines="None" runat="server" 
        onrowdatabound="gvPaymentItems_RowDataBound">
        <Columns>
           
            <asp:BoundField DataField="TransactionName" HeaderText="Description" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" HeaderStyle-HorizontalAlign="Left" />
            <asp:TemplateField >
            <ItemTemplate>
            <asp:HyperLink ID="hlView" Text="(view)" runat="server" />
            </ItemTemplate>


            </asp:TemplateField>
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
