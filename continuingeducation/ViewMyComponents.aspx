<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewMyComponents.aspx.cs" Inherits="continuingeducation_ViewMyComponents" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View My Certification Components
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server">
    </asp:Literal>
    
    <div>
        View Records From:
        <telerik:RadDatePicker ID="dpFrom" runat="server" />
        to
        <telerik:RadDatePicker ID="dpTo" runat="server" />
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" 
            onclick="btnRefresh_Click" />
    </div>
    <style type="text/css">
       .RadGrid .rgDataDiv
       {
           height : 100% !important;
           max-height : 300px !important;
       }
   </style>
   <div align=right>
   <asp:Literal ID="lNoteUnverified" runat="server"><span style="color: grey; font-style: italic">Note that unverified registrations are listed as gray italics.</span></asp:Literal>
   
   </div>
   <hr />
   <div style="margin-top: 10px"/>
   

    <telerik:RadGrid BorderWidth="0px" EnableAjax="true" HeaderStyle-Width="100px" Width="100%"
        AutoGenerateColumns="false" ID="rgMainDataGrid" runat="server" AllowPaging="false"
        AllowCustomPaging="false" AllowSorting="True" GridLines="None" ShowStatusBar="false"
        OnNeedDataSource="rgMainDataGrid_NeedDataSource" AllowMultiRowSelection="false"
        SelectedItemStyle-CssClass="rgSelectedRow" OnItemCreated="rgMainDataGrid_ItemCreated" OnItemDataBound="rgMainGrid_ItemDataBound">
        <MasterTableView TableLayout="Auto">
            <Columns>
                <telerik:GridBoundColumn DataField="Component.StartDate" HeaderText="Program Date" ItemStyle-Width="50px"
                    DataFormatString="{0:d}" />
                <telerik:GridBoundColumn DataField="CreatedDate" HeaderText="Date Entered" DataFormatString="{0:d}" ItemStyle-Width="50px" />
                <telerik:GridBoundColumn DataField="Component.Name" HeaderText="Program"  ItemStyle-Width="300px" />
                
                <telerik:GridBoundColumn DataField="Credits" HeaderText="Credit(s)" />
                
            </Columns>
            <PagerStyle Position="Bottom" />
        </MasterTableView>
        <HeaderStyle Width="100px"></HeaderStyle>
        
    </telerik:RadGrid>
    <div style="float: right">
        <table style="width: 200px">
        <asp:Repeater ID="rptTotals" runat="server"><ItemTemplate>
            <tr>
                <td class="columnHeader">
                    <%#DataBinder.Eval( Container.DataItem,"Key") %>
                </td>
                <td>
                    <%#DataBinder.Eval( Container.DataItem,"Value", "{0:N1}") %>
                </td>
            </tr>
            </ItemTemplate></asp:Repeater>
            
            <tr>
                <td class="columnHeader">
                    GRAND TOTAL
                </td>
                <td>
                    <asp:Literal ID="lGrantTotal" runat="server" />
                </td>
            </tr>
        </table>
    </div>
   
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
