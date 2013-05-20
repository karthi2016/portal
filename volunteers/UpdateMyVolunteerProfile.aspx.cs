using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class volunteers_UpdateMyVolunteerProfile : PortalPage 
{
    protected msVolunteer targetVolunteer;
   
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetVolunteer = LoadObjectFromAPI<msVolunteer>(ContextID);
        if (targetVolunteer == null)
            GoToMissingRecordPage();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        return targetVolunteer.Individual == ConciergeAPI.CurrentEntity.ID;
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {

        CustomFieldSet1.Harvest();

        SaveObject(targetVolunteer );

        GoTo("ViewMyVolunteerProfile.aspx?contextID=" + targetVolunteer.ID, "The changes to your volunteer record have been saved successfully.");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewMyVolunteerProfile.aspx?contextID=" + targetVolunteer.ID);
    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetVolunteer ;

        var pageLayout = GetAppropriatePageLayout(targetVolunteer);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(msVolunteer.CLASS_NAME).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;


        CustomFieldSet1.Render();
    }
}