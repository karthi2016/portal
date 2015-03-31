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
    public static Search GetSearchForOrganizationalLayerMemberships()
    {
        //Organizational Layer Membership
        Search sOrganizationalLayerMembership = new Search(msChapterMembership.CLASS_NAME) { ID = "OrganizationalLayerMembership" };
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.ID");
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.Name");
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.Layer");
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.Layer.Name");
        sOrganizationalLayerMembership.AddOutputColumn("Chapter.Layer.Type.Name");

        //Add the recursive query for all the parent organizational layers
        StringBuilder sbOrganizationalLayer = new StringBuilder("Chapter.Layer");
        //Add Organizational Layers
        for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
        {
            sbOrganizationalLayer.Append(".{0}");
            string parentLayerColumn = String.Format(sbOrganizationalLayer.ToString(), "ParentLayer");
            string parentLayerName = String.Format("{0}.Name", parentLayerColumn);
            string parentLayerTypeName = String.Format("{0}.Type.Name", parentLayerColumn);

            sOrganizationalLayerMembership.AddOutputColumn(parentLayerColumn);
            sOrganizationalLayerMembership.AddOutputColumn(parentLayerName);
            sOrganizationalLayerMembership.AddOutputColumn(parentLayerTypeName);
        }
        sOrganizationalLayerMembership.AddCriteria(Expr.Equals("Membership.Owner.ID", ConciergeAPI.CurrentEntity.ID));
        sOrganizationalLayerMembership.AddCriteria(Expr.Equals("IsCurrent", true));

        sOrganizationalLayerMembership.AddSortColumn("Chapter.Name");
        return sOrganizationalLayerMembership;
    }
    // This addition to the MembershipLogic class assumes that all associations, at minimum,
    // define an active membership as having ReceivesMemberBenefits == true and the membership
    // is either null or in the future.
    public static bool IsActiveMember(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            return false;

        var search = new Search(msMembership.CLASS_NAME);
        search.AddCriteria(Expr.Equals(msMembership.FIELDS.Owner, entityId));
        search.AddCriteria(Expr.Equals(msMembership.FIELDS.ReceivesMemberBenefits, true));        
        var sog = new SearchOperationGroup {GroupType = SearchOperationGroupType.Or};
        sog.Criteria.Add(Expr.IsBlank(msMembership.FIELDS.TerminationDate));
        sog.Criteria.Add(Expr.IsGreaterThan(msMembership.FIELDS.TerminationDate, DateTime.Today.Date));
        search.AddCriteria(sog);

        ConciergeResult<SearchResult> result;
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            result = api.ExecuteSearch(search, 0, null);

        return (result != null && result.ResultValue != null && result.ResultValue.TotalRowCount > 0);
    }
}