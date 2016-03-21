<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SubmitScores.aspx.cs" Inherits="competitions_SubmitScores"
    MasterPageFile="~/App_Master/GeneralPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Submit <asp:Literal runat="server" ID="PageTitleExtension"></asp:Literal>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <asp:Literal ID="PageText" runat="server" />
    <%= targetCompetition.JudgingInstructions%>
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <%= targetCompetition.Name %>
                Judging Criterion</h2>
        </div>
        <div class="sectionContent">
            <asp:GridView ID="gvScoringCriterion" runat="server" GridLines="None" AutoGenerateColumns="false"
                EmptyDataText="No judging criteria have been set up. Unable to score this entry.">
                <Columns>
                    <asp:BoundField DataField="Name" ItemStyle-CssClass="columnHeader" HeaderStyle-HorizontalAlign="Left"
                        HeaderText="Name" />
                    <asp:BoundField DataField="Description" HeaderStyle-HorizontalAlign="Left" HeaderText="Description" />
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Score">
                        <ItemTemplate>
                            <asp:TextBox ID="tbScore" runat="server" Width="60" Text='<%# (bool)Eval("AllowDecimalScores") ? Eval("Score","{0:0.00}") : Eval("Score","{0:0}") %>' />
                            <asp:RequiredFieldValidator runat="server" ID="rfvScore" ControlToValidate="tbScore"
                                Display="None" ErrorMessage='<%# string.Format("Please enter a score for {0}.",Eval("Name")) %>' />
                            <asp:RegularExpressionValidator runat="server" ID="revInteger" ControlToValidate="tbScore"
                                Enabled='<%# !(bool)Eval("AllowDecimalScores") %>' ValidationExpression="^-?\d+"
                                Display="None" ErrorMessage='<%# string.Format("{0} must be a number - decimals are not allowed.", Eval("Name"))%>' />
                            <asp:RegularExpressionValidator runat="server" ID="revDecimal" ControlToValidate="tbScore"
                                Enabled='<%# Bind("AllowDecimalScores") %>' ValidationExpression="^-?\d*(.\d{0,2})?"
                                Display="None" ErrorMessage='<%# string.Format("{0} must be a number - up to two decimal places are allowed.", Eval("Name"))%>' />
                            <asp:CompareValidator runat="server" ID="cvMinimumScore" ControlToValidate="tbScore"
                                Type="Double" ValueToCompare='<%# Eval("MinimumScore") %>' Operator="GreaterThanEqual"
                                Display="None" ErrorMessage='<%# string.Format("{0} must be greater than or equal to {1}", Eval("Name"), Eval("MinimumScore")) %>' />
                            <asp:CompareValidator Display="None" runat="server" ID="cvMaximumScore" ControlToValidate="tbScore"
                                Type="Double" ValueToCompare='<%# Eval("MaximumScore") %>' Operator="LessThanEqual"
                                ErrorMessage='<%# string.Format("{0} must be less than or equal to {1}", Eval("Name"), Eval("MaximumScore")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblCriterionRestrictions" Text='<%# (bool)Eval("AllowDecimalScores") ? string.Format("({0} - {1})", Eval("MinimumScore","{0:0.00}"), Eval("MaximumScore","{0:0.00}")) : string.Format("({0} - {1})", Eval("MinimumScore","{0:0}"), Eval("MaximumScore","{0:0}")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <p>
            </p>
            <asp:Literal ID="lComments" runat="server">
                        Comments
            </asp:Literal>
            <br />
            <asp:TextBox runat="server" ID="tbComments" TextMode="MultiLine" Columns="100" Rows="5" />
        </div>
        <div class="sectionContent">
            <div align="center" style="padding-top: 20px">
                <asp:Button ID="btnContinue" OnClick="btnContinue_Click" Text="Submit Scores" runat="server" />
                <asp:Button ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" runat="server"
                    CausesValidation="false" />
                <div class="clearBothNoSPC">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
