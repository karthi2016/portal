<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="Register_RegistrationForm.aspx.cs" Inherits="events_Register_RegistrationForm" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<script language="javascript">

    function ClientValidation(source, args) {

        args.IsValid = document.all["PageContent_chkAcknowledgement"].checked;

    } 
 
</script>
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
    Event Registration
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Panel ID="pnlGroupRegistration" runat="server" Visible="false">
        <asp:Literal ID="lGroupRegNotice" runat="server">
            <span class="hlte"><B>GROUP REGISTRATION MODE</B></span>
        </asp:Literal>
        <table style="width: 500px; margin-top: 5px" id="GroupRegHeader">
            <tr id="EventRegistrantNameRow">
                <td class="columnHeader" style="width: 100px">
                    <asp:Literal ID="lGroup" runat="Server">Group:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblGroup" runat="server" />
                </td>
            </tr>
            <tr id="EventRegistrantFeeRow">
                <td class="columnHeader">
                    <asp:Literal ID="lRegistrant" runat="Server">Registrant:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblRegistrant" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent" style="margin-top: 10px">
            <%=targetEvent.RegistrationFormInstructions %>
            <br />
            <uc1:CustomFieldSet ID="cfsRegistrationFields" runat="server" />
        </div>
        <div class="sectionContent" id="divAcknowledgement" runat="server">
            <%=targetEvent.AcknowledgementText %>
            <asp:CheckBox runat="server" ID="chkAcknowledgement" />
            <asp:CustomValidator runat="server" ID="cvAcknowledgement" ClientValidationFunction="ClientValidation" ErrorMessage="You must indicate your acknowledgement by checking the box to continue. " Display="None" />
        </div>
        <div class="sectionContent">
            <hr width="100%" />
            <div align="center" style="padding-top: 20px">
                <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Continue" runat="server" />
                <asp:Button ID="btnBack" OnClick="btnBack_Click" Text="Back" runat="server" CausesValidation="False" />
                <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server" CausesValidation="False" />
                <div class="clearBothNoSPC">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
