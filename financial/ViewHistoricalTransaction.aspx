<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ViewHistoricalTransaction.aspx.cs" Inherits="financial_ViewHistoricalTransaction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlEventOwner" Visible="false" />
    <a href="/events/CreateEditEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name %>
        ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Historical Transaction
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <%= targetHistoricalTransaction.Name %></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDate" runat="server" >Date:</asp:Literal>
                    </td>
                    <td>
                        <%=targetHistoricalTransaction.Date %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lType" runat="server" >Type:</asp:Literal>
                    </td>
                    <td>
                        <%=targetHistoricalTransaction.Type.ToString("D") %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lReferenceNumber" runat="server" >Reference Number:</asp:Literal>
                    </td>
                    <td>
                        <%=targetHistoricalTransaction.ReferenceNumber %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lTotal" runat="server" >Total:</asp:Literal>
                    </td>
                    <td>
                        <%=string.Format("{0:F}", targetHistoricalTransaction.Total) %>
                        USD
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lMemo" runat="server" >Memo:</asp:Literal>
                    </td>
                    <td>
                        <%=targetHistoricalTransaction.Memo %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lNotes" runat="server" >Notes:</asp:Literal>
                    </td>
                    <td>
                        <%=targetHistoricalTransaction.Notes %>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server" >Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li runat="server" id="liEd"><a href="/financial/CreateEditHistoricalTransaction.aspx?contextID=<%=targetHistoricalTransaction.ID %>&eventId=<%=targetEvent.ID %>&completeUrl=<%=HttpUtility.UrlEncode(Request.Url.PathAndQuery) %>">Edit Historical Transaction</a></li>
                <li runat="server" id="liDeleteHistoricalTransaction">
                    <asp:LinkButton runat="server" ID="lbDeleteHistoricalTransaction" Text="Delete Historical Transaction"
                        OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;"
                        OnClick="lbDeleteHistoricalTransaction_Click" /></li>
                <li><a href="<%=CompleteUrl %>">Go Back</a></li>
                <li><a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">Back to View
                    <%=targetEvent.Name %>
                    Event</a></li>
                <li runat="server" id="liEventOwnerTask" visible="false">
                    <asp:HyperLink runat="server" ID="hlEventOwnerTask" /></li>
                <li><a href="/"><asp:Literal ID="lGoHome" runat="server" >Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
