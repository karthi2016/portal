using System;
using System.Collections.Generic;
using System.Linq;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class orders_EnterBillingInfo : CreditCardLogic
{

    #region Fields

    private msOrder targetOrder;
    private bool IsTransient
    {
        get { return string.Equals(Request.QueryString["useTransient"], "true", StringComparison.CurrentCultureIgnoreCase); }
    }
    private PreProcessedOrderPacket preProcessedOrderPacket;


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
        hfOrderBillToId.Value = ConciergeAPI.CurrentEntity.ID;

        dvPriorityData.InnerHtml = GetPriorityPaymentsConfig(hfOrderBillToId.Value);

        base.InitializeTargetObject();

        if (IsTransient)
            targetOrder = MultiStepWizards.PlaceAnOrder.TransientShoppingCart;
        else
            targetOrder = MultiStepWizards.PlaceAnOrder.ShoppingCart;

        if (targetOrder == null)
        {
            QueueBannerError("Unable to checkout without an active shopping cart.");
            GoHome();
            return;
        }

        //MS-2823
        if (targetOrder.BillTo == null)
            targetOrder.BillTo = hfOrderBillToId.Value;

        if (targetOrder.ShipTo == null)
            targetOrder.ShipTo = targetOrder.BillTo;


        if (!IsPostBack)
        {
            using (var api = GetServiceAPIProxy())
                BillingInfoWidget.AllowableMethods = api.DetermineAllowableOrderPaymentMethods(targetOrder).ResultValue;

            // let's set default payment info
            switch (targetOrder.PaymentMethod)
            {
                case OrderPaymentMethod.None:
                    break; // do nothing

                case OrderPaymentMethod.CreditCard:
                    CreditCard cc = new CreditCard();
                    // Do NOT keep the credit card information on the page unpon refreshes
                    cc.SavePaymentMethod = targetOrder.SavePaymentMethod;
                    cc.NameOnCard = targetOrder.SafeGetValue<string>("NameOnCreditCard");
                    BillingInfoWidget.SetPaymentInfo(cc);
                    break;

                case OrderPaymentMethod.ElectronicCheck:
                    ElectronicCheck ec = new ElectronicCheck();
                    ec.BankAccountNumber = targetOrder.ACHAccountNumber;
                    ec.RoutingNumber = targetOrder.ACHRoutingNumber;
                    ec.SavePaymentMethod = targetOrder.SavePaymentMethod;
                    BillingInfoWidget.SetPaymentInfo(ec);
                    break;

                case OrderPaymentMethod.SavedPaymentMethod:
                    SavedPaymentInfo spi = new SavedPaymentInfo();
                    spi.SavedPaymentMethodID = targetOrder.SavedPaymentMethod;
                    BillingInfoWidget.SetPaymentInfo(spi);
                    break;

                default:
                    NonElectronicPayment nep = new NonElectronicPayment();
                    nep._OrderPaymentMethod = targetOrder.PaymentMethod;
                    nep.ReferenceNumber = targetOrder.PaymentReferenceNumber;
                    BillingInfoWidget.SetPaymentInfo(nep);
                    break;


            }

            BillingInfoWidget.SetBillingAddress(targetOrder.BillingAddress);
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

        // let's preprocess and figure out whether we need shipping information
        var completeOrder = targetOrder.Clone().ConvertTo<msOrder>();
        var csi = MultiStepWizards.PlaceAnOrder.CrossSellItems;
        if (csi != null && csi.Count > 0)
            completeOrder.LineItems.AddRange(csi.FindAll(x => x.Quantity != 0)); // add any cross sell items

        using (var api = GetConciegeAPIProxy())
        {
            preProcessedOrderPacket = api.PreProcessOrder(completeOrder).ResultValue;
        }


        // no billing, but we want to test Total, and not AmountDueNow
        // because even if nothing is due now we need to capture credit card info
        if (preProcessedOrderPacket.Total == 0)
            GoTo("ConfirmOrder.aspx?useTransient=" + IsTransient);



        lblAmountDue.Text = preProcessedOrderPacket.AmountDueNow.ToString("C");
        RegisterJavascriptConfirmationBox(lbCancel, "Are you sure you want to cancel this order?");
    }


    #endregion

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;


        var ePayment = BillingInfoWidget.GetPaymentInfo();


        if (ePayment == null)
            throw new ApplicationException("No payment type selected.");    // this should be caught during validation

        targetOrder.PaymentMethod = ePayment.OrderPaymentMethod;

        switch (targetOrder.PaymentMethod)
        {
            case OrderPaymentMethod.CreditCard:

                CreditCard cc = ePayment as CreditCard;
                if (cc == null)
                    throw new ApplicationException(
                        "Payment is of type credit card, but no credit card manifest provided");
                targetOrder.CreditCardNumber = cc.CardNumber;
                targetOrder.CreditCardExpirationDate = cc.CardExpirationDate;
                targetOrder.CCVCode = cc.CCVCode;
                targetOrder["NameOnCreditCard"] = cc.NameOnCard;
                break;

            case OrderPaymentMethod.ElectronicCheck:

                ElectronicCheck ec = ePayment as ElectronicCheck;
                if (ec == null)
                    throw new ApplicationException(
                        "Payment is of type electronic check, but no check  manifest provided");

                targetOrder.ACHAccountNumber = ec.BankAccountNumber;
                targetOrder.ACHRoutingNumber = ec.RoutingNumber;
                break;

            case OrderPaymentMethod.SavedPaymentMethod:
                SavedPaymentInfo spi = ePayment as SavedPaymentInfo;
                if (spi == null)
                    throw new ApplicationException(
                        "Payment is of type saved method, but not method manifest provided");

                targetOrder.SavedPaymentMethod = spi.SavedPaymentMethodID;
                break;



        }

        targetOrder["BillingMethodFriendlyName"] = ePayment.ToString();

        targetOrder.SavePaymentMethod = ePayment.SavePaymentMethod;

        targetOrder.BillingAddress = BillingInfoWidget.GetBillingAddress();
        targetOrder.PurchaseOrderNumber = BillingInfoWidget.GetReferenceNumber();

        GoTo("ConfirmOrder.aspx?useTransient=" + IsTransient);

    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        if (IsTransient)
        {
            MultiStepWizards.PlaceAnOrder.TransientShoppingCart = null;
            MultiStepWizards.PlaceAnOrder.CrossSellItems = null;

        }

        targetOrder.DiscountCodes = null;   // important to remove

        GoHome();
    }

    protected void btnApplyDiscountCode_Click(object sender, EventArgs e)
    {
        var order = targetOrder;
        if (order == null)
            return;

        string discountCode = tbPromoCode.Text.ToUpper().Trim();
        tbPromoCode.Text = "";

        if (string.IsNullOrWhiteSpace(discountCode))
            return;

        var discountCodeID = retrieveIDForDiscountCode(discountCode);
        if (discountCodeID == null)
        {
            DisplayBannerMessage(true, "Discount code '{0}' was not found.", discountCode);
            return;
        }

        // put it both in the original order, and the cloned order
        if (order.DiscountCodes == null)
            order.DiscountCodes = new List<msOrderDiscountCode>();

        order.DiscountCodes.Add(new msOrderDiscountCode { DiscountCode = discountCodeID });

        using (var api = GetServiceAPIProxy())
        {
            var preProcessedOrderPacket = api.PreProcessOrder(targetOrder);
            if (preProcessedOrderPacket.Errors.Any())
            {
                DisplayBannerMessage(true, "Error processing the order: {0}", string.Join(", ", preProcessedOrderPacket.Errors.Select(x => x.Message)));
                return;
            }

            // if the discount code was removed, it means it was invalid
            var cleanOrder = preProcessedOrderPacket.ResultValue.FinalizedOrder.ConvertTo<msOrder>();
            var applicable = cleanOrder.DiscountCodes != null && cleanOrder.DiscountCodes.Any(x => x.DiscountCode == discountCodeID);
            var message = applicable ? "was applied successfully" : "is not applicable";
            QueueBannerMessage("Discount code '{0}' {1}.", discountCode, message);
        }

        Refresh();
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
}