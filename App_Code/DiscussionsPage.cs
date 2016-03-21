using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for DiscussionsPage
/// </summary>
public class DiscussionsPage : PortalPage
{
    #region Fields
    private msDiscussionBoard targetDiscussionBoard;

    protected msForum targetForum;
    protected msDiscussionTopic targetDiscussionTopic;
    protected msDiscussionPost targetDiscussionPost;
    
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msCommittee targetCommittee;
    protected msOrganizationalLayer targetOrganizationalLayer;
    protected msEvent targetEvent;

    protected bool editMode;

    protected msMembershipLeader leader;
    protected DataRow drLastPostPendingApproval;
    protected int numberOfPostsPendingApproval;
    protected bool hasLeaderSearchBeenRun;
    protected DataRow drSubscription;

    #endregion

    #region Properties

    protected msDiscussionBoard TargetDiscussionBoard
    {
        get { return targetDiscussionBoard; }
        set
        {
            if(value != null)
            {
                switch(value.ClassType)
                {
                    case msChapterDiscussionBoard.CLASS_NAME:
                        msChapterDiscussionBoard cdb = value.ConvertTo<msChapterDiscussionBoard>();
                        targetChapter = LoadObjectFromAPI<msChapter>(cdb.Chapter);
                        break;
                    case msSectionDiscussionBoard.CLASS_NAME:
                        msSectionDiscussionBoard sdb = value.ConvertTo<msSectionDiscussionBoard>();
                        targetSection = LoadObjectFromAPI<msSection>(sdb.Section);
                        break;
                    case msCommitteeDiscussionBoard.CLASS_NAME:
                        msCommitteeDiscussionBoard committeedb = value.ConvertTo<msCommitteeDiscussionBoard>();
                        targetCommittee = LoadObjectFromAPI<msCommittee>(committeedb.Committee);
                        break;
                    case msOrganizationalLayerDiscussionBoard.CLASS_NAME:
                        msOrganizationalLayerDiscussionBoard oldb = value.ConvertTo<msOrganizationalLayerDiscussionBoard>();
                        targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(oldb.OrganizationalLayer);
                        break;
                    case msEventDiscussionBoard.CLASS_NAME:
                        msEventDiscussionBoard edb = value.ConvertTo<msEventDiscussionBoard>();
                        targetEvent = LoadObjectFromAPI<msEvent>(edb.Event);
                        break;
                }

            }

            targetDiscussionBoard = value;
        }
    }

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
    }

    protected override bool CheckSecurity()
    {
        if(!base.CheckSecurity())
            return false;

        if(targetForum != null && (!targetForum.IsActive || (targetForum.MembersOnly && !MembershipLogic.IsActiveMember())))
            return false;

        return true;
    }

    #endregion

    #region Methods

    protected virtual void loadDataFromConcierge(IConciergeAPIService proxy)
    {
        
    }

    protected string FormatDate(object value)
    {
        if (value == null || !(value is DateTime))
            return null;

        DateTime dt = (DateTime)value;

        if (dt.Date == DateTime.Today)
            return "Today " + dt.ToShortTimeString();

        if (dt.Date == DateTime.Today.AddDays(-1))
            return "Yesterday " + dt.ToShortTimeString();

        return dt.ToShortDateString() + " " + dt.ToShortTimeString();
    }

    protected bool isModerator()
    {
        using(IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            return isModerator(proxy);
        }
    }

    protected bool isModerator(IConciergeAPIService proxy)
    {
        if (hasLeaderSearchBeenRun)
            return leader != null && leader.CanModerateDiscussions;

        if(targetChapter != null)
        {
            Search sChapterLeader = GetChapterLeaderSearch(targetChapter.ID);
            SearchResult srChapterLeader = proxy.GetSearchResult(sChapterLeader, 0, 1);
            leader = ConvertLeaderSearchResult(srChapterLeader);
            hasLeaderSearchBeenRun = true;
        }

        if (targetSection != null)
        {
            Search sSectionLeader = GetSectionLeaderSearch(targetSection.ID);
            SearchResult srSectionLeader = proxy.GetSearchResult(sSectionLeader, 0, 1);
            leader = ConvertLeaderSearchResult(srSectionLeader);
            hasLeaderSearchBeenRun = true;
        }

        if (targetOrganizationalLayer != null)
        {
            Search sOrganizationalLayerLeader = GetOrganizationalLayerLeaderSearch(targetOrganizationalLayer.ID);
            SearchResult srOrganizationalLayerLeader = proxy.GetSearchResult(sOrganizationalLayerLeader, 0, 1);
            leader = ConvertLeaderSearchResult(srOrganizationalLayerLeader);
            hasLeaderSearchBeenRun = true;
        }

        return leader != null && leader.CanModerateDiscussions;
    }

    #endregion

    #region Event Handlers

    #endregion

    protected void loadSubscription()
    {
        using(IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            loadSubscription(proxy);
        }
    }

    protected void loadSubscription(IConciergeAPIService proxy)
    {
        Search sSubscription = new Search(msDiscussionTopicSubscription.CLASS_NAME);
        sSubscription.AddOutputColumn("ID");
        sSubscription.AddCriteria(Expr.Equals("Topic", targetDiscussionTopic.ID));
        sSubscription.AddCriteria(Expr.Equals("Subscriber", ConciergeAPI.CurrentEntity.ID));
        sSubscription.AddSortColumn("Subscriber");

        SearchResult srSubscription = proxy.GetSearchResult(sSubscription, 0, 1);
        if (srSubscription.TotalRowCount > 0 && srSubscription.Table != null && srSubscription.Table.Rows.Count > 0)
            drSubscription = srSubscription.Table.Rows[0];
    }
}