using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for CommitteeLogic
/// </summary>
public static class CommitteeLogic
{
    public static bool IsAdministrativeMember(IConciergeAPIService proxy, string committeeID, string individualID)
    {
        Search s = new Search(msCommitteeMembership.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msCommitteeMembership.FIELDS.Committee, committeeID));
        s.AddCriteria(Expr.Equals(msCommitteeMembership.FIELDS.Member, individualID));
        s.AddCriteria(Expr.Equals(msCommitteeMembership.FIELDS.GrantPortalAdministratorPrivileges, true));

        return proxy.GetSearchResult(s, 0, 1).TotalRowCount > 0;
            
    }

    public static bool IsMember(IConciergeAPIService proxy, string committeeID, string individualID)
    {
        Search s = new Search(msCommitteeMembership.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msCommitteeMembership.FIELDS.Committee, committeeID));
        s.AddCriteria(Expr.Equals(msCommitteeMembership.FIELDS.Member, individualID));


        return proxy.GetSearchResult(s, 0, 1).TotalRowCount > 0;

    }
}