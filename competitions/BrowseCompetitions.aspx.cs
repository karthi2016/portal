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

public partial class committees_BrowseCompetitions : PortalPage
{
    #region Fields

    protected DataView dvCompetitions;
    protected DataTable dtCompetitions;

    #endregion

    #region Intitialization

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
        createDataView();

        gvCompetitions.DataSource = dvCompetitions;
        gvCompetitions.DataBind();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a query against the Concierge API for all open competitions related to the current association
    /// </summary>
    /// <returns></returns>
    private void loadDataFromConcierge()
    {
        Search s = new Search { Type = msCompetition.CLASS_NAME };

        s.AddOutputColumn("ID");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("OpenDate");
        s.AddOutputColumn("CloseDate");
        
        //s.AddCriteria(Expr.IsLessThan("OpenDate",DateTime.UtcNow));
        //s.AddCriteria(Expr.IsGreaterThan("CloseDate", DateTime.UtcNow));
        s.AddCriteria(Expr.DoesNotEqual("IsClosed",1));

        SearchOperationGroup soPostDate = new SearchOperationGroup();
        soPostDate.Criteria.Add(Expr.IsBlank("PostToWeb"));
        soPostDate.Criteria.Add(Expr.IsLessThan("PostToWeb",DateTime.UtcNow));
        soPostDate.GroupType = SearchOperationGroupType.Or;

        SearchOperationGroup soRemoveDate = new SearchOperationGroup();
        soRemoveDate.Criteria.Add(Expr.IsBlank("RemoveFromWeb"));
        soRemoveDate.Criteria.Add(Expr.IsGreaterThan("RemoveFromWeb", DateTime.UtcNow));
        soRemoveDate.GroupType = SearchOperationGroupType.Or;

        s.AddCriteria(soPostDate);
        s.AddCriteria(soRemoveDate);
        
        s.AddSortColumn("CloseDate");

        var searchResult =  APIExtensions.GetSearchResult(s, 0, null);
        dtCompetitions = searchResult.Table;
    }

    private void createDataView()
    {
        DataColumn dcTimeRemaining = new DataColumn("TimeRemaining",typeof(string));
        dtCompetitions.Columns.Add(dcTimeRemaining);

        // we need to know what "now" is in the current timezone

        DateTime dtTimeInCurrentUsersTimeZone = DateTime.UtcNow + ConciergeAPI.CurrentTimeZone.GetUtcOffset( DateTime.Now);
        
        foreach (DataRow row in dtCompetitions.Rows)
        {
            var dtCloseDate = (DateTime) row["CloseDate"];
            
            TimeSpan tsTimeRemaining = dtCloseDate - dtTimeInCurrentUsersTimeZone;

            if (tsTimeRemaining.TotalMilliseconds > 0)
                row[dcTimeRemaining] = string.Format("{0} days, {1} hours, {2} minutes", tsTimeRemaining.Days,
                                                     tsTimeRemaining.Hours, tsTimeRemaining.Minutes);
            else
                row[dcTimeRemaining] = "CLOSED";
        }

        dvCompetitions = new DataView(dtCompetitions);
    }

    #endregion

    #region Event Handlers



    #endregion
}