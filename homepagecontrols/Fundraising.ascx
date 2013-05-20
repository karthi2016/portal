<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Fundraising.ascx.cs" Inherits="homepagecontrols_Fundraising" %>
      <div class="sectCont" runat="server" id="divFundraising">
                <div class="sectHeaderTitle hIconDollar">
                    <h2>
                       <asp:Literal ID="Widget_Fundraising_Title" runat="server">Donations</asp:Literal></h2>
                </div>
                <table>
                    <tr>
                        <td class="columnHeader">
                            <asp:Literal ID="Widget_Fundraising_LastDonation" runat="server">Last Donation:</asp:Literal>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblLastDonation" />
                        </td>
                    </tr>
                </table>
                <p />
                <ul style="margin-left: -20px">
                    <asp:HyperLink ID="Widget_Fundraising_hlMakeDonation" runat="server" NavigateUrl="/donations/MakeDonation.aspx"><li>Make a Donation</li></asp:HyperLink> 
                    <asp:HyperLink ID="Widget_Fundraising_ViewDonationHistory" runat="server" NavigateUrl="/donations/BrowseDonations.aspx"><li>View Donation History</li></asp:HyperLink>
                     
                </ul>
                <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
            </div>