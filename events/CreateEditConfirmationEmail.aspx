<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="CreateEditConfirmationEmail.aspx.cs" Inherits="events_CreateEditConfirmationEmail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<script type="text/javascript" language="javascript">
    function updateMergeFieldCode() {
        var ddlMergeField = document.getElementById('<%=ddlMergeFields.ClientID %>');
        var tbMergeFieldCode = document.getElementById('<%=tbMergeFieldCode.ClientID %>');

        tbMergeFieldCode.value = '##' + ddlMergeField.options[ddlMergeField.selectedIndex].value + '##';
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
    Confirmation Email
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lEmailTemplateInfo" runat="server">Email Template Information</asp:Literal> </h2>
        </div>
        
        <div class="sectionContent">
            <table>
                <tr runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lEvent" runat="server">Event:</asp:Literal>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblEventName"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lTemplateName" runat="server">Template Name:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbName"  />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" Display="None" ErrorMessage="Please specify a value for Name."
                            ControlToValidate="tbName" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lTemplateTarget" runat="server">Template Target:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlTemplateTarget" DataTextField="Text" DataValueField="Value" AppendDataBoundItems="true">
                            <asp:ListItem Text="Event Registration" Value="EventRegistration" Selected="True" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Literal ID="lDescription" runat="server">Description:</asp:Literal>
                        <asp:TextBox runat="server" ID="tbDescription" Columns="130" Rows="5" TextMode="MultiLine" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lEmailTemplateHeader" runat="server">Email Template</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lEmailFrom" runat="server">From:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbFrom" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lEmailSubject" runat="server">Subject:</asp:Literal> <span class="redHighlight">*</span>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbSubject" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvSubject" Display="None" ErrorMessage="Please specify a value for Subject."
                            ControlToValidate="tbSubject" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lEmailCC" runat="server">CC:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbCC" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lEmailBCC" runat="server">BCC:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbBCC" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lMergeFieldsHeader" runat="server">Merge Fields</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Literal ID="lMergeFieldsExplanation" runat="server">You can insert merge fields in your email to personalize the messages to the recipients.
            Use the links below to generate fields that can be interpreted by the email. <b>Hint:</b>
            These merge fields work in the Subject line as well.</asp:Literal>
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lMergeFields" runat="server">Merge Fields:</asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMergeFields" runat="server" DataTextField="Name" DataValueField="Value" OnChange="javascript:updateMergeFieldCode();"  />
                    </td>
                    <td class="columnHeader">
                        <asp:Literal ID="lMergeCode" runat="server">Code:</asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbMergeFieldCode" Width="250" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lHTMLMessageBody" runat="server">HTML Message Body</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <telerik:RadEditor NewLineBr="false" runat="server" ID="reHtmlMessageBody" ToolsFile="~/controls/telerik/ToolsFileDeluxe.xml" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTextOnlyMessageBody" runat="server">Text-Only Message Body</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Literal ID="lTextOnlyExplanation" runat="server">You can leave this blank, and we will automatically create a text-only email template
            from the template above. If you want more control over the text sent, you can enter
            in below.</asp:Literal>
            <asp:TextBox runat="server" ID="tbTextOnlyMessageBody" Columns="130" Rows="5" TextMode="MultiLine" />
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
