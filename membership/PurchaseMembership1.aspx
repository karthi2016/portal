<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/DataPage.master" AutoEventWireup="true"
    CodeFile="PurchaseMembership1.aspx.cs" Inherits="membership_PurchaseMembership1" %>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Join/Renew Membership - Select Membership Type
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Steps" runat="Server">
    <ul>
        <li class="current">1. Select Membership Type</li>
        <li>2. Enter Membership Information</li>
        <li>3. Confirm Information</li>
        <li>4. Complete</li>
    </ul>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Tips" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DataEntry" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="sectContLrg">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lSelectMemTypeFee" runat="server">Select Your Membership Type/Fee:</asp:Literal></h2>
        </div>
        <div class="subHeading">
        </div>
        <p>
            <asp:Literal ID="lBelowIsAListOfTypes" runat="server">Below is a list of the membership types available to you. Select the one that you
            would like to order.</asp:Literal></p>
        <asp:Repeater ID="rptMembershipTypes" OnItemDataBound="rptMembershipTypes_OnItemDataBound" runat="server">
            <ItemTemplate>
                <table style="width: 100%; margin-bottom: 20px">
                    <tr class="productListHeader">
                        <td><h3> 
                            <asp:Literal ID="lRadioButtonMarkup" runat="server" ></asp:Literal>
                            <asp:Label ID="lblMembershipType" runat="server"   /></h3>
                            <asp:HiddenField id="hfMembershipType" runat="server" />
                        </td>
                        <td class="price" style="text-align: right">
                            <asp:Label ID="lblPrice" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-top: 10px" colspan="2">
                            <asp:Literal ID="lMembershipDescription" runat="server" />
                        </td>
                    </tr>
                    <tr id="trSubCategory" runat="server" visible="false" >
                        <td style="padding-top: 20px" colspan="2">
                        <div style="padding-left: 10px">
                           <u>Please select a sub-category:</u>
                            <br />
                            <asp:RadioButtonList ID="rblSubCategory" runat="server" />
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <p />
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" OnClick="btnSave_Click" runat="server" Text="Continue" />
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Cancel"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
