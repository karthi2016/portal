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

    protected override void InitializePage()
    {
        base.InitializePage();

        CustomTitle.Text = string.Format("{0}", targetLink.Name);
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

        return !targetLink.MembersOnly || MembershipLogic.IsActiveMember();
    }

    #endregion
}