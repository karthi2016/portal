<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditRegistrationQuestion.aspx.cs" Inherits="events_CreateEditRegistrationQuestion" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls.CascadingDropDown"
    TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="/js/CascadingDropDownScripts.js"></script>
    <script type="text/javascript" language="javascript">
        function SetLookupTableAjaxComboBox() {
            var ddlData = document.getElementById('<%=ddlDataType.ClientID %>');
            var ddlDisplay = document.getElementById('<%=ddlDisplayType.ClientID %>');
            var ddlLookup = document.getElementById('<%=ddlLookupTables.ClientID %>');

            if (ddlLookup.options[ddlLookup.selectedIndex].value == '0') {
                updateCascadingDropDown(ddlData);
                showAppropriatePanelsForDisplayType();
            }
            else {
                ddlDisplay.options.length = 0;
                var ajaxOption = document.createElement('OPTION');
                ajaxOption.text = 'AJAX Combo Box';
                ajaxOption.value = 'AjaxComboBox';
                ddlDisplay.options[0] = ajaxOption;
                showAppropriatePanelsForDisplayType();
            }
        }
    </script>
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
    Registration Question
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <cc1:CascadingDropDownManager ID="CascadingDropDownManager1" runat="server">
    </cc1:CascadingDropDownManager>
     <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <asp:Literal ID="lCreateQuestionHeader" runat="Server">Create/Edit Registration Question</asp:Literal> </h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lEvent" runat="Server">Event:</asp:Literal>
                    </td>
                    <td colspan="3" align="left">
                        <asp:Label runat="server" ID="lblEventName"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lFieldLabe" runat="Server">Field Label: </asp:Literal><span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbFieldLabel" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvFieldLabel" Display="None" ErrorMessage="Please specify a value for Field Label."
                            ControlToValidate="tbFieldLabel" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lAPIName" runat="Server">API Name:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbApiName" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDataType" runat="Server">Data Type:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlDataType" runat="server" DataTextField="Name" DataValueField="Value" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lRequired" runat="Server">Required?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkRequired" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDisplayType" runat="Server">Display Type:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlDisplayType" runat="server" DataTextField="Name" DataValueField="Value" />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lDefaultValue" runat="Server">Default Value:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbDefaultValue" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" id="S_AcceptableValues">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lAcceptableValues" runat="Server">Acceptable Values</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Literal ID="lAcceptableValuesExplanation" runat="Server">
                The type of field you selected allows the user to select from a list of acceptable
                values. You can enter the acceptable values here, or pull them from a lookup table
                - or both. </asp:Literal>
            <table>
                <tr>
                    <td colspan="2">
                        <asp:Literal ID="lSpecifyByHand" runat="Server">Specify Acceptable Values By Hand:</asp:Literal>
                        <asp:TextBox runat="server" ID="tbAcceptableValues" Columns="130" Rows="5" TextMode="MultiLine" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal ID="lUseLookupTable" runat="Server">Use Values from a System Lookup Table:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlLookupTables" DataTextField="Name" DataValueField="ID" AppendDataBoundItems="true">
                            <asp:ListItem Text="---- Select ----" Value="0" />
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px" id="S_Reference">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lReferenceOptions" runat="Server">Reference Options</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lReferenceType" runat="Server">Reference Type:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlReferenceType" DataTextField="Label" DataValueField="Name" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lPortalSettings" runat="Server">Portal Settings</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lDisplayOrder" runat="Server">Display Order:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbDisplayOrder" Width="75" />
                        <asp:RequiredFieldValidator ID="rfvDisplayOrder" runat="server" Display="None" ErrorMessage="Please specify a value for Display Order."
                            ControlToValidate="tbDisplayOrder" />
                        <asp:CompareValidator runat="server" ID="cvDisplayOrder" ControlToValidate="tbDisplayOrder"
                            Display="None" ErrorMessage="Please specify a valid number for Display Order."
                            Operator="GreaterThanEqual" ValueToCompare="0" />
                        <asp:CompareValidator runat="server" ID="cvDisplayOrderType" ControlToValidate="tbDisplayOrder"
                            Display="None" ErrorMessage="Please specify a valid number for Display Order."
                            Operator="DataTypeCheck" Type="Integer" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lRequiredInPortal" runat="Server">Required in portal?</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkRequiredInPortal" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Literal ID="lHelpText" runat="Server">Help Text:</asp:Literal>
                        <asp:TextBox runat="server" ID="tbHelpText" Columns="130" Rows="5" TextMode="MultiLine" />
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
