<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="MakePayment2.aspx.cs" Inherits="financial_MakePayment2" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="bi" TagName="BillingInfo" Src="~/controls/BillingInfo.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Enter Payment Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <span style="font-family: Arial, Helvetica, sans-serif; font-size: 16px; font-weight: bold">
    <ASP:Literal ID="lAmountDue" runat="server">Amount Due:</ASP:Literal> <ASP:Label ID="lblAmountDue" ForeColor=Green runat="server" Text="$0.00"/>
    </span>
    <div class="sectHeaderTitle" style="margin-top: 20px">
        <h2>
            <asp:Literal ID="lShippingInfo" runat="Server">How would you like to pay?</asp:Literal></h2>
    </div>
    <asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true"
        Font-Bold="true" ForeColor="red" ShowMessageBox="false" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    <asp:Literal ID="PageText" runat="server">
    </asp:Literal>
    <asp:Literal ID="lMessages" runat="server" />
    <bi:BillingInfo ID="BillingInfoWidget" runat="server" />
    <hr style="width: 100%" />
    
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" runat="server" Text="Process Payment" OnClick="btnContinue_Click" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel This Payment" OnClick="lbCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
