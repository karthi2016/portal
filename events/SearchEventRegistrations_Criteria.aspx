<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="SearchEventRegistrations_Criteria.aspx.cs" Inherits="event_SearchEventRegistrations_Criteria" %>

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
    Search
    <%=targetEvent.Name %>
    Registrations
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSearchFields" runat="Server">Search Fields</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lName" runat="Server">Name:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbName" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lFee" runat="Server">Fee:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFee" runat="server" AppendDataBoundItems="true" DataTextField="Name"
                            DataValueField="ID">
                            <asp:ListItem Text="---- Select ----" />
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lOutputFormat" runat="server">Output Format</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:RadioButtonList runat="server" ID="rblOutputFormat">
                <asp:ListItem Text="To My Screen" Value="screen" Selected="True" />
                <asp:ListItem Text="Excel" Value="download" />
            </asp:RadioButtonList>
            <div style="text-align: center; padding-top: 20px">
                <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="Search" /></div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="Server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <li><a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">Back to View
                <%=targetEvent.Name %>
                Event</a></li>
            <li runat="server" id="liEventOwnerTask" visible="false">
                <asp:HyperLink runat="server" ID="hlEventOwnerTask" /></li>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
