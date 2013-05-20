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

public partial class careercenter_SearchJobPostings_Results : PortalPage
{
    #region Fields

    protected Search targetSearch;
    protected DataView dvJobPostings;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get { return PortalConfiguration.Current.JobPostingAccessMode == JobPostingAccess.PublicAccess; }
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

        targetSearch = MultiStepWizards.SearchJobPostings.SearchBuilder.Search.Clone();

        if (targetSearch == null)
        {
            GoTo("~/careercenter/SearchJobPostings_Criteria.aspx");
            return;
        }

        addRequiredSearchParameters();
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

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            loadDataFromConcierge(proxy);
        }

        gvJobPostings.DataSource = dvJobPostings;
        gvJobPostings.DataBind();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge(IConciergeAPIService proxy)
    {
        SearchResult srJobPostings = ExecuteSearch(proxy, targetSearch, 0, null);
        dvJobPostings = new DataView(srJobPostings.Table);

        lblSearchResultCount.Text = string.Format("Search returned {0} result(s).", srJobPostings.TotalRowCount);
    }

    protected void addRequiredSearchParameters()
    {
        if (!targetSearch.OutputColumns.Exists(x => x.Name == "ID"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "ID", DisplayName = "ID" });

        if (!targetSearch.OutputColumns.Exists(x => x.Name == "CompanyName"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "CompanyName", DisplayName = "Company Name" });

        if (!targetSearch.OutputColumns.Exists(x => x.Name == "CompanyName"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "Name", DisplayName = "Job Title" });

        if (!targetSearch.OutputColumns.Exists(x => x.Name == "Location.Name"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "Location.Name", DisplayName = "Location" });


        //Make sure the search has criteria to filter to approved job postings set to be displayed during the current date
        //Start by putting the existing criteria into an "OR" group
        SearchOperationGroup keywordCriteria = new SearchOperationGroup
                                                   {
                                                       GroupType = SearchOperationGroupType.Or,
                                                       Criteria = targetSearch.Criteria.Where(x => true).ToList()
                                                   };

        //Clear the existing criteria then add it back as a group.  This is so that we can group these clauses in an OR and still use AND for the next clauses
        //So it will be like (CompanyName CONTAINS '' OR Name CONTAINS '') AND IsActive = true
        targetSearch.Criteria.Clear();
        targetSearch.GroupType = SearchOperationGroupType.And;
        targetSearch.AddCriteria(keywordCriteria);

        if (!targetSearch.Criteria.Exists(x => x.FieldName == "IsApproved"))
            targetSearch.AddCriteria(Expr.Equals("IsApproved", true));

        if (!targetSearch.Criteria.Exists(x => x.FieldName == "PostOn"))
        {
            SearchOperationGroup postOnGroup = new SearchOperationGroup
                                                   {
                                                       FieldName = "PostOn",
                                                       GroupType = SearchOperationGroupType.Or
                                                   };
            postOnGroup.Criteria.Add(Expr.IsBlank("PostOn"));
            postOnGroup.Criteria.Add(Expr.IsLessThan("PostOn",DateTime.Now));
            targetSearch.AddCriteria(postOnGroup);
        }

        if (!targetSearch.Criteria.Exists(x => x.FieldName == "ExpirationDate"))
        {
            SearchOperationGroup expirationDateGroup = new SearchOperationGroup
            {
                FieldName = "ExpirationDate",
                GroupType = SearchOperationGroupType.Or
            };
            expirationDateGroup.Criteria.Add(Expr.IsBlank("ExpirationDate"));
            expirationDateGroup.Criteria.Add(Expr.IsGreaterThan("ExpirationDate", DateTime.Now));
            targetSearch.AddCriteria(expirationDateGroup);
        }

        //Sorting - for now just by the date created descending
        targetSearch.SortColumns.Clear();
        targetSearch.AddSortColumn("CreatedDate",true);
        targetSearch.AddSortColumn("Name");
    }

    #endregion

    #region Event Handlers

    protected void lbNewSearch_Click(object sender, EventArgs e)
    {
        MultiStepWizards.SearchJobPostings.SearchBuilder = null;
        MultiStepWizards.SearchJobPostings.SearchManifest = null;
        GoTo("~/careercenter/SearchJobPostings_Criteria.aspx");
    }

    #endregion
}