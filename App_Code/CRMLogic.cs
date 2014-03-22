using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for CRMLogic
/// </summary>
public class CRMLogic
{
   

    public static bool HasOrganizationContactRetrictionBeenViolated( msOrganizationContactRestriction restrictionToCheck,
        msOrganization targetOrganization, msIndividual targetIndividual)
    {
        string msql = string.Format("select count(id) from Relationship where LeftSide_Organization='{0}' and IsActive=true",
                              targetOrganization.ID);

        if (restrictionToCheck.RelationshipType != null)
            msql += string.Format(" and Type='{0}'", restrictionToCheck.RelationshipType);

        if (targetIndividual != null && targetIndividual.ID != null)  // we are editing
            msql += string.Format(" and RightSide_Individual <> '{0}'", restrictionToCheck.RelationshipType);

        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            // get the count result
            var count = (int)api.ExecuteMSQL(msql, 0, 1).ResultValue.SearchResult.Table.Rows[0]["id"];

            return count >= restrictionToCheck.MaximumNumberOfContacts;
        }
    }
}