using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Command.Views;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;

public partial class competitions_Enter_EntryForm : PortalPage
{
    #region Field

    protected msCompetition targetCompetition;
    protected List<msCustomField> targetCompetitionQuestions;
    protected CompetitionEntryInformation targetCompetitionInfo;
    protected msCompetitionEntry targetCompetitionEntry;

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

        var contextObject = APIExtensions.LoadObjectFromAPI(ContextID);
        switch (contextObject.ClassType)
        {
            case "CompetitionEntry":
                targetCompetitionEntry = contextObject.ConvertTo<msCompetitionEntry>();
                targetCompetition = LoadObjectFromAPI<msCompetition>(targetCompetitionEntry.Competition);
                targetCompetitionInfo = targetCompetition.GetCompetitionEntryInformation();
                MultiStepWizards.EnterCompetition.EntryFee = LoadObjectFromAPI<msCompetitionEntryFee>(targetCompetitionEntry.EntryFee);
                break;

            case "Competition":
                targetCompetition = contextObject.ConvertTo<msCompetition>();
                targetCompetitionInfo = targetCompetition.GetCompetitionEntryInformation();
                targetCompetitionEntry = new msCompetitionEntry
                {
                    Competition = targetCompetition.ID,
                    Entrant = ConciergeAPI.CurrentEntity.ID,
                    Status = targetCompetitionInfo.PendingPaymentStatusId
                };

                // If the fee is null make sure it is allowed to be. If a session expires in the middle of the form, the user will be
                // redirected here without the proper Fee in session state.
                if (MultiStepWizards.EnterCompetition.EntryFee == null &&
                    targetCompetitionInfo.CompetitionEntryFees.Count > 0)
                {
                    if (targetCompetitionInfo.CompetitionEntryFees.Count > 1)
                    {
                        Response.Redirect("~/competitions/ApplyToCompetition.aspx?contextID=" + ContextID);
                    }

                    MultiStepWizards.EnterCompetition.EntryFee = LoadObjectFromAPI<msCompetitionEntryFee>(targetCompetitionInfo.CompetitionEntryFees[0].ProductID);
                }
                
                break;

            default:
                QueueBannerError("Unknown context object supplied.");
                GoHome();
                return;
        }

        if (targetCompetition == null || targetCompetitionEntry == null || targetCompetitionInfo == null)
        {
            GoToMissingRecordPage();
            return;
        }

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            var s = new Search(msCustomField.CLASS_NAME);
            s.AddCriteria(Expr.Equals(msCustomField.FIELDS.Competition, targetCompetition.ID));
            
            targetCompetitionQuestions =
                proxy.GetObjectsBySearch( s, null,0,null ).
                    ResultValue.Objects.ConvertTo<msCustomField>();

            // sort the competition questions - MS-2648
            targetCompetitionQuestions.Sort((x, y) => x.DisplayOrder.CompareTo(y.DisplayOrder));
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

        btnSaveAsDraft.Visible = !string.IsNullOrWhiteSpace(targetCompetitionInfo.DraftStatusId);

        //Bind
        tbEntryName.Text = targetCompetitionEntry.Name;
    }

    protected ClassMetadata createClassMetadataFromEntryQuestions()
    {
        var result = new ClassMetadata();
        result.Fields = (from q in targetCompetitionQuestions select q.FieldDefinition).ToList();
        return result;
    }

    protected DataEntryViewMetadata createViewMetadataFromEntryQuestions()
    {
        var result = new DataEntryViewMetadata();
        var baseSection = new ViewMetadata.ControlSection();
        baseSection.SubSections = new List<ViewMetadata.ControlSection>();

        var currentSection = new ViewMetadata.ControlSection();
        currentSection.LeftControls = new List<ControlMetadata>();
        baseSection.SubSections.Add(currentSection);
        foreach (var question in targetCompetitionQuestions)
        {
            var field = new ControlMetadata { DataSourceExpression = question.Name };

            // This isn't necessary - the field is aqequately described already 
            ////ControlMetadata field = ControlMetadata.FromFieldMetadata(question.FieldDefinition);
            
            if (field.DisplayType == FieldDisplayType.Separator)
            {
                if (currentSection.LeftControls != null && currentSection.LeftControls.Count > 0)
                // we have to create a new section
                {
                    currentSection = new ViewMetadata.ControlSection();
                    currentSection.LeftControls = new List<ControlMetadata>();
                    baseSection.SubSections.Add(currentSection);
                }

                currentSection.Name = Guid.NewGuid().ToString();
                currentSection.Label = field.PortalPrompt ?? field.Label; // set the section label
                continue;
            }

            currentSection.LeftControls.Add(field);
        }

        result.Sections = new List<ViewMetadata.ControlSection> {baseSection};
        return result;
    }

    #endregion

    #region Data Binding

    protected void unbind()
    {
        // MS-5451 Do not allow entrants to exceed the maximum number of entries per entrant
        var searchCompetitionEntries = new Search {Type = msCompetitionEntry.CLASS_NAME};
        searchCompetitionEntries.AddCriteria(Expr.Equals("Entrant", ConciergeAPI.CurrentEntity.ID));
        searchCompetitionEntries.AddCriteria(Expr.Equals("Competition", targetCompetition.ID));
        searchCompetitionEntries.AddCriteria(Expr.Equals("Status", targetCompetitionInfo.SubmittedStatusId));

        var searchResultsCompetitionEntries = APIExtensions.GetSearchResult(searchCompetitionEntries, 0, null);

        if (!targetCompetitionInfo.CanEnter ||
            searchResultsCompetitionEntries.TotalRowCount >= targetCompetition.MaximumNumberOfEntriesPerEntrant)
        {
            QueueBannerMessage("Please ensure that the competition is still open and that you will not exceed the maximum number of entries with this submission.");
            GoHome();
        }

        targetCompetitionEntry.Name = tbEntryName.Text;
        targetCompetitionEntry.DateSubmitted = DateTime.UtcNow;

        if (MultiStepWizards.EnterCompetition.EntryFee != null)
        {
            targetCompetitionEntry.Status = targetCompetitionInfo.PendingPaymentStatusId;
            targetCompetitionEntry.EntryFee = MultiStepWizards.EnterCompetition.EntryFee.ID;
        }
        else targetCompetitionEntry.Status = targetCompetitionInfo.SubmittedStatusId;

        cfsEntryFields.Harvest();
    }

    #endregion

    #region Methods

    protected void SaveAndGoToNextStep()
    {

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            targetCompetitionEntry = proxy.Save(targetCompetitionEntry).ResultValue.ConvertTo<msCompetitionEntry>();


            if (MultiStepWizards.EnterCompetition.EntryFee != null)
            {
                GoToOrderForm();
                return;
            }

            // send out the confirmation
            proxy.SendTransactionalEmail(
                targetCompetition.ConfirmationEmail ?? EmailTemplates.Awards.CompetitionEntrySubmissionConfirmation,
                targetCompetitionEntry.ID, null);
        }

        QueueBannerMessage(string.Format("Competition Entry #{0} was created successfully.",
                                       targetCompetitionEntry.LocalID));
        GoHome();
    }


    protected void GoToOrderForm()
    {
        msOrder newOrder = new msOrder();
        newOrder.BillTo = newOrder.ShipTo = ConciergeAPI.CurrentEntity.ID;
        newOrder.LineItems = new List<msOrderLineItem>
                                 {
                                     new msOrderLineItem
                                         {
                                             Quantity = 1,
                                             Product = MultiStepWizards.EnterCompetition.EntryFee.ID,
                                             Options = new List<NameValueStringPair>(){ new NameValueStringPair("CompetitionEntryId",targetCompetitionEntry.ID)}
                                         }
                                 };
        
        
        //Add the order as the "shopping cart" for the order processing wizard
        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(newOrder);
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

        //Dynamically render the registration fields - this has to be in the Page_Load not InitializePage for the Harvest to work properly
        cfsEntryFields.Metadata = createClassMetadataFromEntryQuestions();
        cfsEntryFields.PageLayout = createViewMetadataFromEntryQuestions();
        cfsEntryFields.MemberSuiteObject = targetCompetitionEntry;
        cfsEntryFields.Render();
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MultiStepWizards.EnterCompetition.EntryFee = null;
        GoHome();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("~/competitions/ApplyToCompetition.aspx?contextID={0}", ContextID));
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        unbind();
        SaveAndGoToNextStep();
    }

    protected void btnSaveAsDraft_Click(object sender, EventArgs e)
    {
        unbind();
        targetCompetitionEntry.Status = targetCompetitionInfo.DraftStatusId;

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            targetCompetitionEntry = proxy.Save(targetCompetitionEntry).ResultValue.ConvertTo<msCompetitionEntry>();
        }

        QueueBannerMessage(string.Format("Competition Entry #{0} was successfully saved as a draft.",
                                       targetCompetitionEntry.LocalID));
        GoHome();
    }


    #endregion
}