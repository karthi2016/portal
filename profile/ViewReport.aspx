<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewReport.aspx.cs" Inherits="profile_ViewReport" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="MyReports.aspx">My Reports</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Label ID="lblReportName" runat="server">View Report</asp:Label>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<p>
<%=targetSearch.Description  %>
</p>
    <asp:Literal ID="lPageText" runat="server" />
    <telerik:RadGrid BorderWidth="0px" EnableAjax="true" HeaderStyle-Width="100px" Width="100%"
        ID="rgMainDataGrid" runat="server" Height="100%" AllowPaging="True" AllowCustomPaging="True"
        AllowSorting="True" GridLines="None" ShowStatusBar="True" OnNeedDataSource="rgMainDataGrid_NeedDataSource"
        AllowMultiRowSelection="false" SelectedItemStyle-CssClass="rgSelectedRow" OnItemCreated="rgMainDataGrid_ItemCreated"
        PageSize="25">
        <MasterTableView TableLayout="Fixed">
            <PagerStyle Position="Bottom" />
        </MasterTableView>
        <HeaderStyle Width="100px"></HeaderStyle>
        <ClientSettings AllowColumnsReorder="True" ReorderColumnsOnClient="True">
            <Scrolling AllowScroll="true" SaveScrollPosition="true" UseStaticHeaders="true" />
            <Resizing AllowColumnResize="false" EnableRealTimeResize="false" ResizeGridOnColumnResize="false" />
            <Selecting AllowRowSelect="false" />
            <ClientEvents OnRowDeselected="resetSelectAll"></ClientEvents>
        </ClientSettings>
        <ExportSettings>
            <Pdf FontType="Subset" PaperSize="Letter" />
            <Excel Format="ExcelML" />
            <Csv ColumnDelimiter="Colon" RowDelimiter="NewLine" />
        </ExportSettings>
        <StatusBarSettings />
    </telerik:RadGrid>
    <div style="text-align: right">
        <asp:LinkButton ID="lbExcel" OnClick="lbExcel_Click" runat="server">                            <img height=20 src="../Images/documenticons/excel.jpg" />
                            Download to Excel</asp:LinkButton>
    </div>
    <div align="center" style="padding-top: 30px">
        <hr />
        <asp:Button ID="btnMyReports" runat="server" Text="My Reports" OnClick="btnMyReports_Cick" />
        <asp:Button ID="btnGoHome" runat="server" Text="Go Home" OnClick="btnGoHome_Cick" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
