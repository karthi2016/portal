using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Searching;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class directory_SearchDirectory_Results : PortalPage
{
    private const string ColumnHeaderOverridePrefix = "ColumnHeader.";

    protected Search targetSearch;
    protected DataView dvMembers;

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

        if (MultiStepWizards.SearchDirectory.SearchBuilder == null || MultiStepWizards.SearchDirectory.SearchBuilder.Search == null)
        {
            GoTo("~/directory/SearchDirectory_Criteria.aspx");
            return;
        }

        targetSearch = MultiStepWizards.SearchDirectory.SearchBuilder.Search.Clone();

        if(targetSearch == null)
        {
            GoTo("~/directory/SearchDirectory_Criteria.aspx");
            return;
        }

        foreach (var column in targetSearch.OutputColumns)
        {
            var labelOverride = PortalConfiguration.GetOverrideFor(
                Request.Url.LocalPath, ColumnHeaderOverridePrefix + column.Name, "Text");

            if (labelOverride != null)
            {
                column.DisplayName = labelOverride.Value;
            }
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

        loadDataFromConcierge();

        bindResultsGrid();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return isMembershipDirectoryAvailable();
    }

    private bool isMembershipDirectoryAvailable()
    {
        if (!PortalConfiguration.Current.MembershipDirectoryEnabled)
            return false;

        //If the directory is enabled and not restricted to members it's available and no need to check membership status
        if (!PortalConfiguration.Current.MembershipDirectoryForMembersOnly)
            return true;

        //Directory is for members only
        return MembershipLogic.IsActiveMember();
    }

    #endregion

    #region Methods

    protected void addRequiredSearchParameters()
    {
        if (!targetSearch.OutputColumns.Exists(x => x.Name == "Owner.ID"))
            targetSearch.OutputColumns.Add(new SearchOutputColumn { Name = "Owner.ID", DisplayName = "Owner ID" });

        if (!targetSearch.Criteria.Exists(x => x.FieldName == "TerminationDate"))
        {
            SearchOperationGroup terminationGroup = new SearchOperationGroup {FieldName = "TerminationDate"};
            terminationGroup.Criteria.Add(Expr.Equals("TerminationDate", null));
            terminationGroup.Criteria.Add(Expr.IsGreaterThan("TerminationDate", DateTime.Now));
            terminationGroup.GroupType = SearchOperationGroupType.Or;    
            targetSearch.AddCriteria(terminationGroup);
        }
    }

    protected void bindResultsGrid()
    {
        gvMembers.Columns.Clear();

        foreach (var selectedField in targetSearch.OutputColumns)
        {
            if (selectedField.Name == "Owner.ID")
                continue;

            TemplateField templateField = new TemplateField
            {
                HeaderText = selectedField.DisplayName,
                ItemTemplate = new SearchResultColumnTemplate(selectedField.Name),
                HeaderStyle = { HorizontalAlign = HorizontalAlign.Left },
                ItemStyle = { VerticalAlign = VerticalAlign.Top }
            };
            gvMembers.Columns.Add(templateField);
        }

        HyperLinkField field = new HyperLinkField
        {
            DataNavigateUrlFields = new[] { "Owner.ID" },
            DataNavigateUrlFormatString = "~/directory/SearchDirectory_ViewDetails.aspx?contextID={0}",
            Text = "(view)",
            ItemStyle = { VerticalAlign = VerticalAlign.Top }
        };
        gvMembers.Columns.Add(field);

        gvMembers.DataSource = dvMembers;
        gvMembers.DataBind();
    }

    protected void loadDataFromConcierge()
    {
        SearchResult srMembers = APIExtensions.GetSearchResult(targetSearch, PageStart, PAGE_SIZE);

        SetCurrentPageFromResults(srMembers, hlFirstPage, hlPrevPage, hlNextPage, lNumPages, lNumResults, lStartResult,
                                  lEndResult, lCurrentPage);
        
        dvMembers = new DataView(srMembers.Table);
    }

    protected override void AddCustomOverrideEligibleControls(List<msPortalControlPropertyOverride> eligibleControls)
    {
        base.AddCustomOverrideEligibleControls(eligibleControls);

        foreach (var column in targetSearch.OutputColumns)
        {
            eligibleControls.Add(new msPortalControlPropertyOverride
            {
                PageName = Request.Url.LocalPath,
                ControlName = ColumnHeaderOverridePrefix + column.Name,
                PropertyName = "Text",
                Value = Convert.ToString(column.DisplayName)
            });
        }
    }

    #endregion

    #region Event Handlers

    protected void lbNewSearch_Click(object sender, EventArgs e)
    {
        MultiStepWizards.SearchDirectory.SearchBuilder = null;
        MultiStepWizards.SearchDirectory.SearchManifest = null;
        GoTo("~/directory/SearchDirectory_Criteria.aspx");
    }
    #endregion
}