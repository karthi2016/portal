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

public partial class chapters_ManageChapterEvents : PortalPage
{
    #region Fields

    protected msChapter targetChapter;
    protected DataView dvEvents;

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetChapter = LoadObjectFromAPI<msChapter>(ContextID);
        if (targetChapter == null)
        {
            GoToMissingRecordPage();
            return;
        }
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        if (targetChapter == null)
            return false;
     
        if (targetChapter.Leaders == null) return false; // no leaders to speak of

        var chapterLeader = targetChapter.Leaders.Find(x => x.Individual == CurrentEntity.ID);

        // is it null? It means the person isn't a leader
        return chapterLeader != null && chapterLeader.CanManageEvents;
    }


    #endregion

    #region Methods

    /// <summary>
    /// Executes a query against the Concierge API for all committees related to the current association
    /// </summary>
    /// <returns></returns>
    private void loadDataFromConcierge()
    {
        Search s = new Search { Type = msEvent.CLASS_NAME };

        s.AddOutputColumn("ID");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("StartDate");
        s.AddOutputColumn("EndDate");
        s.AddCriteria(Expr.Equals("Chapter.ID", targetChapter.ID));
        s.AddSortColumn("StartDate");
        s.AddSortColumn("EndDate");
        s.AddSortColumn("Name");
        var searchResult = ExecuteSearch(s, 0, null);

        dvEvents = new DataView(searchResult.Table);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        loadDataFromConcierge();

        gvEvents.DataSource = dvEvents;
        gvEvents.DataBind();
    }
    #endregion
}