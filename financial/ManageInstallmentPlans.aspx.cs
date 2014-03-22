using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using Telerik.Web.UI;

public partial class financial_ManageInstallmentPlans : PortalPage 
{
 

    protected void rgMainDataGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        Search s = new Search("BillingSchedule");
        s.AddOutputColumn("Order.CreatedDate");
        s.AddOutputColumn("Order.Name");
        s.AddOutputColumn("Product.Name");
        s.AddOutputColumn("FutureBillingAmount");
        s.AddOutputColumn("PastBillingAmount");
        s.AddCriteria(Expr.Equals("Order.BillTo", ConciergeAPI.CurrentEntity.ID));

        //string msql = string.Format(
          //  "select Product.Name, [Order.Name] from BillingSchedule where Order.BillTo.ID='{0}'",
            // ConciergeAPI.CurrentEntity.ID) ;

        using (var api = GetServiceAPIProxy())
        {
            var result = api.ExecuteSearch(s, 0, null).ResultValue;

            if (result.Table != null)
                rgMainDataGrid.DataSource = result.Table.DefaultView;


            rgMainDataGrid.VirtualItemCount = result.TotalRowCount;

            lNoIntallmentPlans.Visible = result.TotalRowCount == 0;
        }
        
    }
}