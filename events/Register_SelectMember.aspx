<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="Register_SelectMember.aspx.cs" Inherits="events_Register_SelectMember" %>

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
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSelectMember" runat="Server">Select a Member to Register</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:DropDownList ID="ddlMembers" runat="server" DataTextField="Name" DataValueField="ID"
                AppendDataBoundItems="true">
                <asp:ListItem Text="---- Select a Member ----" Value="0"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnContinue" runat="server" Text="Continue" OnClick="btnContinue_Click" />
            <asp:CompareValidator runat="server" ID="cvMember" ControlToValidate="ddlMembers" Display="None"
                ErrorMessage="Please specify a Member." Operator="NotEqual" ValueToCompare="0" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSelectTasks" runat="Server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li><a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">Back to View
                <%=targetEvent.Name %>
                Event</a></li>
            <li runat="server" id="liEventOwnerTask" visible="false">
                <asp:HyperLink runat="server" ID="hlEventOwnerTask" /></li>
          <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
