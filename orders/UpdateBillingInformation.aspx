<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="UpdateBillingInformation.aspx.cs" Inherits="orders_UpdateBillingInformation" %>

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
    Update Electronic Payment Information for <%=targetObject["Name"] %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
     <asp:ValidationSummary ID="vsSummary2" ForeColor="Red" Font-Bold="true" DisplayMode="BulletList"  
        ShowSummary="true" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    <br />
     
     
       <bi:BillingInfo ID="BillingInfoWidget" runat="server" />
    <hr />
    <div class="sectionContent">
        <div align="center" style="padding-top: 20px">
            <asp:Button ID="btnUpdatePaymentInfo" Text="Update Payment Info" runat="server" OnClick="btnUpdatePaymentInfo_Click" />
            ,
            <asp:LinkButton ID="lbClearPaymentInf" runat="server" OnClick="lbClearPaymentInfo_Click" Text="Delete Billing Info"/>,
            or 
            <asp:HyperLink ID="hlCancel" runat="server" Text="Cancel this Operation"/>
            
            
            <div class="clearBothNoSPC">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
