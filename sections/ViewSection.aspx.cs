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

public partial class sections_ViewSection : PortalPage
{
    #region Fields

    protected DataRow drTargetSection;
    protected msSection targetSection;
    protected msOrganization linkedOrganization;
    protected Address sectionAddress = new Address();
    protected DataView dvSectionCommittees;
    protected DataView dvSectionEvents;

    protected msMembershipLeader leader;

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

        targetSection = LoadObjectFromAPI<msSection>(ContextID);
        if (targetSection == null)
        {
            GoToMissingRecordPage();
            return;
        }

        if (!string.IsNullOrWhiteSpace(targetSection.LinkedOrganization))
        {
            linkedOrganization = LoadObjectFromAPI<msOrganization>(targetSection.LinkedOrganization);
            setAddress();
        }
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        loadDataFromConcierge();

        //Bind the section committees
        gvSectionCommittees.DataSource = dvSectionCommittees;
        gvSectionCommittees.DataBind();

        //Bind the section events
        gvSectionEvents.DataSource = dvSectionEvents;
        gvSectionEvents.DataBind();

        setupSectionLeaderOptions();

        hlViewDocuments.NavigateUrl += ContextID;
        hlViewDocuments.Visible = IsModuleActive("Documents");

        hlDiscussionBoard.Visible = IsModuleActive("Discussions");
        hlDiscussionBoard.NavigateUrl += ContextID;

        CustomTitle.Text = string.Format("{0} Section", GetSearchResult( drTargetSection, "Name", null));
    }

    private void setupSectionLeaderOptions()
    {
        if (leader == null) return;

        if (leader.CanViewMembers)
            blLeaderTasks.Items.Add(new ListItem("Search for Section Members", "viewmembers"));

        //Now an option while searching using the view members link
        //if (sectionLeader.CanDownloadRoster)
        //    blLeaderTasks.Items.Add(new ListItem("Download Section Roster", "downloadroster"));

        if (leader.CanManageCommittees)
            blLeaderTasks.Items.Add(new ListItem("Manage Section Committees", "managecommittees"));

        if (leader.CanManageEvents)
            blLeaderTasks.Items.Add(new ListItem("Manage Section Events", "manageevents"));

        //if (sectionLeader.CanManageLeaders)
        //    blLeaderTasks.Items.Add(new ListItem("Manage Section Leaders", "manageleaders"));

        if (leader.CanUpdateInformation)
            blLeaderTasks.Items.Add(new ListItem("Update Section Information", "updateinformation"));

        // only show the section if there are things to do!
        divSectionLeaderTasks.Visible = blLeaderTasks.Items.Count > 0;

        hlActiveMembers.NavigateUrl =
        string.Format("/sections/ViewSectionMembers_SelectFields.aspx?contextID={0}&continue=true&filter=active",
                  GetSearchResult(drTargetSection, "ID", null));
        trExpiredMembers.Visible = hlActiveMembers.Visible =
         trTotalMembers.Visible = leader.CanViewMembers;

        lblActiveMembers.Visible = !hlActiveMembers.Visible;
    }

    protected void setAddress()
    {
        if (linkedOrganization == null)
            return;

        msEntityAddress entityAddress =
            linkedOrganization.Addresses.Where(x => x.Type == linkedOrganization.PreferredAddressType).FirstOrDefault();

        if (entityAddress != null)
            sectionAddress = entityAddress.Address;
    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        cfsSectionFields.MemberSuiteObject = targetSection;

        var pageLayout = targetSection.GetAppropriatePageLayout();
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
        {
            divOtherInformation.Visible = false;
            return;
        }

        // setup the metadata
        cfsSectionFields.Metadata = targetSection.DescribeObject();
        cfsSectionFields.PageLayout = pageLayout.Metadata;

        cfsSectionFields.AddReferenceNamesToTargetObject(proxy);

        cfsSectionFields.Render();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        var searches = new List<Search>();

        // Search for the section to get aggregate information
        var sSection = new Search {Type = msSection.CLASS_NAME, ID = msSection.CLASS_NAME};
        sSection.AddOutputColumn("ActiveMemberCount");
        sSection.AddOutputColumn("TotalMemberCount");
        sSection.AddOutputColumn("LocalID");
        sSection.AddOutputColumn("Name");
        sSection.AddOutputColumn("Description");
        sSection.AddCriteria(Expr.Equals("ID", ContextID));
        searches.Add(sSection);

        // Search for related committees
        var sSectionCommittees = new Search {Type = msCommittee.CLASS_NAME, ID = msCommittee.CLASS_NAME};
        sSectionCommittees.AddOutputColumn("ID");
        sSectionCommittees.AddOutputColumn("Name");
        sSectionCommittees.AddOutputColumn("CurrentMemberCount");
        sSectionCommittees.AddCriteria(Expr.Equals("Section.ID", ContextID));
        sSectionCommittees.AddCriteria(Expr.Equals("ShowInPortal", true));

        searches.Add(sSectionCommittees);

        // Search for related events
        var sSectionEvents = new Search {Type = msEvent.CLASS_NAME, ID = msEvent.CLASS_NAME};
        sSectionEvents.AddOutputColumn("ID");
        sSectionEvents.AddOutputColumn("Name");
        sSectionEvents.AddOutputColumn("StartDate");
        sSectionEvents.AddOutputColumn("EndDate");
        sSectionEvents.AddCriteria(Expr.Equals("Section.ID", targetSection.ID));
        sSectionEvents.AddCriteria(Expr.Equals("VisibleInPortal", true));
        sSectionEvents.AddSortColumn("StartDate");
        sSectionEvents.AddSortColumn("EndDate");
        sSectionEvents.AddSortColumn("Name");
        searches.Add(sSectionEvents);

        var sLeader = GetSectionLeaderSearch(targetSection.ID);
        searches.Add(sLeader);

        var searchResults = APIExtensions.GetMultipleSearchResults(searches, 0, null);

        var srSection = searchResults.Single(x => x.ID == msSection.CLASS_NAME);
        if (srSection.TotalRowCount == 0) GoToMissingRecordPage();

        drTargetSection = srSection.Table.Rows[0];
        dvSectionCommittees = new DataView(searchResults.Single(x => x.ID == msCommittee.CLASS_NAME).Table);
        dvSectionEvents = new DataView(searchResults.Single(x => x.ID == msEvent.CLASS_NAME).Table);

        var srLeader = searchResults.Single(x => x.ID == "SectionLeader");
        if (srLeader != null)
            leader = ConvertLeaderSearchResult(srLeader);
    }

    protected int getExpiredMemberCount()
    {
        int active = !drTargetSection.Table.Columns.Contains("ActiveMemberCount") || drTargetSection["ActiveMemberCount"] == DBNull.Value
                         ? 0
                         : (int) drTargetSection["ActiveMemberCount"];
        int total = !drTargetSection.Table.Columns.Contains("TotalMemberCount") || drTargetSection["TotalMemberCount"] == DBNull.Value
                         ? 0
                         : (int)drTargetSection["TotalMemberCount"];

        return total - active;
    }

    #endregion

    #region Event Handlers
   
    protected void blLeaderTasks_Click(object sender, BulletedListEventArgs e)
    {
        ListItem li = blLeaderTasks.Items[e.Index];
        string nextUrl = "/";

        switch (li.Value.ToLower())
        {
            case "createmember":
                MultiStepWizards.CreateAccount.InitiatedByLeader = true;
                nextUrl = "~/profile/CreateAccount_BasicInfo.aspx?Leader=true";
                break;
            case "downloadroster":
                nextUrl = string.Format("~/sections/ViewSectionMembers_SelectFields.aspx?contextID={0}&download=true", ContextID);
                break;
            case "makepayment":
                nextUrl = string.Format("~/sections/MakePaymentForSectionMember.aspx?contextID={0}", ContextID);
                break;
            case "managecommittees":
                nextUrl = string.Format("~/sections/ManageSectionCommittees.aspx?contextID={0}",ContextID);
                break;
            case "manageevents":
                nextUrl = string.Format("~/sections/ManageSectionEvents.aspx?contextID={0}", ContextID);
                break;
            case "manageleaders":
                nextUrl = string.Format("~/sections/ManageSectionLeaders.aspx?contextID={0}", ContextID);
                break;
            case "updateinformation":
                nextUrl = string.Format("~/sections/EditSectionInfo.aspx?contextID={0}", ContextID);
                break;
            case "viewmembers":
                nextUrl = string.Format("~/sections/ViewSectionMembers_SelectFields.aspx?contextID={0}", ContextID);
                break;
        }

        GoTo(nextUrl);
    }

    #endregion
}