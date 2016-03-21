using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;

public partial class homepagecontrols_Fundraising : HomePageUserControl 
{
    public override List<string> GetFieldsNeededForMainSearch()
    {
        var fields = base.GetFieldsNeededForMainSearch();
        fields.Add ("Fundraising_LastGift.Date");
        fields.Add("Fundraising_LastGift.Total");
        return fields;
    }

    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);
        if (!Visible) return;

        lblLastDonation.Text = Convert.IsDBNull(drMainRecord["Fundraising_LastGift.Date"]) ? "No Records Found" : string.Format("{0} for {1:c}",
            ((DateTime)drMainRecord["Fundraising_LastGift.Date"]).ToString("d"), drMainRecord["Fundraising_LastGift.Total"]);
  
    }
}