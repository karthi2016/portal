using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class volunteers_SubmitTimesheet : PortalPage 
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

    protected void btnGoHome_Click(object sender, EventArgs e)
    {
        GoTo("ViewMyVolunteerProfile.aspx?contextID=" + targetVolunteer.ID);
    }
}