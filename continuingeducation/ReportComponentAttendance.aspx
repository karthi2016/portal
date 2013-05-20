<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ReportComponentAttendance.aspx.cs" Inherits="continuingeducation_ReportComponentAttendance" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Report Component Attendance
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server" />
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="We cannot continue for the following reasons:"
        DisplayMode="BulletList" ForeColor="Red" Font-Bold="true" ShowMessageBox="false"
        ValidationGroup="Custom" ShowSummary="true" />
    <asp:Wizard runat="server" OnNextButtonClick="wzComponent_Click" ID="wzComponent" OnFinishButtonClick="wzComponent_OnFinishButtonClick">
        <FinishNavigationTemplate>
            <asp:Button ID="FinishPreviousButton" runat="server" CausesValidation="False" 
                CommandName="MovePrevious" Text="Previous" />
            <asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" 
                Text="Finish" />
        </FinishNavigationTemplate>
        <WizardSteps>
            <asp:WizardStep>
                <asp:Literal ID="lPage1Text" runat="server">
<h2>Select a Component</h2>
First, we need to know what component you are registering for. Enter in the code or ID number of the component.
                </asp:Literal>
                <asp:CustomValidator ID="cvComponentNotFound" ValidationGroup="Custom" runat="server"
                    Display="None" ErrorMessage="We were unable to find a component with the specific ID or code."
                    ForeColor="Red" Font-Bold="true" />
                <asp:CustomValidator ID="cvDuplicateRegistration" ValidationGroup="Custom" runat="server"
                    Display="None" ErrorMessage="We have already recorded a registration for the specified component - duplicate registrations are not allowed."
                    ForeColor="Red" Font-Bold="true" />
                <table style="margin-top: 10px">
                    <tr>
                        <td class="columnHeader" style="width: 200px">
                            Component ID/Code: <span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbComponentID" runat="server" Width="60" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="tbComponentID" ErrorMessage="Please enter a component ID or code"
                                Display="None" />
                        </td>
                    </tr>
                </table>
            </asp:WizardStep>
            <asp:WizardStep>
                <asp:Literal ID="lConfirmComponent" runat="server">
<h2>Confirm Component Information</h2>
We've located the component you specified. <font color=red>Please confirm that this is correct.</font>
                </asp:Literal>
                <table style="margin-top: 10px">
                    <tr>
                        <td class="columnHeader" style="width: 200px">
                            <asp:Literal ID="lComponentID" runat="server">Component ID:</asp:Literal>
                            <asp:HiddenField ID="hfComponentID" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblComponentID" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader" style="width: 200px">
                            <asp:Literal ID="lComponentCode" runat="server"> Component Code:</asp:Literal>
                        </td>
                        <td>
                            <asp:Label ID="lblComponentCode" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader" style="width: 200px">
                            <asp:Literal ID="lComponentName" runat="server">Component Name:</asp:Literal>
                        </td>
                        <td>
                            <asp:Label ID="lblComponentName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader" style="width: 200px">
                            <asp:Literal ID="lComponentDate" runat="server">Date:</asp:Literal>
                        </td>
                        <td>
                            <asp:Label ID="lblDate" runat="server" />
                        </td>
                    </tr>
                      <tr>
                        <td class="columnHeader" style="width: 200px">
                            <asp:Literal ID="lComponentLocation" runat="server">Location:</asp:Literal>
                        </td>
                        <td>
                            <asp:Label ID="lblComponentLocation" runat="server" >n/a</asp:Label>
                        </td>
                    </tr>
                </table>
            
                <h2>
                    <asp:Literal ID="lComponentDescription" runat="server">Description</asp:Literal></h2>
                <asp:Literal ID="lDescription" runat="server" Text="No description is available for this component." />

                    <h2>Attendance Information</h2>
                <table>
                 <tr>
                        <td class="columnHeader" style="width: 200px">
                            <asp:Literal ID="lDateofAttendance" runat="server">Date of Attendance:</asp:Literal>
                        </td>
                        <td>
                             <telerik:RadDatePicker ID="dpDateOfAttendance" runat="server" />
                             <asp:RequiredFieldValidator runat="server" ControlToValidate="dpDateOfAttendance" ErrorMessage="You must enter a date of attendance." Display=None />

                        </td>
                    </tr>
                </table>
                <br />
                <asp:Panel ID="pnlAttendance" runat="server">
                   
                    <asp:Literal ID="lAttendeanceText" runat="server">Please certifiy that you have completed this certification component.</asp:Literal>
                    <div style="margin-top: 10px">
                        <asp:RadioButton ID="rbFull" Checked="true" runat="server" GroupName="Attendance" onclick="showPercentageSpan( false )"
                            Text="<B>Full Participation </B>- I've completed 100% of the educational program" />
                        <br />
                        <asp:RadioButton ID="rbPartial" runat="server" GroupName="Attendance" onclick="showPercentageSpan( true )" Text="<B>Partial Participation </B>- I've completed only a portion of the program. Credits will be recorded proportionately." /><br />
                        <span id="spanPercentage" style="padding-left: 20px; display: none"><b>Percentage of Program Complete:</b> <span class="requiredField">
                            *</span>
                            <asp:TextBox ID="tbPercentage" Width="30" Text="50" runat="server" />
                            <asp:RangeValidator runat="server" ControlToValidate="tbPercentage" Type="Integer" Display="None" MinimumValue="1" MaximumValue="100"
                             ErrorMessage="Please enter a valid range between 1-100" />
                        </span>
                    </div>
                </asp:Panel>
                <asp:Literal ID="lDigitalSignatureHeader" runat="server"><h2>Digital Signature</h2></asp:Literal>
                <asp:Literal ID="lDigSig" runat="server">Please enter your full name, in uppercase, below to certify program registration.</asp:Literal>
                <table style="margin-top: 10px">
                    <tr>
                        <td class="columnHeader" style="width: 200px">
                            Digital Signature: <span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbDigitalSignature" runat="server" Width="260" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbDigitalSignature"
                                ErrorMessage="Please enter your digital signature" Display="None" />
                        </td>
                    </tr>
                </table>
                <script type="text/javascript">
                    function showPercentageSpan(doShow) {
                        span = document.getElementById('spanPercentage');

                        if (doShow)
                            span.style.display = '';
                         else
                             span.style.display = 'none';
                    }
                </script>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
