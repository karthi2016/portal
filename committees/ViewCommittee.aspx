<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="ViewCommittee.aspx.cs" Inherits="committee_ViewCommittee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
<a href="BrowseCommittees.aspx">All Committees</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">

    View Committee
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><asp:Literal ID="lBasicInfo" runat="server">
            Committee Information</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <table style="width: 100%">
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lCommitteeID" runat="server">Committee ID:
                        </asp:Literal>
                    </td>
                    <td>
                        <%=targetCommittee.LocalID%>
                    </td>
                    <td class="columnHeader">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lCommitteeName" runat="server">Committee Name:
                        </asp:Literal>
                    </td>
                    <td>
                        <%=targetCommittee.Name%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <%=targetCommittee.Description%>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
            <asp:Literal ID="lCommitteeMembers" runat="server">
            Committee Members </asp:Literal>
            
            <asp:DropDownList runat="server" ID="ddlDisplayMembers" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="Name" DataValueField="ID" OnSelectedIndexChanged="ddlDisplayMembers_OnSelectedIndexChanged">
            <asp:ListItem Text="Current Members" Value="Current" Selected="True"></asp:ListItem>
            <asp:ListItem Text="All Members" Value="All"></asp:ListItem>
        </asp:DropDownList></h2>
        </div>
        <div class="sectionContent">
            <asp:UpdatePanel ID="upCommitteeMembership" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                <asp:GridView ID="gvCommitteeMembership" runat="server" GridLines="None" AutoGenerateColumns="false" EmptyDataText="No committee members found.">
                    <Columns>
                        <asp:BoundField DataField="Member.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name" />
                        <asp:BoundField DataField="Position.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Position" />
                        <asp:BoundField DataField="EffectiveStartDate" HeaderStyle-HorizontalAlign="Left" HeaderText="From" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="EffectiveEndDate" HeaderStyle-HorizontalAlign="Left" HeaderText="Until" DataFormatString="{0:d}"  />
                    </Columns>
                </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlDisplayMembers" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2><asp:Literal ID="lCommitteeTasks" runat="server">Committee Tasks
            </asp:Literal></h2>
        </div>
        <div class="sectionContent">
            <asp:Label ID="lblUnableToJoin" runat="server" ForeColor="Red" Visible="false">Based on membership requirements, you are ineligible to join this open committee.</asp:Label>
            <ul>
            <asp:HyperLink ID="hlEdit" runat="server" Visible="false" NavigateUrl="/committees/EditCommittee.aspx?contextID=" ><li>Update Committee Information</li></asp:HyperLink>
            <asp:HyperLink ID="hlDiscussionBoard" runat="server" NavigateUrl="/discussions/ViewDiscussionBoard.aspx?contextID=" ><li>View Discussion Board</li></asp:HyperLink>
            <asp:HyperLink ID="hlViewCommitteeDocuments" runat="server" Visible="false" NavigateUrl="/committees/ViewCommitteeDocuments.aspx?contextID=" ><li>View Committee Documents</li></asp:HyperLink>
            <asp:LinkButton ID="lbJoin" runat="server" Visible="false" OnClick="lbJoin_Click" ><li>Join this Committee</li></asp:LinkButton>
            <asp:LinkButton ID="lbRemove" runat="server" Visible="false" OnClick="lbRemove_Click" ><li>Remove Myself from this Committee</li></asp:LinkButton>
            <li><a href="BrowseCommittees.aspx"><asp:Literal ID="lBackToAllCommittees" runat="server">Back to All Committees</asp:Literal></a></li>
               <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
