using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class homepagecontrols_Events : HomePageUserControl
{
    public override void GenerateSearchesToBeRun(List<Search> searchesToRun)
    {
        base.GenerateSearchesToBeRun(searchesToRun);


        // search for featured events
        Search sFeaturedEvents = new Search {Type = msEvent.CLASS_NAME, ID = "FeaturedEvents"};
        sFeaturedEvents.AddOutputColumn("ID");
        sFeaturedEvents.AddOutputColumn("Name");
        sFeaturedEvents.AddCriteria(Expr.Equals("IsFeatured", 1));
        sFeaturedEvents.AddCriteria(Expr.Equals("VisibleInPortal", 1));
        sFeaturedEvents.AddSortColumn("FeaturedPriority");
        sFeaturedEvents.AddSortColumn("StartDate");
        sFeaturedEvents.AddSortColumn("Name");

        searchesToRun.Add(sFeaturedEvents);
    }

    public override List<string> GetFieldsNeededForMainSearch()
    {
        var fields = base.GetFieldsNeededForMainSearch();
         fields.Add("Events_LastRegistration.Event.Name");
        fields.Add("Events_LastRegistration");

        return fields;
    }

    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);
        if (!Visible) return;

        //Bind Events
        rptFeaturedEvents.DataSource = results.Single(x => x.ID == "FeaturedEvents").Table; ;
        rptFeaturedEvents.DataBind();

        string nameOfLastRegistrationEvent = Convert.ToString( drMainRecord["Events_LastRegistration.Event.Name"] );
        if ( !string.IsNullOrWhiteSpace( nameOfLastRegistrationEvent) )
        {
            lbLastRegistration.Text = nameOfLastRegistrationEvent;
            lbLastRegistration.Enabled = true;
            lbLastRegistration.NavigateUrl = "/events/ViewEventRegistration.aspx?contextID=" +
                                             drMainRecord["Events_LastRegistration"];
        }
    }


}