<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewOpenJobs.aspx.cs" Inherits="volunteers_ViewOpenJobs" %>
        <%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Available Volunteer Jobs
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server">Below are the open and available jobs for which we need volunteers.</asp:Literal>
    <div style="padding-top: 10px">
        <asp:GridView ID="gvJobs" runat="server" EmptyDataText="There are currently no open jobs"
            GridLines="None" AutoGenerateColumns="false" HeaderStyle-HorizontalAlign="Left"
             OnRowDataBound="gvJobs_RowDataBound"
            >
            <Columns>
                <asp:BoundField DataField="Job.Name" HeaderText="Job" />
                <asp:BoundField DataField="Location.Name" HeaderText="Location" />
                <asp:BoundField DataField="StartDateTime" HeaderText="Start" />
                <asp:BoundField DataField="EndDateTime" HeaderText="End" />
                <asp:TemplateField>
                    <ItemTemplate>
                    <asp:HyperLink ID="hlMoreInfo" NavigateUrl="#" runat="server">(hover for more info)</asp:HyperLink>
           
                        <telerik:RadToolTip ID="rttDetails" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <hr />
    <div align="center">
        <asp:Button ID="btnGoToMyProfile" Text="Back to My Volunteer Profile" runat="server"
            Visible="false" OnClick="btnGoToMyProfile_Click" />
        <asp:Button ID="btnGoHome" Text="Go Home" runat="server" Visible="false" OnClick="btnGoHome_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
