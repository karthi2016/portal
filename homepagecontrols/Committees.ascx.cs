using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class homepagecontrols_Committees : HomePageUserControl
{
    public override void GenerateSearchesToBeRun(List<Search> searchesToRun)
    {
        base.GenerateSearchesToBeRun(searchesToRun);

        // search for current committee memberships
        Search sCommitteeMemberships = new Search {Type = msCommitteeMembership.CLASS_NAME, ID = "CommitteeMembership"};
        sCommitteeMemberships.AddOutputColumn("Committee.ID");
        sCommitteeMemberships.AddOutputColumn("Committee.Name");
        sCommitteeMemberships.AddOutputColumn("Position.Name");
        sCommitteeMemberships.AddCriteria(Expr.Equals("Member.ID", ConciergeAPI.CurrentEntity.ID));
        sCommitteeMemberships.AddCriteria(Expr.Equals("IsCurrent", true));
        sCommitteeMemberships.AddCriteria(Expr.Equals("Committee.ShowInPortal", true));
        sCommitteeMemberships.AddSortColumn("Committee.Name");
        searchesToRun.Add(sCommitteeMemberships);
    }

    public override List<string> GetFieldsNeededForMainSearch()
    {
        var list = base.GetFieldsNeededForMainSearch();
        list.Add("Committees_NumberOfCurrentCommitteeMemberships");

        return list;
    }
    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);
        if (!Visible) return;


        //Bind Committees
        gvCommitteeMemberships.DataSource = results.Single(x => x.ID == "CommitteeMembership").Table;
        gvCommitteeMemberships.DataBind();

        lblCommitteeCount.Text = Convert.ToString(drMainRecord["Committees_NumberOfCurrentCommitteeMemberships"]);
    }

}