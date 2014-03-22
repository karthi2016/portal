using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using Telerik.Web.UI;

public partial class onlinestorefront_BrowseMerchandise : PortalPage
{
    protected DataView Categories;
    protected List<ProductInfo> DescribedProducts;
    protected DataTable Merchandise;
    protected PreProcessedOrderPacket PreProcessedOrderPacket;
    protected msProductCategory TargetCategory;
    private bool _filterRequested = false;

    protected override bool IsPublic
    {
        get { return true; }
    }


    /// <summary>
    ///     Initializes the target object for the page
    /// </summary>
    /// <remarks>
    ///     Many pages have "target" objects that the page operates on. For instance, when viewing
    ///     an event, the target object is an event. When looking up a directory, that's the target
    ///     object. This method is intended to be overriden to initialize the target object for
    ///     each page that needs it.
    /// </remarks>
    //protected override void InitializeTargetObject()
    //{
    //    base.InitializeTargetObject();
    //    MultiStepWizards.PlaceAnOrder.InitializeShoppingCart();


    //    LoadFresh();
    //}


    ///// <summary>
    /////     Initializes the page.
    ///// </summary>
    ///// <remarks>
    /////     This method runs on the first load of the page, and does NOT
    /////     run on postbacks. If you want to run a method on PostBacks, override the
    /////     Page_Load event
    ///// </remarks>
    //protected override void Page_Load(object sender, EventArgs e)
    //{
    //    base.Page_Load(sender, e);
    //    if (IsPostBack)
    //        InitializePage();

    //}

    protected override void InitializePage()
    {
        base.InitializePage();
        MultiStepWizards.PlaceAnOrder.InitializeShoppingCart();
        int pn;
        if (int.TryParse(Request.QueryString["pn"], out pn))
            PageNumber = pn;
        else
            PageNumber = 1;

        LoadFresh();

        //MS-1634
        if (CurrentEntity != null && MultiStepWizards.PlaceAnOrder.ShoppingCart != null &&
            (MultiStepWizards.PlaceAnOrder.ShoppingCart.BillTo == null ||
             MultiStepWizards.PlaceAnOrder.ShoppingCart.ShipTo == null))
        {
            MultiStepWizards.PlaceAnOrder.ShoppingCart.BillTo = MultiStepWizards.PlaceAnOrder.ShoppingCart.ShipTo = CurrentEntity.ID;

            using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                PreProcessOrder(proxy);
            }
        }

        hlCategory.Visible = false;
        if (TargetCategory != null)
        {
            hlCategory.Text = string.Format("{0}", TargetCategory.Name);
            hlCategory.NavigateUrl = string.Format("~/onlinestorefront/BrowseMerchandise.aspx?contextID={0}", TargetCategory.ID);
            hlCategory.Visible = true;
        }

        if (Categories.Count > 0)
        {
            blCategories.DataSource = Categories;
            blCategories.DataBind();
        }
        else
            phCategories.Visible = false;

        BindRecentItems();


    }


    protected void LoadDataFromConcierge(IConciergeAPIService proxy)
    {
        //First execute the multi-search which will find products that are available in the given category
        //Only the ID is returned in the merchandise search because we'll then describe the products for any logged in entity which would apply any member discounts



        //Category Search
        SetCategories(proxy);


        if (!string.IsNullOrWhiteSpace(ContextID))
            TargetCategory = LoadObjectFromAPI<msProductCategory>(ContextID);




        PreProcessOrder(proxy);
    }

    private void SetCategories(IConciergeAPIService proxy)
    {
        if (CachedCategories != null)
        {
            Categories = new DataView(CachedCategories);
            return;
        }
        var s = new Search
        {
            Type = msProductCategory.CLASS_NAME,
            ID = msProductCategory.CLASS_NAME
        };
        s.AddOutputColumn("ID");
        s.AddOutputColumn("Name");
        s.AddCriteria(Expr.Equals("IsActive", true));
        s.AddCriteria(Expr.Equals("ProductType", "Merchandise"));
        s.AddSortColumn("Name");


        var results = ExecuteSearch(proxy, s, 0, null);
        CachedCategories = results.Table;
        Categories = new DataView(CachedCategories);

    }


    protected void PreProcessOrder(IConciergeAPIService proxy)
    {
        PreProcessedOrderPacket = proxy.PreProcessOrder(MultiStepWizards.PlaceAnOrder.ShoppingCart).ResultValue;
    }

    protected void BindRecentItems()
    {
        if (MultiStepWizards.PlaceAnOrder.RecentlyAddedItems == null)
            MultiStepWizards.PlaceAnOrder.RecentlyAddedItems = new List<DataRow>();

        rptRecentItems.DataSource = MultiStepWizards.PlaceAnOrder.RecentlyAddedItems;
        rptRecentItems.DataBind();

        if (MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Count == 0)
        {
            divRecentlyAdded.Visible = false;
        }
    }


    protected void blCategories_Click(object sender, BulletedListEventArgs e)
    {
        var li = blCategories.Items[e.Index];
        var nextUrl = string.Format("~/onlinestorefront/BrowseMerchandise.aspx?contextID={0}", li.Value);

        GoTo(nextUrl);
    }


    private void BindMerchandises()
    {

        LoadMerchandises();

        //Describe the products for this entity to see if they're different
        using (var proxy = GetConciegeAPIProxy())
            if ( Merchandise != null &&  Merchandise.Rows.Count > 0)
            {
                var products = (from DataRow dr in Merchandise.Rows select dr["ID"].ToString()).ToList();
                DescribedProducts = proxy.DescribeProducts(CurrentEntity != null ? CurrentEntity.ID : null, products).ResultValue;

                var shoppingCart = MultiStepWizards.PlaceAnOrder.ShoppingCart;
                if ( shoppingCart != null )
                    shoppingCart.LineItems.RemoveAll(x => DescribedProducts.Any(y => y.ProductID == x.Product && !y.IsEligible));
            }
        if (Merchandise != null)
        {
            rptMerchandise.DataSource = Merchandise;
            rptMerchandise.DataBind();
        }
        SetNavigation();

    }

    private void LoadMerchandises()
    {
        var start = PageNumber * PageSize;
        if (CachedMerchandises != null)
        {
            ApplyFilter(start);
            return;
        }


        var s = new Search { Type = msMerchandise.CLASS_NAME, ID = msMerchandise.CLASS_NAME };

        //Output Fields
        s.AddOutputColumn("ID");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("Image");
        s.AddOutputColumn("Price");
        s.AddOutputColumn("MemberPrice");
        s.AddOutputColumn("ShortDescription");
        s.AddOutputColumn("Description");
        s.AddOutputColumn("DisplayPriceAs");
        s.AddOutputColumn("TrackInventory");
        s.AddOutputColumn("AllowBackOrders");
        var sellFromGroup = new SearchOperationGroup { FieldName = "SellFrom" };
        sellFromGroup.Criteria.Add(Expr.Equals("SellFrom", null));
        sellFromGroup.Criteria.Add(Expr.IsLessThan("SellFrom", DateTime.Now));
        sellFromGroup.GroupType = SearchOperationGroupType.Or;
        s.AddCriteria(sellFromGroup);

        var sellUntilGroup = new SearchOperationGroup { FieldName = "SellUntil" };
        sellUntilGroup.Criteria.Add(Expr.Equals("SellUntil", null));
        sellUntilGroup.Criteria.Add(Expr.IsGreaterThan("SellUntil", DateTime.Now));
        sellUntilGroup.GroupType = SearchOperationGroupType.Or;
        s.AddCriteria(sellUntilGroup);
        if (!string.IsNullOrEmpty(tbProductDescription.Text))
        {
            s.AddCriteria(Expr.Contains("ShortDescription", tbProductDescription.Text));
        }
        if (!string.IsNullOrEmpty(tbProductName.Text))
        {
            s.AddCriteria(Expr.Contains("Name", tbProductName.Text));
        }
        if (string.IsNullOrEmpty(tbProductDescription.Text) && string.IsNullOrEmpty(tbProductName.Text) &&
            !string.IsNullOrWhiteSpace(ContextID))
            s.AddCriteria(Expr.Equals("Category", ContextID));

        //Sort
        s.AddSortColumn("DisplayOrder");
        s.AddSortColumn("Name");

        //Criteria
        s.AddCriteria(Expr.Equals("IsActive", true));
        s.AddCriteria(Expr.Equals("SellOnline", true));
        var result = ExecuteSearch(s, 0, null);
        TotalRowCount = result.TotalRowCount;
        var t = result.Table;
        t.PrimaryKey = new[] { t.Columns["ID"] };
        t.Columns.Add("Quantity", typeof(int));
        t.Columns.Add("PriceForCurrentEntity", typeof(decimal));
        CachedMerchandises = t;
        ApplyFilter(start);
    }

    private void ApplyFilter(int start)
    {
        if (TotalRowCount == 0)
            return;
        var max = start + PageSize;
        var expression = string.Format("ROW_NUMBER > {0} AND ROW_NUMBER <={1} ", start, max);
        Merchandise = CachedMerchandises.Select(expression).CopyToDataTable();
    }

    private void SetNavigation()
    {
        btnFirst.Enabled = btnBFirst.Enabled = PageNumber > 1;
        btnPrevous.Enabled = btnBPrevous.Enabled = PageNumber > 0;
        var x = TotalRowCount / PageSize;
        btnNext.Enabled = btnBNext.Enabled = x > PageNumber; ;
        btnLast.Enabled = btnBLast.Enabled = x - 1 > PageNumber;
        tblNavInfo.Visible = tblBottomNavInfo.Visible = TotalRowCount > 0;
        lUpdateInfo.Text = lbUpdateInfo.Text = string.Format("{3} products found. Now viewing {0} of {1} {2}", PageNumber + 1, x + 1, x > 1 ? "Pages" : "Page", TotalRowCount);


        if (TotalRowCount == 0)
        {
            lblNoProducts.Text = string.IsNullOrWhiteSpace(ContextID)
                                     ? "There are no products currently available for sale."
                                     : "There are no products currently available for sale in this category.";
            lblNoProducts.Visible = true;
            return;
        }
        AddPageNumberAttribute();

        

        divLastLine.Visible = TotalRowCount % 2 != 0;
        if (PreProcessedOrderPacket == null)
            return;
        hlCartSubTotal.Text = string.Format("Cart Subtotal: {0:C}", PreProcessedOrderPacket.FinalizedOrder.ConvertTo<msOrder>().LineItems.Sum(xx => xx.UnitPrice * xx.Quantity));

    }
    protected void rptMerchandise_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        var productId = e.CommandArgument.ToString();
        if (string.IsNullOrWhiteSpace(productId))
        {
            return;
        }
        if (CachedMerchandises == null)
            BindMerchandises();
        if (CachedMerchandises == null)
            return;
        var selectedProduct = CachedMerchandises.Rows.Find(e.CommandArgument);

        switch (e.CommandName)
        {
            case "gotodetails":
                GoTo(string.Format("~/onlinestorefront/ViewMerchandiseDetails.aspx?contextID={0}&categoryID={1}",
                                   selectedProduct["ID"], ContextID));
                break;
            case "addtocart":
                var lineItem = MultiStepWizards.PlaceAnOrder.AddItemToShoppingCart(1, selectedProduct);

                BindRecentItems();

                CheckForDemographicsAndRedirectIfNecessary(lineItem, Request.RawUrl);
                var url = string.Format("{0}?pn={1}", Request.Url.LocalPath, PageNumber);
                GoTo(url, "Item successfully added to cart");
                break;
        }
    }

    protected void rptRecentItems_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        if (e.Item == null)
            return;

        var selectedProduct = MultiStepWizards.PlaceAnOrder.RecentlyAddedItems[e.Item.ItemIndex];
        var lineItem = MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.FirstOrDefault(x => x.Product == selectedProduct["ID"].ToString());

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

            using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                PreProcessOrder(proxy);
            }
        }
        var o = PreProcessedOrderPacket.FinalizedOrder.ConvertTo<msOrder>();
        hlCartSubTotal.Text = string.Format("Cart Subtotal: {0:C}", o.LineItems.Sum(x => x.UnitPrice * x.Quantity));

        MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Remove(selectedProduct);
        BindRecentItems();
    }

    protected void lbCheckout_Click(object sender, EventArgs e)
    {
        MultiStepWizards.PlaceAnOrder.ContinueShoppingUrl = "~/onlinestorefront/BrowseMerchandise.aspx";

        //If there's nobody currently logged in go through the account creation process before the checkout
        if (ConciergeAPI.CurrentEntity == null)
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=Storefront");

        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(null);
    }



    protected void rptMerchandise_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var dr = (DataRowView)e.Item.DataItem;

        //if (Page.IsPostBack )
        //    return; // only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                var litPrice = (Literal)e.Item.FindControl("litPrice");
                var imgProduct = (ImageButton)e.Item.FindControl("imgProduct");
                var lbAddToCart = (LinkButton)e.Item.FindControl("lbAddToCart");
                lbAddToCart.OnClientClick = "blockUI();";
                var i = new AsyncPostBackTrigger
                {
                    ControlID = lbAddToCart.UniqueID,
                    EventName = "Click"

                };

              
                decimal theBaseProductPrice = (decimal) dr["Price"];
                litPrice.Text = dr["DisplayPriceAs"] != DBNull.Value &&
                                !string.IsNullOrWhiteSpace(dr["DisplayPriceAs"].ToString())
                                    ? dr["DisplayPriceAs"].ToString()
                                    : string.Format("{0:C}", theBaseProductPrice);

                var productId = dr["ID"].ToString();

                if (DescribedProducts != null)
                {
                    var describedProduct = DescribedProducts.SingleOrDefault(x => x.ProductID == productId);

                    if (describedProduct != null)
                    {
                        if (!string.IsNullOrWhiteSpace(describedProduct.DisplayPriceAs))
                            litPrice.Text = describedProduct.DisplayPriceAs;
                        else
                        {
                            if (describedProduct.Price < theBaseProductPrice)
                                litPrice.Text = string.Format("<strike>{0}</strike> {1:C}", litPrice.Text,
                                                              describedProduct.Price);
                            else

                                litPrice.Text = describedProduct.Price.ToString("C"); // just show the pirce
                        }

                        e.Item.Visible = describedProduct.IsEligible;
                    }
                }
                var allowBackOrder = Convert.ToBoolean(dr["AllowBackOrders"]);
                var trackInventory = Convert.ToBoolean(dr["TrackInventory"]);
                if (trackInventory && !allowBackOrder)
                {
                    var s = new Search(msStockItemInventory.CLASS_NAME);
                    s.AddCriteria(Expr.Equals("Product", productId));
                    s.AddCriteria(Expr.IsGreaterThan("QuantityAvailable", 0));
                    var r = ExecuteSearch(s, 0, 1);
                    if (r.TotalRowCount <= 0)
                    {
                        lbAddToCart.Enabled = false;
                        lbAddToCart.Text = "Out of Stock";
                        lbAddToCart.CssClass = "disabledAddToCart";
                    }
                }
                var imageID = Convert.ToString(dr["Image"]);

                if (!string.IsNullOrWhiteSpace(imageID))
                    imgProduct.ImageUrl = GetImageUrl(imageID);


                break;
        }
    }


    private void LoadFresh()
    {
        CachedMerchandises = CachedCategories = null;
        if (PageNumber < 0 || PageNumber > TotalRowCount)
            PageNumber = 0;
        BindMerchandises();
        using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            LoadDataFromConcierge(proxy);
        }
    }



    private DataTable CachedMerchandises
    {
        get;
        set;
    }

    private DataTable CachedCategories
    {
        get;
        set;
    }

    private int PageNumber
    {
        get
        {
            if (ViewState["PageNumber"] is int)
                return Convert.ToInt32(ViewState["PageNumber"]);

            return 0;
        }
        set
        {
            ViewState["PageNumber"] = value;
        }
    }
    private static int PageSize
    {
        get
        {
            return 12;
        }

    }
    private int TotalRowCount
    {
        get
        {
            if (ViewState["TotalRowCount"] is int)
                return Convert.ToInt32(ViewState["TotalRowCount"]);

            return 0;
        }
        set
        {
            ViewState["TotalRowCount"] = value;
        }
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        PageNumber = 0;
        LoadFresh();
        searchText.Attributes["display"] = "block";
    }

    protected void btnLast_Click(object sender, EventArgs e)
    {
        PageNumber = TotalRowCount / PageSize;
        ((Button)sender).Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        BindMerchandises();

    }

    protected void btnFirst_Click(object sender, EventArgs e)
    {
        PageNumber = 0;
        ((Button)sender).Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        BindMerchandises();
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {

        PageNumber++;
        ((Button)sender).Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        BindMerchandises();
    }

    protected void btnPrevous_Click(object sender, EventArgs e)
    {
        PageNumber--;

        BindMerchandises();

    }
    private void AddPageNumberAttribute()
    {
        btnFirst.Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        btnBFirst.Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        btnNext.Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        btnBNext.Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        btnPrevous.Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        btnBPrevous.Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        btnLast.Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
        btnBLast.Attributes.Add("pn", PageNumber.ToString(CultureInfo.InvariantCulture));
    }
}