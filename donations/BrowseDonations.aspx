<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="BrowseDonations.aspx.cs" Inherits="events_BrowseDonations" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    My Giving History
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <h2>
            My Open Pledges &amp; Recurring Gifts</h2>
        <asp:Literal ID="PageText" runat="server" />
        <telerik:RadGrid BorderWidth="0px" EnableAjax="true" Width="100%" ID="rgOpenRecurringGifts"
            runat="server" GridLines="None" OnNeedDataSource="rgOpenRecurringGifts_OnNeedDataSource"
            AutoGenerateColumns="false" SelectedItemStyle-CssClass="rgSelectedRow"  >
            <MasterTableView DataKeyNames="ID">
                <Columns>
                    <telerik:GridBoundColumn DataField="LocalID" HeaderText="Gift #" />
                    <telerik:GridBoundColumn DataField="Total" HeaderText="Total Amount" DataFormatString="{0:C}"/>
                    <telerik:GridBoundColumn DataField="NextTransactionAmount" HeaderText="Next Amount Due" DataFormatString="{0:C}" />
                    <telerik:GridBoundColumn DataField="NextTransactionDue" HeaderText="Next Due Date" DataFormatString="{0:D}"/>
           
                    <telerik:GridHyperLinkColumn DataNavigateUrlFields="ID" DataNavigateUrlFormatString="ViewGift.aspx?contextID={0}" Text="(view)" ItemStyle-Width="90px" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <asp:Literal ID="lNoOpenPledges" runat="server">
       <i>You have no open pledges or recurring gifts.</i> 
        </asp:Literal>
    </div>
    
     <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><asp:Literal ID="lDonations" runat="server">My Complete Giving History</asp:Literal></h2>
        </div>
        <asp:Literal ID="Literal1" runat="server"/>
        <div class="sectionContent">
             <telerik:RadGrid BorderWidth="0px" EnableAjax="true" Width="100%" ID="rgAllGifts"
            runat="server" GridLines="None" OnNeedDataSource="rgAllGifts_OnNeedDataSource"
            AutoGenerateColumns="false" SelectedItemStyle-CssClass="rgSelectedRow"  >
            <MasterTableView DataKeyNames="ID">
                <Columns>
                    <telerik:GridBoundColumn DataField="LocalID" HeaderText="Gift #" />
                    <telerik:GridBoundColumn DataField="Total" HeaderText="Total Amount" DataFormatString="{0:C}"/>
                    <telerik:GridBoundColumn DataField="Fund.Name" HeaderText="Fund" />
           
                    <telerik:GridHyperLinkColumn DataNavigateUrlFields="ID" DataNavigateUrlFormatString="ViewGift.aspx?contextID={0}" Text="(view)" ItemStyle-Width="90px" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
          <asp:Literal ID="lNoGifts" runat="server">
       <i>You have no previous gifts.</i> 
        </asp:Literal>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
