<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ViewMyTraits.aspx.cs" Inherits="volunteers_ViewMyTraits" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="ViewMyVolunteerProfile.aspx?contextID=<%=targetVolunteer.ID %>">
       My Volunteer Profile</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal ID="lPageHeader" runat='server' />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
     <asp:GridView ID="gvTraits" runat="server" EmptyDataText="You currently do not have any traits of this type."
        GridLines="None" AutoGenerateColumns="false" HeaderStyle-HorizontalAlign="Left">
        <Columns>
            <asp:BoundField DataField="FirstSubTypeName" />
            <asp:BoundField DataField="SecondSubTypeName" />
            <asp:BoundField DataField="TextValue" />
            <asp:BoundField DataField="ExpiresOn" DataFormatString="{0:d}"  HeaderText="Expiration Date" />
            <asp:BoundField DataField="Verified" HeaderText="Verified?" />
           
        </Columns>
    </asp:GridView>
    <hr />
    <div align=center>
    <asp:Button ID="btnGoHome" Text="Back to My Volunteer Profile" runat="server" OnClick="btnGoHome_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>

