using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class continuingeducations_ViewCEUCreditHistory : PortalPage 
{

    protected override void InitializePage()
    {
        base.InitializePage();

        Search s = new Search(msCEUCredit.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Owner", ConciergeAPI.CurrentEntity.ID));
        s.AddOutputColumn("CreditDate");
        s.AddOutputColumn("Type.Name");
        s.AddOutputColumn("Quantity");
        s.AddOutputColumn("ComponentRegistration.Component.Name");
        s.AddOutputColumn(msCEUCredit.FIELDS.Verified );
        s.AddOutputColumn(msCEUCredit.FIELDS.SelfReported);
        s.AddOutputColumn("Event.Name");

        gvCredits.DataSource = APIExtensions.GetSearchResult(s, 0, null).Table;
        gvCredits.DataBind();

        btnReport.Visible = PortalConfiguration.CurrentConfig.CEUSelfReportingMode != CertificationsSelfReportingMode.Disabled;
    }

    protected void btnReport_Click(object sender, EventArgs e)
    {
        GoTo("ReportCredit.aspx");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void gvCredits_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Row.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                HyperLink hlEdit = (HyperLink)e.Row.FindControl("hlEdit");
                LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");

                bool selfReport = Convert.ToBoolean(drv[msCEUCredit.FIELDS.SelfReported]);
                bool verified = Convert.ToBoolean(drv[msCEUCredit.FIELDS.Verified]);

                if (selfReport && !verified)
                {
                    hlEdit.Visible = true;
                    lbDelete.Visible = true;

                    hlEdit.NavigateUrl = "ReportCredit.aspx?contextID=" + drv["ID"];
                    RegisterJavascriptConfirmationBox(lbDelete, "Are you sure you want to delete this CEU credit? This operation cannot be undone.");
                    lbDelete.CommandArgument = drv["ID"].ToString();
                }
                else
                {
                    hlEdit.Visible = false;
                    lbDelete.Visible = false;
                }

                break;
        }
    }

    protected void gvCredits_Command(object sender, GridViewCommandEventArgs e)
    {
        string id = e.CommandArgument .ToString();
        using (var api = GetConciegeAPIProxy())
        {
            api.Delete(id);
        }
        QueueBannerMessage("Your CEU credit has been deleted successfully.");

        Refresh();
   
    }
}