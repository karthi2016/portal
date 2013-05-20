using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class exhibits_PurchaseAdditionalExhibitorProducts : PortalPage 
{
    public msExhibitor targetExhibitor;
    public msExhibitShow targetShow;
     
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetExhibitor = LoadObjectFromAPI<msExhibitor>(ContextID);
        if (targetExhibitor == null) GoToMissingRecordPage();

        targetShow = LoadObjectFromAPI<msExhibitShow>(targetExhibitor.Show);
        if (targetShow == null) GoToMissingRecordPage();
 
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        using (var api = GetConciegeAPIProxy())
            return ExhibitorLogic.CanViewExhibitorRecord(api, targetExhibitor, ConciergeAPI.CurrentEntity.ID);
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        bindExhibitorMerchandise();
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (! Page.IsValid)
            return;

        var o = unbindOrder();

        if (o.LineItems.Count == 0)
        {
            cvAtLeastOne.IsValid = false;
            return;
        }

        MultiStepWizards.PlaceAnOrder.TransientShoppingCart = o;
      MultiStepWizards.PlaceAnOrder.OrderCompleteUrl = "/exhibits/ViewExhibitor.aspx?contextID=" + targetExhibitor.ID;

      MultiStepWizards.PlaceAnOrder.InitiateOrderProcess( o );
      
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewExhibitor.aspx?contextID=" + ContextID);
    }

    private msOrder unbindOrder( )
    {
        // ok, let's create our order
        msOrder o = new msOrder();
        o.ShipTo = o.BillTo = targetExhibitor.Customer;
        o.LineItems = new List<msOrderLineItem>();

        
        unbindMerchandise(o);
        return o;
    }


    private void bindExhibitorMerchandise()
    {
        Search s = new Search(msExhibitorMerchandise.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Show", targetShow.ID));
        s.AddCriteria(Expr.Equals("IsActive", true));
        s.AddCriteria(Expr.Equals("SellOnline", true));

        s.AddSortColumn("DisplayOrder");
        s.AddSortColumn("Name");

        List<string> products = new List<string>();
        foreach (System.Data.DataRow dr in ExecuteSearch(s, 0, null).Table.Rows)
            products.Add(Convert.ToString(dr["ID"]));

        if (products.Count > 0)
        {
            divOtherProducts.Visible = true;
            using (var api = GetServiceAPIProxy())
            {
                var describedProducts = api.DescribeProducts(targetExhibitor.Customer, products).ResultValue;


                rptAdditionalItems.DataSource = describedProducts;
                rptAdditionalItems.DataBind();
            }
        }


    }

    private void unbindMerchandise(msOrder mso)
    {

        if (!divOtherProducts.Visible)
            return;

        foreach (RepeaterItem ri in rptAdditionalItems.Items)
        {
            TextBox tbQuantity = (TextBox)ri.FindControl("tbQuantity");
            HiddenField hfProductID = (HiddenField)ri.FindControl("hfProductID");

            msOrderLineItem li = new msOrderLineItem();
            li.Quantity = decimal.Parse(tbQuantity.Text);
            if (li.Quantity <= 0)
                continue; // don't add

            li.Product = hfProductID.Value;

            mso.LineItems.Add(li);

        }
    }

    protected void rptAdditionalItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
}