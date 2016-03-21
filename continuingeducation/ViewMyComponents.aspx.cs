using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using Telerik.Web.UI;

public partial class continuingeducation_ViewMyComponents : PortalPage 
{
    protected void rgMainDataGrid_ItemCreated(object sender, GridItemEventArgs e)
    {
         
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        dpFrom.SelectedDate = DateTime.Today.AddDays(-30);
       // dpTo.SelectedDate = DateTime.Today;
    }

    protected void rgMainDataGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        Search s = new Search(msCertificationComponentRegistration.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msCertificationComponentRegistration.FIELDS.Student, ConciergeAPI.CurrentEntity.ID));
        s.AddSortColumn("Component.StartDate");

        s.AddOutputColumn("Component.StartDate");
        s.AddOutputColumn("CreatedDate");
        s.AddOutputColumn("Verified");
        s.AddOutputColumn("Component.Name");

        // now, get the credits
        Search s2 = new Search(msCEUCredit.CLASS_NAME);
        s2.AddCriteria( Expr.Equals( msCEUCredit.FIELDS.Owner, ConciergeAPI.CurrentEntity.ID) );
        s2.AddOutputColumn(msCEUCredit.FIELDS.Quantity);
        s2.AddOutputColumn(msCEUCredit.FIELDS.ComponentRegistration);
        s2.AddOutputColumn("Type.Name");
        s2.AddSortColumn("Type.Name");

        if (dpFrom.SelectedDate != null)
        {
            s.AddCriteria(Expr.IsGreaterThanOrEqualTo("Component.StartDate", dpFrom.SelectedDate.Value));
            s2.AddCriteria(Expr.IsGreaterThanOrEqualTo("ComponentRegistration.Component.StartDate",
                                                       dpFrom.SelectedDate.Value));
        }

        if (dpTo.SelectedDate != null)
        {
            s.AddCriteria(Expr.IsLessThanOrEqual("Component.StartDate", dpTo.SelectedDate.Value));
            s2.AddCriteria(Expr.IsLessThanOrEqual("ComponentRegistration.Component.StartDate", dpTo.SelectedDate.Value));
        }

        var dt = APIExtensions.GetSearchResult(s, 0, null).Table;
        
        // let's get the credits, too
        var dtCredits = APIExtensions.GetSearchResult(s2, 0, null).Table;

        dt.Columns.Add("Credits");

        Dictionary<string, decimal> creditDictionary = new Dictionary<string, decimal>();
        foreach (DataRow dr in dt.Rows)
        {
            StringBuilder sbCredits = new StringBuilder();
            // we'll iterate through all the credits and find the ones that match this component
            foreach (DataRow dr2 in dtCredits.Rows)
            {
                if ((Guid)dr["ID"] != (Guid)dr2["ComponentRegistration"]) continue; // not a match

                var creditType = Convert.ToString(dr2["Type.Name"]);
                var qty = (decimal) dr2["Quantity"];
                sbCredits.AppendFormat("{0:N1} ({1}),", qty, creditType);

                decimal totalForCreditType;
                if (!creditDictionary.TryGetValue(creditType, out totalForCreditType))
                    totalForCreditType = 0;

                totalForCreditType += qty;
                creditDictionary[creditType] = totalForCreditType;
            }

            if (sbCredits.Length == 0)
                 dr["Credits"]  = "No credits granted.";
            else
                 dr["Credits"]  = sbCredits.ToString().Trim().TrimEnd(',').Replace(",","<BR/>");
           
        }

        rgMainDataGrid.DataSource = dt;

        // now the totals
        rptTotals.DataSource = creditDictionary;
        rptTotals.DataBind();
        lGrantTotal.Text = creditDictionary.Sum(x => x.Value).ToString("N1");   // grand total
    }
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        rgMainDataGrid.Rebind();
    }

    protected void rgMainGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        DataRowView dr = (DataRowView)e.Item.DataItem;

        if ( dr != null && !(bool)dr["Verified"])
        {
            e.Item.ForeColor = Color.Gray;
            e.Item.Font.Italic = true;
        }
    }
}