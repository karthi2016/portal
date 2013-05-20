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

public partial class competitions_ApplyToCompetition : PortalPage
{
    #region Fields

    protected msCompetition targetCompetition;
    protected msOrder targetOrder;
    protected CompetitionEntryInformation targetCompetitionInfo;

    #endregion

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }
    
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

        if (ConciergeAPI.CurrentEntity == null)   // split screen sign up!
        {
            string url = string.Format("/profile/CreateAccount_BasicInfo.aspx?pageTitle={0}&completionUrl={1}",
                                       Server.UrlEncode("Competition Registration"),
                                       Server.UrlEncode(Request.Url.ToString()));
            GoTo(url);
        }

        MultiStepWizards.EnterCompetition.EntryFee = null;
        targetCompetition = LoadObjectFromAPI(ContextID).ConvertTo<msCompetition>();
        if (targetCompetition == null)
        {
            GoToMissingRecordPage();
            return;
        }

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            targetCompetitionInfo = proxy.GetCompetitionEntryInformation(targetCompetition.ID, ConciergeAPI.CurrentEntity.ID).ResultValue;
        }

        if (targetCompetitionInfo == null)
        {
            GoToMissingRecordPage();
            return;
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

        setFeesAndContinueIfApplicable();

        if(!targetCompetitionInfo.CanEnter)
        {
            lblNoEntryFees.Visible = true;
            btnContinue.Enabled = false;
        }
        else
        {
            rblEntryFees.DataSource = targetCompetitionInfo.CompetitionEntryFees;
            rblEntryFees.DataBind();

            if (MultiStepWizards.EnterCompetition.EntryFee != null)
                rblEntryFees.SelectedValue = MultiStepWizards.EnterCompetition.EntryFee.ID;
        }
    }

    protected void setFeesAndContinueIfApplicable()
    {
        foreach (var fee in targetCompetitionInfo.CompetitionEntryFees)
        {

            fee.ProductName = string.Format("{0} - <font color=green>{1}</font>", fee.ProductName,
                                            fee.DisplayPriceAs ?? fee.Price.ToString("C"));
            if (fee.IsSoldOut)
                fee.ProductName += " SOLD OUT";

            if (!fee.IsEligible)
                fee.ProductName += " (ineligible)";
        }

        switch (targetCompetitionInfo.CompetitionEntryFees.Count)
        {
            case 0:
                MoveToNextStep();
                break;
            case 1: //If there's only one fee then auto-select it
                SetEntryFee(targetCompetitionInfo.CompetitionEntryFees[0].ProductID);
                if (targetCompetitionInfo.NumberOfDraftEntries + targetCompetitionInfo.NumberOfNonDraftEntries == 0) // but only move if we don't need to tell them about the existing entry
                    MoveToNextStep();
                break;
        }

    }

    protected void setExistingRegistrationCountLabel()
    {
        //Display a message if there are existing registrations for the current entity
        if (targetCompetitionInfo.NumberOfDraftEntries > 0)
        {
            lblDraftEntries.Text =
                string.Format(
                        "You currently have {0} entries in draft status. If you do not sumbit these by {1:d} at {2:t}, these entries will be discarded.",
                        targetCompetitionInfo.NumberOfDraftEntries, targetCompetition.CloseDate,
                        targetCompetition.CloseDate);
            lblDraftEntries.Visible = hlViewMyCompetitionEntries.Visible = true;
        }

        if (targetCompetitionInfo.NumberOfNonDraftEntries > 0)
        {
            lblExistingEntries.Text = string.Format("You have already submitted {0} entries for this competition.  ",
                                                    targetCompetitionInfo.NumberOfNonDraftEntries);
            lblExistingEntries.Visible = true;
        }
    }

    #endregion

    #region Methods

    protected void MoveToNextStep()
    {
        GoTo(string.Format("~/competitions/Enter_EntryForm.aspx?contextID={0}",targetCompetition.ID));
    }

    protected void SetEntryFee(string entryFeeId)
    {
        MultiStepWizards.EnterCompetition.EntryFee =
            LoadObjectFromAPI(entryFeeId).ConvertTo<msCompetitionEntryFee>();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        setExistingRegistrationCountLabel();
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        SetEntryFee(rblEntryFees.SelectedValue);
        MoveToNextStep();
    }

    #endregion
}