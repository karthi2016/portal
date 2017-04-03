using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using Telerik.Web.UI;

public partial class exhibits_RegisterForBooths : PortalPage 
{
    public msExhibitorRegistrationWindow targetWindow;
    public msEntity targetEntity;
    public msExhibitShow targetShow;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetWindow = LoadObjectFromAPI<msExhibitorRegistrationWindow>(ContextID);
        if (targetWindow == null) GoToMissingRecordPage();
        targetEntity = LoadObjectFromAPI<msEntity>(Request.QueryString["entityID"]);
        if (targetEntity == null) GoToMissingRecordPage();

        targetShow = LoadObjectFromAPI<msExhibitShow>(targetWindow.Show);
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        using (var api = GetServiceAPIProxy())
        {
            var ps =
                api.GetAvailableExhibitorRegistrationWindows(targetWindow.Show, targetEntity.ID).ResultValue.Permissions;
            if (ps.Count == 0 || ps[0].RegistrationMode != ExhibitorRegistrationMode.PurchaseBoothsByNumber)
                return false;
        }

        if (targetEntity.ID == ConciergeAPI.CurrentEntity.ID)
            return true;

        if (ConciergeAPI.AccessibleEntities == null)
            return false;

        bool found = false;
        foreach (var accessibleEntity in ConciergeAPI.AccessibleEntities)
        {
            if (accessibleEntity != null && accessibleEntity.ID != null &&
                accessibleEntity.ID.Equals(targetEntity.ID, StringComparison.CurrentCultureIgnoreCase))
                found = true;
        }

        return found;
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        lMainInstructions.Text = targetShow.RegistrationInstructions;
        lRegistrationWindowInstructions.Text = targetWindow.RegistrationInstructions;

        if (targetShow.ShowFloor != null)
            lShowFloor.NavigateUrl = GetImageUrl(targetShow.ShowFloor);
        else
            lShowFloor.Visible = false;

        using( var api = GetServiceAPIProxy() )
        {
            var booths = api.GetAvaialbleExhibitBooths(targetShow.ID, targetEntity.ID).ResultValue;

            foreach (var b in booths)
            {
                string nameOfBooth = b.BoothName;
                if (booths.Exists(x => x.BoothID == b.BoothID && b.BoothProductID != x.BoothProductID))   // there are multiple booth products
                    nameOfBooth = b.BoothProductName;

                string name = string.Format("{0} ({1}) - {2:C}",
                                           nameOfBooth, b.BoothTypeName, b.BoothCost);

                dlbCategories.Source.Items.Add(new Telerik.Web.UI.RadListBoxItem(name, b.BoothProductID ));
            }


        }

        bindExhibitorMerchandise();

        CustomTitle.Text = string.Format("{0} Registration", targetShow.Name);

    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        var boothProductsToPurchase = new List<string>();
        foreach (RadListBoxItem item in dlbCategories.Destination.Items)
            boothProductsToPurchase.Add(item.Value);

        if (boothProductsToPurchase.Count == 0)
        {
            cvAtLeastOneBooth.IsValid = false;
            return;
        }

        var o = unbindOrder(boothProductsToPurchase);

        var p = new ExhibitorConfirmationPacket
        {
            SpecialRequests = tbSpecialRequest.Text,
            ConfirmationInstructions = targetWindow.RegistrationConfirmationInstructions
        };
        MultiStepWizards.PlaceAnOrder.OrderConfirmationPacket = p;
        MultiStepWizards.PlaceAnOrder.OrderCompleteUrl = "/exhibits/ViewShow.aspx?contextID=" + targetShow.ID ;
        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(o);
    }

    private msOrder unbindOrder(List<string> boothProductsToPurchase)
    {
        // ok, let's create our order
        var o = new msOrder();
        o.ShipTo = o.BillTo = targetEntity.ID;
        o.LineItems = new List<msOrderLineItem>();

        foreach (var booth in boothProductsToPurchase)
        {
            var oli = new msOrderLineItem {Product = booth, Quantity = 1};
            oli.Options = new List<NameValueStringPair>();
            oli.Options.Add(new NameValueStringPair(OrderLineItemOptions.Exhibits.SpecialRequests, tbSpecialRequest.Text));
            o.LineItems.Add(oli);
        }

        unbindMerchandise(o);
        return o;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewShow.aspx?contextID=" + targetShow.ID);
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
        foreach (System.Data.DataRow dr in APIExtensions.GetSearchResult(s, 0, null).Table.Rows)
            products.Add(Convert.ToString(dr["ID"]));

        if (products.Count > 0)
        {
            divOtherProducts.Visible = true;
            using (var api = GetServiceAPIProxy())
            {
                var describedProducts = api.DescribeProducts(targetEntity.ID, products).ResultValue;


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
            li.Quantity = int.Parse(tbQuantity.Text);
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