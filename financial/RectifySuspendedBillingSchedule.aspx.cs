using System;
using System.Data;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class financial_RectifySuspendedBillingSchedule : CreditCardLogic
{

    protected DataRow targetSchedule;

    protected msOrder targetOrder;

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
        s.AddOutputColumn("Order.BillTo");
        s.AddOutputColumn("Order.FutureBillingAmount");

        var sr = APIExtensions.GetSearchResult(s, 0, 1);
        if (sr.TotalRowCount == 0)
            GoToMissingRecordPage();



        targetSchedule = sr.Table.Rows[0];

        if (Convert.ToString(targetSchedule["Order.BillTo"]) != ConciergeAPI.CurrentEntity.ID)
            throw new ApplicationException("Access denied");


        targetOrder = LoadObjectFromAPI<msOrder>(Convert.ToString(targetSchedule["Order"]));
        if (targetOrder == null)
            GoToMissingRecordPage();

        hfOrderBillToId.Value = ConciergeAPI.CurrentEntity.ID;

        dvPriorityData.InnerHtml = GetPriorityPaymentsConfig(hfOrderBillToId.Value);

        using (var api = GetServiceAPIProxy())
        {
            var methods = api.DetermineAllowableOrderPaymentMethods(targetOrder).ResultValue;

            // some payments NEVER make sense in this context
            methods.AllowBillMeLater = false;

            BillingInfoWidget.AllowableMethods = methods;
        }



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
        Response.Redirect("ViewInstallmentPlan.aspx?contextID=" + ContextID);
    }
    protected void btnUpdatePaymentInfo_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        string orderID = Convert.ToString(targetSchedule["Order"]);
        string orderLocalID = GetSearchResult(targetSchedule, "Order.LocalID", null);

        var paymentManifest = BillingInfoWidget.GetPaymentInfo();

        using (var api = GetServiceAPIProxy())
        {
            api.UpdateBillingInfo(orderID, paymentManifest);
            QueueBannerMessage(string.Format("Order #{0} updated successfully.", orderLocalID));

        }
        Response.Redirect("/financial/ViewInstallmentPlan.aspx?contextID=" + ContextID);
    }
}