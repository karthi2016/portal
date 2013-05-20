using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Command.Views;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class competitions_ViewCompetitionEntry : PortalPage
{
    #region Fields

    protected msCompetitionEntry targetCompetitionEntry;
    protected msCompetition targetCompetition;
    protected List<msCustomField> targetCompetitionQuestions;
    protected msCompetitionEntryStatus targetEntryStatus;
    protected DataRow drCompetitionEntry;
    protected decimal totalScore;
    protected decimal averageScore;

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

        targetCompetitionEntry = LoadObjectFromAPI<msCompetitionEntry>(ContextID);

        if(targetCompetitionEntry == null)
        {
            GoToMissingRecordPage();
            return;
        }


        targetCompetition = LoadObjectFromAPI<msCompetition>(targetCompetitionEntry.Competition);

        if (targetCompetition == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEntryStatus = LoadObjectFromAPI<msCompetitionEntryStatus>(targetCompetitionEntry.Status);

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            Search s = new Search(msCustomField.CLASS_NAME);
            s.AddCriteria(Expr.Equals(msCustomField.FIELDS.Competition, targetCompetition.ID));

            targetCompetitionQuestions =
               proxy.GetObjectsBySearch(s, null, 0, null).
                   ResultValue.Objects.ConvertTo<msCustomField>();
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

        cfsEntryFields.Metadata = createClassMetadataFromEntryQuestions();
        cfsEntryFields.PageLayout = createViewMetadataFromEntryQuestions();
        cfsEntryFields.MemberSuiteObject = targetCompetitionEntry;
        cfsEntryFields.Render();

        loadDataFromConcierge();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();
        
        //Competition Entry with expanded reference fields
        Search sCompetitionEntry = new Search(msCompetitionEntry.CLASS_NAME)
                                       {
                                           ID = msCompetitionEntry.CLASS_NAME
                                       };
        sCompetitionEntry.AddOutputColumn("ID");
        sCompetitionEntry.AddOutputColumn("Name");
        sCompetitionEntry.AddOutputColumn("LocalID");
        sCompetitionEntry.AddOutputColumn("DateSubmitted");
        sCompetitionEntry.AddOutputColumn("Entrant.Name");
        sCompetitionEntry.AddOutputColumn("Individual.PrimaryOrganization.Name");
        sCompetitionEntry.AddOutputColumn("Individual.PrimaryOrganization._Preferred_Address_City");
        sCompetitionEntry.AddOutputColumn("Individual.PrimaryOrganization._Preferred_Address_State");
        sCompetitionEntry.AddCriteria(Expr.Equals("ID", targetCompetitionEntry.ID));
        sCompetitionEntry.AddSortColumn("ID");

        searches.Add(sCompetitionEntry);

        //Scores
        Search sScoreCards = new Search(msScoreCard.CLASS_NAME)
                                 {
                                     ID = msScoreCard.CLASS_NAME
                                 };
        sScoreCards.AddOutputColumn("ID");
        sScoreCards.AddOutputColumn("Entry.ID");
        sScoreCards.AddOutputColumn("TotalScore");
        sScoreCards.AddCriteria(Expr.Equals("Entry.ID", targetCompetitionEntry.ID));
        sScoreCards.AddSortColumn("ID");

        searches.Add(sScoreCards);

        List<SearchResult> searchResults = ExecuteSearches(searches, 0, null);

        SearchResult srCompetitionEntries = searchResults.Single(x => x.ID == msCompetitionEntry.CLASS_NAME);
        if (srCompetitionEntries.Table != null && srCompetitionEntries.Table.Rows.Count > 0)
            drCompetitionEntry = srCompetitionEntries.Table.Rows[0];

        //Calculate average and total scores
        totalScore = averageScore = 0;

        SearchResult srScoreCards = searchResults.Single(x => x.ID == msScoreCard.CLASS_NAME);
        if(srScoreCards.Table != null)
        {
            foreach (DataRow drScoreCard in srScoreCards.Table.Rows)
                totalScore += (decimal) drScoreCard["TotalScore"];

            if (srScoreCards.Table.Rows.Count > 0)
                averageScore = totalScore/srScoreCards.Table.Rows.Count;
        }
    }

    protected ClassMetadata createClassMetadataFromEntryQuestions()
    {
        ClassMetadata result = new ClassMetadata();
        result.Fields = (from q in targetCompetitionQuestions select q.FieldDefinition).ToList();
        return result;
    }

    protected DataEntryViewMetadata createViewMetadataFromEntryQuestions()
    {
        DataEntryViewMetadata result = new DataEntryViewMetadata();
        ViewMetadata.ControlSection baseSection = new ViewMetadata.ControlSection();
        baseSection.SubSections = new List<ViewMetadata.ControlSection>();

        var currentSection = new ViewMetadata.ControlSection();
        currentSection.LeftControls = new List<ControlMetadata>();
        baseSection.SubSections.Add(currentSection);
        foreach (var question in targetCompetitionQuestions)
        {
            ControlMetadata field = ControlMetadata.FromFieldMetadata(question.FieldDefinition);

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
        result.Sections = new List<ViewMetadata.ControlSection> { baseSection };
        return result;
    }


    #endregion
}