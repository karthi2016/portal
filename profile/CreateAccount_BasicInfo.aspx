<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="CreateAccount_BasicInfo.aspx.cs" Inherits="profile_CreateAccount_BasicInfo" %>

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
    <asp:ValidationSummary ID="vsNewUserSummary" DisplayMode="BulletList" ValidationGroup="NewUser"
        ShowSummary="false" ShowMessageBox="true" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    <asp:ValidationSummary ID="vsExistingUserSummary" DisplayMode="BulletList" ValidationGroup="ExistingUser"
        ShowSummary="false" ShowMessageBox="true" HeaderText="We were unable to continue for the following reasons:"
        runat="server" />
    <table cellpadding="7">
        <tr>
            <td valign="top">
                <!-- new user -->
                <div class="sectCont" style="width: 410px;">
                    <div class="sectHeaderTitle">
                        <h2>
                            <asp:Literal ID="lNewToSite" runat="server">New to this site?</asp:Literal>
                        </h2>
                    </div>
                    <div class="helpText">
                        <asp:Literal ID="lIfYouAreNew" runat="server">If you are new to this system or not sure if you've used this site before, enter
                        your information below and click Continue.</asp:Literal>
                    </div>
                </div>
                <div style="width: 410px;">
                    <asp:Panel runat="server" ID="pnlNewUser" DefaultButton="btnNewContinue">
                        <table>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lFirstName" runat="server">First Name:</asp:Literal>
                                    <span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbFirstName" runat="server" TabIndex="10" />
                                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ValidationGroup="NewUser"
                                        ErrorMessage="Please enter a first name." ControlToValidate="tbFirstName" Display="None" />
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lLastName" runat="server">Last Name:</asp:Literal>
                                    <span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbLastName" runat="server" TabIndex="20" />
                                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ValidationGroup="NewUser"
                                        ErrorMessage="Please enter a last name." ControlToValidate="tbLastName" Display="None" />
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lEmail" runat="server">Email:</asp:Literal>
                                    <span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbEmail" runat="server" TabIndex="30" />
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ValidationGroup="NewUser"
                                        ErrorMessage="Please enter an email address." ControlToValidate="tbEmail" Display="None" />
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lPostalCode" runat="server">Postal Code:</asp:Literal>
                                    <span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbPostalCode" runat="server" TabIndex="50" />
                                    <asp:RequiredFieldValidator ID="rfvPostalCode" runat="server" ValidationGroup="NewUser"
                                        ErrorMessage="Please enter a postal code." ControlToValidate="tbPostalCode" Display="None" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Button runat="server" ID="btnNewContinue" OnClick="btnNewContinue_Click" TabIndex="60"
                                        CssClass="btn-new-continue"
                                        ValidationGroup="NewUser" CausesValidation="true" Text="Continue"  
                                        OnClientClick="return memerSuitePortal.createAccountBasicInfo.preventNewContinueDoubleClick();" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
                <!-- / new user -->
            </td>
            <td valign="top" id="tdExistingAccount" runat="server">
                <!-- existing user -->
                <div class="sectCont">
                    <div class="sectHeaderTitle">
                        <h2>
                            <asp:Literal ID="lAlreadyHaveAccount" runat="server">Already have an Account?</asp:Literal></h2>
                    </div>
                    <div class="helpText">
                        <asp:Literal ID="lIfAlreadyHere" runat="server">If you've already been to this site, log in below. If you've forgotten your password,
                        please <a href="/profile/ForgotPassword.aspx">click here</a> to reset it.</asp:Literal></div>
                </div>
                <asp:Panel runat="server" ID="pnlExistingUser" DefaultButton="btnExistingContinue">
                    <table>
                        <tr>
                            <td class="columnHeader">
                                <asp:Literal ID="lLoginID" runat="server">Login ID:</asp:Literal>
                                <span class="requiredField">*</span>
                            </td>
                            <td>
                                <asp:TextBox ID="tbLoginID" runat="server" TabIndex="70" />
                                <asp:RequiredFieldValidator ID="rfvLoginID" runat="server" ValidationGroup="ExistingUser"
                                    ErrorMessage="Please enter your login ID." ControlToValidate="tbLoginID" Display="None" />
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Literal ID="lPassword" runat="server">Password:</asp:Literal>
                                <span class="requiredField">*</span>
                            </td>
                            <td>
                                <asp:TextBox TextMode="Password" ID="tbPassword" runat="server" TabIndex="80" />
                                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ValidationGroup="ExistingUser"
                                    ErrorMessage="Please enter your password." ControlToValidate="tbPassword" Display="None" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button runat="server" ID="btnExistingContinue" OnClick="btnExistingContinue_Click"
                                    ValidationGroup="ExistingUser" TabIndex="90" Text="Continue"   />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- / existing user -->
                <div class="clearBothNoSPC">
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
<asp:Content runat="server" ID="scriptLinks" ContentPlaceHolderID="javascriptLinks">
       <script type="text/javascript" src="/js/createAccountBasicInfo.js"></script>
</asp:Content>