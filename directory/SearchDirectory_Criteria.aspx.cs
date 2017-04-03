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
    private const string ColumnHeaderOverridePrefix = "ColumnHeader.";

    protected List<FieldMetadata> targetCriteriaFields;

    protected override bool IsPublic
    {
        get
        {
            return PortalConfiguration.Current.MembershipDirectoryIsPublic;
        }
    }

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
            var s = new Search("MembershipWithFlowdown");
            PortalConfiguration.Current.MembershipDirectorySearchFields.ForEach(x => s.AddOutputColumn(x));

            targetCriteriaFields = proxy.DescribeCompiledSearch(s).ResultValue.Fields;


            foreach (var f in targetCriteriaFields)
            {
                f.PortalPrompt = null; // MS-923
                if (f.DataType == FieldDataType.Reference && f.DisplayType == FieldDisplayType.TextBox )
                    f.DisplayType = FieldDisplayType.AjaxComboBox; // MS-925

                var labelOverride = PortalConfiguration.GetOverrideFor(
                    Request.Url.LocalPath, ColumnHeaderOverridePrefix + f.Name, "Text");

                if (labelOverride != null)
                {
                    f.Label = labelOverride.Value;
                }
            }
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

        if (PortalConfiguration.Current.MembershipDirectoryIsPublic)
            return true;

        // If the directory is enabled and not restricted to members it's available and no need to check membership status
        if (!PortalConfiguration.Current.MembershipDirectoryForMembersOnly)
            return true;

        // Directory is for members only
        return MembershipLogic.IsActiveMember();
    }

    #endregion

    #region Methods

    private SearchManifest buildSearchManifest()
    {
        SearchManifest result;
        
        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            Search s = new Search("MembershipWithFlowdown");
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
        foreach (
            var fieldMetadata in
                classMetadata.Fields.Where(fieldMetadata => fieldMetadata.DisplayType == FieldDisplayType.CheckBox))
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


        s.AddCriteria(Expr.Equals(msMembership.FIELDS.ReceivesMemberBenefits, true ));
        s.AddCriteria(Expr.Equals(msMembership.FIELDS.MembershipDirectoryOptOut, false));

        var sogTerminationDate = new SearchOperationGroup{GroupType = SearchOperationGroupType.Or };
        sogTerminationDate.Criteria.Add( Expr.IsBlank(msMembership.FIELDS.TerminationDate ));
        sogTerminationDate.Criteria.Add(Expr.IsGreaterThan(msMembership.FIELDS.TerminationDate, DateTime.Today ));
        s.AddCriteria(sogTerminationDate);

        // MS-5850 - Control whether or not inherited memberships are included
        if ( ! PortalConfiguration.Current.MembershipDirectoryIncludeInheritedMemberships )
            s.AddCriteria(Expr.Equals(msMembership.FIELDS.IsInherited, false ));

        s.AddCriteria(sog); // now, add the criteria

        GoTo("~/directory/SearchDirectory_Results.aspx");
    }

    protected override void AddCustomOverrideEligibleControls(List<msPortalControlPropertyOverride> eligibleControls)
    {
        base.AddCustomOverrideEligibleControls(eligibleControls);

        foreach (var targetCriteriaField in targetCriteriaFields)
        {
            eligibleControls.Add(new msPortalControlPropertyOverride
            {
                PageName = Request.Url.LocalPath,
                ControlName = ColumnHeaderOverridePrefix + targetCriteriaField.Name,
                PropertyName = "Text",
                Value = Convert.ToString(targetCriteriaField.Label)
            });
        }
    }

    #endregion
}

