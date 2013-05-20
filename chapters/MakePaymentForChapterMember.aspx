<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="MakePaymentForChapterMember.aspx.cs" Inherits="chapters_MakePaymentForChapterMember" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="/chapters/ViewChapter.aspx?contextID=<%=targetChapter.ID %>"><%=targetChapter.Name %> ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Make a Payment for a Member
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                Select a <%= targetChapter.Name %> Member</h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvMembers" runat="server" GridLines="None" AutoGenerateColumns="false"   EmptyDataText="No chapter members found." >
                <Columns>
                    <asp:BoundField DataField="Membership.Owner.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Membership.Owner.LocalID" HeaderStyle-HorizontalAlign="Left" HeaderText="ID" />
                    <asp:BoundField DataField="Membership.Owner.Invoices_OpenInvoiceCount" HeaderStyle-HorizontalAlign="Left" HeaderText="# Open Invoices" />
                    <asp:HyperLinkField DataNavigateUrlFields="Membership.Owner.ID" DataNavigateUrlFormatString="~/financial/MakePayment.aspx?contextID={0}" Text="(select)" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li><a href="ViewChapter.aspx?contextID=<%=ContextID %>"><asp:Literal ID="lBackToChapter" runat="server" >Back to Chapter</asp:Literal> </a></li>
                <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>

