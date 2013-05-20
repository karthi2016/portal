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

public partial class events_BrowseDonations : PortalPage
{
    #region Fields

    protected DataView dvGifts;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();

        //Execute a search for events in the future and databind
        loadDataFromConcierge();

        gvDonations.DataSource = dvGifts;
        gvDonations.DataBind();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a search against the Concierge API for events with a future start date that are set to be visible in the portal.
    /// </summary>
    /// <returns></returns>
    private void loadDataFromConcierge()
    {
        Search s = new Search { Type = msGift.CLASS_NAME };

        s.AddOutputColumn("ID");
        s.AddOutputColumn("Date");
        s.AddOutputColumn("Fund.Name");
        s.AddOutputColumn("Type");
        s.AddOutputColumn("Amount");
        s.AddCriteria(Expr.Equals("Donor.ID", ConciergeAPI.CurrentEntity.ID));

        var searchResult = ExecuteSearch(s, 0, null);
        dvGifts = new DataView(searchResult.Table);
    }

    #endregion
}