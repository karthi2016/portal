<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewMembership.aspx.cs" Inherits="membership_ViewMembership" %>

<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Membership
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lMembershipInfoHeader" runat="server">Membership Information</asp:Literal></h2>
        </div>
        <table style="width: 100%">
            <tr id="trMembershipID_Row" runat="server">
                <td class="columnHeader">
                    <asp:Literal ID="lMembershipID" runat="server">Membership ID:</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("LocalID") %>
                </td>
                <td class="columnHeader">
                </td>
                <td>
                </td>
            </tr>
            <tr id="trOwner_Row" runat="server">
                <td class="columnHeader">
                    <asp:Literal ID="lOwner" runat="server">Owner:</asp:Literal>
                </td>
                <td>
                    #<%=GetMembershipField("Owner.LocalID") %>
                    -
                    <%=GetMembershipField("Owner.Name") %>
                </td>
                <td class="columnHeader">
                    <asp:Literal ID="lJoinDate" runat="server">Join Date:</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("JoinDate","d") %>
                </td>
            </tr>
            <tr  id="trStatus_Row" runat="server">
                <td class="columnHeader">
                    <asp:Literal ID="lStatus" runat="server">Status:</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("Status.Name") %>
                </td>
                <td class="columnHeader" id="tdExpirationDateLabel" runat="server">
                    <asp:Literal ID="lExpirationDate" runat="server">Expiration Date:</asp:Literal>
                </td>
                <td  id="tdExpirationDate" runat="server">
                    <%=GetMembershipField("ExpirationDate","d") %>
                </td>
            </tr>
            <tr  id="trType_Row" runat="server">
                <td class="columnHeader">
                    <asp:Literal ID="lType" runat="server">Type:</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("Type.Name") %>
                </td>
                <td class="columnHeader">
                    <asp:Literal ID="lRenewalDate" runat="server">Renewal Date:</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("RenewalDate","d") %>
                </td>
            </tr>
            <tr  id="trProduct_Row" runat="server">
                <td class="columnHeader">
                    <asp:Literal ID="lProduct" runat="server">Product:</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("Product.Name") %>
                </td>
                <td class="columnHeader">
                    <asp:Literal ID="lApproval" runat="server">Approved?</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("Approved") %>
                </td>
            </tr>
            <tr  id="trReceivesMemberBenefits_Row" runat="server">
                <td class="columnHeader">
                    <asp:Literal ID="lReceivesMemberBenefits" runat="server">Receives Member Benefits:</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("ReceivesMemberBenefits") %>
                </td>
                <td class="columnHeader">
                    <asp:Literal ID="lMembershipDirectoryOptOut" runat="server">Membership Directory Opt Out?</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("MembershipDirectoryOptOut") %>
                </td>
            </tr>
            <tr id="trTermination" runat="server" style="color: Red">
                <td class="columnHeader">
                    <asp:Literal ID="lTerminationDate" runat="server">Termination Date:</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("TerminationDate") %>
                </td>
                <td class="columnHeader">
                    <asp:Literal ID="lTerminationReason" runat="server">Termination Reason</asp:Literal>
                </td>
                <td>
                    <%=GetMembershipField("TerminationReason.Name") %>
                </td>
            </tr>
        </table>
    </div>
    <div class="section" style="margin-top: 10px" id="tdChapters" runat="server" visible="false">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lChapters" runat="server">Chapters</asp:Literal></h2>
        </div>
        <asp:GridView ID="gvChapters" GridLines="None" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Chapter" DataField="Chapter.Name" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Join Date" DataField="JoinDate"
                    DataFormatString="{0:d}" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Expiration Date" DataField="ExpirationDate"
                    DataFormatString="{0:d}" />
            </Columns>
        </asp:GridView>
    </div>
    <div class="section" style="margin-top: 10px" id="tdSections" runat="server" visible="false">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lSections" runat="server">Sections</asp:Literal></h2>
        </div>
        <asp:GridView ID="gvSections" GridLines="None" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Section" DataField="Section.Name" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Join Date" DataField="JoinDate"
                    DataFormatString="{0:d}" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Expiration Date" DataField="ExpirationDate"
                    DataFormatString="{0:d}" />
            </Columns>
        </asp:GridView>
    </div>
     <div class="section" style="margin-top: 10px" id="divAddOns" runat="server" visible="false">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lAddOns" runat="server">Add-Ons</asp:Literal></h2>
        </div>
        <asp:GridView ID="gvAddOns" GridLines="None" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Type" DataField="Merchandise.Name" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Quantity" DataField="Quantity" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Price" DataField="Price" DataFormatString="{0:C}" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Renews With Membership?" DataField="Renewable" />               
                    
            </Columns>
        </asp:GridView>
    </div>
     <div class="section" style="margin-top: 10px" id="divBillingInformation" runat="server"  >
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lBillingInfo" runat="server">Billing Information</asp:Literal></h2>
        </div>
        You can have this membership renewal automatically charged to your credit card. The current information is below.
        <asp:HyperLink ID="hlUpdateBillingInfo2" runat="server" Text="Click here to update."></asp:HyperLink>
        <table style="width: 100%; margin-top: 10px">
            <tr id="tr1" runat="server">
                <td class="columnHeader">
                    <asp:Literal ID="lPaymentMethod" runat="server">Payment Method:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblPaymentInfo" runat="server">You currently have no payment information associated with this membership.</asp:Label>
                </td>
                <td class="columnHeader">
                </td>
                <td>
                </td>
            </tr>
             
        </table>
    </div>
    <div class="section" style="margin-top: 10px" id="divHistory" runat="server" visible="false">
        <div class="sectHeaderTitle">
            <h2>
                <asp:Literal ID="lHistory" runat="server">History</asp:Literal></h2>
        </div>
        <asp:GridView ID="gvHistory" GridLines="None" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Activity" DataField="Type_Name" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Performed By" DataField="Actor.Name"
                    DataFormatString="{0:d}" />
                <asp:BoundField HeaderStyle-HorizontalAlign="Left" HeaderText="Date" DataField="CreatedDate"
                    DataFormatString="{0:d}" />
            </Columns>
        </asp:GridView>
    </div>
    <div class="section" style="margin-top: 10px" visible="false">
        <uc1:CustomFieldSet ID="CustomFieldSet1" EditMode="False" runat="server" />
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <ul>
                <li runat="server" id="liContactInfo">
                    <asp:HyperLink ID="hlContactInfo" runat="server" Text="Update Contact Info"></asp:HyperLink>
                </li>
                 <li runat="server" id="li1">
                    <asp:HyperLink ID="hlUpdateBillingInfo" runat="server" Text="Update Billing Information"></asp:HyperLink>
                </li>
                <li runat="server" id="liUpdateMembershipInfo">
                    <asp:HyperLink ID="hlUpdateMembershipInfo" runat="server" Text="Update Membership Info"></asp:HyperLink>
                </li>
                <li runat="server" id="liRenewMembership">
                    <asp:HyperLink ID="hlRenewMembership" runat="server" Text="Renew Membership"></asp:HyperLink>
                </li>
                <li runat="server" id="liViewAccountHistory">
                    <asp:HyperLink ID="hlViewAccountHistory" runat="server" Text="View Account History"></asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
