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

public partial class careercenter_SearchEventRegistrations_Results : PortalPage
{
    #region Fields

    protected Search targetSearch;
    protected msEvent targetEvent;
    protected DataView dvEventRegistrations;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    #endregion

    #region Properties

    protected bool Download
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Request.QueryString["download"]))
                return false;

            bool result;
            if (!bool.TryParse(Request.QueryString["download"], out result))
                return false;

            return result;
        }
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

        targetSearch = MultiStepWizards.SearchEventRegistrations.SearchBuilder.Search.Clone();

        if (targetSearch == null)
        {
            GoTo(string.Format("~/events/SearchEventRegistrations_Criteria.aspx?contextID={0}",ContextID));
            return;
        }

        targetEvent = LoadObjectFromAPI<msEvent>(ContextID);

        if(targetEvent == null)
        {
            GoToMissingRecordPage();
            return;
        }

        addRequiredSearchParameters();

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


    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        if (Download)
        {
            string nextUrl = executeDownloadableSearch();
            Response.Redirect(nextUrl);
        }

        loadDataFromConcierge();

        gvEventRegistrations.DataSource = dvEventRegistrations;
        gvEventRegistrations.DataBind();
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
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            SearchResult srEventRegistrations = ExecuteSearch(proxy, targetSearch, 0, null);
            dvEventRegistrations = new DataView(srEventRegistrations.Table);
        
            lblSearchResultCount.Text = string.Format("Search returned {0} result(s).", srEventRegistrations.TotalRowCount);
        }
    }

    protected string executeDownloadableSearch()
    {
        using (IConciergeAPIService serviceProxy = GetConciegeAPIProxy())
        {
            return serviceProxy.ExecuteSearchWithFileOutput(targetSearch, BuiltInSearchOutputTypes.ExcelFormatted, false).ResultValue;
        }
    }

    protected void addRequiredSearchParameters()
    {
        if (!targetSearch.OutputColumns.Exists(x => x.Name == "ID"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "ID", DisplayName = "ID" });

        if (!targetSearch.OutputColumns.Exists(x => x.Name == "Name"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "Name", DisplayName = "Name" });

        if (!targetSearch.OutputColumns.Exists(x => x.Name == "Fee.Name"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "Fee.Name", DisplayName = "Fee Name" });

        targetSearch.AddCriteria(Expr.Equals("Event", targetEvent.ID));
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

    protected void lbNewSearch_Click(object sender, EventArgs e)
    {
        MultiStepWizards.SearchEventRegistrations.SearchBuilder = null;
        MultiStepWizards.SearchEventRegistrations.SearchManifest = null;
        GoTo(string.Format("~/events/SearchEventRegistrations_Criteria.aspx?contextID={0}",targetEvent.ID));
    }

    #endregion
}