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
using Telerik.Web.UI;

public partial class financial_ViewInstallmentPlan : PortalPage
{
    protected msBillingSchedule targetSchedule;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetSchedule = LoadObjectFromAPI<msBillingSchedule>(ContextID);

        if (targetSchedule == null)
            GoToMissingRecordPage();

    }

     
    protected override void InitializePage()
    {
        base.InitializePage();


        Search s = new Search(msBillingSchedule.CLASS_NAME);
        s.AddCriteria(Expr.Equals("ID", ContextID));
        s.AddOutputColumn("Order.LocalID");
        s.AddOutputColumn("Order.Date");
        s.AddOutputColumn("Order.Total");
        s.AddOutputColumn("Order");
        s.AddOutputColumn("Order.BillTo");
        s.AddOutputColumn("Order.SavedPaymentMethod.Name");
        s.AddOutputColumn("FutureBillingAmount");
        s.AddOutputColumn("PastBillingAmount");
        s.AddOutputColumn("Status");

        DataRow dr = ExecuteSearch(s, 0, null).Table.Rows[0];

        if (Convert.ToString(dr["Order.BillTo"]) != ConciergeAPI.CurrentEntity.ID)
            throw new ApplicationException("Access denied");
            
        hlOrderNo.Text = GetSearchResult(dr, "Order.LocalID", null);
        hlOrderNo.NavigateUrl += targetSchedule["Order"];
        lblOrderDate.Text = GetSearchResult(dr, "Order.Date", "d");
        lblOrderTotal.Text = GetSearchResult(dr, "Order.Total", "C");
        lblAmountRemaining.Text = GetSearchResult(dr, "FutureBillingAmount", "C");
        lblAmountAlreadyBilled.Text = GetSearchResult(dr, "PastBillingAmount", "C");
        lblStatus.Text = GetSearchResult(dr, "Status");
        hlUpdateBilling.NavigateUrl += "?contextID=" + targetSchedule.ID;
        hlUpdateBillingInfo2.NavigateUrl = hlUpdateBilling.NavigateUrl;

        string info = GetSearchResult(dr, "Order.SavedPaymentMethod.Name");
        if (info != null)
            lblPaymentInfo.Text = info;
    }

    protected void rgBillingHistory_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        Search s = new Search("BillingScheduleEntry");

        s.AddCriteria(Expr.Equals("Schedule", ContextID));

        s.AddOutputColumn("Status");
        s.AddOutputColumn("Invoice");
        s.AddOutputColumn("Invoice.Name");
        s.AddOutputColumn("Payment");
        s.AddOutputColumn("Payment.Name");
        s.AddOutputColumn("Amount");
        s.AddOutputColumn("DateProcessed");
        s.AddOutputColumn("Date");
        s.AddOutputColumn("Message");

        s.AddSortColumn("Date");

        using (var api = GetServiceAPIProxy())
        {
            var result = api.ExecuteSearch(s, 0, null).ResultValue;

            if (result.Table != null)
                rgBillingHistory.DataSource = result.Table.DefaultView;


            rgBillingHistory.VirtualItemCount = result.TotalRowCount;

            lNoHistory.Visible = result.TotalRowCount == 0;
        }

    }
}