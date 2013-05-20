<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="RectifySuspendedBillingSchedules.aspx.cs" Inherits="financial_RectifySuspendedBillingSchedules" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
Rectify Declined Credit Card &amp; Suspended Billing
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<div style="padding-bottom: 10px">
<asp:Literal ID="PageText" runat="server" >
The orders below have been suspeded due to a declinded credit card. Click to update payment information
and resume billing.</asp:Literal>
 </div>
  <asp:GridView ID="gvTransactions" AutoGenerateColumns="false" EmptyDataText="No suspended orders found." GridLines="None" runat="server">
    <Columns>
   
    <asp:BoundField DataField="Order.Date" DataFormatString="{0:d}" HeaderText="Date"  HeaderStyle-HorizontalAlign="Left" />
    <asp:BoundField DataField="Order.Name" HeaderText="Name"  HeaderStyle-HorizontalAlign="Left"  />
    <asp:BoundField DataField="Order.Memo" HeaderText="Memo"  HeaderStyle-HorizontalAlign="Left"  />
    <asp:BoundField DataField="Order.Total" DataFormatString="{0:C}" HeaderText="Total"  HeaderStyle-HorizontalAlign="Left"    />
    <asp:HyperLinkField Text="(update payment info)" DataNavigateUrlFormatString="RectifySuspendedBillingSchedule.aspx?contextID={0}" DataNavigateUrlFields="ID" />
    </Columns>
    </asp:GridView>
<hr />
  <div class="sectionContent">
            <div align="center" style="padding-top: 20px">
                
                <asp:Button ID="btnCancel" OnClick="btnGoHome_Click" Text="Go Home" runat="server" />
                <div class="clearBothNoSPC">
                </div>
            </div>
        </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

