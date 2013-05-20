using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class chapters_BrowseChapters : PortalPage
{
    #region Fields

    protected msOrganizationalLayer targetOrganizationalLayer;
    protected DataView dvChapters;

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

        //Has to be in the InitializeTargetObject to have the leader before running the CheckSecurity
        loadDataFromConcierge();
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


        hlOrganizationalLayer.NavigateUrl = hlOrganizationalLayerTask.NavigateUrl =
                                            string.Format(
                                                "~/organizationallayers/ViewOrganizationalLayer.aspx?contextID={0}",
                                                targetOrganizationalLayer.ID);

        hlOrganizationalLayer.Text = hlOrganizationalLayerTask.Text = targetOrganizationalLayer.Name;


        gvChapters.DataSource = dvChapters;
        gvChapters.DataBind();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return leader != null;
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();

        //Get all the chapters for the chapter & member counts
        //Setup the clause to recursively find chapters that are somewhere nested under the current organizational layer
        SearchOperationGroup organizationalLayerChapterClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
        organizationalLayerChapterClause.Criteria.Add(Expr.Equals("Layer", ContextID));
        //Add the recursive query for all the parent organizational layers
        StringBuilder sbChapter = new StringBuilder("Layer");
        //Add Organizational Layers
        for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
        {
            sbChapter.Append(".{0}");
            organizationalLayerChapterClause.Criteria.Add(Expr.Equals(string.Format(sbChapter.ToString(), "ParentLayer"), ContextID));
        }

        Search sChapters = new Search(msChapter.CLASS_NAME) { ID = msChapter.CLASS_NAME };
        sChapters.AddOutputColumn("ID");
        sChapters.AddOutputColumn("Name");
        sChapters.AddOutputColumn("LinkedOrganization._Preferred_Address");
        sChapters.AddCriteria(organizationalLayerChapterClause);
        sChapters.AddSortColumn("Name");
        searches.Add(sChapters);

        Search sLeaders = GetOrganizationalLayerLeaderSearch(targetOrganizationalLayer.ID);
        searches.Add(sLeaders);

        List<SearchResult> searchResults = ExecuteSearches(searches, 0, null);
        dvChapters = new DataView(searchResults.Single(x => x.ID == msChapter.CLASS_NAME).Table);
        
        leader = ConvertLeaderSearchResult(searchResults.Single(x => x.ID == "OrganizationalLayerLeader"));
    }

    #endregion
}