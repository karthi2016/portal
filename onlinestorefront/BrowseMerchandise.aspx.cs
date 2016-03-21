using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
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
    private const string CategoryCacheKey = "OnlineStore_Categories";
    private const string CurrentCategoryCacheKey = "OnlineStore_CurrentCategory";
    private const string MerchandiseCacheKey = "OnlineStore_Merchandise";
    private const string MerchandiseTotalCacheKey = "OnlineStore_MerchandiseTotal";
    private const string MerchandiseProcessedCacheKey = "OnlineStore_MerchandiseProcessed";
    private const string MerchandiseDisplayTotalCacheKey = "OnlineStore_MerchandiseDisplayTotal";

    // Until "DescribeProducts" API call is more efficient, we should not look ahead unless absolutely necessary.
    private const int MerchandisePageLookAhead = 0;

    protected DataView Categories;
    protected DataTable Merchandise;
    protected PreProcessedOrderPacket PreProcessedOrderPacket;
    protected msProductCategory TargetCategory;

    protected override bool IsPublic
    {
        get { return true; }
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        MultiStepWizards.PlaceAnOrder.InitializeShoppingCart();
        int pn;
        if (int.TryParse(Request.QueryString["pn"], out pn))
            PageNumber = pn;
        else
            PageNumber = 0;

        var currentCategory = SessionManager.Get<string>(CurrentCategoryCacheKey);
        if (currentCategory != ContextID)
        {
            LoadFresh();
            SessionManager.Set(CurrentCategoryCacheKey, ContextID);
        }
        else
        {
            Bind();
        }

        // MS-1634
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
        // First execute the multi-search which will find products that are available in the given category
        // Only the ID is returned in the merchandise search because we'll then describe the products for any logged in entity which would apply any member discounts

        // Category Search
        SetCategories(proxy);

        if (!string.IsNullOrWhiteSpace(ContextID))
            TargetCategory = LoadObjectFromAPI<msProductCategory>(ContextID);

        PreProcessOrder(proxy);
    }

    private void SetCategories(IConciergeAPIService proxy)
    {
        if (_cachedCategories == null)
        {
            _cachedCategories = SessionManager.Get<DataTable>(CategoryCacheKey);
        }

        if (_cachedCategories != null)
        {
            Categories = new DataView(_cachedCategories);
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

        var results = proxy.GetSearchResult(s, 0, null);
        _cachedCategories = results.Table;

        SessionManager.Set(CategoryCacheKey, results.Table);

        Categories = new DataView(_cachedCategories);
    }

    protected void PreProcessOrder(IConciergeAPIService proxy)
    {
        PreProcessedOrderPacket = proxy.PreProcessOrder(MultiStepWizards.PlaceAnOrder.ShoppingCart).ResultValue;
    }

    protected void BindRecentItems()
    {
        var lineItems = MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems;
        hlCartSubTotal.Text = string.Format("Cart Subtotal: {0:C}", lineItems.Sum(x => x.UnitPrice * x.Quantity));

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

        if (Merchandise != null)
        {
            // Want to refresh inventory every page load for now until it gets to be a performance issue.
            ApplyInventory(Merchandise);

            rptMerchandise.DataSource = Merchandise;
            rptMerchandise.DataBind();
        }

        SetNavigation();
    }

    private void LoadMerchandises()
    {
        if (_cachedMerchandises == null)
        {
            _cachedMerchandises = SessionManager.Get<DataTable>(MerchandiseCacheKey);
        }

        var start = PageNumber * PageSize;
        if (_cachedMerchandises != null && (MerchandiseProcessed == TotalMerchandise || _cachedMerchandises.Rows.Count >= start + PageSize))
        {
            // As long as the Cache contains enough items, go ahead and display the page.
            ApplyFilter(start);
            return;
        }

        var s = new Search { Type = msMerchandise.CLASS_NAME, ID = msMerchandise.CLASS_NAME };

        // Output Fields
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

        // Sort
        s.AddSortColumn("DisplayOrder");
        s.AddSortColumn("Name");

        // Criteria
        s.AddCriteria(Expr.Equals("IsActive", true));
        s.AddCriteria(Expr.Equals("SellOnline", true));

        var searchStart = MerchandiseProcessed;
        var searchLookAhead = PageSize * (1 + MerchandisePageLookAhead);
        int remainingItems;
        var continueSearch = true;

        do
        {
            var result = APIExtensions.GetSearchResult(s, searchStart, searchLookAhead);

            TotalMerchandise = result.TotalRowCount;

            remainingItems = result.TotalRowCount - (searchStart + searchLookAhead);
            if (remainingItems < 0) remainingItems = 0;

            var t = result.Table;
            t.PrimaryKey = new[] { t.Columns["ID"] };
            t.Columns.Add("Quantity", typeof(int));
            t.Columns.Add("PriceToDisplay", typeof(string));
            t.Columns.Add("QuantityAvailable", typeof(decimal));

            List<ProductInfo> describedProducts;
            using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var products = (from DataRow dr in t.Rows select dr["ID"].ToString()).ToList();
                describedProducts =
                    api.DescribeProducts(CurrentEntity != null ? CurrentEntity.ID : null, products).ResultValue;
            }

            if (describedProducts != null)
            {
                // Ported from original code. Not really sure how this would happen, but probably worth keeping around.
                var shoppingCart = MultiStepWizards.PlaceAnOrder.ShoppingCart;
                if (shoppingCart != null)
                    shoppingCart.LineItems.RemoveAll(
                        x => describedProducts.Any(y => y.ProductID == x.Product && !y.IsEligible));
            }

            // Moving this to real-time. Want to refresh inventory every page load for now until it gets to be a performance issue.
            ////ApplyInventory(t);

            var rowsToRemove = new List<DataRow>();
            foreach (DataRow dr in t.Rows)
            {
                var productId = Convert.ToString(dr["ID"]);

                var theBaseProductPrice = (decimal)dr["Price"];
                var priceToDisplay = dr["DisplayPriceAs"] != DBNull.Value &&
                                !string.IsNullOrWhiteSpace(dr["DisplayPriceAs"].ToString())
                                    ? dr["DisplayPriceAs"].ToString()
                                    : string.Format("{0:C}", theBaseProductPrice);

                if (describedProducts != null)
                {
                    var describedProduct = describedProducts.SingleOrDefault(x => x.ProductID == productId);
                    if (describedProduct != null)
                    {
                        if (!describedProduct.IsEligible)
                        {
                            rowsToRemove.Add(dr);

                            // break processing because we are just going to remove this row
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(describedProduct.DisplayPriceAs))
                        {
                            priceToDisplay = describedProduct.DisplayPriceAs;
                        }
                        else
                        {
                            if (describedProduct.Price < theBaseProductPrice)
                                priceToDisplay = string.Format("<strike>{0}</strike> {1:C}", priceToDisplay,
                                    describedProduct.Price);
                            else
                                priceToDisplay = describedProduct.Price.ToString("C"); // just show the pirce
                        }
                    }
                }

                dr["PriceToDisplay"] = priceToDisplay;

                // If adding to existing list
                if (_cachedMerchandises != null)
                {
                    _cachedMerchandises.ImportRow(dr);
                }
            }

            // First pass, so cache entire table
            if (_cachedMerchandises == null)
            {
                // Remove the necessary rows first
                foreach (var dr in rowsToRemove)
                {
                    t.Rows.Remove(dr);
                }

                _cachedMerchandises = t;
            }

            MerchandiseProcessed = searchStart + searchLookAhead;
            if (MerchandiseProcessed > TotalMerchandise) MerchandiseProcessed = TotalMerchandise;

            // If we do not have enough items to display the page yet, continue, otherwise we can stop 
            if (MerchandiseProcessed < TotalMerchandise && _cachedMerchandises.Rows.Count < start + PageSize)
            {
                searchStart += searchLookAhead;
            }
            else
            {
                continueSearch = false;
            }
        } while (continueSearch);

        DisplayTotal = _cachedMerchandises.Rows.Count + remainingItems;

        SessionManager.Set(MerchandiseCacheKey, _cachedMerchandises);

        ApplyFilter(start);
    }

    private void ApplyFilter(int start)
    {
        if (DisplayTotal == 0)
            return;

        var max = start + PageSize;
        var rowList = new List<DataRow>();

        for (int i = start; i < max; i++)
        {
            if (i < _cachedMerchandises.Rows.Count)
            {
                rowList.Add(_cachedMerchandises.Rows[i]);
            }
        }

        if (rowList.Count == 0)
        {
            PageNumber--;
            ApplyFilter(start - PageSize);
        }
        else
        {
            Merchandise = rowList.CopyToDataTable();
        }
    }

    private void SetNavigation()
    {
        btnFirst.Enabled = btnBFirst.Enabled = PageNumber > 0;
        btnPrevous.Enabled = btnBPrevous.Enabled = PageNumber > 0;
        var x = (int)Math.Ceiling((decimal)DisplayTotal / PageSize);

        btnNext.Enabled = btnBNext.Enabled = x > PageNumber + 1;
        btnLast.Enabled = btnBLast.Enabled = x > PageNumber + 1;

        tblNavInfo.Visible = tblBottomNavInfo.Visible = rptMerchandise.Visible = DisplayTotal > 0;
        lblNoProducts.Visible = !rptMerchandise.Visible;

        lUpdateInfo.Text = lbUpdateInfo.Text = string.Format("{3} products found. Now viewing {0} of {1} {2}", PageNumber + 1, x, x > 1 ? "Pages" : "Page", DisplayTotal);

        if (DisplayTotal == 0)
        {
            lblNoProducts.Text = string.IsNullOrWhiteSpace(ContextID)
                                     ? "There are no products currently available for sale."
                                     : "There are no products currently available for sale in this category.";
            return;
        }

        AddPageNumberAttribute();

        divLastLine.Visible = DisplayTotal % 2 != 0;
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

        if (_cachedMerchandises == null)
            BindMerchandises();

        if (_cachedMerchandises == null)
            return;

        var selectedProduct = _cachedMerchandises.Rows.Find(e.CommandArgument);

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

        MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Remove(selectedProduct);
        BindRecentItems();
    }

    protected void lbCheckout_Click(object sender, EventArgs e)
    {
        MultiStepWizards.PlaceAnOrder.ContinueShoppingUrl = "~/onlinestorefront/BrowseMerchandise.aspx";

        // If there's nobody currently logged in go through the account creation process before the checkout
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

                litPrice.Text = Convert.ToString(dr["PriceToDisplay"]);

                var allowBackOrder = Convert.ToBoolean(dr["AllowBackOrders"]);
                var trackInventory = Convert.ToBoolean(dr["TrackInventory"]);
                if (trackInventory && !allowBackOrder)
                {
                    if (Convert.ToDecimal(dr["QuantityAvailable"]) == 0)
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
        _cachedMerchandises = _cachedCategories = null;
        MerchandiseProcessed = 0;
        SessionManager.Set<DataTable>(CategoryCacheKey, null);
        SessionManager.Set<DataTable>(MerchandiseCacheKey, null);

        if (PageNumber < 0 || PageNumber > DisplayTotal)
            PageNumber = 0;

        Bind();
    }

    private void Bind()
    {
        BindMerchandises();
        using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            LoadDataFromConcierge(proxy);
        }
    }

    private DataTable _cachedMerchandises;

    private DataTable _cachedCategories;

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

    private int DisplayTotal
    {
        get
        {
            var count = SessionManager.Get<int?>(MerchandiseDisplayTotalCacheKey);
            return count.HasValue ? count.Value : 0;
        }
        set
        {
            SessionManager.Set<int?>(MerchandiseDisplayTotalCacheKey, value);
        }
    }

    private int TotalMerchandise
    {
        get
        {
            var count = SessionManager.Get<int?>(MerchandiseTotalCacheKey);
            return count.HasValue ? count.Value : 0;
        }
        set
        {
            SessionManager.Set<int?>(MerchandiseTotalCacheKey, value);
        }
    }

    private int MerchandiseProcessed
    {
        get
        {
            var count = SessionManager.Get<int?>(MerchandiseProcessedCacheKey);
            return count.HasValue ? count.Value : 0;
        }
        set
        {
            SessionManager.Set<int?>(MerchandiseProcessedCacheKey, value);
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
        PageNumber = (int)Math.Ceiling((decimal)DisplayTotal / PageSize);
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

    private void ApplyInventory(DataTable productData)
    {
        // Initialize the Dictionary with the ProductIDs as keys
        var inventoriedProducts = (from DataRow dr in productData.Rows
                                   where Convert.ToBoolean(dr["TrackInventory"]) && !Convert.ToBoolean(dr["AllowBackOrders"])
                                   select dr["ID"].ToString()).ToDictionary(k => k, v => 0m);

        if (inventoriedProducts.Count > 0)
        {
            var inventorySearch = new Search(msStockItemInventory.CLASS_NAME);
            inventorySearch.AddOutputColumn("Product");
            inventorySearch.AddOutputColumn("QuantityAvailable");
            inventorySearch.AddCriteria(Expr.IsOneOfTheFollowing("Product", inventoriedProducts.Keys.ToList()));
            inventorySearch.AddCriteria(Expr.IsGreaterThan("QuantityAvailable", 0));
            var inventoryResults = APIExtensions.GetSearchResult(inventorySearch, 0, null);
            foreach (DataRow dr in inventoryResults.Table.Rows)
            {
                inventoriedProducts[Convert.ToString(dr["Product"])] += Convert.ToDecimal(dr["QuantityAvailable"]);
            }

            foreach (DataRow dr in productData.Rows)
            {
                var productId = Convert.ToString(dr["ID"]);
                if (inventoriedProducts.ContainsKey(productId))
                {
                    dr["QuantityAvailable"] = inventoriedProducts[productId];
                }
            }
        }
    }
}