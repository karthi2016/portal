<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ManageInstallmentPlans.aspx.cs" Inherits="financial_ManageInstallmentPlans" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
    Manage Installment Plans
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
    
    
     <asp:Literal ID="lPageText" runat="server" >
        Below are a list of installment plans set up for your account that require recurring billing. You can view these plans 
        and/or update the billing information.
        <br />
        <br />
    </asp:Literal>
    
    <telerik:RadGrid BorderWidth="0px" EnableAjax="true"   Width="100%"
        ID="rgMainDataGrid" runat="server" GridLines="None" OnNeedDataSource="rgMainDataGrid_NeedDataSource"
        AutoGenerateColumns="false" SelectedItemStyle-CssClass="rgSelectedRow" 
        
        >
        
        <MasterTableView DataKeyNames="ID">
            <Columns>
                <telerik:GridBoundColumn DataField="Order.Name" HeaderText="Order"   />
                <telerik:GridBoundColumn DataField="Product.Name" HeaderText="Item"   />
                <telerik:GridBoundColumn DataField="PastBillingAmount" HeaderText="Amount Already Billed" DataFormatString="{0:C}"   />
                <telerik:GridBoundColumn DataField="FutureBillingAmount" HeaderText="Amount to be Billed" DataFormatString="{0:C}"   />
                <telerik:GridHyperLinkColumn Text="(view)" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="ViewInstallmentPlan.aspx?contextID={0}" ItemStyle-Width="90px"/>
                <telerik:GridHyperLinkColumn Text="(update billing)" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="RectifySuspendedBillingSchedule.aspx?contextID={0}" ItemStyle-Width="90px"/>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <asp:Literal ID="lNoIntallmentPlans" runat="server">
       <i>There are no installment plans linked to your account.</i> 
    </asp:Literal>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                   <li>
                            <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                        </li>
            </ul>
        </div>
    </div>
    

</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

