<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewCertification.aspx.cs"
    MasterPageFile="~/App_Master/GeneralPage.master" Inherits="continuingeducation_ViewCertification" %>
    
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>View Certification </title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View a Certification - <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <uc1:CustomFieldSet ID="CustomFieldSet1" EditMode="false" runat="server" />
    <asp:Panel ID="pnlCEURequirements" runat="server" Visible="true">
        <h2>
            <asp:Literal ID="lCEUReq" runat="server">CEU Requirements</asp:Literal>
        </h2>

          <telerik:RadGrid BorderWidth="0px" EnableAjax="true" HeaderStyle-Width="100px" Width="100%"
        ID="rgCEU" runat="server" GridLines="None"  AutoGenerateColumns="false"
        SelectedItemStyle-CssClass="rgSelectedRow">
        <HeaderStyle Width="100px"></HeaderStyle>
        <MasterTableView>
        <Columns>
         
        <telerik:GridBoundColumn DataField="Type.Name" HeaderText="Type" />
        <telerik:GridBoundColumn DataField="QuantityRequired" HeaderText="# Required" />
        <telerik:GridBoundColumn DataField="Quantity" HeaderText="# Earned" />
        <telerik:GridBoundColumn DataField="QuantityNeeded" HeaderText="# Needed" />
        
        </Columns></MasterTableView>
    </telerik:RadGrid>

         
    </asp:Panel>

     <asp:Panel ID="pnlRecommendations" runat="server" Visible="false">
        <h2>
            <asp:Literal ID="Recommendations" runat="server">Recommendations</asp:Literal>
        </h2>

          <telerik:RadGrid BorderWidth="0px" EnableAjax="true" HeaderStyle-Width="100px" Width="100%"
        ID="rgRecommendations" runat="server" GridLines="None"  AutoGenerateColumns="false" OnItemDataBound="rgRecommendations_ItemDataBound"
         OnItemCommand="rgRecommendations_ItemCommand"
        SelectedItemStyle-CssClass="rgSelectedRow">
        <HeaderStyle Width="100px"></HeaderStyle>
        <MasterTableView>
        <Columns>
         
        <telerik:GridBoundColumn DataField="Name" HeaderText="Name" />
        <telerik:GridBoundColumn DataField="EmailAddress" HeaderText="Email" />
        <telerik:GridBoundColumn DataField="Status" HeaderText="Status" />
        <telerik:GridTemplateColumn>
        <ItemTemplate>
        <asp:LinkButton ID="lbResend" runat="server" Text="(resend email)" />
        </ItemTemplate></telerik:GridTemplateColumn>
        
        </Columns></MasterTableView>
    </telerik:RadGrid>

    </asp:Panel>

    
     <asp:Panel ID="pnlExamRequirements" runat="server" Visible="false">
        <h2>
            <asp:Literal ID="lExamReq" runat="server">Exam Requirements</asp:Literal>
        </h2>
           <telerik:RadGrid BorderWidth="0px" EnableAjax="true" HeaderStyle-Width="100px" Width="100%"
        ID="rgExams" runat="server" GridLines="None"  AutoGenerateColumns="false"
        SelectedItemStyle-CssClass="rgSelectedRow">
        <HeaderStyle Width="100px"></HeaderStyle>
        <MasterTableView>
        <Columns>
         
        <telerik:GridBoundColumn DataField="Type.Name" HeaderText="Type" />
        <telerik:GridBoundColumn DataField="Passed" HeaderText="Passed?" />
        
        </Columns></MasterTableView>
    </telerik:RadGrid>
    </asp:Panel>
    <hr />
    <div style="text-align: center">
        <asp:Button ID="btnHistory" runat="server" CausesValidation="false" Text="Certification History"
            OnClick="btnHistory_Click" />
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Go Home"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
