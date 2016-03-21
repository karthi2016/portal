using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for PortalMaster
/// </summary>
public class PortalMaster : MasterPage
{
    public delegate void AddCustomOverrideEligibleControlsEventHandler(List<msPortalControlPropertyOverride> eligibleControls);

    public event AddCustomOverrideEligibleControlsEventHandler AddCustomOverrideEligibleControls;

    public bool OnAddCustomOverrideEligibleControlsEvent(List<msPortalControlPropertyOverride> eligibleControls)
    {
        if (AddCustomOverrideEligibleControls != null)
        {
            AddCustomOverrideEligibleControls(eligibleControls);
            return true;
        }

        return false;
    }
}