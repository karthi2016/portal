<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewControlPropertyOverrides.aspx.cs" Inherits="admin_ViewControlPropertyOverrides" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Control Property Overrides
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div>
        <b>Page:</b>
        <%= MultiStepWizards.CustomizePage.PageName  %>
    </div>
    &nbsp;
    <p>
        Below are a list of controls whose values you can customize. Select <b>Edit...</b>
        to edit the values listed. <b>Advanced Users</b> can manually override the property of any control by using the 
        button below.
    </p>
    <asp:GridView id="gvValues" runat="server" OnRowCommand="gvValues_RowCommand"
     OnRowDataBound="gvValues_RowDataBound"
     GridLines="None" AutoGenerateColumns="false" EmptyDataText="No controls are available for editing on the specified page.">
    <Columns>
    <asp:BoundField DataField="ControlName" HeaderText="Control" HeaderStyle-HorizontalAlign="Left" />
    <asp:BoundField DataField="PropertyName" HeaderText="Property" HeaderStyle-HorizontalAlign="Left" />
    <asp:TemplateField HeaderText="Value" HeaderStyle-HorizontalAlign="Left" >
    <ItemTemplate>
    <asp:Literal ID="lValue" runat="server" />
    </ItemTemplate>
    </asp:TemplateField>
   
    <asp:TemplateField>
    <ItemTemplate>
    <asp:LinkButton ID="lbEdit" runat="server" CommandName="Edit">(edit)</asp:LinkButton>
    <asp:LinkButton ID="lbReset" runat="server" CommandName="Reset">(reset)</asp:LinkButton>
    </ItemTemplate>
    </asp:TemplateField>
    </Columns>
    </asp:GridView>
    <hr />
     
            <div align="center" style="padding-top: 20px">
                <asp:Button ID="btnAddManual" OnClick="btnAddManual_Click" Text="Manually Add Control Property Override" CausesValidation="false" runat="server" />
                <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Go Back" CausesValidation="false" runat="server" />
                <div class="clearBothNoSPC">
                </div>
            </div>
      

</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
