<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewExhibitorListing.aspx.cs" Inherits="exhibits_ViewExhibitorListing" %>
    <%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink ID="hlEvent1" runat="server" Visible="true" NavigateUrl="/events/ViewEvent.aspx?contextID=" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />

     <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lExhibitorBooths" runat="server">Current Exhibitor Listing</asp:Literal></h2>
        </div>
    <table style="margin-top: 20px">
        <asp:Repeater ID="rptExhibitors" runat="server" OnItemDataBound="rptExhibitors_DataBound">
            <ItemTemplate>
                <tr valign="middle">
                    <td>
                        <asp:Image ID="imgLogo" runat="server" Visible="false" Height="30px" />
                    </td>
                    <td>
                        <asp:Label ID="lblExhibitorName" runat="server" />
                        <asp:HyperLink id="hlExhibitorDescription" Visible="false" runat='server' Text="(hover to view bio)" NavigateUrl="#" />
                        <telerik:RadToolTip ID="rppExhibitorBio" runat="server" TargetControlID="hlExhibitorDescription" />
                    </td>
                    <td>
                        <asp:Label ID="lblBooths" runat="server" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    <asp:Literal ID="lNoExhibitors" runat="server">
  No exhibitors have registered yet.</asp:Literal>
  </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lEventTasks" runat="server">Exhibit Show Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <li><a href="ViewShow.aspx?contextID=<%=targetShow.ID %>">Back to
                    <%=targetShow.Name %>
                    Home Page</a></li>
                <li><a href="/">
                    <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
