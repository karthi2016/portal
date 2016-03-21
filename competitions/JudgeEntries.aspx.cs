using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class competitions_JudgeEntries : PortalPage
{
    #region Fields

    protected msJudgingTeam targetJudgingTeam;
    protected msCompetition targetCompetition;
    protected DataTable dtCompetitionEntries;
    protected DataView dvJudgingBuckets;
    protected DataTable dtJudgingRounds;
    protected DataTable dtScoreCards;
    protected msJudgingRound selectedRound;
    protected DataView dvCompetitionEntriesInRound;
    protected msJudgingRound targetRound;
    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the target object for the page
    /// </summary>
    /// <remarks>Many pages have "target" objects that the page operates on. For instance, when viewing
    /// an event, the target object is an event. When looking up a directory, that's the target
    /// object. This method is intended to be overriden to initialize the target object for
    /// each page that needs it.</remarks>
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetJudgingTeam = LoadObjectFromAPI<msJudgingTeam>(ContextID);

        if (targetJudgingTeam == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetCompetition = LoadObjectFromAPI<msCompetition>(targetJudgingTeam.Competition);

        if (Request.QueryString["roundID"] != null)
            targetRound = LoadObjectFromAPI<msJudgingRound>(Request.QueryString["roundID"]);

        if (targetCompetition == null)
        {
            GoToMissingRecordPage();
            return;
        }
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        PageTitleExtension.Text = string.Format(" {0} Entries", targetCompetition.Name);
    }

    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        if (targetJudgingTeam.Buckets == null || targetJudgingTeam.Buckets.Count == 0)
        {
            lblJudgingBucketWarning.Text = "No Judging Buckets have been assigned to your Judging Team.";
            lblJudgingBucketWarning.Visible = true;

            ddlJudgingRounds.Visible = lblSelectedRound.Visible = false;

            return;
        }

        //These steps have to happen in Page_Load not InitializePage to make Ajax work properly
        loadDataFromConcierge();

        //This has to happen after the data is loaded so cannot be moved to InitializePage
        if (!this.IsPostBack)
        {
           
            ddlJudgingRounds.DataSource = dtJudgingRounds;
            ddlJudgingRounds.DataBind();

        

            // MS - 6295
            var cacheKey = string.Format("SelectedRound_{0}", targetCompetition.ID);
            var defaultRound = SessionManager.Get<string>(cacheKey);
            if (!string.IsNullOrEmpty(defaultRound) && ddlJudgingRounds.Items.FindByValue(defaultRound) != null)
            {
                ddlJudgingRounds.SelectedValue = defaultRound;
            }
            else
            {
                // Do current selection code 
                //Select the current judging round by default if possible
                foreach (DataRow judgingRound in dtJudgingRounds.Rows)
                    if ((judgingRound["JudgingBegins"] == DBNull.Value || (DateTime.Now > (DateTime)judgingRound["JudgingBegins"])) && (judgingRound["JudgingBegins"] == DBNull.Value || (DateTime.Now < (DateTime)judgingRound["JudgingEnds"])))
                    {
                        ddlJudgingRounds.SelectedValue = judgingRound["ID"].ToString();
                        break;
                    }


                if (targetRound != null)
                    ddlJudgingRounds.SelectedValue = targetRound.ID;
            } 

           

            setJudgingRoundWarning(); // do this first - MS-4424

            bindJudgingBuckets();
        }

        dvCompetitionEntriesInRound = new DataView(dtCompetitionEntries) { RowFilter = string.Format("JudgingRound.ID = '{0}'", ddlJudgingRounds.SelectedValue) };
        lblInstructions.Text = dtCompetitionEntries.Rows.Count > 0
                                   ? string.Format(
                                       "{0}, you have {1} entries that you need to score for the selected round.",
                                       ConciergeAPI.CurrentEntity.Name, dvCompetitionEntriesInRound.Count)
                                   : "There are no entries assigned to you for the selected round.";
  
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        var searches = new List<Search>();

        // Get all judging rounds related to the competition
        var sJudgingRounds = new Search(msJudgingRound.CLASS_NAME){ID= msJudgingRound.CLASS_NAME};
        sJudgingRounds.AddOutputColumn("ID");
        sJudgingRounds.AddOutputColumn("Name");
        sJudgingRounds.AddOutputColumn("JudgingBegins");
        sJudgingRounds.AddOutputColumn("JudgingEnds");
        sJudgingRounds.AddCriteria(Expr.Equals("Competition.ID", targetCompetition.ID));
        sJudgingRounds.AddSortColumn("RoundNumber");
        searches.Add(sJudgingRounds);

        // Get the judging buckets related to this team and sort by name
        var sJudgingBuckets = new Search(msJudgingBucket.CLASS_NAME) { ID = msJudgingBucket.CLASS_NAME };
        sJudgingBuckets.AddOutputColumn("Name");
        var sJudgingBucketIdClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
        foreach (var bucketId in targetJudgingTeam.Buckets)
            sJudgingBucketIdClause.Criteria.Add(Expr.Equals("ID", bucketId));
        sJudgingBuckets.AddCriteria(sJudgingBucketIdClause);
        sJudgingBuckets.AddSortColumn("Name");
        searches.Add(sJudgingBuckets);

        // Get the all the competition enties related to any of the buckets related to this team (we will filter based on bucket / round with data views later)
        var sCompetitionEntries = new Search(msCompetitionEntry.CLASS_NAME) { ID = msCompetitionEntry.CLASS_NAME };
        sCompetitionEntries.AddOutputColumn("ID");
        sCompetitionEntries.AddOutputColumn("Name");
        sCompetitionEntries.AddOutputColumn("JudgingBucket.ID");
        sCompetitionEntries.AddOutputColumn("JudgingRound.ID");
        sCompetitionEntries.AddOutputColumn("LocalID");
        sCompetitionEntries.AddOutputColumn("Entrant.Name");
        sCompetitionEntries.AddOutputColumn("Individual.PrimaryOrganization.Name");
        sCompetitionEntries.AddOutputColumn("Individual.PrimaryOrganization._Preferred_Address_City");
        sCompetitionEntries.AddOutputColumn("Individual.PrimaryOrganization._Preferred_Address_State");
        sJudgingBucketIdClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
        foreach (var bucketId in targetJudgingTeam.Buckets)
            sJudgingBucketIdClause.Criteria.Add(Expr.Equals("JudgingBucket.ID", bucketId));
        sCompetitionEntries.AddCriteria(sJudgingBucketIdClause);
        searches.Add(sCompetitionEntries);

        // Get the score cards for the current user related to competition entries related to any of the buckets related to this team (we will filter based on bucket / round with data views later)
        var sScoreCards = new Search(msScoreCard.CLASS_NAME){ID = msScoreCard.CLASS_NAME};
        sScoreCards.AddOutputColumn("ID");
        sScoreCards.AddOutputColumn("Round.ID");
        sScoreCards.AddOutputColumn("Entry.ID");
        sScoreCards.AddOutputColumn("TotalScore");
        sScoreCards.AddCriteria(Expr.Equals("Judge.ID", ConciergeAPI.CurrentEntity.ID));
        sJudgingBucketIdClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
        foreach (var bucketId in targetJudgingTeam.Buckets)
            sJudgingBucketIdClause.Criteria.Add(Expr.Equals("Entry.JudgingBucket.ID", bucketId));
        sScoreCards.AddCriteria(sJudgingBucketIdClause);
        searches.Add(sScoreCards);

        // Execute the searches as a multi-search
        var searchResults = APIExtensions.GetMultipleSearchResults(searches, 0, null);

        // Handle the judging round results
        dtJudgingRounds = searchResults.Single(x => x.ID == msJudgingRound.CLASS_NAME).Table;
        dtJudgingRounds.PrimaryKey = new[] { dtJudgingRounds.Columns["ID"] };

        // Handle the bucket results
        dvJudgingBuckets = new DataView(searchResults.Single(x => x.ID == msJudgingBucket.CLASS_NAME).Table);

        // Handle the competition entry results
        dtCompetitionEntries = searchResults.Single(x => x.ID == msCompetitionEntry.CLASS_NAME).Table;

        dtCompetitionEntries.Columns.Add("TeamID");
        foreach (DataRow dr in dtCompetitionEntries.Rows)
            dr["TeamID"] = targetJudgingTeam.ID;    // set this up

        DataColumn dcScore = dtCompetitionEntries.Columns.Add("Score", typeof(decimal));
        DataColumn dcHasScore = dtCompetitionEntries.Columns.Add("HasScore", typeof(bool));

        dtScoreCards = searchResults.Single(x => x.ID == msScoreCard.CLASS_NAME).Table;
        dtScoreCards.PrimaryKey = new[] { dtScoreCards.Columns["Entry.ID"], dtScoreCards.Columns["Round.ID"] };

        foreach (DataRow dataRow in dtCompetitionEntries.Rows)
        {
            DataRow drScoreCard = dtScoreCards.Rows.Find(new [] {dataRow["ID"], dataRow["JudgingRound.ID"]});
            bool hasScore = drScoreCard != null && drScoreCard["TotalScore"] != DBNull.Value;
            dataRow[dcHasScore] = hasScore;
            dataRow[dcScore] = hasScore ? drScoreCard["TotalScore"] : 0;
        }
    }

    protected void setJudgingRoundWarning()
    {
        lblJudgingRoundWarning.Visible = false;
        if (dtJudgingRounds.Rows.Count == 0)
        {
            ddlJudgingRounds.Enabled = false;
            lblJudgingRoundWarning.Text = "No judging rounds have been set up for this competition.";
            lblJudgingRoundWarning.Visible = true;
            return;
        }

        DataRow selectedJudgingRound = dtJudgingRounds.Rows.Find(ddlJudgingRounds.SelectedValue);

        //Check begin date first so the closed statement takes precedence
        if (selectedJudgingRound["JudgingBegins"] != DBNull.Value && (DateTime)selectedJudgingRound["JudgingBegins"] > DateTime.Now)
        {
            lblJudgingRoundWarning.Text = "Judging for the round has not yet begun.";
            lblJudgingRoundWarning.Visible = true;
        }

        if (selectedJudgingRound["JudgingEnds"] != DBNull.Value && (DateTime)selectedJudgingRound["JudgingEnds"] < DateTime.Now)
        {
            lblJudgingRoundWarning.Text = "Judging for this round is closed.";
            lblJudgingRoundWarning.Visible = true;
        }
    }

    protected void bindJudgingBuckets()
    {
        rptJudgingBuckets.DataSource = dvJudgingBuckets;
        rptJudgingBuckets.DataBind();
    }

    #endregion

    #region Event Handlers

    protected void ddlJudgingRounds_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        var cacheKey = string.Format("SelectedRound_{0}", targetCompetition.ID);
        SessionManager.Set(cacheKey, ddlJudgingRounds.SelectedValue);
        GoTo(string.Format("JudgeEntries.aspx?contextID={0}&roundID={1}",
                           targetJudgingTeam.ID, ddlJudgingRounds.SelectedValue));
    }

    protected void rptJudgingBuckets_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView drBucket = (DataRowView)e.Item.DataItem;

        //Create a new Dataview to filter the competitionentires to just this bucket
        
        DataView dvEntriesInBucket = new DataView(dtCompetitionEntries)
                                         {
                                             RowFilter =
                                                 string.Format("JudgingBucket.ID = '{0}' AND JudgingRound.ID = '{1}'", drBucket["ID"].ToString(), ddlJudgingRounds.SelectedValue)
                                         };

        //Now bind the gridview in the repeater
        GridView gv = (GridView)e.Item.FindControl("gvCompetitionEntries");

        //If the current round is not open hide the submit score column (we can use the visibility status determined earlier for the warning. If there's a warning this command shouldn't be visible)
        gv.Columns[4].Visible = !lblJudgingRoundWarning.Visible;

        gv.DataSource = dvEntriesInBucket;
        gv.DataBind();
    }

    protected void gvCompetitionEntries_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Row.DataItem;

        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:

                //Set submit score link visibility
                HyperLink hlSubmitScores = (HyperLink) e.Row.FindControl("hlSubmitScores");
                if ((bool)drv["HasScore"])
                    hlSubmitScores.Text = "(edit scores)";

                //Set view score link visibility
                //If the judging options say that a user can view another judges scores, then ALWAYS show this link. Otherwise, show this link ONLY if this judge has already submitted a score.
                HyperLink hlViewScores = (HyperLink) e.Row.FindControl("hlViewScores");
                hlViewScores.Visible = targetCompetition.OtherJudgeScoreVisibilityMode !=
                                       JudgeScoreVisibilityMode.Never || (bool) drv["HasScore"];

                // get that last control
                HyperLink hlViewEntry = (HyperLink)e.Row.Cells[e.Row.Cells.Count - 1].Controls[0] as HyperLink;

                if (lblJudgingRoundWarning.Visible)
                {
                    hlSubmitScores.Visible = false;
                    hlViewScores.Visible = false;
                    hlViewEntry.Visible = false;
                    
                }

                break;
        }
    }

    #endregion
}