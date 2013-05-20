using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;
using MemberSuite.SDK.WCF;

public partial class orders_ConfirmOrder : PortalPage
{
    private msOrder originalOrder;  // use with care
    private msOrder targetOrder;
    msOrder cleanOrder;
    private bool isTransient;
    private PreProcessedOrderPacket preProcessedOrderPacket;
    private object threadLock = new object();
    
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        if (string.Equals(Request.QueryString["useTransient"], "true", StringComparison.CurrentCultureIgnoreCase))
        {
            originalOrder = MultiStepWizards.PlaceAnOrder.TransientShoppingCart;
            isTransient = true;
        }
        else
            originalOrder = MultiStepWizards.PlaceAnOrder.ShoppingCart;

        //MS-2823
        if (originalOrder.BillTo == null)
            originalOrder.BillTo = ConciergeAPI.CurrentEntity.ID;

        if (originalOrder.ShipTo == null)
            originalOrder.ShipTo = originalOrder.BillTo;

        targetOrder = originalOrder.Clone().ConvertTo<msOrder>();

        // set the anti-dupe key
        ViewState["AntiDupeKey"] = Guid.NewGuid().ToString();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ClientScript.RegisterStartupScript(GetType(), "UpdatePayment", "updatePayment();", true);
    }
    protected override void InitializePage()
    {
        base.InitializePage();


        if (!populateOrder())
            return;

        acBillingAddress.Address = targetOrder.BillingAddress;
        acBillingAddress.DataBind();

        // now - are there future billings?
        setupFutureBillings();

        // ok, if this order has a total, we need to show the payment
        setupPaymentOptions();

        setupShipping();

        setupConfirmationSections();

        btnPlaceOrder.Attributes.Add("onclick", "this.disabled=true;" + Page.ClientScript.GetPostBackEventReference(btnPlaceOrder, "").ToString());
    }

    private void setupConfirmationSections()
    {
        // we have special setions for different kinds of confirmations
        OrderConfirmationPacket confirmationPacket = MultiStepWizards.PlaceAnOrder.OrderConfirmaionPacket;
        if (confirmationPacket == null || ! confirmationPacket.ShouldShow()) return;

        // now, let's got through and try and cast the packet, and see which kind it is
        ExhibitorConfirmationPacket ep = confirmationPacket as ExhibitorConfirmationPacket;

        if (ep != null) setupExhibitorConfirmation(ep);
    }

    private void setupExhibitorConfirmation(ExhibitorConfirmationPacket ep)
    {
        divExhibitorConfirmation.Visible = true;

        using( var api = GetServiceAPIProxy() )
        if (ep.BoothPreferences != null && ep.BoothPreferences.Count > 0)
        {
            divExhibitorConfirmation_BoothPreferences.Visible = true;

            StringBuilder sb = new StringBuilder();
            foreach (var b in ep.BoothPreferences)
                sb.AppendFormat("{0}, ", api.GetName(b).ResultValue );

            lblExhbitor_BoothPreferences.Text = sb.ToString().Trim().TrimEnd(',');
        }

        if (!string.IsNullOrWhiteSpace(ep.SpecialRequests))
            lblExhibitorSpecialRequests.Text = ep.SpecialRequests;
    }

    private bool populateOrder()
    {
        if (targetOrder == null || targetOrder.LineItems == null ||
            targetOrder.LineItems.Count == 0)
        {
            lblShoppingCartEmpty.Visible = true;
            btnPlaceOrder.Enabled = false;
            return false;
        }

        // let's populate the order

        if (isTransient)
        {
            btnContinue.Visible = false; // you can't do this for a transient order - you have to proceed
            lblContinueShoppingInstructions.Visible = false;
            hlChangeShippingMethod.NavigateUrl += "?useTransient=true";
        }

        // we have to preprocess the order to get the prices and such
        if (string.IsNullOrWhiteSpace(targetOrder.BillingEmailAddress))
            targetOrder.BillingEmailAddress = ConciergeAPI.CurrentEntity.EmailAddress;

        if (string.IsNullOrWhiteSpace(targetOrder.BillingEmailAddress))
            targetOrder.BillingEmailAddress = ConciergeAPI.CurrentUser.EmailAddress;

        using (var api = GetConciegeAPIProxy())
        {

            if (targetOrder.BillTo != null && targetOrder.BillingAddress == null)
            {
                msEntity billTo = api.Get(targetOrder.BillTo).ResultValue.ConvertTo<msEntity>();
                if(billTo.Addresses != null && billTo.Addresses.Count > 0)
                {
                    var address = billTo.Addresses.FirstOrDefault(x => x.Type == billTo.PreferredAddressType) ??
                                  billTo.Addresses[0];

                    targetOrder.BillingAddress = address.Address;
                }
            }

            if(targetOrder.ShippingAddress == null && targetOrder.ShipTo != null)
            {
                msEntity shipTo = api.Get(targetOrder.ShipTo).ResultValue.ConvertTo<msEntity>();
                if (shipTo.Addresses != null && shipTo.Addresses.Count > 0)
                {
                    var address = shipTo.Addresses.FirstOrDefault(x => x.Type == shipTo.PreferredAddressType) ??
                                  shipTo.Addresses[0];

                    targetOrder.ShippingAddress = address.Address;
                }
            }

            //if it's still null just set the shipping address to the billing address - whatever it is (null is acceptable)
            if (targetOrder.ShippingAddress == null)
                targetOrder.ShippingAddress = targetOrder.BillingAddress;

            var csi = MultiStepWizards.PlaceAnOrder.CrossSellItems;
            if (csi != null && csi.Count > 0)
            {
                targetOrder.LineItems.AddRange(csi); // add any cross sell items
                hlChangeRemoveAdditionalItems.Visible = true;
                hlChangeRemoveAdditionalItems.NavigateUrl += "?useTransient" + isTransient;
            }
        

            preProcessedOrderPacket = api.PreProcessOrder(targetOrder).ResultValue;
        }


        cleanOrder = preProcessedOrderPacket.FinalizedOrder.ConvertTo<msOrder>();

        // for display, we want to summarize shipping/taxes, so lets remove those items from the order
        List<msOrderLineItem> itemsToDisplay = new List<msOrderLineItem>(cleanOrder.LineItems);
        itemsToDisplay.RemoveAll(x => x.Type == OrderLineItemType.Shipping || x.Type == OrderLineItemType.Taxes || x.Type == OrderLineItemType.Discount);

        // let's hide the demographics warning
        divMissingDemographics.Visible = false;
        btnPlaceOrder.Enabled = true;

        gvShoppingCart.DataSource = itemsToDisplay;
        gvShoppingCart.DataBind();
        lblShoppingCartEmpty.Visible = false;

        lblShipping.Text = preProcessedOrderPacket.ShippingCharges.ToString("C");
        lblTaxes.Text = preProcessedOrderPacket.Taxes.ToString("C");
        lblTotal.Text = preProcessedOrderPacket.Total.ToString("C");
        lblDiscounts.Text = preProcessedOrderPacket.Discount.ToString("C");
        lblTotalDueNow.Text = preProcessedOrderPacket.AmountDueNow.ToString("C");

        return true;
    }

    private void setupShipping()
    {
        if (targetOrder.ShippingMethod == null) return;   // no shipping

        divShipping.Visible = true;
        lShippingMethod.Text = LoadObjectFromAPI<msShippingMethod>(targetOrder.ShippingMethod).Name;

        if (cleanOrder.ShippingAddress != null)
            lShipTo.Text = cleanOrder.ShippingAddress.ToHtmlString();
    }

    private void setupFutureBillings()
    {
        if (preProcessedOrderPacket.FutureBillings == null || preProcessedOrderPacket.FutureBillings.Count <= 0) return;

        trTotalDueNow.Visible = true;
        divFutureBillings.Visible = true;
        dlFutureBillings.DataSource = preProcessedOrderPacket.FutureBillings;

        dlFutureBillings.DataBind();
    }

    private void setupPaymentOptions()
    {
        divPayment.Visible = preProcessedOrderPacket.Total > 0;

        if (!preProcessedOrderPacket.CustomerCanPayLater)    // disable that option
        {
            rbPaymentPayLater.Text += " (not available)";
            rbPaymentPayLater.Enabled = false;
        }
    }

    protected void btnContinueShopping_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MultiStepWizards.PlaceAnOrder.ContinueShoppingUrl))
        {
            GoHome();
            return;
        }

        GoTo(MultiStepWizards.PlaceAnOrder.ContinueShoppingUrl);
    }

    protected void btnPlaceOrder_Click(object sender, EventArgs e)
    {
        // we have to disable the validators so the "Validate" command doesn't fire improporly
        if (!rbPaymentCreditCard.Checked)
        {
            rfvCCNameOnCard.Enabled = false;
            rfvCreditCardNumber.Enabled = false;
            rfvCardSecurity.Enabled = false;
            rfvPromoCode.Enabled = false;
            Validate();

            // now re-enable
            rfvCCNameOnCard.Enabled = true;
            rfvCreditCardNumber.Enabled = true;
            rfvCardSecurity.Enabled = true;
            rfvPromoCode.Enabled = true;
        }

        if (!IsValid)
            return;

        lock (threadLock)
        {
            if (targetOrder == null)
            {
                Refresh();
                return;
            }


            // add cross sell
            var csi = MultiStepWizards.PlaceAnOrder.CrossSellItems;
            if (csi != null && csi.Count > 0)
                targetOrder.LineItems.AddRange(csi); // add any cross sell items
                
            using (var api = GetServiceAPIProxy())
            {
                var processedOrderPacket = api.PreProcessOrder(targetOrder).ResultValue;
                cleanOrder = processedOrderPacket.FinalizedOrder.ConvertTo<msOrder>();
                cleanOrder.Total = processedOrderPacket.Total;
                if (rbPaymentCreditCard.Visible && rbPaymentCreditCard.Checked)
                {
                    cleanOrder.CreditCardNumber = tbCreditCardNumber.Text;
                    cleanOrder.PaymentMethod = OrderPaymentMethod.CreditCard;
                    cleanOrder.CreditCardExpirationDate = myExpiration.Date;
                }

                if (rbPaymentPayLater.Visible && rbPaymentPayLater.Checked)
                    cleanOrder.PurchaseOrderNumber = tbPurchaseOrder.Text;

                if (acBillingAddress.Visible)
                    cleanOrder.BillingAddress = acBillingAddress.Address;

                if (string.IsNullOrWhiteSpace(cleanOrder.BillingEmailAddress))
                    cleanOrder.BillingEmailAddress = CurrentUser.EmailAddress;

                string antiDupeKey = (string)ViewState["AntiDupeKey"];
                
                var trackingKey = api.ProcessOrder(cleanOrder, antiDupeKey).ResultValue;

                // let's wait for the order
                var log = OrderUtilities.WaitForOrderToComplete(api, trackingKey);

                if (log != null && log.Type == AuditLogType.OrderFailure)
                    throw new ConciergeClientException(MemberSuite.SDK.Concierge.ConciergeErrorCode.IllegalOperation,
                        log.Description);

                targetOrder = null;

                // clear the cart
                if (isTransient)
                {
                    // clear out the items
                    if (MultiStepWizards.PlaceAnOrder.TransientShoppingCart != null)
                        MultiStepWizards.PlaceAnOrder.TransientShoppingCart.LineItems.Clear();

                    MultiStepWizards.PlaceAnOrder.TransientShoppingCart = null;
                }
                else
                {
                    MultiStepWizards.PlaceAnOrder.ShoppingCart = null;
                    MultiStepWizards.PlaceAnOrder.RecentlyAddedItems = null;
                }

                MultiStepWizards.PlaceAnOrder.CrossSellItems = null;

            
                // now try to save the objects
                if (MultiStepWizards.PlaceAnOrder.ObjectsToSave != null)
                {
                    foreach (var mso in MultiStepWizards.PlaceAnOrder.ObjectsToSave)
                        SaveObject(mso);

                    MultiStepWizards.PlaceAnOrder.ObjectsToSave = null;
                }

                MultiStepWizards.PlaceAnOrder.EditOrderLineItem = null; // clear this out
                MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductName = null; // clear this out
                MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductDemographics = null; // clear this out
                MultiStepWizards.PlaceAnOrder.OrderConfirmaionPacket = null;

                if (log == null)
                    GoTo("OrderQueued.aspx");

                var order = LoadObjectFromAPI<msOrder>(log.AffectedRecord_ID);
                QueueBannerMessage(string.Format("Order #{0} was processed successfully.",
                                                 order.SafeGetValue<long>(
                                                     msLocallyIdentifiableAssociationDomainObject.FIELDS.LocalID)));


                GoTo("OrderComplete.aspx?contextID=" + order.ID );
            }
        }


    }
    protected void gvShoppingCart_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        msOrderLineItem li = (msOrderLineItem)e.Row.DataItem;


        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                TextBox tbQuantity = (TextBox)e.Row.FindControl("tbQuantity");
                CompareValidator cvQuantity = (CompareValidator)e.Row.FindControl("cvQuantity");
                Label lblProductName = (Label)e.Row.FindControl("lblProductName");
                Label lblProductPrice = (Label)e.Row.FindControl("lblProductPrice");
                Label lblProductType = (Label)e.Row.FindControl("lblProductType");
                var lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                var imgWarning = (Image)e.Row.FindControl("imgWarning");

                tbQuantity.Text = li.Quantity.ToString("F0");

                string productName = preProcessedOrderPacket.ProductNames[e.Row.RowIndex];
                cvQuantity.ErrorMessage = string.Format("The quantity you have specified for '{0}' is invalid.",
                    productName);
                lblProductName.Text = productName;

                lblProductPrice.Text = li.Total.ToString("C");
                lblProductType.Text = preProcessedOrderPacket.ProductTypes[e.Row.RowIndex];

                // ok - so can this product be edited?
                lbEdit.Visible = canProductBeEdited(e.Row.RowIndex);
                lbEdit.CommandArgument = e.Row.RowIndex.ToString();

                if (lbEdit.Visible)   // are there required demographics that are missing?
                    checkForMissingRequiredDemographics(li, imgWarning, e.Row.RowIndex);

                break;
        }
    }

    private void checkForMissingRequiredDemographics(msOrderLineItem item, Image imgWarning, int j)
    {
        // now - IMPORTANT - are there any missing demographics?
        var pp = preProcessedOrderPacket;
        if (pp == null || pp.ProductDemographics == null || pp.ProductDemographics.Count <= j) return;
        var list = pp.ProductDemographics[j];
        if (list == null) return;
        var requiredDemographics = list.FindAll(x => x.IsRequired || x.IsRequiredInPortal);

        foreach (var rd in requiredDemographics)
            if (item.SafeGetValue(rd.Name) == null) // we have a problem
            {
                divMissingDemographics.Visible = true;
                imgWarning.Visible = true;
                btnPlaceOrder.Enabled = false;
                break;
            }
    }

    private bool canProductBeEdited(int rowIndex)
    {
        return (preProcessedOrderPacket != null &&
                preProcessedOrderPacket.ProductDemographics != null &&
                preProcessedOrderPacket.ProductDemographics.Count > rowIndex &&
                preProcessedOrderPacket.ProductDemographics[rowIndex].Count > 0);
    }

    protected void btnApplyDiscountCode_Click(object sender, EventArgs e)
    {


        msOrder order = targetOrder;
        if (order == null) return;


        string discountCode = tbPromoCode.Text.ToUpper().Trim();
        tbPromoCode.Text = "";


        if (string.IsNullOrWhiteSpace(discountCode)) return;

        string discountCodeID = retrieveIDForDiscountCode(discountCode);

        if (discountCodeID == null)
        {
            DisplayBannerMessage(string.Format("Discount code '{0}' was not found.",
                                                                            discountCode), true);
            return;
        }
        
        
        // put it both in the original order, and the cloned order
        if (originalOrder.DiscountCodes == null)
            originalOrder.DiscountCodes = new List<msOrderDiscountCode>();

        if (order.DiscountCodes == null)
            order.DiscountCodes = new List<msOrderDiscountCode>();

        order.DiscountCodes.Add(new msOrderDiscountCode { DiscountCode = discountCodeID } );
        originalOrder.DiscountCodes.Add(new msOrderDiscountCode { DiscountCode = discountCodeID });

        populateOrder();

        setupPaymentOptions();

        // if the discount code was removed, it means it was invalid


        if (cleanOrder.DiscountCodes == null || !cleanOrder.DiscountCodes.Exists( x=> x.DiscountCode == discountCodeID))
            DisplayBannerMessage(string.Format("Discount code '{0}' is not applicable.", discountCode), true);
        else
            DisplayBannerMessage(string.Format("Discount code '{0}' was applied successfully.", discountCode), false);



    }

    private string retrieveIDForDiscountCode(string discountCode)
    {
        Search s = new Search(msDiscountCode.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Code", discountCode));
        using (var api = GetServiceAPIProxy())
        {
            var result = api.ExecuteSearch(s, 0, 1).ResultValue;
            if (result.TotalRowCount > 0)
                return Convert.ToString(result.Table.Rows[0]["ID"]);
        }

        return null;
    }

    protected void gvShoppingCart_OnRowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Edit":
                editOrderLineItem(int.Parse((string)e.CommandArgument));
                break;
        }
    }

    private void editOrderLineItem(int index)
    {
        populateOrder();
        MultiStepWizards.PlaceAnOrder.EditOrderLineItem = targetOrder.LineItems[index];
        MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductName = preProcessedOrderPacket.ProductNames[index];
        MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductDemographics = preProcessedOrderPacket.ProductDemographics[index];
        MultiStepWizards.PlaceAnOrder.EditOrderLineItemRedirectUrl = Request.RawUrl;
        GoTo("EditOrderLineItem.aspx");
    }
}