using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class membership_RegisterForMembership : PortalPage
{
    #region Properties

    protected override bool IsPublic
    {
        get { return true; }
    }

    protected string EntityId
    {
        get { return Request.QueryString["entityID"]; }
    }

    #endregion

    #region Initialization

    protected override void InitializePage()
    {
        base.InitializePage();

        string nextUrl = ConciergeAPI.CurrentEntity == null
                             ? "/profile/CreateAccount_BasicInfo.aspx?t=Membership"
                             : string.Format("~/membership/Join.aspx?contextID={0}", ContextID);

        if (!string.IsNullOrWhiteSpace(EntityId))
            nextUrl += "&entityID=" + EntityId;

        GoTo(nextUrl);
    }

    #endregion

    #region Methods

    #endregion
}