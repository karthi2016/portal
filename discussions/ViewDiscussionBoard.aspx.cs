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

public partial class discussions_ViewDiscussionBoard : DiscussionsPage
{
    #region Fields

    protected DataTable dtForums;

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        MemberSuiteObject targetObject = LoadObjectFromAPI(ContextID);

        if (targetObject != null && targetObject.ParentTypes.Contains(msDiscussionBoard.CLASS_NAME))
        {
            TargetDiscussionBoard = targetObject.ConvertTo<msDiscussionBoard>();
        }
        else
        {
            using (IConciergeAPIService proxy = GetConciegeAPIProxy())
            {
                TargetDiscussionBoard = proxy.GetDiscussionBoard(ContextID).ResultValue.ConvertTo<msDiscussionBoard>();
            }
        }

        if (TargetDiscussionBoard == null)
        {
            GoToMissingRecordPage();
            return;
        }
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            loadDataFromConcierge(proxy);
            trPostsPendingApproval.Visible = isModerator(proxy);
        }

        lblNoForumsFound.Visible = dtForums.Rows.Count == 0;
        lblBy.Visible = drLastPostPendingApproval != null;
        hlPostsPendingApproval.NavigateUrl += TargetDiscussionBoard.ID;

        if(drLastPostPendingApproval != null)
        {
            hlLastPost.Text = Convert.ToString(drLastPostPendingApproval["DiscussionPost.Name"]);
            hlLastPost.NavigateUrl += drLastPostPendingApproval["DiscussionPost"];

            lblLastPostPostedBy.Text = Convert.ToString(drLastPostPendingApproval["DiscussionPost.PostedBy.Name"]);
            lblLastPostDate.Text = FormatDate(drLastPostPendingApproval["DiscussionPost.CreatedDate"]);
        }

        rptForums.DataSource = dtForums;
        rptForums.DataBind();
    }
    
    #endregion

    #region Methods

    protected override void loadDataFromConcierge(IConciergeAPIService proxy)
    {
        base.loadDataFromConcierge(proxy);

        Search sForum = new Search(msForum.CLASS_NAME);
        sForum.AddOutputColumn("ID");
        sForum.AddOutputColumn("Name");
        sForum.AddOutputColumn("Description");
        sForum.AddOutputColumn("LastDiscussionPost");
        sForum.AddOutputColumn("LastDiscussionPost.Topic");
        sForum.AddOutputColumn("LastDiscussionPost.Name");
        sForum.AddOutputColumn("LastDiscussionPost.CreatedDate");
        sForum.AddOutputColumn("LastDiscussionPost.PostedBy.Name");
        sForum.AddOutputColumn("DiscussionTopic_Count");
        sForum.AddOutputColumn("DiscussionPost_Count");

        sForum.AddCriteria(Expr.Equals("DiscussionBoard", TargetDiscussionBoard.ID));
        sForum.AddCriteria(Expr.Equals("IsActive", true));

        if (!isActiveMember())
            sForum.AddCriteria(Expr.Equals("MembersOnly", false));

        sForum.AddSortColumn("GroupName");
        sForum.AddSortColumn("DisplayOrder");
        sForum.AddSortColumn("Name");

        SearchResult srForums = ExecuteSearch(proxy, sForum, PageStart, PAGE_SIZE);
        dtForums = srForums.Table;

        for (int i = 0; i < dtForums.Columns.Count; i++)
        {
            dtForums.Columns[i].ColumnName = dtForums.Columns[i].ColumnName.Replace(".", "_");
        }

        SetCurrentPageFromResults(srForums, hlFirstPage, hlPrevPage, hlNextPage, lNumPages, null, null,
                                  null, lCurrentPage);

        Search sPostsPendingApproval = new Search("DiscussionContents") {ID = msDiscussionPost.CLASS_NAME};
        sPostsPendingApproval.AddOutputColumn("DiscussionPost");
        sPostsPendingApproval.AddOutputColumn("DiscussionPost.Topic");
        sPostsPendingApproval.AddOutputColumn("DiscussionPost.Name");
        sPostsPendingApproval.AddOutputColumn("DiscussionPost.CreatedDate");
        sPostsPendingApproval.AddOutputColumn("DiscussionPost.PostedBy.Name");
        sPostsPendingApproval.AddCriteria(Expr.Equals("DiscussionBoard", TargetDiscussionBoard.ID));
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

    protected void rptForums_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
                break;
        }
    }

    protected void btnSearchGo_Click(object sender, EventArgs e)
    {
        string keywords = tbSearchKeywords.Text;
        keywords = HttpUtility.UrlEncode(keywords);

        string nextUrl = string.Format(@"~\discussions\SearchDiscussionContents_Results.aspx?contextID={0}&keywords={1}", TargetDiscussionBoard.ID, keywords);
        GoTo(nextUrl);
    }

    #endregion
}