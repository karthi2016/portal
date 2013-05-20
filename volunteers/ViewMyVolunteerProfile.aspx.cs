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
using MemberSuite.SDK.Utilities;

public partial class volunteers_ViewMyVolunteerProfile : PortalPage 
{
    protected msVolunteer targetVolunteer;
    protected DataRow drSearchResults;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetVolunteer = LoadObjectFromAPI<msVolunteer>(ContextID);
        if (targetVolunteer == null)
            GoToMissingRecordPage();
    }
    protected override void InitializePage()
    {
        base.InitializePage();

        generateSearch();
        
        // set up types
        setupVolunteerTypes();

        setupTraits();

        setupUnavailability();

        hlUpdateProfile.NavigateUrl += ContextID;
        hlViewOpenJobs.NavigateUrl += ContextID;
        hlViewMyJobHistory.NavigateUrl += ContextID;
        hlSubmitTimesheet.NavigateUrl += ContextID;
    }

    private void setupUnavailability()
    {
        if (targetVolunteer.UnavailableFrom == null && targetVolunteer.UnavailableTo == null && targetVolunteer.AvailabilityComment == null)
            return;

        if (targetVolunteer.UnavailableTo != null && targetVolunteer.UnavailableTo < DateTime.Today)
            return;

        divUnavailability.Visible = true;

        string unavailability = "From ";
        if (targetVolunteer.UnavailableFrom != null)
            unavailability += targetVolunteer.UnavailableFrom.Value.ToShortDateString();
        else
            unavailability = "Until";

        if (targetVolunteer.UnavailableTo != null)
            unavailability += " until " + targetVolunteer.UnavailableTo.Value.ToShortDateString();
        else
            unavailability += " Until Further Notice";

        if ( !string.IsNullOrWhiteSpace( targetVolunteer.AvailabilityComment ) )
        {
            trUnavailabilityComments.Visible = true;
            lblAvailabilityComment.Text = targetVolunteer.AvailabilityComment;
        }

        lblUnavailability.Text = unavailability;


    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        return targetVolunteer.Individual == ConciergeAPI.CurrentEntity.ID;
    }

    private void setupTraits()
    {
        // let's populate trait types
        List<NameValueStringPair> traitTypes = new List<NameValueStringPair>();


        using (var api = GetServiceAPIProxy())
            foreach (var t in targetVolunteer.Traits)
                if (!traitTypes.Exists(x => x.Value == t.Type))
                    traitTypes.Add(new NameValueStringPair(api.GetName(t.Type).ResultValue, t.Type));

        if (traitTypes.Count > 0)
        {
            rptTraitTypes.DataSource = traitTypes;
            rptTraitTypes.DataBind();
        }

    }

    private void setupVolunteerTypes()
    {
        using (var api = GetServiceAPIProxy())
            if (targetVolunteer.Types != null && targetVolunteer.Types.Count > 0)
            {
                string volunteerTypes = "";
                foreach (var t in targetVolunteer.Types)
                {
                    if (t.StartDate != null && t.StartDate > DateTime.Today) continue; // not applicable
                    if (t.EndDate != null && t.EndDate > DateTime.Today) continue; // not applicable
                    volunteerTypes += string.Format("{0} <i>({1})</i>, ", api.GetName(t.Type).ResultValue, t.Status);
                }
                lblVolunteerTypes.Text = volunteerTypes.Trim().TrimEnd(',');
            }
    }

    private void generateSearch()
    {
        Search s = new Search(msVolunteer.CLASS_NAME);
        s.AddCriteria(Expr.Equals("ID", targetVolunteer.ID));

        s.AddOutputColumn("Sponsor.Name");
        s.AddOutputColumn("Individual.Name");
        s.AddOutputColumn("NumberOfAssignments");
        s.AddOutputColumn("TotalHoursWorked");
        s.AddOutputColumn("LastAssignment.JobOccurrence.Job.Name");
        s.AddOutputColumn("LastAssignment.JobOccurrence.StartDateTime");
        s.AddOutputColumn("LastAssignment.JobOccurrence.EndDateTime");

        drSearchResults = ExecuteSearch(s, 0, 1).Table.Rows[0];

        trSponsor.Visible = drSearchResults["Sponsor.Name"] != DBNull.Value;
    }

    protected void rptTraitTypes_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        NameValueStringPair nvp = (NameValueStringPair)e.Item.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;

            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
                goto case ListItemType.Item;

            case ListItemType.Item:
                HyperLink hlViewTraits = (HyperLink)e.Item.FindControl("hlViewTraits");

                hlViewTraits.Text = string.Format("<li>View My {0}</li>", RegularExpressions.GetFriendlyPluralName(nvp.Name));
                hlViewTraits.NavigateUrl += targetVolunteer.ID + "&traitTypeID=" + nvp.Value;

                break;
        }
    }
}