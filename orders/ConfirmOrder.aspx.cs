using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Manifests;
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

        if (originalOrder == null)
        {
            QueueBannerError("No order was found.");
            GoHome();
        }
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
        ClientScript.RegisterStartupScript(GetType(), "UpdatePayment", "if (window.updatePayment) updatePayment();", true);
    }

    protected override void InitializePage()
    {
        base.InitializePage();


        if (!populateOrder())
            return;


        // now - are there future billings?
        setupFutureBillings();


        setupShipping();
        setupBilling();
        setupConfirmationSections();

        btnPlaceOrder.Attributes.Add("onclick",
                                     "this.disabled=true;" +
                                     Page.ClientScript.GetPostBackEventReference(btnPlaceOrder, "").ToString());

    }

    private void setupBilling()
    {
        if (targetOrder.SafeGetValue<string>("BillingMethodFriendlyName") == null) // no billing
        {
            divBilling.Visible = false;
            return;
        }
        lblPaymentMethod.Text = targetOrder.SafeGetValue<string>("BillingMethodFriendlyName");

        if (targetOrder.BillingAddress != null)
            lblBillingAddress.Text = targetOrder.BillingAddress.ToHtmlString();

        lSavingInfo.Visible = targetOrder.SavePaymentMethod;
        hlChangeBilling.NavigateUrl += "?useTransient=" + Request.QueryString["useTransient"];
    }

    private void setupConfirmationSections()
    {
        // we have special setions for different kinds of confirmations
        OrderConfirmationPacket confirmationPacket = MultiStepWizards.PlaceAnOrder.OrderConfirmaionPacket;
        if (confirmationPacket == null || !confirmationPacket.ShouldShow()) return;

        // now, let's got through and try and cast the packet, and see which kind it is
        ExhibitorConfirmationPacket ep = confirmationPacket as ExhibitorConfirmationPacket;

        if (ep != null) setupExhibitorConfirmation(ep);
    }

    private void setupExhibitorConfirmation(ExhibitorConfirmationPacket ep)
    {
        divExhibitorConfirmation.Visible = true;

        using (var api = GetServiceAPIProxy())
            if (ep.BoothPreferences != null && ep.BoothPreferences.Count > 0)
            {
                divExhibitorConfirmation_BoothPreferences.Visible = true;

                StringBuilder sb = new StringBuilder();
                foreach (var b in ep.BoothPreferences)
                    sb.AppendFormat("{0}, ", api.GetName(b).ResultValue);

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
            lbCancel.Enabled = false; // you can't do this for a transient order - you have to proceed
            CancelOrderWrapper.Visible = false;

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
                if (billTo.Addresses != null && billTo.Addresses.Count > 0)
                {
                    var address = billTo.Addresses.FirstOrDefault(x => x.Type == billTo.PreferredAddressType) ??
                                  billTo.Addresses[0];

                    targetOrder.BillingAddress = address.Address;
                }
            }

            if (targetOrder.ShippingAddress == null && targetOrder.ShipTo != null)
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
                targetOrder.LineItems.AddRange(csi.FindAll(x=>x.Quantity != 0)); // add any cross sell items
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
        const string ContentSuffix = "_Contents";
   
        if (!IsValid)
            return;

        lock (threadLock)
        {
            if (targetOrder == null)
            {
                Refresh();
                return;
            }

            targetOrder.Notes = tbNotesComments.Text;

            // add cross sell
            var csi = MultiStepWizards.PlaceAnOrder.CrossSellItems;
            if (csi != null && csi.Count > 0)
                targetOrder.LineItems.AddRange( csi.FindAll(x => x.Quantity != 0)); // add any cross sell items

            using (var api = GetServiceAPIProxy())
            {
                var msPayload = new List<MemberSuiteObject>();
                // Go over line items and generate payoads for all attachments realted fields.
                foreach (var lineItem in targetOrder.LineItems)
                {
                    // We're looking for _Content only. _Content has to be of MemberSuiteFile type.
                    var attachments = lineItem.Fields.Where(f => f.Key.EndsWith(ContentSuffix) && IsNonEmptyMemberSuiteFile(f.Value))
                        .Select(c =>
                        {
                            var msf = (MemberSuiteFile)c.Value;
                            // Generate ID
                            var fileId = api.GenerateIdentifer("File").ResultValue;
                            // Create ms object...
                            var mso = new MemberSuiteObject();
                            mso.ClassType = "File";
                            mso.Fields["ID"] = fileId;
                            mso.Fields["FileContents"] = msf.FileContents;
                            mso.Fields["Name"] = msf.FileName;
                            mso.Fields["ContentLength"] = msf.FileContents.Length;

                            return new { Key = c.Key.Replace(ContentSuffix, string.Empty), FileId = fileId, File = mso };
                        });

                    if (attachments.Count() > 0)
                    {
                        foreach (var a in attachments)
                        {
                            // Add to current lineItem's Options field's name and according file Id.
                            lineItem.Options.Add(new NameValueStringPair { Name = a.Key, Value = a.FileId });
                            // Add according ms file to payload.
                            msPayload.Add(a.File);
                        }
                    }
                }

                OrderPayload payload = MultiStepWizards.PlaceAnOrder.Payload;

                if (msPayload.Count() > 0)
                {
                    if (payload == null)
                    {
                        payload = new OrderPayload();
                    }

                    if (payload.ObjectsToSave == null)
                    {
                        payload.ObjectsToSave = new List<MemberSuiteObject>();
                    }

                    payload.ObjectsToSave.AddRange(msPayload);
                }

                var processedOrderPacket = api.PreProcessOrder(targetOrder).ResultValue;
                cleanOrder = processedOrderPacket.FinalizedOrder.ConvertTo<msOrder>();
                cleanOrder.Total = processedOrderPacket.Total;

                if (string.IsNullOrWhiteSpace(cleanOrder.BillingEmailAddress))
                    cleanOrder.BillingEmailAddress = CurrentUser.EmailAddress;

                string antiDupeKey = (string)ViewState["AntiDupeKey"];


                string trackingKey;
                if (payload == null)
                    trackingKey = api.ProcessOrder(cleanOrder, antiDupeKey).ResultValue;
                else
                    trackingKey = api.ProcessOrderWithPayload(cleanOrder, payload, antiDupeKey).ResultValue;

                // let's wait for the order
                var log = OrderUtilities.WaitForOrderToComplete(api, trackingKey);

                if (log != null && log.Type == AuditLogType.OrderFailure)
                    throw new ConciergeClientException(MemberSuite.SDK.Concierge.ConciergeErrorCode.IllegalOperation,
                        log.Description);

                string url = MultiStepWizards.PlaceAnOrder.OrderCompleteUrl ?? "OrderComplete.aspx";
                if (url.Contains("?"))
                    url += "&";
                else
                    url += "?";


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




                MultiStepWizards.PlaceAnOrder.EditOrderLineItem = null; // clear this out
                MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductName = null; // clear this out
                MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductDemographics = null; // clear this out
                MultiStepWizards.PlaceAnOrder.OrderConfirmaionPacket = null;

                if (log == null)
                {
                    // MS-5204. Don't create job posting here. If JES is down then, job posting will be created 
                    // during order processing. Otherwise we'll endup with duplicate job postings.
                    // hack - let's save the job posting
                    //if (MultiStepWizards.PostAJob.JobPosting != null)
                    //    SaveObject(MultiStepWizards.PostAJob.JobPosting);

                    MultiStepWizards.PostAJob.JobPosting = null;

                    GoTo("OrderQueued.aspx");
                }

                var order = LoadObjectFromAPI<msOrder>(log.AffectedRecord_ID);
                QueueBannerMessage(string.Format("Order #{0} was processed successfully.",
                                                 order.SafeGetValue<long>(
                                                     msLocallyIdentifiableAssociationDomainObject.FIELDS.LocalID)));

                url += "orderID=" + order.ID;

                if (!url.Contains("contextID="))
                    url += "&contextID=" + order.ID;

                GoTo(url);
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
                if (!string.IsNullOrWhiteSpace(li.Description))
                    lblProductName.Text += " (" + li.Description + ")";

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

    private static bool IsNonEmptyMemberSuiteFile(object o)
    {
        var msf = o as MemberSuiteFile;
        if (msf == null)
        {
            return false;
        }

        if (msf.FileContents.Length == 0)
        {
            return false;
        }

        return true;
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