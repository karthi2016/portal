using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Results;

public partial class events_ViewCompetition : PortalPage
{
    #region Fields

    protected msCompetition targetCompetition;
    protected CompetitionEntryInformation targetEntryInfo;
    protected string judgingTeamID;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
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

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadDataFromConcierge(proxy);
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

        liEnterCompetition.Visible = targetEntryInfo != null && targetEntryInfo.CanEnter;
        liJudgeEntries.Visible = !string.IsNullOrWhiteSpace(judgingTeamID);

        if (targetEntryInfo != null)
        {
            if(targetEntryInfo.NumberOfDraftEntries > 0)
            {
                lblDraftEntries.Text =
                    string.Format(
                        "You currently have {0} entries in draft status. If you do not submit these by {1:d} at {2:t}, these entries will be discarded.",
                        targetEntryInfo.NumberOfDraftEntries, targetCompetition.CloseDate,
                        targetCompetition.CloseDate);
                lblDraftEntries.Visible = hlViewMyCompetitionEntries.Visible = true;
            }

            if(targetEntryInfo.NumberOfNonDraftEntries > 0)
            {
                lblExistingEntries.Text = string.Format("You have already submitted {0} entries for this competition.",
                                                        targetEntryInfo.NumberOfNonDraftEntries);
                lblExistingEntries.Visible = true;
            }
        }
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge(IConciergeAPIService serviceProxy )
    {
        targetCompetition = serviceProxy.LoadObjectFromAPI<msCompetition>(ContextID);

        if (targetCompetition == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEntryInfo = targetCompetition.GetCompetitionEntryInformation();

        // now check judging
        if (ConciergeAPI.CurrentEntity == null)
            return;

        Search s = new Search("JudgingTeamMember");
        s.AddOutputColumn("Team.ID");
        s.AddCriteria(Expr.Equals("Team.Competition.ID", targetCompetition.ID));
        s.AddCriteria(Expr.Equals("Member.ID", ConciergeAPI.CurrentEntity.ID));
        SearchResult sr = APIExtensions.GetSearchResult(s, 0, 1);

        if (sr.TotalRowCount > 0 && sr.Table != null && sr.Table.Rows.Count > 0)
            judgingTeamID = sr.Table.Rows[0]["Team.ID"].ToString();
    }

    #endregion
}