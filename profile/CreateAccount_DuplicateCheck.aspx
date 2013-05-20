<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="CreateAccount_DuplicateCheck.aspx.cs" Inherits="profile_CreateAccount_DuplicateCheck" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="Server">
    Create Account
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lPotentialMatches" runat="server">Potential Matches</asp:Literal></h2>
        </div>
        <p>
            <asp:Literal ID="PageText" runat="server">We have located some existing records in our system that may be you.  If you are any of the people listed below please click the (select) link for your record.  If you are not listed click the New Account button at the bottom of the screen.</asp:Literal></p>
        <asp:GridView ID="gvDuplicates" runat="server" GridLines="None" AutoGenerateColumns="false"
            OnRowCommand="gvDuplicates_RowCommand">
            <Columns>
                <asp:BoundField DataField="FirstName" HeaderStyle-HorizontalAlign="Left" HeaderText="FirstName" />
                <asp:BoundField DataField="LastName" HeaderStyle-HorizontalAlign="Left" HeaderText="LastName" />
                <asp:BoundField DataField="AddressClean" HeaderStyle-HorizontalAlign="Left" HeaderText="Address" />
                <asp:BoundField DataField="_Preferred_Address_City" HeaderStyle-HorizontalAlign="Left"
                    HeaderText="City" />
                <asp:BoundField DataField="_Preferred_Address_PostalCode" HeaderStyle-HorizontalAlign="Left"
                    HeaderText="PostalCode" />
                <asp:BoundField DataField="EmailClean" HeaderStyle-HorizontalAlign="Left" HeaderText="EmailAddress" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lbSelect" Text="(select)" CommandName="select"
                            CommandArgument='<%# Bind("ID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
        <center>
            <asp:Button runat="server" ID="btnNewAccount" Text="New Account" OnClick="btnNewAccount_Click" /></center>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
