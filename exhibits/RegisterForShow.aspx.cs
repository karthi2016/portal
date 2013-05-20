using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class exhibits_RegisterForShow : PortalPage
{
    public msExhibitShow targetShow;
    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetShow = LoadObjectFromAPI<msExhibitShow>(ContextID);
        if (targetShow == null) GoToMissingRecordPage();
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        if (ConciergeAPI.CurrentEntity == null) // we need them to go through registration
        {
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=ExhibitShow&showID=" + targetShow.ID);
            return;
        }

        var relatedOrganizations = getRelatedOrganizations(ConciergeAPI.CurrentEntity.ID);

        if ( relatedOrganizations == null || relatedOrganizations.Count == 0)
            initiateRegistration(ConciergeAPI.CurrentEntity.ID);

        //Add the item for the current entity
        rblEntity.Items.Add(
            new ListItem(string.Format("I am registering for myself, <b>{0}</b>", ConciergeAPI.CurrentEntity.Name),
                         ConciergeAPI.CurrentEntity.ID));
        rblEntity.Items[0].Selected = true;

        rblEntity.DataSource = relatedOrganizations;
        rblEntity.DataBind();

    }

    private void initiateRegistration(string entityID)
    {
        using (var api = GetConciegeAPIProxy())
        {

            // is there a registration already?
            Search s = new Search(msExhibitor.CLASS_NAME);
            s.AddCriteria(Expr.Equals(msExhibitor.FIELDS.Show, targetShow.ID));
            s.AddCriteria(Expr.Equals(msExhibitor.FIELDS.Customer, entityID));

            if (api.ExecuteSearch(s, 0, 1).ResultValue.TotalRowCount > 0)
            {
                QueueBannerError("Unable to continue - an exhibitor record is already present for " +
                                 api.GetName(entityID).ResultValue);
                GoTo("ViewShow.aspx?contextID=" + targetShow.ID);
            }

            var windows = api.GetAvailableExhibitorRegistrationWindows(targetShow.ID, entityID).ResultValue;

            var window = windows.Permissions.FirstOrDefault(x => x.EntityID == entityID);
            if ( window == null )
                GoTo( string.Format( "RegistrationNotAvailable.aspx?contextID={0}&entityID={1}", targetShow.ID, entityID ) );

            switch (window.RegistrationMode)
            {
                case ExhibitorRegistrationMode.PurchaseBoothsByNumber:
                    GoTo( string.Format( "RegisterForBooths.aspx?contextID={0}&entityID={1}", window.RegistrationWindowID, entityID ));
                    break;

                case ExhibitorRegistrationMode.PurchaseBoothsByType:
                    GoTo(string.Format("RegisterForBoothTypes.aspx?contextID={0}&entityID={1}", window.RegistrationWindowID, entityID));
                    break;

                case ExhibitorRegistrationMode.IndicateBoothPreferencesOnly:
                    GoTo(string.Format("RegisterForBoothPreferences.aspx?contextID={0}&entityID={1}", window.RegistrationWindowID, entityID));
                    break;

                default:
                    throw new NotSupportedException("Unable to support registration mode " + window.RegistrationMode);
            }
        }
        
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        initiateRegistration(rblEntity.SelectedValue);

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewShow.aspx?contextID=" + targetShow.ID);
    }
}