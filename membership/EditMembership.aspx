<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="EditMembership.aspx.cs" Inherits="membership_EditMembership" %>
<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    
    <asp:HyperLink ID="hlViewMembership" runat="server" NavigateUrl="ViewMembership.aspx">View Membership &gt;</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Edit Membership
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<asp:Literal ID="PageText" runat="server" />
 <div class="section" id="divOtherInformation" runat="server">
        <div class="sectHeaderTitle">
            <h2>
               <asp:Literal ID="lMembershipInfoHeader" runat="server">Membership Information:</asp:Literal>
            </h2>
        </div>
        <asp:CheckBox ID="cbMembershipDirectoryOptOut" runat="server" Text="I would like to opt out of the membership directory" /><br />
        <asp:CheckBox ID="cbAutomaticallyPay" Checked=true runat="server" Text="Please charge my credit card and automatically renew my membership when it expires" />
        </div>

        <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" />
    <p />
    <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" OnClick="btnSave_Click" runat="server" Text="Save" />
        or <asp:HyperLink ID="hlViewMembership2" runat="server">cancel</asp:HyperLink>
        
    </div>

</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

