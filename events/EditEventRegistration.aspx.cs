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

public partial class events_EditEventRegistration : PortalPage
{
    #region Fields

    protected msEventRegistration targetEventRegistration;
    protected msEntity targetOwner;
    protected msEvent targetEvent;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected DataView dvRegistrationFees;
    protected DataView dvRegistrationCategories;
    protected DataView dvRegistrationClasses;

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

        targetEventRegistration = LoadObjectFromAPI<msEventRegistration>(ContextID);

        if(targetEventRegistration == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEvent = LoadObjectFromAPI<msEvent>(targetEventRegistration.Event);
        targetOwner = LoadObjectFromAPI<msEntity>(targetEventRegistration.Owner);

        if (targetOwner == null || targetEvent == null)
        {
            GoToMissingRecordPage();
            return;
        }

        loadEventOwners();
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

        ddlFee.DataSource = dvRegistrationFees;
        ddlFee.DataBind();

        ddlCategory.DataSource = dvRegistrationCategories;
        ddlCategory.DataBind();

        ddlClass.DataSource = dvRegistrationClasses;
        ddlClass.DataBind();

        bindEventRegistration();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (targetChapter != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetChapter.Leaders);

        if (targetSection != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetOrganizationalLayer.Leaders);

        //Default to false for now because currently only leaders can create events in the portal
        return false;
    }

    #endregion

    #region Methods

    protected bool canManageEvents(List<msMembershipLeader> leaders)
    {
        if (leaders == null)
            // no leaders to speak of
            return false;

        var leader = leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanManageEvents;
    }

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();

        //Registration Fees
        Search sRegistrationFees = new Search(msRegistrationFee.CLASS_NAME){ID = "RegistrationFees"};
        sRegistrationFees.AddOutputColumn("ID");
        sRegistrationFees.AddOutputColumn("Name");
        sRegistrationFees.AddCriteria(Expr.Equals("Event", targetEventRegistration.Event));
        sRegistrationFees.AddSortColumn("Name");
        
        searches.Add(sRegistrationFees);

        //Registration Categories
        Search sRegistrationCategories = new Search(msRegistrationCategory.CLASS_NAME) {ID = "RegistrationCategories"};
        sRegistrationCategories.AddOutputColumn("ID");
        sRegistrationCategories.AddOutputColumn("Name");
        sRegistrationCategories.AddCriteria(Expr.Equals("Event", targetEventRegistration.Event));
        sRegistrationCategories.AddSortColumn("Name");

        searches.Add(sRegistrationCategories);

        //Registration Classes
        Search sRegistrationClasses = new Search(msRegistrationClass.CLASS_NAME) {ID = "RegistrationClasses"};
        sRegistrationClasses.AddOutputColumn("ID");
        sRegistrationClasses.AddOutputColumn("Name");
        sRegistrationClasses.AddCriteria(Expr.Equals("Event", targetEventRegistration.Event));
        sRegistrationClasses.AddSortColumn("Name");

        searches.Add(sRegistrationClasses);

        List<SearchResult> searchResults = ExecuteSearches(searches, 0, null);
        dvRegistrationFees = new DataView(searchResults.Single(x => x.ID == "RegistrationFees").Table);
        dvRegistrationCategories = new DataView(searchResults.Single(x => x.ID == "RegistrationCategories").Table);
        dvRegistrationClasses = new DataView(searchResults.Single(x => x.ID == "RegistrationClasses").Table);
    }

    protected void loadEventOwners()
    {
        if (!string.IsNullOrWhiteSpace(targetEvent.Chapter))
            targetChapter = LoadObjectFromAPI<msChapter>(targetEvent.Chapter);

        if (!string.IsNullOrWhiteSpace(targetEvent.Section))
            targetSection = LoadObjectFromAPI<msSection>(targetEvent.Section);

        if (!string.IsNullOrWhiteSpace(targetEvent.OrganizationalLayer))
            targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(targetEvent.OrganizationalLayer);
    }

    protected void unbindAndSaveEventRegistration()
    {
        unbindEventRegistration();
        targetEventRegistration = SaveObject(targetEventRegistration).ConvertTo<msEventRegistration>();
    }

    #endregion

    #region Data Binding

    protected void bindEventRegistration()
    {
        ddlCategory.SelectedValue = targetEventRegistration.Category;
        ddlClass.SelectedValue = targetEventRegistration.Class;
        ddlFee.SelectedValue = targetEventRegistration.Fee;

        chkApproved.Checked = targetEventRegistration.Approved;
        dtpCancellationDate.SelectedDate = targetEventRegistration.CancellationDate;
        dtpDateApproved.SelectedDate = targetEventRegistration.DateApproved;
        tbCancellationReason.Text = targetEventRegistration.CancellationReason;
        chkOnWaitList.Checked = targetEventRegistration.OnWaitList;
        dtpCheckInDate.SelectedDate = targetEventRegistration.CheckInDate;

        tbBadgeName.Text = targetEventRegistration.BadgeName;
        tbBadgeOrganization.Text = targetEventRegistration.BadgeOrganization;
        tbBadgeTitle.Text = targetEventRegistration.BadgeTitle;
        tbBadgeCity.Text = targetEventRegistration.BadgeCity;
        tbBadgeState.Text = targetEventRegistration.BadgeState;
        tbBadgeCountry.Text = targetEventRegistration.BadgeCountry;
        
    }

    protected void unbindEventRegistration()
    {
        targetEventRegistration.Category = ddlCategory.SelectedValue;
        targetEventRegistration.Class = ddlClass.SelectedValue;
        targetEventRegistration.Fee = ddlFee.SelectedValue;

        targetEventRegistration.Approved = chkApproved.Checked;
        targetEventRegistration.CancellationDate = dtpCancellationDate.SelectedDate;
        targetEventRegistration.DateApproved = dtpDateApproved.SelectedDate;
        targetEventRegistration.CancellationReason = tbCancellationReason.Text;
        targetEventRegistration.OnWaitList = chkOnWaitList.Checked;
        targetEventRegistration.CheckInDate = dtpCheckInDate.SelectedDate;

        targetEventRegistration.BadgeName = tbBadgeName.Text;
        targetEventRegistration.BadgeOrganization = tbBadgeOrganization.Text;
        targetEventRegistration.BadgeTitle = tbBadgeTitle.Text;
        targetEventRegistration.BadgeCity = tbBadgeCity.Text;
        targetEventRegistration.BadgeState = tbBadgeState.Text;
        targetEventRegistration.BadgeCountry = tbBadgeCountry.Text;
      
    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        unbindAndSaveEventRegistration();
        GoTo(string.Format("~/events/ViewEventRegistration.aspx?ContextID={0}", targetEventRegistration.ID));
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("~/events/ViewEventRegistration.aspx?ContextID={0}", targetEventRegistration.ID));
    }

    #endregion
}