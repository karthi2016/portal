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
using Telerik.Web.UI;

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

       
    }

    #endregion

    #region Methods

   

    #endregion

    protected void rgOpenRecurringGifts_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        string msql = string.Format(
            "select LocalID, Total, Type, NextTransactionDue, NextTransactionAmount from Gift where Donor='{0}'",
            ConciergeAPI.CurrentEntity.ID);

        using (var api = GetServiceAPIProxy())
        {
            var result = api.ExecuteMSQL(msql, 0, null).ResultValue.SearchResult;

            if (result.Table != null)
                rgOpenRecurringGifts.DataSource = result.Table;


            rgOpenRecurringGifts.VirtualItemCount = result.TotalRowCount;

            lNoOpenPledges.Visible = result.TotalRowCount == 0;
            rgOpenRecurringGifts.Visible = !lNoOpenPledges.Visible;
        }
    }

    protected void rgAllGifts_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        Search s = new Search { Type = msGift.CLASS_NAME };

        s.AddOutputColumn("ID");
        s.AddOutputColumn("Date");
        s.AddOutputColumn("Fund.Name");
        s.AddOutputColumn("Type");
        s.AddOutputColumn("LocalID");
        s.AddOutputColumn("Total");
        s.AddCriteria(Expr.Equals("Donor.ID", ConciergeAPI.CurrentEntity.ID));

        var result = APIExtensions.GetSearchResult(s, 0, null);


        if (result.Table != null)
            rgAllGifts.DataSource = result.Table;


        rgAllGifts.VirtualItemCount = result.TotalRowCount;

        lNoGifts.Visible = result.TotalRowCount == 0;
        rgAllGifts.Visible = !lNoGifts.Visible;

    }
}