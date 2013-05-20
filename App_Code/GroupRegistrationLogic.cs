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
/// Summary description for GroupRegistrationLogic
/// </summary>
public class GroupRegistrationLogic
{
	public GroupRegistrationLogic()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static List<string> GetEntitiesEligibleForGroupRegistration(msEvent targetEvent, msEntity currentEntity, IConciergeAPIService api)
    {
        if (targetEvent == null) throw new ArgumentNullException("targetEvent");
        if (api == null) throw new ArgumentNullException("api");

        List<string> applicableRelationshipTypes = targetEvent.SafeGetValue<List<string>>("GroupRegistrationRelationshipTypes");
        if (applicableRelationshipTypes == null || applicableRelationshipTypes.Count == 0)
            return null ; // there are no registration types enabled

        if ( currentEntity == null ) return null;
        

        List<string> entities = new List<string>();

        if (currentEntity.ClassType == msOrganization.CLASS_NAME) // an organization can always manage themselves
        {
            entities.Add(currentEntity.ID);
            return entities ;
        }

        // OK, let's see if this person is linked to any companies
        Search s = new Search("RelationshipsForARecord");
        s.Context = currentEntity.ID ;
        s.AddCriteria(Expr.Equals("IsLeftSide", false ));  // the right side of the relationship is the individual
        
        // now, we do an is one of the follow for relationship types
        IsOneOfTheFollowing isTypes = new IsOneOfTheFollowing { FieldName = "Type_ID" };
        isTypes.ValuesToOperateOn = new List<object>(applicableRelationshipTypes);

        s.AddCriteria(isTypes);

        s.AddOutputColumn("Target_ID");

        var values = api.ExecuteSearch(s, 0, null).ResultValue.Table;

        foreach (DataRow dr in values.Rows)
            entities.Add(Convert.ToString(dr["Target_ID"]));

        // keep in mind we may have orphaned relationships, so we have to make sure each org ID exists!
        return entities;

    }

    public static bool IsGroupRegistrationOpen(msEvent targetEvent)
    {
        if (targetEvent.AllowGroupRegistrationsFrom != null)
        {
            if (targetEvent.AllowGroupRegistrationsFrom > DateTime.Today)
                return false;
        }
        else if (targetEvent.RegistrationOpenDate != null)
            if (targetEvent.RegistrationOpenDate > DateTime.Today)
                return false;

        if (targetEvent.AllowGroupRegistrationsUntil != null)
        {
            if (targetEvent.AllowGroupRegistrationsUntil < DateTime.Today)
                return false;
        }
        else
            if (targetEvent.RegistrationCloseDate != null && targetEvent.RegistrationCloseDate < DateTime.Today)
                return false;

        return targetEvent.EndDate > DateTime.Today;
    }
}