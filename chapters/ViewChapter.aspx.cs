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

public partial class chapters_ViewChapter : PortalPage
{
    #region Fields

    protected DataRow drTargetChapter;
    protected msChapter targetChapter;
    protected DataRow drLinkedOrganization;

    protected DataView dvChapterCommittees;
    protected DataView dvChapterEvents;

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

        targetChapter = LoadObjectFromAPI<msChapter>(ContextID);
        if (targetChapter == null)
        {
            GoToMissingRecordPage();
            return;
        }
    } 

    private void setupChapterLeaderOptions()
    {
        if ( leader == null ) return ;   // no leaders to speak of

        // ok, we're dealing with a leader
        if (leader.CanViewMembers)
            blLeaderTasks.Items.Add(new ListItem("Search for Chapter Members", "viewmembers"));

        //Now an option while searching using the view members link
        //if (chapterLeader.CanDownloadRoster)
        //    blLeaderTasks.Items.Add(new ListItem("Download Chapter Roster", "downloadroster"));


        if (leader.CanCreateMembers)
            blLeaderTasks.Items.Add(new ListItem("Create Member", "createmember"));


        if (leader.CanMakePayments)
            blLeaderTasks.Items.Add(new ListItem("Make a Payment on Behalf of a Member", "makepayment"));

        if (leader.CanMakePayments && !string.IsNullOrWhiteSpace(targetChapter.LinkedOrganization))
            blLeaderTasks.Items.Add(new ListItem("Make a Payment on Behalf of the Chapter", "makepaymentforchapter"));

        if (leader.CanManageCommittees)
            blLeaderTasks.Items.Add(new ListItem("Manage Chapter Committees", "managecommittees"));

        if (leader.CanManageEvents)
            blLeaderTasks.Items.Add(new ListItem("Manage Chapter Events", "manageevents"));

        if (leader.CanManageLeaders)
            blLeaderTasks.Items.Add(new ListItem("Manage Chapter Leaders", "manageleaders"));

        if (leader.CanUpdateInformation)
            blLeaderTasks.Items.Add(new ListItem("Update Chapter Information", "updateinformation"));


        if (leader.CanViewAccountHistory && drLinkedOrganization != null)
        {
            trChapterBalance.Visible = true;
            blLeaderTasks.Items.Add(new ListItem("View Chapter Account History", "viewaccounthistory"));

            //MS-4275 - need to put in ViewState, or else page forgets it on postback
            ViewState["LinkedOrganizationID"] = Convert.ToString( drLinkedOrganization["ID"] );
        }


        // only show the section if there are things to do!
        divChapterLeaderTasks.Visible = blLeaderTasks.Items.Count > 0;

        hlActiveMembers.NavigateUrl =
            string.Format("/chapters/ViewChapterMembers_SelectFields.aspx?contextID={0}&continue=true&filter=active",
                          GetSearchResult(drTargetChapter, "ID", null));
        trExpiredMembers.Visible = hlActiveMembers.Visible =
         trTotalMembers.Visible = leader.CanViewMembers;

        lblActiveMembers.Visible = !hlActiveMembers.Visible;
    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        cfsChapterFields.MemberSuiteObject = targetChapter;

        var pageLayout = GetAppropriatePageLayout(targetChapter);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
        {
            divOtherInformation.Visible = false;
            return;
        }

        // setup the metadata
        cfsChapterFields.Metadata = proxy.DescribeObject(msChapter.CLASS_NAME).ResultValue;
        cfsChapterFields.PageLayout = pageLayout.Metadata;

        cfsChapterFields.AddReferenceNamesToTargetObject(proxy);

        cfsChapterFields.Render();

    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();

        // Search for the chapter to get aggregate information
        Search sChapter = new Search { Type = msChapter.CLASS_NAME, ID = msChapter.CLASS_NAME };
        sChapter.AddOutputColumn("ActiveMemberCount");
        sChapter.AddOutputColumn("TotalMemberCount");
        sChapter.AddOutputColumn("LocalID");
        sChapter.AddOutputColumn("Name");
        sChapter.AddOutputColumn("Description");
        sChapter.AddOutputColumn("Layer");
        sChapter.AddOutputColumn("Layer.Name");
        sChapter.AddOutputColumn("Layer.Type.Name");
        sChapter.AddCriteria(Expr.Equals("ID", ContextID));
        searches.Add(sChapter);

        //Search for related committees
        Search sChapterCommittees = new Search { Type = msCommittee.CLASS_NAME, ID = msCommittee.CLASS_NAME };
        sChapterCommittees.AddOutputColumn("ID");
        sChapterCommittees.AddOutputColumn("Name");
        sChapterCommittees.AddOutputColumn("CurrentMemberCount");
        sChapterCommittees.AddCriteria(Expr.Equals("Chapter.ID", ContextID));
        sChapterCommittees.AddCriteria(Expr.Equals("ShowInPortal", true));
        searches.Add(sChapterCommittees);

        //Search for related events
        Search sChapterEvents = new Search { Type = msEvent.CLASS_NAME, ID = msEvent.CLASS_NAME };

        sChapterEvents.AddOutputColumn("ID");
        sChapterEvents.AddOutputColumn("Name");
        sChapterEvents.AddOutputColumn("StartDate");
        sChapterEvents.AddOutputColumn("EndDate");
        sChapterEvents.AddCriteria(Expr.Equals("Chapter.ID", targetChapter.ID));
        sChapterEvents.AddCriteria(Expr.Equals("VisibleInPortal", true));
        sChapterEvents.AddSortColumn("StartDate");
        sChapterEvents.AddSortColumn("EndDate");
        sChapterEvents.AddSortColumn("Name");
        searches.Add(sChapterEvents);

        //Search for the linked organization
        if(!string.IsNullOrWhiteSpace(targetChapter.LinkedOrganization))
        {
            Search sLinkedOrganizaition = new Search(msOrganization.CLASS_NAME) {ID = msOrganization.CLASS_NAME};
            sLinkedOrganizaition.AddOutputColumn("ID");
            sLinkedOrganizaition.AddOutputColumn("Name");
            sLinkedOrganizaition.AddOutputColumn("Invoices_OpenInvoiceBalance");
            sLinkedOrganizaition.AddOutputColumn("_Preferred_Address_Line1");
            sLinkedOrganizaition.AddOutputColumn("_Preferred_Address_Line2");
            sLinkedOrganizaition.AddOutputColumn("_Preferred_Address_City");
            sLinkedOrganizaition.AddOutputColumn("_Preferred_Address_State");
            sLinkedOrganizaition.AddOutputColumn("_Preferred_Address_PostalCode");
            sLinkedOrganizaition.AddCriteria(Expr.Equals("ID", targetChapter.LinkedOrganization));
            searches.Add(sLinkedOrganizaition);
        }

        Search sLeader = GetChapterLeaderSearch(targetChapter.ID);
        searches.Add(sLeader);

        var searchResults = ExecuteSearches(searches, 0, null);
        if (searchResults.Single(x => x.ID == msChapter.CLASS_NAME).TotalRowCount == 0) GoToMissingRecordPage();

        drTargetChapter = searchResults.Single(x => x.ID == msChapter.CLASS_NAME).Table.Rows[0];
        dvChapterCommittees = new DataView(searchResults.Single(x => x.ID == msCommittee.CLASS_NAME).Table);
        dvChapterEvents = new DataView(searchResults.Single(x => x.ID == msEvent.CLASS_NAME).Table);

        SearchResult srLinkedOrganization = searchResults.FirstOrDefault(x => x.ID == msOrganization.CLASS_NAME);
        if (srLinkedOrganization != null)
            drLinkedOrganization = srLinkedOrganization.Table.Rows[0];

        SearchResult srLeader = searchResults.Single(x => x.ID == "ChapterLeader");
        if (srLeader != null)
            leader = ConvertLeaderSearchResult(srLeader);
    }

    protected int getExpiredMemberCount()
    {
        int active = !drTargetChapter.Table.Columns.Contains("ActiveMemberCount") || drTargetChapter["ActiveMemberCount"] == DBNull.Value
                         ? 0
                         : (int) drTargetChapter["ActiveMemberCount"];
        int total = !drTargetChapter.Table.Columns.Contains("TotalMemberCount") || drTargetChapter["TotalMemberCount"] == DBNull.Value
                         ? 0
                         : (int)drTargetChapter["TotalMemberCount"];

        return total - active;
    }

    #endregion

    #region Event Handlers

    protected override void InitializePage()
    {
        base.InitializePage();
    
        loadDataFromConcierge();
        
        trLayer.Visible = drTargetChapter["Layer"] != DBNull.Value;

        //Bind the chapter committees
        gvChapterCommittees.DataSource = dvChapterCommittees;
        gvChapterCommittees.DataBind();

        //Bind the chapter events
        gvChapterEvents.DataSource = dvChapterEvents;
        gvChapterEvents.DataBind();

        setupChapterLeaderOptions();

        hlViewDocuments.NavigateUrl += ContextID;

        hlDiscussionBoard.Visible = IsModuleActive("Discussions");
        hlDiscussionBoard.NavigateUrl += ContextID;
    }

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
                nextUrl = string.Format("~/chapters/ViewChapterMembers_SelectFields.aspx?contextID={0}&download=true", ContextID);
                break;
            case "makepayment":
                nextUrl = string.Format("~/chapters/MakePaymentForChapterMember.aspx?contextID={0}", ContextID);
                break;
            case "makepaymentforchapter":
                nextUrl = string.Format("~/financial/MakePayment.aspx?contextID={0}", targetChapter.LinkedOrganization);
                break;
            case "managecommittees":
                nextUrl = string.Format("~/chapters/ManageChapterCommittees.aspx?contextID={0}",ContextID);
                break;
            case "manageevents":
                nextUrl = string.Format("~/chapters/ManageChapterEvents.aspx?contextID={0}", ContextID);
                break;
            case "manageleaders":
                nextUrl = string.Format("~/chapters/ManageChapterLeaders.aspx?contextID={0}", ContextID);
                break;
            case "updateinformation":
                nextUrl = string.Format("~/chapters/EditChapterInfo.aspx?contextID={0}", ContextID);
                break;
            case "viewmembers":
                nextUrl = string.Format("~/chapters/ViewChapterMembers_SelectFields.aspx?contextID={0}", ContextID);
                break;
            case "viewaccounthistory":
                //MS-4275 - need to put in ViewState, or else page forgets it on postback
                string linkedOrg = (string) ViewState["LinkedOrganizationID"];
                nextUrl = string.Format("~/financial/AccountHistory.aspx?contextID={0}&leaderOfId={1}", linkedOrg, ContextID);
                break;
        }

        GoTo(nextUrl);
    }

    #endregion
}