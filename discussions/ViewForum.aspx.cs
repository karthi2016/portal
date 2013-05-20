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

public partial class discussions_ViewForum : DiscussionsPage
{
   #region Fields

    protected DataTable dtDiscussionTopics;

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetForum = LoadObjectFromAPI<msForum>(ContextID);

        if(targetForum == null)
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
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        using(IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            loadDataFromConcierge(proxy);
            trPostsPendingApproval.Visible = isModerator(proxy);
        }

        lblNoTopicsFound.Visible = dtDiscussionTopics.Rows.Count == 0;
        lblBy.Visible = drLastPostPendingApproval != null;
        hlPostsPendingApproval.NavigateUrl += targetForum.ID;

        if (drLastPostPendingApproval != null)
        {
            lblLastPostPostedBy.Text = Convert.ToString(drLastPostPendingApproval["DiscussionPost.PostedBy.Name"]);
            lblLastPostDate.Text = FormatDate(drLastPostPendingApproval["DiscussionPost.CreatedDate"]);
        }

        rptTopics.DataSource = dtDiscussionTopics;
        rptTopics.DataBind();
    }

    #endregion

    #region Methods

    protected override void loadDataFromConcierge(IConciergeAPIService proxy)
    {
        base.loadDataFromConcierge(proxy);

        Search sDiscussionTopic = new Search(msDiscussionTopic.CLASS_NAME);
        sDiscussionTopic.AddOutputColumn("ID");
        sDiscussionTopic.AddOutputColumn("Name");
        sDiscussionTopic.AddOutputColumn("PostedBy");
        sDiscussionTopic.AddOutputColumn("PostedBy.Name");
        sDiscussionTopic.AddOutputColumn("LastDiscussionPost");
        sDiscussionTopic.AddOutputColumn("LastDiscussionPost.Name");
        sDiscussionTopic.AddOutputColumn("LastDiscussionPost.CreatedDate");
        sDiscussionTopic.AddOutputColumn("LastDiscussionPost.PostedBy.Name");
        sDiscussionTopic.AddOutputColumn("DiscussionPost_Count");

        sDiscussionTopic.AddCriteria(Expr.Equals("Forum", targetForum.ID));

        sDiscussionTopic.AddSortColumn("LastDiscussionPost.CreatedDate", true);
        sDiscussionTopic.AddSortColumn("Name");

        SearchResult srDiscussionTopic = ExecuteSearch(proxy, sDiscussionTopic, PageStart, PAGE_SIZE);
        dtDiscussionTopics = srDiscussionTopic.Table;

        for (int i = 0; i < dtDiscussionTopics.Columns.Count; i++)
        {
            dtDiscussionTopics.Columns[i].ColumnName = dtDiscussionTopics.Columns[i].ColumnName.Replace(".", "_");
        }

        SetCurrentPageFromResults(srDiscussionTopic, hlFirstPage, hlPrevPage, hlNextPage, lNumPages, null, null,
          null, lCurrentPage);

        Search sPostsPendingApproval = new Search("DiscussionContents") { ID = msDiscussionPost.CLASS_NAME };
        sPostsPendingApproval.AddOutputColumn("DiscussionPost");
        sPostsPendingApproval.AddOutputColumn("DiscussionPost.Topic");
        sPostsPendingApproval.AddOutputColumn("DiscussionPost.Name");
        sPostsPendingApproval.AddOutputColumn("DiscussionPost.CreatedDate");
        sPostsPendingApproval.AddOutputColumn("DiscussionPost.PostedBy.Name");
        sPostsPendingApproval.AddCriteria(Expr.Equals("Forum", targetForum.ID));
        sPostsPendingApproval.AddCriteria(Expr.Equals("DiscussionPost.Status",
                                                      DiscussionPostStatus.Pending.ToString()));
        sPostsPendingApproval.AddSortColumn("DiscussionPost.CreatedDate", true);

        SearchResult srPostsPendingApproval = ExecuteSearch(proxy, sPostsPendingApproval, 0, 1);
        if (srPostsPendingApproval.Table.Rows.Count > 0)
            drLastPostPendingApproval = srPostsPendingApproval.Table.Rows[0];
        numberOfPostsPendingApproval = srPostsPendingApproval.TotalRowCount;
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
                if (drv["LastDiscussionPost"] == DBNull.Value)
                    e.Item.FindControl("lblBy").Visible = false;

                HyperLink hlEditDiscussionTopic = (HyperLink)e.Item.FindControl("hlEditDiscussionTopic");
                hlEditDiscussionTopic.Visible = drv["PostedBy"] != DBNull.Value && drv["PostedBy"].ToString() == ConciergeAPI.CurrentEntity.ID;
                break;
        }
    }

    protected void btnSearchGo_Click(object sender, EventArgs e)
    {
        string keywords = tbSearchKeywords.Text;
        keywords = HttpUtility.UrlEncode(keywords);

        string nextUrl = string.Format(@"~\discussions\SearchDiscussionContents_Results.aspx?contextID={0}&keywords={1}", targetForum.ID, keywords);
        GoTo(nextUrl);
    }

    #endregion
}