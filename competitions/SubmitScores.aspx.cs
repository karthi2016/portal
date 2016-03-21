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

public partial class competitions_SubmitScores : PortalPage
{
    #region Fields

    protected msCompetition targetCompetition;
    protected msCompetitionEntry targetCompetitionEntry;
    protected msJudgingRound targetJudgingRound;
    protected msIndividual targetEntrant;
    protected msScoreCard targetScoreCard;
    protected DataTable dtScoringCriterion;
    
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

    #region Inititalization

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

        if (targetCompetitionEntry == null || targetJudgingRound == null)
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

        //Search for an existing score card for this judge/competition entry/round and if not found create a new one
        Search sScoreCard = new Search { Type = msScoreCard.CLASS_NAME, Context = targetCompetitionEntry.ID };
        sScoreCard.AddOutputColumn("ID");
        sScoreCard.AddCriteria(Expr.Equals("Judge", ConciergeAPI.CurrentEntity.ID));
        sScoreCard.AddCriteria(Expr.Equals("Entry", targetCompetitionEntry.ID));
        sScoreCard.AddCriteria(Expr.Equals("Round", targetJudgingRound.ID));

        SearchResult srScoreCard = APIExtensions.GetSearchResult(sScoreCard, 0, 1);
        targetScoreCard = srScoreCard.Table != null && srScoreCard.Table.Rows.Count > 0
                              ? LoadObjectFromAPI<msScoreCard>(srScoreCard.Table.Rows[0]["ID"].ToString())
                              : new msScoreCard
                                    {
                                        Entry = targetCompetitionEntry.ID,
                                        Judge = ConciergeAPI.CurrentEntity.ID,
                                        Round = targetJudgingRound.ID,
                                        Scores = new List<msScoreCardScore>(),
                                        Name =
                                            string.Format("{0} Scores for {1} {2} Round", targetCompetition.Name,
                                                          targetEntrant.Name, targetJudgingRound.Name)
                                    };
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        PageTitleExtension.Text = targetCompetitionEntry.Name;
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        //This has to happen in the Page_Load not Initialize Page for the unbind to locate the row on the postback
        loadDataFromConcierge();

        //This could have been put in the Page Load IF the loadDataFromConcierge was called before the base.Page_Load
        //but since you cannot count on what might be required in that lifecycle (like checking Public before calling the concierge) just do a postback check here
        if (!IsPostBack)
            bindScoreCard();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sScoringCriterion = new Search(msScoringCriterion.CLASS_NAME);
        sScoringCriterion.AddOutputColumn("Name");
        sScoringCriterion.AddOutputColumn("Description");
        sScoringCriterion.AddOutputColumn("MinimumScore");
        sScoringCriterion.AddOutputColumn("MaximumScore");
        sScoringCriterion.AddOutputColumn("AllowDecimalScores");
        sScoringCriterion.AddCriteria(Expr.Equals("Competition",targetCompetitionEntry.Competition));
        sScoringCriterion.AddSortColumn("DisplayOrder");

        SearchResult srScoringCriterion = APIExtensions.GetSearchResult(sScoringCriterion, 0, null);
        dtScoringCriterion = srScoringCriterion.Table;
        DataColumn scScore = dtScoringCriterion.Columns.Add("Score", typeof (decimal));

        foreach (DataRow drScoringCriterion in dtScoringCriterion.Rows)
        {
            DataRow criterion = drScoringCriterion;
            msScoreCardScore scoreCardScore =
                targetScoreCard.Scores.Where(x => x.Criterion == criterion["ID"].ToString()).SingleOrDefault();

            if (scoreCardScore != null) drScoringCriterion[scScore] = scoreCardScore.Score;
        }
    }

   protected void bindScoreCard()
   {
       gvScoringCriterion.DataSource = dtScoringCriterion;
       gvScoringCriterion.DataBind();

       tbComments.Text = targetScoreCard.Comments;
   }

    protected void unbindScoreCard()
    {
        targetScoreCard.Comments = tbComments.Text;

        foreach (GridViewRow gridViewRow in gvScoringCriterion.Rows)
        {
            DataRow drScoringCriteria = dtScoringCriterion.Rows[gridViewRow.DataItemIndex];
            msScoreCardScore scoreCardScore =
                targetScoreCard.Scores.Where(x => x.Criterion == drScoringCriteria["ID"].ToString()).SingleOrDefault();

            if (scoreCardScore == null)
            {
                scoreCardScore = new msScoreCardScore
                                     {
                                         Criterion = drScoringCriteria["ID"].ToString()
                                     };
                targetScoreCard.Scores.Add(scoreCardScore);
            }

            TextBox tbScore = (TextBox)gridViewRow.Cells[2].FindControl("tbScore");
            scoreCardScore.Score = decimal.Parse(tbScore.Text);
        }
    }

    #endregion

    #region Event Handlers

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        unbindScoreCard();

        SaveObject(targetScoreCard);

        GoTo("~/competitions/JudgeEntries.aspx?contextID=" + JudgingTeamId);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("~/competitions/JudgeEntries.aspx?contextID=" + JudgingTeamId);
    }

    #endregion
}