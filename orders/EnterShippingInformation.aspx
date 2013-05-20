<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="EnterShippingInformation.aspx.cs" Inherits="orders_EnterShippingInformation" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Enter Shipping Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">

 <div class="section" id="divShipping" runat="server">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lShippingInfo" runat="Server">Shipping Information</asp:Literal></h2>
        </div>
        <asp:Literal ID="PageText" runat="server">
        One or more items in your order require shipping. Please select your shipping method
        and address.</asp:Literal>
        <div style="margin-top: 20px">
            <table style="width: 100%">
                <tr>
                    <td style="width: 30px">
                        <h3>
                            <asp:Literal ID="lShippingAddress" runat="server">Shipping Address</asp:Literal></h3>
                    </td>
                    <td>
                        <h3>
                            <asp:Literal ID="lSelectAShippingMethod" runat="server">Select a Shipping Method</asp:Literal></h3>
                    </td>
                </tr>
                <tr style="vertical-align: top">
                    <td>
                        <cc1:AddressControl ID="acShipping" IsRequired="true" runat="server" />
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblShipping" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
     <p />
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" 
            runat="server" Text="Continue" onclick="btnContinue_Click" />
            or <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel Your Order" 
            onclick="lbCancel_Click" />
        
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

