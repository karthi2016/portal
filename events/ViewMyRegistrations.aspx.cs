using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_ViewMyRegistrations : PortalPage 
{

    protected override void InitializePage()
    {
        base.InitializePage();

        Search s = new Search(msEventRegistration.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Owner", CurrentEntity.ID));
        s.AddSortColumn("CreatedDate", true);

        s.AddOutputColumn("Event");
        s.AddOutputColumn("Event.Name");
        s.AddOutputColumn("Event.StartDate");
        s.AddOutputColumn("Event.AllowRegistrantsToChangeSessions");
        s.AddOutputColumn("Event.DeadlineForChangingSessions");
        s.AddOutputColumn("CreatedDate");

        var results = APIExtensions.GetSearchResult(s, 0, null);

        gvEvents.DataSource = results.Table;
        gvEvents.DataBind();

        if (results.Table != null
            && results.Table.Rows != null && results.Table.Rows.Count > 0)
        {
            var eventId = string.Empty;
            if (results.Table.Columns.Contains("Event"))
                eventId = Convert.ToString(results.Table.Rows[0]["Event"]);

            bool allowRegistrantsToChangeSessions = false;
            if (results.Table.Columns.Contains("Event.AllowRegistrantsToChangeSessions"))
                bool.TryParse(Convert.ToString(results.Table.Rows[0]["Event.AllowRegistrantsToChangeSessions"]), out allowRegistrantsToChangeSessions);

            DateTime deadlineForChangingSessions = DateTime.MaxValue;
            if (results.Table.Columns.Contains("Event.DeadlineForChangingSessions")
                && results.Table.Rows[0]["Event.DeadlineForChangingSessions"] != DBNull.Value)
                deadlineForChangingSessions = Convert.ToDateTime(results.Table.Rows[0]["Event.DeadlineForChangingSessions"]);

            if (!allowRegistrantsToChangeSessions
                || deadlineForChangingSessions < DateTime.Now
                || (!string.IsNullOrWhiteSpace(eventId) && !EventLogic.HasSessions(eventId)))
                gvEvents.Columns[3].Visible = false; // Hide the (change sessions) button
        }
    }
}