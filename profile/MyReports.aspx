<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="MyReports.aspx.cs" Inherits="profile_MyReports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" Runat="Server">
My Reports
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" Runat="Server">
<asp:Literal ID="lPageText" runat="server">
You have been granted access to the following reports:</asp:Literal>  <div style="padding-top:20px">&nbsp;</div>
    <asp:Repeater ID="rptFolders" runat="server"  
        >
        <ItemTemplate>
            <tr class="documentItemRow">
                <td>
                    <table class="killTablePadding folderTable">
                        <tr class="documentItem">
                            <td rowspan="2" style="width: 50px;" class="documentIcon">
                                <img src="../Images/icons/reportIcon.jpg" height=60 />
                            </td>
                            <td style="width: 10px">
                            </td>
                            <td style="padding-top: 7px" class="divFolderName">
                                <a href="ViewReport.aspx?contextID=<%#DataBinder.Eval(Container.DataItem,"ID") %>"><%#DataBinder.Eval(Container.DataItem,"Name") %></a>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px">
                            </td>
                            <td class="divFolderDetails" colspan="2">
                                <%#DataBinder.Eval(Container.DataItem,"Description") %>
                                <br />
                                <span style="font-size: 12px; color: #B0B0B0; font-style: italic;"><%#DataBinder.Eval(Container.DataItem,"Expiration") %></span>
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
