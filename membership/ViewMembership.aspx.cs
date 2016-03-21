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

public partial class membership_ViewMembership : PortalPage
{
    #region Fields

    protected msMembership targetMembership;
    protected msEntity owner;
    protected DataRow drMembership;
    protected DataView dvChapterMembership;
    protected DataView dvSectionMembership;
    protected DataView dvAuditLogs;
    protected DataView dvAddOns;

    protected Dictionary<string, msMembershipLeader> leaders;

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
        targetMembership = LoadObjectFromAPI<msMembership>(ContextID);

        if (targetMembership == null)
        {
            GoToMissingRecordPage();
            return;
        }

        owner = LoadObjectFromAPI<msEntity>(targetMembership.Owner);
        if(owner == null)
        {
            GoToMissingRecordPage();
            return;
        }

        _runSearch();

        string info = GetMembershipField("SavedPaymentMethod.Name");
        if (info != null)
            lblPaymentInfo.Text = info;

        // only show termination if terminated
        trTermination.Visible = drMembership["TerminationDate"] != DBNull.Value;

        loadLeaders();
    }

    protected override void setupHyperLinks()
    {
        base.setupHyperLinks();

        hlContactInfo.NavigateUrl = owner.ClassType == msOrganization.CLASS_NAME
                                        ? string.Format("~/profile/EditOrganizationInfo.aspx?contextID={0}", owner.ID)
                                        : string.Format("~/profile/EditIndividualInfo.aspx?contextID={0}", owner.ID);
        liContactInfo.Visible = canUpdateContactInfo();

        hlUpdateMembershipInfo.NavigateUrl = string.Format("~/membership/EditMembership.aspx?contextID={0}", ContextID);
        liUpdateMembershipInfo.Visible = canUpdateMembershipInfo();
        
        hlRenewMembership.NavigateUrl = CurrentEntity.ID != targetMembership.Owner
                                            ? string.Format(
                                                "~/membership/RegisterForMembership.aspx?contextID={0}&entityID={1}",
                                                targetMembership.MembershipOrganization, targetMembership.Owner)
                                            : string.Format("~/membership/RegisterForMembership.aspx?contextID={0}",
                                                            targetMembership.MembershipOrganization);
        liRenewMembership.Visible = canRenew();

        hlViewAccountHistory.NavigateUrl = CurrentEntity.ID != targetMembership.Owner
                                               ? string.Format(
                                                   "~/financial/AccountHistory.aspx?contextID={0}&leaderOfId={1}",
                                                   targetMembership.Owner, getAccountHistoryLeaderOfId())
                                               : string.Format("~/financial/AccountHistory.aspx?contextID={0}",
                                                               targetMembership.Owner);
        liViewAccountHistory.Visible = canViewAccountHistory();

        hlUpdateBillingInfo2.NavigateUrl = hlUpdateBillingInfo.NavigateUrl = string.Format("/orders/UpdateBillingInformation.aspx?contextID={0}", ContextID);
    }

    private void _runSearch()
    {
        var sMembership = new Search { Type = msMembership.CLASS_NAME, ID=msMembership.CLASS_NAME };
        sMembership.AddCriteria(Expr.Equals("ID", ContextID));

        // output columns
        sMembership.AddOutputColumn("LocalID");
        sMembership.AddOutputColumn("Owner.LocalID");
        sMembership.AddOutputColumn("Owner.Name");
        sMembership.AddOutputColumn("JoinDate");
        sMembership.AddOutputColumn("ExpirationDate");
        sMembership.AddOutputColumn("RenewalDate");
        sMembership.AddOutputColumn("Approved");
        sMembership.AddOutputColumn("Status.Name");
        sMembership.AddOutputColumn("Type.Name");
        sMembership.AddOutputColumn("Product.Name");
        sMembership.AddOutputColumn("TerminationDate");
        sMembership.AddOutputColumn("SavedPaymentMethod.Name");
        
        sMembership.AddOutputColumn("TerminationReason.Name");
        sMembership.AddOutputColumn("ReceivesMemberBenefits");
        sMembership.AddOutputColumn("MembershipDirectoryOptOut");

        var sChapters = new Search { Type = msChapterMembership.CLASS_NAME, ID=msChapterMembership.CLASS_NAME };
        sChapters.AddCriteria(Expr.Equals("Membership", ContextID));
        sChapters.AddOutputColumn("Chapter");
        sChapters.AddOutputColumn("Chapter.Name");
        sChapters.AddOutputColumn("JoinDate");
        sChapters.AddOutputColumn("ExpirationDate");
        sChapters.AddSortColumn("ExpirationDate", true);
        sChapters.AddSortColumn("JoinDate", true);

        var sSections = new Search { Type = msSectionMembership.CLASS_NAME, ID=msSectionMembership.CLASS_NAME };
        sSections.AddCriteria(Expr.Equals("Membership", ContextID));
        sSections.AddOutputColumn("Section.Name");
        sSections.AddOutputColumn("JoinDate");
        sSections.AddOutputColumn("ExpirationDate");
        sSections.AddSortColumn("ExpirationDate", true);
        sSections.AddSortColumn("JoinDate", true);

        var sAuditLogs = new Search { Type = msAuditLog.CLASS_NAME, ID=msAuditLog.CLASS_NAME };
        sAuditLogs.AddCriteria(Expr.Equals(msAuditLog.FIELDS.AffectedRecord_ID, ContextID));
        sAuditLogs.AddCriteria(Expr.IsOneOfTheFollowing(msAuditLog.FIELDS.Type, new List<string> {"Renewal", "Drop"} ));
        sAuditLogs.AddOutputColumn("Type_Name");
        sAuditLogs.AddOutputColumn("Actor.Name");
        sAuditLogs.AddOutputColumn("CreatedDate");
        sAuditLogs.AddSortColumn("CreatedDate", true);

        var sAddOns = new Search("MembershipAddOn");
        sAddOns.ID = "AddOns";
        sAddOns.AddCriteria(Expr.Equals("Membership", ContextID));
        sAddOns.AddOutputColumn("Merchandise.Name");
        sAddOns.AddOutputColumn("Quantity");
        sAddOns.AddOutputColumn("Price");
        sAddOns.AddOutputColumn("Renewable");
        sAddOns.AddSortColumn("ListIndex");

        var searchesToRun = new List<Search> { sMembership, sChapters, sSections, sAuditLogs, sAddOns  };

        var searchResults = APIExtensions.GetMultipleSearchResults(searchesToRun, 0, null);

        drMembership = searchResults[0].Table.Rows[0];

        dvChapterMembership = new DataView(searchResults.Single(x => x.ID == msChapterMembership.CLASS_NAME).Table);
        dvSectionMembership = new DataView(searchResults.Single(x => x.ID == msSectionMembership.CLASS_NAME).Table);
        dvAuditLogs = new DataView(searchResults.Single(x => x.ID == msAuditLog.CLASS_NAME).Table);
        dvAddOns = new DataView(searchResults.Single(x => x.ID == "AddOns").Table);

        // bind chapters
        if (dvChapterMembership.Count > 0)
        {
            tdChapters.Visible = true;
            gvChapters.DataSource = dvChapterMembership;
            gvChapters.DataBind();
        }

        // bind sections
        if (dvSectionMembership.Count > 0)
        {
            tdSections.Visible = true;
            gvSections.DataSource = dvSectionMembership;
            gvSections.DataBind();
        }

        // bind history
        if (dvAuditLogs.Count > 0)
        {
            divHistory.Visible = true;
            gvHistory.DataSource = dvAuditLogs;
            gvHistory.DataBind();
        }

        if (dvAddOns.Count > 0)
        {
            divAddOns.Visible = true;
            gvAddOns.DataSource = dvAddOns;
            gvAddOns.DataBind();
        }

        CustomFieldSet1.DataBind();

    }

    protected override bool CheckSecurity()
    {
        return base.CheckSecurity() && CanViewMembers();
    }

    #endregion

    #region Custom Fields

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetMembership;

        var pageLayout = targetMembership.GetAppropriatePageLayout();
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = targetMembership.DescribeObject();
        CustomFieldSet1.PageLayout = pageLayout.Metadata;

        CustomFieldSet1.AddReferenceNamesToTargetObject(proxy);

        CustomFieldSet1.Render();
        divCustomFields.Visible = true;
    }

    #endregion

    #region Utils

    protected string GetMembershipField(string fieldName)
    {
        return GetMembershipField(fieldName, null);
    }

    protected string GetMembershipField(string fieldName, string formatString)
    {
        if (drMembership == null) return null;
        if (!drMembership.Table.Columns.Contains(fieldName)) return "(column not in search)";

        object obj = drMembership[fieldName];
        if (obj == DBNull.Value) return null;

        if (formatString == null) return Convert.ToString(obj);

        return string.Format("{0:" + formatString + "}", obj);
    }

    protected void loadLeaders()
    {
        //If there's no chapters on this membership then the owner can't be a chapter leader
        if(dvChapterMembership.Count == 0)
        {
            leaders = new Dictionary<string, msMembershipLeader>();
            return;
        }

        Search sLeaders = GetChapterLeaderSearch(null);
        sLeaders.AddOutputColumn("Chapter");
        sLeaders.Criteria.Clear();
        sLeaders.AddCriteria(Expr.Equals("Individual", ConciergeAPI.CurrentEntity.ID));

        SearchOperationGroup chapterGroup = new SearchOperationGroup();
        chapterGroup.FieldName = "Chapter";
        chapterGroup.GroupType = SearchOperationGroupType.Or;

        foreach (DataRowView drvChapterMembership in dvChapterMembership)
            chapterGroup.Criteria.Add(Expr.Equals("Chapter", drvChapterMembership["Chapter"].ToString()));

        sLeaders.AddCriteria(chapterGroup);

        SearchResult srLeaders = APIExtensions.GetSearchResult(sLeaders, 0, null);

        leaders = new Dictionary<string, msMembershipLeader>();
        foreach (DataRow drLeader in srLeaders.Table.Rows)
        {
            //Store and clear the chapter to avoid a casting exception
            string chapterId = drLeader["Chapter"].ToString();
            drLeader["Chapter"] = DBNull.Value;

            msMembershipLeader leader = MemberSuiteObject.FromDataRow(drLeader).ConvertTo<msMembershipLeader>();
            leaders[chapterId] = leader;
        }
    }

    protected bool CanViewMembers()
    {
        if (targetMembership.Owner == CurrentEntity.ID)
            return true;

        if (leaders.Values.ToList().Exists(x => x.CanViewMembers))
            return true;
            
        Search s = new Search(msEntity.CLASS_NAME);
        s.AddOutputColumn("Membership");
        s.AddCriteria(Expr.Equals("ID", CurrentEntity.ID));

        SearchResult sr = APIExtensions.GetSearchResult(s, 0, 1);
        return sr.TotalRowCount > 0 && sr.Table.Rows[0]["Membership"] != DBNull.Value && string.Equals(sr.Table.Rows[0]["Membership"].ToString(), targetMembership.ID, StringComparison.CurrentCultureIgnoreCase);
    }

    protected bool canUpdateMembershipInfo()
    {
        if (targetMembership.Owner == CurrentEntity.ID)
            return true;

        bool result = leaders.Values.ToList().Exists(x => x.CanUpdateMembershipInfo);
        return result;
    }

    protected bool canUpdateContactInfo()
    {
        if (targetMembership.Owner == CurrentEntity.ID)
            return true;

        bool result = leaders.Values.ToList().Exists(x => x.CanUpdateContactInfo);
        return result;
    }

    protected bool canViewAccountHistory()
    {
        if (targetMembership.Owner == CurrentEntity.ID)
            return true;

        bool result = leaders.Values.ToList().Exists(x => x.CanViewAccountHistory);
        return result;
    }

    protected string getAccountHistoryLeaderOfId()
    {
        return (from kvp in leaders where kvp.Value.CanViewAccountHistory select kvp.Key).FirstOrDefault();
    }

    protected bool canRenew()
    {
        // MS-5847 - prevent inherited membership renewal in the portal
        if (targetMembership.IsInherited)
            return false;   

        if (targetMembership.Owner == CurrentEntity.ID)
            return true;

        bool result = leaders.Values.ToList().Exists(x => x.CanCreateMembers);
        return result;
    }

    #endregion

    #region Event Handlers

    #endregion
}