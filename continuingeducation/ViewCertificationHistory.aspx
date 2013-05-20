<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewCertificationHistory.aspx.cs" MasterPageFile="~/App_Master/GeneralPage.master" Inherits="continuingeducation_ViewCertificationHistory" %>
 

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>View Certification History</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View My Certification History
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">

 <asp:Literal ID="PageText" runat="server"/>
    <asp:GridView ID="gvCredits" AutoGenerateColumns="false" GridLines="None" AlternatingRowStyle-CssClass="even"
    
       DataKeyNames="ID"
        EmptyDataText="No certifications are linked to your account." runat="server">
        <Columns>
            <asp:BoundField DataField="Program.Name" HeaderText="Program"
                HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="Status.Name" HeaderText="Status"
                HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="EffectiveDate" HeaderText="Effective Date" DataFormatString="{0:d}"
                HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="ExpirationDate" HeaderText="Expiration Date" DataFormatString="{0:d}"
                HeaderStyle-HorizontalAlign="Left" />
                <asp:HyperLinkField DataNavigateUrlFormatString="ViewCertification.aspx?contextID={0}" DataNavigateUrlFields="ID" Text="(view)" />
            
        </Columns>
    </asp:GridView>
    <hr />
    <div style="text-align: center">
        
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Go Home"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
