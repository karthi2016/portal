<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="SubmitRecommendation.aspx.cs" Inherits="recommendations_SubmitRecommendation" %>
<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
Submit a Recommendation
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<asp:Literal ID="lPageText" runat="server" />
<table style="width: 300px">
<tr>
<td  style="width: 150px" class="columnHeader">Recommendation for:</td>
<td><%=target.Name  %></td>
</tr>
</table>
    <asp:Wizard ID="wzEnterInfo" runat="server" NavigationStyle-HorizontalAlign="Center" StartNextButtonText="Continue &amp; Confirm" 
     StepNextButtonText="Submit Recommendation" FinishCompleteButtonType="Link"
     FinishCompleteButtonText="Go Home" FinishDestinationPageUrl="/"
     OnActiveStepChanged="wzEnterInfo_StepChanged"  >
        <WizardSteps>
            <asp:WizardStep>
            <h2>Enter Recommendation Info</h2>
                <asp:Literal ID="lRecInfo" runat="server">Please fill in the information below. You will have an opportunity to review your answers before submission.</asp:Literal>
                <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" />
                <hr />
            </asp:WizardStep >
             <asp:WizardStep  >
           <asp:Literal ID="lReview" runat="server">Please review your answers below. Once you click <b>Submit Recommendation</b>, your recommendation will be submitted.</asp:Literal>
                <uc1:CustomFieldSet ID="CustomFieldSet2" EditMode="false" runat="server" />
                <hr />
            </asp:WizardStep>
              <asp:WizardStep AllowReturn="false" >
           <asp:Literal ID="lRecommendationSuccessful" runat="server">
           Your recommendation has been submitted successfully.  
           </asp:Literal>
                
                <hr />
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

