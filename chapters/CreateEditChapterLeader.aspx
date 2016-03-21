<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="CreateEditChapterLeader.aspx.cs" Inherits="chapters_CreateEditChapterLeader" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="/chapters/ViewChapter.aspx?contextID=<%=targetChapter.ID %>"><%=targetChapter.Name %> ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
        <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader"><asp:Literal ID="lMember" runat="server" >Member:</asp:Literal></td>
                    <td><asp:Label runat="server" ID="lblMember" /><asp:DropDownList ID="ddlMember" DataTextField="Membership.Owner.Name" DataValueField="Membership.Owner.ID" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanCreateMembers">
                    <td class="columnHeader"><asp:Literal ID="lCanCreateMembers" runat="server" >Can Create Members?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanCreateMembers" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanDownloadRoster">
                    <td class="columnHeader"><asp:Literal ID="lCanDownloadRoster" runat="server" >Can Download Roster?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanDownloadRoster" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanMakePayments">
                    <td class="columnHeader"><asp:Literal ID="lCanMakePayments" runat="server" >Can Make Payments?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanMakePayments" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanManageCommittees">
                    <td class="columnHeader"><asp:Literal ID="lCanManageCommittees" runat="server" >Can Manage Committees?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanManageCommittees" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanManageEvents">
                    <td class="columnHeader"><asp:Literal ID="lCanManageEvents" runat="server" >Can Manage Events?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanManageEvents" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanManageLeaders">
                    <td class="columnHeader"><asp:Literal ID="lCanManageLeaders" runat="server" >Can Manage Leaders?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanManageLeaders" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanUpdateInformation">
                    <td class="columnHeader"><asp:Literal ID="lCanUpdateChapterInfo" runat="server" >Can Update Chapter Info?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanUpdateInformation" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanUpdateContactInfo">
                    <td class="columnHeader"><asp:Literal ID="lCanUpdateMemberContactInfo" runat="server" >Can Update Member Contact Info?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanUpdateContactInfo" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanUpdateMembershipInfo">
                    <td class="columnHeader"><asp:Literal ID="lCanUpdateMembershipInfo" runat="server" >Can Update Membership Info?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanUpdateMembershipInfo" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanViewAccountHistory">
                    <td class="columnHeader"><asp:Literal ID="lCanViewAccountHistory" runat="server" >Can View Account History?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanViewAccountHistory" runat="server" /></td>
                </tr>
                <tr runat="server" id="trCanViewMembers">
                    <td class="columnHeader"><asp:Literal ID="lCanViewMembers" runat="server" >Can View Members?</asp:Literal></td>
                    <td><asp:CheckBox ID="chkCanViewMembers" runat="server" /></td>
                </tr>
                 <tr runat="server" id="trCanManageDocuments">
                    <td class="columnHeader"><asp:Literal ID="lCanManageDocuments" runat="server" >Can Manage Documents?</asp:Literal></td>
                    <td><asp:CheckBox ID="cbManageDocuments" runat="server" /></td>
                </tr>
            </table>
        </div>
    </div>
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>


