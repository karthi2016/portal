using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class subscriptions_ViewSubscription : PortalPage 
{
    protected msSubscription targetSubscription;
    protected msEntity targetEntity;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetSubscription = LoadObjectFromAPI<msSubscription>(ContextID);
        if (targetSubscription == null) GoToMissingRecordPage();

        targetEntity = LoadObjectFromAPI<msEntity>(targetSubscription.Owner);
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return targetSubscription.Owner == ConciergeAPI.CurrentEntity.ID;
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        bindFields();

        hlModify.NavigateUrl += ContextID;
    }

    protected override void setupHyperLinks()
    {
        base.setupHyperLinks();
        hlUpdateBillingInfo2.NavigateUrl = hlUpdateBillingInfo.NavigateUrl = string.Format("/orders/UpdateBillingInformation.aspx?contextID={0}", ContextID);
    
    }

    private void bindFields()
    {
        using (var api = GetServiceAPIProxy())
        {
            
            lblPublication.Text = api.GetName(targetSubscription.Publication).ResultValue;
            lblOwner.Text = api.GetName(targetSubscription.Owner).ResultValue;
            lblProduct.Text = api.GetName(targetSubscription.Fee).ResultValue;

            if (targetSubscription.SavedPaymentMethod != null)
            {
                string paymentInfo = api.GetName(targetSubscription.SavedPaymentMethod).ResultValue;
                if (paymentInfo != null)
                    lblPaymentInfo.Text = paymentInfo;
            }

            if (targetSubscription.StartDate != null)
                lblStartDate.Text = targetSubscription.StartDate.Value.ToShortDateString();
            if (targetSubscription.ExpirationDate != null)
                lblExpirationDate.Text = targetSubscription.ExpirationDate.Value.ToShortDateString();
            if (targetSubscription.TerminationDate != null)
            {
                trTermination.Visible = true;
                lblTerminationDate.Text = targetSubscription.TerminationDate.Value.ToShortDateString();
                if (targetSubscription.TerminationReason != null)
                    lblTerminationReason.Text = api.GetName(targetSubscription.TerminationReason).ResultValue;
            }

            if (targetSubscription.OriginalOrder != null)
            {
                hlOriginalOrder.Text = api.GetName(targetSubscription.OriginalOrder).ResultValue;
                hlOriginalOrder.NavigateUrl = "/orders/ViewOrder.aspx?contextID=" + targetSubscription.OriginalOrder;
            }

            if (targetSubscription.LastOrder != null)
            {
                hlRenewalOrder.Text = api.GetName(targetSubscription.LastOrder).ResultValue;
                hlRenewalOrder.NavigateUrl = "/orders/ViewOrder.aspx?contextID=" + targetSubscription.LastOrder;
            }

            if (!string.IsNullOrWhiteSpace(targetSubscription.OverrideShipToName))
                lblShipTo.Text = targetSubscription.OverrideShipToName;
            else
                lblShipTo.Text = api.GetName(targetSubscription.Owner).ResultValue;

            if (targetSubscription.OverrideShipToAddress)
                lblShippingAddress.Text = targetSubscription.Address.ToHtmlString();
            else
            {
                msEntityAddress ea = Utils.GetEntityPreferredAddress(targetEntity);
                if (ea != null && ea.Address != null)
                    lblShippingAddress.Text = ea.Address.ToHtmlString();
            }
        }
    
    }
    protected void lblRenewSubscription_Click(object sender, EventArgs e)
    {
        using (var api = GetServiceAPIProxy())
        {
            var order = api.GenerateRenewalOrder(targetSubscription.ID).ResultValue;
            if (order == null) return;

            MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(order.ConvertTo<msOrder>());
        }
    }
}