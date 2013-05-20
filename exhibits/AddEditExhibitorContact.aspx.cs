using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class exhibits_AddEditExhibitorContact : PortalPage
{
    public msExhibitor targetExhibitor;
    public msExhibitShow targetShow;
    msExhibitorContact targetContact;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetExhibitor = LoadObjectFromAPI<msExhibitor>(ContextID);
        if (targetExhibitor == null) GoToMissingRecordPage();

        targetShow = LoadObjectFromAPI<msExhibitShow>(targetExhibitor.Show);
        if (targetShow == null) GoToMissingRecordPage();

        int index;
        if (int.TryParse(Request.QueryString["itemIndex"], out index) && targetExhibitor.Contacts != null
            && targetExhibitor.Contacts.Count >= index)
            targetContact = targetExhibitor.Contacts[index];
    }
     

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        using (var api = GetConciegeAPIProxy())
            return ExhibitorLogic.CanEditExhibitorRecord(api, targetExhibitor, ConciergeAPI.CurrentEntity.ID);
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        Search s = new Search( msExhibitorContactType.CLASS_NAME );
        s.AddOutputColumn( "Name" );
        s.AddSortColumn( "DisplayOrder" );
        s.AddSortColumn( "Name");
        s.AddCriteria( Expr.Equals( "IsActive", true ) );
        ddlType.DataSource = ExecuteSearch(s, 0, null).Table;

        ddlType.DataTextField = "Name";
        ddlType.DataValueField = "ID";
        ddlType.DataBind();

        if (targetContact != null)
        {
            ListItem li = ddlType.Items.FindByValue(targetContact.Type);
            if (li == null)
            {
                li = new ListItem("[Inactive Type]", targetContact.Type);
                ddlType.Items.Add(li);
            }
            li.Selected = true;

            tbFirstName.Text = targetContact.FirstName;
            tbLastName.Text = targetContact.LastName;
            tbEmail.Text = targetContact.EmailAddress;
            tbWorkPhone.Text = targetContact.WorkPhone;
            tbMobilePhone.Text = targetContact.MobilePhone;
        }
    }

    

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        // check to see if there's a restriction
        string contactTypeID = ddlType.SelectedValue;
        using (var api = GetServiceAPIProxy())
        {
            var restriction = api.CheckForExhibitorContactRestriction(targetExhibitor.ID, contactTypeID).ResultValue;
            if (restriction != null && restrictionHasBeenViolated( restriction.ConvertTo<msExhibitorContactRestriction>()))
            {

                cvContactRestriction.ErrorMessage = restriction.SafeGetValue<string>("ErrorMessage") ??
                                                    "Unable to add contact - a restriction is in place: " +
                                                    restriction["Name"];
                cvContactRestriction.IsValid = false;
                return;
            }
        }

        if (targetContact == null)
        {
            targetContact = new msExhibitorContact();
            if (targetExhibitor.Contacts == null)
                targetExhibitor.Contacts = new List<msExhibitorContact>();

            targetExhibitor.Contacts.Add(targetContact);
        }
        targetContact.Type = ddlType.SelectedValue;
        targetContact.FirstName = tbFirstName.Text;
        targetContact.LastName = tbLastName.Text;
        targetContact.EmailAddress = tbEmail.Text;
        targetContact.WorkPhone = tbWorkPhone.Text;
        targetContact.MobilePhone = tbMobilePhone.Text;

     

        SaveObject(targetExhibitor);

        GoTo("ViewExhibitor.aspx?contextID=" + ContextID, "The contact has been saved successfully.");
    }

    /// <summary>
    /// Checks to see if the contact restirction has bee violated
    /// </summary>
    /// <param name="restrictionToCheck"></param>
    /// <returns></returns>
    private bool restrictionHasBeenViolated(msExhibitorContactRestriction restrictionToCheck)
    {
        if (targetExhibitor.Contacts == null)
            targetExhibitor.Contacts = new List<msExhibitorContact>();  // this makes it easier

        List<msExhibitorContact> contactsToCheck = new List<msExhibitorContact>();
        if ( targetExhibitor.Contacts != null )
            contactsToCheck.AddRange( targetExhibitor.Contacts );

        // now, remove the current contact
        if (targetContact != null)
            contactsToCheck.Remove(targetContact);

        if (restrictionToCheck.ContactType == null)
            return contactsToCheck.Count >= restrictionToCheck.MaximumNumberOfContacts;
        else
            return contactsToCheck.Count(x => x.Type == restrictionToCheck.ContactType) >=
                restrictionToCheck.MaximumNumberOfContacts;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewExhibitor.aspx?contextID=" + ContextID);
    }
}