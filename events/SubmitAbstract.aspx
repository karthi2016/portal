<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="SubmitAbstract.aspx.cs" Inherits="events_SubmitAbstract" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name%></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <asp:Wizard ID="Wizard1" runat="server" DisplaySideBar="false" FinishCompleteButtonText="Submit Abstract"
        OnFinishButtonClick="Wizard1_FinishButtonClick" OnNextButtonClick="Wizard1_NextButtonClick">
        <WizardSteps>
            <asp:WizardStep runat="server" Title="Step 1">
                <p>
                    <asp:Literal ID="lPleaseEnterSubmission" runat="server">Please enter your abstract submission information below.</asp:Literal>
                </p>
                <table style="width: 500px">
                    <tr>
                        <td class="columnHeader">
                             <asp:Literal ID="lName" runat="server">Name:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbName" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="None"
                                ControlToValidate="tbName" ErrorMessage="Please enter an abstract name." />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader" colspan="2">
                            <br />
                             <asp:Literal ID="lAbstractSummary" runat="server">Abstract Summary/Description:</asp:Literal><span class="requiredField">*</span>
                            <asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine" Rows="10" Columns="100" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="None"
                                ControlToValidate="tbDescription" ErrorMessage="Please enter an abstract summary/description." />
                        </td>
                    </tr>
                    <tr id="trTracks" runat="server">
                        <td colspan="2" class="columnHeader">
                            <br />
                            <asp:Literal ID="lTracks" runat="server"> Under what tracks should this abstract be considered?</asp:Literal>
                             
                             <span class="requiredField">*</span> 
                            <br />
                            <asp:CheckBoxList ID="cblTracks" RepeatColumns="2" runat="server" />
                        </td>
                    </tr>
                </table>
                <h2>
                     <asp:Literal ID="lPresenterInfoHeader" runat="server">Presenter Information</asp:Literal></h2>
                <table style="width: 500px">
                    <tr id="trPresenterName" runat="server">
                        <td class="columnHeader">
                             <asp:Literal ID="lPresenterName" runat="server">Presenter Name:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbPresenterName" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="None"
                                ControlToValidate="tbPresenterName" ErrorMessage="Please enter the name of the presenter." />
                        </td>
                    </tr>
                  
                    <tr id="trPresenterEmail" runat="server">
                        <td class="columnHeader">
                             <asp:Literal ID="lPresenterEmail" runat="server">Presenter E-Mail:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbPresenterEmail" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="None"
                                ControlToValidate="tbPresenterEmail" ErrorMessage="Please enter an email address for the presenter." />
                        </td>
                    </tr>
                    <tr id="trPresenterContactNumber" runat="server">
                        <td class="columnHeader">
                             <asp:Literal ID="lPresenterContact" runat="server">Presenter Contact #:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="tbPresenterPhone" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Display="None"
                                ControlToValidate="tbPresenterPhone" ErrorMessage="Please enter a contact number for the presenter." />
                        </td>
                    </tr>
                    <tr id="trPresenterBio" runat="server">
                        <td class="columnHeader" colspan="2">
                            <br />
                             <asp:Literal ID="lPresenterBio" runat="server">Presenter Bio:</asp:Literal><span class="requiredField">*</span>
                            <asp:TextBox ID="tbPresenterBio" runat="server" TextMode="MultiLine" Rows="10" Columns="100" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Display="None"
                                ControlToValidate="tbPresenterBio" ErrorMessage="Please enter a bio for the presenter." />
                        </td>
                    </tr>
                </table>
                <hr width="100%" />
                <div runat="server" id="divAdditionalInfo">
                   <table width="100%">
                        <tr>
                            <td align=left>
                                <uc1:CustomFieldSet ID="cfsAbstractCustomFields" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="WizardStep1" runat="server" Title="Step 2">
              <asp:Literal ID="lConfirmAbstractNotComplete" runat="server">
                    <span class="hlteWarn">Your abstract submission is not complete!</span> Please review
                    the details and click <b>Submit Abstract</b> below to submit your abstract.
             </asp:Literal>
                <table style="width: 500px">
                    <tr>
                        <td class="columnHeader">
                             <asp:Literal ID="lConfirmName" runat="server">Name:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:Label ID="lblName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader" colspan="2">
                            <br />
                             <asp:Literal ID="lConfirmAbstractSummaryDescription" runat="server">Abstract Summary/Description:</asp:Literal><span class="requiredField">*</span>
                            <br />
                            <asp:Label ID="lblDescription" runat="server" />
                        </td>
                    </tr>
                    <tr id="trSessionTracksConfirm" runat="server">
                        <td colspan="2" class="columnHeader">
                            <br />
                             <asp:Literal ID="Literal12" runat="server">
                             Tracks</asp:Literal><span class="requiredField">*</span>
                            <br />
                            <asp:Label ID="lblTracks" runat="server" />
                        </td>
                    </tr>
                </table>
                <h2>
                     <asp:Literal ID="lConfirmPresenterInfo" runat="server">Presenter Information</asp:Literal></h2>
                <table style="width: 500px">
                    <tr>
                        <td class="columnHeader">
                             <asp:Literal ID="lConfirmPresenterName" runat="server">Presenter Name:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:Label ID="lblPresenterName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                             <asp:Literal ID="lConfirmSubmitterName" runat="server">Submitter Name:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:Label ID="lblSubmitterName" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                             <asp:Literal ID="lConfirmPresenterEmail" runat="server">Presenter E-Mail:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:Label ID="lblPresenterEmail" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                             <asp:Literal ID="lConfirmPresenterContactNumber" runat="server">Presenter Contact #:</asp:Literal><span class="requiredField">*</span>
                        </td>
                        <td>
                            <asp:Label ID="lblPresenterContactNumber" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader" colspan="2">
                            <br />
                             <asp:Literal ID="lConfirmPresenterBio" runat="server">Presenter Bio:</asp:Literal><span class="requiredField">*</span>
                            <br />
                            <asp:Label ID="lblPresenterBio" runat="server" />
                        </td>
                    </tr>
                </table>
                <hr width="100%" />
                <div runat="server" id="divAdditionalInfoConfirm">
                    <table width="100%">
                        <tr>
                            <td align=left>
                                <uc1:CustomFieldSet ID="cfsAbstractCustomFieldsConfirm" EditMode="false" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
