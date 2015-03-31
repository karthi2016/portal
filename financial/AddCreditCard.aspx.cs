using System;
using MemberSuite.SDK.Types;

public partial class financial_AddCreditCard : CreditCardLogic
{
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        hfOrderBillToId.Value = ConciergeAPI.CurrentEntity.ID;

        dvPriorityData.InnerHtml = GetPriorityPaymentsConfig(hfOrderBillToId.Value);
    }

    protected void btnSaveCard_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        msSavedCreditCard scc = new msSavedCreditCard();

        scc.Card = new CreditCard();
        scc.Card.CardNumber = tbCardNumber.Text;
        scc.Card.NameOnCard = tbNameOnCard.Text;
        scc.Card.CCVCode = tbCCV.Text;
        scc.Card.CardExpirationDate = myExpiration.Date.Value;

        scc.Owner = ConciergeAPI.CurrentEntity.ID;

        SaveObject(scc);

        GoTo("ManagePaymentOptions.aspx", "Credit card has been saved successfully.");
    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        GoTo("ManagePaymentOptions.aspx");
    }
}