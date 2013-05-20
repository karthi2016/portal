using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class orders_CrossSellItems : PortalPage 
{

    #region Fields

    private msOrder targetOrder;
    private bool isTransient;
    private PreProcessedOrderPacket preProcessedOrderPacket;


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

        if ( string.Equals( Request.QueryString["useTransient"] ,"true", StringComparison.CurrentCultureIgnoreCase  ))
        {
            targetOrder = MultiStepWizards.PlaceAnOrder.TransientShoppingCart;
            isTransient = true;
        }
        else
            targetOrder = MultiStepWizards.PlaceAnOrder.ShoppingCart;

        if (targetOrder == null)
        {
            QueueBannerError("Unable to checkout without an active shopping cart.");
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

        // let's preprocess and figure out whether we need shipping information
        using (var api = GetConciegeAPIProxy())
        {
            preProcessedOrderPacket = api.PreProcessOrder(targetOrder).ResultValue;
        }

        if (preProcessedOrderPacket.CrossSellCandidates == null || preProcessedOrderPacket.CrossSellCandidates.Count  == 0)
            GoTo("EnterShippingInformation.aspx?useTransient=" + isTransient);

        rptItems.DataSource = preProcessedOrderPacket.CrossSellCandidates;
        rptItems.DataBind();

        List<msOrderLineItem> csi = MultiStepWizards.PlaceAnOrder.CrossSellItems;
        if (csi != null &&
             csi.Count > 0)
        {
        
            pnlItems.Visible = true;
            gvShoppingCart.DataSource = csi;
            gvShoppingCart.DataBind();
        }
    }


    #endregion

     
    protected void btnContinue_Click(object sender, EventArgs e)
    {
        GoTo("EnterShippingInformation.aspx?useTransient=" + isTransient);
    }

    protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
                Image imgProduct = (Image)e.Item.FindControl("imgProduct");
                TextBox tbQuantity = (TextBox)e.Item.FindControl("tbQuantity");
                Literal lProductName = (Literal)e.Item.FindControl("lProductName");
                Button btnAddToCart = (Button)e.Item.FindControl("btnAddToCart");
                var lblProductPrice = (Label)e.Item.FindControl("lblProductPrice");
                HiddenField hfProductID = (HiddenField)e.Item.FindControl("hfProductID");
                HiddenField hfPrice = (HiddenField)e.Item.FindControl("hfPrice");

                if (pi.Image != null)
                {
                    imgProduct.Visible = true;
                    imgProduct.ImageUrl = GetImageUrl(pi.Image);
                }

                hfProductID.Value = pi.ProductID;
                hfPrice.Value = pi.Price.ToString();

                lProductName.Text = pi.ProductName;
                lblProductPrice.Text = pi.DisplayPriceAs ?? pi.Price.ToString("C");

                btnAddToCart.CommandArgument = e.Item.ItemIndex.ToString();
                break;
        }
    }

    protected void gvShoppingCart_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        msOrderLineItem li = (msOrderLineItem)e.Row.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                Label lblProductName = (Label)e.Row.FindControl("lblProductName");
                using (var api = GetServiceAPIProxy())
                    lblProductName.Text = api.GetName(li.Product).ResultValue;


                break;
        }
    }

    protected void gvShoppingCart_OnRowCommand(object sender, GridViewCommandEventArgs e)
    {
        var csi = MultiStepWizards.PlaceAnOrder.CrossSellItems;
        int index = int.Parse( e.CommandArgument.ToString()  );
        if (csi == null || csi.Count <= index) return;
        csi.RemoveAt(index);

        QueueBannerMessage("Item has been removed successfully.");
        Refresh();
    }

    protected void rptItems_Command(object source, RepeaterCommandEventArgs e)
    {
        int index = int.Parse((string) e.CommandArgument);

        var ri = rptItems.Items[index];
        TextBox tbQuantity = (TextBox) ri.FindControl("tbQuantity");
        HiddenField hfProductID = (HiddenField) ri.FindControl("hfProductID");
            HiddenField hfPrice = (HiddenField) ri.FindControl("hfPrice");


        msOrderLineItem li = new msOrderLineItem();
        li.Product = hfProductID.Value;

        decimal qty;
        if (!decimal.TryParse(tbQuantity.Text, out qty))
            qty = 1;
        li.Quantity = qty;
        li.UnitPrice = decimal.Parse(hfPrice.Value);
        li.Total = qty * li.UnitPrice;

       
        if (MultiStepWizards.PlaceAnOrder.CrossSellItems == null)
            MultiStepWizards.PlaceAnOrder.CrossSellItems = new List<msOrderLineItem>();
        MultiStepWizards.PlaceAnOrder.CrossSellItems.Add(li);

        QueueBannerMessage("The items has been successfully added to your order.");
        Refresh();
    }
}