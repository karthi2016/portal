using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_SubmitAbstract :  PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msAbstract targetAbstract;

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


        var contextObject = LoadObjectFromAPI(ContextID);

        switch (contextObject.ClassType)
        {
            case msEvent.CLASS_NAME:
                targetEvent = LoadObjectFromAPI<msEvent>(ContextID);
                targetAbstract = new msAbstract
                             {
                                 Owner = ConciergeAPI.CurrentEntity.ID,
                                 Event = targetEvent.ID
                             };
                break;
            case msAbstract.CLASS_NAME:
                targetAbstract = LoadObjectFromAPI<msAbstract>(ContextID);
                if(targetAbstract == null)
                {
                    GoToMissingRecordPage();
                    return;
                }
                targetEvent = LoadObjectFromAPI<msEvent>(targetAbstract.Event);
                break;
            default:
                QueueBannerError("Invalid context supplied.");
                GoHome();
                return;
        }

        if (targetEvent == null)
        {
            GoToMissingRecordPage();
            return;
        }
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        if (!targetEvent.EnableAbstracts) return false;

        if (targetEvent.AcceptAbstractsFrom != null && targetEvent.AcceptAbstractsFrom.Value > DateTime.Now)
            return false;

        if (targetEvent.AcceptAbstractsUntil != null && targetEvent.AcceptAbstractsUntil.Value < DateTime.Now)
            return false;

        if (!string.IsNullOrWhiteSpace(targetAbstract.ID)) return targetEvent.AllowEditAbstracts;

        return true;

        
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

        setupTracks();

        dataBind();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {

        if (ConciergeAPI.CurrentEntity == null)
            GoTo(string.Format("~/profile/CreateAccount_BasicInfo.aspx?completionUrl={0}",
                                                            Server.UrlEncode(
                                                                string.Format(
                                                                    "~/events/SubmitAbstract.aspx?contextID={0}",
                                                                    ContextID))));

        base.Page_Load(sender, e);
    }


    #endregion

    #region Methods

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        cfsAbstractCustomFields.MemberSuiteObject = targetAbstract;
        cfsAbstractCustomFieldsConfirm.MemberSuiteObject = targetAbstract;

        var pageLayout = GetAppropriatePageLayout(targetAbstract);
        divAdditionalInfo.Visible = false;

        if ((pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty()))
            return;

        divAdditionalInfoConfirm.Visible = divAdditionalInfo.Visible = true;

        // setup the metadata
        cfsAbstractCustomFields.Metadata = proxy.DescribeObject(msAbstract.CLASS_NAME).ResultValue;
        cfsAbstractCustomFields.PageLayout = pageLayout.Metadata;

        cfsAbstractCustomFields.Render();

        //The lifecycle here is a little strange because of the wizard.  Force a bind/harvest at this point to set the confirm fields
        cfsAbstractCustomFieldsConfirm.Metadata = cfsAbstractCustomFields.Metadata;
        cfsAbstractCustomFieldsConfirm.PageLayout = cfsAbstractCustomFields.PageLayout;

        cfsAbstractCustomFieldsConfirm.AddReferenceNamesToTargetObject(proxy);

        cfsAbstractCustomFieldsConfirm.Render();


    }

    protected void dataBind()
    {
        tbName.Text = targetAbstract.Name;
        tbDescription.Text = targetAbstract.Description;

        tbPresenterName.Text = targetAbstract.PresenterName;
        tbPresenterEmail.Text = targetAbstract.PresenterEmailAddress;
        tbPresenterPhone.Text = targetAbstract.PresenterPhoneNumber;
        tbPresenterBio.Text = targetAbstract.PresenterBiography;

        foreach (ListItem item in cblTracks.Items)
            item.Selected = targetAbstract.Tracks.Contains(item.Text);
    }

    private void setupTracks()
    {
        Search s = new Search(msSessionTrack.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        s.AddOutputColumn("Name");
        s.AddSortColumn("Name");

        var results = ExecuteSearch(s, 0, null);
        if (results.TotalRowCount == 0)
        {
            trTracks.Visible = false;
            return;
        }

        cblTracks.DataSource = results.Table;
        cblTracks.DataTextField = "Name";
        cblTracks.DataValueField = "ID";
        cblTracks.DataBind();
    }

    #endregion

    #region Event Handlers

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("ViewEvent.aspx?contextID=" + targetEvent.ID);
    }

    protected void Wizard1_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        targetAbstract.Name = tbName.Text;
        targetAbstract.Description = tbDescription.Text;

        string abstractsConfirm = null;
        if (cblTracks.Items.Count > 0)
        {
            targetAbstract.Tracks = new List<string>();
            foreach (ListItem li in cblTracks.Items)
                if (li.Selected)
                {
                    targetAbstract.Tracks.Add(li.Value);
                    abstractsConfirm += li.Text + ", ";
                }

            if (abstractsConfirm != null)
                SessionManager.Set("AbstractTracks",  abstractsConfirm.Trim().TrimEnd(',') );

        }


        targetAbstract.PresenterName = tbPresenterName.Text;
        targetAbstract.PresenterEmailAddress = tbPresenterEmail.Text;
        targetAbstract.PresenterPhoneNumber = tbPresenterPhone.Text;
        targetAbstract.PresenterBiography = tbPresenterBio.Text;
        
        cfsAbstractCustomFields.Harvest();
        cfsAbstractCustomFieldsConfirm.Bind();

        SessionManager.Set("Abstract",  targetAbstract );

        lblName.Text = targetAbstract.Name;
        
        if (targetAbstract.Description != null)
            lblDescription.Text = targetAbstract.Description.Replace("\r", "<BR/>");

        if (targetAbstract.Tracks == null || targetAbstract.Tracks.Count == 0)
        {
            trSessionTracksConfirm.Visible = false;
        }
        else
        {

            lblTracks.Text = SessionManager.Get<string>("AbstractTracks");
        }

        lblPresenterName.Text = targetAbstract.PresenterName;
        lblPresenterEmail.Text = targetAbstract.PresenterEmailAddress;
        lblPresenterContactNumber.Text = targetAbstract.PresenterPhoneNumber;
        lblPresenterBio.Text = targetAbstract.PresenterBiography;
        

    }

    protected void Wizard1_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        msAbstract ab = SessionManager.Get<msAbstract>("Abstract");
        if (ab == null)
            Response.Redirect(this.Request.Url.ToString()); // refresh

        using (var api = GetServiceAPIProxy())
        {
            // save the abstract
            var result = api.Save(ab);

            // and send the email
            api.SendEmail(EmailTemplates.Events.AbstractSubmission, new List<string> { result.ResultValue.SafeGetValue<string>("ID") },
                  null );

            QueueBannerMessage(string.Format("Abstract '{0}' submitted successfully.", ab.Name));

            // go back to the event
            Response.Redirect("ViewEvent.aspx?contextID=" + targetEvent.ID);
        }
    }

    #endregion
}