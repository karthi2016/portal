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

public partial class events_CreateEditWaivedRegistrationList : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msWaivedRegistrationList targetWaivedRegistrationList;
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected ClassMetadata waivedRegistrationListClassMetadata;
    protected Dictionary<string, FieldMetadata> waivedRegistrationListFieldMetadata;

    protected DataView dvMembers;

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

        //Describe an Event Discount Code.  We will need this metadata to bind to the acceptable values for certain fields and for creating a new Discount / Promo Code
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            waivedRegistrationListClassMetadata = proxy.DescribeObject(msWaivedRegistrationList.CLASS_NAME).ResultValue;
            waivedRegistrationListFieldMetadata = waivedRegistrationListClassMetadata.GenerateFieldDictionary();
        }

        var contextObject = APIExtensions.LoadObjectFromAPI(ContextID);

        if (contextObject.ClassType == msEvent.CLASS_NAME)
        {
            targetEvent = contextObject.ConvertTo<msEvent>();
            targetWaivedRegistrationList = msWaivedRegistrationList.FromClassMetadata(waivedRegistrationListClassMetadata);
            lblTitleAction.Text = "Create";
        }
        else
        {
            targetWaivedRegistrationList = contextObject.ConvertTo<msWaivedRegistrationList>();
            targetEvent = LoadObjectFromAPI<msEvent>(targetWaivedRegistrationList.Event);
            lblTitleAction.Text = "Edit";
        }

        if (targetEvent == null || targetWaivedRegistrationList == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetWaivedRegistrationList.Event = targetEvent.ID;

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

        lblEventName.Text = targetEvent.Name;

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");

        bindWaivedRegistrationList();

        gvMembers.DataSource = dvMembers;
        gvMembers.DataBind();
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

    protected void loadDataFromConcierge()
    {
        Search sMembersOnList = new Search("WaivedRegistrationListMember");
        sMembersOnList.AddOutputColumn("Individual.ID");
        sMembersOnList.AddOutputColumn("Individual.LocalID");
        sMembersOnList.AddOutputColumn("Individual.FirstName");
        sMembersOnList.AddOutputColumn("Individual.LastName");
        sMembersOnList.AddOutputColumn("IsRegistered");
        sMembersOnList.AddCriteria(Expr.Equals("List", targetWaivedRegistrationList.ID));
        sMembersOnList.AddSortColumn("Individual.LastName");
        sMembersOnList.AddSortColumn("Individual.FirstName");

        SearchResult srMembersOnList = APIExtensions.GetSearchResult(sMembersOnList, 0, null);
        dvMembers = new DataView(srMembersOnList.Table);
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

    #region Data Binding

    protected void unbindAndSave()
    {
        unbindWaivedRegistrationList();
        targetWaivedRegistrationList = SaveObject(targetWaivedRegistrationList).ConvertTo<msWaivedRegistrationList>();
    }

    protected void bindWaivedRegistrationList()
    {
        tbName.Text = targetWaivedRegistrationList.Name;
        tbDiscountPercentage.Text = string.Format("{0:F}", targetWaivedRegistrationList.DiscountPercentage);
        chkIsActive.Checked = targetWaivedRegistrationList.IsActive;

        chkAppliesToSessions.Checked = targetWaivedRegistrationList.AppliesToSessions;
        chkAppliesToGuestRegistrations.Checked = targetWaivedRegistrationList.AppliesToGuestRegistration;
        chkAppliesToEventMerchandise.Checked = targetWaivedRegistrationList.AppliesToEventMerchandise;

        tbNotes.Text = targetWaivedRegistrationList.Notes;
    }

    protected void unbindWaivedRegistrationList()
    {
        targetWaivedRegistrationList.Name = tbName.Text;
        targetWaivedRegistrationList.DiscountPercentage = decimal.Parse(tbDiscountPercentage.Text);
        targetWaivedRegistrationList.IsActive = chkIsActive.Checked;

        targetWaivedRegistrationList.AppliesToSessions = chkAppliesToSessions.Checked;
        targetWaivedRegistrationList.AppliesToGuestRegistration = chkAppliesToGuestRegistrations.Checked;
        targetWaivedRegistrationList.AppliesToEventMerchandise = chkAppliesToEventMerchandise.Checked;

        targetWaivedRegistrationList.Notes = tbNotes.Text;
    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        unbindAndSave();
        GoTo(string.Format("~/events/CreateEditEvent.aspx?contextID={0}", targetEvent.ID));
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("~/events/CreateEditEvent.aspx?contextID={0}", targetEvent.ID));
    }

    protected void gvMembers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string memberId = (string)e.CommandArgument;

        switch (e.CommandName.ToLower())
        {
            case "removemember":
                if (!targetWaivedRegistrationList.Members.Exists(x => string.Equals(x, memberId, StringComparison.CurrentCultureIgnoreCase)))
                    QueueBannerError("Unable to remove member.");
                else
                {
                    targetWaivedRegistrationList.Members.RemoveAll(
                        x => string.Equals(x, memberId, StringComparison.CurrentCultureIgnoreCase));
                    targetWaivedRegistrationList =
                        SaveObject(targetWaivedRegistrationList).ConvertTo<msWaivedRegistrationList>();
                    QueueBannerMessage("Member successfully removed.");
                }
                Refresh();
                break;
        }
    }

    protected void lblAddMember_Click(object sender, EventArgs e)
    {
        unbindAndSave();
        GoTo(string.Format("~/events/WaivedRegistrationList_SelectMember.aspx?contextID={0}", targetWaivedRegistrationList.ID));
    }

    #endregion
}