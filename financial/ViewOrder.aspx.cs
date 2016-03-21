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

public partial class financial_ViewOrder : PortalPage
{
    #region Fields

    protected DataRow targetOrder;
    protected msEntity targetEntity;
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    #endregion

    #region Properties

    protected string LeaderOfId
    {
        get { return Request.QueryString["leaderofid"]; }
    }

    #endregion

    #region Initialization

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        var billTo = GetSearchResult(targetOrder, "BillTo", null);
        var shipTo = GetSearchResult(targetOrder, "ShipTo", null);

        if (billTo == ConciergeAPI.CurrentEntity.ID ||
            shipTo == ConciergeAPI.CurrentEntity.ID)
            return true;

        if (targetChapter != null)
            return canViewAccountHistory(targetChapter.Leaders);

        if (targetSection != null)
            return canViewAccountHistory(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return canViewAccountHistory(targetOrganizationalLayer.Leaders);

        return ConciergeAPI.AccessibleEntities.Exists( x=>x.ID == billTo ) ||
            ConciergeAPI.AccessibleEntities.Exists( x=>x.ID == shipTo ) ;
    }

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        if (ContextID == null)
            GoToMissingRecordPage();

        Search s = new Search(msOrder.CLASS_NAME);
        s.AddCriteria(Expr.Equals("ID", ContextID));
        s.AddOutputColumn("LocalID");
        s.AddOutputColumn("Date");
        s.AddOutputColumn("Total");
        
        s.AddOutputColumn("Status");
        s.AddOutputColumn("Order.FutureBillingAmount");
        s.AddOutputColumn("BalanceDue");
        s.AddOutputColumn("ShipDate");
        s.AddOutputColumn("CustomerNotes");
        s.AddOutputColumn("TrackingNumber");
        s.AddOutputColumn("AmountPaid");
        s.AddOutputColumn("BillTo.Name");
        s.AddOutputColumn("BillTo");
        s.AddOutputColumn("ShipTo.Name");
        s.AddOutputColumn("ShipTo");
        s.AddOutputColumn("BillingAddress");
        s.AddOutputColumn("ShippingAddress");
        s.AddOutputColumn("PurchaseOrderNumber");

        var sr = APIExtensions.GetSearchResult(s, 0, 1);
        if (sr.TotalRowCount == 0)
            GoToMissingRecordPage();

        targetOrder = sr.Table.Rows[0];

        if (string.IsNullOrWhiteSpace(LeaderOfId))
            return;

        var leaderOf = APIExtensions.LoadObjectFromAPI(LeaderOfId);
        switch (leaderOf.ClassType)
        {
            case msChapter.CLASS_NAME:
                targetChapter = leaderOf.ConvertTo<msChapter>();
                break;
            case msSection.CLASS_NAME:
                targetSection = leaderOf.ConvertTo<msSection>();
                break;
            case msOrganizationalLayer.CLASS_NAME:
                targetOrganizationalLayer = leaderOf.ConvertTo<msOrganizationalLayer>();
                break;
            default:
                QueueBannerError(string.Format("Invalid leader object type specified '{0}'",
                                               leaderOf.ClassType));
                GoHome();
                return;
        }
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        var sOrderItems = generateOrderItemsSearch();
        var sInvoices = generateInvoicesSearch();
        var sPayments = generatePaymentsSearch();
        var sInstallmentsSearch = generateInstallmentsSearch();

        var searchesToRun = new List<Search> { sOrderItems, sInvoices, sPayments, sInstallmentsSearch  };
        var searchResults = APIExtensions.GetMultipleSearchResults(searchesToRun, 0, null);

        gvOrderItems.DataSource = searchResults[0].Table;
        gvOrderItems.DataBind();

        gvInvoices.DataSource = searchResults[1].Table;
        gvInvoices.DataBind();

        gvPayments.DataSource = searchResults[2].Table;
        gvPayments.DataBind();

        //if (searchResults[3].TotalRowCount > 0)
        //{
        //    pnlInstallments.Visible = true;
        //    gvInstallments.DataSource = searchResults[3].Table;
        //    gvInstallments.DataBind();

        //    if ( (string) targetOrder["BillingSchedule.Status"] == "Completed")
        //        btnUpdateCreditCardInfo.Visible = false;
        //}

        if (string.IsNullOrWhiteSpace(GetSearchResult(targetOrder, "CustomerNotes")))
            divNotes.Visible = false;

        PageTitleExtenstion.Text = GetSearchResult(targetOrder, "LocalID", null);

    }

    #endregion

    #region Methods

    protected bool canViewAccountHistory(List<msMembershipLeader> leaders)
    {
        if (leaders == null)
            // no leaders to speak of
            return false;

        var leader = leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanViewAccountHistory;
    }

    private Search generateInstallmentsSearch()
    {
        Search s = new Search(msBillingScheduleEntry.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Schedule.Order.ID", ContextID));

        s.AddOutputColumn("Date");
        s.AddOutputColumn("Product.Name");
        s.AddOutputColumn("Status");
        s.AddOutputColumn("Amount");

        s.AddSortColumn("Date");

        return s;
    }

    private Search generatePaymentsSearch()
    {
        Search s = new Search(msPaymentLineItem.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Invoice.Order", ContextID));
        
        s.AddOutputColumn("Payment.Date");
        s.AddOutputColumn("Payment.Owner.Name");
        s.AddOutputColumn(msPaymentLineItem.FIELDS.Total);
        s.AddOutputColumn("Payment");
        s.AddOutputColumn("Payment.Name");
        
        s.AddSortColumn("Payment.Date", true);


        return s;
    }

    private Search generateInvoicesSearch()
    {
        Search s = new Search(msInvoice.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Order", ContextID));
        s.AddOutputColumn("Date");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("Total");
        s.AddOutputColumn("AmountPaid");
        s.AddOutputColumn("BalanceDue");
        
        s.AddSortColumn("Date", true );

        return s;
    }

    private Search generateOrderItemsSearch()
    {
        Search s = new Search( msOrderLineItem.CLASS_NAME );

        s.AddCriteria(Expr.Equals("Order", ContextID));

        s.AddOutputColumn("Product.Name");
        s.AddOutputColumn("Description");
        s.AddOutputColumn("Quantity");
        s.AddOutputColumn("UnitPrice");
        s.AddOutputColumn("Total");
        s.AddOutputColumn("Status");

        s.AddSortColumn("ListIndex");
        return s;
    }

    #endregion

    #region Event Handlers

    protected void btnAccountHistory_Click(object sender, EventArgs e)
    {
        string nextUrl = GetSearchResult(targetOrder, "ShipTo", null) == ConciergeAPI.CurrentEntity.ID
                             ? string.Format("/financial/AccountHistory.aspx?contextID={0}",
                                                       GetSearchResult(targetOrder, "ShipTo", null))
                             : string.Format("/financial/AccountHistory.aspx?contextID={0}",
                                                       GetSearchResult(targetOrder, "BillTo", null));
        if (!string.IsNullOrWhiteSpace(LeaderOfId))
            nextUrl = string.Format("{0}&leaderOfID={1}", nextUrl, LeaderOfId);

        Response.Redirect(nextUrl);
    }

    protected void btnGoHome_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void btnUpdateCreditCard_Click(object sender, EventArgs e)
    {
        Response.Redirect("RectifySuspendedBillingSchedule.aspx?contextID=" + targetOrder["BillingSchedule"]);
    }

    #endregion

    protected void rgInstallments_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        Search s = new Search("BillingSchedule");
        s.AddOutputColumn("Order.CreatedDate");
        s.AddOutputColumn("Order.Name");
        s.AddOutputColumn("Product.Name");
        s.AddOutputColumn("FutureBillingAmount");
        s.AddOutputColumn("PastBillingAmount");
        s.AddCriteria(Expr.Equals("Order", ContextID ));

        //string msql = string.Format(
        //  "select Product.Name, [Order.Name] from BillingSchedule where Order.BillTo.ID='{0}'",
        // ConciergeAPI.CurrentEntity.ID) ;

        using (var api = GetServiceAPIProxy())
        {
            var result = api.GetSearchResult(s, 0, null);

            if (result.Table != null)
                rgInstallments.DataSource = result.Table.DefaultView;

            rgInstallments.VirtualItemCount = result.TotalRowCount;

            rgInstallments.Visible = result.TotalRowCount > 0;
            lNoIntallmentPlans.Visible = result.TotalRowCount == 0;
        }
    }
}