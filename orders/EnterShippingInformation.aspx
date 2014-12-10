<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EnterShippingInformation.aspx.cs" Inherits="orders_EnterShippingInformation" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Enter Shipping Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" id="divShipping" runat="server">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lShippingInfo" runat="Server">Shipping Information</asp:Literal></h2>
        </div>
        <asp:Literal ID="PageText" runat="server">
        One or more items in your order require shipping. Please select your shipping method
        and address.</asp:Literal>
          <asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true"
        Font-Bold="true" ForeColor="red" ShowMessageBox="false" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
        <ASP:CustomValidator ID="cvInvalidAddress" runat="server" OnServerValidate="cvInvalidAddress_OnServerValidate" ErrorMessage="The address you have selected is invalid - a line 1, city, state, and postal code are required. Please select another address or enter in a new address."
                             Display="None"/>
        <ASP:CustomValidator ID="cvWrongShippingMethod" runat="server" OnServerValidate="cvWrongShippingMethod_OnServerValidate"
                             Display="None"/>
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
                    <td style="width:500px">
                              
                            <div id="billingAddresses">
                                <table style="margin-top: 20px;" cellpadding="5">
                                    <asp:Repeater ID="rptBillingAddress" runat="server" OnItemDataBound="rptBillingAddress_OnItemDataBound">
                                        <ItemTemplate>
                                            <tr style="vertical-align: top; margin-top: 20px">
                                                <td style="width: 30px">
                                                    <asp:RadioButton ID="rbAddress" runat="server" onchange="updateBillingAddress();" />
                                                </td>
                                                <td>
                                                    <asp:Literal ID="lAddress" runat="server" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <tr style="vertical-align: top; margin-top: 20px">
                                        <td style="width: 30px">
                                            <asp:RadioButton ID="rbNewBillingAddress" GroupName="BillingAddress" onchange="updateBillingAddress();"
                                                runat="server" />
                                        </td>
                                        <td>
                                            Enter a new address:
                                            <div id="newBillingAddress" runat="server" style="display: none">
                                                <cc1:AddressControl ID="acBillingAddress" IsRequired="false" EnableValidation="False"
                                                    runat="server" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblShipping" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <script>
        function updateBillingAddress() {

            var rbNewBillingAddress = document.getElementById('<%= rbNewBillingAddress.ClientID %>');
            var newBillingAddress = document.getElementById('<%= newBillingAddress.ClientID %>');

            if (newBillingAddress == null) return;  // it's not shown, nothing to do

            newBillingAddress.style.display = 'none';

            if (rbNewBillingAddress.checked)
                newBillingAddress.style.display = '';



        }
        jQuery(document).ready(function ($) {
            var rbNewBillingAddress = document.getElementById('<%= rbNewBillingAddress.ClientID %>');
            $("#billingAddresses input:radio").attr("name", rbNewBillingAddress.name);

            updateBillingAddress();
        });
    </script>
    <p />
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" runat="server" Text="Continue" OnClick="btnContinue_Click" />
        or
        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel Your Order" OnClick="lbCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
