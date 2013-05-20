<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="Register_SelectFee.aspx.cs" Inherits="events_Register_SelectFee" %>

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
    Event Registration - Create Registration
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <asp:Panel ID="pnlGroupRegistration" runat="server" Visible="false">
        <asp:Literal ID="lGroupRegNotice" runat="server">
 <font color=green><B>GROUP REGISTRATION MODE</B></font>
        </asp:Literal>
        <table style="width: 500px; margin-top: 5px">
            <tr>
                <td class="columnHeader" style="width: 100px">
                    <asp:Literal ID="lGroup" runat="Server">Group:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblGroup" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Literal ID="lRegistrant" runat="Server">Registrant:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblRegistrant" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSelectFee" runat="Server">Select a Registration Fee</asp:Literal>
            </h2>
        </div>
        <div class="sectionContent">
            <%=targetEvent.RegistrationFeeInstructions %>
            <table>
                <tr>
                    <td>
                        <asp:Label CssClass="redHighlight" ID="lblExistingRegistration" runat="server" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButtonList runat="server" ID="rblRegistrationFees" DataTextField="ProductName"
                            DataValueField="ProductID" OnDataBound="rblRegistrationFees_DataBound">
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator runat="server" ID="rfvRegistrationFees" ControlToValidate="rblRegistrationFees"
                            Display="None" ErrorMessage="Please select a registration fee." />
                        <asp:Label CssClass="redHighlight" ID="lblNoRegistrationFees" runat="server" Visible="false">No registration fees are available for you – you may not be eligible to register for this event.</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button runat="server" ID="btnContinue" OnClick="btnContinue_Click" CausesValidation="true"
                            Text="Continue" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
