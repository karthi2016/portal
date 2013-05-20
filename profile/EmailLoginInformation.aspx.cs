using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Searching;

public partial class profile_EmailLoginInformation : PortalPage
{
    #region Fields

    protected msPortalUser targetPortalUser;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this page is public, meaning you don't
    /// have to be logged in to access it.
    /// </summary>
    /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected string CompleteUrl
    {
        get
        {
            return Request.QueryString["completeUrl"];
        }
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
            determineContextAndSendEmail(proxy);
        }

        if (targetPortalUser == null)
            GoToMissingRecordPage();
    }

    protected void determineContextAndSendEmail(IConciergeAPIService serviceProxy)
    {
        targetPortalUser = serviceProxy.Get(ContextID).ResultValue.ConvertTo<msPortalUser>();

        //Send the forgotten password email
        serviceProxy.SendForgottenPortalPasswordEmail(targetPortalUser.Name, CompleteUrl);
    }

    #endregion
}