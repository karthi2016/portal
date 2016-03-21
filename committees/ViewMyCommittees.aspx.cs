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

public partial class committees_ViewMyCommittees : PortalPage
{
    #region Fields

    protected DataView dvCurrentCommitteeMemberships;
    protected DataView dvPastCommitteeMemberships;

    #endregion

    #region Methods

    /// <summary>
    /// Executes a search against the Concierge API for committee memberships related to the current entity
    /// </summary>
    /// <returns></returns>
    private void loadDataFromConcierge()
    {
        Search s = new Search { Type = msCommitteeMembership.CLASS_NAME };

        s.AddOutputColumn("Committee.ID");
        s.AddOutputColumn("Committee.Name");
        s.AddOutputColumn("Term.Name");
        s.AddOutputColumn("Position.Name");
        s.AddOutputColumn("EffectiveStartDate");
        s.AddOutputColumn("EffectiveEndDate");
        s.AddOutputColumn("IsCurrent");
        s.AddCriteria(Expr.Equals("Member.ID",ConciergeAPI.CurrentEntity.ID));
        s.AddCriteria(Expr.Equals("Committee.ShowInPortal", true));
        s.AddSortColumn("Committee.Name");

        var searchResult =  APIExtensions.GetSearchResult(s, 0, null);
        //Create a data view from the search results filtering on just current memberships
        dvCurrentCommitteeMemberships = new DataView(searchResult.Table);
        dvCurrentCommitteeMemberships.RowFilter = "IsCurrent=1";

        //Create a data view from the search results filtering on non-current memberships 
        dvPastCommitteeMemberships = new DataView(searchResult.Table);
        dvPastCommitteeMemberships.RowFilter = "IsCurrent=0";
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

        //Execute a search for the current entities committee memberships
        loadDataFromConcierge();

        //data bind to the current grid
        gvCurrentCommitteeMembership.DataSource = dvCurrentCommitteeMemberships;
        gvCurrentCommitteeMembership.DataBind();

        //data bind to the past grid
        gvPastCommitteeMembership.DataSource = dvPastCommitteeMemberships;
        gvPastCommitteeMembership.DataBind();
    }

    #endregion
}