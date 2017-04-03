using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class committees_EditCommittee : PortalPage
{
    #region Fields

    protected msCommittee targetCommittee;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

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

        targetCommittee = LoadObjectFromAPI<msCommittee>(ContextID);
        if(targetCommittee == null || !targetCommittee.ShowInPortal)
        {
            GoToMissingRecordPage();
            return;
        }

        loadCommitteeOwners();
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
#pragma warning disable 0618
        reDescription.NewLineBr = false;
#pragma warning restore 0618
        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");

        bindObjectToPage();

        PageTitleExtension.Text = targetCommittee.Name;
    }

    protected override bool CheckSecurity()
    {
        if(!base.CheckSecurity())
            return false;

        if (targetChapter != null)
            return targetCommittee.ShowInPortal && canManageCommittees(targetChapter.Leaders);

        if (targetSection != null)
            return targetCommittee.ShowInPortal && canManageCommittees(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return targetCommittee.ShowInPortal && canManageCommittees(targetOrganizationalLayer.Leaders);

        using (var api = GetServiceAPIProxy())
            return CommitteeLogic.IsAdministrativeMember(api, targetCommittee.ID, ConciergeAPI.CurrentEntity.ID);
    }

    #endregion

#region Methods


    protected void loadCommitteeOwners()
    {
        if (!string.IsNullOrWhiteSpace(targetCommittee.Chapter))
            targetChapter = LoadObjectFromAPI<msChapter>(targetCommittee.Chapter);

        if (!string.IsNullOrWhiteSpace(targetCommittee.Section))
            targetSection = LoadObjectFromAPI<msSection>(targetCommittee.Section);

        if (!string.IsNullOrWhiteSpace(targetCommittee.OrganizationalLayer))
            targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(targetCommittee.OrganizationalLayer);
    }


    protected bool canManageCommittees(List<msMembershipLeader> leaders)
    {
        if (leaders == null)
            // no leaders to speak of
            return false;

        var leader = leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanManageCommittees;
    }

    protected void setOwnerBackLinks(string ownerId, string ownerName, string viewUrl, string manageEventsUrl)
    {
        hlEventOwner.NavigateUrl = string.Format("{0}?contextID={1}", viewUrl, ownerId);
        hlEventOwner.Text = string.Format("{0} >", ownerName);
        hlEventOwner.Visible = true;
    }

#endregion

    #region Data Binding

    protected void bindObjectToPage()
    {
        tbName.Text = targetCommittee.Name;
        reDescription.Content = targetCommittee.Description;
    }

    protected void unbindObjectFromPage()
    {
        targetCommittee.Name = tbName.Text;
        targetCommittee.Description = reDescription.Content;
    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;


        unbindObjectFromPage();

        targetCommittee = SaveObject(targetCommittee).ConvertTo<msCommittee>();

        QueueBannerMessage("Committee information was updated successfully.");

        GoTo(string.Format("~/committees/ViewCommittee.aspx?contextID={0}", ContextID));
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("~/chapters/ViewChapter.aspx?contextID={0}", ContextID));
    }

    #endregion
}