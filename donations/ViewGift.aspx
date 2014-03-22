<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewGift.aspx.cs" Inherits="donations_ViewGift" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Gift -
    <%=targetGift.Name  %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <table>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lName" runat="Server">Type:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult( dr, "Type") %>
            </td>
            <td class="columnHeader">
                <asp:Literal ID="lMasterGift" runat="Server">Master Gift:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblNoMasterGift" runat="server">No master gift</asp:Label>
                <asp:PlaceHolder ID="phMasterGift" runat="server" Visible="false">
                <a href="ViewGift.aspx?contextID=<%= GetSearchResult( dr, "MasterGift.ID") %>">
                    <%= GetSearchResult( dr, "MasterGift.Name")  %></a>
                    </asp:PlaceHolder>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="Literal1" runat="Server">Date:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult( dr, "Date", "D") %>
            </td>
            <td class="columnHeader">
                <asp:Literal ID="Literal2" runat="Server">Amount:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult( dr, "Amount", "C") %>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="Literal3" runat="Server">Fund:</asp:Literal>
            </td>
            <td>
                <%= GetSearchResult( dr, "Fund.Name") %>
            </td>
            <td class="columnHeader">
              
            </td>
            <td>
                
            </td>
        </tr>
       
    </table>
    <asp:Panel ID="pnlRecurring" runat="server">
        <h2>
            Recurring Information</h2>
        This is an open, recurring gift.
        <table style="margin-top: 10px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="Literal8" runat="Server">Saved Payment Method:</asp:Literal>
                </td>
                <td>
                    <%= GetSearchResult(dr, "SavedPaymentMethod.Name") ?? "No payment method is associated with this gift."%>
                     <asp:HyperLink ID="hlUpdatePaymentMethod2" runat="server" >(edit)</asp:HyperLink>
              
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="Literal5" runat="Server">Next Installment Due:</asp:Literal>
                </td>
                <td>
                    <%= GetSearchResult(dr, "NextTransactionDue","D")%>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="Literal6" runat="Server">Next Installment Amount:</asp:Literal>
                </td>
                <td>
                    <%= GetSearchResult(dr, "NextTransactionAmount", "C")%>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="Literal7" runat="Server">Balance Due:</asp:Literal>
                </td>
                <td>
                    <%= GetSearchResult(dr, "BalanceDue", "C")%>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lStatus" runat="Server">Status:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblStatus" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlInstallmentHistory" runat="server" Style="margin-top: 10px">
        <h2>
            Installment History</h2>
        <telerik:RadGrid BorderWidth="0px" EnableAjax="true" Width="100%" ID="rgInstallments"
            runat="server" GridLines="None" OnNeedDataSource="rgInstallments_OnNeedDataSource"
            AutoGenerateColumns="false" SelectedItemStyle-CssClass="rgSelectedRow">
            <MasterTableView>
                <Columns>
                    <telerik:GridBoundColumn DataField="Date" HeaderText="Date" DataFormatString="{0:D}" />
                    <telerik:GridBoundColumn DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" />
                    <telerik:GridBoundColumn DataField="AmountPaid" HeaderText="Amount Paid" DataFormatString="{0:C}" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <asp:HyperLink ID="hlUpdatePaymentMethod" runat="server" ><li>Update Billing Information</li></asp:HyperLink>
                <li>
                    <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
