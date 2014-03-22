using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class discussions_ViewPostsPendingApproval : DiscussionsPage
{
    #region Fields

    protected DataTable dtPostsPendingApproval;

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        MemberSuiteObject targetObject = LoadObjectFromAPI(ContextID);

        if (targetObject == null)
        {
            GoToMissingRecordPage();
            return;
        }

        switch (targetObject.ClassType)
        {
            case msDiscussionPost.CLASS_NAME:
                targetDiscussionPost = targetObject.ConvertTo<msDiscussionPost>();
                targetDiscussionTopic = LoadObjectFromAPI<msDiscussionTopic>(targetDiscussionPost.Topic);
                targetForum = LoadObjectFromAPI<msForum>(targetDiscussionTopic.Forum);
                TargetDiscussionBoard = LoadObjectFromAPI<msDiscussionBoard>(targetForum.DiscussionBoard);
                break;
            case msDiscussionTopic.CLASS_NAME:
                targetDiscussionTopic = targetObject.ConvertTo<msDiscussionTopic>();
                targetForum = LoadObjectFromAPI<msForum>(targetDiscussionTopic.Forum);
                TargetDiscussionBoard = LoadObjectFromAPI<msDiscussionBoard>(targetForum.DiscussionBoard);
                break;
            case msForum.CLASS_NAME:
                targetForum = targetObject.ConvertTo<msForum>();
                TargetDiscussionBoard = LoadObjectFromAPI<msDiscussionBoard>(targetForum.DiscussionBoard);
                break;
            case msDiscussionBoard.CLASS_NAME:
                TargetDiscussionBoard = targetObject.ConvertTo<msDiscussionBoard>();
                break;
        }

        if (targetObject.ParentTypes.Contains(msDiscussionBoard.CLASS_NAME))
            TargetDiscussionBoard = targetObject.ConvertTo<msDiscussionBoard>();
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            loadDataFromConcierge(proxy);
        }

        lblNoPostsFound.Visible = dtPostsPendingApproval.Rows.Count == 0;

        rptPosts.DataSource = dtPostsPendingApproval;
        rptPosts.DataBind();
    }


    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return isModerator();
    }

    #endregion

    #region Methods

    protected override void loadDataFromConcierge(IConciergeAPIService proxy)
    {
        base.loadDataFromConcierge(proxy);

        Search sDiscussionPosts = new Search(msDiscussionPost.CLASS_NAME);
        sDiscussionPosts.AddOutputColumn("ID");
        sDiscussionPosts.AddOutputColumn("Name");
        sDiscussionPosts.AddOutputColumn("CreatedDate");
        sDiscussionPosts.AddOutputColumn("Post");
        sDiscussionPosts.AddOutputColumn("PostedBy");
        sDiscussionPosts.AddOutputColumn("PostedBy.Name");
        sDiscussionPosts.AddOutputColumn("PostedBy.Discussions_DiscussionPosts");
        sDiscussionPosts.AddOutputColumn("PostedBy.Image");

        if (targetDiscussionPost != null)
            sDiscussionPosts.AddCriteria(Expr.Equals("ID", targetDiscussionPost.ID));

        if(targetDiscussionTopic != null)
            sDiscussionPosts.AddCriteria(Expr.Equals("Topic", targetDiscussionTopic.ID));

        if (targetForum != null)
            sDiscussionPosts.AddCriteria(Expr.Equals("Topic.Forum", targetForum.ID));

        if (TargetDiscussionBoard != null)
            sDiscussionPosts.AddCriteria(Expr.Equals("Topic.Forum.DiscussionBoard", TargetDiscussionBoard.ID));

        sDiscussionPosts.AddCriteria(Expr.Equals("Status", DiscussionPostStatus.Pending.ToString()));

        sDiscussionPosts.AddSortColumn("CreatedDate", true);
        sDiscussionPosts.AddSortColumn("Name");

        SearchResult srDiscussionPosts = ExecuteSearch(proxy, sDiscussionPosts, PageStart, PAGE_SIZE);
        dtPostsPendingApproval = srDiscussionPosts.Table;

        for (int i = 0; i < dtPostsPendingApproval.Columns.Count; i++)
        {
            dtPostsPendingApproval.Columns[i].ColumnName = dtPostsPendingApproval.Columns[i].ColumnName.Replace(".", "_");
        }

        SetCurrentPageFromResults(srDiscussionPosts, hlFirstPage, hlPrevPage, hlNextPage, lNumPages, null, null,
                          null, lCurrentPage);
    }

    #endregion

    #region Event Handlers

    protected void rptPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Item.DataItem;

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;

            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
                goto case ListItemType.Item;

            case ListItemType.Item:
                Image img = (Image)e.Item.FindControl("profileImg");
                HtmlTableRow trProfileImage = (HtmlTableRow)e.Item.FindControl("trProfileImage");
                Label lblPosts = (Label)e.Item.FindControl("lblPosts");

                lblPosts.Visible = drv["PostedBy"] != DBNull.Value;

                if (drv["PostedBy_Image"] == DBNull.Value || !PortalPage.setProfileImage(img, drv["PostedBy_Image"].ToString()))
                    trProfileImage.Visible = false;

                HtmlTableRow trMessageTitle = (HtmlTableRow)e.Item.FindControl("trMessageTitle");
                trMessageTitle.Visible = !string.IsNullOrWhiteSpace(drv["Name"] as string);
                break;
        }
    }

    protected void rptPosts_OnItemCommand(object source, RepeaterCommandEventArgs e)
    {
        var discussionPostId = e.CommandArgument.ToString();

        msDiscussionPost discussionPost = LoadObjectFromAPI<msDiscussionPost>(discussionPostId);

        switch (e.CommandName)
        {
            case "approve":
                discussionPost.Status = DiscussionPostStatus.Approved;

                discussionPost = SaveObject(discussionPost);
                using (IConciergeAPIService proxy = GetConciegeAPIProxy())
                {
                    if (!string.IsNullOrWhiteSpace(discussionPost.PostedBy))
                        proxy.SendEmail("BuiltIn:DiscussionPostApproval", new List<string> {discussionPost.ID}, null);
                    proxy.SendEmailsToSubscribedEntities(discussionPost.ID);
                }
                QueueBannerMessage("The selected post has been approved");
                Refresh();
                break;
            case "reject":
                discussionPost.Status = DiscussionPostStatus.Rejected;
                discussionPost = SaveObject(discussionPost);
                if (!string.IsNullOrWhiteSpace(discussionPost.PostedBy))
                {
                    using (IConciergeAPIService proxy = GetConciegeAPIProxy())
                    {
                        proxy.SendEmail("BuiltIn:DiscussionPostRejection", new List<string> {discussionPost.ID}, null);
                    }
                }
                QueueBannerMessage("The selected post has been rejected");
                Refresh();
                break;
        }
    }

    #endregion
}