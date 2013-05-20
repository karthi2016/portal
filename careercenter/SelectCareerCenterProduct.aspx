<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="SelectCareerCenterProduct.aspx.cs" Inherits="careercenter_SelectCareerCenterProduct" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Purchase Job Postings - Select Product
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lSelectAProduct" runat="server">Select a Job Posting Product</asp:Literal></h2>
        </div>
         <asp:Literal ID="PageText" runat="server" />
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="redHighlight">
                        You currently have <%=numberOfPostingsAvailable %> job postings available.
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButtonList runat="server" ID="rblCareerCenterProducts" DataTextField="ProductName" DataValueField="ProductID">
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator runat="server" ID="rfvCareerCenterProducts" ControlToValidate="rblCareerCenterProducts" Display="None" ErrorMessage="Please select a Job Posting Product." />
                        <asp:Label CssClass="redHighlight" ID="lblNoCareerCenterProducts" runat="server" Visible="false">Unfortunately, there are no Job Posting Products available at this time. Please check back later.</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button runat="server" ID="btnContinue" OnClick="btnContinue_Click" CausesValidation="true" Text="Continue" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>