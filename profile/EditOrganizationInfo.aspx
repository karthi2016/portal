<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EditOrganizationInfo.aspx.cs" Inherits="profile_EditOrganizationInfo" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="Server">
    Edit Organization Information
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <p><asp:Literal ID="PageText" runat="server">Please enter your contact information below.</asp:Literal>
    </p>
    <span class="requiredField">*</span> - indicates a required field. &nbsp;<h2>
        <ASP:Literal ID="lBasicInfo" runat="server">Basic Information</ASP:Literal></h2>
    <table>
        <tr>
            <td valign="top">
                <table style="width: 100%">
                    <tr>
                        <td class="columnHeader">
                            <ASP:Literal ID="lName" runat="server">Name:</ASP:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="tbName" runat="server" TabIndex="10" />
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top">
                <table id="tblLogo" runat="server">
                    <tr>
                        <td class="columnHeader">
                            <ASP:Literal ID="lLogo" runat="server">Logo</ASP:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Image ID="img" Width="120" Height="120" BorderWidth="1px" runat="server" ImageUrl="~/Images/noimage.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <ASP:Literal ID="lChangeLogo" runat="server">Change Logo</ASP:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:FileUpload ID="imageUpload" Width="300px" runat="server" /><br />
                            Images larger than 120x120 will be resized
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CustomValidator runat="server" ControlToValidate="imageUpload" ForeColor="Red"
                                OnServerValidate="imageValidate" ErrorMessage="The file specified is not a valid image." />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:PlaceHolder ID="phBillingContact" runat="server">
    <h2>
        <ASP:Literal ID="lBillingContact" runat="server">Billing Contact</ASP:Literal></h2>
    <table>
    <tr>
            <td class="columnHeader">
                <ASP:Literal ID="lContactName" runat="server">Name:</ASP:Literal>
                <span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbBillingContactName" runat="server" TabIndex="100" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter billing contact name."
                    ControlToValidate="tbBillingContactName" Display="None" />
            </td>
        </tr>
          <tr>
            <td class="columnHeader">
                <ASP:Literal ID="lBillingPhoneNumber" runat="server">Phone Number:</ASP:Literal>
                <span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbBillingContactPhoneNumber" runat="server" TabIndex="100" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please enter billing contact phone number."
                    ControlToValidate="tbBillingContactPhoneNumber" Display="None" />
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <ASP:Literal ID="lBillingEmailAddress" runat="server">Email Address:</ASP:Literal>
                <span class="requiredField">*</span>
            </td>
            <td>
                <asp:TextBox ID="tbEmail" runat="server" TabIndex="100" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Please enter an email address."
                    ControlToValidate="tbEmail" Display="None" />
            </td>
        </tr>
    </table></asp:PlaceHolder>
    <asp:PlaceHolder ID="phPhoneNumbers" Visible="false" runat="server">
        <h2>
            <ASP:Literal ID="lPhoneNumbers" runat="server">Phone Numbers</ASP:Literal></h2>
        <ASP:Literal ID="lAtLeastOnePhone" runat="server">Please enter your contact numbers below. Use the <b>Preferred?</b> radio
        button to indicate the phone number at which you prefer to be contacted.</ASP:Literal>
        <asp:GridView ID="gvPhoneNumbers" OnRowDataBound="gvPhoneNumbers_OnRowDataBound"
            GridLines="None" AutoGenerateColumns="false" Width="460px" runat="server">
            <Columns>
                <asp:TemplateField HeaderStyle-BackColor="White">
                    <ItemTemplate>
                        <asp:Label ID="lblPhoneNumberType" runat="server" />
                        Phone Number:
                        <asp:Literal ID="lPhoneNumberRequired" runat="server" Visible="false"><span class="requiredField">*</span></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-BackColor="White">
                    <ItemTemplate>
                        <asp:TextBox ID="tbPhoneNumber" runat="server" /><asp:RequiredFieldValidator ID="rfvPhoneNumber" Enabled="false" Display="None" ControlToValidate="tbPhoneNumber"
                            runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                    HeaderText="Preferred?" HeaderStyle-CssClass="columnHeader" HeaderStyle-BackColor="White">
                    <ItemTemplate>
                        <!-- We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                    in repeater controls

                    http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->
                        <asp:Literal ID="lRadioButtonMarkup" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phAddresses" runat="server">
        <h2>
            <ASP:Literal ID="lAddressInfo" runat="server">Address Information</ASP:Literal></h2>
        <ASP:Literal ID="lAtLeastOneAddress" runat="server">Please enter your addresses below.</ASP:Literal>
        <div>
            <table cellpadding="0" cellspacing="0" style="margin-top: 10px;">
                <tr>
                    <asp:Repeater ID="rptAddresses" OnItemCreated="rptAddresses_OnItemCreated" OnItemDataBound="rptAddresses_OnItemDataBound"
                        runat="server">
                        <ItemTemplate>
                            <td>
                                <b><u>
                                    <asp:Label ID="lblAddressType" runat="server"></asp:Label>
                                    Address</u></b>
                                <br />
                                <asp:HiddenField ID="hfAddressCode" runat="server" />
                                <cc1:AddressControl ID="acAddress" runat="server" EnableValidation="True" />
                            </td>
                            <asp:Literal ID="lNewRowTag" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
                <asp:Literal ID="lAddressEndRow" runat="server" />
            </table>
        </div>
        <div style="padding-top: 10px">
            <h3>
                <ASP:Literal ID="lAddressPreferences" runat="server">Address Preferences</ASP:Literal></h3>
        </div>
        <table style="width: 650px">
            <tr>
                <td>
                    <ASP:Literal ID="lPreferredMailing" runat="server">What is your preferred mailing address?</ASP:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="ddlPreferredAddress" runat="server" />
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phOtherInformation" runat="server">
        <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" />
    </asp:PlaceHolder>
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
