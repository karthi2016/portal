using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;


/// <summary>
/// Summary description for ExhibitorLogic
/// </summary>
public static class ExhibitorLogic
{

    public static bool CanViewExhibitorRecord(IConciergeAPIService api, msExhibitor ex, string entityIDToCheck)
    {
        if (api == null) throw new ArgumentNullException("api");
        if (ex == null) throw new ArgumentNullException("ex");

        if (entityIDToCheck == null)
            return false;

        if (ex.Customer == entityIDToCheck)
            return true;

        return api.GetAccessibleEntitiesForEntity(entityIDToCheck).ResultValue.Exists(x => x.ID == ex.Customer);

    }

    public static bool CanEditExhibitorRecord(IConciergeAPIService api, msExhibitor ex, string entityIDToCheck)
    {
        return CanViewExhibitorRecord(api, ex, entityIDToCheck);
    }
}