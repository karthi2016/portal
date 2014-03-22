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

public partial class careercenter_ViewMyJobPostings : PortalPage
{
    #region Fields

    protected DataView dvJobPostings;

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

        loadDataFromConcierge();
        gvJobPostings.DataSource = dvJobPostings;
        gvJobPostings.DataBind();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sJobPostings = new Search {Type = msJobPosting.CLASS_NAME};
        sJobPostings.AddOutputColumn("ID");
        sJobPostings.AddOutputColumn("LocalID");
        sJobPostings.AddOutputColumn("Name");
        sJobPostings.AddOutputColumn("PostOn");
        sJobPostings.AddOutputColumn("ExpirationDate");
        sJobPostings.AddCriteria(Expr.Equals("Owner",ConciergeAPI.CurrentEntity.ID));

        // MS-4857 - show newest first
        sJobPostings.AddSortColumn("PostOn", true );


        SearchResult srJobPostings = ExecuteSearch(sJobPostings, 0, null);
        dvJobPostings = new DataView(srJobPostings.Table);
    }

    #endregion

}