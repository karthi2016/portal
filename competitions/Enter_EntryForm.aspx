<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="Enter_EntryForm.aspx.cs" Inherits="competitions_Enter_EntryForm" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Competition Entry - Entry Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent" style="margin-top: 10px">
            <span class="columnHeader">
            <asp:Literal ID="lEntryName" runat="server">
            Please Give this Entry a Name: </asp:Literal> </span><span class="requiredField">
                *</span>
                
                &nbsp;
                
                <asp:TextBox runat="server" ID="tbEntryName" /><asp:RequiredFieldValidator
                    ID="rfvEntryName" runat="server" ErrorMessage="Please enter a name for this entry."
                    ControlToValidate="tbEntryName" Display="None" />
            <div class="sectionHeaderTitle">
                <h2>
                   <asp:Literal ID="lEntryForm" runat="server">
                    Entry Form
                    </asp:Literal>
                    </h2>
            </div>
            <p>
            </p>
            <uc1:CustomFieldSet ID="cfsEntryFields" runat="server" />
        </div>
        <div class="sectionContent">
            <hr width="100%" />
            <div align="center" style="padding-top: 20px">
                <asp:Button ID="btnSaveAsDraft" OnClick="btnSaveAsDraft_Click" Text="Save As Draft" runat="server" />
                <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Continue" runat="server" />
                <asp:Button ID="btnBack" OnClick="btnBack_Click" Text="Back" runat="server" />
                <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server" />
                <div class="clearBothNoSPC">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
