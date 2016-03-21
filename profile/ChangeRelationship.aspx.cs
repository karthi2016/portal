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

public partial class profile_ChangeRelationship : PortalPage
{
    #region Fields

    protected msRelationship targetRelationship;
    protected msRelationshipType targetRelationshipType;
    protected DataView dvRelationshipTypes;
    protected MemberSuiteObject leftSide;
    protected MemberSuiteObject rightSide;

    #endregion

    #region Properties

    protected string OrganizationId
    {
        get { return Request.QueryString["organizationID"]; }
    }

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

        targetRelationship = LoadObjectFromAPI<msRelationship>(ContextID);
        if(targetRelationship == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetRelationshipType = LoadObjectFromAPI<msRelationshipType>(targetRelationship.Type);
        if (targetRelationshipType == null)
        {
            GoToMissingRecordPage();
            return;
        }

        leftSide = APIExtensions.LoadObjectFromAPI(targetRelationship.LeftSide);
        rightSide = APIExtensions.LoadObjectFromAPI(targetRelationship.RightSide);
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

        ddlRelationshipType.DataSource = dvRelationshipTypes;
        ddlRelationshipType.DataBind();

        BindRelationship();

        PageTitleExtension.Text = string.Format("{0} and {1}", leftSide["Name"], rightSide["Name"]);
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sRelationshipTypes = new Search(msRelationshipType.CLASS_NAME);
        sRelationshipTypes.AddOutputColumn("ID");
        sRelationshipTypes.AddOutputColumn("Name");
        sRelationshipTypes.AddOutputColumn("LeftSideType");
        sRelationshipTypes.AddOutputColumn("RightSideType");

        var individualGroup = new SearchOperationGroup
        {
            FieldName = "LeftSideType",
            GroupType = SearchOperationGroupType.Or
        };
        individualGroup.Criteria.Add(Expr.Equals("LeftSideType", msIndividual.CLASS_NAME));
        individualGroup.Criteria.Add(Expr.Equals("RightSideType", msIndividual.CLASS_NAME));

        var organizationGroup = new SearchOperationGroup
        {
            FieldName = "RightSideType",
            GroupType = SearchOperationGroupType.Or
        };
        organizationGroup.Criteria.Add(Expr.Equals("LeftSideType", msOrganization.CLASS_NAME));
        organizationGroup.Criteria.Add(Expr.Equals("RightSideType", msOrganization.CLASS_NAME));

        var statusGroup = new SearchOperationGroup
        {
            GroupType = SearchOperationGroupType.Or
        };
        statusGroup.Criteria.Add(Expr.Equals("ID", targetRelationship.Type));
        statusGroup.Criteria.Add(Expr.Equals("IsActive", true));

        sRelationshipTypes.AddCriteria(individualGroup);
        sRelationshipTypes.AddCriteria(organizationGroup);
        sRelationshipTypes.AddCriteria(statusGroup);

        sRelationshipTypes.AddSortColumn("DisplayOrder");
        sRelationshipTypes.AddSortColumn("Name");

        SearchResult srRelationshipTypes = APIExtensions.GetSearchResult(sRelationshipTypes, 0, null);
        dvRelationshipTypes = new DataView(srRelationshipTypes.Table);
    }

    protected void GoToManageContacts()
    {
        GoTo(string.Format("~/profile/ManageContacts.aspx?contextID={0}", OrganizationId));
    }

    #endregion

    #region Data Binding

    protected void BindRelationship()
    {
        ddlRelationshipType.SelectedValue = targetRelationship.Type;
        dtpStartDate.SelectedDate = targetRelationship.StartDate;
        dtpEndDate.SelectedDate = targetRelationship.EndDate;
        tbDescription.Text = targetRelationship.Description;
    }

    protected void UnbindRelationship()
    {
        // Test to ensure this change does not violate Org Contact Restrictions
        var orgId = leftSide.ClassType == msOrganization.CLASS_NAME
            ? leftSide.SafeGetValue<string>("ID")
            : rightSide.SafeGetValue<string>("ID");

        
        var indId = leftSide.ClassType == msIndividual.CLASS_NAME
            ? leftSide.SafeGetValue<string>("ID")
            : rightSide.SafeGetValue<string>("ID");

        CRMLogic.ErrorOutIfOrganizationContactRestrictionApplies(
            orgId,
            ddlRelationshipType.SelectedValue,
            indId);

        targetRelationship.Type = ddlRelationshipType.SelectedValue;
        targetRelationship.StartDate = dtpStartDate.SelectedDate;
        targetRelationship.EndDate = dtpEndDate.SelectedDate;
        targetRelationship.Description = tbDescription.Text;
    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        UnbindRelationship();

        targetRelationship = SaveObject(targetRelationship);

        GoToManageContacts();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoToManageContacts();
    }

    #endregion
}