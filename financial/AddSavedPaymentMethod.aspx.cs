using System;
using MemberSuite.SDK.Types;

public partial class financial_AddSavedPaymentMethod : CreditCardLogic
{

    protected msAssociationDomainObject targetObject;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetObject = LoadObjectFromAPI<msAssociationDomainObject>(ContextID);
        if (targetObject == null)
            GoToMissingRecordPage();

        using (var api = GetServiceAPIProxy())
            switch (targetObject.ClassType)
            {
                case msGift.CLASS_NAME:
                    BillingInfoWidget.AllowableMethods =
                        api.DetermineAllowableGiftPaymentMethods(targetObject).ResultValue;
                    break;

                default:
                    throw new NotSupportedException("Unable to update a record of type " + targetObject.ClassType);
            }

        hfOrderBillToId.Value = targetObject.SafeGetValue<string>(msGift.FIELDS.Donor);

        dvPriorityData.InnerHtml = GetPriorityPaymentsConfig(hfOrderBillToId.Value);
    }

    protected override void InitializePage()
    {
        base.InitializePage();
    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        goBackToReturnUrl();
    }

    private void goBackToReturnUrl()
    {
        GoTo(Request.QueryString["returnURL"] ?? "/");
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        var ePayment = BillingInfoWidget.GetPaymentInfo();


        if (ePayment == null)
            throw new ApplicationException("No payment type selected.");    // this should be caught during validation

        msSavedPaymentMethod spi;


        switch (ePayment.OrderPaymentMethod)
        {
            case OrderPaymentMethod.CreditCard:
                spi = CreateNewObject<msSavedCreditCard>();
                ((msSavedCreditCard)spi).Card = (CreditCard)ePayment;
                break;

            case OrderPaymentMethod.ElectronicCheck:
                spi = CreateNewObject<msSavedElectronicCheck>();
                ((msSavedElectronicCheck)spi).Check = (ElectronicCheck)ePayment;
                break;

            case OrderPaymentMethod.SavedPaymentMethod:
                string savedPaymentMethodID = ((SavedPaymentInfo)ePayment).SavedPaymentMethodID;

                msSavedPaymentMethod existingMethod = LoadObjectFromAPI<msSavedPaymentMethod>(savedPaymentMethodID);
                spi = existingMethod.Clone().ConvertTo<msSavedPaymentMethod>();
                spi.ID = spi.SystemTimestamp = null;

                break;

            default:
                throw new NotSupportedException("Unable to deal with order payment method " +
                                                ePayment.OrderPaymentMethod);
        }


        bool shouldSaveTargetObject = false;

        switch (targetObject.ClassType)
        {
            case msGift.CLASS_NAME:

                spi.Type = SavedPaymentMethodType.Gift;

                targetObject[msGift.FIELDS.DateOfInstallmentSuspension] = null; // clear the suspension
                shouldSaveTargetObject = true;
                break;

            default:
                throw new NotSupportedException("Unable to update a record of type " + targetObject.ClassType);
        }

        spi.Owner = null;   // always to this, so it's not linked to the entity

        using (var api = GetServiceAPIProxy())
        {
            string id = api.Save(spi).ResultValue.SafeGetValue<string>("ID");

            targetObject["SavedPaymentMethod"] = id;

            if (shouldSaveTargetObject)
                if (targetObject.ClassType == msGift.CLASS_NAME)
                    api.SaveDetails(targetObject);
                else
                    api.Save(targetObject);
        }

        QueueBannerMessage("Payment information has been updated successfully.");

        goBackToReturnUrl();


    }
}