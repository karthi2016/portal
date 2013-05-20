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

public partial class careercenter_SearchJobPostings_Criteria : PortalPage
{

    #region Properties

    protected override bool IsPublic
    {
        get { return PortalConfiguration.Current.JobPostingAccessMode == JobPostingAccess.PublicAccess; }
    }

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        using(IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            buildSearchManifest(proxy);
        }
    }

    #endregion


    #region Methods

    private void buildSearchManifest(IConciergeAPIService proxy)
    {
        if (MultiStepWizards.SearchJobPostings.SearchManifest == null)
            MultiStepWizards.SearchJobPostings.SearchManifest = proxy.DescribeSearch(msJobPosting.CLASS_NAME, null).ResultValue;;
    }

    #endregion

    #region Event Handlers

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        MultiStepWizards.SearchJobPostings.SearchBuilder =
                 new SearchBuilder(Search.FromManifest(MultiStepWizards.SearchJobPostings.SearchManifest));

        //Search the company name with the supplied keywords using a contains clause
        SearchOperation soCompany = new Contains
                                        {
                                            FieldName = "CompanyName",
                                            ValuesToOperateOn = new List<object> { tbKeywords.Text }
                                        };
        MultiStepWizards.SearchJobPostings.SearchBuilder.AddOperation(soCompany, SearchOperationGroupType.Or);

        //Search the posting name with the supplied keywords using a contains clause
        SearchOperation soTitle = new Contains
        {
            FieldName = "Name",
            ValuesToOperateOn = new List<object> { tbKeywords.Text }
        };
        MultiStepWizards.SearchJobPostings.SearchBuilder.AddOperation(soTitle, SearchOperationGroupType.Or);

        //Search the posting body with the supplied keywords using a contains clause
        SearchOperation soBody = new Contains
        {
            FieldName = "Body",
            ValuesToOperateOn = new List<object> { tbKeywords.Text }
        };
        MultiStepWizards.SearchJobPostings.SearchBuilder.AddOperation(soBody, SearchOperationGroupType.Or);

        //Forward to results page
        GoTo("~/careercenter/SearchJobPostings_Results.aspx");
    }

    #endregion
}