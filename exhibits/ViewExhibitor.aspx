<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewExhibitor.aspx.cs" Inherits="exhibits_ViewExhibitor" %>

<%@ Register TagPrefix="uc1" TagName="CustomFieldSet" Src="~/controls/CustomFieldSet.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewShow.aspx?contextID=<%=targetShow.ID %>">
        <%=targetShow.Name  %></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <%=targetShow.Name%>
    Exhibitor -
    <%=targetExhibitor.Name %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lExhibitorBooths" runat="server">Exhibitor Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lName" runat="server">Exhibiting As:</asp:Literal>
                    </td>
                    <td>
                        <%=targetExhibitor.Name ?? targetEntity.Name %>
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lExhibitingCompany" runat="server">Exhibiting Company:</asp:Literal>
                    </td>
                    <td>
                        <%=targetEntity.Name%>
                    </td>
                </tr>
                <tr id="trAssigned" runat="server" visible="false">
                    <td class="columnHeader">
                        <asp:Literal ID="lAssignedBooths" runat="server">Assigned Booth(s):</asp:Literal>
                    </td>
                    <td>
                        <%=assignedBooths %>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr id="trPreferred" runat="server" visible="false">
                    <td class="columnHeader">
                        <asp:Literal ID="lPreferredBooths" runat="server">Preferred Booth(s):</asp:Literal>
                    </td>
                    <td>
                        <%= preferredBooths  %>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr id="trBoothTypes" runat="server" visible="false">
                    <td class="columnHeader">
                        <asp:Literal ID="Literal1" runat="server">Booth Type(s):</asp:Literal>
                    </td>
                    <td>
                        <%=boothTypes %>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" EditMode="False" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lContacts" runat="server">Exhibitor Contacts</asp:Literal></h2>
        </div>
        <br />
        <asp:GridView ID="gvBoothContacts" runat="server" GridLines="None" Width="800px"
            OnRowCommand="gvBoothContacts_Command" OnRowDataBound="gvBoothContacts_DataBound"
            AutoGenerateColumns="false" EmptyDataText="You have no exhibitor contacts listed."
            HeaderStyle-HorizontalAlign="Left">
            <Columns>
                <asp:BoundField HeaderText="Type" DataField="Type.Name" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="First Name" DataField="FirstName" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Last Name" DataField="LastName" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Email Address" DataField="EmailAddress" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Work Phone" DataField="WorkPhone" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField HeaderText="Mobile Phone" DataField="MobilePhone" ItemStyle-HorizontalAlign="Left" />
                <asp:ButtonField Text="(edit)" ButtonType="Link" CommandName="Edit" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lbDelete" runat="server" Text="(remove)" CommandName="Delete" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lAdditionalProducts" runat="server">Additional Products</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvAdditionalProducts" runat="server" GridLines="None" Width="800px"
                AutoGenerateColumns="false" EmptyDataText="You have not purchased any additional exhibitor merchandise."
                HeaderStyle-HorizontalAlign="Left">
                <Columns>
                    <asp:HyperLinkField HeaderText="Order #" DataNavigateUrlFormatString="/financial/ViewOrder.aspx?contextID={0}"
                        DataNavigateUrlFields="Order.ID" DataTextField="Order.Name" />
                    <asp:BoundField HeaderText="Product" DataField="Product.Name" ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Qty" DataField="Quantity" ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Unit Cost" DataFormatString="{0:C}" DataField="UnitPrice"
                        ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Total" DataFormatString="{0:C}" DataField="Total" ItemStyle-HorizontalAlign="Left" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="Literal2" runat="server">Notes/Special Requests</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <%=targetExhibitor.Notes?? "No notes/special requests."%>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lEventTasks" runat="server">Exhibitor Tasks</asp:Literal></h2>
        </div>
        <asp:Literal ID="lStartDatePassed" Visible="false" runat="server">
        <span style="color: Red;"><BR />Note that the exhibit show start date has passed - you are no longer able to update your exhibitor information or add contacts.</span>
        </asp:Literal>
        <div class="sectionContent" style="width: 400px">
            <ul>
                <asp:HyperLink runat="server" ID="hlUpdateExhibitorInfo" NavigateUrl="/exhibits/UpdateExhibitorInfo.aspx?contextID="><LI>Update Exhibitor Logo/Information</LI></asp:HyperLink>
                <asp:HyperLink runat="server" ID="hlAddExhibitorContact" NavigateUrl="/exhibits/AddEditExhibitorContact.aspx?contextID="><LI>Add Exhibitor Contact</LI></asp:HyperLink>
                <asp:HyperLink runat="server" ID="hlPurchaseAdditionalProducts" NavigateUrl="/exhibits/PurchaseAdditionalExhibitorProducts.aspx?contextID="><LI>Purchase Additional Products</LI></asp:HyperLink>
                <asp:HyperLink runat="server" ID="hlDownloadShowFloor" Visible="false"><LI>Download Show Floor Layout</LI></asp:HyperLink>
                <li><a href="ViewShow.aspx?contextID=<%=targetShow.ID %>">Back to
                    <%=targetShow.Name %>
                    Home Page</a></li>
                <li><a href="/">
                    <asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
