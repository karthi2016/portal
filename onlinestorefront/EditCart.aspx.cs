using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class onlinestorefront_EditCart : PortalPage
{
    #region Fields

    private msOrder targetOrder;
    private PreProcessedOrderPacket preProcessedOrder;
    

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
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
        base.InitializeTargetObject();

        targetOrder = MultiStepWizards.PlaceAnOrder.ShoppingCart;

        if (targetOrder == null)
        {
            GoTo("~/onlinestorefront/BrowseMerchandise.aspx");
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

        //MS-1634
        if (CurrentEntity != null && MultiStepWizards.PlaceAnOrder.ShoppingCart != null && (MultiStepWizards.PlaceAnOrder.ShoppingCart.BillTo == null || MultiStepWizards.PlaceAnOrder.ShoppingCart.ShipTo == null))
            MultiStepWizards.PlaceAnOrder.ShoppingCart.BillTo = MultiStepWizards.PlaceAnOrder.ShoppingCart.ShipTo = CurrentEntity.ID;

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadDataFromConcierge(proxy);
        }
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge(IConciergeAPIService proxy)
    {
      
        preProcessedOrder = proxy.PreProcessOrder(targetOrder).ResultValue;

        if (targetOrder == null || targetOrder.LineItems == null ||
            targetOrder.LineItems.Count == 0)
        {
            lblShoppingCartEmpty.Visible = true;
            lblContinueShoppingInstructions.Visible = false;
            btnCheckout.Enabled = false;
            gvShoppingCart.Visible = false;
            return;
        }


        // this is just the shopping cart, so we don't want to display shipping/taxes yet (they should not exist at this point).  Remove those items from the order if they exist for some reason
        List<msOrderLineItem> itemsToDisplay = new List<msOrderLineItem>(preProcessedOrder.FinalizedOrder.ConvertTo<msOrder>().LineItems);
        itemsToDisplay.RemoveAll(x => x.Type == OrderLineItemType.Shipping || x.Type == OrderLineItemType.Taxes || x.Type == OrderLineItemType.Discount);

        gvShoppingCart.Visible = true;
        gvShoppingCart.DataSource = itemsToDisplay;
        gvShoppingCart.DataBind();
        lblShoppingCartEmpty.Visible = false;
        lblContinueShoppingInstructions.Visible = true;
    }

    #endregion

    #region Event Handlers

    protected void btnContinueShopping_Click(object sender, EventArgs e)
    {
        GoTo("~/onlinestorefront/BrowseMerchandise.aspx");
    }

    protected void btnCheckout_Click(object sender, EventArgs e)
    {
        MultiStepWizards.PlaceAnOrder.ContinueShoppingUrl = "~/onlinestorefront/BrowseMerchandise.aspx";


        //If there's nobody currently logged in go through the account creation process before the checkout
        if (ConciergeAPI.CurrentEntity == null)
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=Storefront");

        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(null);
    }

    private static msOrderLineItem GetLineItem(string lineItemId)
    {
        var lineItem = MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.SingleOrDefault(
            li => li.OrderLineItemID == lineItemId);

        return lineItem;
    }

    private static msOrderLineItem GetLineItem(Control gridRow)
    {
        var ctl = gridRow.FindControl("lblLineItemID") as HiddenField;

        return ctl == null ? null : GetLineItem(ctl.Value);
    }

    protected void gvShoppingCart_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        msOrderLineItem li = (msOrderLineItem)e.Row.DataItem;


        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                //Add the cart total to the footer
                if (e.Row.Cells.Count >= 4)
                {
                    e.Row.Cells[3].CssClass = "columnHeader";
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[3].Text = string.Format("Cart Total: {0}",
                                                        preProcessedOrder.FinalizedOrder.ConvertTo<msOrder>().LineItems.
                                                            Sum(
                                                                x => x.UnitPrice * x.Quantity).ToString("C"));
                }
                break;

            case DataControlRowType.DataRow:
                TextBox tbQuantity = (TextBox)e.Row.FindControl("tbQuantity");
                CompareValidator cvQuantity = (CompareValidator)e.Row.FindControl("cvQuantity");
                Label lblProductName = (Label)e.Row.FindControl("lblProductName");
                Label lblProductPrice = (Label)e.Row.FindControl("lblProductPrice");

                tbQuantity.Text = li.Quantity.ToString("F0");

                // MS-4788 We're using hidden field value to associate visual grid row with actual shopping cart item. 
                var lblLineItemId = (HiddenField)e.Row.FindControl("lblLineItemID");
                lblLineItemId.Value = li.OrderLineItemID;
                // MS-4788 Enable the grid row if it represents actual shopping cart item.
                // The item can have bundled items which don't have associated row in shopping cart. 
                // Such bundled items are handled by server.
                e.Row.Enabled = GetLineItem(li.OrderLineItemID) != null;
                

                string productName = "Product";

                if (li.Product != null)
                    using (var api = GetServiceAPIProxy())
                        productName = api.GetName(li.Product).ResultValue;


                cvQuantity.ErrorMessage = string.Format("The quantity you have specified for '{0}' is invalid.",
                    productName);
                lblProductName.Text = productName;

                lblProductPrice.Text = li.Total.ToString("C");

                break;
        }

    }

    protected void gvShoppingCart_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string commandArg = e.CommandArgument.ToString();

        //If the command argument is null then check the command source
        if (string.IsNullOrWhiteSpace(commandArg))
        {
            WebControl commandSource = e.CommandSource as WebControl;
            if (commandSource == null)
                return;

            if (commandSource.ID == "btnUpdate")
            {
                foreach (GridViewRow row in gvShoppingCart.Rows)
                {
                    //Get the quantity from the textbox in the gridview
                    TextBox tbQuantity = (TextBox)row.FindControl("tbQuantity");
                    int quantity = int.Parse(tbQuantity.Text);

                    // MS-4788 Check if visual grid row has actual line item in shopping cart.
                    var lineItem = GetLineItem(row);
                    if (lineItem == null)
                    {
                        continue;
                    }

                    if (quantity < 1)
                    {
                        MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.Remove(lineItem);
                    }
                    else
                    {
                        lineItem.Quantity = quantity;
                        lineItem.Total = quantity * lineItem.UnitPrice;
                    }
                }
            }

            using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                loadDataFromConcierge(proxy);
            }
            return;
        }

        //It was a command on a specific row so get the info on the selected row and check the command name
        int selectedIndex = int.Parse(commandArg);
        GridViewRow gvr = gvShoppingCart.Rows[selectedIndex];

        switch (e.CommandName.ToLower())
        {
            case "delete":
                MultiStepWizards.PlaceAnOrder.ShoppingCart.DiscountCodes = null;    // remove any discounts
                // MS-4788 Check if visual grid row has actual line item in shopping cart.
                var lineItem = GetLineItem(gvr);
                if (lineItem != null)
                {
                    MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.Remove(lineItem);                    
                }
                break;
        }
    }

    protected void gvShoppingCart_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadDataFromConcierge(proxy);
        }
    }

    #endregion

    protected void btnClear_Click(object sender, EventArgs e)
    {
        MultiStepWizards.PlaceAnOrder.Clear();
        QueueBannerMessage("Your shopping cart has been cleared.");
        Refresh();
    }
}