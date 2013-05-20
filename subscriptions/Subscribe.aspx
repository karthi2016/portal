<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="Subscribe.aspx.cs" Inherits="subscriptions_Subscribe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Subscribe to a Publication
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSelectaPublication" runat="server">
            Select a Subscription Plan</asp:Literal>
            </h2>
        </div>
        <div class="sectionContent">
        <asp:RadioButtonList ID="rbSubscriptionPlans" runat="server" Visible="false" />
        <asp:Literal ID="lNoSubscriptionFees" runat="server">No subscription plans are currently available.</asp:Literal>
        </div>
    </div>
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" CausesValidation="true" OnClick="btnContinue_Click"
            runat="server" Text="Continue " />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
