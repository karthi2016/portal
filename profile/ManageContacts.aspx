<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageContacts.aspx.cs" MasterPageFile="~/App_Master/GeneralPage.master"
    Inherits="profile_ManageContacts" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Manage Contacts for <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <telerik:RadAjaxManager ID="AjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="gvCurrentRelationships">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gvCurrentRelationships"/>
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="gvPastRelationships">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gvPastRelationships"/>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>        
    </telerik:RadAjaxManager>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lCurrentContacts" runat="server">Current Contacts</asp:Literal></h2>
        </div>
        <asp:Literal ID="PageText" runat="server"/>
        <div class="sectionContent">
            <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">                           
            <telerik:RadGrid ID="gvCurrentRelationships" runat="server" GridLines="None" OnNeedDataSource="CurrentContactsNeedDataSource" PageSize="10" AllowAutomaticDeletes="True"
                OnItemDataBound="ContactRowBound" OnDeleteCommand="DeleteCurrentContact">
               <PagerStyle AlwaysVisible="True"/>
               <MasterTableView AllowPaging="True" AllowCustomPaging="True" AutoGenerateColumns="False" ItemStyle-VerticalAlign="Top" 
                    AlternatingItemStyle-VerticalAlign="Top" ShowFooter="true" EnableViewState="True">
                    <Columns>
                       <telerik:GridBoundColumn DataField="RightSide_Individual.LocalID" HeaderStyle-HorizontalAlign="Left" HeaderText="ID"/>
                       <telerik:GridBoundColumn DataField="Target_NAme" HeaderStyle-HorizontalAlign="Left" HeaderText="Name"/>    
                       <telerik:GridBoundColumn DataField="RightSide_Individual.EmailAddress" HeaderStyle-HorizontalAlign="Left" HeaderText="Email Address" />
                       <telerik:GridBoundColumn DataField="RelationshipTypeName" HeaderStyle-HorizontalAlign="Left" HeaderText="Relationship"/>
                       <telerik:GridHyperLinkColumn DataNavigateUrlFormatString="~\profile\EditIndividualInfo.aspx?contextID={0}" DataNavigateUrlFields="Target_ID" Text="(edit)"/>
                       <telerik:GridHyperLinkColumn DataNavigateUrlFormatString="~\profile\ChangeRelationship.aspx?contextID={0}" DataNavigateUrlFields="ID" Text="(change relationship)" />
                       <telerik:GridTemplateColumn>
                         <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lbDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="Delete" CausesValidation="false" Text="(delete)" OnClientClick="if (!window.confirm($('#DeleteMsg').html())) return false;" />
                         </ItemTemplate>                           
                       </telerik:GridTemplateColumn>
                   </Columns>
               </MasterTableView> 
            </telerik:RadGrid>
            </telerik:RadCodeBlock>
        </div>
    </div>
    <div style="display:none" id="DeleteMsg"><asp:Literal runat="server" ID="lDeleteMessage">Are you sure you want to unlink this person from your organization? DO NOT DO THIS if the person has simply left the organization – if the person has left the organization, update the relationship with an end date.</asp:Literal></div>
    <div class="section" style="margin-top: 10px" runat="server" id="divPastRelationships">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lPastContacts" runat="server">Past Contacts</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">                           
            <telerik:RadGrid ID="gvPastRelationships" runat="server" GridLines="None" OnNeedDataSource="PastContactsNeedDataSource" PageSize="5" AllowAutomaticDeletes="True"
                OnItemDataBound="ContactRowBound" OnDeleteCommand="DeleteCurrentContact">
               <PagerStyle AlwaysVisible="True"/>
               <MasterTableView AllowPaging="True" AllowCustomPaging="True" AutoGenerateColumns="False" ItemStyle-VerticalAlign="Top" AlternatingItemStyle-VerticalAlign="Top" ShowFooter="true" EnableViewState="True">
                 <Columns>
                    <telerik:GridBoundColumn DataField="RightSide_Individual.LocalID" HeaderStyle-HorizontalAlign="Left" HeaderText="ID" />
                    <telerik:GridBoundColumn DataField="Target_Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                    <telerik:GridBoundColumn DataField="RightSide_Individual.EmailAddress" HeaderStyle-HorizontalAlign="Left" HeaderText="Email Address" />
                    <telerik:GridBoundColumn DataField="RelationshipTypeName" HeaderStyle-HorizontalAlign="Left" HeaderText="Relationship" />
                    <telerik:GridHyperLinkColumn DataNavigateUrlFormatString="~\profile\EditIndividualInfo.aspx?contextID={0}" DataNavigateUrlFields="Target_ID" Text="(edit)" />
                    <telerik:GridHyperLinkColumn DataNavigateUrlFormatString="~\profile\ChangeRelationship.aspx?contextID={0}" DataNavigateUrlFields="ID" Text="(change relationship)" />
                    <telerik:GridTemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lbDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="Delete" CausesValidation="false" Text="(delete)" OnClientClick="if (!window.confirm('Are you sure you want to unlink this person from your organization? DO NOT DO THIS if the person has simply left the organization – if the person has left the organization, update the relationship with an end date.')) return false;" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
              </MasterTableView>
            </telerik:RadGrid>
            </telerik:RadCodeBlock>
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
