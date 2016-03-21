<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="AccountHistory.aspx.cs" Inherits="financial_AccountHistory" %>
 

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Account History for <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <asp:GridView ID="gvTransactions" AutoGenerateColumns="false" GridLines="None"  AlternatingRowStyle-CssClass="even"
        runat="server" onrowdatabound="gvTransactions_RowDataBound">
    <Columns>
    <asp:BoundField DataField="Date" DataFormatString="{0:d}" HeaderText="Date"  HeaderStyle-HorizontalAlign="Left" />
    <asp:BoundField DataField="Name" HeaderText="Name"  HeaderStyle-HorizontalAlign="Left"  />
    <asp:BoundField DataField="Memo" HeaderText="Memo"  HeaderStyle-HorizontalAlign="Left"  />
    <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total"  HeaderStyle-HorizontalAlign="Left"    />
    <asp:TemplateField >
    <ItemTemplate>
    <asp:HyperLink ID="hlView" runat="server" Text="(view)" />
    </ItemTemplate>
    </asp:TemplateField>
    </Columns>
    </asp:GridView>
    <asp:Label ID="lblNoTransactions" runat="server" Font-Italic="true">
    You have no transactions on file to review.</asp:Label>
    <hr />
    <div style="text-align: center">
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Go Home"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
