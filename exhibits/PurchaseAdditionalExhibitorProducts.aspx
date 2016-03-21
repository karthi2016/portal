<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="PurchaseAdditionalExhibitorProducts.aspx.cs" Inherits="exhibits_PurchaseAdditionalExhibitorProducts" %>

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
    <asp:Literal runat="server" ID="CustomTitle"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
     
      <div id="divOtherProducts" runat="server" style="padding-top: 20px">
        <h2>
            <asp:Literal ID="lAdditionalItems" runat="server">Additional Items</asp:Literal></h2>
        <asp:Literal ID="lIfYouWouldLikeToAddAnyItems" runat="server">
        If you would like to add any of the items below to your registration, enter the quantities
        below.
        </asp:Literal>
         <asp:CustomValidator ID="cvAtLeastOne" runat="server" ForeColor=Red ErrorMessage="Error: You must select at least one product." Display=Dynamic />
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
    
    <hr />
    <div align="center">
        <asp:Button ID="btnSave" runat="server" Text="Continue" OnClick="btnContinue_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
