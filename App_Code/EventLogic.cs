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
            return api.ExecuteSearch(s, 0, 1).ResultValue.TotalRowCount > 0;
    }

    public static bool IsRegistered(string eventID, string entityID)
    {
        Search s = new Search(msEventRegistration.CLASS_NAME);
        s.Context = eventID;
        s.AddCriteria(Expr.Equals(msRegistrationBase.FIELDS.Owner, entityID));
        s.AddCriteria(Expr.IsBlank(msRegistrationBase.FIELDS.CancellationDate));    // check to make sure cancelled are not counted


        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            return api.ExecuteSearch(s, 0, 1).ResultValue.TotalRowCount > 0;
         
    }
}