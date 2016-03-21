using System;
using System.Text;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for MembershipLogic
/// </summary>
public static class MembershipLogic
{
    private const string HasOpenMembershipInvoiceCacheKeyTemplate = "MembershipLogic::HasOpenMembershipInvoice{0}";

    private const string IsActiveMemberCacheKeyTemplate = "MembershipLogic::IsActiveMember{0}";

    public static Search GetSearchForOrganizationalLayerMemberships()
    {
        //Organizational Layer Membership
        var sOrganizationalLayerMembership = new Search(msChapterMembership.CLASS_NAME)
        {
            ID = "OrganizationalLayerMembership"
        };
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.ID");
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.Name");
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.Layer");
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.Layer.Name");
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.Layer.Type.Name");

        //Add the recursive query for all the parent organizational layers
        var sbOrganizationalLayer = new StringBuilder("Chapter.Layer");
        //Add Organizational Layers
        for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
        {
            sbOrganizationalLayer.Append(".{0}");
            var parentLayerColumn = string.Format(sbOrganizationalLayer.ToString(), "ParentLayer");
            var parentLayerName = string.Format("{0}.Name", parentLayerColumn);
            var parentLayerTypeName = string.Format("{0}.Type.Name", parentLayerColumn);

            sOrganizationalLayerMembership.AddOutputColumn(parentLayerColumn);
            sOrganizationalLayerMembership.AddOutputColumn(parentLayerName);
            sOrganizationalLayerMembership.AddOutputColumn(parentLayerTypeName);
        }
        sOrganizationalLayerMembership.AddCriteria(Expr.Equals("Membership.Owner.ID", ConciergeAPI.CurrentEntity.ID));
        sOrganizationalLayerMembership.AddCriteria(Expr.Equals("IsCurrent", true));

        sOrganizationalLayerMembership.AddSortColumn("Chapter.Name");
        return sOrganizationalLayerMembership;
    }

    /// <summary>
    /// Determine if the given Entity has an Open (unpaid) Invoice containing a Membership Product.
    /// </summary>
    /// <param name="entityId">Entity to search Invoices for.</param>
    /// <returns>True if the Entity has a Membership Invoice.</returns>
    public static bool HasOpenMembershipInvoice(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            return false;

        var key = string.Format(HasOpenMembershipInvoiceCacheKeyTemplate, entityId);
        var hasOpenInvoice = SessionManager.Get<bool?>(key, () =>
        {
            var s = new Search(msInvoiceLineItem.CLASS_NAME);
            s.AddOutputColumn("Product");
            s.AddSortColumn("Product");
            ////s.AddCriteria(Expr.Equals("Invoice.ProForma", true));
            s.AddCriteria(Expr.IsGreaterThan("Invoice.BalanceDue", 0));
            s.AddCriteria(Expr.Equals("Invoice.BillTo", entityId));
            s.AddCriteria(Expr.Equals("Product.ClassType", "MEMBR"));
            var result = APIExtensions.GetSearchResult(s, 0, 1);

            return result != null && result.TotalRowCount > 0;
        });

        return hasOpenInvoice.HasValue && hasOpenInvoice.Value;
    }

    /// <summary>
    /// Determine if the Current Entity has an Open (unpaid) Invoice containing a Membership Product.
    /// </summary>
    /// <returns>True if the Current Entity has a Membership Invoice.</returns>
    public static bool HasOpenMembershipInvoice()
    {
        return HasOpenMembershipInvoice(ConciergeAPI.CurrentEntity.ID);
    }

    /// <summary>
    /// This addition to the MembershipLogic class assumes that all associations, at minimum,
    /// define an active membership as having ReceivesMemberBenefits == true and the termination
    /// date is either null or in the future.
    /// </summary>
    /// <param name="entityId">Entity to check Membership for.</param>
    /// <returns>True if the Entity is a Member.</returns>
    public static bool IsActiveMember(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            return false;

        var key = string.Format(IsActiveMemberCacheKeyTemplate, entityId);
        var isActiveMember = SessionManager.Get<bool?>(key, () =>
        {
            var search = new Search("MembershipWithFlowdown");
            search.AddCriteria(Expr.Equals(msMembership.FIELDS.Owner, entityId));
            search.AddCriteria(Expr.Equals(msMembership.FIELDS.ReceivesMemberBenefits, true));

            var sog = new SearchOperationGroup {GroupType = SearchOperationGroupType.Or};
            sog.Criteria.Add(Expr.IsBlank(msMembership.FIELDS.TerminationDate));
            sog.Criteria.Add(Expr.IsGreaterThan(msMembership.FIELDS.TerminationDate, DateTime.Today.Date));
            search.AddCriteria(sog);

            var result = APIExtensions.GetSearchResult(search, 0, 1);

            return result != null && result.TotalRowCount > 0;
        });

        return isActiveMember.HasValue && isActiveMember.Value;
    }

    /// <summary>
    /// This addition to the MembershipLogic class assumes that all associations, at minimum,
    /// define an active membership as having ReceivesMemberBenefits == true and the termination
    /// date is either null or in the future.
    /// </summary>
    /// <returns>True if the Current Entity is a Member.</returns>
    public static bool IsActiveMember()
    {
        return IsActiveMember(ConciergeAPI.CurrentEntity.ID);
    }
    
    /// <summary>
    /// Clear the caches related to Membership for the given Entity.
    /// </summary>
    /// <param name="entityId">Entity to clear caches for.</param>
    public static void ClearMemberCaches(string entityId)
    {
        var key = string.Format(IsActiveMemberCacheKeyTemplate, entityId);
        SessionManager.Set<bool?>(key, null);

        key = string.Format(HasOpenMembershipInvoiceCacheKeyTemplate, entityId);
        SessionManager.Set<bool?>(key, null);
    }

    /// <summary>
    /// Clear the caches related to Membership for the Current Entity and related Entities.
    /// </summary>
    public static void ClearMemberCaches()
    {
        ClearMemberCaches(ConciergeAPI.CurrentEntity.ID);

        // also clear this for any related entities
        if (ConciergeAPI.AccessibleEntities != null)
        {
            foreach (var accessibleEntity in ConciergeAPI.AccessibleEntities)
            {
                ClearMemberCaches(accessibleEntity.ID);
            }
        }
    }
}