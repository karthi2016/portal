<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="RegisterForBooths.aspx.cs" Inherits="exhibits_RegisterForBooths" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <a href="/exhibits/ViewShow.aspx?contextID=<%=targetShow.ID %>">
        <%=targetShow.Name%>
        > </a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <%=targetShow.Name%>
    Registration
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="lPageText" runat="server">During this registration window, you can select and purchase booths directly.</asp:Literal>
    <asp:Literal ID="lMainInstructions" runat="server" /><br />
    <asp:Literal ID="lRegistrationWindowInstructions" runat="server" /><br />
    <asp:HyperLink ID="lShowFloor" runat="server" Text="Download Show Floor Layout<br /><br />" />
    <h2>
    <asp:Literal ID="lSelectBooths" runat="server">Select Your Booth(s) Below:</asp:Literal></h2>
    <cc1:DualListBox runat="server" ID="dlbCategories" Width="400" />
      <div id="divOtherProducts" runat="server" style="padding-top: 20px" visible="false">
        <h2>
            <asp:Literal ID="lAdditionalItems" runat="server">Additional Items</asp:Literal></h2>
        <asp:Literal ID="lIfYouWouldLikeToAddAnyItems" runat="server">
        If you would like to add any of the items below to your registration, enter the quantities
        below.
        </asp:Literal>
        <table style="width: 100%; margin-top: 20px">
            <asp:Repeater ID="rptAdditionalItems" runat="server" OnItemDataBound="rptAdditionalItems_ItemDataBound">
                <ItemTemplate>
                    <tr>
                        <td style="width: 100px">
                            <asp:HiddenField ID="hfProductID" runat="server" />
                            <asp:TextBox ID="tbQuantity" runat="server" Text="0" Width="30" />
                            <asp:CompareValidator ID="cvQuantity" runat="server" ControlToValidate="tbQuantity"
                                Operator="GreaterThanEqual" ValueToCompare="0" ErrorMessage="One or more item quantities are invalid."
                                Type="Integer" Display="None" />
                        </td>
                        <td>
                            <asp:Label ID="lblProductName" runat="server" />
                        </td>
                        <td style="width: 50px">
                            <asp:Label ID="lblProductPrice" CssClass="price" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    <h2>
        Special Requests</h2>
    <asp:Literal ID="lSpecialRequestInstructions" runat="server" >Use the space below to enter any special request you have for your exhibit.</asp:Literal>
 <br />
    <asp:TextBox ID="tbSpecialRequest" Columns="125" Rows="10" TextMode="MultiLine" runat="server" />
    <div style="padding-top: 30px">
    <asp:Literal ID="lAfterRegComplete" runat="server">
    After your registration is complete, you will have a chance to upload your booth
    logo and bio.</asp:Literal>
    </div>
    <hr />
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Continue" OnClick="btnContinue_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
