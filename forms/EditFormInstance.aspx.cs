using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class forms_EditFormInstance : PortalPage
{
    protected msCustomObjectPortalForm targetForm;
    protected PortalFormInfo targetFormManifest;
    protected msCustomObject targetObject;
    protected msCustomObjectInstance targetRecord;

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return targetFormManifest.CanView;
    }

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetForm = LoadObjectFromAPI<msCustomObjectPortalForm>(Request.QueryString["formID"]);
        if (targetForm == null)
            GoToMissingRecordPage();

        targetRecord = LoadObjectFromAPI<msCustomObjectInstance>(ContextID);
        if (targetRecord == null)
            GoToMissingRecordPage();

        targetObject = LoadObjectFromAPI<msCustomObject>(targetForm.CustomObject);

        using (var api = GetServiceAPIProxy())
            targetFormManifest = api.DescribePortalForm(targetForm.ID, ConciergeAPI.CurrentEntity.ID).ResultValue;
    }

    protected override void InstantiateCustomFields(MemberSuite.SDK.Concierge.IConciergeAPIService proxy)
    {
        base.InstantiateCustomFields(proxy);

        CustomFieldSet1.MemberSuiteObject = targetRecord;

        var pageLayout = LoadObjectFromAPI<msPortalPageLayoutContainer>(targetForm.CreatePageLayout);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(targetFormManifest.UnderlyingObjectName).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;


        CustomFieldSet1.Render();
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        PageTitleExtension.Text = targetForm.Name;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        CustomFieldSet1.Harvest();

        SaveObject(targetRecord);
        QueueBannerMessage("Your changes have been saved successfully.");

        GoTo(string.Format("ViewFormInstance.aspx?contextID={0}&formID={1}",
            targetRecord.ID, targetForm.ID));
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("ViewFormInstance.aspx?contextID={0}&formID={1}",
           targetRecord.ID, targetForm.ID));
    }
}