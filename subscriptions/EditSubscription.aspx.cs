using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class subscriptions_EditSubscription : PortalPage
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
        hlViewSubscription.NavigateUrl += ContextID;
        using (var api = GetServiceAPIProxy())
        {

            lblPublication.Text = api.GetName(targetSubscription.Publication).ResultValue;
            lblOwner.Text = api.GetName(targetSubscription.Owner).ResultValue;

        }

        bindFields();
    }

    private void bindFields()
    {
        tbShipTo.Text = targetSubscription.OverrideShipToName;
        if (targetSubscription.OverrideShipToAddress)
            rbSpecify.Checked = true;
        else
            rbUseDefault.Checked = true;

        acAddress.Address = targetSubscription.Address;

        cbOnHold.Checked = targetSubscription.OnHold;
        rbAddress_Changed(null, null);
        
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        targetSubscription.OverrideShipToName = tbShipTo.Text;
        targetSubscription.OnHold = cbOnHold.Checked;

        if (rbSpecify.Checked)
        {
            targetSubscription.OverrideShipToAddress = true;
            targetSubscription.Address = acAddress.Address;
        }
        else
        {
            targetSubscription.OverrideShipToAddress = false;
            targetSubscription.Address = null;
        }

        SaveObject(targetSubscription);

        GoTo("ViewSubscription.aspx?contextID=" + targetSubscription.ID, "Your subscription has been updated successfully.");
    }

    protected void rbAddress_Changed(object sender, EventArgs e)
    {
        trSpecifyAddress.Visible = rbSpecify.Checked;
    }
}