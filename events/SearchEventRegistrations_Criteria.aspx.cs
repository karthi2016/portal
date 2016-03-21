using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Searching;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class event_SearchEventRegistrations_Criteria : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected DataView dvRegistrationFees;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    #endregion

    #region Properties

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

        targetEvent = LoadObjectFromAPI<msEvent>(ContextID);

        if(targetEvent == null)
        {
            GoToMissingRecordPage();
            return;
        }

        buildSearchManifest();

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

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");

        loadDataFromConcierge();

        ddlFee.DataSource = dvRegistrationFees;
        ddlFee.DataBind();

        PageTitleExtension.Text = string.Format("{0} Registrations", targetEvent.Name);
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

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

    protected void loadDataFromConcierge()
    {
        Search sRegistrationFees = new Search(msRegistrationFee.CLASS_NAME);
        sRegistrationFees.AddOutputColumn("ID");
        sRegistrationFees.AddOutputColumn("Name");
        sRegistrationFees.AddCriteria(Expr.Equals("Event",targetEvent.ID));
        sRegistrationFees.AddSortColumn("Name");

        SearchResult srRegistrationFees = APIExtensions.GetSearchResult(sRegistrationFees, 0, null);
        dvRegistrationFees = new DataView(srRegistrationFees.Table);
    }

    protected void buildSearchManifest()
    {
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            if (MultiStepWizards.SearchEventRegistrations.SearchManifest == null)
                MultiStepWizards.SearchEventRegistrations.SearchManifest =
                    proxy.DescribeSearch(msEventRegistration.CLASS_NAME, null).ResultValue;
        }
    }


    protected bool canManageEvents(List<msMembershipLeader> leaders)
    {
        if (leaders == null)
            // no leaders to speak of
            return false;

        var leader = leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanManageEvents;
    }

    protected void setOwnerBackLinks(string ownerId, string ownerName, string viewUrl, string manageEventsUrl)
    {
        hlEventOwner.NavigateUrl = string.Format("{0}?contextID={1}", viewUrl, ownerId);
        hlEventOwner.Text = string.Format("{0} >", ownerName);
        hlEventOwner.Visible = true;

        hlEventOwnerTask.Text = string.Format("Back to Manage {0} Events", ownerName);
        hlEventOwnerTask.NavigateUrl = string.Format("{0}?contextID={1}", manageEventsUrl, ownerId);
        liEventOwnerTask.Visible = true;
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

    #endregion

    #region Event Handlers

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        MultiStepWizards.SearchEventRegistrations.SearchBuilder =
                 new SearchBuilder(Search.FromManifest(MultiStepWizards.SearchEventRegistrations.SearchManifest));

  

        //Add the name criteria as a contains
        SearchOperation soName = new Contains
        {
            FieldName = "Name",
            ValuesToOperateOn = new List<object> { tbName.Text }
        };
        MultiStepWizards.SearchEventRegistrations.SearchBuilder.AddOperation(soName, SearchOperationGroupType.And);

        //Add the fee as an exact match (since it's a drop down)
        SearchOperation soFee = new Equals()
        {
            FieldName = "Fee.Name",
            ValuesToOperateOn = new List<object> { ddlFee.SelectedValue }
        };
        MultiStepWizards.SearchEventRegistrations.SearchBuilder.AddOperation(soName, SearchOperationGroupType.And);

        string nextUrl = rblOutputFormat.SelectedValue == "download"
                     ? string.Format("~/events/SearchEventRegistrations_Results.aspx?contextID={0}&download=true", targetEvent.ID)
                     : string.Format("~/events/SearchEventRegistrations_Results.aspx?contextID={0}",
                                     targetEvent.ID);

        //Forward to results page
        GoTo(nextUrl);
    }

    #endregion
}