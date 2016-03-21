using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class financial_RectifySuspendedBillingSchedules : PortalPage 
{
    protected override void InitializePage()
    {
        base.InitializePage();


        // check for failed deferred billing schedules
        Search sBillingSchedule = new Search(msBillingSchedule.CLASS_NAME);
        sBillingSchedule.AddCriteria(Expr.Equals("Order.BillTo", CurrentEntity.ID));
        sBillingSchedule.AddCriteria(Expr.Equals(msBillingSchedule.FIELDS.Status, "Suspended"));
        sBillingSchedule.AddOutputColumn("Order.Name");
        sBillingSchedule.AddOutputColumn("Order.Date");
        sBillingSchedule.AddOutputColumn("Order.Total");
        sBillingSchedule.AddOutputColumn("Order.Memo");

        var results = APIExtensions.GetSearchResult(sBillingSchedule, 0, null);

        if (results.TotalRowCount == 1)   // just redirect
            Response.Redirect("RectifySuspendedBillingSchedule.aspx?contextID=" + results.Table.Rows[0]["ID"]);

        gvTransactions.DataSource = results.Table;
        gvTransactions.DataBind();


    }

     

    protected void btnGoHome_Click(object sender, EventArgs e)
    {
        GoHome();
    }
}