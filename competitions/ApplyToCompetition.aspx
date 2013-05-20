<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ApplyToCompetition.aspx.cs" Inherits="competitions_ApplyToCompetition" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Competition Entry - Select Fee
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <asp:Label runat="server" ID="lblExistingEntries" CssClass="redHighlight" Visible="false" /><br />
    <asp:Label runat="server" ID="lblDraftEntries" CssClass="redHighlight" Visible="false" /> <asp:HyperLink runat="server" ID="hlViewMyCompetitionEntries" Text="Click here to view." NavigateUrl="~/competitions/ViewMyCompetitionEntries.aspx" Visible="false" />

    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
            <asp:Literal ID="lSelectFee" runat="server">
                Select a Entry Fee
                </asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td>
                        <asp:RadioButtonList runat="server" ID="rblEntryFees" DataTextField="ProductName" DataValueField="ProductID">
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator runat="server" ID="rfvEntryFees" ControlToValidate="rblEntryFees" Display="None" ErrorMessage="Please select an entry fee." />
                        <asp:Label CssClass="redHighlight" ID="lblNoEntryFees" runat="server" Visible="false">No entry fees are available for you – you may not be eligible to enter this competition.</asp:Label>
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
