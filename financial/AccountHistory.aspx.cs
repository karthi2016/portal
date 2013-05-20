using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class financial_AccountHistory : PortalPage
{
    #region Fields

    protected DataView dvFinancialTransactions;
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

    /// <summary>
    /// Initializes the target object for the page
    /// </summary>
    /// <remarks>Many pages have "target" objects that the page operates on. For instance, when viewing
    /// an event, the target object is an event. When looking up a directory, that's the target
    /// object. This method is intended to be overriden to initialize the target object for
    /// each page that needs it.</remarks>
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetEntity = string.IsNullOrWhiteSpace(ContextID) ? ConciergeAPI.CurrentEntity : LoadObjectFromAPI<msEntity>(ContextID);

        if (targetEntity == null)
        {
            GoToMissingRecordPage();
            return;
        }

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

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();

        loadDataFromConcierge();

        if (dvFinancialTransactions.Count == 0)
            return;

        lblNoTransactions.Visible = false;

        gvTransactions.DataSource = dvFinancialTransactions;
        gvTransactions.DataBind();
    }

    /// <summary>
    /// Checks to make sure that this page is being access properly.
    /// </summary>
    /// <returns></returns>
    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (targetEntity.ID == ConciergeAPI.CurrentEntity.ID)
            return true;

        if (targetChapter != null)
            return canViewAccountHistory(targetChapter.Leaders);

        if (targetSection != null)
            return canViewAccountHistory(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return canViewAccountHistory(targetOrganizationalLayer.Leaders);

        return ConciergeAPI.AccessibleEntities.Exists(x => x.ID == targetEntity.ID);
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

    private void loadDataFromConcierge()
    {
        Search sFinancialTransactions = new Search { Type = "FinancialTransactions" };
        sFinancialTransactions.AddCriteria(Expr.Equals("Owner", targetEntity.ID));

        sFinancialTransactions.AddOutputColumn("TransactionType");
        sFinancialTransactions.AddOutputColumn("Date");
        sFinancialTransactions.AddOutputColumn("ID");
        sFinancialTransactions.AddOutputColumn("Name");
        sFinancialTransactions.AddOutputColumn("Memo");
        sFinancialTransactions.AddOutputColumn("Total");
        sFinancialTransactions.AddSortColumn("Date", true);

        SearchResult srFinancialTransactions = ExecuteSearch(sFinancialTransactions, 0, null);
        dvFinancialTransactions = new DataView(srFinancialTransactions.Table);
    }

    #endregion

    #region Event Handlers

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void gvTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
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
                string nextUrl;

                var transactionType = (string)drv["TransactionType"];
                var transactionID = Convert.ToString( drv["ID"] );
               
                if (transactionID.ToUpper().Contains("-00D7-"))   // it's a historical transaction
                {
                    hlView.Visible = false;
                    return;
                }

                switch (transactionType)
                {
                    case "Order":
                        nextUrl = "/financial/ViewOrder.aspx?contextID=" + drv["ID"];
                        if (!string.IsNullOrWhiteSpace(LeaderOfId))
                            nextUrl += string.Format("{0}&leaderOfID={1}", nextUrl, LeaderOfId);
                        hlView.NavigateUrl = nextUrl;
                        break;

                    case "Invoice":
                        nextUrl = "/financial/ViewInvoice.aspx?contextID=" + drv["ID"];
                        if (!string.IsNullOrWhiteSpace(LeaderOfId))
                            nextUrl += string.Format("{0}&leaderOfID={1}", nextUrl, LeaderOfId);
                        hlView.NavigateUrl = nextUrl;
                        break;

                    case "Payment":
                        nextUrl = "/financial/ViewPayment.aspx?contextID=" + drv["ID"];
                        if (!string.IsNullOrWhiteSpace(LeaderOfId))
                            nextUrl += string.Format("{0}&leaderOfID={1}", nextUrl, LeaderOfId);
                        hlView.NavigateUrl = nextUrl;
                        break;

                    default:
                        hlView.Visible = false;
                        break;
                }


                break;
        }
    }

    #endregion
}