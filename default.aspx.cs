using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASP;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using System.Configuration;

public partial class Home : PortalPage
{
    #region Properties


    #endregion



    protected override void InitializePage()
    {
        base.InitializePage();

        ((App_Master_GeneralPage)Page.Master).HideHomeBreadcrumb = true;

        // We have a bunch of searches that we need to run to get the information for
        // this page. If we run a Multi-Search, the performance is much better than running them
        // one by run

        var results = runAllNecessarySearches();

        retrievePortalFormInformation();

        //Initialize each widget - this will show/hide and bind widgets if they are active
        ucMyProfile1.DeliverSearchResults(results);
        ucMyProfile1.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "CRM"));

        if (IsModuleActive("Membership"))
        {
            ucMembership.DeliverSearchResults(results);
            ucMembership.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Membership"));
        }

        if (IsModuleActive("Committees"))
        {
            ucCommittees.DeliverSearchResults(results);
            ucCommittees.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Committees"));
        }

        if (IsModuleActive("Financial"))
        {
            ucMyAccount.DeliverSearchResults(results);
            ucMyAccount.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Financial"));
        }

        if (IsModuleActive("Events"))
        {
            ucEvents.DeliverSearchResults(results);
            ucEvents.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Events"));
        }

        if (IsModuleActive("Fundraising"))
        {
            ucFundraising.DeliverSearchResults(results);
            ucFundraising.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Fundraising"));
        }

        if (IsModuleActive("Certifications"))
        {
            ucCEU.DeliverSearchResults(results);
            ucCEU.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Certifications"));
        }

        if (IsModuleActive("Awards"))
        {
            ucCompetitions.DeliverSearchResults(results);
            ucCompetitions.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Awards"));
        }

        if (IsModuleActive("CareerCenter"))
        {
            ucCareerCenter.DeliverSearchResults(results);
            ucCareerCenter.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "CareerCenter"));
        }

        if (IsModuleActive("Subscriptions"))
        {
            ucSubscriptions.DeliverSearchResults(results);
            ucSubscriptions.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Subscriptions"));
        }

        if (IsModuleActive("Discussions"))
        {
            ucDiscussions.DeliverSearchResults(results);
            ucDiscussions.GenerateFormLinks(forms.Forms.FindAll(x => x.Module == "Discussions"));
        }

        // process alerts
        processAnyAlerts(results);

        // set the home page title
        lHomePageTitle.Text = string.Format("Welcome to {0}",
            ConciergeAPI.CurrentAssociation.Name);

        //Client caching the home page does not re-evaluate Page_Load so all modules are disabled when using the back button
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        
    }

    PortalFormsManifest forms;
    private void retrievePortalFormInformation()
    {
        
        using( var api = GetServiceAPIProxy())
        forms = api.GetAccessiblePortalForms( ConciergeAPI.CurrentEntity.ID ).ResultValue;

        forms.Forms.RemoveAll(x => !x.DisplayOnHomeScreen);
    }

    private void processAnyAlerts(List<SearchResult> results)
    {
        // billing schedule
        SearchResult sr = results.Single(x => x.ID == "BillingSchedules");
        if (sr.TotalRowCount > 0)
            divDeclinedPayments.Visible = true;
    }

    private List<SearchResult> runAllNecessarySearches()
    {
        List<Search> searchesToRun = new List<Search>();

        // first, the main search
        Search sMain = new Search { Type = CurrentEntity.ClassType, ID = "Main" };
        sMain.AddCriteria(Expr.Equals("ID", CurrentEntity.ID));

        // check for failed deferred billing schedules
        Search sBillingSchedule = new Search(msBillingSchedule.CLASS_NAME)
                                      {
                                          ID = "BillingSchedules"
                                      };
        sBillingSchedule.AddCriteria(Expr.Equals("Order.BillTo", CurrentEntity.ID));
        sBillingSchedule.AddCriteria(Expr.Equals(msBillingSchedule.FIELDS.Status, "Suspended"));
        searchesToRun.Add(sBillingSchedule);

        // always prepare the main widget
        prepareWidget(sMain, searchesToRun, ucMyProfile1);

        //Accounting Widget
        if (IsModuleActive("Financial"))
            prepareWidget(sMain, searchesToRun, ucMyAccount);
        else
            ucMyAccount.Visible = false;


        //Membership Widget
        if (IsModuleActive("Membership"))
            prepareWidget(sMain, searchesToRun, ucMembership);
        else
            ucMembership.Visible = false;

        //Committees Widget
        if (IsModuleActive("Committees"))
            prepareWidget(sMain, searchesToRun, ucCommittees);
        else
            ucCommittees.Visible = false;

        if (IsModuleActive("Events"))
            prepareWidget(sMain, searchesToRun, ucEvents);
        else
            ucEvents.Visible = false;

        if (IsModuleActive("Fundraising"))
            prepareWidget(sMain, searchesToRun, ucFundraising);
        else
            ucFundraising.Visible = false;

        if (IsModuleActive("Certifications"))
            prepareWidget(sMain, searchesToRun, ucCEU);
        else
            ucCEU.Visible = false;

        if (IsModuleActive("Awards"))
            prepareWidget(sMain, searchesToRun, ucCompetitions);
        else
            ucCompetitions.Visible = false;

        if (IsModuleActive("CareerCenter"))
            prepareWidget(sMain, searchesToRun, ucCareerCenter);
        else
            ucCareerCenter.Visible = false;

        if (IsModuleActive("Subscriptions"))
            prepareWidget(sMain, searchesToRun, ucSubscriptions);
        else
            ucSubscriptions.Visible = false;

        if (IsModuleActive("Discussions"))
            prepareWidget(sMain, searchesToRun, ucDiscussions);
        else
            ucDiscussions.Visible = false;


        searchesToRun.Add(sMain);

        addCustomSearches(searchesToRun);

        return ExecuteSearches(searchesToRun, 0, null);


    }

    private void prepareWidget(Search sMain, List<Search> searchesToRun, HomePageUserControl hpControl)
    {
        foreach (var outputColumn in hpControl.GetFieldsNeededForMainSearch())
            if (!sMain.OutputColumns.Exists(x => x.Name == outputColumn))
                sMain.AddOutputColumn(outputColumn);

        hpControl.GenerateSearchesToBeRun(searchesToRun);
    }

    private void addCustomSearches(List<Search> searchesToRun)
    {

    }

}


