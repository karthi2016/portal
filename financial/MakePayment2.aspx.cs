using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class financial_MakePayment2 : CreditCardLogic
{
    #region Fields

    private msPayment targetPayment;
    protected ElectronicPaymentManifest paymentInfo;

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

        targetPayment = MultiStepWizards.MakePayment.Payment;

        hfOrderBillToId.Value = ConciergeAPI.CurrentEntity.ID;

        dvPriorityData.InnerHtml = GetPriorityPaymentsConfig(hfOrderBillToId.Value);

        if (!IsPostBack)
        {
            using (var api = GetServiceAPIProxy())
                BillingInfoWidget.AllowableMethods = api.DetermineAllowableInvoicePaymentMethods(targetPayment).ResultValue;

            BillingInfoWidget.SetBillingAddress(new Address());
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

        if (targetPayment == null)
            GoToMissingRecordPage();


        lblAmountDue.Text = targetPayment.Total.ToString("C");
        RegisterJavascriptConfirmationBox(lbCancel, "Are you sure you want to cancel this payment?");
        RegisterJavascriptConfirmationBox(btnContinue,
                                     "We are about to process your payment. Please confirm that you are ready to proceed.");
    }


    #endregion

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
        {
            var tbCardNumber = BillingInfoWidget.FindControl("tbCardNumber") as TextBox;
            if (tbCardNumber != null)
                tbCardNumber.Text = string.Empty;
            return;
        }

        var payment = BillingInfoWidget.GetPaymentInfo();
        targetPayment.PaymentMethod = payment.PaymentMethod;
        // MSIV-330
        targetPayment.Total = targetPayment.LineItems.Sum(x => x.Total);

        processPayment(payment);

        MultiStepWizards.MakePayment.Clear();

        // Clear any cached Membership checks since a payment could trigger a Membership Update.
        MembershipLogic.ClearMemberCaches();

        QueueBannerMessage(string.Format("Your payment for {0:C} has been processed.", targetPayment.Total));

        GoHome();
    }

    protected void processPayment(ElectronicPaymentManifest payment)
    {
        decimal sumOfItems = targetPayment.LineItems.Sum(x => x.Total);
        decimal overpayment = targetPayment.Total - sumOfItems;

        // add the overpayment
        if (overpayment > 0)
        {
            // not allow
            throw new ConciergeClientException(MemberSuite.SDK.Concierge.ConciergeErrorCode.IllegalOperation, "The sum of the invoice payments is less than the total payment amount. Please make sure all of the money in your payment is applied to invoices. ");
            //targetPayment.LineItems.Add(new msPaymentLineItem
            //                              {Amount = overpayment, Type = PaymentLineItemType.OverPayment});
        }

        string antiDupeKey = (string)Request["AntiDupeKey"];
        using (var api = GetServiceAPIProxy())
        {
            
                var merchantAccount = DetermineMerchantAccount();
                targetPayment.CashAccount = merchantAccount;
                var r = api.ProcessPayment(targetPayment, payment , null );
                if (!r.Success)
                    throw new ConciergeClientException(MemberSuite.SDK.Concierge.ConciergeErrorCode.GeneralException, r.FirstErrorMessage);
                PaymentProcessorResponse resp = r.ResultValue;

                if (!resp.Success)
                    // ok, we're throwing an exception 
                    throw new ConciergeClientException(
                        MemberSuite.SDK.Concierge.ConciergeErrorCode.CreditCardAuthorizationFailed,
                        "Unable to process payment: [{0}] - {1}", resp.GatewayResponseReasonCode, resp.GatewayResponseReasonText);

                targetPayment = LoadObjectFromAPI<msPayment>(resp.PaymentID);
            

            // now, send a confirmation email
            api.SendTransactionalEmail(EmailTemplates.Financial.Payment, targetPayment.ID , ConciergeAPI.CurrentUser.EmailAddress);
        }
    }

    private string DetermineMerchantAccount()
    {
        var businessUnit = ElectronicPaymentLogic.GetDefaultBusinessUnit();
        if (businessUnit == null)
            throw new ConciergeClientException(MemberSuite.SDK.Concierge.ConciergeErrorCode.RecordNotFound,
                                    "Default Business Unit has not been set.");

        var merchantAccount = ElectronicPaymentLogic.GetDefaultMerchantAccount(businessUnit.ID);
        if (merchantAccount == null)
            throw new ConciergeClientException(MemberSuite.SDK.Concierge.ConciergeErrorCode.RecordNotFound,
                                    "Default Merchant Account has not been set.");

        return merchantAccount.ID;
    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }
}
