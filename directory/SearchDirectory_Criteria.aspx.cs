using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Command.Views;
using MemberSuite.SDK.Manifests.Searching;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;
using MemberSuite.SDK.Web.Controls;
using Telerik.Web.UI;

public partial class directory_SearchDirectory_Criteria : PortalPage
{
    #region Constants


    #endregion

    #region Fields

    protected List<FieldMetadata> targetCriteriaFields;
    protected DataRow drMembership;


    #endregion

    #region Intialization

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

        if (MultiStepWizards.SearchDirectory.SearchManifest == null)
           MultiStepWizards.SearchDirectory.SearchManifest  = buildSearchManifest();

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            Search s = new Search(msMembership.CLASS_NAME);
            PortalConfiguration.Current.MembershipDirectorySearchFields.ForEach(x => s.AddOutputColumn(x));

            targetCriteriaFields = proxy.DescribeCompiledSearch(s).ResultValue.Fields;


            foreach (var f in targetCriteriaFields)
            {
                f.PortalPrompt = null; // MS-923
                if (f.DataType == FieldDataType.Reference && f.DisplayType == FieldDisplayType.TextBox )
                    f.DisplayType = FieldDisplayType.AjaxComboBox; // MS-925
            }

            Search sMembership = new Search(msEntity.CLASS_NAME) { ID = msMembership.CLASS_NAME };
            sMembership.AddOutputColumn("ID");
            sMembership.AddOutputColumn("Membership");
            sMembership.AddOutputColumn("Membership.ReceivesMemberBenefits");
            sMembership.AddOutputColumn("Membership.TerminationDate");
            sMembership.AddCriteria(Expr.Equals("ID", ConciergeAPI.CurrentEntity.ID));
            sMembership.AddSortColumn("ID");

            SearchResult srMembership = proxy.ExecuteSearch(sMembership, 0, 1).ResultValue;
            drMembership = srMembership != null && srMembership.Table != null &&
                           srMembership.Table.Rows.Count > 0
                               ? srMembership.Table.Rows[0]
                               : null;
        }
    }

    protected override bool CheckSecurity()
    {
        if(!base.CheckSecurity())
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
        return isActiveMember();
    }

    protected bool isActiveMember()
    {
        if (drMembership == null)
            return false;

        //Check if the appropriate fields exist - if they do not then the membership module is inactive
        if (
            !(drMembership.Table.Columns.Contains("Membership") &&
              drMembership.Table.Columns.Contains("Membership.ReceivesMemberBenefits") &&
              drMembership.Table.Columns.Contains("Membership.TerminationDate")))
            return false;

        //Check there is a membership
        if (string.IsNullOrWhiteSpace(Convert.ToString(drMembership["Membership"])))
            return false;

        //Check the membership indicates membership benefits
        if (!drMembership.Field<bool>("Membership.ReceivesMemberBenefits"))
            return false;

        //At this point if the termination date is null the member should be able to see the restricted directory
        DateTime? terminationDate = drMembership.Field<DateTime?>("Membership.TerminationDate");

        if (terminationDate == null)
            return true;

        //There is a termination date so check if it's future dated
        return terminationDate > DateTime.Now;
    }

    #endregion

    #region Methods

    private SearchManifest buildSearchManifest()
    {
        SearchManifest result;
        
        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            Search s= new Search( msMembership.CLASS_NAME );
            foreach( var f in PortalConfiguration.Current.MembershipDirectoryTabularResultsFields )
                s.AddOutputColumn( f );

            result = proxy.DescribeCompiledSearch(s).ResultValue;
        }

        //Always add the owner ID as an output column and sort on name
        result.DefaultSelectedFields = new List<SearchOutputColumn> { new SearchOutputColumn { Name = "Owner.ID", DisplayName = "Owner ID" } };
        result.DefaultSortFieds = new List<SearchSortColumn> { new SearchSortColumn { Name = "Owner.Name" } };

        result.DefaultSelectedFields.AddRange(
            from field in result.Fields
            select new SearchOutputColumn { Name = field.Name, DisplayName = field.Label});

        return result;
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

        //Dynamically render the search criteria fields - this has to happen in the page load not initialize page in order for the Harvest to work properly
        var classMetadata = createClassMetadata(targetCriteriaFields);

        //Change any checkbox fields to a drop down list
        foreach (var fieldMetadata in classMetadata.Fields.Where(fieldMetadata => fieldMetadata.DisplayType == FieldDisplayType.CheckBox))
        {
            fieldMetadata.DisplayType = FieldDisplayType.DropDownList;
            fieldMetadata.PickListEntries.Clear();
            fieldMetadata.PickListEntries.Add(new PickListEntry("True", "true"));
            fieldMetadata.PickListEntries.Add(new PickListEntry("False", "false"));
            fieldMetadata.DefaultValue = null;
        }

        cfsSearchCriteria.Metadata = classMetadata;
        cfsSearchCriteria.PageLayout = createViewMetadata(classMetadata.Fields);
        cfsSearchCriteria.MemberSuiteObject = new MemberSuiteObject();
        cfsSearchCriteria.Render();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        MultiStepWizards.SearchDirectory.SearchBuilder =
            new SearchBuilder(Search.FromManifest(MultiStepWizards.SearchDirectory.SearchManifest));

        cfsSearchCriteria.Harvest();
        MemberSuiteObject mso = cfsSearchCriteria.MemberSuiteObject;

        ParseSearchCriteria(targetCriteriaFields, mso, MultiStepWizards.SearchDirectory.SearchBuilder);

        // MS-2152 - we need to make sure we're excluded terminated folks, and opt out
        Search s = MultiStepWizards.SearchDirectory.SearchBuilder.Search;

        SearchOperationGroup sog = new SearchOperationGroup();
        sog.Criteria.AddRange(s.Criteria);
        s.Criteria.Clear();
        s.GroupType = SearchOperationGroupType.And;

        s.AddCriteria(Expr.Equals(msMembership.FIELDS.MembershipDirectoryOptOut, false));
        s.AddCriteria(Expr.IsBlank(msMembership.FIELDS.TerminationDate ));
        s.AddCriteria(sog); // now, add the criteria

        GoTo("~/directory/SearchDirectory_Results.aspx");
    }

    //protected void lbAddCriterion_Click(object sender, EventArgs e)
    //{
    //    // now, get the operation type
    //    var currentOperationSpanElement = ddlOperations.Attributes["currentOperation"];

    //    // we know the validation groups all match up to the attributes, because
    //    // we coded it that way... so now, let's validate the inputs
    //    Validate(currentOperationSpanElement);  // validate 

    //    if (!IsValid)
    //        return;      // things aren't as advertised!

    //    var so = extractSearchOperationFromPage(currentOperationSpanElement);

    //    if (so == null)
    //        return;

    //    // now, let's pull the search from the command session
    //    SearchBuilder sb = MultiStepWizards.SearchDirectory.SearchBuilder;

    //    if (sb == null)
    //        return;

    //    // add the criteria
    //    if (ddlOpenParens.SelectedValue == "(")
    //        sb.OpenParenthesis();

    //    sb.AddOperation(so, ddlConjunction.SelectedValue == "or" ? SearchOperationGroupType.Or : SearchOperationGroupType.And);

    //    if (ddlCloseParens.SelectedValue == ")")
    //        sb.CloseParenthesis();


    //    rpAjaxPanel.Redirect(Request.RawUrl);
    //    // Refresh(); doesn't work in ajax
    //    // see http://www.telerik.com/help/aspnet-ajax/ajxredirectingtoanotherpage.html
    //    return;

    //}

    #endregion
}

