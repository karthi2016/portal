<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App_Master/GeneralPage.master"
    CodeFile="ErrorPage.aspx.cs" Inherits="ErrorPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Oops! Something Went Wrong!
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <table style="width: 800px;">
        <tr>
            <td>
                <p>
                    It looks like we experienced an error. Please try your action again.
                </p>
            </td>
        </tr>
        <tr id="trDetails" runat="server" visible="false">
            <td>
                <p>
                    <asp:Literal runat="server" ID="litErrorSummary"></asp:Literal>
                </p>
            </td>
        </tr>
        <tr>
            <td>
                <p>
                    When you're ready, <a href="/">click here to return to the home screen.</a>
                </p>
            </td>
        </tr>
    </table>
    <div runat="server" id="divErrorDetails" visible="false">
        <a href="javascript:showError()">Show Error Details &gt;&gt;</a>
        <div id="errorDetails" style="display: none">
            <asp:Literal ID="litErrorDetails" runat="server">Error Details</asp:Literal>
        </div>
        <script type="text/javascript">
            function showError() {
                document.getElementById('errorDetails').style.display = '';
            }
        </script>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
