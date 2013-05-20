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

public partial class events_CreateEditInformationLink : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msEventInformationLink targetEventInformationLink;
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected ClassMetadata eventInformationLinkClassMetadata;
    protected Dictionary<string, FieldMetadata> eventInformationLinkFieldMetadata;

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

        //Describe an Event Information Link.  We will need this metadata to bind to the acceptable values for certain fields and for creating a new Information link
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            eventInformationLinkClassMetadata = proxy.DescribeObject(msEventInformationLink.CLASS_NAME).ResultValue;
            eventInformationLinkFieldMetadata = eventInformationLinkClassMetadata.GenerateFieldDictionary();
        }

        MemberSuiteObject contextObject = LoadObjectFromAPI(ContextID);

        if (contextObject.ClassType == msEvent.CLASS_NAME)
        {
            targetEvent = contextObject.ConvertTo<msEvent>();
            targetEventInformationLink = msEventInformationLink.FromClassMetadata(eventInformationLinkClassMetadata);
            lblTitleAction.Text = "Create";
        }
        else
        {
            targetEventInformationLink = contextObject.ConvertTo<msEventInformationLink>();
            targetEvent = LoadObjectFromAPI<msEvent>(targetEventInformationLink.Event);
            lblTitleAction.Text = "Edit";
        }

        if (targetEvent == null || targetEventInformationLink == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEventInformationLink.Event = targetEvent.ID;

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

        lblEventName.Text = targetEvent.Name;

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");

        bindEventInformationLink();
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

    protected void setOwnerBackLinks(string ownerId, string ownerName, string viewUrl, string manageEventsUrl)
    {
        hlEventOwner.NavigateUrl = string.Format("{0}?contextID={1}", viewUrl, ownerId);
        hlEventOwner.Text = string.Format("{0} >", ownerName);
        hlEventOwner.Visible = true;

        hlEventOwnerTask.Text = string.Format("Back to Manage {0} Events", ownerName);
        hlEventOwnerTask.NavigateUrl = string.Format("{0}?contextID={1}", manageEventsUrl, ownerId);
        liEventOwnerTask.Visible = true;
    }

    protected void unbindAndSave()
    {
        unbindEventDiscountCode();
        targetEventInformationLink = SaveObject(targetEventInformationLink).ConvertTo<msEventInformationLink>();
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

    #region Data Binding

    protected void bindEventInformationLink()
    {
        tbName.Text = targetEventInformationLink.Name;
        tbCode.Text = targetEventInformationLink.Code;

        chkIsActive.Checked = targetEventInformationLink.IsActive;
        tbDisplayOrder.Text = targetEventInformationLink.DisplayOrder.ToString();

        reHtml.Content = targetEventInformationLink.Html;
    }

    protected void unbindEventDiscountCode()
    {
        targetEventInformationLink.Name = tbName.Text;
        targetEventInformationLink.Code = tbCode.Text;

        targetEventInformationLink.IsActive = chkIsActive.Checked;

        if (string.IsNullOrWhiteSpace(tbDisplayOrder.Text))
            targetEventInformationLink.DisplayOrder = 0;
        else targetEventInformationLink.DisplayOrder = int.Parse(tbDisplayOrder.Text);

        targetEventInformationLink.Html = reHtml.Content;

    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        unbindAndSave();
        GoTo(string.Format("~/events/CreateEditEvent.aspx?contextID={0}",targetEvent.ID));
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("~/events/CreateEditEvent.aspx?contextID={0}", targetEvent.ID));
    }

    #endregion
}