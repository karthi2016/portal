<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ManageGroupRegistration.aspx.cs" Inherits="events_ManageGroupRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name%></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <%=targetEvent.Name%>
    Group Registation -
    <%=targetOrganization.Name%>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server" />
    <p>
        <asp:Literal ID="lStatus" runat="server">Group Registration Status:</asp:Literal>
        <asp:Label ID="lblGroupRegStatus" runat="server" ForeColor="Green">Open until 12/15/2011</asp:Label>
    </p>
    <h2>
        Current Registrations</h2>
    <asp:GridView ID="gvRegistrants" runat="server" EmptyDataText="There are currently no registrations for the specified organization."
        AutoGenerateColumns="false" GridLines="None"
         OnRowCommand="gvRegistrants_Command"
         >
         <Columns>
                <asp:BoundField HeaderText="ID" DataField="Owner.LocalID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Name" DataField="Owner.Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Fee" DataField="Fee.Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Status" DataField="Status" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                 <asp:TemplateField>
                    <ItemTemplate>
                      <asp:LinkButton ID="lbRemove" runat="server" Text="Cancel" Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    <asp:Panel ID="pnlPending" runat="server" Visible="false">
        <h2>
            <asp:Literal ID="lPendingRegistrations" runat="server">Pending Registrations</asp:Literal></h2>
        <b><font color="red"> 
            Important: The registrants below are in progress but have not yet been saved. Once you have finished adding your registrants, you must click on the "Complete Group Registration" button below to process these
            registrations, or they will not be saved!</font></b>
        <asp:GridView ID="gvPendingRegistrations" runat="server" AutoGenerateColumns="false"    
         OnRowDataBound="gvPendingRegistrations_OnRowDataBound"
        OnRowCommand="gvPendingRegistrations_Command"
            GridLines="None">
            <Columns>
                <asp:BoundField HeaderText="ID" DataField="LocalID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Name" DataField="Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Fee" DataField="Fee" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Cost" DataField="Cost" DataFormatString="{0:C}" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lbCancel" runat="server" Text="(remove/cancel)" CommandName="Remove" Visible="true" OnClientClick="if (!window.confirm('Are you sure you want to cancel this registration?')) return false;"/>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div style="float: right; padding-top: 10px; padding-bottom: 10px">
            <asp:Literal ID="lTotalPending" runat="server"><b>Total Pending Amount: </b></asp:Literal>
            <asp:Label ID="lblTotalPending" runat="server" ForeColor="Green" />
            <br />
            <asp:Button ID="btnCompleteGroup" runat="server" Text="Complete Group Registration" OnClick="btnCompleteGroup_Click"
                Visible="false" />
                 <asp:Button ID="btnCancelGroupRegistration" runat="server" Text="Cancel Group Registration" OnClick="btnCancelGroup_Click"
                 OnClientClick="if (!window.confirm('Are you sure you want to clear out all group registrations?')) return false;"
                  />
        </div>
    </asp:Panel>
    <div class="section" style="margin-top: 70px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lEventTasks" runat="server">Group Registration Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <asp:HyperLink runat="server" ID="hlRegistration" NavigateUrl="~/events/GroupRegistrationStep1.aspx?contextID="><LI>Add a Registration</LI></asp:HyperLink>
                <asp:HyperLink runat="server" ID="hlBackToEvents"><LI>Back to Event Home Page</LI></asp:HyperLink>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
