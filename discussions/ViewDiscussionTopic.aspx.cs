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

public partial class discussions_ViewDiscussionTopic : DiscussionsPage
{
    #region Fields

    protected DataTable dtDiscussionPosts;

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetDiscussionTopic = LoadObjectFromAPI<msDiscussionTopic>(ContextID);

        if (targetDiscussionTopic == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetForum = LoadObjectFromAPI<msForum>(targetDiscussionTopic.Forum);

        if (targetForum == null)
        {
            GoToMissingRecordPage();
            return;
        }

        TargetDiscussionBoard = LoadObjectFromAPI<msDiscussionBoard>(targetForum.DiscussionBoard);

        if (TargetDiscussionBoard == null)
        {
            GoToMissingRecordPage();
            return;
        }

        loadSubscription();
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        using(IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            loadDataFromConcierge(proxy);
            hlPostsPendingApproval.Visible = isModerator(proxy);
        }

        lblNoPostsFound.Visible = dtDiscussionPosts.Rows.Count == 0;
        
        hlPostsPendingApproval.NavigateUrl += targetDiscussionTopic.ID;

        if (drSubscription != null)
            lbSubscribeUnsubscribe.Text = "Unsubscribe from this Topic";

        rptTopics.DataSource = dtDiscussionPosts;
        rptTopics.DataBind();
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

        sDiscussionPosts.AddCriteria(Expr.Equals("Topic", targetDiscussionTopic.ID));
        sDiscussionPosts.AddCriteria(Expr.Equals("Status", DiscussionPostStatus.Approved.ToString()));

        sDiscussionPosts.AddSortColumn("CreatedDate", true);
        sDiscussionPosts.AddSortColumn("Name");

        SearchResult srDiscussionPosts = ExecuteSearch(proxy, sDiscussionPosts, PageStart, PAGE_SIZE);
        dtDiscussionPosts = srDiscussionPosts.Table;

        for (int i = 0; i < dtDiscussionPosts.Columns.Count; i++)
        {
            dtDiscussionPosts.Columns[i].ColumnName = dtDiscussionPosts.Columns[i].ColumnName.Replace(".", "_");
        }

        SetCurrentPageFromResults(srDiscussionPosts, hlFirstPage, hlPrevPage, hlNextPage, lNumPages, null, null,
                          null, lCurrentPage);
    }

    #endregion

    #region Event Handlers

    protected void rptTopics_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

                HyperLink hlRemovePost = (HyperLink)e.Item.FindControl("hlRemovePost");
                HyperLink hlEditPost = (HyperLink)e.Item.FindControl("hlEditPost");
                hlRemovePost.Visible = hlEditPost.Visible = drv["PostedBy"] != DBNull.Value &&
                                     drv["PostedBy"].ToString() == ConciergeAPI.CurrentEntity.ID;
                break;
        }
    }

    protected void btnSearchGo_Click(object sender, EventArgs e)
    {
        string keywords = tbSearchKeywords.Text;
        keywords = HttpUtility.UrlEncode(keywords);

        string nextUrl = string.Format(@"~\discussions\SearchDiscussionContents_Results.aspx?contextID={0}&keywords={1}", targetDiscussionTopic.ID, keywords);
        GoTo(nextUrl);
    }

    protected void lbSubscribeUnsubscribe_Click(object sender, EventArgs e)
    {
        msDiscussionTopicSubscription subscription = new msDiscussionTopicSubscription
                                                         {
                                                             Topic = targetDiscussionTopic.ID,
                                                             Subscriber = ConciergeAPI.CurrentEntity.ID
                                                         };

        using(IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            if (drSubscription == null)
            {
                proxy.Save(subscription);
            }
            else
            {
                proxy.Delete(drSubscription["ID"].ToString());
            }
        }

        Refresh();
    }

    #endregion
}