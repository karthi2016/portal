using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Types;

public partial class homepagecontrols_MyProfile : HomePageUserControl 
{

    protected override void InitializeWidget()
    {
        base.InitializeWidget();
        populateMyProfile();
        setProfileEditLink();

        liManageContacts.Visible = ConciergeAPI.CurrentEntity.ClassType == msOrganization.CLASS_NAME;
    }

    #region My Profile

    private void populateMyProfile()
    {
        if (!PortalPage.setProfileImage(img, ConciergeAPI.CurrentEntity))
            tdProfileImage.Visible = false;

        _populatePreferredAddress();

        // set the email
        var e = ConciergeAPI.CurrentEntity;
        if (e.EmailAddress != null)
        {
            hlEmail.NavigateUrl = "mailto:" + e.EmailAddress;
            hlEmail.Text = e.EmailAddress;

        }
        // set the phone numbers
        _populatePreferredPhoneNumber();

        setLoginAs();

        _populateDigitalLocker();
        _setupMyReports();
    }

    private void _setupMyReports()
    {
        List<EntitlementReport> report;
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            report =
                api.ListEntitlements(ConciergeAPI.CurrentEntity.ID, msSearchEntitlement.CLASS_NAME).
                    ResultValue;

        liReports.Visible = report != null && report.Count > 0;

    }

    private void _populateDigitalLocker()
    {
        List<EntitlementReport> report;
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            report =
                api.ListEntitlements( ConciergeAPI.CurrentEntity.ID, msFileFolderEntitlement.CLASS_NAME).
                    ResultValue;

        liDigitalLibrary.Visible = report != null && report.Count> 0;
        
    }

    private void _populatePreferredPhoneNumber()
    {
        var e = ConciergeAPI.CurrentEntity;
        if (e.PhoneNumbers == null || e.PhoneNumbers.Count == 0)
            return;

        // see if we can get the default
        var defaultPhoneNumber = e.PhoneNumbers.ToList().Find(x => x.Type == e.PreferredPhoneNumberType);

        if (defaultPhoneNumber == null)   // no default, let's use the first
            defaultPhoneNumber = e.PhoneNumbers[0];


        lPhoneNumber.Text = defaultPhoneNumber.PhoneNumber;  // set the address
    }

    private void _populatePreferredAddress()
    {

        var e = ConciergeAPI.CurrentEntity;
        var defaultAddress = Utils.GetEntityPreferredAddress(e);
        if ( defaultAddress != null && defaultAddress.Address != null )
            lAddress.Text = defaultAddress.Address.ToHtmlString();  // set the address
    }

    private void setProfileEditLink()
    {
        if (ConciergeAPI.CurrentEntity.ClassType == "Organization")
            hlEditMyInfo.NavigateUrl = "~/profile/EditOrganizationInfo.aspx";

    }

    private void setLoginAs()
    {
        divLoginAs.Visible = false;

        if(ConciergeAPI.AccessibleEntities == null || ConciergeAPI.AccessibleEntities.Count == 0)
            return;

        var accessibleEntities =
            ConciergeAPI.AccessibleEntities.Where(
                x => !x.ID.Equals(ConciergeAPI.CurrentEntity.ID, StringComparison.CurrentCultureIgnoreCase)).ToList();

        if(accessibleEntities.Count == 0)
            return;

        divLoginAs.Visible = true;
        lblLoginAsInstructions.Text =
            string.Format(
                "You are linked to {0} other record(s). You can login as them to update or review their account information:", accessibleEntities.Count);

        foreach (var e in accessibleEntities)
            blLoginAs.Items.Add(new ListItem(string.Format("{0} ({1})", e.Name, e.Type), e.ID));
    }

    #endregion

    #region Event Handlers

    protected void blLoginAs_Click(object sender, BulletedListEventArgs e)
    {
        ListItem li = blLoginAs.Items[e.Index];
        string loginAsEntityId = li.Value;

        if (!ConciergeAPI.AccessibleEntities.Exists(x => x.ID.Equals(loginAsEntityId, StringComparison.CurrentCultureIgnoreCase)))
            return;

        msEntity newEntity = null;
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            newEntity = api.Get(loginAsEntityId).ResultValue.ConvertTo<msEntity>();


            if (newEntity == null) return; // entity was erased?
            
             var newAccessibleEntities = api.GetAccessibleEntitiesForEntity(newEntity.ID).ResultValue;
            if(newAccessibleEntities == null)
                newAccessibleEntities = new List<LoginResult.AccessibleEntity>();

            //MS-1391
            // automatically insert the current entity so there's a login link to switch back
             if (!newAccessibleEntities.Exists(x => x.ID == ConciergeAPI.CurrentEntity.ID))
                 newAccessibleEntities.Insert(0, new LoginResult.AccessibleEntity
                {
                    ID = ConciergeAPI.CurrentEntity.ID,
                    Name = ConciergeAPI.CurrentEntity.Name,
                    Type = ConciergeAPI.CurrentEntity.ClassType
                });

            ConciergeAPI.AccessibleEntities = newAccessibleEntities;
            ConciergeAPI.CurrentEntity = newEntity;

            // record that this was the last one logged in
            // let's re-load it from the database
            var pu = api.Get(ConciergeAPI.CurrentUser.ID).ResultValue.ConvertTo<msPortalUser>();
            pu.LastLoggedInAs = newEntity.ID;
            api.Save(pu);
        }

        MultiStepWizards.ClearAll();
        Response.Redirect("/default.aspx");

    }

    #endregion

    public override List<string> GetFieldsNeededForMainSearch()
    {
        var list = base.GetFieldsNeededForMainSearch();
        list.Add("PrimaryOrganization.Name");
        list.Add("Company");

        if ( PortalPage.IsModuleActive("Volunteers" ))
            list.Add("Volunteers_VolunteerRecord");

        return list;
    }

    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);

        try
        {
            string company = Convert.ToString(drMainRecord["PrimaryOrganization.Name"]);
            if (string.IsNullOrWhiteSpace(company))
                company = Convert.ToString(drMainRecord["Company"]);
            
            if (!string.IsNullOrWhiteSpace(company))
                lblCompany.Text = company + "<BR/>";

            if (PortalPage.IsModuleActive("Volunteers"))
            {
                string volunteerID = Convert.ToString(drMainRecord["Volunteers_VolunteerRecord"]);
                if ( ! string.IsNullOrWhiteSpace(  volunteerID ))
                {
                    liVolunteer.Visible = true;
                    hlViewVolunteerProfile.NavigateUrl += volunteerID;
                }
            }

        }
        catch
        {
            // catch, if the field is missing, hidden, or if it's a company
        }

    }
}
