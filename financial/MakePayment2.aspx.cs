using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.WCF;

public partial class financial_MakePayment2 : PortalPage
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
        
        
        if (!IsPostBack)
        {


            using (var api = GetServiceAPIProxy())
                BillingInfoWidget.AllowableMethods = api.DetermineAllowableInvoicePaymentMethods(targetPayment).ResultValue;

          
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

        

        lblAmountDue.Text = targetPayment.Total.ToString("C");
        RegisterJavascriptConfirmationBox(lbCancel, "Are you sure you want to cancel this payment?");
        RegisterJavascriptConfirmationBox(btnContinue,
                                     "We are about to process your payment. Please confirm that you are ready to proceed.");
    }


    #endregion
    

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        var payment = BillingInfoWidget.GetPaymentInfo();
        targetPayment.PaymentMethod = payment.PaymentMethod;
        processPayment(payment);
        MultiStepWizards.MakePayment.Clear();
        QueueBannerMessage(string.Format("Your payment for {0:C} has been processed.", targetPayment.Total));

        GoHome();

    }

    protected void processPayment(ElectronicPaymentManifest payment)
    {

        decimal sumOfItems = targetPayment.LineItems.Sum(x => x.Amount);
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
            if (targetPayment.Total > 0)
            {
                
                PaymentProcessorResponse resp = api.ProcessCreditCardPayment(targetPayment, payment, antiDupeKey).ResultValue;

                if (!resp.Success)
                    // ok, we're throwing an exception 
                    throw new ConciergeClientException(
                        MemberSuite.SDK.Concierge.ConciergeErrorCode.CreditCardAuthorizationFailed,
                        "Unable to process payment: [{0}] - {1}", resp.GatewayResponseReasonCode, resp.GatewayResponseReasonText);

                targetPayment = LoadObjectFromAPI(resp.PaymentID).ConvertTo<msPayment>();
            }
            else
            {
                targetPayment.PaymentMethod = PaymentMethod.CustomerCredit;
                targetPayment = api.RecordPayment(targetPayment).ResultValue.ConvertTo<msPayment>();
            }


            // now, send a confirmation email
            api.SendEmail("BuiltIn:Payment", new List<string> { targetPayment.ID }, ConciergeAPI.CurrentUser.EmailAddress);
        }
    }


    protected void lbCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }
}