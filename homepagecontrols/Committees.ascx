<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Committees.ascx.cs" Inherits="homepagecontrols_Committees" %>
   <div class="sectCont" runat="server" id="divCommittees">
            <div class="sectHeaderTitle hIconCommittees">
                <h2>
                   <asp:Literal ID="Widget_MyCommittees_Title" runat="server">My Committees</asp:Literal> </h2>
            </div>
            <p />
           <asp:Literal ID="Widget_MyCommittees_CurrentlyOn" runat="server"> <B>Committees I am currently on:</B></asp:Literal> <asp:Label ID="lblCommitteeCount" runat="server" />
            <asp:GridView runat="server" ID="gvCommitteeMemberships" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No visible committees found.">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFormatString="~\committees\ViewCommittee.aspx?contextID={0}"
                        DataTextField="Committee.Name" DataNavigateUrlFields="Committee.ID" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="Committee" />
                    <asp:BoundField DataField="Position.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Position" />
                </Columns>
            </asp:GridView>
            <ul style="margin-left: -20px">
                <ASP:HyperLink ID="Widget_MyCommittees_hlViewMyCommittees" runat="server" NavigateUrl="/committees/ViewMyCommittees.aspx"><li>View My Committees</li></ASP:HyperLink>
                <ASP:HyperLink ID="Widget_MyCommittees_hlBrowseCommittees" runat="server" NavigateUrl="/committees/BrowseCommittees.aspx"><li>Browse Committees</li></ASP:HyperLink>
            </ul>
            <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
        </div>