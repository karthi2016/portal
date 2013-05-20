<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageContacts.aspx.cs" MasterPageFile="~/App_Master/GeneralPage.master"
    Inherits="profile_ManageContacts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Manage Contacts for
    <%= targetOrganization.Name %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lCurrentContacts" runat="server">Current Contacts</asp:Literal></h2>
        </div>
        <asp:Literal ID="PageText" runat="server"/>
        <div class="sectionContent">
            <asp:GridView ID="gvCurrentRelationships" runat="server" GridLines="None" AutoGenerateColumns="false"
                OnRowCommand="gvRelationships_RowCommand" OnRowDataBound="gvRelationships_RowDataBound"
                EmptyDataText="There are currently no contacts tied to this organization.">
                <Columns>
                    <asp:BoundField DataField="RightSide_Individual.LocalID" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="ID" />
                    <asp:BoundField DataField="Target_Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="RightSide_Individual.EmailAddress" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="Email Address" />
                    <asp:BoundField DataField="RelationshipTypeName" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="Relationship" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\profile\EditIndividualInfo.aspx?contextID={0}"
                        DataNavigateUrlFields="Target_ID" Text="(edit)" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\profile\ChangeRelationship.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(change relationship)" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lbDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deleterelationship" CausesValidation="false" Text="(delete)" OnClientClick="if (!window.confirm('Are you sure you want to unlink this person from your organization? DO NOT DO THIS if the person has simply left the organization – if the person has left the organization, update the relationship with an end date.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divPastRelationships">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lPastContacts" runat="server">Past Contacts</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvPastRelationships" runat="server" GridLines="None" AutoGenerateColumns="false"
                OnRowCommand="gvRelationships_RowCommand" OnRowDataBound="gvRelationships_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="RightSide_Individual.LocalID" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="ID" />
                    <asp:BoundField DataField="Target_Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <asp:BoundField DataField="RightSide_Individual.EmailAddress" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="Email Address" />
                    <asp:BoundField DataField="RelationshipTypeName" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="Relationship" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\profile\EditIndividualInfo.aspx?contextID={0}"
                        DataNavigateUrlFields="Target_ID" Text="(edit)" />
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\profile\ChangeRelationship.aspx?contextID={0}"
                        DataNavigateUrlFields="ID" Text="(change relationship)" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lbDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deleterelationship" CausesValidation="false" Text="(delete)" OnClientClick="if (!window.confirm('Are you sure you want to unlink this person from your organization? DO NOT DO THIS if the person has simply left the organization – if the person has left the organization, update the relationship with an end date.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
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
                <li><a href="/profile/AddContact.aspx?contextID=<%=targetOrganization.ID %>"> <asp:Literal ID="lAddContact" runat="server">Add a Contact</asp:Literal></a></li>
                <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
