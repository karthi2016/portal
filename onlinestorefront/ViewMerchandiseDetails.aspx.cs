using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class onlinestorefront_ViewMerchandiseDetails : PortalPage
{
    #region Fields

    protected ProductInfo describedProduct;
    protected PreProcessedOrderPacket preProcessedOrderPacket;
    protected DataRow targetMerchandise;
    protected DataRow targetCategory;
    protected DataTable dtCategories;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected string CategoryID
    {
        get { return Request.QueryString["categoryID"]; }
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

        MultiStepWizards.PlaceAnOrder.InitializeShoppingCart();

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadDataFromConcierge(proxy);
        }

        if (targetMerchandise == null)
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

        //MS-1634
        if (CurrentEntity != null && MultiStepWizards.PlaceAnOrder.ShoppingCart != null &&
            (MultiStepWizards.PlaceAnOrder.ShoppingCart.BillTo == null ||
             MultiStepWizards.PlaceAnOrder.ShoppingCart.ShipTo == null))
        {
            MultiStepWizards.PlaceAnOrder.ShoppingCart.BillTo =
                MultiStepWizards.PlaceAnOrder.ShoppingCart.ShipTo = CurrentEntity.ID;

            using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                preProcessOrder(proxy);
            }
        }

        hlCategory.Visible = false;
        if (targetCategory != null)
        {
            hlAllMerchandise.Text += " >";
            hlCategory.Text = string.Format("{0}", targetCategory["Name"]);
            hlCategory.NavigateUrl = string.Format("~/onlinestorefront/BrowseMerchandise.aspx?contextID={0}",
                                                   targetCategory["ID"]);
            hlCategory.Visible = true;
        }

        imgMerchandise.ImageUrl = GetImageUrl( Convert.ToString( targetMerchandise["Image"]) );


        blCategories.DataSource = dtCategories;
        blCategories.DataBind();

        bindRecentItems();

        hlCartSubTotal.Text = string.Format("Cart Subtotal: {0:C}",
                                            preProcessedOrderPacket.FinalizedOrder.ConvertTo<msOrder>().LineItems.Sum(
                                                x => x.UnitPrice*x.Quantity));

        litPrice.Text = targetMerchandise["DisplayPriceAs"] != DBNull.Value &&
                        string.IsNullOrWhiteSpace(targetMerchandise["DisplayPriceAs"].ToString())
                            ? targetMerchandise["DisplayPriceAs"].ToString()
                            : string.Format("{0:C}", (decimal)targetMerchandise["Price"]);

        // MS-4786
        if (describedProduct == null)
        {
            return;
        }

        var describedPrice = string.IsNullOrWhiteSpace(describedProduct.DisplayPriceAs)
                                 ? string.Format("{0:C}", describedProduct.Price) : describedProduct.DisplayPriceAs;

        litPrice.Text = describedProduct.Price < (decimal)targetMerchandise["Price"] 
                            ? string.Format("<strike>{0}</strike> {1}", litPrice.Text, describedPrice) : describedPrice;
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge(IConciergeAPIService proxy)
    {
        List<Search> searches = new List<Search>();

        Search sMerchandise = new Search(msMerchandise.CLASS_NAME) { ID = msMerchandise.CLASS_NAME };
        sMerchandise.AddOutputColumn("ID");
        sMerchandise.AddOutputColumn("Name");
        sMerchandise.AddOutputColumn("Image");
        sMerchandise.AddOutputColumn("Description");
        sMerchandise.AddOutputColumn("ShortDescription");
        sMerchandise.AddOutputColumn("Price");
        sMerchandise.AddOutputColumn("MemberPrice");
        sMerchandise.AddOutputColumn("DisplayPriceAs");
        sMerchandise.AddCriteria(Expr.Equals("ID", ContextID));

        searches.Add(sMerchandise);

        //Category Search
        Search sCategories = new Search { Type = msProductCategory.CLASS_NAME, ID = msProductCategory.CLASS_NAME };
        sCategories.AddOutputColumn("ID");
        sCategories.AddOutputColumn("Name");
        sCategories.AddCriteria(Expr.Equals("ProductType", "Merchandise"));
        sCategories.AddCriteria(Expr.Equals("IsActive", true));
        sCategories.AddSortColumn("Name");

        searches.Add(sCategories);

        List<SearchResult> searchResults = ExecuteSearches(proxy, searches, 0, null);

        if (searchResults == null || searchResults.Count < 2 || searchResults[0] == null || searchResults[0].Table == null || searchResults[0].Table.Rows.Count != 1)
        {
            GoToMissingRecordPage();
            return;
        }

        //Add a quantity column so that this row can be added properly to the recently added items list if Add to Cart is used
        SearchResult srMerchandise = searchResults.Single(x => x.ID == msMerchandise.CLASS_NAME);
        srMerchandise.Table.Columns.Add("Quantity", typeof(int));
        srMerchandise.Table.Columns.Add("PriceForCurrentEntity", typeof(decimal));
        targetMerchandise = srMerchandise.Table.Rows[0];

        SearchResult srCategories = searchResults.Single(x => x.ID == msProductCategory.CLASS_NAME);
        dtCategories = srCategories.Table;
        dtCategories.PrimaryKey = new[] { dtCategories.Columns["ID"] };


        if (!string.IsNullOrWhiteSpace(CategoryID))
            targetCategory = srCategories.Table.Rows.Find(CategoryID);


        preProcessOrder(proxy);

        //Describe the products for this entity to see if they're different
        //if (CurrentEntity == null || srMerchandise.Table.Rows.Count == 0) return;
        // MS-4786. We still need to describer products even the entity is null.
        if (srMerchandise.Table.Rows.Count == 0) return;

        describedProduct = proxy.DescribeProducts(CurrentEntity == null ? string.Empty : CurrentEntity.ID, new List<string>() { targetMerchandise["ID"].ToString() }).ResultValue[0];
    }

    protected void preProcessOrder(IConciergeAPIService proxy)
    {
        preProcessedOrderPacket = proxy.PreProcessOrder(MultiStepWizards.PlaceAnOrder.ShoppingCart).ResultValue;
    }

    protected void bindRecentItems()
    {
        if (MultiStepWizards.PlaceAnOrder.RecentlyAddedItems == null)
            MultiStepWizards.PlaceAnOrder.RecentlyAddedItems = new List<DataRow>();

        rptRecentItems.DataSource = MultiStepWizards.PlaceAnOrder.RecentlyAddedItems;
        rptRecentItems.DataBind();

        if (MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Count == 0)
        {
            divRecentlyAdded.Visible = false;
            return;
        }
    }
    #endregion



    #region Event Handlers

    protected void lbAddToCart_Click(object sender, EventArgs e)
    {
        int quantity = int.Parse(tbQuantity.Text);

        var lineItem = MultiStepWizards.PlaceAnOrder.AddItemToShoppingCart(quantity, targetMerchandise);

        bindRecentItems();

        string redirectUrl = "/onlinestorefront/BrowseMerchandise.aspx";
        if (targetCategory != null)
            redirectUrl += "?contextID=" + targetCategory["ID"];

        // are there demographics?
        CheckForDemographicsAndRedirectIfNecessary(lineItem, redirectUrl);
        GoTo(redirectUrl, "Item successfully added to cart.");

    }

    protected void rptRecentItems_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        if (e.Item == null)
            return;

        DataRow selectedProduct = MultiStepWizards.PlaceAnOrder.RecentlyAddedItems[e.Item.ItemIndex];
        msOrderLineItem lineItem =
            MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.FirstOrDefault(x => x.Product == selectedProduct["ID"].ToString());

        //If the line item is null it has been removed by editing the cart so remove it from the recent item list
        if (lineItem == null)
        {
            MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Remove(selectedProduct);
            return;
        }

        if (lineItem.Quantity > 1)
            lineItem.Quantity--;
        else
        {
            MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.Remove(lineItem);

            using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                preProcessOrder(proxy);
            }
        }

        hlCartSubTotal.Text = string.Format("Cart Subtotal: {0:C}",
            preProcessedOrderPacket.FinalizedOrder.ConvertTo<msOrder>().LineItems.Sum(x => x.UnitPrice * x.Quantity));


        MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Remove(selectedProduct);
        bindRecentItems();
    }

    protected void lbCheckout_Click(object sender, EventArgs e)
    {
        MultiStepWizards.PlaceAnOrder.ContinueShoppingUrl = "~/onlinestorefront/BrowseMerchandise.aspx";

        

        //If there's nobody currently logged in go through the account creation process before the checkout
        if (ConciergeAPI.CurrentEntity == null)
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=Storefront");

        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(null);
    }

    protected void lbContinueShopping_Click(object sender, EventArgs e)
    {
        GoTo(targetCategory == null
                 ? "~/onlinestorefront/BrowseMerchandise.aspx"
                 : string.Format("~/onlinestorefront/BrowseMerchandise.aspx?contextID={0}",
                                 targetCategory["ID"].ToString()));
    }

    protected void blCategories_Click(object sender, BulletedListEventArgs e)
    {
        ListItem li = blCategories.Items[e.Index];
        string nextUrl = string.Format("~/onlinestorefront/BrowseMerchandise.aspx?contextID={0}", li.Value);

        GoTo(nextUrl);
    }

    #endregion
}
