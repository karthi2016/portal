<%@ Page Language="C#"  MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ViewMyCompetitionEntries.aspx.cs" Inherits="competitions_ViewMyCompetitionEntries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View My Competition Entries
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
 <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <asp:GridView ID="gvCompetitionEntries" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="You have not entered any competitions." OnRowCommand="gvCompetitionEntries_RowCommand" OnRowDataBound="gvCompetitionEntries_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Competition.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Competition" />
                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Entry Name" />
                    <asp:BoundField DataField="DateSubmitted" HeaderStyle-HorizontalAlign="Left" HeaderText="Date Submitted" />
                    <asp:BoundField DataField="Status.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Status" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lbComplete" CommandArgument='<%# DataBinder.GetPropertyValue( Container.DataItem,"ID") %>' Text="(complete)" Visible="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\competitions\ViewCompetitionEntry.aspx?contextID={0}" DataNavigateUrlFields="ID" Text="(view)" />
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

