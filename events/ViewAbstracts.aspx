<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewAbstracts.aspx.cs" Inherits="events_ViewAbstracts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name%></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <asp:GridView ID="gvAbstracts" AutoGenerateColumns="false" GridLines="None" AlternatingRowStyle-CssClass="even"
        EmptyDataText="You have not submitted any abstracts for this event." runat="server">
        <Columns>
            <asp:BoundField DataField="LocalID" HeaderText="ID" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="PresenterName" HeaderText="Presenter Name" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Status.Name" HeaderText="Status" HeaderStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="CreatedDate" DataFormatString="{0:d}" HeaderText="Date"
                HeaderStyle-HorizontalAlign="Left" />
            <asp:HyperLinkField Text="(view)" DataNavigateUrlFormatString="ViewAbstract.aspx?contextID={0}"
                DataNavigateUrlFields="ID" />
            <asp:HyperLinkField Text="(edit)" DataNavigateUrlFormatString="SubmitAbstract.aspx?contextID={0}"
                DataNavigateUrlFields="ID" />
        </Columns>
    </asp:GridView>
    <hr />
    <div style="text-align: center">
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Back to Event"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
