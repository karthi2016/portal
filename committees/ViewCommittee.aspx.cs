using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class committee_ViewCommittee : PortalPage
{
    #region Fields

    protected msCommittee targetCommittee;
    protected DataView dvCommitteeTerms;
    protected DataView dvCommitteeMembers;

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

        targetCommittee = LoadObjectFromAPI<msCommittee>(ContextID);
        if (targetCommittee == null || !targetCommittee.ShowInPortal) GoToMissingRecordPage();
    }

    protected override bool CheckSecurity()
    {
        if(!base.CheckSecurity())
            return false;

        return targetCommittee.ShowInPortal;
    }

    #endregion

    

    protected override void InitializePage()
    {
        base.InitializePage();

        //Execute searches as multi-search to improve performance
        loadDataFromConcierge();

        //Add committee terms to the filter drop down list
        ddlDisplayMembers.DataSource = dvCommitteeTerms;
        ddlDisplayMembers.DataBind();

        bindCommitteeMembers();

        setupJoinRemove();

        setupDocuments();

        setupCommitteeAdminLinks();

        hlDiscussionBoard.Visible = IsModuleActive("Discussions");
        hlDiscussionBoard.NavigateUrl += ContextID;
    }

    private void setupDocuments()
    {
        if (!isOnCommittee())
            return;

        hlViewCommitteeDocuments.Visible = IsModuleActive("Documents");
        hlViewCommitteeDocuments.NavigateUrl += ContextID;
    }

    private void setupJoinRemove()
    {
        if (ConciergeAPI.CurrentEntity == null)
            return; // can't join if not logged in

        if (targetCommittee.IsOpen)
        {
            if (isInApplicableMembershipType())
            {
                // check to see if I'm on the committee
                if (isOnCommittee())
                {
                    lbRemove.Visible = true;
                    RegisterJavascriptConfirmationBox(lbRemove,
                                                      "Are you sure you want to remove yourself from this committee?");


                    return;
                }

                lbJoin.Visible = true;
                RegisterJavascriptConfirmationBox(lbJoin, "Are you sure you want to join this committee?");
            }
            else lblUnableToJoin.Visible = true;
        }
    }

    private bool isOnCommittee()
    {
        // check to see if I'm on the committee
        foreach (DataRowView dr in dvCommitteeMembers)
            if (Convert.ToString(dr["Member.ID"]) == ConciergeAPI.CurrentEntity.ID)
                return true;

        return false;
               
    }

    #region Methods

    /// <summary>
    /// Executes all querys required for the current page against the Concierge API in a multi-search fashion to improve performance
    /// </summary>
    private void loadDataFromConcierge()
    {
        List<Search> searchesToRun = new List<Search>();

        //Committee terms related to target committee
        Search sCommitteeTerm = new Search { Type = msCommitteeTerm.CLASS_NAME };
        sCommitteeTerm.AddOutputColumn("ID");
        sCommitteeTerm.AddOutputColumn("Name");
        sCommitteeTerm.AddCriteria(Expr.Equals("Committee.ID", ContextID));
        sCommitteeTerm.AddSortColumn("Name", true);
        searchesToRun.Add(sCommitteeTerm);

        //Committee Memberships related to target committee
        Search sCommitteeMembership = new Search { Type = msCommitteeMembership.CLASS_NAME };
        sCommitteeMembership.AddOutputColumn("Member.ID");
        sCommitteeMembership.AddOutputColumn("Member.Name");
        sCommitteeMembership.AddOutputColumn("Position.Name");
        sCommitteeMembership.AddOutputColumn("EffectiveStartDate");
        sCommitteeMembership.AddOutputColumn("EffectiveEndDate");
        sCommitteeMembership.AddOutputColumn("IsCurrent");
        sCommitteeMembership.AddOutputColumn("Term.ID");
        sCommitteeMembership.AddCriteria(Expr.Equals("Committee.ID", ContextID));
        searchesToRun.Add(sCommitteeMembership);

        var results = ExecuteSearches(searchesToRun, 0, null);

        // now, assign the search results
        dvCommitteeTerms = new DataView(results[0].Table);
        dvCommitteeMembers = new DataView(results[1].Table);
    }

    /// <summary>
    /// Executes a search against the Concierge API for all committee members then filters based on the selected Term and databinds
    /// </summary>
    private void bindCommitteeMembers()
    {
        //Filter based on Drop Down List selection
        dvCommitteeMembers.RowFilter = buildCommitteeMemberFilter();

        gvCommitteeMembership.DataSource = dvCommitteeMembers;
        gvCommitteeMembership.DataBind();
    }

    /// <summary>
    /// Translate the currently selected value from the Term drop down list to a string that can filter a data view
    /// </summary>
    /// <returns></returns>
    private string buildCommitteeMemberFilter()
    {
        if (ddlDisplayMembers.SelectedValue == "All")
            return null;

        if (ddlDisplayMembers.SelectedValue == "Current")
            return "IsCurrent=1";

        return string.Format("Term.ID='{0}'", ddlDisplayMembers.SelectedValue);
    }

    /// <summary>
    /// This will tell you if a msEntity is in the applicable membership type
    /// </summary>
    /// <returns></returns>
    private bool isInApplicableMembershipType()
    {
        // We need to now check the Membership Type to see if the 
        if (!targetCommittee.RestrictByMembershipType)
            return true;    // ok, we're good

        Search s = new Search { Type = msMembership.CLASS_NAME };
        s.AddOutputColumn("Type");
        s.AddCriteria(Expr.Equals("Owner", ConciergeAPI.CurrentEntity.ID));
        var searchReult = ExecuteSearch(s, 0, null);
        if (searchReult.TotalRowCount > 0)
        {
            var aMembershipTypes = targetCommittee["ApplicableOpenMembershipTypes"] as List<string>;
            var dr = searchReult.Table.Rows[0];
            if (aMembershipTypes == null)
                return false;
            return dr.ItemArray.Any(item => aMembershipTypes.Contains(item.ToString()));
        }
        return false;
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

       
    }

    protected void ddlDisplayMembers_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        loadDataFromConcierge();

        bindCommitteeMembers();
    }

    #endregion

    protected void lbJoin_Click(object sender, EventArgs e)
    {
        msCommitteeMembership cm = new msCommitteeMembership();
        cm.Member = ConciergeAPI.CurrentEntity.ID;
        cm.Committee = targetCommittee.ID;
        cm.Notes = "Self-joined via the portal.";

        cm = SaveObject(cm);

        using (var api = GetServiceAPIProxy())
        {
            // let's send the welcome email
            api.SendEmail(EmailTemplates.Committees.OpenCommitteeWelcome, new List<string> { cm.SafeGetValue<string>("ID") }, null);
        }

        QueueBannerMessage("You have successfully joined this committee.");
        Refresh();
    }

    protected void lbRemove_Click(object sender, EventArgs e)
    {
        Search s = new Search(msCommitteeMembership.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msCommitteeMembership.FIELDS.Committee, targetCommittee.ID));
        s.AddCriteria(Expr.Equals(msCommitteeMembership.FIELDS.Member, ConciergeAPI.CurrentEntity.ID ));

        // find all instances of myself and delete them
        using (var api = GetServiceAPIProxy())
            foreach (DataRow dr in ExecuteSearch(s, 0, null).Table.Rows)
                api.Delete(Convert.ToString(dr["ID"]));

        QueueBannerMessage("You have successfully removed yourself from this committee.");
        Refresh();
    }

    private void setupCommitteeAdminLinks()
    {
        if (ConciergeAPI.CurrentEntity == null)
            return; // can't join if not logged in

        using (var api = GetServiceAPIProxy())
            if (!CommitteeLogic.IsAdministrativeMember(api, targetCommittee.ID, ConciergeAPI.CurrentEntity.ID))
                return;
        hlEdit.Visible = true;
        hlEdit.NavigateUrl += ContextID;


    }
}