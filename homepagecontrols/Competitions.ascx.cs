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

public partial class homepagecontrols_Competitions : HomePageUserControl
{
    protected DataView dvJudgingTeams;

    public override List<string> GetFieldsNeededForMainSearch()
    {
        var list =  base.GetFieldsNeededForMainSearch();
        list.Add("Awards_LastEntry");
        list.Add("Awards_LastEntry.LocalID");
        list.Add("Awards_LastEntry.Name");
        list.Add("Awards_LastEntry.Competition.Name");
        list.Add("Awards_LastEntry.DateSubmitted");
        list.Add("Awards_NumberOfEntries");

        return list;
    }

    protected override void InitializeWidget()
    {
        base.InitializeWidget();

        rptJudgingTeams.DataSource = dvJudgingTeams;
        rptJudgingTeams.DataBind();
    }

    public override void GenerateSearchesToBeRun(List<Search> searchesToRun)
    {
        base.GenerateSearchesToBeRun(searchesToRun);

        Search s = new Search("JudgingTeamMember") {ID = "JudgingTeams"};
        s.AddOutputColumn("Team.ID");
        s.AddOutputColumn("Team.Competition.Name");
        s.AddCriteria(Expr.Equals("Member.ID", ConciergeAPI.CurrentEntity.ID));
        s.AddCriteria(Expr.Equals("Team.Competition.IsJudgingOpen", true));
        searchesToRun.Add(s);
    }

    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);
        if (!Visible) return;

        string name = string.IsNullOrWhiteSpace(drMainRecord["Awards_LastEntry.Name"] as string)
                          ? drMainRecord["Awards_LastEntry.LocalID"].ToString()
                          : (string)drMainRecord["Awards_LastEntry.Name"];

        if (drMainRecord["Awards_LastEntry"] != DBNull.Value)
            lblLastCompetitionEntry.Text = string.Format("\"{0}\" for {1} on {2}", name,
                                                         drMainRecord["Awards_LastEntry.Competition.Name"],
                                                         drMainRecord["Awards_LastEntry.DateSubmitted"]);

        lblCompetitionEntryCount.Text = Convert.ToString(drMainRecord["Awards_NumberOfEntries"]);

        dvJudgingTeams = new DataView(results.Single(x => x.ID == "JudgingTeams").Table);

    }
    
}