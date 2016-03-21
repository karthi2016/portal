using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class financial_AddElectronicCheck : CreditCardLogic
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

        msSavedElectronicCheck check = new msSavedElectronicCheck();
        check.Owner = ConciergeAPI.CurrentEntity.ID;
        check.Check = new ElectronicCheck();
        check.Check.BankAccountNumber = tbBankAccountNumber.Text;
        check.Check.RoutingNumber = tbRoutingNumber.Text;

        SaveObject(check);

        GoTo("ManagePaymentOptions.aspx", "Electronic checking account information has been saved successfully.");

    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        GoTo("ManagePaymentOptions.aspx");
    }
}