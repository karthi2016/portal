<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ViewEventRegistration.aspx.cs" Inherits="events_ViewEventRegistration" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
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
                <asp:Literal ID="lGeneralInformation" runat="server">General Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lRegistrant" runat="server">Registrant:</asp:Literal>
                    </td>
                    <td>
                        <%=targetEntity.Name %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lEmailAddress" runat="server">E-mail Address:</asp:Literal>
                    </td>
                    <td>
                        <%=targetEntity.EmailAddress %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lConfirmationNumber" runat="server">Confirmation #:</asp:Literal>
                    </td>
                    <td>
                        <%= targetRegistration.LocalID.HasValue ? targetRegistration.LocalID.ToString() : targetRegistration.ID %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lRegistrationStatus" runat="server">Registration Status:</asp:Literal>
                    <td>
                        <%= status %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lBalanceDue" runat="server">Balance Due:</asp:Literal>
                    </td>
                    <td>
                        <%= drOrder != null ? ((decimal)drOrder["BalanceDue"]).ToString("C") : "$0.00" %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lRegistered" runat="server">Registered:</asp:Literal>
                    </td>
                    <td>
                        <%= targetRegistration.CreatedDate %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lUpdated" runat="server">Updated:</asp:Literal>
                    </td>
                    <td>
                        <%=targetRegistration.LastModifiedDate %>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lRegisteredBy" runat="server">Registered By:</asp:Literal>
                    </td>
                    <td>
                        <%= string.Format("{0}, {1}",createdBy.LastName,createdBy.FirstName) %>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <uc1:CustomFieldSet ID="cfsRegistrationFields" runat="server" EditMode="false" />
        </div>
    </div>

    <div class="section" style="margin-top: 10px" id="divSessions" runat="server">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSessions" runat="server">Sessions</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvSessions" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No sessions found.">
                <Columns>
                    <asp:BoundField DataField="Event.StartDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Date" DataFormatString="{0:M/dd/yyy}"/>
                    <asp:BoundField DataField="Event.TimeSlot.StartTime" HeaderStyle-HorizontalAlign="Left" HeaderText="Time" DataFormatString="{0:h:mm tt}"/>
                    <asp:BoundField DataField="Event.TimeSlot.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Time Slot" />
                    <asp:BoundField DataField="Event.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Session" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="section" style="margin-top: 10px" id="divPayments" runat="server">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lPayments" runat="server">Payments</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvPayments" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No payments found.">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\financial\ViewPayment.aspx?contextID={0}"
                        HeaderStyle-HorizontalAlign="Left" HeaderText="Name" DataNavigateUrlFields="Payment.ID"
                        DataTextField="Payment.Name" />
                    <asp:BoundField DataField="Payment.Date" HeaderStyle-HorizontalAlign="Left" HeaderText="Date" />
                    <asp:BoundField DataField="Total" HeaderStyle-HorizontalAlign="Left" HeaderText="Amount"
                        DataFormatString="{0:C}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="section" style="margin-top: 10px" id="divHistoricalTransactions" runat="server">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTransactions" runat="server">Transactions</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvHistoricalTransactions" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No transactions found." OnRowDataBound="gvHistoricalTransactions_RowDataBound"
                OnRowCommand="gvHistoricalTransactions_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Date" HeaderStyle-HorizontalAlign="Left" HeaderText="Date" />
                    <asp:BoundField DataField="Type" HeaderStyle-HorizontalAlign="Left" HeaderText="Type" />
                    <asp:BoundField DataField="Total" HeaderStyle-HorizontalAlign="Left" HeaderText="Total"
                        DataFormatString="{0:C}" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink runat="server" ID="hlEdit" NavigateUrl='<%# string.Format(@"~\financial\ViewHistoricalTransaction.aspx?contextID={0}&eventId={1}&completeUrl={2}", DataBinder.Eval(Container.DataItem, "ID"), targetEvent.ID, HttpUtility.UrlEncode(Request.Url.PathAndQuery)) %>'
                                Text="(view)" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink runat="server" ID="hlEdit" NavigateUrl='<%# string.Format(@"~\financial\CreateEditHistoricalTransaction.aspx?contextID={0}&eventId={1}&completeUrl={2}", DataBinder.Eval(Container.DataItem, "ID"), targetEvent.ID, HttpUtility.UrlEncode(Request.Url.PathAndQuery)) %>'
                                Text="(edit)" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnDelete" CommandArgument='<%# Bind("ID") %>'
                                CommandName="deletehistoricaltransaction" CausesValidation="false" Text="(delete)"
                                OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <div style="text-align: right;" id="divAddHistoricalTransaction" runat="server">
                <hr />
                <a href="/financial/CreateEditHistoricalTransaction.aspx?contextID=<%=targetRegistration.Order %>&eventId=<%=targetEvent.ID %>&completeUrl=<%=HttpUtility.UrlEncode(Request.Url.PathAndQuery) %>">
                    Add Transaction</a>
            </div>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lEventRegTasks" runat="server">Event Registration Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li runat="server" id="liPrintAgenda"> <asp:HyperLink ID="hlPrintAgenda" runat="server">Print Agenda</asp:HyperLink></li>
                <li runat="server" id="liChangeSessions">
                    <a href="/events/Register_CreateRegistration.aspx?contextID=<%=targetRegistration.ID %>">Change Sessions</a>
                </li>              
                <li runat="server" id="liEditRegistration">
                    <a href="/events/EditEventRegistration.aspx?contextID=<%=targetRegistration.ID %>">Edit Registration</a></li>
                <li runat="server" id="liCancelRegistration">
                    <asp:LinkButton runat="server" ID="lbCancelRegistration" Text="Cancel Registration"
                        OnClick="lbCancelRegistration_Click" /></li>
                <li runat="server" id="liDeleteRegistration">
                    <asp:LinkButton runat="server" ID="lbDeleteRegistration" Text="Delete Registration"
                        OnClick="lbDeleteRegistration_Click" OnClientClick="if (!window.confirm('Are you sure you want to delete this item?')) return false;" /></li>
                <li runat="server" id="liEventOwnerTask" visible="false">
                    <asp:HyperLink runat="server" ID="hlEventOwnerTask" /></li>
                 <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
