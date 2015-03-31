using System;
using System.Collections.Generic;
using MemberSuite.SDK.Types;

public partial class orders_UpdateBillingInformation : CreditCardLogic
{
    protected msAssociationDomainObject targetObject;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetObject = LoadObjectFromAPI<msAssociationDomainObject>(ContextID);


        // determine allowed payments
        PortalPaymentMethods methods;

        using (var api = GetServiceAPIProxy())
            switch (targetObject.ClassType)
            {
                case msMembership.CLASS_NAME:
                    msOrder targetOrder = new msOrder();
                    targetOrder.BillTo = targetOrder.ShipTo = targetObject.SafeGetValue<string>("Owner");
                    targetOrder.LineItems = new List<msOrderLineItem>();
                    targetOrder.LineItems.Add(new msOrderLineItem
                    {
                        Product = targetObject.SafeGetValue<string>(msMembership.FIELDS.Product)
                    });

                    methods = api.DetermineAllowableOrderPaymentMethods(targetOrder).ResultValue;

                    break;

                case msSubscription.CLASS_NAME:
                    msOrder targetOrder2 = new msOrder();
                    targetOrder2.BillTo = targetOrder2.ShipTo = targetObject.SafeGetValue<string>("Owner");
                    targetOrder2.LineItems = new List<msOrderLineItem>();
                    targetOrder2.LineItems.Add(new msOrderLineItem
                    {
                        Product = targetObject.SafeGetValue<string>(msSubscription.FIELDS.Fee)
                    });

                    methods = api.DetermineAllowableOrderPaymentMethods(targetOrder2).ResultValue;

                    break;

                case msGift.CLASS_NAME:
                    methods = api.DetermineAllowableGiftPaymentMethods(targetObject).ResultValue;
                    break;

                default:
                    throw new NotSupportedException("Cannot deal with class type " + targetObject.ClassType);

            }
        // some payments NEVER make sense in this context
        methods.AllowBillMeLater = false;
        BillingInfoWidget.AllowableMethods = methods;

        hfOrderBillToId.Value = ConciergeAPI.CurrentEntity.ID;

        dvPriorityData.InnerHtml = GetPriorityPaymentsConfig(hfOrderBillToId.Value);
    }

    protected override void setupHyperLinks()
    {
        base.setupHyperLinks();

        hlCancel.NavigateUrl = getReferringScreen();
    }

    protected void btnUpdatePaymentInfo_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        var manifest = this.BillingInfoWidget.GetPaymentInfo();

        using (var api = GetServiceAPIProxy())
            api.UpdateBillingInfo(ContextID, manifest);

        QueueBannerMessage("Payment information has been successfully updated.");
        GoTo(getReferringScreen());
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        RegisterJavascriptConfirmationBox(lbClearPaymentInf,
                                          "Are you sure you want to clear the payment information from this record?");
    }

    protected void lbClearPaymentInfo_Click(object sender, EventArgs e)
    {
        using (var api = GetServiceAPIProxy())
            api.UpdateBillingInfo(ContextID, null);

        QueueBannerMessage("Payment information has been successfully removed from the record.");
        GoTo(getReferringScreen());
    }

    private string getReferringScreen()
    {
        switch (targetObject.ClassType)
        {
            case msMembership.CLASS_NAME:
                return string.Format("/membership/ViewMembership.aspx?contextID={0}", ContextID);

            case msGift.CLASS_NAME:
                return string.Format("/donations/ViewGift.aspx?contextID={0}", ContextID);

            case msSubscription.CLASS_NAME:
                return string.Format("/subscriptions/ViewSubscription.aspx?contextID={0}", ContextID);

            default:
                throw new NotSupportedException("Cannot deal with class type " + targetObject.ClassType);
        }
    }
}