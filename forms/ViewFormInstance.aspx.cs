using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;

public partial class forms_ViewFormInstance : PortalPage 
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

        // edit
        hlEditInstance.Visible = targetFormManifest.CanEdit;
        hlEditInstance.NavigateUrl += targetRecord.ID + "&formID=" + targetForm.ID;
        hlEditInstance.Text = string.Format("<LI>Edit this Record</LI>");

        lbDelete.Visible = targetFormManifest.CanDelete;
        // go back
        hlBack.NavigateUrl += targetForm.ID;
        hlBack.Text = string.Format("<LI>Back to {0}</LI>", targetFormManifest.ManageLink );

        RegisterJavascriptConfirmationBox(lbDelete, "Are you sure you want to delete this record? This cannot be undone.");

        PageTitleExtension.Text = targetForm.Name;
    }

    protected void lbDelete_Click(object sender, EventArgs e)
    {
        using (var api = GetServiceAPIProxy())
            api.Delete(targetRecord.ID);

        QueueBannerMessage("The record was deleted successfully.");
        GoTo("ManageFormInstances.aspx?contextID=" + targetForm.ID);
    }
}