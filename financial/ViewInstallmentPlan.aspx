<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewInstallmentPlan.aspx.cs" Inherits="financial_ViewInstallmentPlan" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ManageInstallmentPlans.aspx">Manage Installment Plans</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Installment Plan
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server" />
    <table style="width: 750px; padding-bottom: 20px">
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lOrderNum" runat="server">Order #</asp:Literal>
            </td>
            <td>
                <asp:HyperLink ID="hlOrderNo" runat="server" NavigateUrl="/financial/ViewOrder.aspx?contextID="
                    Text="21032" />
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lDate" runat="server">Date</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblOrderDate" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lAmount" runat="server">Amount Remaining to be Billed:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblAmountRemaining" runat="server" />
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lOrderTotal" runat="server">Order Total:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblOrderTotal" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lAmountPastBilled" runat="server">Amount Already  Billed:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblAmountAlreadyBilled" runat="server" />
            </td>
            <td class="columnHeader">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lStatus" runat="server">Status:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblStatus" runat="server" />
            </td>
            <td class="columnHeader">
            </td>
            <td>
            </td>
        </tr>
    </table>
      <div style="margin-top: 10px" id="divBillingInformation" runat="server"  >
        
            <h2>
                <asp:Literal ID="lBillingInfo" runat="server">Billing Information</asp:Literal></h2>
        
        Your credit card billing information is below.
        <asp:HyperLink ID="hlUpdateBillingInfo2" runat="server" Text="Click here to update."></asp:HyperLink>
        <table style="width: 100%; margin-top: 10px">
            <tr id="tr1" runat="server">
                <td class="columnHeader">
                    <asp:Literal ID="lPaymentMethod" runat="server">Payment Method:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblPaymentInfo" runat="server">You currently have no payment information associated with this billing schedule.</asp:Label>
                </td>
                <td class="columnHeader">
                </td>
                <td>
                </td>
            </tr>
             
        </table>
    </div>
    <h2>
        Billing Schedule</h2>
        <asp:Literal ID="lBSchedule" runat="server">Below are a list of all past/future installments and amounts for
        this billing schedule. If an invoice/payment has been generated, then are available for viewing.</asp:Literal>
    <telerik:RadGrid BorderWidth="0px" EnableAjax="true" Width="100%" ID="rgBillingHistory"
        runat="server" GridLines="None" OnNeedDataSource="rgBillingHistory_NeedDataSource"
        AutoGenerateColumns="false" SelectedItemStyle-CssClass="rgSelectedRow">
        <MasterTableView >
            <Columns>
                <telerik:GridBoundColumn DataField="Date" HeaderText="Date" DataFormatString="{0:d}" />
                <telerik:GridBoundColumn DataField="Amount" HeaderText="Amount"   DataFormatString="{0:C}" />
                <telerik:GridBoundColumn DataField="Status" HeaderText="Status"    />
                <telerik:GridBoundColumn DataField="Message" HeaderText="Message"    />
                
                <telerik:GridHyperLinkColumn DataNavigateUrlFields="Invoice" DataNavigateUrlFormatString="ViewInvoice.aspx?contextID={0}"
                 DataTextField="Invoice.Name" HeaderText="Invoice"/>
                     <telerik:GridHyperLinkColumn DataNavigateUrlFields="Payment" DataNavigateUrlFormatString="ViewPayment.aspx?contextID={0}"
                 DataTextField="Payment.Name" HeaderText="Payment"/>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <asp:Literal ID="lNoHistory" runat="server">There is no past/future billing history for this installment plan.</asp:Literal>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li>
                    <asp:HyperLink ID="hlUpdateBilling" runat="server" NavigateUrl="~/financial/RectifySuspendedBillingSchedule.aspx">Update Billing Info</asp:HyperLink>
                </li>
                <li>
                    <asp:HyperLink ID="hlManagePlans" runat="server" NavigateUrl="~/financial/ManageInstallmentPlans.aspx">View My Open Installment Plans</asp:HyperLink>
                </li>
                <li>
                    <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
