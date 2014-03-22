<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitle" runat="Server">
<asp:Literal ID="lLoginScreenTitle1" runat="server" />
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server"/>
    <table style="width: 100%">
        <tr>
            <td style="width: 30%">
                <br />
                <table style="width: 100%">
                    <tr>
                        <th colspan="2">
                            <asp:Literal ID="lEnterLoginInfo" runat="server" Text="Enter your login information"/>
                        </th>
                    </tr>
                <tr>
                        
                        <td>
                            <asp:Literal ID="lLoginId" runat="server" Text="Login ID: "/>
                            <span class="requiredField">*</span>
                       <br />
                            <asp:TextBox ID="tbLoginID" Style="width: 160px" runat="server" />
                            <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ErrorMessage="Please enter a login ID"
                                ControlToValidate="tbLoginID" Display="None" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Literal ID="lPassword" runat="server" Text="Password: "/>
                            <span class="requiredField">*</span>
                            
                        <br />
                            <asp:TextBox ID="tbPassword" Style="width: 160px" TextMode="Password" runat="server" /> 
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Please enter a password"
                                ControlToValidate="tbPassword" Display="None" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <a href="/profile/ForgotPassword.aspx">Forgot your password? Click here</a>
                            <br />
                            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                            <asp:CustomValidator ID="cvLoginValidator" runat="server" Display="None" ErrorMessage="CustomValidator"></asp:CustomValidator>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%; margin-top: 20px">
                    <tr>
                        <th>
                            <asp:Literal ID="lNewUsers" runat="server" Text="New Users"/>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <div runat="server" id="divCreateAccount">
                                <asp:HyperLink runat="server" ID="hlCreateAccount"
                                    Text="Create a User Account" NavigateUrl="~/profile/CreateAccount_BasicInfo.aspx" /><br />
                            </div>
                            <div runat="server" id="divBecomeMember">
                                <asp:HyperLink runat="server" ID="hlBecomeMember"
                                    Text="Become a Member" NavigateUrl="~/membership/RegisterForMembership.aspx" /><br />
                            </div>
                            <div runat="server" id="divMakeDonation">
                                <asp:HyperLink runat="server" ID="hlMakeDonation"
                                    Text="Make a Donation" NavigateUrl="~/donations/MakeDonation.aspx" />
                            </div>
                            <asp:Repeater ID="rptForms" runat="server">
                            <ItemTemplate>
                            <a href="/forms/CreateFormInstance.aspx?contextID=<%#DataBinder.Eval(Container.DataItem, "FormID") %>">
                            <%#DataBinder.Eval(Container.DataItem, "CreateLink") %>
                            </a><br />
                            </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 10px" />
            <!-- spacer coloumn -->
            <td valign="top">
                <asp:Literal ID="SkinLoginText" runat="server">
                    <h2>
                        Welcome!</h2>
                   
                    <p>
                        With this portal, you will have the capability to update your own membership information,
                        renew your membership, and register for events.
                    </p>
                    <p>
                       Please log in with your username and password in the box to the left. If you don't know your password,
                       or you've forgotten it, select <b>Forgot Password</b>. 
                    </p>
                </asp:Literal>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
