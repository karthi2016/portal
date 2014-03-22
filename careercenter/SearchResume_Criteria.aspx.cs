using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Searching;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;

public partial class careercenter_SearchResume_Criteria : PortalPage
{
    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            return api.CheckEntitlement(msResumeAccessEntitlement.CLASS_NAME, ConciergeAPI.CurrentEntity.ID, null).ResultValue.IsEntitled;
    }
    #region Constants


    #endregion

    #region Fields

    protected List<FieldMetadata> targetCriteriaFields;


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

        if (PortalConfiguration.Current.ResumeSearchFields == null || PortalConfiguration.Current.ResumeTabularResultsFields == null || PortalConfiguration.Current.ResumeDetailsFields == null)
        {
            QueueBannerError("The resume search fields must first be configured in the Console.  Please set the fields in the Career Center Setup.");
            GoHome();
        }

        if (MultiStepWizards.SearchResumeBank.SearchManifest == null)
            MultiStepWizards.SearchResumeBank.SearchManifest = buildSearchManifest();

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            targetCriteriaFields =
                proxy.GetSearchFieldMetadataFromFullPath(msResume.CLASS_NAME, null,
                                                         PortalConfiguration.Current.ResumeSearchFields).
                    ResultValue;
        }
    }

    #endregion

    #region Methods

    private SearchManifest buildSearchManifest()
    {
        SearchManifest result;

        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            result = proxy.DescribeSearch(msResume.CLASS_NAME, null).ResultValue;
            result.Fields.Clear();
            result.Fields =
                proxy.GetSearchFieldMetadataFromFullPath(msResume.CLASS_NAME, null, PortalConfiguration.Current.ResumeTabularResultsFields).ResultValue;
        }

        //Always add the resume ID as an output column and sort on name
        result.DefaultSelectedFields = new List<SearchOutputColumn> { new SearchOutputColumn { Name = "ID", DisplayName = "ID" } };
        result.DefaultSortFieds = new List<SearchSortColumn> { new SearchSortColumn { Name = "Owner.Name" } };

        result.DefaultSelectedFields.AddRange(
            from field in result.Fields
            select new SearchOutputColumn { Name = field.Name, DisplayName = field.Label });

        return result;
    }

    protected void addSearchOperations(SearchOperationGroup group, MemberSuiteObject mso)
    {
        foreach (var searchOperation in group.Criteria)
        {
            SearchOperationGroup sog = searchOperation as SearchOperationGroup;
            if (sog != null)
            {
                addSearchOperations(sog, mso);
                continue;
            }

            string safeFieldName = RegularExpressions.GetSafeFieldName(searchOperation.FieldName);

            if (mso.Fields.ContainsKey(safeFieldName))
                continue;

            switch (searchOperation.ValuesToOperateOn.Count)
            {
                case 0:
                    mso[safeFieldName] = null;
                    break;
                case 1:
                    mso[safeFieldName] = searchOperation.ValuesToOperateOn[0];
                    break;
                default:
                    mso[safeFieldName] = searchOperation.ValuesToOperateOn;
                    break;
            }
        }
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

        // MS-1251 - DON'T use the portal prompt
        classMetadata.Fields.ForEach(x => x.PortalPrompt = null);

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
        var searchBuilder = new SearchBuilder(Search.FromManifest(MultiStepWizards.SearchResumeBank.SearchManifest));
        MultiStepWizards.SearchResumeBank.SearchBuilder =
            searchBuilder;

        cfsSearchCriteria.Harvest();
        MemberSuiteObject mso = cfsSearchCriteria.MemberSuiteObject;

        string keywords = tbKeywords.Text;
        if (!string.IsNullOrWhiteSpace(keywords))
            searchBuilder.AddOperation(
                new Keyword { FieldName = "File", ValuesToOperateOn = new List<object> { keywords } },
                SearchOperationGroupType.And);


        ParseSearchCriteria(targetCriteriaFields, mso, searchBuilder);

        string nextUrl = "~/careercenter/SearchResume_Results.aspx";
        if (rblOutputFormat.SelectedValue == "zip")
            nextUrl += "?output=zip";
        GoTo(nextUrl);
    }

    #endregion
}