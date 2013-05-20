using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_GroupRegistrationStep1 : PortalPage 
{
    protected msEvent targetEvent;
    protected msOrganization targetOrganization;

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

        targetOrganization = LoadObjectFromAPI<msOrganization>(Request.QueryString["organizationID"]);
        if (targetOrganization == null) GoToMissingRecordPage();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
    }
    protected override bool CheckSecurity()
    {
        var api = GetConciegeAPIProxy();

        
        var entities = GroupRegistrationLogic.GetEntitiesEligibleForGroupRegistration(targetEvent, CurrentEntity, api);

        if (entities == null || !entities.Contains(targetOrganization.ID) || ! GroupRegistrationLogic.IsGroupRegistrationOpen( targetEvent ))
            GoTo("/AccessDenied.aspx"); // security violation
        return base.CheckSecurity();
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        initializeContacts();
        initializeRoles();
    }

    private void initializeRoles()
    {
        Search sRelationshipTypes = new Search(msRelationshipType.CLASS_NAME);
        sRelationshipTypes.AddOutputColumn("ID");
        sRelationshipTypes.AddOutputColumn("Name");
        sRelationshipTypes.AddOutputColumn("LeftSideType");
        sRelationshipTypes.AddOutputColumn("RightSideType");
        sRelationshipTypes.AddOutputColumn("RightSideName");

        sRelationshipTypes.AddCriteria(Expr.Equals("LeftSideType", "Organization"));
        sRelationshipTypes.AddCriteria(Expr.Equals("RightSideType", "Individual"));
        sRelationshipTypes.AddCriteria(Expr.Equals("IsActive", true ));

        sRelationshipTypes.AddSortColumn("RightSideName");
        lbRoles.DataSource = ExecuteSearch(sRelationshipTypes, 0, null).Table ;
        lbRoles.DataTextField = "RightSideName";
        lbRoles.DataValueField = "ID";
        lbRoles.DataBind();
    }

    private void initializeContacts()
    {
        Search sRelationships = new Search("RelationshipsForARecord")
        {
            ID = msRelationship.CLASS_NAME,
            Context = targetOrganization.ID
        };
        sRelationships.AddOutputColumn("ID");
        sRelationships.AddOutputColumn("RightSide_Individual.LocalID");
        sRelationships.AddOutputColumn("RightSide_Individual.ID");
        sRelationships.AddOutputColumn("RightSide_Individual.Name");
        sRelationships.AddOutputColumn("RightSide_Individual.EmailAddress");
        sRelationships.AddSortColumn("RightSide_Individual.Name");

        var dt = ExecuteSearch(sRelationships, 0, null).Table;

        Hashtable htContacts = new Hashtable();
        foreach (DataRow dr in dt.Rows)
        {
            string id = Convert.ToString(dr["RightSide_Individual.ID"]);

            if (string.IsNullOrWhiteSpace(id))
                continue;   // orphan

            if (htContacts.ContainsKey(id))
                continue;   // it's a dupe

            htContacts.Add(id, true);

            string email = Convert.ToString( dr["RightSide_Individual.EmailAddress"] );

            if (!string.IsNullOrWhiteSpace(email))
                email = string.Format(" [{0}] ", email);

            string text = string.Format("{0}{1}#{2}",
                                        dr["RightSide_Individual.Name"],
                                        email ,
                                        dr["RightSide_Individual.LocalID"]);
            ddlIndividual.Items.Add(new ListItem(text, id));
        }

    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        bool enableValidators = ddlIndividual.SelectedValue == "-1";

        rfvFirstName.Enabled = enableValidators;
        rfvLastName.Enabled = enableValidators;
        rfvEmail.Enabled = enableValidators;
        rfvRoles.Enabled = enableValidators;

        Validate();

        if (!IsValid)
            return;

        // ok - do we have a registration?
        string userID = ddlIndividual.SelectedValue;

        if (userID == "-1") // then we have to create an individual
        {
            msIndividual i = new msIndividual();
            i.FirstName = tbFirstName.Text;
            i.LastName = tbLastName.Text;
            i.EmailAddress = tbEmail.Text;
            i["PrimaryOrganization__rtg"] = targetOrganization.ID;

            List<string> roles = new List<string>();
            foreach (ListItem li in lbRoles.Items)
                if (li.Selected)
                    roles.Add(li.Value);
            i["PrimaryOrganizationRoles__rtg"] = roles;

            i = SaveObject<msIndividual>(i);
            userID = i.ID;
            QueueBannerMessage(string.Format("Individual #{0} - {1} {2} has been saved successfully.",
                                             i.LocalID, i.FirstName, i.LastName));
        }
        else
        {
            // is this person in our list? Can't add them if so

            var order = MultiStepWizards.GroupRegistration.Order;
            if (order != null)
                foreach (msOrderLineItem li in order.LineItems)
                    if (li.OverrideShipTo == userID)
                    {
                        cvSelectedTwice.IsValid = false;
                        return;
                    }

            // ok, let's figure out if this person is already registered
            if (EventLogic.IsRegistered(targetEvent.ID, userID))
            {
                cvAlreadyRegistered.IsValid = false;
                return;
            }

        }

        MultiStepWizards.GroupRegistration.Group  = targetOrganization;
        MultiStepWizards.GroupRegistration.RegistrantID = userID;
        MultiStepWizards.GroupRegistration.Event = targetEvent; ;

        GoTo(string.Format("Register_SelectFee.aspx?contextID={0}&mode=group&organizationID={1}&individualID={2}",
            targetEvent.ID, targetOrganization.ID, userID));

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
            GoTo(string.Format( "ManageGroupRegistration.aspx?contextID={0}&organizationID={1}",
                targetEvent.ID, targetOrganization.ID ) );
    }
}