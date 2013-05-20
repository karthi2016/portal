<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="BrowseFileFolder.aspx.cs" Inherits="documents_BrowseFileFolder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" type="text/css" href="../Images/FolderBrowser.css" />
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
    <asp:Label ID="lblFolderName" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div id="divMain">
        <div style="float: right">
            <asp:HyperLink ID="hlUploadFile" runat="server">Upload File(s) to this Folder</asp:HyperLink>
            <asp:HyperLink ID="hlCreateSubFolder" runat="server">Create a New Folder</asp:HyperLink>
            <asp:Label ID="lblNoWriteAccess" ForeColor="Red" Visible="false" runat="server">You are not authorized to upload files into this folder.</asp:Label>
        </div>
        <div id="divFolderDescription">
            <asp:Literal ID="lFolderDescription" runat="server">
            This folder has no description.</asp:Literal>
            <asp:HyperLink ID="hlChangeFolder" runat="server">(edit folder info)</asp:HyperLink>
        </div>
        <div align="right" id="divFolderType" runat='server'>
            <b>Type: </b>
            <asp:Literal ID="lFolderType" runat="server">Private/Restricted Access</asp:Literal>
        </div>
        <br />
        <asp:PlaceHolder ID="phMainFolderTable" runat="server">
            <table style="width: 650px">
                <tr style="margin-bottom: 20px">
                    <td>
                    </td>
                    <td style="width: 160px">
                        &nbsp;
                    </td>
                    <td class="divFolderActions">
                        <span id="spanCheckAll" runat="server">
                            <input type="checkbox" id="checkAllBox" onclick="checkOruncheckAll()" />
                        </span>
                    </td>
                </tr>
                <asp:Repeater ID="rptFolders" runat="server" OnItemDataBound="rptFolders_OnItemDataBound"
                    OnItemCommand="rptFolders_OnItemCommand">
                    <ItemTemplate>
                        <tr class="documentItemRow">
                            <td>
                                <table class="killTablePadding folderTable">
                                    <tr class="documentItem">
                                        <td rowspan="2" style="width: 50px;" class="documentIcon">
                                            <asp:Image ID="imgIcon" runat="server" />
                                        </td>
                                        <td style="width: 10px">
                                        </td>
                                        <td style="padding-top: 7px" class="divFolderName">
                                            <asp:HyperLink ID="hlItemName" runat="server" /><asp:HiddenField ID="hfID" runat="server" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10px">
                                        </td>
                                        <td class="divFolderDetails" colspan="2">
                                            <asp:Label ID="lblItemDescription" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="divFolderActions">
                                <asp:HyperLink ID="hlEdit" runat="server" CssClass="editImage">
                                   (edit)
                                </asp:HyperLink>
                                <asp:HyperLink ID="hlCopy" runat="server" Visible="false" CssClass="editImage">
                                   (copy)
                                </asp:HyperLink>
                                <asp:HyperLink ID="hlMove" runat="server" CssClass="editImage">
                                   (move)
                                </asp:HyperLink>
                                <asp:LinkButton ID="hlDelete" runat="server" CssClass="editImage">
                                   (delete)
                                </asp:LinkButton>
                            </td>
                            <td class="divFolderActions">
                                <asp:CheckBox ID="cbSelect" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Repeater ID="rptFiles" runat="server" OnItemDataBound="rptFiles_OnItemDataBound"
                    OnItemCommand="rptFiles_OnItemCommand">
                    <ItemTemplate>
                        <tr class="documentItemRow">
                            <td>
                                <table class="killTablePadding folderTable">
                                    <tr class="documentItem">
                                        <td rowspan="2" style="width: 50px;" class="documentIcon">
                                            <asp:Image Width="50" Height="50" ID="imgIcon" runat="server" />
                                        </td>
                                        <td style="width: 10px">
                                        </td>
                                        <td style="padding-top: 7px" class="divFolderName">
                                            <asp:HyperLink ID="hlItemName" runat="server" /><asp:HiddenField ID="hfID" runat="server" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10px">
                                        </td>
                                        <td class="divFolderDetails" colspan="2">
                                            <asp:Label ID="lblItemDescription" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="divFolderActions">
                                <asp:HyperLink ID="hlEdit" runat="server" CssClass="editImage">
                                   (edit)
                                </asp:HyperLink>
                                <asp:HyperLink ID="hlCopy" runat="server" Visible="false" CssClass="editImage">
                                   (copy)
                                </asp:HyperLink>
                                <asp:HyperLink ID="hlMove" runat="server" CssClass="editImage">
                                   (move)
                                </asp:HyperLink>
                                <asp:LinkButton ID="hlDelete" runat="server" CssClass="editImage">
                                   (delete)
                                </asp:LinkButton>
                            </td>
                            <td class="divFolderActions">
                                <asp:CheckBox ID="cbSelect" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td>
                    </td>
                    <td colspan="2" style="text-align: right">
                        <asp:Button ID="btnDeleteSelected" runat="server" Text="Delete Selected" OnClick="btnDeleteSelected_Click" />
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <div align="center" style="text-align: center" id="divEmptyFolder" runat="server"
            visible="false">
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <img src="../Images/documenticons/emptyfolder.jpg" style="text-align: center" />
            <div class="emtpyFolderDeclaration">
                This Folder is Empty!
            </div>
            There are no files in this folder.
        </div>
    </div>
    <script type="text/javascript">
        function checkOruncheckAll() {
            
                var cb = document.getElementById('checkAllBox');
                <ASP:Literal ID="lSelectAllScript" runat="server" />
                
            }
    </script>
    <div align="center" style="padding-top: 40px">
        <hr />
        <asp:Button ID="btnBack" Text="Back to Committee" OnClick="btnBack_Click" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
