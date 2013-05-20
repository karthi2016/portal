<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewMyCommittees.aspx.cs" Inherits="committees_ViewMyCommittees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View My Committees
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
            <asp:Literal ID="lCurrentCommitteeMembership" runat="server">
            Current Committee Membership
            </asp:Literal>
            </h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvCurrentCommitteeMembership" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="You are not on any committees.">
                <Columns>
                    <asp:BoundField DataField="Committee.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Term.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Term" />
                    <asp:BoundField DataField="Position.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Position" />
                    <asp:BoundField DataField="EffectiveStartDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Start" />
                    <asp:BoundField DataField="EffectiveEndDate" HeaderStyle-HorizontalAlign="Left" HeaderText="End" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\committees\ViewCommittee.aspx?contextID={0}" DataNavigateUrlFields="Committee.ID" Text="(view committee)" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
            <asp:Literal ID="lPastCommitteeMemberships" runat="server">
            Past Committee Membership</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvPastCommitteeMembership" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="No past committeed memberships found.">
                <Columns>
                    <asp:BoundField DataField="Committee.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="Term.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Term" />
                    <asp:BoundField DataField="Position.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Position" />
                    <asp:BoundField DataField="EffectiveStartDate" HeaderStyle-HorizontalAlign="Left" HeaderText="From" />
                    <asp:BoundField DataField="EffectiveEndDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Until" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\committees\ViewCommittee.aspx?contextID={0}" DataNavigateUrlFields="Committee.ID" Text="(view committee)" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
      <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
       
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
