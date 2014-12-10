<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="CreateFormInstance.aspx.cs" Inherits="forms_CreateFormInstance" %>

<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <%=targetForm.Name  %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:HiddenField ID="CurrentFormID" runat="server"/>
    <asp:Literal ID="lPageText" runat="server" />
    <asp:Wizard ID="wzEnterInfo" runat="server" NavigationStyle-HorizontalAlign="Center" StartNextButtonText="Continue &amp; Confirm" 
        DisplayCancelButton="True"
        OnCancelButtonClick="wzEnterInfo_OnCancelButtonClick"
        StepNextButtonText="Save Changes" FinishCompleteButtonType="Link"
        FinishCompleteButtonText="Go Home" FinishDestinationPageUrl="/"
        OnActiveStepChanged="wzEnterInfo_StepChanged">
        <WizardSteps>
            <asp:WizardStep>
                <br />
                <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" />
                <hr />
            </asp:WizardStep >
             <asp:WizardStep  AllowReturn="false">
             <%=targetForm.ConfirmationInstructions  %>
                <br />
                <uc1:CustomFieldSet ID="CustomFieldSet2" EditMode="false" runat="server" />
                <hr />
            </asp:WizardStep>
              <asp:WizardStep >
             <%= targetForm.PostSubmissionInstructions ?? "Your form submission has been received." %>
                <br />
                
                <hr />
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
