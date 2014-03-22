<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="ViewTableSeats.aspx.cs" Inherits="events_ViewTableSeats" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.1.519.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlEventOwner" Visible="false" />
    <a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name %> 
        ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Table Seats
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
      <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSelectFee" runat="Server">Your Table Assignments</asp:Literal>
            </h2>
        </div>
        <div class="sectionContent">
             
          <telerik:RadGrid BorderWidth="0px" EnableAjax="true" HeaderStyle-Width="100px" Width="100%"
        ID="rgTableReservations" runat="server" GridLines="None"  AutoGenerateColumns="false" 
        SelectedItemStyle-CssClass="rgSelectedRow">
        <HeaderStyle Width="100px"></HeaderStyle>
        <MasterTableView>
        <Columns>
         
         
        <telerik:GridBoundColumn DataField="NumberOfSeats" HeaderText="# Seats" />
        <telerik:GridBoundColumn DataField="Table.Name" HeaderText="Table" />
        <telerik:GridBoundColumn DataField="Fee.Name" HeaderText="Fee" />
        <telerik:GridBoundColumn DataField="Type.Name" HeaderText="Type" />
        
        </Columns></MasterTableView>
    </telerik:RadGrid>
        <asp:Literal ID="lNoRecords" runat="server" Visible="false">There are no table or seats assigned to you.</asp:Literal>
        </div>
    </div>
     <hr style="width: 100%" />
    <div style="text-align: center">
        
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Go Back"
            OnClick="btnBack_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
