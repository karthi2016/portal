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

public partial class chapters_CreateEditChapterLeader : PortalPage
{
    #region Fields

    protected msChapter targetChapter;
    protected msMembershipLeader targetLeader;
    protected msIndividual targetIndividual;
    protected DataView dvAvailableMembers;

    protected msMembershipLeader currentLeader;

    #endregion

    #region Properties

    protected string IndividualID
    {
        get
        {
            return Request.QueryString["individualID"];
        }
    }

    protected bool IsInEditMode { get; set; }

    #endregion

    #region Intitalization

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

        //Load the chapter and individual from the context id in the query string
        targetChapter = LoadObjectFromAPI<msChapter>(ContextID);

        if (targetChapter == null)
        {
            GoToMissingRecordPage();
            return;
        }

        if (targetChapter.Leaders == null)
            targetChapter.Leaders = new List<msMembershipLeader>();

        if (string.IsNullOrWhiteSpace(IndividualID))
            targetLeader = new msMembershipLeader();
        else
        {
            //Check the individual ID - if it's specified then this is an edit command and load the individual
            targetIndividual = LoadObjectFromAPI<msIndividual>(IndividualID);
            targetLeader = targetChapter.Leaders.Where(x => x.Individual == IndividualID).SingleOrDefault();

            if (targetIndividual == null)
            {
                GoToMissingRecordPage();
                return;
            }

            IsInEditMode = true;
        }

        if (targetLeader == null)
        {
            GoToMissingRecordPage();
            return;
        }

            currentLeader = targetChapter.Leaders.Find(x => x.Individual == CurrentEntity.ID);
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



        loadDataFromConcierge();


            bindObjectToPage();


            lblMember.Visible = IsInEditMode;
            ddlMember.Visible = !lblMember.Visible;

            if (!IsInEditMode)
            {
                ddlMember.DataSource = dvAvailableMembers;
                ddlMember.DataBind();
            }

        setVisiblePermissions();

        if (!IsInEditMode)
            CustomTitle.Text = string.Format("Create {0} Chapter Leader", targetChapter.Name);
        else
            CustomTitle.Text = string.Format("Edit {0} Chapter Leader", targetChapter.Name);

 
    }

    protected override bool CheckSecurity()
    {
        if(!base.CheckSecurity()) return false;

        if (IsInEditMode && targetIndividual.ID == ConciergeAPI.CurrentEntity.ID)
            return false;

        return currentLeader != null && currentLeader.CanManageLeaders;
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sAvailableLeaders = new Search(msChapterMembership.CLASS_NAME);
        sAvailableLeaders.AddOutputColumn("Membership.Owner.ID");
        sAvailableLeaders.AddOutputColumn("Membership.Owner.Name");
        sAvailableLeaders.AddCriteria(Expr.Equals("Chapter", ContextID));
        sAvailableLeaders.AddCriteria(Expr.Equals("IsCurrent", true));

        SearchOperationGroup terminationGroup = new SearchOperationGroup { FieldName = "Membership.TerminationDate" };
        terminationGroup.Criteria.Add(Expr.Equals("Membership.TerminationDate", null));
        terminationGroup.Criteria.Add(Expr.IsGreaterThan("Membership.TerminationDate", DateTime.Now));
        terminationGroup.GroupType = SearchOperationGroupType.Or;
        sAvailableLeaders.AddCriteria(terminationGroup);

        //Exclude all individuals that are already leaders of the chapter
        foreach (var leader in targetChapter.Leaders)
            sAvailableLeaders.AddCriteria(Expr.DoesNotEqual("Membership.Owner.ID", leader.Individual));

        sAvailableLeaders.AddSortColumn("Membership.Owner.Name");

        SearchResult srAvailableLeaders = APIExtensions.GetSearchResult(sAvailableLeaders, 0, null);

        //Use toTable to make sure we have distinct values
        DataView dvTempAvailableLeaders = new DataView(srAvailableLeaders.Table);
        DataTable dtAvailableLeaders = dvTempAvailableLeaders.ToTable(true,
                                                                      new[]
                                                                          {
                                                                              "Membership.Owner.ID",
                                                                              "Membership.Owner.Name"
                                                                          });

        //Remove existing leaders from the available members list
        dtAvailableLeaders.PrimaryKey = new[]{dtAvailableLeaders.Columns["Membership.Owner.ID"]};
        foreach (var membershipLeader in targetChapter.Leaders)
        {
            DataRow drMember = dtAvailableLeaders.Rows.Find(membershipLeader.Individual);
            if (drMember != null)
                dtAvailableLeaders.Rows.Remove(drMember);
        }

        dvAvailableMembers = new DataView(srAvailableLeaders.Table);
    }

    protected void setVisiblePermissions()
    {
        trCanCreateMembers.Visible = currentLeader.CanCreateMembers;
        trCanDownloadRoster.Visible = currentLeader.CanDownloadRoster;
        trCanMakePayments.Visible = currentLeader.CanMakePayments;
        trCanManageCommittees.Visible = currentLeader.CanManageCommittees;
        trCanManageEvents.Visible = currentLeader.CanManageEvents;
        trCanManageLeaders.Visible = currentLeader.CanManageLeaders;
        trCanUpdateContactInfo.Visible = currentLeader.CanUpdateContactInfo;
        trCanUpdateInformation.Visible = currentLeader.CanUpdateInformation;
        trCanUpdateMembershipInfo.Visible = currentLeader.CanUpdateMembershipInfo;
        trCanViewAccountHistory.Visible = currentLeader.CanViewAccountHistory;
        trCanViewMembers.Visible = currentLeader.CanViewMembers;
    }

    #endregion

    #region Data Binding

    protected void bindObjectToPage()
    {
        ddlMember.SelectedValue = targetLeader.Individual;

        if(IsInEditMode)
            lblMember.Text = targetIndividual.Name;

        chkCanCreateMembers.Checked = targetLeader.CanCreateMembers;
        chkCanDownloadRoster.Checked = targetLeader.CanDownloadRoster;
        chkCanMakePayments.Checked = targetLeader.CanMakePayments;
        chkCanManageCommittees.Checked = targetLeader.CanManageCommittees;
        chkCanManageEvents.Checked = targetLeader.CanManageEvents;
        chkCanManageLeaders.Checked = targetLeader.CanManageLeaders;
        chkCanUpdateContactInfo.Checked = targetLeader.CanUpdateContactInfo;
        chkCanUpdateInformation.Checked = targetLeader.CanUpdateInformation;
        chkCanUpdateMembershipInfo.Checked = targetLeader.CanUpdateMembershipInfo;
        chkCanViewAccountHistory.Checked = targetLeader.CanViewAccountHistory;
        chkCanViewMembers.Checked = targetLeader.CanViewMembers;
    }

    protected void unbindObjectFromPage()
    {
        if (!IsInEditMode)
            targetLeader.Individual = ddlMember.SelectedValue;

        if(currentLeader.CanCreateMembers)
            targetLeader.CanCreateMembers = chkCanCreateMembers.Checked;
        
        if(currentLeader.CanDownloadRoster)
            targetLeader.CanDownloadRoster = chkCanDownloadRoster.Checked;
        
        if(currentLeader.CanMakePayments)
            targetLeader.CanMakePayments = chkCanMakePayments.Checked;
        
        if(currentLeader.CanManageCommittees)
            targetLeader.CanManageCommittees = chkCanManageCommittees.Checked;
        
        if(currentLeader.CanManageEvents)
            targetLeader.CanManageEvents = chkCanManageEvents.Checked;
        
        if(currentLeader.CanManageLeaders)
            targetLeader.CanManageLeaders = chkCanManageLeaders.Checked;
        
        if(currentLeader.CanUpdateContactInfo)
            targetLeader.CanUpdateContactInfo = chkCanUpdateContactInfo.Checked;
        
        if(currentLeader.CanUpdateInformation)
            targetLeader.CanUpdateInformation = chkCanUpdateInformation.Checked;
        
        if(currentLeader.CanUpdateMembershipInfo)
            targetLeader.CanUpdateMembershipInfo = chkCanUpdateMembershipInfo.Checked;
        
        if(currentLeader.CanViewAccountHistory)
            targetLeader.CanViewAccountHistory = chkCanViewAccountHistory.Checked;
        
        if(currentLeader.CanViewMembers)
            targetLeader.CanViewMembers = chkCanViewMembers.Checked;

        targetChapter.Leaders.RemoveAll(x => x.Individual == targetLeader.Individual);
        targetChapter.Leaders.Add(targetLeader);
    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        unbindObjectFromPage();

        using (IConciergeAPIService serviceProxy = GetServiceAPIProxy())
        {
            targetChapter = serviceProxy.Save(targetChapter).ResultValue.ConvertTo<msChapter>();
        }

        GoTo(string.Format("~/chapters/ManageChapterLeaders.aspx?contextID={0}", ContextID));
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("~/chapters/ManageChapterLeaders.aspx?contextID={0}", ContextID));
    }

    #endregion
}