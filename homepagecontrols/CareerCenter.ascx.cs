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

public partial class homepagecontrols_CareerCenter : HomePageUserControl
{
    public override List<string> GetFieldsNeededForMainSearch()
    {
        var list = base.GetFieldsNeededForMainSearch();
        list.Add("CareerCenter_NumberOfResumes");
        list.Add("CareerCenter_NumberOfJobPostings");
      
        return list;
    }

    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);
        if (!Visible) return;


        int singleCareerCenterProducts = 0;
        int multipleCareerCenterProducts = 0;

        foreach (DataRow drProduct in results.Single(x => x.ID == "CareerCenterProducts").Table.Rows)
        {
            if (drProduct["NumberOfJobPostings"] == DBNull.Value) continue;
            
            if ((int) drProduct["NumberOfJobPostings"] > 1)
                multipleCareerCenterProducts++;
            else singleCareerCenterProducts++;
        }

        liPurchaseJobPosting.Visible = multipleCareerCenterProducts > 0;
        liSearchJobPostings.Visible = PortalConfiguration.Current.JobPostingAccessMode != JobPostingAccess.MembersOnly || isActiveMember();

        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            liSearchResumes.Visible = api.CheckEntitlement(msResumeAccessEntitlement.CLASS_NAME, ConciergeAPI.CurrentEntity.ID, null).ResultValue.IsEntitled;
        //liSearchResumes.Visible = drMainRecord["ResumeAccessUntil"] != DBNull.Value && (DateTime)drMainRecord["ResumeAccessUntil"] > DateTime.Now;
        //liPostJob.Visible = (drMainRecord["JobPostingsAvailable"] != DBNull.Value &&
                            //(int)drMainRecord["JobPostingsAvailable"] > 0) || (singleCareerCenterProducts > 0);


        lblNumberOfResumes.Text = drMainRecord["CareerCenter_NumberOfResumes"].ToString();
        lblJobsPosted.Text = drMainRecord["CareerCenter_NumberOfJobPostings"].ToString();
    }

    public override void GenerateSearchesToBeRun(List<Search> searchesToRun)
    {
        base.GenerateSearchesToBeRun(searchesToRun);

        Search sCareerCenterProducts = new Search(msCareerCenterProduct.CLASS_NAME)
                                           {
                                               ID = "CareerCenterProducts"
                                           };
        sCareerCenterProducts.AddOutputColumn("ID");
        sCareerCenterProducts.AddOutputColumn("Name");
        sCareerCenterProducts.AddOutputColumn("NumberOfJobPostings");
        sCareerCenterProducts.AddCriteria(Expr.Equals("SellOnline", true));

        //Create sell from group
        SearchOperationGroup sellFromGroup = new SearchOperationGroup
        {
            FieldName = "SellFrom",
            GroupType = SearchOperationGroupType.Or
        };
        sellFromGroup.Criteria.Add(Expr.Equals("SellFrom", null));
        sellFromGroup.Criteria.Add(Expr.IsLessThanOrEqual("SellFrom", DateTime.Now));
        sCareerCenterProducts.AddCriteria(sellFromGroup);

        //Create sell until group
        SearchOperationGroup sellUntilGroup = new SearchOperationGroup
        {
            FieldName = "SellUntil",
            GroupType = SearchOperationGroupType.Or
        };
        sellUntilGroup.Criteria.Add(Expr.Equals("SellUntil", null));
        sellUntilGroup.Criteria.Add(Expr.IsGreaterThanOrEqualTo("SellUntil", DateTime.Now));
        sCareerCenterProducts.AddCriteria(sellUntilGroup);
        sCareerCenterProducts.AddSortColumn("Name");


        searchesToRun.Add(sCareerCenterProducts);
    }
}