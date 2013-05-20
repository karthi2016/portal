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
using Telerik.Charting;

public partial class onlinestorefront_BrowseMerchandise : PortalPage
{
    #region Fields

    protected List<ProductInfo> describedProducts;
    protected PreProcessedOrderPacket preProcessedOrderPacket;
    protected DataTable dtMerchandise;
    protected DataView dvCategories;
    protected msProductCategory targetCategory;

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

        MultiStepWizards.PlaceAnOrder.InitializeShoppingCart();

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadDataFromConcierge(proxy);
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
            hlCategory.Text = string.Format("{0}", targetCategory.Name);
            hlCategory.NavigateUrl = string.Format("~/onlinestorefront/BrowseMerchandise.aspx?contextID={0}", targetCategory.ID);
            hlCategory.Visible = true;
        }

        if (dvCategories.Count > 0)
        {
            blCategories.DataSource = dvCategories;
            blCategories.DataBind();
        }
        else
            phCategories.Visible = false;

        bindRecentItems();

        if (dtMerchandise.Rows.Count == 0)
        {
            lblNoProducts.Text += string.IsNullOrWhiteSpace(ContextID) ? "There are no products currently available for sale." : "There are no products currently available for sale in this category.";
            lblNoProducts.Visible = true;
            return;
        }


        rptMerchandise.DataSource = dtMerchandise;
        rptMerchandise.DataBind();

        divLastLine.Visible = dtMerchandise.Rows.Count % 2 != 0;

        hlCartSubTotal.Text = string.Format("Cart Subtotal: {0:C}",
             preProcessedOrderPacket.FinalizedOrder.ConvertTo<msOrder>().LineItems.Sum(x => x.UnitPrice * x.Quantity));
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge(IConciergeAPIService proxy)
    {
        //First execute the multi-search which will find products that are available in the given category
        //Only the ID is returned in the merchandise search because we'll then describe the products for any logged in entity which would apply any member discounts


        List<Search> searches = new List<Search>();

        //Category Search
        Search sCategories = new Search { Type = msProductCategory.CLASS_NAME, ID=msProductCategory.CLASS_NAME };
        sCategories.AddOutputColumn("ID");
        sCategories.AddOutputColumn("Name");
        sCategories.AddCriteria(Expr.Equals("IsActive", true));
        sCategories.AddCriteria(Expr.Equals("ProductType", "Merchandise"));
        sCategories.AddSortColumn("Name");

        searches.Add(sCategories);

        //Merchandise Search (in the current category)
        Search sMerchandise = new Search { Type = msMerchandise.CLASS_NAME, ID=msMerchandise.CLASS_NAME };

        //Output Fields
        sMerchandise.AddOutputColumn("ID");
        sMerchandise.AddOutputColumn("Name");
        sMerchandise.AddOutputColumn("Image");
        sMerchandise.AddOutputColumn("Price");
        sMerchandise.AddOutputColumn("MemberPrice");
        sMerchandise.AddOutputColumn("ShortDescription");
        sMerchandise.AddOutputColumn("Description");
        sMerchandise.AddOutputColumn("DisplayPriceAs");

        //Criteria
        sMerchandise.AddCriteria(Expr.Equals("IsActive", true));
        sMerchandise.AddCriteria(Expr.Equals("SellOnline", true));

        SearchOperationGroup sellFromGroup = new SearchOperationGroup { FieldName = "SellFrom" };
        sellFromGroup.Criteria.Add(Expr.Equals("SellFrom", null));
        sellFromGroup.Criteria.Add(Expr.IsLessThan("SellFrom", DateTime.Now));
        sellFromGroup.GroupType = SearchOperationGroupType.Or;
        sMerchandise.AddCriteria(sellFromGroup);

        SearchOperationGroup sellUntilGroup = new SearchOperationGroup { FieldName = "SellUntil" };
        sellUntilGroup.Criteria.Add(Expr.Equals("SellUntil", null));
        sellUntilGroup.Criteria.Add(Expr.IsGreaterThan("SellUntil", DateTime.Now));
        sellUntilGroup.GroupType = SearchOperationGroupType.Or;
        sMerchandise.AddCriteria(sellUntilGroup);

        if (!string.IsNullOrWhiteSpace(ContextID))
            sMerchandise.AddCriteria(Expr.Equals("Category", ContextID));

        //Sort
        sMerchandise.AddSortColumn("DisplayOrder");
        sMerchandise.AddSortColumn("Name");

        searches.Add(sMerchandise);


        List<SearchResult> results = ExecuteSearches(proxy, searches, 0, null);
        dvCategories = new DataView(results.Single(x => x.ID == msProductCategory.CLASS_NAME).Table);

        dtMerchandise = results.Single(x => x.ID == msMerchandise.CLASS_NAME).Table;
        dtMerchandise.PrimaryKey = new[] { dtMerchandise.Columns["ID"] };
        dtMerchandise.Columns.Add("Quantity", typeof(int));
        dtMerchandise.Columns.Add("PriceForCurrentEntity", typeof(decimal));

        if (!string.IsNullOrWhiteSpace(ContextID))
            targetCategory = LoadObjectFromAPI<msProductCategory>(ContextID);


        //Describe the products for this entity to see if they're different
        if (CurrentEntity != null && dtMerchandise.Rows.Count > 0)
        {

            List<string> products = (from DataRow dr in dtMerchandise.Rows select dr["ID"].ToString()).ToList();
            describedProducts = proxy.DescribeProducts(CurrentEntity.ID, products).ResultValue;

            MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.RemoveAll(
                x => describedProducts.Any(y => y.ProductID == x.Product && !y.IsEligible));
        }

        preProcessOrder(proxy);

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

    protected void blCategories_Click(object sender, BulletedListEventArgs e)
    {
        ListItem li = blCategories.Items[e.Index];
        string nextUrl = string.Format("~/onlinestorefront/BrowseMerchandise.aspx?contextID={0}", li.Value);

        GoTo(nextUrl);
    }


    protected void rptMerchandise_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        string productId = e.CommandArgument.ToString();
        if (string.IsNullOrWhiteSpace(productId))
            return;

        DataRow selectedProduct = dtMerchandise.Rows.Find((string)e.CommandArgument);

        switch (e.CommandName)
        {
            case "gotodetails":
                GoTo(string.Format("~/onlinestorefront/ViewMerchandiseDetails.aspx?contextID={0}&categoryID={1}", selectedProduct["ID"].ToString(), ContextID));
                break;
            case "addtocart":
                var lineItem = MultiStepWizards.PlaceAnOrder.AddItemToShoppingCart( 1, selectedProduct );
                  
                bindRecentItems();

                checkForDemographicsAndRedirectIfNecessary(lineItem, Request.RawUrl );
       
                GoTo(Request.RawUrl, "Item successfully added to cart");
                break;
        }
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
            preProcessedOrderPacket.FinalizedOrder.ConvertTo<msOrder>().LineItems.Sum(x => x.UnitPrice*x.Quantity));

        MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Remove(selectedProduct);
        bindRecentItems();
    }

    protected void lbCheckout_Click(object sender, EventArgs e)
    {
        MultiStepWizards.PlaceAnOrder.ContinueShoppingUrl = "~/onlinestorefront/BrowseMerchandise.aspx";

        //If there's nobody currently logged in go through the account creation process before the checkout
        if (ConciergeAPI.CurrentEntity == null)
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=Storefront");

        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess( null );
    }

    #endregion

    protected void rptMerchandise_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Item.DataItem;

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
                Literal litPrice = (Literal)e.Item.FindControl("litPrice");
                ImageButton imgProduct = (ImageButton)e.Item.FindControl("imgProduct");

                litPrice.Text = drv["DisplayPriceAs"] != DBNull.Value &&
                                string.IsNullOrWhiteSpace(drv["DisplayPriceAs"].ToString())
                                    ? drv["DisplayPriceAs"].ToString()
                                    : string.Format("{0:C}", (decimal) drv["Price"]);

                if(describedProducts != null)
                {
                    ProductInfo describedProduct =
                        describedProducts.SingleOrDefault(x => x.ProductID == drv["ID"].ToString());

                    if (describedProduct != null)
                    {
                        if (describedProduct.Price != (decimal) drv["Price"])
                            litPrice.Text = string.Format("<strike>{0}</strike> {1}", litPrice.Text,
                                                          !string.IsNullOrWhiteSpace(describedProduct.DisplayPriceAs)
                                                              ? describedProduct.DisplayPriceAs
                                                              : string.Format("{0:C}", describedProduct.Price));

                        e.Item.Visible = describedProduct.IsEligible;
                    }

                    
                }

                string imageID = Convert.ToString(drv["Image"]);

                if (!string.IsNullOrWhiteSpace(imageID))
                    imgProduct.ImageUrl = GetImageUrl(imageID);
                        

                break;
        }

    }
}