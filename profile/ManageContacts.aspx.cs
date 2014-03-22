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

        if (targetOrganization == null)
        {
            GoToMissingRecordPage();
            return;
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

        loadDataFromConcierge();

        divPastRelationships.Visible = dvPastRelationships != null && dvPastRelationships.Count > 0;

        gvCurrentRelationships.DataSource = dvCurrentRelationships;
        gvCurrentRelationships.DataBind();

        gvPastRelationships.DataSource = dvPastRelationships;
        gvPastRelationships.DataBind();

        //rptRelationshipTypes.DataSource = dvRelationshipTypes;
        //rptRelationshipTypes.DataBind();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();

        //Relationships
        Search sRelationships = new Search("RelationshipsForARecord")
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
        //sRelationships.AddCriteria(Expr.DoesNotEqual("Target_ID", CurrentUser.Owner));
        sRelationships.AddCriteria(Expr.IsNotBlank("RightSide_Individual.Name"));    // bring individuals over only MS-4426
        sRelationships.AddSortColumn("Target_Name");
        searches.Add(sRelationships);

        ////Relationship Types
        //Search sRelationshipTypes = new Search(msRelationshipType.CLASS_NAME) {ID = msRelationshipType.CLASS_NAME};
        //sRelationshipTypes.AddOutputColumn("ID");
        //sRelationshipTypes.AddOutputColumn("Name");
        //sRelationshipTypes.AddOutputColumn("LeftSideType");
        //sRelationshipTypes.AddOutputColumn("RightSideType");

        //SearchOperationGroup individualGroup = new SearchOperationGroup
        //{
        //    FieldName = "LeftSideType",
        //    GroupType = SearchOperationGroupType.Or
        //};
        //individualGroup.Criteria.Add(Expr.Equals("LeftSideType", msIndividual.CLASS_NAME));
        //individualGroup.Criteria.Add(Expr.Equals("RightSideType", msIndividual.CLASS_NAME));

        //SearchOperationGroup organizationGroup = new SearchOperationGroup
        //{
        //    FieldName = "RightSideType",
        //    GroupType = SearchOperationGroupType.Or
        //};
        //organizationGroup.Criteria.Add(Expr.Equals("LeftSideType", msOrganization.CLASS_NAME));
        //organizationGroup.Criteria.Add(Expr.Equals("RightSideType", msOrganization.CLASS_NAME));

        //sRelationshipTypes.AddCriteria(individualGroup);
        //sRelationshipTypes.AddCriteria(organizationGroup);
        //sRelationshipTypes.AddSortColumn("DisplayOrder");
        //sRelationshipTypes.AddSortColumn("Name");
        //searches.Add(sRelationshipTypes);

        List<SearchResult> searchResults = ExecuteSearches(searches, 0, null);
        
        SearchResult srRelationships = searchResults.Single(x => x.ID == msRelationship.CLASS_NAME);
        dvCurrentRelationships = new DataView(srRelationships.Table, "IsActive = true", "Target_Name", DataViewRowState.CurrentRows);
        dvPastRelationships = new DataView(srRelationships.Table, "IsActive = false", "Target_Name", DataViewRowState.CurrentRows);

        //SearchResult srRelationshipTypes = searchResults.Single(x => x.ID == msRelationshipType.CLASS_NAME);
        //dvRelationshipTypes = new DataView(srRelationshipTypes.Table);
    }

    protected void deleteItem(string id)
    {
        using (IConciergeAPIService serviceProxy = GetServiceAPIProxy())
        {
            serviceProxy.Delete(id);
        }
    }

    #endregion

    #region Event Handlers

    protected void gvRelationships_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deleterelationship":
                deleteItem(e.CommandArgument.ToString());
                QueueBannerMessage("Relationship deleted successfully.");
                Refresh();
                break;
        }
    }

    protected void gvRelationships_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.DataItem == null)
            return;

        DataRowView drv = (DataRowView) e.Row.DataItem;

        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                if (ConciergeAPI.CurrentUser.Owner == drv["Target_ID"].ToString())
                {
                    LinkButton lbDelete = (LinkButton) e.Row.Cells[6].FindControl("lbDelete");
                    if(lbDelete != null)
                        lbDelete.Visible = false;
                }
                break;
        }
    }

    #endregion
}