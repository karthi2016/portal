<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="DonationComplete.aspx.cs" Inherits="donations_DonationComplete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Donation #<%=targetOrder.LocalID %>
    Completed Successfully
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    Donation #<%=targetOrder.LocalID%>
    has been processed successfully. Thank you for your support!
    <%
        if (!string.IsNullOrWhiteSpace(targetOrder.BillingEmailAddress))%>
    A confirmation email has been sent to <%=targetOrder.BillingEmailAddress%>.
    <asp:Literal ID="PageText" runat="server"/>

    <div class="section" style="margin-top: 10px" id="divTasks" runat="server">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li>
                    <asp:HyperLink ID="hlViewOrder" runat="server">View Donation</asp:HyperLink></li>
                <li>
                    <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
