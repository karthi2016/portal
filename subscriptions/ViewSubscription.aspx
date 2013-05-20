<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewSubscription.aspx.cs" Inherits="subscriptions_ViewSubscription" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewMySubscriptions.aspx">View My Subscriptions</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Subscription
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <table>
        <tr id="trPublication" runat="server">
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lPublication" runat="server">Publication:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblPublication" runat="server" />
            </td>
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lStartDate" runat="server">Start Date:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblStartDate" runat="server">None.</asp:Label>
            </td>
        </tr>
        <tr id="trProduct" runat="server">
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lProduct" runat="server">Product:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblProduct" runat="server" />
            </td>
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lExpirationDate" runat="server">Expiration Date:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblExpirationDate" runat="server">None.</asp:Label>
            </td>
        </tr>
        <tr id="trOwner" runat="server">
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lOwner" runat="server">Owner:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblOwner" runat="server" />
            </td>
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lOnHold" runat="server">On Hold?:</asp:Literal>
            </td>
            <td>
                <%=targetSubscription.OnHold %>
            </td>
        </tr>
        <tr>
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lOriginalOrder" runat="server">Original Order:</asp:Literal>
            </td>
            <td>
                <asp:HyperLink ID="hlOriginalOrder" runat="server" Text="n/a" />
            </td>
            <td class="columnHeader" style="width: 150px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lRenewalOrder" runat="server">Renewal Order:</asp:Literal>
            </td>
            <td>
                <asp:HyperLink ID="hlRenewalOrder" runat="server" Text="n/a" />
            </td>
            <td class="columnHeader" style="width: 150px">
            </td>
            <td>
            </td>
        </tr>
        <tr id="trTermination" runat="server" visible="false">
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lTerminationDate" runat="server">Termination Date:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblTerminationDate" runat="server" />
            </td>
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lTerminationReason" runat="server">Termination Reason:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblTerminationReason" runat="server">None given.</asp:Label>
            </td>
        </tr>
    </table>
    <h2>
        Shipping &amp; Fulfillment</h2>
    <table>
        <tr id="tr1" runat="server"  >
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lShipTo" runat="server">Ship To:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblShipTo" runat="server" />
            </td>
             
        </tr>
         <tr id="tr2" runat="server" valign=top >
            <td class="columnHeader" style="width: 150px">
                <asp:Literal ID="lShippingAddress" runat="server">Shipping Address:</asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblShippingAddress" runat="server" >No address on file. Please update your profile address.</asp:Label>
            </td>
             
        </tr>
    </table>
    <hr />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <ul>
            <asp:LinkButton ID="lblRenewSubscription" runat="server" 
                onclick="lblRenewSubscription_Click"><li>Renew this Subscription</li></asp:LinkButton>
            <asp:HyperLink ID="hlModify" runat="server" NavigateUrl="/subscriptions/EditSubscription.aspx?contextID="><LI>Modify this Subscription</LI></asp:HyperLink>
            <li>
                <asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
