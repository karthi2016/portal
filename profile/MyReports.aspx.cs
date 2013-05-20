using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class profile_MyReports : PortalPage 
{
    protected override void InitializePage()
    {
        base.InitializePage();

        // let's get the reports
        List<EntitlementReport> entitlements;
        List<MemberSuiteObject> searches = new List<MemberSuiteObject>();
        using (var api = GetServiceAPIProxy())
        {
            entitlements =
                api.ListEntitlements(ConciergeAPI.CurrentEntity.ID, msSearchEntitlement.CLASS_NAME).ResultValue;

            foreach (var e in entitlements)
            {

                var ss = api.Get(e.Context).ResultValue;
                if (ss == null)
                    continue;


                if (e.AvailableUntil == null)
                    ss["Expiration"] = "You have access to this report indefinitely.";
                else
                    ss["Expiration"] = string.Format("You have access to this report until {0} at {1}.",
                                                     e.AvailableUntil.Value.ToLongDateString(),
                                                     e.AvailableUntil.Value.ToShortTimeString());

                searches.Add(ss);
            }

            rptFolders.DataSource = searches;
            rptFolders.DataBind();
        }

    }

    protected void btnGoHome_Cick(object sender, EventArgs e)
    {
        GoHome();
    }
}