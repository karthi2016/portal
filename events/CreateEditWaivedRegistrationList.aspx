<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditWaivedRegistrationList.aspx.cs" Inherits="events_CreateEditWaivedRegistrationList" %>
<%@ Import Namespace="System.Data" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
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
    <asp:Label runat="server" ID="lblTitleAction" />
    Waived Registration List
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
             <asp:Literal ID="lBasicInfo" runat="server">Basic Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lEvent" runat="server">Event:</asp:Literal>
                    </td>
                    <td colspan="3" align="left">
                        <asp:Label runat="server" ID="lblEventName"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lName" runat="server">Name: </asp:Literal><span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbName" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" Display="None" ErrorMessage="Please specify a value for Name."
                            ControlToValidate="tbName" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lAppliesToSessions" runat="server">Applies To Sessions?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkAppliesToSessions" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDiscountPercentage" runat="server">Discount Percentage: </asp:Literal><span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbDiscountPercentage" Width="75" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvPercentage" Display="None" ErrorMessage="Please specify a value for Discount Percentage."
                            ControlToValidate="tbDiscountPercentage" />
                        <asp:CompareValidator runat="server" ID="cfPercentage" ControlToValidate="tbDiscountPercentage"
                            Display="None" ErrorMessage="Please specify a valid number for Discount Percentage."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cfPercentageType" ControlToValidate="tbDiscountPercentage"
                            Display="None" ErrorMessage="Please specify a valid number for Discount Percentage."
                            Operator="DataTypeCheck" Type="Currency" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lAppliesToGuestRegistrations" runat="server">Applies To Guest Registrations?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkAppliesToGuestRegistrations" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lIsActive" runat="server">Is Active?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsActive" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lAppliesToEventMerchandise" runat="server">Applies To Event Merchandise?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkAppliesToEventMerchandise" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lNotes" runat="server">Notes</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:TextBox runat="server" ID="tbNotes" Columns="130" Rows="5" TextMode="MultiLine" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="div1">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lMembers" runat="server">Members</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvMembers" runat="server" AutoGenerateColumns="false" EmptyDataText="No members were found."
                GridLines="None" OnRowCommand="gvMembers_RowCommand">
                <Columns>
                    <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="ID" DataField="Individual.LocalID" />
                    <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Last Name" DataField="Individual.LastName" />
                    <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="First Name" DataField="Individual.FirstName" />
                    <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="IsRegistered" DataField="IsRegistered" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="btnRemove" CommandArgument='<%# ((DataRowView)Container.DataItem)["Individual.ID"] %>'
                                CommandName="removemember" CausesValidation="false" Text="(remove)" OnClientClick="if (!window.confirm('Are you sure you want to remove this member?  This will also save the Waived Registration List.')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hr />
            <div style="text-align: right;">
                <asp:LinkButton runat="server" ID="lblAddMember" OnClientClick="if (!window.confirm('This action will save the waived registration list.')) return false;"
                    OnClick="lblAddMember_Click" Text="Add Member" />
            </div>
        </div>
    </div>
    <div class="sectionContent">
        <center>
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" />
            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"
                CausesValidation="false" />
        </center>
    </div>
     <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
        <li><a href="/events/CreateEditEvent.aspx?contextID=<%=targetEvent.ID %>">Back to Edit
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
