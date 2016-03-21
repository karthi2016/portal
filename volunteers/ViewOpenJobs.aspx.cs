using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using Telerik.Web.UI;

public partial class volunteers_ViewOpenJobs : PortalPage 
{
    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }
    protected msVolunteer targetVolunteer;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetVolunteer = LoadObjectFromAPI<msVolunteer>(ContextID);
         
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        return targetVolunteer == null || ConciergeAPI.CurrentEntity == null ||  targetVolunteer.Individual == ConciergeAPI.CurrentEntity.ID;
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        setupButtons();

        bindJobs();
    }

    private void bindJobs()
    {
        Search s = new Search(msVolunteerJobOccurrence.CLASS_NAME);
        s.AddCriteria(Expr.IsGreaterThan("VolunteerSlotsAvailable", 0));
        s.AddCriteria(Expr.Equals(msVolunteerJobOccurrence.FIELDS.IsActive, true));

        s.AddOutputColumn("Job.Name");
        s.AddOutputColumn("Location.Name");
        s.AddOutputColumn("StartDateTime");
        s.AddOutputColumn("EndDateTime");
        s.AddOutputColumn("Comments");

        s.AddSortColumn("StartDateTime");

        gvJobs.DataSource = APIExtensions.GetSearchResult(s, 0, null).Table;
        gvJobs.DataBind();
    }

    private void setupButtons()
    {
        btnGoHome.Visible = ConciergeAPI.CurrentEntity != null;
        btnGoToMyProfile.Visible = targetVolunteer != null;
        
    }

    protected void btnGoToMyProfile_Click(object sender, EventArgs e)
    {
        GoTo("ViewMyVolunteerProfile.aspx?contextID=" + ContextID);
    }

    protected void btnGoHome_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void gvJobs_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataRowView dr = (DataRowView)e.Row.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                HyperLink hlMoreInfo = (HyperLink)e.Row.FindControl("hlMoreInfo");
                RadToolTip rttDetails = (RadToolTip)e.Row.FindControl("rttDetails");

                var comments = Convert.ToString( dr[msVolunteerJobOccurrence.FIELDS.Comments] );
                if (string.IsNullOrWhiteSpace(comments))
                {
                    hlMoreInfo.Visible = false;
                    return;
                }

                rttDetails.Text = comments;
                rttDetails.TargetControlID = hlMoreInfo.ID;

                
                break;
        }
    }
}