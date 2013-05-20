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

public partial class financial_ViewPayment : PortalPage
{
    #region Fields
    
    protected DataRow targetPayment;
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

        var owner = GetSearchResult(targetPayment, "Owner", null);
        if (owner == ConciergeAPI.CurrentEntity.ID )
            return true;

        if (targetChapter != null)
            return canViewAccountHistory(targetChapter.Leaders);

        if (targetSection != null)
            return canViewAccountHistory(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return canViewAccountHistory(targetOrganizationalLayer.Leaders);

        return ConciergeAPI.AccessibleEntities.Exists(x => x.ID == owner);
    }

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        if (ContextID == null)
            GoToMissingRecordPage();

        Search s = new Search(msPayment.CLASS_NAME);
        s.AddCriteria(Expr.Equals("ID", ContextID));
        s.AddOutputColumn("LocalID");
        s.AddOutputColumn("Date");
        s.AddOutputColumn("Total");
        s.AddOutputColumn("Owner.Name");
        s.AddOutputColumn("Owner");
        s.AddOutputColumn("AmountRefunded");
        s.AddOutputColumn("BillingAddress");
        
        
        var sr = ExecuteSearch(s, 0, 1);
        if (sr.TotalRowCount == 0)
            GoToMissingRecordPage();

        targetPayment = sr.Table.Rows[0];

        if (string.IsNullOrWhiteSpace(LeaderOfId))
            return;

        MemberSuiteObject leaderOf = LoadObjectFromAPI(LeaderOfId);
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

        Search sPaymentItems = generatePaymentItemsSearch();
     
      
        List<Search> searchesToRun = new List<Search> { sPaymentItems };
        var searchResults = ExecuteSearches(searchesToRun, 0, null);

        gvPaymentItems.DataSource = searchResults[0].Table;
        gvPaymentItems.DataBind();
 

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

    private Search generatePaymentItemsSearch()
    {
        Search s = new Search( msPaymentLineItem.CLASS_NAME );

        s.AddCriteria(Expr.Equals("Payment", ContextID));

        s.AddOutputColumn("Type");
        s.AddOutputColumn("TransactionName");
        s.AddOutputColumn("Amount");
        s.AddOutputColumn("TransactionID");
        
      

        s.AddSortColumn("ListIndex");
        return s;
    }

    #endregion

    #region Event Handlers

    protected void btnAccountHistory_Click(object sender, EventArgs e)
    {
        string nextUrl = string.Format("/financial/AccountHistory.aspx?contextID={0}",
                                       GetSearchResult(targetPayment, "Owner", null));
        if (!string.IsNullOrWhiteSpace(LeaderOfId))
            nextUrl = string.Format("{0}&leaderOfID={1}", nextUrl, LeaderOfId);

        Response.Redirect(nextUrl);
    }

    protected void btnGoHome_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void gvPaymentItems_RowDataBound(object sender, GridViewRowEventArgs e)
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
                HyperLink hlView = (HyperLink)e.Row.FindControl("hlView");
                if ( (string) drv["Type"] == "Invoice")
                {
                    hlView.Visible = true;
                    hlView.NavigateUrl = "/financial/ViewInvoice.aspx?contextID=" + drv["TransactionID"];
                }
                else
                    hlView.Visible = false;



                break;
        }
    }

    #endregion
}