using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class membership_EditMembership : PortalPage
{
    #region Setup
    protected override void setupHyperLinks()
    {
        base.setupHyperLinks();
        hlViewMembership.NavigateUrl += "?contextID=" + ContextID;
        hlViewMembership2.NavigateUrl = hlViewMembership.NavigateUrl;

    }
    private msMembership targetMembership;
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetMembership = LoadObjectFromAPI<msMembership>(ContextID);
        if (targetMembership == null) GoToMissingRecordPage();
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        cbAutomaticallyPay.Checked = targetMembership.AutomaticallyPayForRenewal;
        cbMembershipDirectoryOptOut.Checked = targetMembership.MembershipDirectoryOptOut;
    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetMembership;

        var pageLayout = targetMembership.GetAppropriatePageLayout();
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = targetMembership.DescribeObject();
        CustomFieldSet1.PageLayout = pageLayout.Metadata;


        CustomFieldSet1.Render();
    }


    #endregion

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        targetMembership.MembershipDirectoryOptOut = cbMembershipDirectoryOptOut.Checked;
        targetMembership.AutomaticallyPayForRenewal = cbAutomaticallyPay.Checked;
        CustomFieldSet1.Harvest();

        SaveObject(targetMembership);
        GoTo(hlViewMembership.NavigateUrl, "Membership was updated successfully.");
    }
}