using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class careercenter_SelectCareerCenterProduct : PortalPage
{
    #region Fields

    protected msOrder targetOrder;
    protected List<ProductInfo> careerCenterProducts;
    protected DataTable dtCareerCenterProducts;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get { return PortalConfiguration.Current.JobPostingAccessMode == JobPostingAccess.PublicAccess; }
    }

    /// <summary>
    /// Used by the Job Posting Creation page to indicate that the Job Posting details have already been specified
    /// and the Job Posting is waiting in the MultiStepWizards.PlaceAnOrder.ObjectsToSave collection.  This setting
    /// will determine if the list should show products with multiple job postings or not and determine where
    /// to redirect when done with the order flow.
    /// </summary>
    public bool TransientJobPosting
    {
        get
        {
            bool result;
            if (!bool.TryParse(Request.QueryString["transientJobPosting"], out result))
                return false;

            return result;
        }
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
        if (ConciergeAPI.CurrentEntity == null)
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=JobPosting");

        base.InitializeTargetObject();

        targetOrder = new msOrder();
        targetOrder.BillTo = targetOrder.ShipTo = ConciergeAPI.CurrentEntity.ID;
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
        setFeesAndContinueIfApplicable();

        using (var api = GetServiceAPIProxy())
            numberOfPostingsAvailable = Convert.ToInt32(api.CheckEntitlement(msJobPostingEntitlement.CLASS_NAME, ConciergeAPI.CurrentEntity.ID, null).ResultValue.Quantity);
    }

    #endregion

    #region Methods

    protected int numberOfPostingsAvailable = 0;

    protected void loadDataFromConcierge()
    {
        Search sCareerCenterProducts = new Search(msCareerCenterProduct.CLASS_NAME);
        sCareerCenterProducts.AddOutputColumn("ID");
        sCareerCenterProducts.AddOutputColumn("Name");
        sCareerCenterProducts.AddCriteria(Expr.Equals("SellOnline", true));

        //If the job posting details have already been specified in the create job posting page then
        //this is part of a larger workflow and we can only process career center products that specify one job posting
        if (TransientJobPosting)
            sCareerCenterProducts.AddCriteria(Expr.Equals("NumberOfJobPostings", 1));

        //Create sell from group
        SearchOperationGroup sellFromGroup = new SearchOperationGroup
                                                 {
                                                     FieldName = "SellFrom",
                                                     GroupType = SearchOperationGroupType.Or
                                                 };
        sellFromGroup.Criteria.Add(Expr.Equals("SellFrom", null));
        sellFromGroup.Criteria.Add(Expr.IsLessThanOrEqual("SellFrom", DateTime.Now));
        sCareerCenterProducts.AddCriteria(sellFromGroup);

        //Create sell until group
        SearchOperationGroup sellUntilGroup = new SearchOperationGroup
        {
            FieldName = "SellUntil",
            GroupType = SearchOperationGroupType.Or
        };
        sellUntilGroup.Criteria.Add(Expr.Equals("SellUntil", null));
        sellUntilGroup.Criteria.Add(Expr.IsGreaterThanOrEqualTo("SellUntil", DateTime.Now));
        sCareerCenterProducts.AddCriteria(sellUntilGroup);

        sCareerCenterProducts.AddSortColumn("Name");

        SearchResult srCareerCenterProducts = ExecuteSearch(sCareerCenterProducts, 0, null);
        dtCareerCenterProducts = srCareerCenterProducts.Table;

        //Now describe the products for the logged-in entitiy - this will apply any membership discounts
        List<string> productIDs = (from DataRow productRow in dtCareerCenterProducts.Rows select productRow["ID"].ToString()).ToList();

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            careerCenterProducts = proxy.DescribeProducts(ConciergeAPI.CurrentEntity.ID, productIDs).ResultValue;
        }
    }

    protected void setFeesAndContinueIfApplicable()
    {
        foreach (var productInfo in careerCenterProducts)
        {
            productInfo.ProductName = string.Format("{0} - <font color=green>{1}</font>", productInfo.ProductName,
                                            productInfo.DisplayPriceAs ?? productInfo.Price.ToString("C"));
            if (productInfo.IsSoldOut)
                productInfo.ProductName += " SOLD OUT";

            if (!productInfo.IsEligible)
                productInfo.ProductName += " (ineligible)";
        }

        switch (careerCenterProducts.Count)
        {
            case 0:
                lblNoCareerCenterProducts.Visible = true;
                btnContinue.Enabled = false;
                break;
            case 1: //If there's only one product then auto-select it
                SetCareerCenterProduct(careerCenterProducts[0].ProductID);
                //if (ConciergeAPI.CurrentEntity.JobPostingsAvailable == 0) // but only move if we don't need to tell them about the existing job postings
                //    MoveToOrderProcessing();
                break;
        }

        rblCareerCenterProducts.DataSource = careerCenterProducts;
        rblCareerCenterProducts.DataBind();
    }

    protected void MoveToOrderProcessing()
    {

        MultiStepWizards.PlaceAnOrder.ContinueShoppingUrl = "~/careercenter/SelectCareerCenterProduct.aspx";

        //If the job posting details have already been specified then this is part of a larger workflow
        //and just complete the order as normal (the job posting will be saved since it's in the PlaceAnOrder.ObjectsToBeSaved collection)
        //Otherwise this is the start of the workflow and after the order direct the user to the page to enter job posting details.
        MultiStepWizards.PlaceAnOrder.OrderCompleteUrl = TransientJobPosting
                                                             ? "~/careercenter/ConfirmJobPosting.aspx?post=true"
                                                             : "~/careercenter/CreateEditJobPosting.aspx";

        MultiStepWizards.PlaceAnOrder.ReloadEntityOnOrderComplete = true;

        OrderPayload pl = new OrderPayload();

                var targetJobPosting = MultiStepWizards.PostAJob.JobPosting;


        pl.EntitlementAdjustments = new List<OrderPayloadEntitlementAdjustments>();
        pl.EntitlementAdjustments.Add(new OrderPayloadEntitlementAdjustments
            {
                EntityID = targetOrder.BillTo,
                AmountToAdjust = -1,
                EntitlementType = msJobPostingEntitlement.CLASS_NAME,
                Memo = "Job posting '" + targetJobPosting.Name + "'" 

            });

        targetJobPosting.Order = "{OrderID}";
        pl.ObjectsToSave = new List<MemberSuiteObject> { new MemberSuiteObject( targetJobPosting )} ;

        
        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(targetOrder, pl );

    }

    protected void SetCareerCenterProduct(string careerCenterProductID)
    {
        targetOrder.LineItems = new List<msOrderLineItem> { new msOrderLineItem { Quantity = 1, Product = careerCenterProductID } };
    }

    #endregion

    #region Event Handlers

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        SetCareerCenterProduct(rblCareerCenterProducts.SelectedValue);
        MoveToOrderProcessing();
    }

    #endregion
}