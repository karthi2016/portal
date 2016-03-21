using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for CRMLogic
/// </summary>
public static class CRMLogic
{
    public static void CheckToSeeIfMultipleIdentitiesAreAccessible()
    {
        // now - are there alternate accessible entities?
        var entities = ConciergeAPI.AccessibleEntities;
        if (entities == null || entities.Count == 0) // well, what entity do we login as?
            return;

        // automatically insert the current entity
        if (!entities.Exists(x => x.ID == ConciergeAPI.CurrentEntity.ID))
        {
            entities.Insert(0, new LoginResult.AccessibleEntity
            {
                ID = ConciergeAPI.CurrentEntity.ID,
                Name = ConciergeAPI.CurrentEntity.Name,
                Type = ConciergeAPI.CurrentEntity.ClassType
            });

            ConciergeAPI.AccessibleEntities = entities;
            // remember, the session is out of proc, so we need to restore it
        }

        msPortalUser currentUser = ConciergeAPI.CurrentUser;
        if (currentUser.LastLoggedInAs != null && entities.Exists(x => x.ID == currentUser.LastLoggedInAs))
        {
            // use this one
            ConciergeAPI.CurrentEntity = APIExtensions.LoadObjectFromAPI<msEntity>(currentUser.LastLoggedInAs);
            return;
        }

        // we have multiple identiites
        HttpContext.Current.Response.Redirect("/MultipleEntitiesDetected.aspx?redirectURL=" + HttpContext.Current.Request.QueryString["redirectURL"]);
    }

    /// <summary>
    /// MS-5424 -System is not respecting organization contact restrictions
    /// </summary>
    /// <param name="orgId"></param>
    /// <param name="relationshipType"></param>
    /// <param name="individualId"></param>
    /// <remarks>I placed this code here because it might also be needed when adding/editing contacts from ManagedOrganization  page</remarks>
    public static void ErrorOutIfOrganizationContactRestrictionApplies(string orgId, string relationshipType, string individualId = null)
    {
        if (string.IsNullOrWhiteSpace(relationshipType) || string.IsNullOrWhiteSpace(orgId))
            return;

        // check restriction
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            var restrictionResult = api.GetApplicableOrganizationContactRestriction(orgId, relationshipType);

            if (!restrictionResult.Success)
            {
                throw new ConciergeClientException(ConciergeErrorCode.GeneralException, restrictionResult.FirstErrorMessage);
            }

            if (restrictionResult.ResultValue == null)
            {
                // If no restrictions, we have nothing to check
                return;
            }

            var restriction = restrictionResult.ResultValue.ConvertTo<msOrganizationContactRestriction>();

            var max = restriction.MaximumNumberOfContacts;
            var errorMessage = restriction.ErrorMessage;
            if (string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = string.Format(
                    "Unable to add contact - a restriction is in place: {0}",
                    restriction["Name"]);

            if (max == 0)
                throw new ConciergeClientException(ConciergeErrorCode.GeneralException, errorMessage);

            // Get all unexpired relationships
            var relationshipSearch = new Search {Type = msRelationship.CLASS_NAME};

            // Only apply this filter if the Restriction calls for it (otherwise is against ALL relationships)
            if (!string.IsNullOrEmpty(restriction.RelationshipType))
                relationshipSearch.AddCriteria(Expr.Equals("Type", restriction.RelationshipType));

            relationshipSearch.AddCriteria(Expr.Equals("IsActive", true));

            var filterClause = new SearchOperationGroup{GroupType = SearchOperationGroupType.Or};
            if (string.IsNullOrEmpty(individualId))
            {
                filterClause.Criteria.Add(Expr.Equals("LeftSide_Organization", orgId));
                filterClause.Criteria.Add(Expr.Equals("RightSide_Organization", orgId));
            }
            else
            {
                var filter1 = new SearchOperationGroup();
                filter1.Criteria.Add(Expr.Equals("LeftSide_Organization", orgId));
                filter1.Criteria.Add(Expr.DoesNotEqual("RightSide_Individual", individualId));

                var filter2 = new SearchOperationGroup();
                filter2.Criteria.Add(Expr.Equals("RightSide_Organization", orgId));
                filter2.Criteria.Add(Expr.DoesNotEqual("LeftSide_Individual", individualId));

                filterClause.Criteria.Add(filter1);
                filterClause.Criteria.Add(filter2);
            }
            
            relationshipSearch.AddCriteria(filterClause);

            var rr = api.ExecuteSearch(relationshipSearch, 0, null);

            if (rr.ResultValue == null)
                throw new ConciergeClientException(ConciergeErrorCode.GeneralException, rr.FirstErrorMessage);

            var i = rr.ResultValue.Table.Rows.Count;

            // If we are below the initial threshold, we are fine, so return
            if (i < max)
                return;

            // Now let's see if there are any Entitlements that allow this Organization to exceed the restriction
            var entitlementSearch = new Search { Type = msOrganizationContactEntitlement.CLASS_NAME };
            entitlementSearch.AddCriteria(Expr.Equals(msEntitlement.FIELDS.Owner, orgId));

            var availableFromClause = new SearchOperationGroup {GroupType = SearchOperationGroupType.Or};
            availableFromClause.Criteria.Add(Expr.IsBlank(msEntitlement.FIELDS.AvailableFrom));
            availableFromClause.Criteria.Add(Expr.IsLessThanOrEqual(msEntitlement.FIELDS.AvailableFrom, DateTime.Today));

            var availableToClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
            availableToClause.Criteria.Add(Expr.IsBlank(msEntitlement.FIELDS.AvailableUntil));
            availableToClause.Criteria.Add(Expr.IsGreaterThanOrEqualTo(msEntitlement.FIELDS.AvailableUntil, DateTime.Today));

            entitlementSearch.AddCriteria(availableFromClause);
            entitlementSearch.AddCriteria(availableToClause);

            var entitlementResult = api.GetObjectsBySearch(entitlementSearch, null, 0, null);

            if (!entitlementResult.Success)
                throw new ConciergeClientException(ConciergeErrorCode.GeneralException, entitlementResult.FirstErrorMessage);

            var entitlementTotal = 0;
            if (entitlementResult.ResultValue != null)
            {
                var entitlements = entitlementResult.ResultValue.Objects.ConvertTo<msOrganizationContactEntitlement>();

                foreach (var entitlement in entitlements)
                {
                    if (string.IsNullOrEmpty(restriction.RelationshipType) ||
                        entitlement.RelationshipTypes.Any(
                            id => id.Equals(restriction.RelationshipType, StringComparison.OrdinalIgnoreCase)))
                    {
                        entitlementTotal += (int)entitlement.Quantity;
                    }
                }
            }

            if (i >= max + entitlementTotal)
                throw new ConciergeClientException(ConciergeErrorCode.GeneralException, errorMessage);
        }
    }
}