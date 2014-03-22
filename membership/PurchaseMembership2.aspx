<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/DataPage.master" AutoEventWireup="true"
    CodeFile="PurchaseMembership2.aspx.cs" Inherits="membership_PurchaseMembership2" %>

<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Contexnt12" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .quantity
        {
            width: 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="Server">
    Join/Renew Membership - Enter Membership Information
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Steps" runat="Server">
    <asp:Literal ID="lSteps" runat="server">
    <ul>
        <li>1. Select Membership Type</li>
        <li class="current">2. Enter Membership Information</li>
        <li>3. Confirm Information</li>
        <li>4. Complete</li>
    </ul>
    </asp:Literal>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Tips" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DataEntry" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".quantity").NumericOnly();
        });
    </script>
    <asp:Literal ID="PageText" runat="server" />
    <div class="sectContLrg">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lYourMembershipType" runat="server">Your Membership Type/Fee:</asp:Literal></h2>
        </div>
        <table style="width: 100%">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lMembershipType" runat="server">Membership Type:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblMembershipType" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lMembershipFee" runat="server">Membership Fee:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblMembershipFee" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <div class="sectContLrg" id="divChapter" runat="server">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lChapterSelection" runat="server">Chapter Selection:</asp:Literal></h2>
        </div>
        <table style="width: 100%">
            <tr>
                <td>
                    <asp:Literal ID="lSelectYourPrimaryChapter" runat="server">Select your primary chapter: </asp:Literal><span
                        class="requiredField">*</span>
                </td>
                <td>
                    <asp:DropDownList Width="400px" ID="ddlSelectChapter" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlSelectChapter" ErrorMessage="Please select your primary chapter."
                        Display="None" />
                </td>
            </tr>
            <tr id="trChapterAssigned" runat="server" visible="false">
                <td colspan="2">
                    <asp:Literal ID="lPrimaryChapterAssigned" runat="server">
                        <span style="color:Green">Your primary chapter has been assigned based on your postal code and cannot be changed.</span>
                    </asp:Literal>
                </td>
            </tr>
        </table>
        <div id="divAdditionalChapters" runat="server">
            <div style="padding-top: 20px" class="subHeading">
                <asp:Literal ID="lAdditionalChapters" runat="server">Additional Chapters</asp:Literal></div>
            <asp:ListBox ID="lbAdditionalChapters" runat="server" Rows="10" Width="550px" SelectionMode="Multiple" />
        </div>
    </div>
    <div class="sectContLrg" id="divSections" runat="server">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lSections" runat="server">Sections:</asp:Literal></h2>
        </div>
        <asp:Repeater ID="rptSections" runat="server" OnItemDataBound="rptSections_OnItemDataBound">
            <ItemTemplate>
                <div style="padding-top: 20px" class="subHeading">
                    <asp:Label ID="lblSectionType" runat="server" />
                </div>
                <asp:CheckBoxList ID="cbSections" runat="server" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="sectContLrg" id="divOtherInformation" runat="server">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lOtherInformation" runat="server">Other Information:</asp:Literal>
            </h2>
        </div>
        <asp:CheckBox ID="cbMembershipDirectoryOptOut" runat="server" Text="I would like to opt out of the membership directory" /><br />
        <asp:CheckBox ID="cbAutomaticallyPay" Checked="true" runat="server" Text="Please charge my credit card and automatically renew my membership when it expires" />
        <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" />
    </div>
    <div class="sectContLrg" id="divOtherProducts" runat="server">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lAdditionalItems" runat="server">Additional Items</asp:Literal></h2>
        </div>
        <asp:Literal ID="lIfYouWouldLikeToAddAnyItems" runat="server">
        If you would like to add any of the items below to your membership, enter the quantities
        below.
        </asp:Literal>
      
      
       <table style="width: 100%; margin-top: 20px">
            <asp:Repeater ID="rptAdditionalItems" runat="server" OnItemDataBound="rptAdditionalItems_ItemDataBound">
                <ItemTemplate>
                    <tr>
                        <td style="width: 100px">
                            <asp:HiddenField ID="hfProductID" runat="server" />
                            <asp:TextBox ID="tbQuantity" runat="server" Text="0" Width="30" />
                            <asp:CompareValidator ID="cvQuantity" runat="server" ControlToValidate="tbQuantity"
                                Operator="GreaterThanEqual" ValueToCompare="0" ErrorMessage="One or more item quantities are invalid."
                                Type="Integer" Display="None" />
                        </td>
                        <td>
                            <asp:Label ID="lblProductName" runat="server" />
                        </td>
                        <td style="width: 50px">
                            <asp:Label ID="lblProductPrice" CssClass="price" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
      
    </div>
    <div class="sectContLrg" id="divDonations" runat="server">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lWouldYouLikeToMakeDonation" runat="server">Would you like to make a donation with your membership?</asp:Literal></h2>
        </div>
        <asp:Literal ID="lWeNeedYourHelp" runat="server">
        We need your help! If you would like, you can make a donation below along with your
        membership. Enter the amounts you would like to donate below.</asp:Literal>
        <table style="width: 100%; margin-top: 20px">
            <asp:Repeater ID="rptDonations" runat="server" OnItemDataBound="rptDonations_ItemDataBound">
                <ItemTemplate>
                    <tr>
                        <td style="width: 100px">
                            <asp:HiddenField ID="hfProductID" runat="server" />
                            <asp:TextBox ID="tbAmount" runat="server" Text="0" Width="60" />
                            <asp:CompareValidator ID="cvAmount" runat="server" ControlToValidate="tbAmount" Operator="GreaterThanEqual"
                                ValueToCompare="0" ErrorMessage="One or more dontation amounts are invalid."
                                Type="Currency" Display="None" />
                            <asp:CompareValidator ID="cvAmountType" runat="server" ControlToValidate="tbAmount"
                                Operator="DataTypeCheck" ErrorMessage="One or more dontation amounts are invalid."
                                Type="Currency" Display="None" />
                        </td>
                        <td>
                            <asp:Label ID="lblProductName" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    <p />
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" OnClick="btnSave_Click" runat="server" Text="Continue" />
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Cancel"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
