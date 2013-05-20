using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;

public partial class events_ViewAbstract : PortalPage
{
    #region Fields

    protected msAbstract targetAbstract;
    protected System.Data.DataRow targetAbstractRow;
    protected DataTable targetTracks;

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

        //Have to get the actual MemberSuiteObject for the custom fields
        targetAbstract = LoadObjectFromAPI<msAbstract>(ContextID);

        if(targetAbstract == null)
        {
            GoToMissingRecordPage();
            return;
        }
        
       
        Search s = new Search(msAbstract.CLASS_NAME);
        s.AddCriteria(Expr.Equals("ID", ContextID));
        s.AddOutputColumn("Event.Name");
        s.AddOutputColumn("Status.Name");

        Search s2 = new Search("AbstractSessionTrack");
        s2.AddCriteria(Expr.Equals("Abstract", ContextID));
        s2.AddOutputColumn("SessionTrack.Name");
        s2.AddSortColumn("SessionTrack.Name");

        var results = ExecuteSearches( new List<Search>{ s, s2 }, 0, 1);
        if (results[0].TotalRowCount == 0) GoToMissingRecordPage();

        targetAbstractRow = results[0].Table.Rows[0];

        targetTracks = results[1].Table;
        

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

        setupTracks();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        return Convert.ToString(targetAbstract["Owner"]) == ConciergeAPI.CurrentEntity.ID;
    }

    #endregion

    #region Methods

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        cfsAbstractCustomFields.MemberSuiteObject = targetAbstract;

        var pageLayout = GetAppropriatePageLayout(targetAbstract);
        divAdditionalInfo.Visible =
            !(pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty());

        if (!divAdditionalInfo.Visible)
            return;

        // setup the metadata
        cfsAbstractCustomFields.Metadata = proxy.DescribeObject(msAbstract.CLASS_NAME).ResultValue;
        cfsAbstractCustomFields.PageLayout = pageLayout.Metadata;

        cfsAbstractCustomFields.AddReferenceNamesToTargetObject(proxy);

        cfsAbstractCustomFields.Render();

    }

    private void setupTracks()
    {
        if (targetTracks.Rows.Count == 0)
            return;

        foreach (DataRow dr in targetTracks.Rows)
            lblTracks.Text += dr["SessionTrack.Name"] + ", ";

        lblTracks.Text = lblTracks.Text.Trim().TrimEnd(',');
    }

    #endregion

    #region Event Handlers

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("ViewEvent.aspx?contextID=" + targetAbstract["Event"]);
    }

    protected void btnMyAbstracts_Click(object sender, EventArgs e)
    {
        Response.Redirect("ViewAbstracts.aspx?contextID=" + targetAbstract["Event"]);
    }

    #endregion
}