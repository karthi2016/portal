using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class volunteers_ViewMyJobAssignments : PortalPage 
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
        GoTo("ViewMyVolunteerProfile.aspx?contextID=" + ContextID); 
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        Search s = new Search(msVolunteerJobAssignment.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msVolunteerJobAssignment.FIELDS.Volunteer, targetVolunteer.ID));
        s.AddOutputColumn("JobOccurrence.Job.Name");
        s.AddOutputColumn(msVolunteerJobAssignment.FIELDS.StartDateTime );
        s.AddOutputColumn(msVolunteerJobAssignment.FIELDS.EndDateTime);
        s.AddOutputColumn("HoursWorked");


        s.AddSortColumn(msVolunteerJobAssignment.FIELDS.StartDateTime, true);

        gvHistory.DataSource = APIExtensions.GetSearchResult(s, 0, null).Table;
        gvHistory.DataBind();
    }
}