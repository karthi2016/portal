<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyProfile.ascx.cs" Inherits="homepagecontrols_MyProfile" %>
<div class="sectCont">
    <div class="sectHeaderTitle hIconGlobe">
        <h2>
            <asp:Literal ID="Widget_MyProfile_Title" runat="Server">My Profile</asp:Literal></h2>
    </div>
    <table>
        <tr>
            <td width="140px" id="tdProfileImage" runat="server">
                <div id="myAccountProfileImg">
                    <asp:Image ID="img" Width="150"   runat="server" BorderWidth="1px" ImageUrl="~/Images/noimage.gif" />
                </div>
            </td>
            <td valign="top">
                <p class="myAccountNumber" id="trMyAccountNumber" runat="server">
                    <asp:Literal ID="Widget_MyProfile_CustomerID" runat="Server">Your Customer ID: </asp:Literal><span
                        class="highlight">
                        <%= ConciergeAPI.CurrentEntity.LocalID  %>
                    </span>
                </p>
                <span id="trMyName" runat="server">
                    <div id="myAccountProfileInfo">
                        <%= ConciergeAPI.CurrentEntity.Name  %>
                        <br />
                        <asp:Label ID="lblCompany" runat="server" />
                        <asp:Literal ID="lAddress" runat="server" />
                    </div>
                </span>
            </td>
        </tr>
    </table>
    <div class="clearBothNoSPC">
    </div>
    <p>
        <span id="trPreferredContactNumber" runat="server">
            <asp:Literal ID="Widget_MyProfile_PreferredContact" runat="Server"><b>Preferred Contact #:</b></asp:Literal>
            <asp:Literal ID="lPhoneNumber" runat="server">(none)</asp:Literal></span>
        <br />
        <span id="trEmail" runat="server">
            <asp:Literal ID="Widget_MyProfile_Email" runat="Server"><b>Email:</b></asp:Literal>
            <asp:HyperLink ID="hlEmail" runat="server" Text="(no email on file)" />
        </span>
        <br />
         <asp:Literal ID="Widget_MyProfile_LoginID" runat="Server"><b>Login ID:</b></asp:Literal>
         <%=ConciergeAPI.CurrentUser.Name %> <a href="/Profile/ChangeLoginId.aspx">(change)</a>
    </p>
    <br />
    <ul class="buttonList">
        <li>
            <asp:HyperLink ID="hlEditMyInfo" runat="server" CssClass="iconBtn iconBtnGear" NavigateUrl="~/profile/EditIndividualInfo.aspx">Edit My Information</asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="hlChangePassword" runat="server" CssClass="iconBtn iconBtnGear"
                NavigateUrl="~/profile/ChangePassword.aspx?n=/">Change My Password</asp:HyperLink>
        </li>
        <li runat="server" id="liManageContacts"><a href="/profile/ManageContacts.aspx" class="iconBtn iconBtnGear">
            Manage Organization Contacts</a></li>
            <li runat="server" id="liVolunteer" visible="false">
            <asp:HyperLink ID="hlViewVolunteerProfile" runat="server" NavigateUrl="/volunteers/ViewMyVolunteerProfile.aspx?contextID=" CssClass="iconBtn iconBtnGear" Text="View My Volunteer Profile" Font-Bold=true />
            </li>
            <li runat="server" id="liDigitalLibrary" visible="false">
            <asp:HyperLink ID="hlDigitalLibrary" runat="server" NavigateUrl="/documents/DigitalLibrary.aspx" CssClass="iconBtn iconBtnGear" Text="My Digital Library" Font-Bold=true />
            </li>
            <li runat="server" id="liReports" visible="false">
            <asp:HyperLink ID="hlReports" runat="server" NavigateUrl="/profile/MyReports.aspx" CssClass="iconBtn iconBtnGear" Text="My Reports" Font-Bold=true />
            </li>
    </ul>
    <div class="clearBothNoSPC">
    </div>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
    
    
    <br />
    <div runat="server" id="divLoginAs" visible="false">
        <asp:Label ID="lblLoginAsInstructions" runat="server" />
        <asp:BulletedList runat="server" ID="blLoginAs" DisplayMode="LinkButton" OnClick="blLoginAs_Click" />
    </div>
    <div class="clearBothNoSPC">
    </div>
</div>
