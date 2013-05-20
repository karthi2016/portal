using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class orders_OrderComplete : PortalPage 
{

    protected msOrder targetOrder;
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetOrder = LoadObjectFromAPI<msOrder>(ContextID);
        if (targetOrder == null) GoToMissingRecordPage();

        lblConfirmationText.Text = getConfirmationText();

        if(MultiStepWizards.PlaceAnOrder.ReloadEntityOnOrderComplete)
        {
            MultiStepWizards.PlaceAnOrder.ReloadEntityOnOrderComplete = false;
            ConciergeAPI.CurrentEntity = LoadObjectFromAPI<msEntity>(ConciergeAPI.CurrentEntity.ID);
        }

        if(!string.IsNullOrWhiteSpace(MultiStepWizards.PlaceAnOrder.OrderCompleteUrl))
        {
            string nextUrl = MultiStepWizards.PlaceAnOrder.OrderCompleteUrl;
            nextUrl += nextUrl.Contains("?") ? "&OrderID=" + ContextID : "?OrderID=" + ContextID;

            MultiStepWizards.PlaceAnOrder.OrderCompleteUrl = null;
            QueueBannerMessage(lblConfirmationText.Text);

            GoTo(nextUrl);
        }
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        hlViewOrder.NavigateUrl = "/financial/ViewOrder.aspx?contextID=" + targetOrder.ID;
    }

    protected string getConfirmationText()
    {
        return string.Format("Order  #{0}  has been processed successfully. A confirmation email has been sent to  {1}.", targetOrder.LocalID, targetOrder.BillingEmailAddress);
    }
}