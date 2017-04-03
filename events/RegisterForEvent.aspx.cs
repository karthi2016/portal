using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class events_RegisterForEvent : PortalPage
{
    #region Fields

    #endregion

    protected msEvent targetEvent;

    #region Properties

    protected override bool IsPublic
    {
        get { return true; }
    }

    #endregion

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
        
        targetEvent = LoadObjectFromAPI<msEvent>(ContextID);
        if (targetEvent == null) GoToMissingRecordPage();
    }

    #region Initialization

    protected override void InitializePage()
    {
        base.InitializePage();

       string nextUrl = ConciergeAPI.CurrentEntity != null
                                 ? string.Format("~/events/Register_SelectFee.aspx?contextID={0}", ContextID)
                                 : string.Format(
                                     "/profile/CreateAccount_BasicInfo.aspx?pageTitle={0}&completionUrl={1}",
                                     Server.UrlEncode("Event Registration"),
                                     Server.UrlEncode(
                                         string.Format("/events/Register_SelectFee.aspx?contextID={0}",
                                                       ContextID)));

        GoTo(nextUrl);
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;


        if (targetEvent.InviteOnly && 
            (ConciergeAPI.CurrentEntity == null ||
            !EventLogic.IsInvited(targetEvent.ID, ConciergeAPI.CurrentEntity.ID)))
            return false;

        // MS-3032
        //if (ConciergeAPI.CurrentEntity != null && EventLogic.IsRegistered(targetEvent.ID, ConciergeAPI.CurrentEntity.ID))
            //return false;

        if ( ! targetEvent.VisibleInPortal && ! ConciergeAPI.HasBackgroundConsoleUser)
            return false;

        if ( (targetEvent.RegistrationCloseDate ?? targetEvent.EndDate)  < DateTime.Today)
            return false;

        // MS-4745
        if (EventLogic.IsRegistrationClosed(targetEvent))
            return false;

        // is the registration open?
        if (targetEvent.RegistrationOpenDate != null && targetEvent.RegistrationOpenDate > DateTime.Now)
            return false;


        if (targetEvent.RegistrationMode == EventRegistrationMode.Normal &&
            ConciergeAPI.CurrentEntity != null && EventLogic.IsRegistered(targetEvent.ID, ConciergeAPI.CurrentEntity.ID)

            )
            return false;

        if (targetEvent.RegistrationMode == EventRegistrationMode.Tabled) // tabled event
            return false;

        return true;
    }
    #endregion
}