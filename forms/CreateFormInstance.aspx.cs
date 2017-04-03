using System.ServiceModel.Activities;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;
using System;
using System.Collections.Generic;

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

        // MS-5922 - only check this on initial load so that it will not fail on completion of creation of last allowed instance
        if (!IsPostBack)
        {
            if (!targetFormManifest.CanCreate)
                GoTo("/AccessDenied.aspx");
        }

        if (ConciergeAPI.CurrentEntity == null && !targetForm.AllowAnonymousSubmissions ) // we need them to go through registration
        {
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=PortalForm&formID=" + targetForm.ID);
            return;
        }

        // ok, so we allow anonymous submissions here
        // MS-5360
        if (string.IsNullOrWhiteSpace(CurrentFormID.Value))
        {
            targetInstance = MetadataLogic.CreateNewObject(targetFormManifest.UnderlyingObjectName);
            targetInstance["Owner"] = entityID;            
        }
        else
        {
            targetInstance = APIExtensions.LoadObjectFromAPI(CurrentFormID.Value);
        }        
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        CustomTitle.Text = string.Format("{0}", targetForm.Name);
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        // MS-5922 - only check this on initial load so that it will not fail on completion of creation of last allowed instance
        return IsPostBack || targetFormManifest.CanCreate;
    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        InstantiateEditFields(proxy);
        InstantiateViewFields(proxy);
    }

    private void InstantiateEditFields(IConciergeAPIService proxy)
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

    private void InstantiateViewFields(IConciergeAPIService proxy)
    {        
        CustomFieldSet2.MemberSuiteObject = targetInstance;
 
        // setup the metadata
        CustomFieldSet2.Metadata = CustomFieldSet1.Metadata;
        CustomFieldSet2.PageLayout = CustomFieldSet1.PageLayout;


        CustomFieldSet2.Render();
    }

    // MS-5360
    private void DeleteCurrentForm()
    {
        if (string.IsNullOrWhiteSpace(CurrentFormID.Value))
            return;

        using (var api = GetServiceAPIProxy())
            api.Delete(CurrentFormID.Value);
        CurrentFormID.Value = null;        
    }

    protected void wzEnterInfo_StepChanged(object sender, EventArgs e)
    {
        switch (wzEnterInfo.ActiveStepIndex)
        {
            // MS-5360
            case 0:
                DeleteCurrentForm();
                break;

            case 1:
                CustomFieldSet1.Harvest();
                // MS-5360
                targetInstance = APIExtensions.SaveObject(targetInstance);
                CurrentFormID.Value = targetInstance["ID"].ToString();
                CustomFieldSet2.MemberSuiteObject = targetInstance;
                CustomFieldSet2.Bind();                
                break;

            case 2:
                SubmitForm();
                if (ConciergeAPI.CurrentEntity == null && targetForm.AnonymousSubmissionCompletionUrl != null)
                    wzEnterInfo.FinishDestinationPageUrl = targetForm.AnonymousSubmissionCompletionUrl;

                wzEnterInfo.DisplayCancelButton = false;

                break;
        }
    }

    private void SubmitForm()
    {
        targetInstance = APIExtensions.SaveObject(targetInstance);

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
            if (!string.IsNullOrWhiteSpace(targetForm.ConfirmationEmail))
            {
                using (var api = GetServiceAPIProxy())
                {
                    var emailTemplate = api.Get(targetForm.ConfirmationEmail).ResultValue.ConvertTo<msEmailTemplateContainer>();
                    api.SendTransactionalEmail(emailTemplate.Name, CurrentEntity.ID, null);
                }
            }
        }
    }

    // MS-5360
    protected void wzEnterInfo_OnCancelButtonClick(object sender, EventArgs e)
    {
        DeleteCurrentForm();
        GoHome();
    }
}