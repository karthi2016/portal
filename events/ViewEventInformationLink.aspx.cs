using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Results;

public partial class events_ViewEventInformationLink : PortalPage
{
    #region Fields

    protected msEventInformationLink targetEventInformationLink;
    protected msEvent targetEvent;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected string EventInformationLinkID
    {
        get { return Request.QueryString["eventInformationLinkID"]; }
    }

    #endregion

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
            loadDataFromConcierge(proxy);
        }

        if (targetEvent == null) GoToMissingRecordPage();
        if (targetEventInformationLink == null) GoToMissingRecordPage();
    }

    protected void loadDataFromConcierge(IConciergeAPIService serviceProxy)
    {
        targetEvent = serviceProxy.LoadObjectFromAPI<msEvent>(ContextID);
        targetEventInformationLink = serviceProxy.LoadObjectFromAPI<msEventInformationLink>(EventInformationLinkID);
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

        return targetEvent.VisibleInPortal;
    }

    #endregion
}