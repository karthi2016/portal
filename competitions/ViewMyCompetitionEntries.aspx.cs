using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations; 
using MemberSuite.SDK.Types;

public partial class competitions_ViewMyCompetitionEntries : PortalPage
{
    #region Fields

    protected DataView dvCompetitionEntries;

    #endregion

    #region Initialization
    
    protected override void  Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        // This has to happen on Page_Load not InitializePage for the row commands to work properly
        loadDataFromConcierge();

        gvCompetitionEntries.DataSource = dvCompetitionEntries;
        gvCompetitionEntries.DataBind();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        var sCompetitionEntries = new Search { Type = msCompetitionEntry.CLASS_NAME };
        sCompetitionEntries.AddOutputColumn("ID");
        sCompetitionEntries.AddOutputColumn("Competition");
        sCompetitionEntries.AddOutputColumn("Competition.Name");
        sCompetitionEntries.AddOutputColumn("Name");
        sCompetitionEntries.AddOutputColumn("DateSubmitted");
        sCompetitionEntries.AddOutputColumn("Status");
        sCompetitionEntries.AddOutputColumn("Status.Name");
        sCompetitionEntries.AddCriteria(Expr.Equals("Entrant",ConciergeAPI.CurrentEntity.ID));
        sCompetitionEntries.AddSortColumn("Competition.Name");
        sCompetitionEntries.AddSortColumn("Competition.CloseDate");

        var srCompetitionEntries = APIExtensions.GetSearchResult(sCompetitionEntries, 0, null);
        dvCompetitionEntries = new DataView(srCompetitionEntries.Table);
    }

    #endregion

    #region Event Handlers

    protected void gvCompetitionEntries_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                var drv = (DataRowView)e.Row.DataItem;
                var competitionEntryInfo = CompetitionLogic.GetCompetitionEntryInformation(
                    drv["Competition"].ToString(),
                    CurrentEntity.ID);
                var lbComplete = (LinkButton)e.Row.FindControl("lbComplete");

                if (competitionEntryInfo == null) 
                {
                    lbComplete.Visible = false;
                    break;
                }

                if (drv["Status"] != DBNull.Value && drv["Status"].ToString() == competitionEntryInfo.DraftStatusId)
                {
                    gvCompetitionEntries.Columns[4].Visible = true;
                    lbComplete.Visible = true;
                    lbComplete.CommandName = "completedraft";
                }

                if (drv["Status"] != DBNull.Value && drv["Status"].ToString() == competitionEntryInfo.PendingPaymentStatusId)
                {
                    gvCompetitionEntries.Columns[4].Visible = true;
                    lbComplete.Visible = true;
                    lbComplete.CommandName = "completepayment";
                }

                break;
        }
    }

    protected void gvCompetitionEntries_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "completedraft":
                GoTo(string.Format("~/competitions/Enter_EntryForm.aspx?contextID={0}", e.CommandArgument));
                break;

            case "completepayment":
                var entry = LoadObjectFromAPI<msCompetitionEntry>((string) e.CommandArgument);
                var competitionEntryInfo = CompetitionLogic.GetCompetitionEntryInformation(
                    entry.Competition);

                if(entry.EntryFee == null)
                {
                    entry.Status = competitionEntryInfo.SubmittedStatusId;
                    using(IConciergeAPIService proxy = GetConciegeAPIProxy())
                    {
                        proxy.Save(entry);
                    }

                    QueueBannerMessage(string.Format("Competition entry {0} successfully processed.", entry.LocalID));
                    GoHome();
                }

                var newOrder = new msOrder();
                newOrder.BillTo = newOrder.ShipTo = ConciergeAPI.CurrentEntity.ID;
                newOrder.LineItems = new List<msOrderLineItem>
                {
                    new msOrderLineItem
                    {
                        Quantity = 1,
                        Product = entry.EntryFee,
                        Options =
                            new List<NameValueStringPair>() {new NameValueStringPair("CompetitionEntryId", entry.ID)}
                    }
                };

                //Add the order as the "shopping cart" for the order processing wizard
                MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(newOrder);
                break;
        }
    }

    #endregion
}