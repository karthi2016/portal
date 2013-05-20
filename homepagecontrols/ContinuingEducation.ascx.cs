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
using MemberSuite.SDK.Types;

public partial class homepagecontrols_ContinuingEducation : HomePageUserControl 
{

    public override void GenerateSearchesToBeRun(List<Search> searchesToRun)
    {
        base.GenerateSearchesToBeRun(searchesToRun);

        Search s = new Search(msCEUCredit.CLASS_NAME) {ID = "CEU"};
        s.AddCriteria(Expr.Equals("Owner", ConciergeAPI.CurrentEntity.ID));
        s.AddOutputColumn("CreditDate");
        s.AddOutputColumn("Type.Name");
        s.AddOutputColumn("Quantity");

        searchesToRun.Add(s);

        return;

    }

    protected override void InitializeWidget()
    {
        base.InitializeWidget();

        Widgets_CEU_hlReportCEUCredits.Visible = PortalConfiguration.CurrentConfig.CEUSelfReportingMode != CertificationsSelfReportingMode.Disabled;
        Widgets_CEU_hlViewCreditHistory.Visible = PortalConfiguration.CurrentConfig.ShowCEUCreditsInPortal;
        Widgets_CEU_hlMyCertificationHistory.Visible = PortalConfiguration.CurrentConfig.ShowCertificationsInPortal ;
        Widgets_CEU_hlReportComponentAttendance.Visible = PortalConfiguration.CurrentConfig.ComponentSelfReportingMode != CertificationsSelfReportingMode.Disabled;
        Widgets_CEU_hlViewMyComponents.Visible = PortalConfiguration.CurrentConfig.ShowComponentRegistrationsInPortal;
    }

    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);

        if (!Visible)
            return;

        SearchResult sr = results.FirstOrDefault(x => x.ID == "CEU");
        if(sr == null)
            return;

        decimal total = 0;
        decimal ytdTotal = 0;
        DateTime dtBeginningOfYear = new DateTime( DateTime.Now.Year, 1, 1 );
        foreach( DataRow dr in sr.Table.Rows )
        {
            decimal qty = (decimal)dr["Quantity"];
            total += qty;

            if ((DateTime)dr["CreditDate"] > dtBeginningOfYear)
                ytdTotal += qty;
        }

        lblNumberOfCredts.Text = total.ToString("N1");
        lblYtdCredits.Text = ytdTotal.ToString("N1");
    }
}