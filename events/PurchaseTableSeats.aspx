<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="PurchaseTableSeats.aspx.cs" Inherits="events_PurchaseTableSeats" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlEventOwner" Visible="false" />
    <a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name %> Purchase Seats
        ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Purchase Table Seats
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
      <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lSelectFee" runat="Server">Select a Table Fee</asp:Literal>
            </h2>
        </div>
        <div class="sectionContent">
            <%=targetEvent.RegistrationFeeInstructions %>
            
             <table style="width: 100%; margin-top: 20px">
            <asp:Repeater ID="rpTableProducts" runat="server" OnItemDataBound="rpTableProducts_ItemDataBound">
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
    </div>
     <hr style="width: 100%" />
    <div style="text-align: center">
        <asp:Button ID="btnContinue" OnClick="btnSave_Click" runat="server" Text="Continue" />
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Cancel"
            OnClick="btnCancel_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
