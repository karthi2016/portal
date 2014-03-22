<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="AddSavedPaymentMethod.aspx.cs" Inherits="financial_AddSavedPaymentMethod" %>
<%@ Register TagPrefix="bi" TagName="BillingInfo" Src="~/controls/BillingInfo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
    Update Payment Method - <%=targetObject.Name %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
     <asp:Literal ID="PageText" runat="server" >
         
         You can update the payment method by using one of the options below:
     </asp:Literal>
     
     <asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true"
        Font-Bold="true" ForeColor="red" ShowMessageBox="false" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    
    <bi:BillingInfo ID="BillingInfoWidget" runat="server" />
    <hr style="width: 100%" />
  
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" runat="server" Text="Save Changes" OnClick="btnContinue_Click" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel the Update" CausesValidation="false"
            OnClick="lbCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

