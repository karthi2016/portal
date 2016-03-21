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
using Telerik.Web.UI;

public partial class profile_ManageContacts : PortalPage
{
    #region Fields

    protected msOrganization targetOrganization;
    protected DataView dvCurrentRelationships;
    protected DataView dvPastRelationships;
    protected DataView dvRelationshipTypes;

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

        targetOrganization = string.IsNullOrWhiteSpace(ContextID) ? ConciergeAPI.CurrentEntity.ConvertTo<msOrganization>() : LoadObjectFromAPI<msOrganization>(ContextID);

        if (targetOrganization == null || targetOrganization.ClassType != msOrganization.CLASS_NAME)
        {
            GoToMissingRecordPage();
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
        
        //loadDataFromConcierge();

        //divPastRelationships.Visible = dvPastRelationships != null && dvPastRelationships.Count > 0;

        //gvCurrentRelationships.DataSource = dvCurrentRelationships;
        //gvCurrentRelationships.DataBind();

        //gvPastRelationships.DataSource = dvPastRelationships;
        //gvPastRelationships.DataBind();

        //rptRelationshipTypes.DataSource = dvRelationshipTypes;
        //rptRelationshipTypes.DataBind();

        PageTitleExtension.Text = targetOrganization.Name;
    }

    #endregion

    //MS-4881
    protected void CurrentContactsNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        int totalCount;

        gvCurrentRelationships.DataSource = LoadDataFromConcierge(true, gvCurrentRelationships.CurrentPageIndex * gvCurrentRelationships.PageSize, gvCurrentRelationships.PageSize, out totalCount);
        gvCurrentRelationships.VirtualItemCount = totalCount;
    }

    //MS-4881
    protected void PastContactsNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        int totalCount;

        gvPastRelationships.DataSource = LoadDataFromConcierge(false, gvPastRelationships.CurrentPageIndex * gvPastRelationships.PageSize, gvPastRelationships.PageSize, out totalCount);
        gvPastRelationships.VirtualItemCount = totalCount;

        if (divPastRelationships != null)
            divPastRelationships.Visible = totalCount > 0;
    }

    #region Methods

    //MS-4881
    protected DataTable LoadDataFromConcierge(bool currentContacts, int start, int count, out int totalCount)
    {
        var searches = new List<Search>();

        // Relationships
        var sRelationships = new Search("RelationshipsForARecord")
        {
            ID = msRelationship.CLASS_NAME,
            Context = targetOrganization.ID
        };
        sRelationships.AddOutputColumn("ID");
        sRelationships.AddOutputColumn("RightSide_Individual.LocalID");
        sRelationships.AddOutputColumn("Target_ID");
        sRelationships.AddOutputColumn("Target_Name");
        sRelationships.AddOutputColumn("IsActive");
        sRelationships.AddOutputColumn("RightSide_Individual.EmailAddress");
        sRelationships.AddOutputColumn("RelationshipTypeName");
        sRelationships.AddCriteria(Expr.IsNotBlank("RightSide_Individual.Name"));    // bring individuals over only MS-4426
        sRelationships.AddCriteria(Expr.Equals("IsActive", currentContacts));
        sRelationships.AddSortColumn("Target_Name");
        searches.Add(sRelationships);

        var searchResults = APIExtensions.GetMultipleSearchResults(searches, start, count);        
        var srRelationships = searchResults.Single(x => x.ID == msRelationship.CLASS_NAME);

        totalCount = srRelationships.TotalRowCount;

        return srRelationships.Table;
    }

    protected void DeleteItem(string id)
    {
        using (IConciergeAPIService serviceProxy = GetServiceAPIProxy())
        {
            serviceProxy.Delete(id);
        }
    }

    #endregion

    #region Event Handlers

    protected void DeleteCurrentContact(object sender, CommandEventArgs e)
    {
        DeleteItem(e.CommandArgument.ToString());
        QueueBannerMessage("Relationship deleted successfully.");
        Refresh();        
    }

    protected void ContactRowBound(object sender, GridItemEventArgs e)
    {
        var dataItem = e.Item as GridDataItem;
        if (dataItem == null)
            return;

        var drv = (DataRowView)dataItem.DataItem;

        if (ConciergeAPI.CurrentUser.Owner != drv["Target_ID"].ToString()) 
            return;

        var lbdelete = (LinkButton) dataItem.FindControl("lbDelete");
        if (lbdelete == null)
            return;

        lbdelete.Visible = false;
    }

    #endregion
}