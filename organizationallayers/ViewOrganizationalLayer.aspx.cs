using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class chapters_ViewOrganizationalLayer : PortalPage
{
    #region Fields

    protected DataRow drTargetOrganizationalLayer;
    protected DataRow drTargetOrganizationalLayerType;

    //protected List<DataRow> childOrganizationalLayerTypeRows;
    protected Dictionary<string, List<DataRow>> childOrganizationalLayers;

    protected msOrganizationalLayer targetOrganizationalLayer;
    protected DataView dvOrganizationalLayerCommittees;
    protected DataView dvOrganizationalLayerEvents;

    protected int memberCount;
    protected int activeMemberCount;
    protected int chapterCount;

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

        targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(ContextID);
        if (targetOrganizationalLayer == null)
        {
            GoToMissingRecordPage();
            return;
        }

        drTargetOrganizationalLayerType =
            PortalConfiguration.OrganizationalLayerTypes.Rows.Find(targetOrganizationalLayer.Type);
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

        trParentLayer.Visible = drTargetOrganizationalLayer["ParentLayer"] != DBNull.Value;

        //Bind the child layers
        rptChildOrganizationalLayers.DataSource = childOrganizationalLayers.Keys;
        rptChildOrganizationalLayers.DataBind();

        //Bind the chapter committees
        gvOrganizationalLayerCommittees.DataSource = dvOrganizationalLayerCommittees;
        gvOrganizationalLayerCommittees.DataBind();

        //Bind the chapter events
        gvOrganizationalLayerEvents.DataSource = dvOrganizationalLayerEvents;
        gvOrganizationalLayerEvents.DataBind();

        setupOrganizationalLayerLeaderOptions();

        hlViewDocuments.NavigateUrl += ContextID;
        hlViewDocuments.Visible = IsModuleActive("Documents");

        hlDiscussionBoard.Visible = IsModuleActive("Discussions");
        hlDiscussionBoard.NavigateUrl += ContextID;

        CustomTitle.Text = string.Format("{0} {1}", GetSearchResult(drTargetOrganizationalLayer, "Name", null), GetSearchResult(drTargetOrganizationalLayerType, "Name", null));
    }



    private void setupOrganizationalLayerLeaderOptions()
    {
        if (leader == null) return;   // no leaders to speak of

        // ok, we're dealing with a leader
        if (leader.CanViewMembers)
            blLeaderTasks.Items.Add(new ListItem(string.Format("Search for {0} Members", drTargetOrganizationalLayerType["Name"]), "viewmembers"));

        //Now an option while searching using the view members link
        //if (organizationalLayerLeader.CanDownloadRoster)
        //    blLeaderTasks.Items.Add(new ListItem("Download OrganizationalLayer Roster", "downloadroster"));


        //if (organizationalLayerLeader.CanCreateMembers)
        //    blLeaderTasks.Items.Add(new ListItem("Create Member", "createmember"));


        //if (organizationalLayerLeader.CanMakePayments)
        //    blLeaderTasks.Items.Add(new ListItem("Make a Payment on Behalf of a Member", "makepayment"));

        if (leader.CanManageCommittees)
            blLeaderTasks.Items.Add(new ListItem(string.Format("Manage {0} Committees", drTargetOrganizationalLayerType["Name"]), "managecommittees"));

        if (leader.CanManageEvents)
            blLeaderTasks.Items.Add(new ListItem(string.Format("Manage {0} Events", drTargetOrganizationalLayerType["Name"]), "manageevents"));

        //if (organizationalLayerLeader.CanManageLeaders)
        //    blLeaderTasks.Items.Add(new ListItem(string.Format("Manage {0} Leaders", drTargetOrganizationalLayerType["Name"]), "manageleaders"));

        if (leader.CanUpdateInformation)
            blLeaderTasks.Items.Add(new ListItem(string.Format("Update {0} Information", drTargetOrganizationalLayerType["Name"]), "updateinformation"));



        // only show the section if there are things to do!
        divOrganizationalLayerLeaderTasks.Visible = blLeaderTasks.Items.Count > 0;

        trActiveMembers.Visible = trExpiredMembers.Visible =
            trChapterCount.Visible = trTotalMembers.Visible = leader.CanViewMembers;

    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        cfsOrganizationalLayerFields.MemberSuiteObject = targetOrganizationalLayer;

        var pageLayout = targetOrganizationalLayer.GetAppropriatePageLayout();
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
        {
            divOtherInformation.Visible = false;
            return;
        }

        // setup the metadata
        cfsOrganizationalLayerFields.Metadata = targetOrganizationalLayer.DescribeObject();
        cfsOrganizationalLayerFields.PageLayout = pageLayout.Metadata;

        cfsOrganizationalLayerFields.AddReferenceNamesToTargetObject(proxy);

        cfsOrganizationalLayerFields.Render();

    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        var searches = new List<Search>();

        // Search for the organizational layer to get aggregate information
        var sOrganizationalLayer = new Search
                                          {
                                              Type = msOrganizationalLayer.CLASS_NAME,
                                              ID = targetOrganizationalLayer.ID
                                          };
        sOrganizationalLayer.AddOutputColumn("LocalID");
        sOrganizationalLayer.AddOutputColumn("Name");
        sOrganizationalLayer.AddOutputColumn("Description");
        sOrganizationalLayer.AddOutputColumn("ParentLayer");
        sOrganizationalLayer.AddOutputColumn("ParentLayer.Name");
        sOrganizationalLayer.AddOutputColumn("ParentLayer.Type.Name");
        sOrganizationalLayer.AddCriteria(Expr.Equals("ID", ContextID));
        searches.Add(sOrganizationalLayer);

        // Search for related committees
        var sOrganizationalLayerCommittees = new Search {Type = msCommittee.CLASS_NAME, ID = msCommittee.CLASS_NAME};
        sOrganizationalLayerCommittees.AddOutputColumn("ID");
        sOrganizationalLayerCommittees.AddOutputColumn("Name");
        sOrganizationalLayerCommittees.AddOutputColumn("CurrentMemberCount");
        sOrganizationalLayerCommittees.AddCriteria(Expr.Equals("OrganizationalLayer.ID", ContextID));
        sOrganizationalLayerCommittees.AddCriteria(Expr.Equals("ShowInPortal", true));
        searches.Add(sOrganizationalLayerCommittees);

        // Search for related events
        var sOrganizationalLayerEvents = new Search { Type = msEvent.CLASS_NAME, ID = msEvent.CLASS_NAME };
        sOrganizationalLayerEvents.AddOutputColumn("ID");
        sOrganizationalLayerEvents.AddOutputColumn("Name");
        sOrganizationalLayerEvents.AddOutputColumn("StartDate");
        sOrganizationalLayerEvents.AddOutputColumn("EndDate");
        sOrganizationalLayerEvents.AddCriteria(Expr.Equals("OrganizationalLayer.ID", targetOrganizationalLayer.ID));
        sOrganizationalLayerEvents.AddCriteria(Expr.Equals("VisibleInPortal", true));
        sOrganizationalLayerEvents.AddSortColumn("StartDate");
        sOrganizationalLayerEvents.AddSortColumn("EndDate");
        sOrganizationalLayerEvents.AddSortColumn("Name");
        searches.Add(sOrganizationalLayerEvents);

        // Get all the chapters for the chapter & member counts
        // Setup the clause to recursively find chapters that are somewhere nested under the current organizational layer
        var organizationalLayerChapterClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
        organizationalLayerChapterClause.Criteria.Add(Expr.Equals("Layer", ContextID));

        // Add the recursive query for all the parent organizational layers
        var sbChapter = new StringBuilder("Layer");

        // Add Organizational Layers
        for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
        {
            sbChapter.Append(".{0}");
            organizationalLayerChapterClause.Criteria.Add(Expr.Equals(string.Format(sbChapter.ToString(), "ParentLayer"), ContextID));
        }

        var sChapters = new Search(msChapter.CLASS_NAME) {ID = msChapter.CLASS_NAME};
        sChapters.AddOutputColumn("ID");
        sChapters.AddOutputColumn("Name");
        sChapters.AddOutputColumn("ActiveMemberCount");
        sChapters.AddOutputColumn("TotalMemberCount");
        sChapters.AddCriteria(organizationalLayerChapterClause);
        sChapters.AddSortColumn("Name");
        searches.Add(sChapters);

        // Search for child layers
        var sOrganizationalLayers = new Search(msOrganizationalLayer.CLASS_NAME)
                                           {ID = msOrganizationalLayer.CLASS_NAME};
        sOrganizationalLayers.AddOutputColumn("ID");
        sOrganizationalLayers.AddOutputColumn("Name");
        sOrganizationalLayers.AddOutputColumn("Type");
        sOrganizationalLayers.AddOutputColumn("Type.Name");
        sOrganizationalLayers.AddCriteria(Expr.Equals("ParentLayer", ContextID));
        sOrganizationalLayers.AddSortColumn("Type.ParentType.Name");
        sOrganizationalLayers.AddSortColumn("Name");
        searches.Add(sOrganizationalLayers);

        // Search for leaders (by searching it will include inherited leaders not included in the targetOrganizationalLayer.Leaders property)
        var sLeaders = GetOrganizationalLayerLeaderSearch(targetOrganizationalLayer.ID);
        searches.Add(sLeaders);

        var searchResults = APIExtensions.GetMultipleSearchResults(searches, 0, null);

        var targetOrganizationalLayerSearchResult = searchResults.Single(x => x.ID == targetOrganizationalLayer.ID);
        if (targetOrganizationalLayerSearchResult.TotalRowCount == 0) GoToMissingRecordPage();
        drTargetOrganizationalLayer = targetOrganizationalLayerSearchResult.Table.Rows[0];
        
        dvOrganizationalLayerCommittees = new DataView(searchResults.Single(x => x.ID == msCommittee.CLASS_NAME).Table);
        dvOrganizationalLayerEvents = new DataView(searchResults.Single(x => x.ID == msEvent.CLASS_NAME ).Table);

        // Determine chapter counts
        var chapterSearchResult = searchResults.Single(x => x.ID == msChapter.CLASS_NAME);

        chapterCount = chapterSearchResult.Table.Rows.Count;

        // Determine member counts
        foreach (DataRow chapterRow in chapterSearchResult.Table.Rows)
        {
            activeMemberCount += !chapterRow.Table.Columns.Contains("ActiveMemberCount") || chapterRow["ActiveMemberCount"] == DBNull.Value ? 0 : (int)chapterRow["ActiveMemberCount"];
            memberCount += !chapterRow.Table.Columns.Contains("TotalMemberCount") || chapterRow["TotalMemberCount"] == DBNull.Value ? 0 : (int)chapterRow["TotalMemberCount"];
        }

        childOrganizationalLayers = new Dictionary<string, List<DataRow>>();
        foreach (DataRow drOrganizationalLayer in searchResults.Single(x => x.ID == msOrganizationalLayer.CLASS_NAME).Table.Rows)
        {
            string typeName = (string)drOrganizationalLayer["Type.Name"];

            if (!childOrganizationalLayers.ContainsKey(typeName))
                childOrganizationalLayers.Add(typeName, new List<DataRow>());

            childOrganizationalLayers[typeName].Add(drOrganizationalLayer);
        }

        leader = ConvertLeaderSearchResult(searchResults.Single(x => x.ID == "OrganizationalLayerLeader"));
    }

    protected bool hasPermissionsToView(string organizationalLayerId)
    {
        // Organizational Layer Leadership
        var sOrganizationalLayerLeadership = new Search { Type = "OrganizationalLayerLeader" };
        sOrganizationalLayerLeadership.AddOutputColumn("OrganizationalLayer.ID");
        sOrganizationalLayerLeadership.AddCriteria(Expr.Equals("Individual.ID", ConciergeAPI.CurrentEntity.ID));
        sOrganizationalLayerLeadership.AddCriteria(Expr.Equals("OrganizationalLayer.ID", organizationalLayerId));
        sOrganizationalLayerLeadership.AddSortColumn("OrganizationalLayer.Name");

        var srOrganizationalLayerLeadership = APIExtensions.GetSearchResult(sOrganizationalLayerLeadership, 0, 1);
        if (srOrganizationalLayerLeadership.Table.Rows.Count > 0)
            return true;

        var searches = new List<Search>();

        // Setup the clause to recursively determine if the user has a membership somewhere nested under the supplied organizational layer
        var organizationalLayerMembershipClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
        organizationalLayerMembershipClause.Criteria.Add(Expr.Equals("Chapter.Layer", organizationalLayerId));

        // Add the recursive query for all the parent organizational layers

        var sbOrganizationalLayer = new StringBuilder("Chapter.Layer");
        // Add Organizational Layers
        for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
        {
            sbOrganizationalLayer.Append(".{0}");
            organizationalLayerMembershipClause.Criteria.Add(Expr.Equals(string.Format(sbOrganizationalLayer.ToString(), "ParentLayer"), organizationalLayerId));
        }

        // Organizational Layer Membership
        var sOrganizationalLayerMembership = new Search(msChapterMembership.CLASS_NAME);
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.ID");
        sOrganizationalLayerMembership.AddCriteria(organizationalLayerMembershipClause);
        sOrganizationalLayerMembership.AddCriteria(Expr.Equals("Membership.Owner.ID", ConciergeAPI.CurrentEntity.ID));
        sOrganizationalLayerMembership.AddSortColumn("Chapter.Name");
        searches.Add(sOrganizationalLayerMembership);

        var srOrganizationalLayerMembership = APIExtensions.GetMultipleSearchResults(searches, 0, 1);
        return srOrganizationalLayerMembership.Any(searchResult => searchResult.Table.Rows.Count > 0);
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
                nextUrl = string.Format("~/organizationallayers/ViewOrganizationalLayerMembers_SelectFields.aspx?contextID={0}&download=true", ContextID);
                break;
            case "makepayment":
                nextUrl = string.Format("~/organizationallayers/MakePaymentForOrganizationalLayerMember.aspx?contextID={0}", ContextID);
                break;
            case "managecommittees":
                nextUrl = string.Format("~/organizationallayers/ManageOrganizationalLayerCommittees.aspx?contextID={0}", ContextID);
                break;
            case "manageevents":
                nextUrl = string.Format("~/organizationallayers/ManageOrganizationalLayerEvents.aspx?contextID={0}", ContextID);
                break;
            case "manageleaders":
                nextUrl = string.Format("~/organizationallayers/ManageOrganizationalLayerLeaders.aspx?contextID={0}", ContextID);
                break;
            case "updateinformation":
                nextUrl = string.Format("~/organizationallayers/EditOrganizationalLayerInfo.aspx?contextID={0}", ContextID);
                break;
            case "viewmembers":
                nextUrl = string.Format("~/organizationallayers/ViewOrganizationalLayerMembers_SelectFields.aspx?contextID={0}", ContextID);
                break;
        }

        GoTo(nextUrl);
    }

    protected void rptChildOrganizationalLayers_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        string typeName = (string)e.Item.DataItem;

        //Now bind the gridview in the repeater
        GridView gv = (GridView)e.Item.FindControl("gvOrganizationalLayers");

        gv.DataSource = childOrganizationalLayers[typeName];
        gv.DataBind();

    }

    protected void gvOrganizationalLayers_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataRow drChildLayerRow = (DataRow)e.Row.DataItem;

        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                HyperLink hlChildLayer = (HyperLink)e.Row.FindControl("hlChildOrganizationalLayer");
                Label lblChildLayer = (Label)e.Row.FindControl("lblChildOrganizationalLayer");

                hlChildLayer.Visible = hasPermissionsToView(drChildLayerRow["ID"].ToString());
                lblChildLayer.Visible = !hlChildLayer.Visible;

                break;
        }
    }

    #endregion
}