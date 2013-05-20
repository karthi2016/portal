<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewCEUCreditHistory.aspx.cs" Inherits="continuingeducations_ViewCEUCreditHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>View CEU Credit History</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View CEU Credit History
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server">
Your CEU credits are listed below. Note that you may only edit/delete CEU credits that have been self reported <i>and</i>
that have not yet been verified.</asp:Literal>
<br /><br />
    <asp:GridView ID="gvCredits" AutoGenerateColumns="false" GridLines="None" AlternatingRowStyle-CssClass="even"
     OnRowDataBound="gvCredits_RowDataBound" OnRowCommand="gvCredits_Command" DataKeyNames="ID"
        EmptyDataText="No CEU credits are linked to your account." runat="server">
        <Columns>
            <asp:BoundField DataField="CreditDate" DataFormatString="{0:d}" HeaderText="Date"
                HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="ComponentRegistration.Component.Name" HeaderText="Component" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Type.Name" HeaderText="Type" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Quantity" DataFormatString="{0:N1}" HeaderText="Quantity"
                HeaderStyle-HorizontalAlign="Left" />
            
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="hlEdit" runat="server" Text="(edit)" />
                    <asp:LinkButton ID="lbDelete" runat="server" CommandName="Delete" Text="(delete)" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <hr />
    <div style="text-align: center">
        <asp:Button ID="btnReport" runat="server" CausesValidation="false" Text="Report CEU Credits"
            OnClick="btnReport_Click" />
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Go Home"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
