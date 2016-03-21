<%@ Page Title="" Language="C#" MasterPageFile="~/App_Master/GeneralPage.master"
    AutoEventWireup="true" CodeFile="RegisterForBoothTypes.aspx.cs" Inherits="exhibits_RegisterForBoothTypes" %>

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
    <asp:Literal ID="lPageText" runat="server">During this registration window, you can select the type of booth you would like to purchase. Once you
    select the booth type, you will be able to indicate your booth preferences.</asp:Literal>
    <asp:Literal ID="lMainInstructions" runat="server" /><br />
    <asp:Literal ID="lRegistrationWindowInstructions" runat="server" /><br />
    <asp:HyperLink ID="lShowFloor" runat="server" Text="Download Show Floor Layout<br /><br />" />
    <asp:Wizard ID="wzBoothType" runat="server" DisplayCancelButton="true" OnCancelButtonClick="wzBoothType_Cancel" OnNextButtonClick="wzBoothType_Next"
        FinishCompleteButtonText="Continue to Order/Payment" OnFinishButtonClick="wzBoothType_Finish" >
        <WizardSteps>
            <asp:WizardStep>
                <h2>
                    <asp:Literal ID="lSelectBooths" runat="server">Select Booth Type:</asp:Literal></h2>
                     <asp:CustomValidator ID="cvAtLeastOneBoothType" runat="server" ForeColor="Red" ErrorMessage="Error: You must select a booth type."
                    Display="Dynamic" />
                <asp:RadioButtonList ID="rblBoothTypes" runat="server" />
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
            </asp:WizardStep>
            <asp:WizardStep AllowReturn=true >
            <asp:Literal ID="lSelectedBoothType" runat="server"><b>Selected Booth Type: </b></asp:Literal> <asp:Label ID="lblBoothType" runat="server" />
                <h2>
                    <asp:Literal ID="Literal1" runat="server">Booth Preferences:</asp:Literal></h2>
                <asp:CustomValidator ID="cvAtLeastOneBooth" runat="server" ForeColor="Red" ErrorMessage="Error: You must select at least one booth."
                    Display="Dynamic" />
                <table>
                    <asp:Repeater ID="rptChoices" runat="server" OnItemDataBound="rptChoices_OnItemDataBound">
                        <ItemTemplate>
                            <tr>
                                <td style="width: 150px">
                                    <asp:Literal ID="lChoiceLabel" runat="server">1st Choice:</asp:Literal>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlChoice" runat="server" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
                <asp:PlaceHolder runat="server" ID="secSpecialRequests">
                    <h2>
                        <asp:Literal ID="lSpecialRequests" runat="server">Special Requests</asp:Literal></h2>
                    <asp:Literal ID="lSpecialRequestInstructions" runat="server">Use the space below to enter any special request you have for your exhibit.</asp:Literal>
                    <br />
                    <asp:TextBox ID="tbSpecialRequest" Columns="125" Rows="10" TextMode="MultiLine" runat="server" />
                </asp:PlaceHolder>
                <div style="padding-top: 30px">
                    <asp:Literal ID="lAfterRegComplete" runat="server">After your registration is complete, you will have a chance to upload your booth logo and bio.</asp:Literal>
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
