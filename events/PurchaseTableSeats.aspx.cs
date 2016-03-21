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

public partial class events_PurchaseTableSeats : PortalPage 
{
    #region Initialization

    protected msEvent targetEvent;
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

        targetEvent = LoadObjectFromAPI<msEvent>(ContextID);
        if (targetEvent == null)
        {
            GoToMissingRecordPage();
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

        using (var api = GetServiceAPIProxy())
        {
            // let's get all of the products
            Search s = new Search(msEventTableFee.CLASS_NAME);
            s.AddCriteria(Expr.Equals(msEventTableFee.FIELDS.Event, targetEvent.ID));
            s.AddCriteria(Expr.Equals("IsActive", true));
            s.AddSortColumn("DisplayOrder");
            s.AddSortColumn("Name");

            var results = api.GetSearchResult(s, 0, null);

            List<string> productIDs = new List<string>();
            foreach (DataRow dr in results.Table.Rows)
                productIDs.Add(Convert.ToString(dr["ID"]));

            var productInfos = api.DescribeProducts(ConciergeAPI.CurrentEntity.ID, productIDs).ResultValue;
            rpTableProducts.DataSource =productInfos;
            rpTableProducts.DataBind();
        }

    }

    
    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

        if (targetEvent.RegistrationMode != EventRegistrationMode.Tabled)
            return false;


        return true;
    }


    #endregion

    protected void rpTableProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ProductInfo pi = (ProductInfo)e.Item.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;

            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
                goto case ListItemType.Item;

            case ListItemType.Item:
                TextBox tbQuantity = (TextBox)e.Item.FindControl("tbQuantity");
                CompareValidator cvQuantity = (CompareValidator)e.Item.FindControl("cvQuantity");
                Label lblProductName = (Label)e.Item.FindControl("lblProductName");
                Label lblProductPrice = (Label)e.Item.FindControl("lblProductPrice");
                HiddenField hfProductID = (HiddenField)e.Item.FindControl("hfProductID");

                hfProductID.Value = pi.ProductID;


                cvQuantity.ErrorMessage = string.Format("You have entered an invalid donation amount for {0}", pi.ProductName);
                lblProductName.Text = pi.ProductName;
                lblProductPrice.Text = pi.DisplayPriceAs ?? pi.Price.ToString("C");

                break;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewEvent.aspx?contextID=" + targetEvent.ID);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        // set our transient shopping cart
        MultiStepWizards.PlaceAnOrder.OrderCompleteUrl = string.Format("~/events/ViewEvent.aspx?contextID={0}&complete=true", targetEvent.ID );

        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(unbindObjectsFromPage());


    }

    private msOrder unbindObjectsFromPage()
    {
        msOrder mso = new msOrder();
        mso.BillTo = mso.ShipTo = ConciergeAPI.CurrentEntity.ID;

        foreach (RepeaterItem ri in rpTableProducts.Items)
        {
            TextBox tbQuantity = (TextBox)ri.FindControl("tbQuantity");
            HiddenField hfProductID = (HiddenField)ri.FindControl("hfProductID");

            msOrderLineItem li = new msOrderLineItem();
            li.Quantity = int.Parse(tbQuantity.Text);
            if (li.Quantity <= 0)
                continue;   // don't add

            li.Product = hfProductID.Value;
            
            mso.LineItems.Add(li);

        }

        return mso;
    }

}