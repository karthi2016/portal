<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="MoveFile.aspx.cs" Inherits="documents_MoveFile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink ID="hlFolderContext" runat="server" />
    <asp:Repeater ID="rptParentFolders" runat="server">
        <ItemTemplate>
            &gt; <a href="BrowseFileFolder.aspx?contextID=<%#DataBinder.Eval(Container.DataItem,"FolderID") %>">
                <%#DataBinder.Eval(Container.DataItem,"FolderName") %></a>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Move File
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server" />
    <table>
        <tr>
            <td style="width: 150px" class="columnHeader">
                Source File:
            </td>
            <td>
                <asp:Label ID="lblSourceFolder" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 150px" class="columnHeader">
                New Destination: <span class="requiredField">*</span>
            </td>
            <td>
                <asp:DropDownList ID="ddlDestination" runat="server" Width="600" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlDestination"
                    ErrorMessage="ERROR: Please specify a file destination" Display="None" ForeColor="Red" />
            </td>
        </tr>
    </table>
    <hr />
    <div align="center">
        <asp:Button ID="btnUpload" Text="Move Folder" runat="server" OnClick="btnMove_Click" />
        <asp:Button ID="btnCancel" Text="Cancel" CausesValidation="false" runat="server"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
