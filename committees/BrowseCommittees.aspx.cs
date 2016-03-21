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

public partial class committees_BrowseCommittees : PortalPage
{
    #region Fields

    protected DataView dvCommittees;

    #endregion

    #region Intialization

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

        gvCommittees.DataSource = dvCommittees;
        gvCommittees.DataBind();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a query against the Concierge API for all committees related to the current association
    /// </summary>
    /// <returns></returns>
    private void loadDataFromConcierge()
    {
        Search s = new Search { Type = msCommittee.CLASS_NAME };

        s.AddOutputColumn("ID");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("CurrentMemberCount");
        s.AddCriteria(Expr.Equals("ShowInPortal",true));
        s.AddSortColumn("Name");


        var searchResult =  APIExtensions.GetSearchResult(s, 0, null);

        dvCommittees = new DataView(searchResult.Table);
    }

    #endregion

    #region Event Handlers

    #endregion
}