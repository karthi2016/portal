using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class subscriptions_ViewMySubscriptions : PortalPage 
{
    protected override void InitializePage()
    {
        base.InitializePage();

        bindSubscriptions();
    }

    private void bindSubscriptions()
    {
        Search s = new Search(msSubscription.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msSubscription.FIELDS.Owner, ConciergeAPI.CurrentEntity.ID) );
        s.AddSortColumn(msSubscription.FIELDS.ExpirationDate);
        s.AddOutputColumn("Publication.Name");
        s.AddOutputColumn(msSubscription.FIELDS.StartDate);
        s.AddOutputColumn(msSubscription.FIELDS.ExpirationDate);
        s.AddOutputColumn("IsActive");

        gvSubscriptions.DataSource = APIExtensions.GetSearchResult(s, 0, null).Table;
        gvSubscriptions.DataBind();
    }
}