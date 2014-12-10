<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="EditSectionInfo.aspx.cs" Inherits="sections_EditSectionInfo" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="/sections/ViewSection.aspx?contextID=<%=targetSection.ID %>">
        <%=targetSection.Name%>
        ></a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="Server">
    Edit Section Information
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">

        <div class="sectionHeaderTitle">
            <h2>
                <ASP:Literal ID="lBasicInfo" runat="Server">Basic Information</ASP:Literal></h2>
        </div>
        <div class="sectionContent">
            <span class="requiredField">*</span> - indicates a required field.
            <table style="width: 100%">
                <tr>
                    <td class="columnHeader">
                         <ASP:Literal ID="lSectionName" runat="Server">Section Name:</ASP:Literal><span class="requiredField">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="tbName" runat="server" TabIndex="10" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Please enter a section name."
                            ControlToValidate="tbName" Display="None" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <ASP:Literal ID="lDescription" runat="Server">Description</ASP:Literal></h2>
        </div>
        <div class="sectionContent">
            <telerik:RadEditor runat="server" ID="reDescription" ToolsFile="~/controls/telerik/ToolsFileDeluxe.xml" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divPhoneNumbers">
        <div class="sectionHeaderTitle">
            <h2>
             <ASP:Literal ID="lPhoneNumbers" runat="Server">Phone Numbers </ASP:Literal></h2>
        </div>
        <div class="sectionContent">
             <ASP:Literal ID="lAtLeastOne" runat="Server">Please enter your contact numbers below. Use the <b>Preferred?</b> radio
            button to indicate the phone number at which you prefer to be contacted.</ASP:Literal>
            <asp:GridView ID="gvPhoneNumbers" OnRowDataBound="gvPhoneNumbers_OnRowDataBound"
                GridLines="None" AutoGenerateColumns="false" Width="460px" runat="server">
                <Columns>
                    <asp:TemplateField HeaderStyle-BackColor="White">
                        <ItemTemplate>
                            <asp:Label ID="lblPhoneNumberType" runat="server" />
                            Phone Number:
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-BackColor="White">
                        <ItemTemplate>
                            <asp:TextBox ID="tbPhoneNumber" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                        HeaderText="Preferred?" HeaderStyle-CssClass="columnHeader" HeaderStyle-BackColor="White">
                        <ItemTemplate>
                            <!-- We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                    in repeater controls

                    http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->
                            <asp:Literal ID="lRadioButtonMarkup" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divAddressInformation">
        <div class="sectionHeaderTitle">
            <h2>
                 <ASP:Literal ID="lAddressInfo" runat="Server">Address Information</ASP:Literal></h2>
        </div>
        <div class="sectionContent">
             <ASP:Literal ID="lAtLeastOneAddress" runat="Server">Please enter your addresses below.</ASP:Literal>
            <div>
                <table cellpadding="0" cellspacing="0" style="margin-top: 10px;">
                    <tr>
                        <asp:Repeater ID="rptAddresses" OnItemCreated="rptAddresses_OnItemCreated" OnItemDataBound="rptAddresses_OnItemDataBound"
                            runat="server">
                            <ItemTemplate>
                                <td>
                                    <b><u>
                                        <asp:Label ID="lblAddressType" runat="server"></asp:Label>
                                        Address</u></b>
                                    <br />
                                    <asp:HiddenField ID="hfAddressCode" runat="server" />
                                    <cc1:AddressControl ID="acAddress" runat="server" />
                                </td>
                                <asp:Literal ID="lNewRowTag" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                    <asp:Literal ID="lAddressEndRow" runat="server" />
                </table>
            </div>
            <div style="padding-top: 10px">
                <h3>
                     <ASP:Literal ID="lAddressPrefs" runat="Server">Address Preferences</ASP:Literal></h3>
            </div>
            <table style="width: 650px">
                <tr>
                    <td>
                         <ASP:Literal ID="lWhatIsYourPreferred" runat="Server">What is your preferred mailing address?</ASP:Literal>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPreferredAddress" runat="server" />
                    </td>
                </tr>
                <tr id="trSeasonality" runat="server" visible="false">
                    <td style="padding-top: 10px">
                        Would you like to receive mail to your
                        <asp:Label ID="lblSeasonalAddressType" runat="server" />
                        address during certain times of the year?
                    </td>
                    <td style="width: 400px">
                        <asp:RadioButton ID="rbSeasonalYes" runat="server" Text="Yes" GroupName="Seasonal" />
                        - from
                        <cc1:MonthDayPicker ID="mdpSeasonalStart" runat="server" />
                        to
                        <cc1:MonthDayPicker ID="mdpSeasonalEnd" runat="server" />
                        <br />
                        <asp:RadioButton ID="rbSeasonalNo" runat="server" Text="No, that will not be necessary"
                            GroupName="Seasonal" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divOtherInformation">
        <div class="sectionContent">
            <uc1:CustomFieldSet ID="cfsSectionFields" runat="server" />
        </div>
    </div>
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
