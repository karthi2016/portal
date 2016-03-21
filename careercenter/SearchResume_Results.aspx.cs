using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;

public partial class careercenter_SearchResume_Results : PortalPage
{
    #region Fields

    protected Search targetSearch;
    protected DataView dvResumes;

    #endregion

    #region Properties

    public string Output
    {
        get { return Request.QueryString["output"]; }
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

        targetSearch = MultiStepWizards.SearchResumeBank.SearchBuilder.Search.Clone();

        if (targetSearch == null)
        {
            GoTo("~/careercenter/SearchResume_Criteria.aspx");
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

        if (Output == "zip")
        {
            string downloadLocation;
            using (var api = GetServiceAPIProxy())
            {
                downloadLocation = api.ExecuteSearchWithFileOutput(targetSearch, targetSearch.OutputFormat, false).ResultValue;
            }
            if (string.IsNullOrWhiteSpace(downloadLocation))
                GoTo("~/SearchQueued.aspx");
            Response.Redirect(downloadLocation);
        }

        loadDataFromConcierge();

        bindResultsGrid();
    }

    #endregion

    #region Methods

    protected void addRequiredSearchParameters()
    {
        if (!targetSearch.OutputColumns.Exists(x => x.Name == "ID"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "ID", DisplayName = "ID" });

        if (!targetSearch.Criteria.Exists(x => x.FieldName == "IsActive"))
            targetSearch.AddCriteria(Expr.Equals("IsActive", true));

        if (!targetSearch.Criteria.Exists(x => x.FieldName == "IsApproved"))
            targetSearch.AddCriteria(Expr.Equals("IsApproved", true));

        if (Output == "zip")
            targetSearch.OutputFormat = "builtin:zip";
    }

    protected void bindResultsGrid()
    {
        foreach (var selectedField in targetSearch.OutputColumns)
        {
            if (selectedField.Name == "ID")
                continue;

            TemplateField templateField = new TemplateField
                                              {
                                                  HeaderText = selectedField.DisplayName,
                                                  ItemTemplate = new SearchResultColumnTemplate(selectedField.Name),
                                                  HeaderStyle = {HorizontalAlign = HorizontalAlign.Left},
                                                  ItemStyle = {VerticalAlign = VerticalAlign.Top}
                                              };
            gvMembers.Columns.Add(templateField);
        }

        HyperLinkField field = new HyperLinkField
        {
            DataNavigateUrlFields = new[] { "ID" },
            DataNavigateUrlFormatString = "~/careercenter/SearchResume_ViewDetails.aspx?contextID={0}",
            Text = "(view)",
            ItemStyle = { VerticalAlign = VerticalAlign.Top }
        };
        gvMembers.Columns.Add(field);

        gvMembers.DataSource = dvResumes;
        gvMembers.DataBind();
    }


    protected void loadDataFromConcierge()
    {
        SearchResult srResumes = APIExtensions.GetSearchResult(targetSearch, 0, null);
        dvResumes = new DataView(srResumes.Table);

        lblSearchResultCount.Text = string.Format("Search returned {0} result(s).", srResumes.TotalRowCount);
    }

    #endregion

    #region Event Handlers

    protected void lbNewSearch_Click(object sender, EventArgs e)
    {
        MultiStepWizards.SearchResumeBank.SearchBuilder = null;
        MultiStepWizards.SearchResumeBank.SearchManifest = null;
        GoTo("~/careercenter/SearchResume_Criteria.aspx");
    }

    #endregion
}