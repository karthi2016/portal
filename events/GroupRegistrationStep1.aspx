<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="GroupRegistrationStep1.aspx.cs" Inherits="events_GroupRegistrationStep1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="ViewEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name%></a> &gt; <a href="ManageGroupRegistration.aspx?contextID=<%=targetEvent.ID %>&organizationID=<%=targetOrganization.ID %>">
            <%=targetOrganization.Name%>
            Group Registration</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Register an Individual for <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:ValidationSummary ID="vsSummary" DisplayMode="BulletList" ShowSummary="true" ForeColor="Red"
                        ShowMessageBox="false" HeaderText="We were unable to continue for the following reasons:"
                        runat="server" />

    <asp:Literal ID="lPageText" runat="server"> 
You can create a registration for an existing contact, or you can create
a new individual. If you do not see the person for whom you want to create a registration, select <b>Create a registration for a person not listed </b>.    </asp:Literal>
  
    <asp:CustomValidator ID="cvAlreadyRegistered" runat="server" Display="None" ErrorMessage="The contact you have selected has already been registered for this event."/>
    <asp:CustomValidator ID="cvSelectedTwice" runat="server"  Display="None" ErrorMessage="The contact you have selected is already in the registration list."/>
  
    <table style="width: 500px; margin-top: 20px">
        <tr>
            <td class="columnHeader">
                <asp:Literal ID="lSelect" runat="server">Select an Individual:</asp:Literal><span
                    class="requiredField">*</span>
            </td>
            <td>
                <asp:DropDownList ID="ddlIndividual" runat="server" onchange="showRegistrationInfo()">
                    <asp:ListItem Text="--- Select an Individual ---" Value=""></asp:ListItem>
                    <asp:ListItem Text="Create a registration for a person not listed" Value="-1"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="None"
                    ControlToValidate="ddlIndividual" ErrorMessage="Please select an individual to register." />
            </td>
        </tr>
    </table>
    <div id="divRegInfo" style="display: none;">
        <h2>
            <asp:Literal runat="server" ID="lRegistrantInfo">Registrant Information</asp:Literal></h2>
        <asp:Literal ID="lContactInfo" runat="server">
    Enter the information below for the person you are registering. This individual will be created and linked to this organization.
        </asp:Literal>
        <table style="width: 500px; margin-top: 20px">
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lFirstName" runat="server">First Name:</asp:Literal><span class="requiredField">*</span>
                </td>
                <td>
                    <asp:TextBox ID="tbFirstName" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" Display="None"
                        ControlToValidate="tbFirstName" ErrorMessage="Please specify a first name." EnableClientScript="false" />
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lLastName" runat="server">Last Name:</asp:Literal><span class="requiredField">*</span>
                </td>
                <td>
                    <asp:TextBox ID="tbLastName" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" Display="None"
                        ControlToValidate="tbLastName" ErrorMessage="Please specify a last name." EnableClientScript="false" />
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lEmailAddress" runat="server">Email Address:</asp:Literal><span
                        class="requiredField">*</span>
                </td>
                <td>
                    <asp:TextBox ID="tbEmail" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="None"
                        ControlToValidate="tbEmail" ErrorMessage="Please specify an email address." EnableClientScript="false" />
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lRelationship" runat="server">Role(s):</asp:Literal><span class="requiredField">*</span>
                </td>
                <td>
                    <asp:ListBox ID="lbRoles" runat="server" SelectionMode="Multiple" />
                    <asp:RequiredFieldValidator ID="rfvRoles" runat="server" Display="None"
                        ControlToValidate="lbRoles" ErrorMessage="Please specify one or more roles for the person in this organization."
                        EnableClientScript="false" />
                </td>
            </tr>
        </table>
    </div>
  <%--  <h2>
            Discount Codes</h2>
        <asp:Literal ID="Literal1" runat="server">
    You can apply discount codes to the registration by entering them in the box below. Be sure to enter a separate code on each line.
        </asp:Literal>
        
    <asp:TextBox ID="tbDiscountCodes" TextMode="MultiLine" Rows=5 Columns=80 runat="server"/>--%>
        
    <hr width="100%" />
    <div class="sectionContent">
        <div align="center" style="padding-top: 20px">
            <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Continue" runat="server" />
            <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server"
                CausesValidation="false" />
            <div class="clearBothNoSPC">
            </div>
        </div>
    </div>

    <script type="text/javascript">
        showRegistrationInfo();
        function showRegistrationInfo() {
            var div = document.getElementById('divRegInfo');
            var ddl = document.getElementById( '<%= ddlIndividual.ClientID %>' );

            if (ddl.options[ddl.selectedIndex].value == '-1')
                div.style.display = '';
            else
                div.style.display = 'none';
            
        }
    </script>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
