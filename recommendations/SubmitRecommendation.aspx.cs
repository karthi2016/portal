using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class recommendations_SubmitRecommendation : PortalPage 
{
    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected msRecommendation targetRecommendation;
    protected msIndividual target;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetRecommendation = LoadObjectFromAPI<msRecommendation>(ContextID);
        if (targetRecommendation == null)
            GoToMissingRecordPage();
        setupRecommendationTarget();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (targetRecommendation.Status != RecommendationStatus.Pending)
            return false;

        return true;
    }
    protected override void InitializePage()
    {
        base.InitializePage();

      
    }

    private void setupRecommendationTarget()
    {
        switch (targetRecommendation.ClassType)
        {
            case msCertificationRecommendation.CLASS_NAME:
                target = LoadObjectFromAPI<msIndividual>(
                    LoadObjectFromAPI<msCertification>(
                        targetRecommendation.SafeGetValue<string>(msCertificationRecommendation.FIELDS.Certification)).Certificant );
                break;

            default:
                throw new NotSupportedException("Unknown recommendation type");
        }

    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {

        instantiateEditFields(proxy);


        instantiateViewFields(proxy);
    }

    private void instantiateEditFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetRecommendation ;

        var pageLayout = GetAppropriatePageLayout(targetRecommendation);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(targetRecommendation.ClassType ).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;


        CustomFieldSet1.Render();
    }

    private void instantiateViewFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.Harvest();

        CustomFieldSet2.MemberSuiteObject = targetRecommendation;


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
                CustomFieldSet2.MemberSuiteObject = targetRecommendation;
                CustomFieldSet2.Bind();

                break;

            case 2:
                submitForm();
                
                break;
        }
    }

    private void submitForm()
    {
        CustomFieldSet1.Harvest();

        targetRecommendation.Status  = RecommendationStatus.Completed;
        SaveObject(targetRecommendation);



       

    }
}