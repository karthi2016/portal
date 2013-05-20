<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true" CodeFile="Register_CreateRegistration.aspx.cs" Inherits="events_Register_CreateRegistration" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
       <script type="text/javascript">
           function SetUniqueRadioButton(nameregex, current) {
               re = new RegExp(nameregex);
               for (i = 0; i < document.forms[0].elements.length; i++) {
                   elm = document.forms[0].elements[i]
                   if (elm.type == 'radio') {
                       if (re.test(elm.name)) {
                           elm.checked = false;
                       }
                   }
               }
               current.checked = true;
           }

            </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
    <asp:HyperLink runat="server" ID="hlEventOwner" Visible="false" />
    <a href="/events/ViewEvent.aspx?contextID=<%=targetEvent.ID %>">
        <%=targetEvent.Name %>
        ></a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
     Event Registration - Select Fee
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
 <asp:Literal ID="PageText" runat="server"/>
 <asp:Panel ID="pnlGroupRegistration" runat="server" Visible="false">
        <asp:Literal ID="lGroupRegNotice" runat="server">
 <font color=green><B>GROUP REGISTRATION MODE</B></font>
        </asp:Literal>
        <table style="width: 500px; margin-top: 5px">
            <tr>
                <td class="columnHeader" style="width: 100px">
                    <asp:Literal ID="lGroup" runat="Server">Group:</asp:Literal>
                </td>
                <td>
                    <asp:Label ID="lblGroup" runat="server" />
                </td>
            </tr>
            
        </table>
    </asp:Panel>
    <div class="killTablePadding">
        <table >
            <tr>
                <td style="width: 200px">
                    <h3>
                       <asp:Literal ID="lRegistrant" runat="server">Registrant:</asp:Literal></h3>
                </td>
                <td>
                    <asp:Label ID="lblRegistrant" runat="server"> </asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <h3>
                        <asp:Literal ID="lFee" runat="server">Fee:</asp:Literal></h3>
                </td>
                <td>
                    <asp:Label ID="lblFee" runat="server"> </asp:Label>
                </td>
            </tr>
        </table>
    </div>

     <asp:Panel ID="pnlSessions" CssClass="section" Visible="false" runat="server" Style="padding-top: 10px">
    <div class="sectionContent">
        <div class="sectionHeaderTitle">
        <h2><asp:Literal ID="lSessionsWorkshops" runat="server">Sessions &amp; Workshops</asp:Literal></h2>
        </div>
        <div class="sectionContent">
            Hint: You can hover over a session to see its description.
            <asp:Repeater ID="rptSessions" runat="server"   OnItemDataBound="rptSessions_OnItemDataBound">
                <ItemTemplate>
                <div style="padding-top: 15px">
                    <h4>
                        <asp:Label ID="lblTimeSlot" runat="server" /></h4>
                        <hr width="100%" />
                        </div>
                        <div class="killAutoWidth">
                    <asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="false" DataKeyNames="SessionID"
                      GridLines="None" AlternatingRowStyle-CssClass="even" HeaderStyle-CssClass="tableHeaderRow"
                       ShowHeader="false"
                     OnRowDataBound="gvSessions_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="60px">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbRegister" Visible="false" runat="server" />
                                    <asp:RadioButton ID="rbRegister" Visible="false" runat="server" />
                                    <asp:TextBox ID="tbQuantity" Width="20px" CssClass="inputText" Visible="false" runat="server" Text="0" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="tbQuantity" ValueToCompare="0" Type="Integer" Operator="GreaterThanEqual" ErrorMessage="Please enter quantities of zero or more" Display="None" />
                                    
                                </ItemTemplate>
                            </asp:TemplateField>
                               <asp:TemplateField HeaderStyle-Width="60px" HeaderText="Session">
                                <ItemTemplate>
                                    <asp:Label ID="lblSessionName" runat="server"/>
                                    <telerik:RadToolTip ID="rtpSessionDescription" runat="server" TargetControlID="lblSessionName"/>
                                </ItemTemplate></asp:TemplateField>
                            
                            <asp:BoundField DataField="Tracks" HeaderText="Tracks" />
                        
                            <asp:TemplateField HeaderText="Fee" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlFee" runat="server" />
                                    <asp:Label ID="lblPrice" Visible="false" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView></div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    </asp:Panel>

    <asp:Panel ID="pnlGuests" CssClass="section" runat="server" Visible="false" Style="padding-top: 10px">
    <div class="sectionContent">
           <div class="sectionHeaderTitle">
                   <h2><asp:Literal ID="lGuestSpouseReg" runat="server">Guest/Spouse Registration</asp:Literal></h2>
            </div>
            <div class="sectionContent">
             <asp:GridView ID="gvGuests" runat="server" AutoGenerateColumns="false" DataKeyNames="ProductID"
                  GridLines="None" AlternatingRowStyle-CssClass="even" HeaderStyle-CssClass="tableHeaderRow"
                   ShowHeader="false"
                 OnRowDataBound="gvMerchandise_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderStyle-Width="60px" HeaderText="Qty">
                            <ItemTemplate>
                               
                                <asp:TextBox ID="tbQuantity" Width="20px" CssClass="inputText"  runat="server" Text="0" />
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="tbQuantity" ValueToCompare="0" Type="Integer" Operator="GreaterThanEqual" ErrorMessage="Please enter quantities of zero or more" Display="None" />
                                
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ProductName" HeaderText="Guest Registration" />
                        
                        <asp:TemplateField HeaderText="Price" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                
                                <asp:Label ID="lblPrice"  runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView></div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlMerchandise" CssClass="section" runat="server" Visible="false" Style="padding-top: 10px">
    <div class="sectionContent">
            <div class="sectionHeaderTitle">
                <h2><asp:Literal ID="lOtherEventItems" runat="server">Other Event-Related Items</asp:Literal></h2>  
             </div>
             <div class="sectionContent">
                 <asp:GridView ID="gvMerchandise" runat="server" AutoGenerateColumns="false" DataKeyNames="ProductID"
                  GridLines="None" AlternatingRowStyle-CssClass="even" HeaderStyle-CssClass="tableHeaderRow"
                   ShowHeader="false"
                 OnRowDataBound="gvMerchandise_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderStyle-Width="60px" HeaderText="Qty">
                            <ItemTemplate>
                               
                                <asp:TextBox ID="tbQuantity" Width="20px" CssClass="inputText"  runat="server" Text="0" />
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="tbQuantity" ValueToCompare="0" Type="Integer" Operator="GreaterThanEqual" ErrorMessage="Please enter quantities of zero or more" Display="None" />
                                
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ProductName" HeaderText="Merchandise" />
                        
                        <asp:TemplateField HeaderText="Price" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                
                                <asp:Label ID="lblPrice" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
             </div>
        </div>
    </asp:Panel>
    <div class="sectionContent">
      <hr width="100%" />
            <div align="center" style="padding-top: 20px">
                <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Continue" runat="server" />
                <asp:Button ID="btnBack" OnClick="btnBack_Click" Text="Back" runat="server" />
                <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server" />
                <div class="clearBothNoSPC">
                </div>
            </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>


