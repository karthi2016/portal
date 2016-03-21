<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ManagePaymentOptions.aspx.cs" Inherits="financial_ManagePaymentOptions" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Manage Payment Options
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server" >
        
        You can save credit cards &amp; checking accounts to use for future payments. Keep in mind that even though you
        can enter a checking account, eChecks may not be allowed for certain transactions on this site.
        <br />
    </asp:Literal>
    
    <telerik:RadGrid BorderWidth="0px" EnableAjax="true" HeaderStyle-Width="100px" Width="100%"
        ID="rgMainDataGrid" runat="server" GridLines="None" OnNeedDataSource="rgMainDataGrid_NeedDataSource"
        AutoGenerateColumns="false" SelectedItemStyle-CssClass="rgSelectedRow" OnItemDataBound="rgMainDataGrid_ItemDataBound"
         OnItemCommand="rgMainDataGrid_ItemCommand"
        >
        <HeaderStyle Width="100px"></HeaderStyle>
        <MasterTableView DataKeyNames="ID">
            <Columns>
                <telerik:GridBoundColumn DataField="Name" HeaderText="Payment Option" ItemStyle-Width="750px" />
                <%--<telerik:GridHyperLinkColumn Text="(edit)" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="EditSavedPayment.aspx?contextID={0}"/>--%>
                <telerik:GridButtonColumn UniqueName="DeleteButton" Text="(delete)" ButtonType="LinkButton" CommandName="Delete" ItemStyle-Width="20px" ConfirmText="Are you sure you want to delete this payment option?" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <asp:Literal ID="lNoSavedOptions" runat="server">
        
        You currently have no saved credit cards or eCheck accounts. You can add cards or checking accounts below
        to speed up checkouts in future orders.
    </asp:Literal>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li id="liCreditCard" runat="server">
                    <asp:HyperLink ID="hlAddCreditCard" runat="server" NavigateUrl="AddCreditCard.aspx">Add a Credit Card</asp:HyperLink>
                </li>
                <li id="liElectronicCheck" runat="server">
                    <asp:HyperLink ID="hlAddElectronicCheck" runat="server" NavigateUrl="AddElectronicCheck.aspx">Add an eCheck Account</asp:HyperLink>
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
