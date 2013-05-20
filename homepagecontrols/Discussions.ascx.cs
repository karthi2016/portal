using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class homepagecontrols_Discussions : HomePageUserControl
{
    public override List<string> GetFieldsNeededForMainSearch()
    {
        var list =  base.GetFieldsNeededForMainSearch();

        list.Add("Discussions_LastDiscussionPost");
        list.Add("Discussions_LastDiscussionPost.Name");
        list.Add("Discussions_LastDiscussionPost.Topic");
        list.Add("Discussions_LastDiscussionPost.CreatedDate");

        return list;
    }

    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);

        if (drMainRecord["Discussions_LastDiscussionPost"] != DBNull.Value)
        {
            hlLastPost.Text = Convert.ToString(drMainRecord["Discussions_LastDiscussionPost.Name"]);
            hlLastPost.NavigateUrl += Convert.ToString(drMainRecord["Discussions_LastDiscussionPost.Topic"]);
            lblLastPostDate.Text = FormatDate(drMainRecord["Discussions_LastDiscussionPost.CreatedDate"]);
        }
        else
            hlLastPost.Enabled = false;
    }

    protected string FormatDate(object value)
    {
        if (value == null || !(value is DateTime))
            return null;

        DateTime dt = (DateTime)value;

        if (dt.Date == DateTime.Today)
            return "Today " + dt.ToShortTimeString();

        if (dt.Date == DateTime.Today.AddDays(-1))
            return "Yesterday " + dt.ToShortTimeString();

        return dt.ToShortDateString() + " " + dt.ToShortTimeString();
    }
}