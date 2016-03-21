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

public partial class financial_ViewInvoice : PortalPage
{
    #region Fields

    protected DataRow targetInvoice;
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

        var billTo = GetSearchResult(targetInvoice, "BillTo", null);
        if (billTo == ConciergeAPI.CurrentEntity.ID )
            return true;

        if (targetChapter != null)
            return canViewAccountHistory(targetChapter.Leaders);

        if (targetSection != null)
            return canViewAccountHistory(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return canViewAccountHistory(targetOrganizationalLayer.Leaders);

        return ConciergeAPI.AccessibleEntities.Exists(x => x.ID == billTo);
    }

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        if (ContextID == null)
            GoToMissingRecordPage();

        Search s = new Search(msInvoice.CLASS_NAME);
        s.AddCriteria(Expr.Equals("ID", ContextID));
        s.AddOutputColumn("LocalID");
        s.AddOutputColumn("Date");
        s.AddOutputColumn("Total");
        s.AddOutputColumn("BalanceDue");
        s.AddOutputColumn("AmountPaid");
        s.AddOutputColumn("Order");
        s.AddOutputColumn("Order.Name");
        s.AddOutputColumn("BillTo.Name");
        s.AddOutputColumn("BillTo");
        s.AddOutputColumn("BillingAddress");
        s.AddOutputColumn("PurchaseOrderNumber");
        
        var sr = APIExtensions.GetSearchResult(s, 0, 1);
        if (sr.TotalRowCount == 0)
            GoToMissingRecordPage();

        targetInvoice = sr.Table.Rows[0];


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

        var sInvoiceItems = generateInvoiceItemsSearch();
     
        var sPayments = generatePaymentsSearch();

        var searchesToRun = new List<Search> { sInvoiceItems, sPayments };
        var searchResults = APIExtensions.GetMultipleSearchResults(searchesToRun, 0, null);

        gvInvoiceItems.DataSource = searchResults[0].Table;
        gvInvoiceItems.DataBind();
 
        gvPayments.DataSource = searchResults[1].Table;
        gvPayments.DataBind();

        PageTitleExtenstion.Text = GetSearchResult(targetInvoice, "LocalID", null);

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

    private Search generatePaymentsSearch()
    {
        Search s = new Search(msPaymentLineItem.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Invoice", ContextID));
        
        s.AddOutputColumn("Payment.Date");
        s.AddOutputColumn("Payment.Owner.Name");
        s.AddOutputColumn(msPaymentLineItem.FIELDS.Total);
        s.AddOutputColumn("Payment");
        s.AddOutputColumn("Payment.Name");
        
        s.AddSortColumn("Payment.Date", true);


        return s;
    }

    

    private Search generateInvoiceItemsSearch()
    {
        Search s = new Search( msInvoiceLineItem.CLASS_NAME );

        s.AddCriteria(Expr.Equals("Invoice", ContextID));

        s.AddOutputColumn("Product.Name");
        s.AddOutputColumn("Description");
        s.AddOutputColumn("Quantity");
        s.AddOutputColumn("UnitPrice");
        s.AddOutputColumn("Total");
      

        s.AddSortColumn("ListIndex");
        return s;
    }

    #endregion

    #region Event Handlers

    protected void btnAccountHistory_Click(object sender, EventArgs e)
    {
        string nextUrl = string.Format("/financial/AccountHistory.aspx?contextID={0}",
                                       GetSearchResult(targetInvoice, "BillTo", null));
        if (!string.IsNullOrWhiteSpace(LeaderOfId))
            nextUrl = string.Format("{0}&leaderOfID={1}", nextUrl, LeaderOfId);

        Response.Redirect(nextUrl);
    }

    protected void btnGoHome_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    #endregion
}