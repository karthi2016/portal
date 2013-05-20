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

public partial class financial_RectifySuspendedBillingSchedule : PortalPage
{

    protected DataRow targetSchedule;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        if (ContextID == null)
            GoToMissingRecordPage();

        Search s = new Search(msBillingSchedule.CLASS_NAME);
        s.AddCriteria(Expr.Equals("ID", ContextID));
        s.AddOutputColumn("Order.LocalID");
        s.AddOutputColumn("Order.Date");
        s.AddOutputColumn("Order.Total");
        s.AddOutputColumn("Order");
        s.AddOutputColumn("Order.FutureBillingAmount");

        var sr = ExecuteSearch(s, 0, 1);
        if (sr.TotalRowCount == 0)
            GoToMissingRecordPage();

        targetSchedule = sr.Table.Rows[0];



    }

    protected override void InitializePage()
    {
        base.InitializePage();

        hlOrderNo.Text = GetSearchResult(targetSchedule, "Order.LocalID", null);
        hlOrderNo.NavigateUrl += targetSchedule["Order"];
        lblOrderDate.Text = GetSearchResult(targetSchedule, "Order.Date", "d");
        lblOrderTotal.Text = GetSearchResult(targetSchedule, "Order.Total", "C");
        lblAmountRemaining.Text = GetSearchResult(targetSchedule, "Order.FutureBillingAmount", "C");
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("ViewOrder.aspx?contextID=" + targetSchedule["Order"]);
    }
    protected void btnUpdatePaymentInfo_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        string orderID = Convert.ToString( targetSchedule["Order"] );
        string orderLocalID = GetSearchResult(targetSchedule, "Order.LocalID", null);
        using (var api = GetServiceAPIProxy())
        {
            api.UpdateOrderBillingInfo(orderID, tbCreditCardNumber.Text, null, myExpiration.Date, acBillingAddress.Address);
            QueueBannerMessage(string.Format("Order #{0} updated successfully.", orderLocalID));
            
        }
        Response.Redirect("/financial/ViewOrder.aspx?contextID=" + orderID);
    }
}