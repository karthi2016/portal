<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EditIndividualInfo.aspx.cs" Inherits="profile_EditIndividualInfo" %>

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
    Edit My Personal Information
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <p>
        <asp:Literal ID="PageText" runat="server">Please enter your contact information below.</asp:Literal>
    </p>
    <asp:CustomValidator ID="cvPhoneNumber" runat="server" Display="None" ValidationGroup="PhoneNumberAddress"
        ErrorMessage="Please enter at least one phone number." />
    <asp:CustomValidator ID="cvAddress" runat="server" Display="None" ValidationGroup="PhoneNumberAddress"
        ErrorMessage="Please enter at least one address." />
    <asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true"
        ValidationGroup="PhoneNumberAddress" ForeColor="Red" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    <span class="requiredField">*</span> - indicates a required field. &nbsp;<h2>
        <asp:Literal ID="lBasicInfo" runat="server">Basic Information</asp:Literal></h2>
    <table>
        <tr>
            <td>
                <table style="width: 100%">
                    <tr id="trTitle" runat="server">
                        <td class="columnHeader">
                            <asp:Literal ID="lTitle" runat="server">Title:</asp:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="tbTitle" runat="server" TabIndex="10" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lFirstName" runat="server">First Name:</asp:Literal>
                            <span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbFirstName" runat="server" TabIndex="20" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter a first name."
                                ControlToValidate="tbFirstName" Display="None" />
                        </td>
                    </tr>
                    <tr id="trMiddleName" runat="server">
                        <td class="columnHeader">
                            <asp:Literal ID="lMiddleName" runat="server">Middle Name:</asp:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="tbMiddleName" runat="server" TabIndex="30" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lLastName" runat="server">Last Name:</asp:Literal>
                            <span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbLastName" runat="server" TabIndex="40" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please enter a last name."
                                ControlToValidate="tbLastName" Display="None" />
                        </td>
                    </tr>
                    <tr id="trSuffix" runat="server">
                        <td class="columnHeader">
                            <asp:Literal ID="lSuffix" runat="server">Suffix:</asp:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="tbSuffix" runat="server" TabIndex="50" />
                        </td>
                    </tr>
                    <tr id="trNickName" runat="server">
                        <td class="columnHeader">
                            <asp:Literal ID="lNickname" runat="server">Nickname:</asp:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="tbNickName" runat="server" TabIndex="60" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lEmailAddress" runat="server">Email Address:</asp:Literal>
                            <span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbEmail" runat="server" TabIndex="100" />
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Please enter an email address."
                                ControlToValidate="tbEmail" Display="None" />
                        </td>
                    </tr>
                    <tr id="trEmailAddress2" runat="server">
                        <td class="columnHeader">
                            <asp:Literal ID="lEmailAddress2" runat="server">Email Address #2:</asp:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="tbEmail2" runat="server" TabIndex="110" />
                        </td>
                    </tr>
                    <tr id="trEmailAddress3" runat="server">
                        <td class="columnHeader">
                            <asp:Literal ID="lEmailAddress3" runat="server">Email Address #3:</asp:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="tbEmail3" runat="server" TabIndex="120" />
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top">
                <table>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lProfilePhoto" runat="server">Profile Photo</asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Image ID="img" Width="120" Height="120" BorderWidth="1px" runat="server" ImageUrl="~/Images/noimage.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lChangeProfilePhoto" runat="server">Change Profile Photo</asp:Literal>
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
    <asp:PlaceHolder ID="phPhoneNumbers" Visible="false" runat="server">
        <h2>
            <asp:Literal ID="lPhoneNumbers" runat="server">Phone Numbers</asp:Literal></h2>
        <asp:Literal ID="lAtLeastOneNumber" runat="server">Please enter at least one contact number below. Use the <b>Preferred?</b> radio
        button to indicate the phone number at which you prefer to be contacted.</asp:Literal>
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
                        <asp:TextBox ID="tbPhoneNumber" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvPhoneNumber" Enabled="false" Display="None" ControlToValidate="tbPhoneNumber"
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
            <asp:Literal ID="lAddressInfo" runat="server">Address Information</asp:Literal></h2>
        <asp:Literal ID="lAtLeastOneAddress" runat="server">Please enter at least one address below.</asp:Literal>
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
                                <cc1:AddressControl ID="acAddress" EnableValidation="false" runat="server" />
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
                <asp:Literal ID="lAddressPreferences" runat="server">Address Preferences</asp:Literal></h3>
        </div>
        <table style="width: 650px">
            <tr>
                <td>
                    <asp:Literal ID="lPreferredMailingAddress" runat="server">What is your preferred mailing address?</asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="ddlPreferredAddress" runat="server" />
                </td>
            </tr>
            <tr id="trSeasonality" runat="server" visible="false">
                <td style="padding-top: 10px">
                    Would you like to receive mail to your
                    <asp:Label ID="lblSeasonalAddressType" runat="server" />
                    address during certain times of the year?
                </td>
                <td style="width: 400px">
                    <asp:RadioButton ID="rbSeasonalYes" runat="server" Text="Yes" GroupName="Seasonal" />
                    - from
                    <cc1:MonthDayPicker ID="mdpSeasonalStart" runat="server" />
                    to
                    <cc1:MonthDayPicker ID="mdpSeasonalEnd" runat="server" />
                    <br />
                    <asp:RadioButton ID="rbSeasonalNo" runat="server" Text="No, that will not be necessary"
                        GroupName="Seasonal" />
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <h2>
            <asp:Literal ID="lCommunicationPrefs" runat="server">Communication Preferences</asp:Literal></h2>
        <div class="communicationPreferencesWrapper">
            <h3>
                <asp:Literal ID="lGeneralOptions" runat="server">General Communication Options</asp:Literal></h3>
            <p>
                <asp:Literal ID="lNoteIfYouSelect" runat="server">Note that if you select <b>Do Not Email?</b> you will not receive any email blasts,
                but you will still receive confirmation emails.</asp:Literal>
            </p>
            <table cellpadding="0" cellspacing="0" style="margin-top: 10px;">
                <tr id="trDoNotEmail" runat="server">
                    <td width="50px">
                        &nbsp;
                    </td>
                    <td class="columnHeader" width="100px">
                        <asp:Literal ID="lDoNotEmail" runat="server">Do Not Email?</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:CheckBox runat="server" ID="chkDoNotEmail" />
                    </td>
                </tr>
                <tr id="trDoNotMail" runat="server">
                    <td width="50px">
                        &nbsp;
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lDoNotMail" runat="server">Do Not Mail?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkDoNotMail" />
                    </td>
                </tr>
                <tr id="trDoNotFax" runat="server">
                    <td width="50px">
                        &nbsp;
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lDoNotFax" runat="server">Do Not Fax?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkDoNotFax" />
                    </td>
                </tr>
            </table>
            <p>
            </p>
            <h3>
                <asp:Literal ID="lMessageCategories" runat="server">Message Categories</asp:Literal></h3>
            <p>
                <asp:Literal ID="lBelowYouCan" runat="server">Below you can opt out of certain "categories" of communication, allowing you to
                control what kinds of emails you get. When an email blast is sent, if you have chosen
                to opt out of the category you will be automatically excluded from the blast. Below
                are the message categories for which you will receive messages.</asp:Literal>
            </p>
            <cc1:DualListBox runat="server" ID="dlbMessageCategories" LeftLabel="You are opted IN to these lists"
                RightLabel="You are opted OUT of these lists" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phOtherInformation" runat="server">
        <div class="customFieldsWrapper">
        <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" />
        </div>
    </asp:PlaceHolder>
    <div align="center" class="bottomButtonContainer">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
