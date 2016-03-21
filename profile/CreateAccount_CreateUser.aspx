<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="CreateAccount_CreateUser.aspx.cs" Inherits="profile_CreateAccount_Complete" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .noWrap {
            white-space: nowrap;
        }
        .padded {
            margin: 10px;
        }
        .leftPadded {
            margin-left: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Label ID="lblTitle" runat="server">Create Account</asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <script type="text/javascript">
        var onNextClick = (function () {
            var nextClicked = false;
            return function () {
                if (!Page_ClientValidate() || nextClicked)
                    return false;

                nextClicked = true;
                return true;
            };
        })();

        $(document).ready(function () {
            $("#rbNotListed").change(function () {
                enableOrgSelector(false);
                enableNotListed(true);
            });

            $("#rbNoAffiliation").change(function () {
                enableOrgSelector(false);
                enableNotListed(false);
            });

            $("#rbHaveOrg").change(function () {
                enableOrgSelector(true);
                enableNotListed(false);
            });

            enableOrgSelector(false);
            enableNotListed(false);

            // Initialize the Organization Selector validators based on default selection
            if ($("#rbHaveOrg").is(':checked')) {
                enableOrgSelector(true);
            } else if ($("#rbNotListed").is(':checked')) {
                enableNotListed(true);
            }
        });

        function enableOrgSelector(enabled) {
            if (enabled) {
                $("#HaveOrgSection .padded").show();
            } else {
                $("#HaveOrgSection .padded").hide();
            }

            var temp = $("#rfvddlOrganization")[0];
            if (temp) window.ValidatorEnable(temp, enabled);

            temp = $("#cfvddlOrganization")[0];
            if (temp) window.ValidatorEnable(temp, enabled);
        }

        function enableNotListed(enabled) {
            if (enabled) {
                $("#OrgNotListedSection .leftPadded").show();
            } else {
                $("#OrgNotListedSection .leftPadded").hide();
            }
        }

        //MS-5317
        function ValidateSelectedOrganizationValue(sender, args) {
            args.IsValid = false;
            var orgCtl = $("#ddlOrganization")[0];
            if (orgCtl && orgCtl.control) {
                var node = orgCtl.control.findItemByText(orgCtl.control.get_text());
                if (node) {
                    args.IsValid = true;
                }
            }
        }
    </script>
    <asp:Literal ID="PageText" runat="server" />
            <asp:PlaceHolder ID="wizIndividualInformationStep" runat="server">
                <div class="sectionContent">
                    <p>
                        <asp:Literal ID="lPleaseEnterBelow" runat="server">Please enter your contact information below.</asp:Literal>
                    </p>
                    <asp:CustomValidator ID="cvIndividualPhoneNumber" runat="server" Display="None" ValidationGroup="IndividualPhoneNumberAddress"
                        ErrorMessage="Please enter at least one phone number." />
                    <asp:CustomValidator ID="cvIndividualAddress" runat="server" Display="None" ValidationGroup="IndividualPhoneNumberAddress"
                        ErrorMessage="Please enter at least one address." />
                    <asp:ValidationSummary ID="vsIndividualSummary" DisplayMode="BulletList" ShowSummary="true"
                        ValidationGroup="IndividualPhoneNumberAddress" ForeColor="Red" HeaderText="We were unable to continue for the following reasons:"
                        runat="server" />
                    <span class="requiredField">*</span> - indicates a required field. &nbsp;
                    <div id="divOrganizationInformation" runat="server" visible="false">
                        <h2>
                            <asp:Literal ID="lOrgInfo" runat="server">Organization Information</asp:Literal></h2>
                        <p>
                            <asp:Literal ID="lOrgTIedTo" runat="server">What organization do you belong to?</asp:Literal></p>
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top;" id="HaveOrgSection">
                                    <asp:RadioButton ID="rbHaveOrg" runat="server" GroupName="orgOption" Text=" I am affiliated with an organization."
                                        ClientIDMode="Static" />
                                    <table class="padded">
                                        <tr>
                                            <%--  <td class="columnHeader" style="width: 400px; vertical-align: top">
                                                <asp:Literal ID="lWhatOrg" runat="server">With what organization are you primarily affiliated?</asp:Literal>
                                                <span class="requiredField">*</span>
                                            </td>--%>
                                            <td id="tdOrganization" runat="server" style="float: left; vertical-align: top" colspan="2">
                                                <%--DO NOT REMOVE THIS COLUM- IT IS DYNAMICALLY FILLED FROM CODE BEHIND--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="lblOrgTIedTo" runat="server">
                                                    Start typing your organization's name in the field above. If your organization does not appear, be sure your spelling is correct or an alternate name isn't available.  If your organization's name still does not appear, choose the "I am affiliated with an organization, but it does not appear" option below.
                                                </asp:Label>
                                            </td>
                                        </tr>
                                        <tr runat="server" id="trOrganizationRole">
                                            <td class="columnHeader">
                                                <asp:Literal ID="lWhatRole" runat="server">What is your role in the organization?</asp:Literal>
                                            </td>
                                            <td>
                                                <asp:DropDownList runat="server" ID="ddlPortalSignupRelationshipTypes" DataTextField="Name"
                                                    DataValueField="ID" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top;" id="OrgNotListedSection">
                                    <asp:RadioButton ID="rbNotListed" runat="server" GroupName="orgOption" Text=" I am affiliated with an organization, but it does not appear."
                                        ClientIDMode="Static" />
                                    <div class="leftPadded">
                                        <asp:Literal ID="lNoAffiliation" runat="server" />
                                        <table>
                                            <tr runat="server" id="trOrganizationRole_New" style="margin-bottom: 10px;">
                                                <td class="columnHeader">
                                                    <asp:Literal ID="lWhatRole_New" runat="server">What is your role in the organization?</asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:DropDownList runat="server" ID="ddlPortalSignupRelationshipTypes_New" DataTextField="Name"
                                                        DataValueField="ID" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top;">
                                    <asp:RadioButton ID="rbNoAffiliation" runat="server" GroupName="orgOption" Text=" I am not affiliated with an organization."
                                        ClientIDMode="Static" CssClass="noWrap" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <h2>
                        <asp:Literal ID="lBasicInfo" runat="server">Basic Information</asp:Literal></h2>
                    <table>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lLoginID" runat="server">Login (email address):</asp:Literal>
                                            <span class="requiredField">*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbLoginID" runat="server" TabIndex="3" />
                                            <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ErrorMessage="Please enter your login ID"
                                                ControlToValidate="tbLoginID" Display="None" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lPassword" runat="server">Password:</asp:Literal>
                                            <span class="requiredField">*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbPassword" TextMode="Password" runat="server" TabIndex="5" />
                                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Please enter a new password"
                                                ControlToValidate="tbPassword" Display="None" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lConfirmPassword" runat="server">Confirm Password:</asp:Literal>
                                            <span class="requiredField">*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbConfirmPassword" TextMode="Password" runat="server" TabIndex="8" />
                                            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ErrorMessage="Please enter a password confirmation"
                                                ControlToValidate="tbConfirmPassword" Display="None" />
                                            <asp:CompareValidator ID="cvPasswordConfirm" runat="server" ErrorMessage="Password and confirmation password do not match"
                                                ControlToValidate="tbConfirmPassword" ControlToCompare="tbPassword" Display="None" />
                                        </td>
                                    </tr>
                                    <!--<tr>
                                        <td class="columnHeader">
                                            Prefix:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualTitle" runat="server" TabIndex="10" />
                                        </td>
                                    </tr>-->
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lFirstName" runat="server">First Name:</asp:Literal>
                                            <span class="requiredField">*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualFirstName" runat="server" TabIndex="20" />
                                            <asp:RequiredFieldValidator ID="rfvIndividualFirstName" runat="server" ErrorMessage="Please enter a first name."
                                                ControlToValidate="tbIndividualFirstName" Display="None" />
                                        </td>
                                    </tr>
                                    <!-- <tr>
                                        <td class="columnHeader">
                                            Middle Name:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualMiddleName" runat="server" TabIndex="30" />
                                        </td>
                                    </tr>-->
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lLastName" runat="server">Last Name:</asp:Literal>
                                            <span class="requiredField">*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualLastName" runat="server" TabIndex="40" />
                                            <asp:RequiredFieldValidator ID="rfvIndividualLastName" runat="server" ErrorMessage="Please enter a last name."
                                                ControlToValidate="tbIndividualLastName" Display="None" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lSuffix" runat="server">Suffix:</asp:Literal>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualSuffix" runat="server" TabIndex="50" />
                                        </td>
                                    </tr>
                                    <!-- <tr>
                                        <td class="columnHeader">
                                            Nickname:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualNickName" runat="server" TabIndex="60" />
                                        </td>
                                    </tr>-->
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lEmailAddress" runat="server">Email Address:</asp:Literal>
                                            <span class="requiredField">*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualEmail" runat="server" TabIndex="100" />
                                            <asp:RequiredFieldValidator ID="rfvIndividualEmail" runat="server" ErrorMessage="Please enter an email address."
                                                ControlToValidate="tbIndividualEmail" Display="None" />
                                        </td>
                                    </tr>
                                    <!--  <tr>
                                        <td class="columnHeader">
                                            Email Address #2:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualEmail2" runat="server" TabIndex="110" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            Email Address #3:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbIndividualEmail3" runat="server" TabIndex="120" />
                                        </td>
                                    </tr>-->
                                </table>
                            </td>
                            <td>
                                <table id="tblProfilePhoto" runat="server">
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
                                            <asp:FileUpload ID="individualImageUpload" Width="300px" runat="server" /><br />
                                            Images larger than 120x120 will be resized
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CustomValidator runat="server" ControlToValidate="individualImageUpload" ForeColor="Red"
                                                OnServerValidate="imageValidate" ErrorMessage="The file specified is not a valid image." />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <asp:PlaceHolder ID="phIndividualPhoneNumbers" Visible="false" runat="server">
                        <h2>
                            <asp:Literal ID="lPhoneNumbers" runat="server">Phone Numbers</asp:Literal></h2>
                        <asp:Literal ID="lPleaseEnterAtLeastOneNumber" runat="server">Use the <b>Preferred?</b> radio
                        button to indicate the phone number at which you prefer to be contacted.</asp:Literal>
                        <asp:GridView ID="gvIndividualPhoneNumbers" OnRowDataBound="gvIndividualPhoneNumbers_OnRowDataBound"
                            GridLines="None" AutoGenerateColumns="false" Width="460px" runat="server">
                            <Columns>
                                <asp:TemplateField HeaderStyle-BackColor="White">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIndividualPhoneNumberType" runat="server" />
                                        Phone Number:
                                        <asp:Literal ID="lPhoneNumberRequired" runat="server" Visible="false"><span class="requiredField">*</span></asp:Literal>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-BackColor="White">
                                    <ItemTemplate>
                                        <asp:TextBox ID="tbIndividualPhoneNumber" runat="server" />
                                        <asp:RequiredFieldValidator ID="rfvPhoneNumber" ControlToValidate="tbIndividualPhoneNumber"
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                    HeaderText="Preferred?" HeaderStyle-CssClass="columnHeader" HeaderStyle-BackColor="White">
                                    <ItemTemplate>
                                        <!-- We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                    in repeater controls

                    http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->
                                        <asp:Literal ID="lIndividualRadioButtonMarkup" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phIndividualAddresses" runat="server">
                        <h2>
                            <asp:Literal ID="lAddressInfo" runat="server">Address Information</asp:Literal></h2>
                        <div>
                            <table cellpadding="0" cellspacing="0" style="margin-top: 10px;">
                                <tr>
                                    <asp:Repeater ID="rptIndividualAddresses" OnItemCreated="rptIndividualAddresses_OnItemCreated"
                                        OnItemDataBound="rptIndividualAddresses_OnItemDataBound" runat="server">
                                        <ItemTemplate>
                                            <td>
                                                <b><u>
                                                    <asp:Label ID="lblIndividualAddressType" runat="server"></asp:Label>
                                                    Address</u></b>
                                                <br />
                                                <asp:HiddenField ID="hfIndividualAddressCode" runat="server" />
                                                <cc1:AddressControl ID="acIndividualAddress" EnableValidation="True" runat="server" />
                                            </td>
                                            <asp:Literal ID="lIndividualNewRowTag" runat="server" />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tr>
                                <asp:Literal ID="lIndividualAddressEndRow" runat="server" />
                            </table>
                        </div>
                        <div style="padding-top: 10px">
                            <h3>
                                <asp:Literal ID="lAddressPrefs" runat="server">Address Preferences</asp:Literal></h3>
                        </div>
                        <table style="width: 650px">
                            <tr>
                                <td>
                                    <asp:Literal ID="lPreferredAddress" runat="server">What is your preferred mailing address?</asp:Literal>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlIndividualPreferredAddress" runat="server" />
                                </td>
                            </tr>
                            <tr id="trIndividualSeasonality" runat="server" visible="false">
                                <td style="padding-top: 10px">
                                    Would you like to receive mail to your
                                    <asp:Label ID="lblIndividualSeasonalAddressType" runat="server" />
                                    address during certain times of the year?
                                </td>
                                <td style="width: 400px">
                                    <asp:RadioButton ID="rbIndividualSeasonalYes" runat="server" Text="Yes" GroupName="Seasonal" />
                                    - from
                                    <cc1:MonthDayPicker ID="mdpIndividualSeasonalStart" runat="server" />
                                    to
                                    <cc1:MonthDayPicker ID="mdpIndividualSeasonalEnd" runat="server" />
                                    <br />
                                    <asp:RadioButton ID="rbIndividualSeasonalNo" runat="server" Text="No, that will not be necessary"
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
                    <asp:PlaceHolder ID="phIndividualOtherInformation" runat="server">
                        <div class="customFieldsWrapper">
                        <uc1:CustomFieldSet ID="cfsIndividualCustomFields" runat="server" />
                        </div>
                    </asp:PlaceHolder>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" Visible="False" ID="wizOrganizationInformationStep">
                <p>
                    <asp:Literal ID="lOrgContactInfo" runat="server">Please enter the contact information <b>for the Organization</b> below.</asp:Literal>
                </p>
                <span class="requiredField">*</span> - indicates a required field. &nbsp;<h2>
                    <asp:Literal ID="lOrgBasic" runat="server">Basic Information</asp:Literal></h2>
                <table>
                    <tr>
                        <td>
                            <table style="width: 100%">
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Literal ID="lOrgName" runat="server">Organization Name:</asp:Literal>
                                        <span class="requiredField">*</span>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbOrganizationName" runat="server" TabIndex="10" />
                                        <asp:RequiredFieldValidator ID="rfvOrganizationName" ControlToValidate="tbOrganizationName"
                                            runat="server" ErrorMessage="Please enter the name of the organization." Display="None" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <h2>
                    <asp:Literal ID="lOrgBillingContact" runat="server">Billing Contact</asp:Literal></h2>
                <table>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lOrgBillingName" runat="server">Name:</asp:Literal>
                            <span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbBillingContactName" runat="server" TabIndex="100" />
                            <asp:RequiredFieldValidator ID="rfvBillingContactName" runat="server" ErrorMessage="Please enter billing contact name."
                                ControlToValidate="tbBillingContactName" Display="None" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lOrgPhoneNumber" runat="server">Phone Number:</asp:Literal>
                            <span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbBillingContactPhoneNumber" runat="server" TabIndex="100" />
                            <asp:RequiredFieldValidator ID="rfvBillingContactPhoneNumber" runat="server" ErrorMessage="Please enter billing contact phone number."
                                ControlToValidate="tbBillingContactPhoneNumber" Display="None" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lOrgEmail" runat="server">Email Address:</asp:Literal>
                            <span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbOrganizationEmail" runat="server" TabIndex="100" />
                            <asp:RequiredFieldValidator ID="rfvOrganizationBillingContactEmail" runat="server"
                                ErrorMessage="Please enter an email address." ControlToValidate="tbOrganizationEmail"
                                Display="None" />
                        </td>
                    </tr>
                </table>
                <asp:PlaceHolder ID="phOrganizationPhoneNumbers" Visible="false" runat="server">
                    <h2>
                        <asp:Literal ID="lOrgPhoneNumbers" runat="server">Phone Numbers</asp:Literal></h2>
                    <asp:Literal ID="lAtLeastOneContact" runat="server">Please enter contact numbers below. Use the <b>Preferred?</b> radio
                    button to indicate the phone number at which you prefer to be contacted.</asp:Literal>
                    <asp:GridView ID="gvOrganizationPhoneNumbers" OnRowDataBound="gvOrganizationPhoneNumbers_OnRowDataBound"
                        GridLines="None" AutoGenerateColumns="false" Width="460px" runat="server">
                        <Columns>
                            <asp:TemplateField HeaderStyle-BackColor="White">
                                <ItemTemplate>
                                    <asp:Label ID="lblOrganizationPhoneNumberType" runat="server" />
                                    Phone Number:
                                    <asp:Literal ID="lPhoneNumberRequired" runat="server" Visible="false"><span class="requiredField">*</span></asp:Literal>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-BackColor="White">
                                <ItemTemplate>
                                    <asp:TextBox ID="tbOrganizationPhoneNumber" runat="server" /><asp:RequiredFieldValidator
                                        ID="rfvPhoneNumber" Enabled="false" Display="None" ControlToValidate="tbOrganizationPhoneNumber"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                HeaderText="Preferred?" HeaderStyle-CssClass="columnHeader" HeaderStyle-BackColor="White">
                                <ItemTemplate>
                                    <!-- We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                    in repeater controls

                    http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->
                                    <asp:Literal ID="lOrganizationRadioButtonMarkup" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phOrganizationAddresses" runat="server">
                    <h2>
                        <asp:Literal ID="lOrgAddressInfo" runat="server">Address Information</asp:Literal></h2>
                    <asp:Literal ID="lOrgAtLeastOneAddress" runat="server">Please enter your addresses below.</asp:Literal>
                    <div>
                        <table cellpadding="0" cellspacing="0" style="margin-top: 10px;">
                            <tr>
                                <asp:Repeater ID="rptOrganizationAddresses" OnItemCreated="rptOrganizationAddresses_OnItemCreated"
                                    OnItemDataBound="rptOrganizationAddresses_OnItemDataBound" runat="server">
                                    <ItemTemplate>
                                        <td>
                                            <b><u>
                                                <asp:Label ID="lblOrganizationAddressType" runat="server"></asp:Label>
                                                Address</u></b>
                                            <br />
                                            <asp:HiddenField ID="hfOrganizationAddressCode" runat="server" />
                                            <cc1:AddressControl ID="acOrganizationAddress" EnableValidation="True" runat="server" />
                                        </td>
                                        <asp:Literal ID="lOrganizationNewRowTag" runat="server" />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                            <asp:Literal ID="lOrganizationAddressEndRow" runat="server" />
                        </table>
                    </div>
                    <div style="padding-top: 10px">
                        <h3>
                            <asp:Literal ID="lOrgAddressPrefs" runat="server">Address Preferences</asp:Literal></h3>
                    </div>
                    <table style="width: 650px">
                        <tr>
                            <td>
                                <asp:Literal ID="lOrgPreferredAddress" runat="server">What is your preferred mailing address?</asp:Literal>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlOrganizationPreferredAddress" runat="server" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phOrganizationOtherInformation" runat="server">
                    <uc1:CustomFieldSet ID="cfsOrganizationCustomFields" runat="server" />
                </asp:PlaceHolder>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" Visible="False" ID="wizDuplicateOrganizationStep">
            

                <div class="sectionContent" style="margin-top: 10px">
                    <div class="sectHeaderTitle">
                        <h2>
                            <asp:Literal ID="lPotentialMatches" runat="server">Potential Matches</asp:Literal>
                        </h2>
                    </div>
                    <p>
                        <asp:Literal ID="Literal1" runat="server">We have located some existing records in our system that may be you.</asp:Literal>
                    </p>
                    <asp:GridView ID="gvDuplicates" runat="server" GridLines="None" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="FirstName" />
                            <asp:BoundField DataField="_Preferred_Address_City" HeaderStyle-HorizontalAlign="Left" HeaderText="City" />
                            <asp:BoundField DataField="_Preferred_Address_State" HeaderStyle-HorizontalAlign="Left" HeaderText="State" />
                        </Columns>
                    </asp:GridView>
                    <br />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" Visible="False" ID="wizConfirmationStep">
                <div class="sectionContent">
                    <p>
                        <asp:Literal ID="lConfirmFollowing" runat="server"><b>Please confirm the information below then click "Submit".</b></asp:Literal>
                    </p>
                    <asp:Literal ID="lFollowingRecordsCreated" runat="server">The following records will be created:</asp:Literal>
                    <ul>
                        <li>
                            <asp:Literal runat="server" ID="litPortalUser" />
                        </li>
                        <li>
                            <asp:Literal runat="server" ID="litIndividual" />
                        </li>
                        <li id="liOrganization" runat="server">
                            <asp:Literal runat="server" ID="litOrganization" />
                        </li>
                        <li id="liRelationship" runat="server">
                            <asp:Literal runat="server" ID="litRelationship" />
                        </li>
                    </ul>
                </div>
            </asp:PlaceHolder>
            <table width="100%">
                <tr>
                    <td align="center">
                        <asp:Button runat="server" ID="btnPrevious" OnClick="wizCreateAccount_PreviousButtonClick" CausesValidation="false"
                            Text="Back" />
                        <asp:Button runat="server" ID="btnFinish" OnClick="wizCreateAccount_FinishButtonClick" CausesValidation="true"
                            Text="Submit" />
                        <asp:Button runat="server" ID="btnNext" OnClick="wizCreateAccount_NextButtonClick" CausesValidation="true"
                            onClientClick="return onNextClick();"
                            Text="Next" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnCancel" OnClick="wizCreateAccount_CancelButtonClick" CausesValidation="false"
                            Text="Cancel" />
                    </td>
                </tr>
            </table>

    <%--        <StartNavigationTemplate>
            <table width="100%">
                <tr>
                    <td align="center">
                        <asp:Button runat="server" ID="btnNext" CommandName="MoveNext" CausesValidation="true"
                            onClientClick="return onNextClick();"
                            Text="Next" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnCancel" CommandName="Cancel" CausesValidation="false"
                            Text="Cancel" />
                    </td>
                </tr>
            </table>
        </StartNavigationTemplate>
        <StepNavigationTemplate>
        </StepNavigationTemplate>
        <FinishNavigationTemplate>
            <table width="100%">
                <tr>
                    <td align="center">
                        <asp:Button runat="server" ID="btnPrevious" CommandName="MovePrevious" CausesValidation="false"
                            Text="Back" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnCancel" CommandName="Cancel" CausesValidation="false"
                            Text="Cancel" />
                    </td>
                </tr>
            </table>
        </FinishNavigationTemplate>
    </asp:Wizard>
        --%>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
