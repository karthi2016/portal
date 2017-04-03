using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for EventLogic
/// </summary>
public static class EventLogic
{
    public static bool IsInvited(string eventID, string individualID)
    {
        Search s = new Search("EventInvitee");
        s.AddCriteria(Expr.Equals("Event", eventID));
        s.AddCriteria(Expr.Equals("Invitee", individualID));

        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            return api.GetSearchResult(s, 0, 1).TotalRowCount > 0;
    }

    public static bool IsRegistered(string eventID, string entityID)
    {
        Search s = new Search(msEventRegistration.CLASS_NAME);
        s.Context = eventID;
        s.AddCriteria(Expr.Equals(msRegistrationBase.FIELDS.Owner, entityID));
        s.AddCriteria(Expr.IsBlank(msRegistrationBase.FIELDS.CancellationDate));    // check to make sure cancelled are not counted


        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            return api.GetSearchResult(s, 0, 1).TotalRowCount > 0;
         
    }

    public static bool IsRegistrationClosed(msEvent targetEvent)
    {
        if ( targetEvent.IsClosed ) return true;
        if (targetEvent.RegistrationMode == EventRegistrationMode.Disabled) return true;

        if ( targetEvent.RegistrationCloseDate == null ) return false;

        TimeZoneInfo tzi;

        try
        {
            tzi = TimeZoneInfo.FindSystemTimeZoneById(targetEvent.TimeZone);
        }
        catch 
        {
            tzi = TimeZoneInfo.Local;
        }

        // MS-4745 - let's take the current time and convert it to the time of the event
        DateTime eventCurrentDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tzi);

        // now, compare it
        if ( eventCurrentDate > targetEvent.RegistrationCloseDate )
             return true;

        return false;
        
    }

    public static bool HasSessions(string eventId)
    {
        var key = string.Format("EventLogic::HasSessions_{0}", eventId);

        var returnValue = SessionManager.Get<bool?>(key, () =>
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return false;
            }

            var search = new Search(msSession.CLASS_NAME);
            search.AddCriteria(Expr.Equals("ParentEvent", eventId));
            search.AddCriteria(Expr.Equals("VisibleInPortal", true));

            using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var result = api.ExecuteSearch(search, 0, 1);

                return result != null && result.ResultValue != null && result.ResultValue.TotalRowCount > 0;
            }
        });

        return returnValue.HasValue ? returnValue.Value : false;
    }
}