using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class continuingeducation_ViewCertificationHistory : PortalPage 
{
    protected override void InitializePage()
    {
        base.InitializePage();

        Search s = new Search(msCertification.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msCertification.FIELDS.Certificant, ConciergeAPI.CurrentEntity.ID));

        s.AddOutputColumn("Program.Name");
        s.AddOutputColumn("Status.Name");
        s.AddOutputColumn("EffectiveDate");
        s.AddOutputColumn("ExpirationDate");

        s.AddSortColumn("EffectiveDate", true);

        var results = ExecuteSearch(s, 0, null);

        gvCredits.DataSource = results.Table;
        gvCredits.DataBind();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }
}