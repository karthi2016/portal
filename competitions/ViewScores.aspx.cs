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

public partial class competitions_ViewScores : PortalPage
{
    #region Fields

    protected msCompetitionEntry targetCompetitionEntry;
    protected msJudgingRound targetJudgingRound;
    protected msIndividual targetEntrant;
    protected DataTable dtTeamMembers;
    protected DataTable dtScoreCards;
    protected DataTable dtScoreCardScores;
    protected DataTable dtScoresSummary;
    protected DataRow drCompetitionEntry;
    protected msCompetition targetCompetition;

    #endregion

    #region Properties

    protected string JudgingRoundId
    {
        get
        {
            return Request.QueryString["judgingRoundId"];
        }
    }

    protected string JudgingTeamId
    {
        get
        {
            return Request.QueryString["judgingTeamId"];
        }
    }

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

        targetCompetitionEntry = LoadObjectFromAPI<msCompetitionEntry>(ContextID);
        targetJudgingRound = LoadObjectFromAPI<msJudgingRound>(JudgingRoundId);
        
        if(targetCompetitionEntry == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetCompetition = LoadObjectFromAPI<msCompetition>(targetCompetitionEntry.Competition);
        if (targetCompetition == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEntrant = LoadObjectFromAPI<msIndividual>(targetCompetitionEntry.Entrant);

        if (targetEntrant == null)
        {
            GoToMissingRecordPage();
            return;
        }
    }

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();

        loadDataFromConcierge();
        setSummaryInformation();
        generateScoresGrid();

        hlReviewEntries.NavigateUrl = "/competitions/JudgeEntries.aspx?contextID=" + JudgingTeamId;
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        //Load either all team members or the current team member based on the portal settings
        Search sTeamMembers = new Search("JudgingTeamMember");
        sTeamMembers.AddOutputColumn("Member");
        sTeamMembers.AddOutputColumn("Member.Name");
        sTeamMembers.AddCriteria(Expr.Equals("Team", JudgingTeamId));
        if (targetCompetition.OtherJudgeScoreVisibilityMode == JudgeScoreVisibilityMode.Never)
            sTeamMembers.AddCriteria(Expr.Equals("Member", ConciergeAPI.CurrentEntity.ID));
        sTeamMembers.AddSortColumn("Member.Name");

        dtTeamMembers = ExecuteSearch(sTeamMembers, 0, null).Table;


        //Set up a multi-search
        List<Search> searches = new List<Search>();

        //Load all score cards along with the total
        Search sScoreCards = new Search { Type = msScoreCard.CLASS_NAME, Context = targetCompetitionEntry.ID };
        sScoreCards.AddOutputColumn("Judge");
        sScoreCards.AddOutputColumn("Judge.Name");
        sScoreCards.AddOutputColumn("TotalScore");
        sScoreCards.AddOutputColumn("Comments");
        sScoreCards.AddCriteria(Expr.Equals("Round",targetJudgingRound.ID));
        sScoreCards.AddCriteria(Expr.Equals("Entry", targetCompetitionEntry.ID));
        sScoreCards.AddCriteria(CreateJudgingTeamCriteria("Judge"));
        searches.Add(sScoreCards);

        //Load all score cards for all team members
        Search sScoreCardScores = new Search(msScoreCardScore.CLASS_NAME);
        sScoreCardScores.AddOutputColumn("Score");
        sScoreCardScores.AddOutputColumn("Criterion.Name");
        sScoreCardScores.AddOutputColumn("ScoreCard.Judge");
        sScoreCardScores.AddOutputColumn("ScoreCard.Judge.Name");
        sScoreCardScores.AddCriteria(Expr.Equals("ScoreCard.Entry", targetCompetitionEntry.ID));
        sScoreCardScores.AddCriteria(Expr.Equals("ScoreCard.Round", JudgingRoundId));
        sScoreCardScores.AddCriteria(CreateJudgingTeamCriteria("ScoreCard.Judge"));
        sScoreCardScores.AddSortColumn("ScoreCard.Judge.Name");
        searches.Add(sScoreCardScores);


        //Load the competition entry summary information
        Search sCompetitionEntry = new Search(msCompetitionEntry.CLASS_NAME);
        sCompetitionEntry.AddOutputColumn("JudgingRoundCombinedScore");
        sCompetitionEntry.AddOutputColumn("JudgingRoundAvgScore");
        sCompetitionEntry.AddOutputColumn("JudgingRoundScoreSpread");
        sCompetitionEntry.AddCriteria(Expr.Equals("ID",targetCompetitionEntry.ID));
        searches.Add(sCompetitionEntry);

        List<SearchResult> searchResults = ExecuteSearches(searches, 0, null);

        //Handle the search results
        dtScoreCards = searchResults[0].Table;
        dtScoreCards.PrimaryKey = new[] {dtScoreCards.Columns["Judge"]};
        dtScoreCardScores = searchResults[1].Table;
        drCompetitionEntry = searchResults[2].Table.Rows[0];
    }

    protected void setSummaryInformation()
    {
        lblCombinedScore.Text = string.Format("{0:0.00}", drCompetitionEntry["JudgingRoundCombinedScore"]);
        lblAverageScore.Text = string.Format("{0:0.00}", drCompetitionEntry["JudgingRoundAvgScore"]);
        lblSpread.Text = dtScoreCards.Rows.Count > 1 ? string.Format("{0:0.00}", Math.Abs((decimal) drCompetitionEntry["JudgingRoundScoreSpread"])) : "n/a";
    }

    protected void generateScoresGrid()
    {
        dtScoresSummary = new DataTable();

        //Add the criterion name column to the summary table which we'll populate in a second and to the gridview
        DataColumn dcCriterion = dtScoresSummary.Columns.Add("Criterion.Name");
        dtScoresSummary.PrimaryKey = new []{dcCriterion};

        BoundField criterionBoundField = new BoundField
        {
            DataField = "Criterion.Name",
            HeaderText = "Criterion"
        };
        criterionBoundField.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
        gvScoringSummary.Columns.Add(criterionBoundField);

        foreach (DataRow drTeamMember in dtTeamMembers.Rows)
        {
            if (targetCompetition.OtherJudgeScoreVisibilityMode == JudgeScoreVisibilityMode.OnceSubmitted && !dtScoreCards.Rows.Contains(drTeamMember["Member"]))
                continue;

            //Add a column to the summary table that we'll populate with scores in a second
            dtScoresSummary.Columns.Add(drTeamMember["Member"].ToString(), typeof (string));

            //Add a column to the gridview for the judge
            BoundField boundField = new BoundField
                                        {
                                            DataField = drTeamMember["Member"].ToString(),
                                            HeaderText = (string) drTeamMember["Member.Name"]
                                        };
            boundField.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            gvScoringSummary.Columns.Add(boundField);
        }

        foreach (DataRow drScoreCardScore in dtScoreCardScores.Rows)
        {
            DataRow newScoreRow = dtScoresSummary.Rows.Find(drScoreCardScore["Criterion.Name"]);
            bool newRow = newScoreRow == null;

            if (newRow)
            {
                newScoreRow = dtScoresSummary.NewRow();
                newScoreRow[dcCriterion] = drScoreCardScore["Criterion.Name"];
            }

            newScoreRow[drScoreCardScore["ScoreCard.Judge"].ToString()] = string.Format("{0:0.00}", drScoreCardScore["Score"]);
            
            if(newRow)
                dtScoresSummary.Rows.Add(newScoreRow);
        }

        //Add the comments row
        DataRow drComments = dtScoresSummary.NewRow();
        drComments[dcCriterion] = "Comments";

        foreach (DataRow drScoreCard in dtScoreCards.Rows)
            drComments[drScoreCard["Judge"].ToString()] = drScoreCard["Comments"];

        dtScoresSummary.Rows.Add(drComments);

        gvScoringSummary.DataSource = dtScoresSummary;
        gvScoringSummary.DataBind();
    }

    protected SearchOperationGroup CreateJudgingTeamCriteria(string fieldName)
    {
        //Set up the criteria for filtering by this judging team
        SearchOperationGroup result = new SearchOperationGroup
        {
            GroupType = SearchOperationGroupType.Or,
            FieldName = fieldName
        };
        foreach (DataRow dataRow in dtTeamMembers.Rows)
            result.Criteria.Add(Expr.Equals(fieldName, dataRow["Member"].ToString()));

        return result;
    }

    #endregion

    #region Event Handlers

    protected void gvScoringSummary_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Footer:
                e.Row.Cells[0].Text = "Total";
                int judgeIndex = 1;
                foreach (DataRow drTeamMember in dtTeamMembers.Rows)
                {
                    if (targetCompetition.OtherJudgeScoreVisibilityMode == JudgeScoreVisibilityMode.OnceSubmitted && !dtScoreCards.Rows.Contains(drTeamMember["Member"]))
                        continue;

                    e.Row.Cells[judgeIndex].Text = dtScoreCards.Rows.Contains(drTeamMember["Member"]) ? string.Format("{0:0.00}",(decimal)dtScoreCards.Rows.Find(drTeamMember["Member"])["TotalScore"]) : "n/a";
                    judgeIndex++;
                }
                break;
        }
    }

    #endregion

}