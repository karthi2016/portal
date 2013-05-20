<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="DigitalLibrary.aspx.cs" Inherits="documents_DigitalLibrary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<link rel="stylesheet" type="text/css" href="../Images/FolderBrowser.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    My Digital Library
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server">You have been granted access to the following folders:</asp:Literal>
    <div style="padding-top:20px">&nbsp;</div>
    <asp:Repeater ID="rptFolders" runat="server"  
        >
        <ItemTemplate>
            <tr class="documentItemRow">
                <td>
                    <table class="killTablePadding folderTable">
                        <tr class="documentItem">
                            <td rowspan="2" style="width: 50px;" class="documentIcon">
                                <img src="../Images/documenticons/foldericon.jpg" />
                            </td>
                            <td style="width: 10px">
                            </td>
                            <td style="padding-top: 7px" class="divFolderName">
                                <a href="BrowseFileFolder.aspx?contextID=<%#DataBinder.Eval(Container.DataItem,"FolderID") %>"><%#DataBinder.Eval(Container.DataItem,"FolderName") %></a>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px">
                            </td>
                            <td class="divFolderDetails" colspan="2">
                                <%#DataBinder.Eval(Container.DataItem,"FolderDescription") %>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="divFolderActions">
                </td>
                <td class="divFolderActions">
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <hr />
    <div align=center>
    <asp:Button ID="btnGoHome" runat="server" Text="Go Home" OnClick="btnGoHome_Cick" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
