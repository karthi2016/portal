using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;

public partial class membership_PurchaseMembership1 : PortalPage
{

    #region Fields

    private DataRow drMembershipOrganization;
    private msEntity targetEntity;
    
    #endregion

    #region Properties

    protected string EntityID
    {
        get
        {
            return Request.QueryString["entityID"];
        }
    }

    #endregion

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

        Search sDefaultMemOrg = new Search {Type = msMembershipOrganization.CLASS_NAME};
        sDefaultMemOrg.AddOutputColumn("ID");
        sDefaultMemOrg.AddOutputColumn(msMembershipOrganization.FIELDS.MembersCanJoinThroughThePortal);
        sDefaultMemOrg.AddCriteria(string.IsNullOrWhiteSpace(ContextID)
                                       ? Expr.Equals("IsDefault", true)
                                       : Expr.Equals("ID", ContextID));

        SearchResult srDefaultMemOrg = ExecuteSearch(sDefaultMemOrg, 0, null);
        if (srDefaultMemOrg != null && srDefaultMemOrg.Table != null && srDefaultMemOrg.Table.Rows.Count > 0)
            drMembershipOrganization = srDefaultMemOrg.Table.Rows[0];

        targetEntity = MultiStepWizards.RenewMembership.Entity ??
                      (!string.IsNullOrWhiteSpace(EntityID) ? LoadObjectFromAPI<msEntity>(EntityID) : CurrentEntity);


        if (drMembershipOrganization == null)
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

        MultiStepWizards.RenewMembership.Membership = null; // clear it out
       
        // ok - if there's an existing membership, we want to get it so
        // we can set the default product/type
        Search sExistingMembership = new Search { Type = msMembership.CLASS_NAME };
        sExistingMembership.AddCriteria(Expr.Equals("Owner", targetEntity.ID));
        sExistingMembership.AddCriteria(Expr.Equals("MembershipOrganization", drMembershipOrganization["ID"].ToString()));
        sExistingMembership.AddSortColumn("ExpirationDate", true);
        sExistingMembership.AddOutputColumn(msMembership.FIELDS.Product);   // get the product
        sExistingMembership.AddOutputColumn(msMembership.FIELDS.Product + ".RenewsWith");   // get the product
        sExistingMembership.AddOutputColumn(msMembership.FIELDS.Type);   // get the product

        var dtResult = ExecuteSearch(sExistingMembership, 0, 1);
        if (dtResult.TotalRowCount > 0)
        {
            // ok, there's a current membership
            // so - if the current membership product renews with ANOTHER product, we want to use that - 
            // otherwise, we want to use the same product

            var dr = dtResult.Table.Rows[0];
            string renewalProduct = Convert.ToString(dr[msMembership.FIELDS.Product+ ".RenewsWith"]);
            string currentProduct = Convert.ToString(dr[msMembership.FIELDS.Product]);
            selectedMembershipFeeID = string.IsNullOrWhiteSpace(renewalProduct) ? currentProduct : renewalProduct;
            selectedMembershipTypeID = Convert.ToString( dr[msMembership.FIELDS.Type] );

            // and let's set the membership here
            MultiStepWizards.RenewMembership.Membership = LoadObjectFromAPI<msMembership>(
                Convert.ToString(dr["ID"]));
        }

        populateAvailableMembershipTypesAndProducts();

       
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data. </param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // now, render the javascript so that subcategories are selected automatically
        // we have to do this on every page load, because viewstate doesn't apply to javascript
        renderRadiobuttonJavascript();
    }

    List<MembershipProductInfo> allProducts;
    List<ProductInfo> allDescribedProducts;
    private void populateAvailableMembershipTypesAndProducts()
    {
        
       
        // ok, let's find all of the active membership types
        List<msMembershipType> allTypes =GetAllObjects<msMembershipType>( msMembershipType.CLASS_NAME);

        // now, let's pull out the inactive ones, and the ones that don't match this organization
        allTypes.RemoveAll(x => !x.IsActive );

        allTypes = allTypes.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToList(); // now, sort by display order then name
        
        // let's get all products

  
        using (var api = GetServiceAPIProxy())
        {
            allProducts = api.GetApplicableMembershipDuesProducts( drMembershipOrganization["ID"].ToString() , targetEntity.ID ).ResultValue;
        }

        allProducts.RemoveAll(x => !x.SellOnline);

        // ok, now sort by name
        //MS-1381 - let the API do this allProducts.Sort((x, y) => string.Compare(x.ProductName, y.ProductName, true));  // now, sort by name

        if (allProducts.Count == 0)
            return; // nothing to do

        using (var api = GetServiceAPIProxy())
        {
            allDescribedProducts =
                api.DescribeProducts(targetEntity.ID, allProducts.Select(x => x.ProductID).ToList()).ResultValue;
        }

        //MS-1952 Special prices showing up as 0.00 when not matched
        allProducts.RemoveAll(x => allDescribedProducts.Exists(y => y.ProductID == x.ProductID && !y.IsEligible));
        if (allProducts.Count == 0)
            return; // nothing to do

        allDescribedProducts.RemoveAll(x => !x.IsEligible);

        // set the default
        if (string.IsNullOrWhiteSpace(selectedMembershipFeeID) || !allProducts.Exists(x => x.ProductID == selectedMembershipFeeID))  // use the first, by default
        {
            selectedMembershipFeeID = allProducts[0].ProductID;
            selectedMembershipTypeID = allProducts[0].MembershipType;
        }

        // now, let's create our sections
        List<msMembershipType> typesToRender = new List<msMembershipType>();
        foreach (var type in allTypes)
            if (allProducts.Exists(x => x.MembershipType == type.ID)) // we don't have a single type
                typesToRender.Add(type);

        // we need to build the javascript to show/hide the subcategories along with the sections
       
        rptMembershipTypes.DataSource = typesToRender;
        rptMembershipTypes.DataBind();
 
    }

    #region Javascript

    
    private List<NameValueStringPair> RadioButtonSubCategoryPairs = new List<NameValueStringPair>();
    
    /// <summary>
    /// Renders the radiobutton javascript that shows the products for each
    /// membership type when a user clicks on the type
    /// </summary>
    private void renderRadiobuttonJavascript()
    {
        var sbRadioButtonSelectorJavascript = new StringBuilder("function showAppropriateSubCategory(){");

        // ok, let's first hide all of the buttons
        foreach (var pair in RadioButtonSubCategoryPairs)
        {
            sbRadioButtonSelectorJavascript.AppendLine(string.Format(
                "document.getElementById('{0}').style.display = document.getElementById('{1}').checked ? '' : 'none';",
                pair.Value, pair.Name));
        }

        sbRadioButtonSelectorJavascript.AppendLine("}");

        // now, register the script with the page so it renders
        ClientScript.RegisterClientScriptBlock(GetType(), "showAppropriateSubCategory", sbRadioButtonSelectorJavascript.ToString(), true);
        ClientScript.RegisterStartupScript(GetType(), "showSubCategory", "showAppropriateSubCategory();", true);

    }

    #endregion
    #region Buttons and Events

    private string selectedMembershipFeeID;
    private string selectedMembershipTypeID;
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;
        selectedMembershipTypeID = Request.Form["MembershipType"];
        if (selectedMembershipTypeID == null) return;

        string selectedMembershipTypeName = null;
        string selectedMembershipFeeName = null ;

        // let's go through every repeater item and figure out the membership type and product
        foreach (RepeaterItem ri in rptMembershipTypes.Items)
        {
            HiddenField hfMembershipType = (HiddenField) ri.FindControl("hfMembershipType");

            if (hfMembershipType.Value != selectedMembershipTypeID)   // this isn't the one
                continue;

            // ok, this is the one
            var lblMembershipType = (Label)ri.FindControl("lblMembershipType");
            RadioButtonList rblSubCategory = (RadioButtonList)ri.FindControl("rblSubCategory");
            
            // this shouldn't happen, since we tested for blank items
            // when we rendered the membership type, and we didn't render any membership type
            // with no products - but defensive programming
            if ( rblSubCategory.Items.Count == 0 ) return;  

            // harvest the type name/fee name/fee id info
            selectedMembershipTypeName = lblMembershipType.Text;

            if ( rblSubCategory.SelectedItem == null ) rblSubCategory.SelectedIndex = 0;// use the first
            
            selectedMembershipFeeName = rblSubCategory.SelectedItem.Text;
            selectedMembershipFeeID = rblSubCategory.SelectedItem.Value;

            // ok, let's create the membership we use for the session
            msMembership mem = MultiStepWizards.RenewMembership.Membership;

            if ( mem == null )  // no previous membership
            {
                mem = new msMembership();
                MultiStepWizards.RenewMembership.Membership = mem;
            }
            mem.Type = selectedMembershipTypeID;
            mem.Product = selectedMembershipFeeID;
            mem["Type_Name"] = selectedMembershipTypeName;
            mem["Product_Name"] = selectedMembershipFeeName;
            mem.MembershipOrganization = drMembershipOrganization["ID"].ToString();
             
            // now, force this object to be stored in memory
            // by default, we'll store the ID, and not the entire object
            mem.SystemTimestamp = null;
            MultiStepWizards.RenewMembership.Membership = mem;

            MultiStepWizards.RenewMembership.Entity = targetEntity;

            // go to the next step
            GoTo("PurchaseMembership2.aspx");
 
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MultiStepWizards.RenewMembership.Clear();
        GoHome();
    }

    protected void rptMembershipTypes_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        msMembershipType mt = (msMembershipType)e.Item.DataItem;

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
                var lRadioButtonMarkup = (Literal)e.Item.FindControl("lRadioButtonMarkup");
                var lblMembershipType = (Label)e.Item.FindControl("lblMembershipType");
                Label lblPrice = (Label)e.Item.FindControl("lblPrice");
                Literal lMembershipDescription = (Literal)e.Item.FindControl("lMembershipDescription");
                RadioButtonList rblSubCategory = (RadioButtonList)e.Item.FindControl("rblSubCategory");
                HtmlTableRow trSubCategory = (HtmlTableRow)e.Item.FindControl("trSubCategory");
                HiddenField hfMembershipType = (HiddenField)e.Item.FindControl("hfMembershipType");
                lblMembershipType.Text = mt.Name;

                lMembershipDescription.Text = mt.Description;
                hfMembershipType.Value = mt.ID; // important, when we need to figure out the selected membership

                /* We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                  in repeater controls
                  http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->*/

                bool isSelected = mt.ID == selectedMembershipTypeID ;   // select if necessary
                
                
                string radioButtonID = "MembershipType" + e.Item.ItemIndex;

                
                
                lRadioButtonMarkup.Text = string.Format(
                    @"<input type=radio name=MembershipType " +
                    @"id={0} value='{1}' {2} onclick='showAppropriateSubCategory();' />", radioButtonID,
                    mt.ID,
                    isSelected ? "checked" : "");


                if (allProducts == null) return;

                

                var relatedMembershipProducts = allProducts.FindAll(x => x.MembershipType == mt.ID);
                var relatedProducts =
                    allDescribedProducts.Where(x => relatedMembershipProducts.Exists(y => y.ProductID == x.ProductID)).ToList();
             
                switch (relatedProducts.Count)
                {
                    case 0:
                        return; // should never happen, we filtered before this

                    case 1:
                        var singleProduct = relatedProducts[0];
                        lblPrice.Text = singleProduct.DisplayPriceAs ?? singleProduct.Price.ToString("C");

                        // let's still add it, b/c we need to reference this on the save
                        rblSubCategory.Items.Add(new ListItem(
                             string.Format("{0} - <span style='color: green'>{1}</span>", singleProduct.ProductName,
                             singleProduct.DisplayPriceAs ?? singleProduct.Price.ToString("C")), singleProduct.ProductID));

                        trSubCategory.Visible = false;
                        break;

                    default:
                        List<string> prices = (from p in allProducts
                                               join dp in allDescribedProducts on p.ProductID equals dp.ProductID
                                               where p.MembershipType == mt.ID 
                                               select
                                                   string.IsNullOrWhiteSpace(dp.DisplayPriceAs)
                                                       ? dp.Price.ToString("C")
                                                       : dp.DisplayPriceAs
                                              ).ToList();

                        //MS-2090
                        //If there is at least one string that starts with a price (like "$10", "15.99" or "$30/month")
                        //then use our custom price sorting algorithm
                        //Otherwise just sort alphabetically
                        IComparer<string> sorter = StringComparer.CurrentCultureIgnoreCase;
                        if(prices.Exists(PriceComparer.StartsWithPrice))
                            sorter = new PriceComparer();

                        prices.Sort(sorter);

                        if(prices.Count > 0)
                            lblPrice.Text = string.Format("{0}-{1}", prices.First(), prices.Last());

                        trSubCategory.Visible = true;

                        foreach( var p in relatedProducts )
                        {
                            ListItem listItem = new ListItem( 
                                string.Format( "{0} - <span style='color: green'>{1}</span>",  p.ProductName, p.DisplayPriceAs ?? p.Price.ToString("C")), p.ProductID );
                            if (p.ProductID == selectedMembershipFeeID)
                                listItem.Selected = true;

                            rblSubCategory.Items.Add( listItem);
                        }

                        rblSubCategory.SelectedIndex = 0;   // make sure the first item is selected

                        if (!isSelected)
                            trSubCategory.Attributes["style"] = "display: none;";   // hide it by default

                        // we also need to add javascript such that when this subcategory only appears
                        // when the parent radio button is selected
                        // so, since this is visible, we'll track it
                        RadioButtonSubCategoryPairs.Add(new NameValueStringPair(radioButtonID, trSubCategory.ClientID));
                        break;
                }





                break;
        }
    }

    #endregion
}