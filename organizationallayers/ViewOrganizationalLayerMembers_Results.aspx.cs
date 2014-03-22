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

public partial class organizationalLayers_ViewOrganizationalLayerMembers_Results : PortalPage
{
    #region Fields

    protected msOrganizationalLayer targetOrganizationalLayer;
    protected DataView dvResults;
    protected Search targetSearch;

    protected DataRow drTargetOrganizationalLayerType;

    protected msMembershipLeader leader;

    #endregion

    #region Properties

    protected bool Download
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Request.QueryString["download"]))
                return false;

            bool result;
            if (!bool.TryParse(Request.QueryString["download"], out result))
                return false;

            return result;
        }
    }

    protected string Filter
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Request.QueryString["filter"]))
                return null;

            return Request.QueryString["filter"].ToLower();
        }
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

        if (MultiStepWizards.ViewOrganizationalLayerMembers.SearchManifest == null || MultiStepWizards.ViewOrganizationalLayerMembers.SearchBuilder == null)
        {
            GoTo(string.Format("~/organizationalLayers/ViewOrganizationalLayerMembers_SelectFields.aspx?contextID={0}", ContextID));
            return;
        }

        targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(ContextID);

        if (targetOrganizationalLayer == null) GoToMissingRecordPage();

        targetSearch = getSearch();

        drTargetOrganizationalLayerType =
    PortalConfiguration.OrganizationalLayerTypes.Rows.Find(targetOrganizationalLayer.Type);

        //Has to be in the InitializeTargetObject to have the leader before running the CheckSecurity
        getLeader();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        return leader != null && leader.CanViewMembers;
    }

    #endregion

    #region Methods

    protected void getLeader()
    {
        Search sLeaders = GetOrganizationalLayerLeaderSearch(targetOrganizationalLayer.ID);
        SearchResult srLeaders = ExecuteSearch(sLeaders, 0, 1);

        leader = ConvertLeaderSearchResult(srLeaders);
    }

    protected Search getSearch()
    {
        Search s = MultiStepWizards.ViewOrganizationalLayerMembers.SearchBuilder.Search.Clone();
        if (s.OutputColumns.FirstOrDefault(x => x.Name == "Membership.ID") == null)
            s.OutputColumns.Add(new SearchOutputColumn() { Name = "Membership.ID" });

        //Setup the clause to recursively members who have a membership somewhere nested under the supplied organizational layer
        SearchOperationGroup organizationalLayerMembershipClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
        organizationalLayerMembershipClause.Criteria.Add(Expr.Equals("Chapter.Layer", ContextID));
        //Add the recursive query for all the parent organizational layers
        StringBuilder sbOrganizationalLayer = new StringBuilder("Chapter.Layer");
        //Add Organizational Layers
        for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
        {
            sbOrganizationalLayer.Append(".{0}");
            organizationalLayerMembershipClause.Criteria.Add(Expr.Equals(string.Format(sbOrganizationalLayer.ToString(), "ParentLayer"), ContextID));
        }

        s.AddCriteria(organizationalLayerMembershipClause);
        s.UniqueResult = true;

        SearchOperationGroup filter = getFilter();
        if (filter != null)
            s.AddCriteria(filter);
        
        return s;
    }

    protected SearchOperationGroup getFilter()
    {
        SearchOperationGroup result = new SearchOperationGroup();
        SearchOperationGroup terminationGroup;

        switch (Filter)
        {
            case "expired":
                result.GroupType = SearchOperationGroupType.Or;
                result.Criteria.Add(Expr.IsLessThan("Membership.TerminationDate", DateTime.Now));
                result.Criteria.Add(Expr.Equals("Membership.ReceivesMemberBenefits", false));
                result.Criteria.Add(Expr.Equals("IsCurrent", 0));
                break;
            case "active":
                result.GroupType = SearchOperationGroupType.And;
                terminationGroup = new SearchOperationGroup { FieldName = "Membership.TerminationDate" };
                terminationGroup.Criteria.Add(Expr.Equals("Membership.TerminationDate", null));
                terminationGroup.Criteria.Add(Expr.IsGreaterThan("Membership.TerminationDate", DateTime.Now));
                terminationGroup.GroupType = SearchOperationGroupType.Or;

                result.Criteria.Add(terminationGroup);
                result.Criteria.Add(Expr.Equals("Membership.ReceivesMemberBenefits", true));
                result.Criteria.Add(Expr.Equals("IsCurrent", 1));
                break;
            default:
                result = null;
                break;
        }

        return result;
    }

    protected string executeDownloadableSearch()
    {
        using (IConciergeAPIService serviceProxy = GetConciegeAPIProxy())
        {
            return serviceProxy.ExecuteSearchWithFileOutput(targetSearch, BuiltInSearchOutputTypes.ExcelFormatted, false).ResultValue;
        }
    }

    protected void loadDataFromConcierge()
    {
        SearchResult sr = ExecuteSearch(targetSearch, 0, null);
        
        //get distinct rows with the ToTable method
        DataView tempdv = new DataView(sr.Table);
        dvResults = new DataView(tempdv.ToTable(true, (from DataColumn column in sr.Table.Columns where column.ColumnName.ToLower() != "row_number" select column.ColumnName).ToArray()));

        lblSearchResultCount.Text = string.Format("Search returned {0} result(s).", sr.TotalRowCount);
    }

    #endregion

    #region Data Binding

    protected void bindResultsGrid()
    {
        foreach (var selectedField in targetSearch.OutputColumns)
        {
            if(selectedField.Name == "Membership.ID")
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
            DataNavigateUrlFields = new[] { "Membership.ID" },
            DataNavigateUrlFormatString = "~/membership/ViewMembership.aspx?contextID={0}",
            Text = "(view)",
            ItemStyle = { VerticalAlign = VerticalAlign.Top }
        };
        gvMembers.Columns.Add(field);

        gvMembers.DataSource = dvResults;
        gvMembers.DataBind();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        if (Download)
        {
            string nextUrl = executeDownloadableSearch();
            Response.Redirect(nextUrl ?? "~/SearchQueued.aspx");
        }

        loadDataFromConcierge();

        bindResultsGrid();
    }

    #endregion
}