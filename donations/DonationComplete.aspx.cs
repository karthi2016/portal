using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class donations_DonationComplete : PortalPage
{
    #region Fields

    protected msOrder targetOrder;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    #endregion

    protected override void InitializePage()
    {
        base.InitializePage();
        divTasks.Visible = ConciergeAPI.CurrentUser != null;
    }

    #region Initialization

    /// <summary>
    /// Initializes the target object for the page
    /// </summary>
    /// <remarks>Many pages have "target" objects that the page operates on. For instance, when viewing
    /// an event, the target object is an event. When looking up a directory, that's the target
    /// object. This method is intended to be overriden to initialize the target object for
    /// each page that needs it.</remarks>
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadTargetOrder(proxy);
        }

        if (targetOrder == null) GoToMissingRecordPage();
    }

    protected void loadTargetOrder(IConciergeAPIService serviceProxy)
    {
        targetOrder = LoadObjectFromAPI<msOrder>(serviceProxy, ContextID);
    }

    #endregion
}