using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class forms_CreateFormInstance : PortalPage 
{

    protected msCustomObjectPortalForm targetForm;
    protected PortalFormInfo targetFormManifest;
    protected MemberSuiteObject targetInstance;

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetForm = LoadObjectFromAPI<msCustomObjectPortalForm>(ContextID);
        if (targetForm == null)
            GoToMissingRecordPage();

        string entityID = null;
        if (ConciergeAPI.CurrentEntity != null)
            entityID = ConciergeAPI.CurrentEntity.ID;

        using (var api = GetServiceAPIProxy())
            targetFormManifest = api.DescribePortalForm(targetForm.ID, entityID).ResultValue;

        if (!targetFormManifest.CanCreate)
            GoTo("/AccessDenied.aspx");
        
        if (ConciergeAPI.CurrentEntity == null && !targetForm.AllowAnonymousSubmissions ) // we need them to go through registration
        {
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=PortalForm&formID=" + targetForm.ID);
            return;
        }
     
        
        // ok, so we allow anonymous submissions here

        targetInstance = CreateNewObject(targetFormManifest.UnderlyingObjectName);
        targetInstance["Owner"] = entityID;
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;
        
        return targetFormManifest.CanCreate;
    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {

        instantiateEditFields(proxy);

        
            instantiateViewFields(proxy);
    }

    private void instantiateEditFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetInstance;

        var pageLayout = LoadObjectFromAPI<msPortalPageLayoutContainer>(targetForm.CreatePageLayout);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(targetFormManifest.UnderlyingObjectName).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;


        CustomFieldSet1.Render();
    }

    private void instantiateViewFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.Harvest();
        
        CustomFieldSet2.MemberSuiteObject = targetInstance;
 

        // setup the metadata
        CustomFieldSet2.Metadata = CustomFieldSet1.Metadata;
        CustomFieldSet2.PageLayout = CustomFieldSet1.PageLayout;


        CustomFieldSet2.Render();
    }

    protected void wzEnterInfo_StepChanged(object sender, EventArgs e)
    {
        switch (wzEnterInfo.ActiveStepIndex)
        {
            case 1:
                CustomFieldSet1.Harvest();
                CustomFieldSet2.MemberSuiteObject = targetInstance;
                CustomFieldSet2.Bind();
                
                break;

            case 2:
                submitForm();
                if (ConciergeAPI.CurrentEntity == null && targetForm.AnonymousSubmissionCompletionUrl != null)
                    wzEnterInfo.FinishDestinationPageUrl = targetForm.AnonymousSubmissionCompletionUrl;
                break;
        }
    }

    private void submitForm()
    {
        CustomFieldSet1.Harvest();
        targetInstance =  SaveObject(targetInstance);

        // ok, log an activity
        if (ConciergeAPI.CurrentEntity != null)
        {
            if (targetForm.ActivityType != null)
            {
                msActivity a = CreateNewObject<msActivity>();
                a.Type = targetForm.ActivityType;
                a.Name = targetForm.Name;
                a.Date = DateTime.Now;
                a.Entity = ConciergeAPI.CurrentEntity.ID;
                SaveObject(a);
            }

            // ok, send an email
            if (targetForm.ConfirmationEmail != null)
            {
                using (var api = GetServiceAPIProxy())
                    api.SendEmail(targetForm.ConfirmationEmail, new List<string> { targetInstance.SafeGetValue<string>("ID") }, null);
                
            }
        }
    }
}