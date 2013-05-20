<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="EditOrderLineItem.aspx.cs" Inherits="orders_EditOrderLineItem" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
  Edit Order Item Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<asp:Literal ID="PageText" runat="server" />
  <div class="sectHeaderTitle">
            <h2>
                <%= MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductName  %></h2>
        </div>
           <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" />

          <p />
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" 
            runat="server" Text="Continue" onclick="btnContinue_Click" />
            or <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" 
            onclick="lbCancel_Click" />
        
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

