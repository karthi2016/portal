<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="CreateAccount_CreateUser.aspx.cs" Inherits="profile_CreateAccount_Complete" %>

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
    <asp:Label ID="lblTitle" runat="server">Create Account</asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <asp:Wizard ID="wizCreateAccount" runat="server" DisplaySideBar="false" OnFinishButtonClick="wizCreateAccount_FinishButtonClick"
        OnNextButtonClick="wizCreateAccount_NextButtonClick" OnCancelButtonClick="wizCreateAccount_CancelButtonClick"
        CssClass="sectionContent" Width="300">
        <WizardSteps>
            <asp:WizardStep ID="wizIndividualInformationStep" runat="server" Title="Account Information">
                <div class="sectionContent">
                    <p>
                       <asp:Literal ID="lPleaseEnterBelow" runat="server">Please enter your contact information below.</ASP:Literal>
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
                            <asp:Literal ID="lOrgTIedTo" runat="server">We’d like to know what organization you are tied to. If you don’t see the organization
                            in the list below, select <b>My Organization is not listed</b> from the drop downlist.</asp:Literal></p>
                        <table style="width: 100%">
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lWhatOrg" runat="server">What organization do you belong to?</asp:Literal>
                                </td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlAllOrganizations" DataTextField="Name" DataValueField="ID"
                                        AppendDataBoundItems="true">
                                        <asp:ListItem Text="I am not affiliated with an organization" Value="" Selected="True" />
                                        <asp:ListItem Text="My Organization is not listed" Value="notlisted" />
                                    </asp:DropDownList>
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
                    </div>
                    <h2>
                        <asp:Literal ID="lBasicInfo" runat="server">Basic Information</asp:Literal></h2>
                    <table>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lLoginID" runat="server">Login ID:</asp:Literal>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbLoginID" runat="server" TabIndex="3" /><font color="red">*</font>
                                            <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ErrorMessage="Please enter your login ID"
                                                ControlToValidate="tbLoginID" Display="None" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lPassword" runat="server">Password:</asp:Literal>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbPassword" TextMode="Password" runat="server" TabIndex="5" /><font
                                                color="red">*</font>
                                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Please enter a new password"
                                                ControlToValidate="tbPassword" Display="None" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Literal ID="lConfirmPassword" runat="server">Confirm Password:</asp:Literal>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbConfirmPassword" TextMode="Password" runat="server" TabIndex="8" /><font
                                                color="red">*</font>
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
                                            <asp:Literal ID="lFirstName" runat="server">First Name:</asp:Literal><span class="requiredField">*</span>
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
                                            <asp:Literal ID="lLastName" runat="server">Last Name:</asp:Literal> <span class="requiredField">*</span>
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
                                            <asp:Literal ID="lEmailAddress" runat="server">Email Address:</asp:Literal><span class="requiredField">*</span>
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
                            <td valign="top">
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
                                        <asp:RequiredFieldValidator ID="rfvPhoneNumber" ControlToValidate="tbIndividualPhoneNumber" runat="server" />
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
                                                <cc1:AddressControl ID="acIndividualAddress" EnableValidation="false" runat="server" />
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
                    <asp:PlaceHolder ID="phIndividualOtherInformation" runat="server">
                        <uc1:CustomFieldSet ID="cfsIndividualCustomFields" runat="server" />
                    </asp:PlaceHolder>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="wizOrganizationInformationStep">
                <p>
                    <asp:Literal ID="lOrgContactInfo" runat="server">Please enter the contact information <b>for the Organization</b> below.</asp:Literal>
                </p>
                <span class="requiredField">*</span> - indicates a required field. &nbsp;<h2>
                    <asp:Literal ID="lOrgBasic" runat="server">Basic Information</asp:Literal></h2>
                <table>
                    <tr>
                        <td valign="top">
                            <table style="width: 100%">
                                <tr>
                                    <td class="columnHeader">
                                       <asp:Literal ID="lOrgName" runat="server">Organization Name:</asp:Literal><span class="requiredField">*</span>
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
                            <asp:Literal ID="lOrgBillingName" runat="server">Name:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbBillingContactName" runat="server" TabIndex="100" />
                            <asp:RequiredFieldValidator ID="rfvBillingContactName" runat="server" ErrorMessage="Please enter billing contact name."
                                ControlToValidate="tbBillingContactName" Display="None" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lOrgPhoneNumber" runat="server">Phone Number:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbBillingContactPhoneNumber" runat="server" TabIndex="100" />
                            <asp:RequiredFieldValidator ID="rfvBillingContactPhoneNumber" runat="server" ErrorMessage="Please enter billing contact phone number."
                                ControlToValidate="tbBillingContactPhoneNumber" Display="None" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="lOrgEmail" runat="server">Email Address:</asp:Literal><span class="requiredField">*</span>
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
                                    Phone Number: <asp:Literal ID="lPhoneNumberRequired" runat="server" Visible="false"><span class="requiredField">*</span></asp:Literal>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-BackColor="White">
                                <ItemTemplate>
                                    <asp:TextBox ID="tbOrganizationPhoneNumber" runat="server" /><asp:RequiredFieldValidator ID="rfvPhoneNumber" Enabled="false" Display="None" ControlToValidate="tbOrganizationPhoneNumber"
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
                                            <cc1:AddressControl ID="acOrganizationAddress" EnableValidation="false" runat="server" />
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
            </asp:WizardStep>
            <asp:WizardStep ID="wizDuplicateOrganizationStep" runat="server">
            

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
            </asp:WizardStep>
            <asp:WizardStep ID="wizConfirmationStep">
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
            </asp:WizardStep>
        </WizardSteps>
        <StartNavigationTemplate>
            <table width="100%">
                <tr>
                    <td align="center">
                        <asp:Button runat="server" ID="btnNext" CommandName="MoveNext" CausesValidation="true"
                            Text="Next" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnCancel" CommandName="Cancel" CausesValidation="false"
                            Text="Cancel" />
                    </td>
                </tr>
            </table>
        </StartNavigationTemplate>
        <StepNavigationTemplate>
            <table width="100%">
                <tr>
                    <td align="center">
                        <asp:Button runat="server" ID="btnNext" CommandName="MoveNext" CausesValidation="true"
                            Text="Next" />
                        <asp:Button runat="server" ID="btnPrevious" CommandName="MovePrevious" CausesValidation="false"
                            Text="Back" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnCancel" CommandName="Cancel" CausesValidation="false"
                            Text="Cancel" />
                    </td>
                </tr>
            </table>
        </StepNavigationTemplate>
        <FinishNavigationTemplate>
            <table width="100%">
                <tr>
                    <td align="center">
                        <asp:Button runat="server" ID="btnFinish" CommandName="MoveComplete" CausesValidation="true"
                            Text="Submit" />
                        <asp:Button runat="server" ID="btnPrevious" CommandName="MovePrevious" CausesValidation="false"
                            Text="Back" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnCancel" CommandName="Cancel" CausesValidation="false"
                            Text="Cancel" />
                    </td>
                </tr>
            </table>
        </FinishNavigationTemplate>
    </asp:Wizard>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
