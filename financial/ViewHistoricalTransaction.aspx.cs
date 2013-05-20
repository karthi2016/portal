using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class financial_ViewHistoricalTransaction : PortalPage
{
    #region Fields

    protected msHistoricalTransaction targetHistoricalTransaction;

    protected msEvent targetEvent;
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;
    
    #endregion

    #region Properties

    protected string CompleteUrl
    {
        get { return Request.QueryString["completeUrl"]; }
    }

    protected string EventId
    {
        get { return Request.QueryString["eventId"]; }
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

        targetHistoricalTransaction = LoadObjectFromAPI<msHistoricalTransaction>(ContextID);
        targetEvent = LoadObjectFromAPI<msEvent>(EventId);

        if(targetHistoricalTransaction == null || targetEvent == null)
        {
            GoToMissingRecordPage();
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

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");
    }

    /// <summary>
    /// Checks to make sure that this page is being access properly.
    /// </summary>
    /// <returns></returns>
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
        return targetHistoricalTransaction.Owner == ConciergeAPI.CurrentEntity.ID ||
               ConciergeAPI.AccessibleEntities.Exists(x => x.ID == targetHistoricalTransaction.Owner);
    }

    #endregion

    #region Methods

    protected void loadEventOwners()
    {
        if (!string.IsNullOrWhiteSpace(targetEvent.Chapter))
            targetChapter = LoadObjectFromAPI<msChapter>(targetEvent.Chapter);

        if (!string.IsNullOrWhiteSpace(targetEvent.Section))
            targetSection = LoadObjectFromAPI<msSection>(targetEvent.Section);

        if (!string.IsNullOrWhiteSpace(targetEvent.OrganizationalLayer))
            targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(targetEvent.OrganizationalLayer);
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

    protected void GoToNextUrl(string message)
    {
        if (!string.IsNullOrWhiteSpace(CompleteUrl))
            GoTo(CompleteUrl, message);

        GoHome(message);
    }

    protected void GoToNextUrl()
    {
        if (!string.IsNullOrWhiteSpace(CompleteUrl))
            GoTo(CompleteUrl);

        GoHome();
    }

    #endregion

    #region Event Handlers

    protected void lbDeleteHistoricalTransaction_Click(object sender, EventArgs e)
    {
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            proxy.Delete(targetHistoricalTransaction.ID);
        }

        GoToNextUrl("Historical Transaction was deleted successfully.");
    }

    #endregion
}