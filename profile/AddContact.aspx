<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddContact.aspx.cs" Inherits="profile_AddContact"
    MasterPageFile="~/App_Master/GeneralPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Add a Contact to
    <%=targetOrganization.Name %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <asp:Literal ID="PageText" runat="server" />
        <asp:ValidationSummary ID="vsNewUserSummary" DisplayMode="BulletList" ForeColor="Red"
            Font-Bold="true" ShowSummary="true" HeaderText="We were unable to continue for the following reasons:"
            runat="server" />
        <asp:Wizard ID="wizAddContact" runat="server" DisplaySideBar="false" OnFinishButtonClick="wizAddContact_FinishButtonClick"
            OnNextButtonClick="wizAddContact_NextButtonClick" OnCancelButtonClick="wizAddContact_CancelButtonClick"
            CssClass="sectionContent" Width="300">
            <WizardSteps>
                <asp:WizardStep ID="wizSearchEmailStep">
                    <asp:Label runat="server" ID="lblAlreadyExists" Visible="false" CssClass="redHighlight"></asp:Label>
                    <div class="sectionContent">
                        <p>
                            <asp:Literal ID="lAddContact" runat="server">If we can, we’re going to add an existing contact to your organization by using
                            the email address. If the email doesn’t exist, we’ll ask you for additional information.</asp:Literal>
                        </p>
                        <table>
                            <tr>
                                <td class="columnHeader" width="25%">
                                    <asp:Literal ID="lEmailAddress" runat="server">Email Address:</asp:Literal><span
                                        class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="tbEmailAddress" />
                                    <asp:RequiredFieldValidator runat="server" ID="rfvEmailAddress" ErrorMessage="Please enter an email address to be searched for."
                                        ControlToValidate="tbEmailAddress" Display="None" />
                                </td>
                            </tr>
                            <tr runat="server" id="trRelationshipType">
                                <td class="columnHeader">
                                    <asp:Literal ID="lContactType" runat="server">Contact Type:</asp:Literal><span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlRelationshipType" DataTextField="Name" DataValueField="ID" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                </asp:WizardStep>
                <asp:WizardStep ID="wizNotFound">
                    <asp:CustomValidator ID="cvContactRestriction" runat="server" Display="None" ForeColor="Red"
                        Font-Bold="true" />
                    <div class="sectionContent">
                        <p>
                            <asp:Literal ID="lUnableToLocate" runat="server">We were unable to locate an individual with that email address. Please enter the
                            contact’s information below:</asp:Literal>
                        </p>
                        <table>
                            <tr>
                                <td class="columnHeader" width="25%">
                                    <asp:Literal ID="lFirstName" runat="server">First Name:</asp:Literal><span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="tbFirstName" />
                                    <asp:RequiredFieldValidator runat="server" ID="rfvFirstName" ErrorMessage="Please enter a first name."
                                        ControlToValidate="tbFirstName" Display="None" />
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lLastName" runat="server">Last Name:</asp:Literal><span class="requiredField">*</span>
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="tbLastName" />
                                    <asp:RequiredFieldValidator runat="server" ID="rfvLastName" ErrorMessage="Please enter a last name."
                                        ControlToValidate="tbLastName" Display="None" />
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Literal ID="lEmailAddressConfirm" runat="server">Email Address:</asp:Literal>
                                </td>
                                <td>
                                    <%=targetEmailAddress %>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:CheckBox runat="server" ID="chkSendInvitation" /><asp:Literal ID="lSendInvitation"
                                        runat="server"><b>Would you like to send this
                                        individual an invitation to login?</b></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                </asp:WizardStep>
                <asp:WizardStep ID="wizConfirmationStep">
                    <div class="sectionContent">
                        <p>
                            A <b>
                                <%=targetRelationshipType.Name %></b> relationship will be created between <b>
                                    <%=targetOrganization.Name %></b> and the following individual:
                        </p>
                        <table>
                            <tr>
                                <td class="columnHeader" width="25%">
                                    First Name:
                                </td>
                                <td>
                                    <%=targetIndividual.FirstName %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    Last Name:
                                </td>
                                <td>
                                    <%=targetIndividual.LastName %>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    Email Address:
                                </td>
                                <td>
                                    <%=targetEmailAddress %>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                </asp:WizardStep>
            </WizardSteps>
            <StartNavigationTemplate>
                <table width="100%">
                    <tr>
                        <td align="center">
                            <asp:Button runat="server" ID="btnNext" CommandName="MoveNext" CausesValidation="true"
                                Text="Continue" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
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
                                Text="Continue" />
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
                                Text="Add Contact" />
                            <asp:Button runat="server" ID="btnPrevious" CommandName="MovePrevious" CausesValidation="false"
                                Text="Back" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button runat="server" ID="btnCancel" CommandName="Cancel" CausesValidation="false"
                                Text="Cancel" />
                        </td>
                    </tr>
                </table>
            </FinishNavigationTemplate>
        </asp:Wizard>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
