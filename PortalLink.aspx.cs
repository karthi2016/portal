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

public partial class PortalLink : PortalPage
{
    #region Fields

    protected msPortalLink targetLink;
    protected DataRow drMembership;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get { return true; }
    }

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadTargetLink(proxy);
        }

        if(targetLink == null)
            GoToMissingRecordPage();
    }

    private void loadTargetLink(IConciergeAPIService serviceproxy)
    {
        var obj = serviceproxy.Get(ContextID).ResultValue;
        if (obj != null)
            targetLink = obj.ConvertTo<msPortalLink>();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if(targetLink.IsPublic)
            return true;

        if (CurrentUser == null)
            return false;

        return !targetLink.MembersOnly || isActiveMember();
    }

    #endregion

    #region Methods

    protected bool isActiveMember()
    {
        if (CurrentEntity == null)
            return false;

        loadMembership();

        if (drMembership == null)
            return false;

        //Check if the appropriate fields exist - if they do not then the membership module is inactive
        if (
            !(drMembership.Table.Columns.Contains("Membership") &&
              drMembership.Table.Columns.Contains("Membership.ReceivesMemberBenefits") &&
              drMembership.Table.Columns.Contains("Membership.TerminationDate")))
            return false;

        //Check there is a membership
        if (string.IsNullOrWhiteSpace(Convert.ToString(drMembership["Membership"])))
            return false;

        //Check the membership indicates membership benefits
        if (!drMembership.Field<bool>("Membership.ReceivesMemberBenefits"))
            return false;

        //At this point if the termination date is null the member should be able to see the restricted directory
        DateTime? terminationDate = drMembership.Field<DateTime?>("Membership.TerminationDate");

        if (terminationDate == null)
            return true;

        //There is a termination date so check if it's future dated
        return terminationDate > DateTime.Now;
    }

    protected virtual void loadMembership()
    {
        Search sMembership = new Search(msEntity.CLASS_NAME) { ID = msMembership.CLASS_NAME };
        sMembership.AddOutputColumn("ID");
        sMembership.AddOutputColumn("Membership");
        sMembership.AddOutputColumn("Membership.ReceivesMemberBenefits");
        sMembership.AddOutputColumn("Membership.TerminationDate");
        sMembership.AddCriteria(Expr.Equals("ID", ConciergeAPI.CurrentEntity.ID));
        sMembership.AddSortColumn("ID");

        SearchResult srMembership = ExecuteSearch(sMembership, 0, 1);
        drMembership = srMembership != null && srMembership.Table != null &&
                       srMembership.Table.Rows.Count > 0
                           ? srMembership.Table.Rows[0]
                           : null;
    }

    #endregion
}