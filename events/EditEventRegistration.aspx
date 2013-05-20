<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="EditEventRegistration.aspx.cs" Inherits="events_EditEventRegistration" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="/events/CreateEditEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name %>
        ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Edit Event Registration
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lRegInfo" runat="server">Registration Information</asp:Literal> </h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lOwner" runat="server">Owner:</asp:Literal>
                    </td>
                    <td align="left">
                        <%=targetOwner.Name %>
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lCategory" runat="server">Category:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlCategory" AppendDataBoundItems="true" DataTextField="Name"
                            DataValueField="ID">
                            <asp:ListItem Text="---- Select ----" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lName" runat="server">Name:</asp:Literal>
                    </td>
                    <td align="left">
                        <%=targetEventRegistration.Name %>
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lClass" runat="server">Class:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlClass" AppendDataBoundItems="true" DataTextField="Name"
                            DataValueField="ID">
                            <asp:ListItem Text="---- Select ----" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lFee" runat="server"> Fee:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlFee" DataTextField="Name" DataValueField="ID">
                        </asp:DropDownList>
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lLinkedReg" runat="server">Linked Registration:</asp:Literal>
                    </td>
                    <td align="left">
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lApprovals" runat="server">Approvals & Cancellation</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lApproved" runat="server">Approved?</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:CheckBox runat="server" ID="chkApproved" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lCancellationFee" runat="server">Cancellation Date:</asp:Literal>
                    </td>
                    <td align="left">
                        <telerik:RadDateTimePicker runat="server" ID="dtpCancellationDate" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lDateApproved" runat="server">Date Approved:</asp:Literal>
                    </td>
                    <td align="left">
                        <telerik:RadDateTimePicker runat="server" ID="dtpDateApproved" />
                    </td>
                    <td class="columnHeader">
                         <asp:Literal ID="lCancellationReason" runat="server">Cancellation Reason:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:TextBox runat="server" ID="tbCancellationReason" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lOnWaitList" runat="server">On Wait List?</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:CheckBox runat="server" ID="chkOnWaitList" />
                    </td>
                    <td class="columnHeader">
                        &nbsp;
                    </td>
                    <td align="left">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lCheckInDate" runat="server">Check In Date:</asp:Literal>
                    </td>
                    <td align="left">
                        <telerik:RadDateTimePicker runat="server" ID="dtpCheckInDate" />
                    </td>
                    <td class="columnHeader">
                        &nbsp;
                    </td>
                    <td align="left">
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lBadgeInformation" runat="server">Badge Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lBadgeName" runat="server">Badge Name:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:TextBox runat="server" ID="tbBadgeName" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lBadgeOrganizatio" runat="server">Badge Organization:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:TextBox runat="server" ID="tbBadgeOrganization" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lBadgeTitle" runat="server">Badge Title:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:TextBox runat="server" ID="tbBadgeTitle" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lBadgeCity" runat="server">Badge City:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:TextBox runat="server" ID="tbBadgeCity" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lBadgeState" runat="server">Badge State:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:TextBox runat="server" ID="tbBadgeState" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lBadgeCountry" runat="server">Badge Country:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:TextBox runat="server" ID="tbBadgeCountry" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lBadgeRegType" runat="server">Badge Registration Type:</asp:Literal>
                    </td>
                    <td align="left">
                        <asp:TextBox runat="server" ID="tbBadgeRegistrationType" />
                    </td>
                </tr>
            </table>
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
        <li><a href="/events/CreateEditEvent.aspx?contextID=<%=targetEvent.ID %>">View
                    <%=targetEvent.Name %></a></li>
                
                
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
     
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
