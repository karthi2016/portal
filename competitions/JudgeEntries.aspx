<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JudgeEntries.aspx.cs" MasterPageFile="~/App_Master/GeneralPage.master"
    Inherits="competitions_JudgeEntries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    Judge
    <%=targetCompetition.Name %>
    Entries
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
<asp:Literal ID="PageText" runat="server" />
    <asp:UpdatePanel ID="upCommitteeMembership" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Label runat="server" ID="lblJudgingBucketWarning" CssClass="redHighlight" Visible="false" />
            <p>
                <asp:Label runat="server" ID="lblInstructions" CssClass="columnHeader" />
            </p>
            <asp:Label runat="server" ID="lblSelectedRound" Text="Selected Round" />
            <asp:DropDownList runat="server" ID="ddlJudgingRounds" AutoPostBack="true" OnSelectedIndexChanged="ddlJudgingRounds_OnSelectedIndexChanged"
                DataValueField="ID" DataTextField="Name" />
            <asp:Label runat="server" ID="lblJudgingRoundWarning" CssClass="redHighlight" Visible="false" />
            <asp:Repeater ID="rptJudgingBuckets" runat="server" OnItemDataBound="rptJudgingBuckets_ItemDataBound">
                <ItemTemplate>
                    <div class="section" style="margin-top: 10px">
                        <div class="sectionHeaderTitle">
                            <h2><asp:Literal ID="litBucketName" runat="server" Text='<%# Bind("Name") %>' /></h2>
                        </div>
                        <div class="sectionContent">
                            <asp:GridView ID="gvCompetitionEntries" runat="server" GridLines="None" AutoGenerateColumns="false"
                                EmptyDataText="No competition entries were found in this bucket." OnRowDataBound="gvCompetitionEntries_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="LocalID" HeaderStyle-HorizontalAlign="Left" HeaderText="Entry #" />
                                    <asp:BoundField DataField="Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Name"
                                        NullDisplayText="n/a" />
                                    <asp:BoundField DataField="Entrant.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Owner" />
                                    <asp:BoundField DataField="Individual.PrimaryOrganization.Name" HeaderStyle-HorizontalAlign="Left" HeaderText="Entrant Organization" />
                                    <asp:BoundField DataField="Individual.PrimaryOrganization._Preferred_Address_City" HeaderStyle-HorizontalAlign="Left" HeaderText="Organization City" />
                                    <asp:BoundField DataField="Individual.PrimaryOrganization._Preferred_Address_State" HeaderStyle-HorizontalAlign="Left" HeaderText="Organization State" />
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Score">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblScore" ForeColor="Green" Text='<%# DataBinder.Eval(Container.DataItem, "Score") %>'
                                                Visible='<%# DataBinder.Eval(Container.DataItem, "HasScore") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="hlSubmitScores" Text="(submit scores)" NavigateUrl='<%# string.Format(@"~\competitions\SubmitScores.aspx?contextID={0}&judgingRoundId={1}&judgingTeamId={2}", DataBinder.Eval(Container.DataItem, "ID"), ddlJudgingRounds.SelectedValue, targetJudgingTeam.ID) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="hlViewScores" Text="(view scores)" NavigateUrl='<%# string.Format(@"~\competitions\ViewScores.aspx?contextID={0}&judgingRoundId={1}&judgingTeamId={2}", DataBinder.Eval(Container.DataItem, "ID"), ddlJudgingRounds.SelectedValue, targetJudgingTeam.ID) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:HyperLinkField DataNavigateUrlFormatString="~/competitions/ViewCompetitionEntry.aspx?contextID={0}&teamID={1}" DataNavigateUrlFields="ID,TeamID" Text="(view entry)" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div class="section" style="margin-top: 10px">
                <div class="sectionHeaderTitle">
                    <h2>
                       <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
                </div>
                <ul>
                    <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
                    </li>
                </ul>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlJudgingRounds" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
