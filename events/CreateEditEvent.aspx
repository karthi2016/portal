<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditEvent.aspx.cs" Inherits="events_CreateEditEvent" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlEventOwner" Visible="false" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Label runat="server" ID="lblTitleAction">Create</asp:Label>
    Event
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <asp:Literal ID="PageText" runat="server" />
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lBasicInformation" runat="server">Basic Information</asp:Literal> </h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr runat="server" id="trOwner" visible="false">
                    <td class="columnHeader">
                        <asp:Label ID="lblOwner" runat="server"></asp:Label>
                    </td>
                    <td colspan="3" align="left">
                        <asp:Label runat="server" ID="lblOwnerName"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lName" runat="server">Name:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td colspan="3" align="left">
                        <asp:TextBox runat="server" ID="tbName" Width="450" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" ControlToValidate="tbName"
                            Display="None" ErrorMessage="Please specify a Name." />
                    </td>
                </tr>
                
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lType" runat="server">Type:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlType" DataTextField="Text" DataValueField="Value"
                            AppendDataBoundItems="true">
                            <asp:ListItem Text="---- Select ----" Value="0" />
                        </asp:DropDownList>
                        <asp:CompareValidator runat="server" ID="cvType" ControlToValidate="ddlType" Display="None"
                            ErrorMessage="Please specify a Type." Operator="NotEqual" ValueToCompare="0" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lStartDate" runat="server">Start Date:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpStartDate" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvStartDate" ControlToValidate="dtpStartDate"
                            Display="None" ErrorMessage="Please specify a Start Date." />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lCategory" runat="server">Category:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlCategory" AppendDataBoundItems="true" DataTextField="Text"
                            DataValueField="Value">
                            <asp:ListItem Text="---- Select ----" />
                        </asp:DropDownList>
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lEndDate" runat="server">End Date:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpEndDate" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvEndDate" ControlToValidate="dtpEndDate"
                            Display="None" ErrorMessage="Please specify an End Date." />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lCode" runat="server">Code:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbCode" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lIsClosed" runat="server">Is Closed?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkClosed" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lURL" runat="server">Url:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbUrl" Width="450" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lExternalMerchantAccount" runat="server">External Merchant Account:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlMerchant" AppendDataBoundItems="true" DataTextField="Name"
                            DataValueField="ID">
                            <asp:ListItem Text="Use Default" Value="" />
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lDescription" runat="server">Description</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <telerik:RadEditor runat="server" ID="reDescription" ToolsFile="~/controls/telerik/ToolsFileDeluxe.xml" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lRegistrationSettings" runat="server">Registration Settings</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lRegistrationMode" runat="server">Registration Mode: </asp:Literal><span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlRegistrationMode" DataTextField="Text" DataValueField="Value" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lRegistrationOpens" runat="server">Registration Opens:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpRegistrationOpens" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lRequiresApproval" runat="server">Requires Approval:</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkRequiresApproval" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lRegistrationCloses" runat="server">Registration Closes:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpRegistrationCloses" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lCapacity" runat="server">Capacity:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" Width="75" ID="tbCapacity" />
                        <asp:CompareValidator runat="server" ID="cvCapacity" ControlToValidate="tbCapacity"
                            Display="None" ErrorMessage="Please specify a valid number for Capacity." Operator="GreaterThanEqual"
                            ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvCapacityType" ControlToValidate="tbCapacity"
                            Display="None" ErrorMessage="Please specify a valid number for Capacity." Operator="DataTypeCheck"
                            Type="Integer" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lPreRegistration" runat="server">Pre-Registration:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpPreRegistration" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lAllowWaitList" runat="server">Allow Wait List:</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkWaitList" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lEarlyReg" runat="server">Early Registration:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpEarlyRegistration" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lConfirmationEmail" runat="server">Confirmation Email:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlConfirmationEmail" AppendDataBoundItems="true"
                            DataTextField="Text" DataValueField="Value">
                            <asp:ListItem Text="Use Default" Value="" />
                        </asp:DropDownList>
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lRegularRegistration" runat="server"> Regular Registration:</asp:Literal>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker runat="server" ID="dtpRegularRegistration" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lRegistrationUrl" runat="server">Registration Url:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbRegistrationUrl" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lGoals" runat="server">Goals and Estimates</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lProjectedAttendance" runat="server">Projected Attendance:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" Width="75" ID="tbProjectedAttendance" />
                        <asp:CompareValidator runat="server" ID="cvProjectedAttendance" ControlToValidate="tbProjectedAttendance"
                            Display="None" ErrorMessage="Please specify a valid number for Projected Attendance."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvProjectedAttendanceType" ControlToValidate="tbProjectedAttendance"
                            Display="None" ErrorMessage="Please specify a valid number for Projected Attendance."
                            Operator="DataTypeCheck" Type="Integer" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lRevenueGoal" runat="server">Revenue Goal:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" Width="75" ID="tbRevenueGoal" />
                        USD
                        <asp:CompareValidator runat="server" ID="gvRevenueGoal" ControlToValidate="tbRevenueGoal"
                            Display="None" ErrorMessage="Please specify a valid amount for Revenue Goal."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvRevenueGoalType" ControlToValidate="tbRevenueGoal"
                            Display="None" ErrorMessage="Please specify a valid amount for Revenue Goal."
                            Operator="DataTypeCheck" Type="Currency" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lAttendance" runat="server">Guaranteed Attendance:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" Width="75" ID="tbGuaranteedAttendance" />
                        <asp:CompareValidator runat="server" ID="cvGaran" ControlToValidate="tbGuaranteedAttendance"
                            Display="None" ErrorMessage="Please specify a valid number for Guaranteed Attendance."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvGuaranteedAttendance" ControlToValidate="tbGuaranteedAttendance"
                            Display="None" ErrorMessage="Please specify a valid number for Guaranteed Attendance."
                            Operator="DataTypeCheck" Type="Integer" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lRegistrationGoal" runat="server">Registration Goal:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" Width="75" ID="tbRegistrationGoal" />
                        <asp:CompareValidator runat="server" ID="cvRegistrationGoal" ControlToValidate="tbRegistrationGoal"
                            Display="None" ErrorMessage="Please specify a valid number for Registration Goal."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvRegistrationGoalType" ControlToValidate="tbRegistrationGoal"
                            Display="None" ErrorMessage="Please specify a valid number for Registration Goal."
                            Operator="DataTypeCheck" Type="Integer" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divRegistrationFees">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lRegistrationFees" runat="server">Registration Fees</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Label runat="server" ID="lblRegistrationFeeSaveFirst" Text="There are no Registration Fees assigned to this event." />
            <asp:GridView ID="gvRegistrationFees" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no Registration Fees assigned to this event." OnRowCommand="gvRegistrationFees_RowCommand">
                <Columns>
                    <asp:BoundField DataField="DisplayOrder" HeaderStyle-HorizontalAlign="Left" HeaderText="DisplayOrder" />
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Code" HeaderStyle-HorizontalAlign="Left" HeaderText="Code" />
                    <asp:BoundField DataField="Price" HeaderStyle-HorizontalAlign="Left" HeaderText="Price"
                        DataFormatString="{0:C}" />
                    <asp:BoundField DataField="IsActive" HeaderStyle-HorizontalAlign="Left" HeaderText="Active" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# Bind("ID") %>' CommandName="editregistrationfee"
                                CausesValidation="true" Text="(edit)" OnClientClick="if (!window.confirm('This action will save the event.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deleteregistrationfee" CausesValidation="false" Text="(delete)"
                                OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hr />
            <div style="text-align: right;">
                <asp:LinkButton runat="server" ID="lbAddRegistrationFee" OnClientClick="if (!window.confirm('This action will save the event.')) return false;"
                    OnClick="lbAddRegistrationFee_Click" Text="Add Registration Fee" />
            </div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divRegistrationQuestions">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lRegistrationQuestions" runat="server">Registration Questions</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Label runat="server" ID="lblRegistrationQuestionSaveFirst" Text="There are no Registration Questions assigned to this event." />
            <asp:GridView ID="gvRegistrationQuestions" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no Registration Questions assigned to this event." OnRowCommand="gvRegistrationQuestions_RowCommand">
                <Columns>
                    <asp:BoundField DataField="DisplayOrder" HeaderStyle-HorizontalAlign="Left" HeaderText="DisplayOrder" />
                    <asp:BoundField DataField="Label" HeaderStyle-HorizontalAlign="Left" HeaderText="Label" />
                    <asp:BoundField DataField="DataType" HeaderStyle-HorizontalAlign="Left" HeaderText="DataType" />
                    <asp:BoundField DataField="DisplayType" HeaderStyle-HorizontalAlign="Left" HeaderText="DisplayType" />
                    <asp:BoundField DataField="IsRequired" HeaderStyle-HorizontalAlign="Left" HeaderText="Required?" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# Bind("ID") %>' CommandName="editregistrationquestions"
                                CausesValidation="true" Text="(edit)" OnClientClick="if (!window.confirm('This action will save the event.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deleteregistrationquestions" Text="(delete)" CausesValidation="false"
                                OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hr />
            <div style="text-align: right;">
                <asp:LinkButton runat="server" ID="lbAddRegistrationQuestion" OnClientClick="if (!window.confirm('This action will save the event.')) return false;"
                    OnClick="lbAddRegistrationQuestion_Click" Text="Add Registration Question" />
            </div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divPromoCodes">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lDiscountPromoCodes" runat="server">Discount / Promo Codes</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Label runat="server" ID="lblPromoCodeSaveFirst" Text="There are no Discounts / Promo Codes assigned to this event." />
            <asp:GridView ID="gvPromoCodes" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no Discounts / Promo Codes assigned to this event."
                OnRowCommand="gvPromoCodes_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Code" HeaderStyle-HorizontalAlign="Left" HeaderText="Code" />
                    <asp:BoundField DataField="IsActive" HeaderStyle-HorizontalAlign="Left" HeaderText="IsActive" />
                    <asp:BoundField DataField="ValidUntil" HeaderStyle-HorizontalAlign="Left" HeaderText="ValidUntil" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# Bind("ID") %>' CommandName="editpromocode"
                                CausesValidation="true" Text="(edit)" OnClientClick="if (!window.confirm('This action will save the event.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deletepromocode" Text="(delete)" CausesValidation="false" OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hr />
            <div style="text-align: right;">
                <asp:LinkButton runat="server" ID="lbAddPromoCode" OnClientClick="if (!window.confirm('This action will save the event.')) return false;"
                    OnClick="lbAddPromoCode_Click" Text="Add Discount / Promo Code" />
            </div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divInformationLinks">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lSupplementalInformtionLinks" runat="server">Supplemental Information Links</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Label runat="server" ID="lblInformationLinkSaveFirst" Text="There are no Supplemental Information Links assigned to this event." />
            <asp:GridView ID="gvInformationLinks" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no Supplemental Information Links assigned to this event."
                OnRowCommand="gvInformationLinks_RowCommand">
                <Columns>
                    <asp:BoundField DataField="DisplayOrder" HeaderStyle-HorizontalAlign="Left" HeaderText="DisplayOrder" />
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Code" HeaderStyle-HorizontalAlign="Left" HeaderText="Code" />
                    <asp:BoundField DataField="IsActive" HeaderStyle-HorizontalAlign="Left" HeaderText="IsActive" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# Bind("ID") %>' CommandName="editinformationlink"
                                CausesValidation="true" Text="(edit)" OnClientClick="if (!window.confirm('This action will save the event.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deleteinformationlink" Text="(delete)" CausesValidation="false"
                                OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hr />
            <div style="text-align: right;">
                <asp:LinkButton runat="server" ID="lbAddInformationLink" OnClientClick="if (!window.confirm('This action will save the event.')) return false;"
                    OnClick="lbAddInformationLink_Click" Text="Add Supplimental Information Link" />
            </div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divWaivedRegistrationLists">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lWaivedLists" runat="server">Waived Registration Lists</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Label runat="server" ID="lblWaivedRegistrationListSaveFirst" Text="There are no Waived Registration Lists assigned to this event." />
            <asp:GridView ID="gvWaivedRegistrationLists" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no Waived Registration Lists assigned to this event."
                OnRowCommand="gvWaivedRegistrationLists_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="MemberCount" HeaderStyle-HorizontalAlign="Left" HeaderText="MemberCount" />
                    <asp:BoundField DataField="RegisteredMemberCount" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="RegisteredMemberCount" />
                    <asp:BoundField DataField="IsActive" HeaderStyle-HorizontalAlign="Left" HeaderText="IsActive" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# Bind("ID") %>' CommandName="editwaivedregistrationlist"
                                CausesValidation="true" Text="(edit)" OnClientClick="if (!window.confirm('This action will save the event.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deletewaivedregistrationlist" Text="(delete)" CausesValidation="false"
                                OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hr />
            <div style="text-align: right;">
                <asp:LinkButton runat="server" ID="lbAddWaivedRegistrationList" OnClientClick="if (!window.confirm('This action will save the event.')) return false;"
                    OnClick="lbAddWaivedRegistrationList_Click" Text="Add Waived Registration List" />
            </div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divConfirmationEmails">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lConfirmationEmails" runat="server">Confirmation Emails</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Label runat="server" ID="lblConfirmationEmailSaveFirst" Text="There are no Confirmation Emails assigned to this event." />
            <asp:GridView ID="gvConfirmationEmails" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There are no Confirmation Emails assigned to this event." OnRowCommand="gvConfirmationEmails_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="ApplicableType" HeaderStyle-HorizontalAlign="Left" HeaderText="ApplicableType" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# Bind("ID") %>' CommandName="editconfirmationemail"
                                CausesValidation="true" Text="(edit)" OnClientClick="if (!window.confirm('This action will save the event.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deleteconfirmationemail" Text="(delete)" CausesValidation="false"
                                OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hr />
            <div style="text-align: right;">
                <asp:LinkButton runat="server" ID="lbAddConfirmationEmail" OnClientClick="if (!window.confirm('This action will save the event.')) return false;"
                    OnClick="lbAddConfirmationEmail_Click" Text="Add Confirmation Email" />
            </div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="div1">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lMerchandise" runat="server">Event Merchandise</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Label runat="server" ID="lblEventMerchandiseSaveFirst" Text="There is no Event Merchandise assigned to this event." />
            <asp:GridView ID="gvEventMerchandise" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="There is no Event Merchandise assigned to this event." OnRowCommand="gvEventMerchandise_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Code" HeaderStyle-HorizontalAlign="Left" HeaderText="Code" />
                    <asp:BoundField DataField="Price" HeaderStyle-HorizontalAlign="Left" HeaderText="Price"
                        DataFormatString="{0:C}" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnEdit" CommandArgument='<%# Bind("ID") %>' CommandName="editeventmerchandise"
                                CausesValidation="true" Text="(edit)" OnClientClick="if (!window.confirm('This action will save the event.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deleteeventmerchandise" CausesValidation="false" Text="(delete)"
                                OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hr />
            <div style="text-align: right;">
                <asp:LinkButton runat="server" ID="lbAddEventMerchandise" OnClientClick="if (!window.confirm('This action will save the event.')) return false;"
                    OnClick="lbAddEventMerchandise_Click" Text="Add Event Merchandise" />
            </div>
        </div>
    </div>
    <div class="sectionContent">
        <center>
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" />
            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"
                CausesValidation="false" />
        </center>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li runat="server" id="liEventOwnerTask" visible="false">
                <asp:HyperLink runat="server" ID="hlEventOwnerTask" /></li>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
