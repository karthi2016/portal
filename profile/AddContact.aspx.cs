using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class profile_AddContact : PortalPage
{
    #region Fields

    protected msOrganization targetOrganization;
    protected DataView dvRelationshipTypes;
    protected msIndividual targetIndividual;
    protected msRelationshipType targetRelationshipType;
    protected string targetEmailAddress;
    protected bool sendInvitation;

    #endregion

    #region Properties

    public string RelationshipTypeId
    {
        get { return Request.QueryString["relationshipTypeId"]; }
    }

    #endregion

    #region Initialization

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

        targetOrganization = string.IsNullOrWhiteSpace(ContextID) ? ConciergeAPI.CurrentEntity.ConvertTo<msOrganization>() : LoadObjectFromAPI<msOrganization>(ContextID);

        if (targetOrganization == null || targetOrganization.ClassType != msOrganization.CLASS_NAME)
        {
            GoToMissingRecordPage();
        }
    }

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();

        trRelationshipType.Visible = string.IsNullOrWhiteSpace(RelationshipTypeId);

        if(trRelationshipType.Visible)
        {
            loadDataFromConcierge();

            ddlRelationshipType.DataSource = dvRelationshipTypes;
            ddlRelationshipType.DataBind();
        }

        PageTitleExtension.Text = targetOrganization.Name;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        targetEmailAddress = MultiStepWizards.AddContact.EmailAddress;
        targetIndividual = MultiStepWizards.AddContact.Individual;
        targetRelationshipType = MultiStepWizards.AddContact.RelationshipType;
        sendInvitation = MultiStepWizards.AddContact.SendInvitation;
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sRelationshipTypes = new Search(msRelationshipType.CLASS_NAME);
        sRelationshipTypes.AddOutputColumn("ID");
        sRelationshipTypes.AddOutputColumn("Name");
        sRelationshipTypes.AddOutputColumn("LeftSideType");
        sRelationshipTypes.AddOutputColumn("RightSideType");

        SearchOperationGroup individualGroup = new SearchOperationGroup
        {
            FieldName = "LeftSideType",
            GroupType = SearchOperationGroupType.Or
        };
        individualGroup.Criteria.Add(Expr.Equals("LeftSideType", msIndividual.CLASS_NAME));
        individualGroup.Criteria.Add(Expr.Equals("RightSideType", msIndividual.CLASS_NAME));

        SearchOperationGroup organizationGroup = new SearchOperationGroup
        {
            FieldName = "RightSideType",
            GroupType = SearchOperationGroupType.Or
        };
        organizationGroup.Criteria.Add(Expr.Equals("LeftSideType", msOrganization.CLASS_NAME));
        organizationGroup.Criteria.Add(Expr.Equals("RightSideType", msOrganization.CLASS_NAME));

        sRelationshipTypes.AddCriteria(individualGroup);
        sRelationshipTypes.AddCriteria(organizationGroup);

        sRelationshipTypes.AddSortColumn("DisplayOrder");
        sRelationshipTypes.AddSortColumn("Name");

        SearchResult srRelationshipTypes = APIExtensions.GetSearchResult(sRelationshipTypes, 0, null);
        dvRelationshipTypes = new DataView(srRelationshipTypes.Table);
    }

    protected msIndividual unbindAndSearch(string emailAddress)
    {
        Search sIndividual = new Search(msIndividual.CLASS_NAME);
        sIndividual.AddOutputColumn("ID");
        sIndividual.AddOutputColumn("Name");
        sIndividual.AddOutputColumn("FirstName");
        sIndividual.AddOutputColumn("LastName");

        SearchOperationGroup emailGroup = new SearchOperationGroup();
        emailGroup.FieldName = "EmailAddress";
        emailGroup.GroupType = SearchOperationGroupType.Or;
        emailGroup.Criteria.Add(Expr.Equals("EmailAddress", emailAddress));
        emailGroup.Criteria.Add(Expr.Equals("EmailAddress2", emailAddress));
        emailGroup.Criteria.Add(Expr.Equals("EmailAddress3", emailAddress));

        sIndividual.AddCriteria(emailGroup);

        sIndividual.AddSortColumn("LastName");
        sIndividual.AddSortColumn("FirstName");

        SearchResult srIndividual = APIExtensions.GetSearchResult(sIndividual, 0, null);

        if (srIndividual.Table.Rows.Count > 0)
         return LoadObjectFromAPI<msIndividual>(srIndividual.Table.Rows[0]["ID"].ToString());

        return null;
    }

    protected msIndividual unbindNewIndividual()
    {
        msIndividual result = new msIndividual();
        result.FirstName = tbFirstName.Text;
        result.LastName = tbLastName.Text;
        result.EmailAddress = tbEmailAddress.Text;

        return result;
    }

    protected bool activeRelationshipExists(msOrganization organization, msIndividual individual, msRelationshipType relationshipType)
    {
        Search sRelationship = new Search("RelationshipsForARecord");
        sRelationship.Context = organization.ID;
        sRelationship.AddOutputColumn("ID");
        sRelationship.AddCriteria(Expr.Equals("Type_ID", relationshipType.ID));
        sRelationship.AddCriteria(Expr.Equals("Target_ID", individual.ID));
        sRelationship.AddCriteria(Expr.Equals("IsActive", true));
        sRelationship.AddSortColumn("ID");

        SearchResult srRelationship = APIExtensions.GetSearchResult(sRelationship, 0, null);

        return srRelationship.TotalRowCount > 0;
    }

    #endregion

    #region Event Handlers

    protected void wizAddContact_CancelButtonClick(object sender, EventArgs e)
    {
        GoTo(string.Format("~/profile/ManageContacts.aspx?contextID={0}", ContextID));
    }

    protected void wizAddContact_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            case 0:
                lblAlreadyExists.Visible = false;
                targetEmailAddress = tbEmailAddress.Text;
                MultiStepWizards.AddContact.EmailAddress = targetEmailAddress;

                string relationshipTypeId = string.IsNullOrWhiteSpace(RelationshipTypeId)
                                                ? ddlRelationshipType.SelectedValue
                                                : RelationshipTypeId;
                targetRelationshipType = LoadObjectFromAPI<msRelationshipType>(relationshipTypeId);
                MultiStepWizards.AddContact.RelationshipType = targetRelationshipType;

                targetIndividual = unbindAndSearch(targetEmailAddress);

                try
                {
                    CRMLogic.ErrorOutIfOrganizationContactRestrictionApplies(
                        targetOrganization.ID,
                        targetRelationshipType.ID);
                }
                catch (Exception ex)
                {
                    cvContactRestriction.ErrorMessage = ex.Message;
                    cvContactRestriction.IsValid = false;
                    e.Cancel = true;
                    return;
                }

                if(targetIndividual != null)
                {
                    if(activeRelationshipExists(targetOrganization, targetIndividual, targetRelationshipType))
                    {
                        lblAlreadyExists.Text =
                            string.Format("Email Address <b>{0}</b> was found related to <b>{1} {2}</b>, who already has an active <b>{3}</b> relationship to <b>{4}</b>.", targetEmailAddress, targetIndividual.FirstName, targetIndividual.LastName, targetRelationshipType.Name, targetOrganization.Name);
                        lblAlreadyExists.Visible = true;

                        e.Cancel = true;
                        return;
                    }

                    MultiStepWizards.AddContact.Individual = targetIndividual;
                    wizAddContact.ActiveStepIndex = 2;
                }

                break;
            case 1:
                targetIndividual = unbindNewIndividual();
                MultiStepWizards.AddContact.Individual = targetIndividual;
                MultiStepWizards.AddContact.SendInvitation = chkSendInvitation.Checked;
                break;
        }
    }

    protected void wizAddContact_FinishButtonClick(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        
        targetIndividual = SaveObject(targetIndividual).ConvertTo<msIndividual>();
        
        msRelationship newRelationship = new msRelationship {Type = targetRelationshipType.ID};

        string targetEmail;

        if(targetRelationshipType.LeftSideType == msOrganization.CLASS_NAME)
        {
            newRelationship.LeftSide = targetOrganization.ID;
            newRelationship.RightSide = targetIndividual.ID;
            targetEmail = targetIndividual.EmailAddress;
        }
        else
        {
            newRelationship.RightSide = targetOrganization.ID;
            newRelationship.LeftSide = targetIndividual.ID;
            targetEmail = targetOrganization.EmailAddress;
        }

        newRelationship = SaveObject(newRelationship).ConvertTo<msRelationship>();


        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            //The API GetOrCreatePortalUser will attempt to match the supplied credentials to a portal user, individual, or organization. 
            //If a portal user is found it will be returned.  If not and an individual / organization uses the email address it will create and return a new Portal User
            var result = proxy.SearchAndGetOrCreatePortalUser(targetEmail);

            if (sendInvitation)
            {
                //Send the welcome email
                proxy.SendWelcomePortalUserEmail(result.ResultValue.ID, targetEmail);
            }
        }
        string nextUrl = "~/profile/ManageContacts.aspx";
        if(!string.IsNullOrWhiteSpace(ContextID))
            string.Format("{0}?contextID{1}", nextUrl, ContextID);

        GoTo(nextUrl, string.Format("{0} has been successfully linked to {1}.", targetIndividual.Name, targetOrganization.Name));
    }

    #endregion
}