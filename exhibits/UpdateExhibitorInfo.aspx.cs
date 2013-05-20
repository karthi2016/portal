using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class exhibits_UpdateExhibitorInfo : PortalPage
{
    public msExhibitor targetExhibitor;
    public msExhibitShow targetShow;
     
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetExhibitor = LoadObjectFromAPI<msExhibitor>(ContextID);
        if (targetExhibitor == null) GoToMissingRecordPage();

        targetShow = LoadObjectFromAPI<msExhibitShow>(targetExhibitor.Show);
        if (targetShow == null) GoToMissingRecordPage();

       
    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetExhibitor;

        var pageLayout = GetAppropriatePageLayout(targetExhibitor);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(msExhibitor.CLASS_NAME).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;


        CustomFieldSet1.Render();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        using (var api = GetConciegeAPIProxy())
            return ExhibitorLogic.CanEditExhibitorRecord(api, targetExhibitor, ConciergeAPI.CurrentEntity.ID);
    }

    private void save()
    {
       
    }


    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        CustomFieldSet1.Harvest();

        SaveObject(targetExhibitor);

        GoTo("ViewExhibitor.aspx?contextID=" + ContextID, "The changes to your exhibitor record have been saved successfully.");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewExhibitor.aspx?contextID=" + ContextID);
    }
}