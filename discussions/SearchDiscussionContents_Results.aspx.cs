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

public partial class discussions_SearchDiscussionContents_Results : DiscussionsPage
{
    #region Fields

    protected DataTable dtSearchResults;

    #endregion

    #region Properties

    protected string Keywords
    {
        get { return Request.QueryString["keywords"]; }
    }

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        var targetObject = APIExtensions.LoadObjectFromAPI(ContextID);

        if (targetObject == null)
        {
            GoToMissingRecordPage();
            return;
        }

        switch (targetObject.ClassType)
        {
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

        if(targetObject.ParentTypes.Contains(msDiscussionBoard.CLASS_NAME))
            TargetDiscussionBoard = targetObject.ConvertTo<msDiscussionBoard>();
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            loadDataFromConcierge(proxy);
        }

        lblNoRecordsFound.Visible = dtSearchResults.Rows.Count == 0;

        rptSearchResults.DataSource = dtSearchResults;
        rptSearchResults.DataBind();

        CustomTitle.Text = getTitle();
    }

    #endregion

    #region Methods

    protected override void loadDataFromConcierge(IConciergeAPIService proxy)
    {
        base.loadDataFromConcierge(proxy);

        Search s = new Search("DiscussionContents");
        s.AddOutputColumn("ID");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("ContentType");
        s.AddOutputColumn("DiscussionPost_Count");
        s.AddOutputColumn("LastDiscussionPost");
        s.AddOutputColumn("LastDiscussionPost.Topic");
        s.AddOutputColumn("LastDiscussionPost.Name");
        s.AddOutputColumn("LastDiscussionPost.CreatedDate");
        s.AddOutputColumn("LastDiscussionPost.PostedBy.Name");
        s.AddOutputColumn("DiscussionBoard");
        s.AddOutputColumn("Forum");
        s.AddOutputColumn("DiscussionTopic");

        SearchOperationGroup sogActive = new SearchOperationGroup()
                                             {
                                                 FieldName = "Forum.IsActive",
                                                 GroupType = SearchOperationGroupType.Or
                                             };
        sogActive.Criteria.Add(Expr.IsBlank("Forum"));
        sogActive.Criteria.Add(Expr.Equals("Forum.IsActive", 1));

        s.AddCriteria(sogActive);

        if (!MembershipLogic.IsActiveMember())
        {
            SearchOperationGroup sogMembersOnly = new SearchOperationGroup()
            {
                FieldName = "Forum.MembersOnly",
                GroupType = SearchOperationGroupType.Or
            };
            sogMembersOnly.Criteria.Add(Expr.IsBlank("Forum"));
            sogMembersOnly.Criteria.Add(Expr.Equals("Forum.MembersOnly", false));
        }

        if(TargetDiscussionBoard != null)
            s.AddCriteria(Expr.Equals("DiscussionBoard", TargetDiscussionBoard.ID));

        if (targetForum != null)
            s.AddCriteria(Expr.Equals("Forum", targetForum.ID));

        if (targetDiscussionTopic != null)
            s.AddCriteria(Expr.Equals("DiscussionTopic", targetDiscussionTopic.ID));

        if(!string.IsNullOrWhiteSpace(Keywords))
            s.AddCriteria(Expr.Contains("Keywords", Keywords));

        SearchResult sr = proxy.GetSearchResult(s, PageStart, PAGE_SIZE);
        dtSearchResults = sr.Table;

        for (int i = 0; i < dtSearchResults.Columns.Count; i++)
        {
            dtSearchResults.Columns[i].ColumnName = dtSearchResults.Columns[i].ColumnName.Replace(".", "_");
        }

        SetCurrentPageFromResults(sr, hlFirstPage, hlPrevPage, hlNextPage, lNumPages, null, null,
                          null, lCurrentPage);
    }

    protected string getTitle()
    {
        string result = "Discussion Search Results";

        if(targetDiscussionTopic != null)
            return string.Format("{0} within the {1} Topic", result, targetDiscussionTopic);

        if (targetForum != null)
            return string.Format("{0} within the {1} Forum", result, targetForum);

        if (TargetDiscussionBoard != null)
            return string.Format("{0} within the {1} Discussion Board", result, targetForum);

        return result;
    }

    #endregion

    #region Event Handlers

    protected void rptSearchResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

                HyperLink hlTitle = (HyperLink) e.Item.FindControl("hlTitle");

                switch (drv["ContentType"].ToString())
                {
                    case "Forum":
                        hlTitle.NavigateUrl = string.Format(@"~\discussions\ViewForum.aspx?contextID={0}", drv["ID"]);
                        break;
                    case "Topic":
                        hlTitle.NavigateUrl = string.Format(@"~\discussions\ViewDiscussionTopic.aspx?contextID={0}", drv["ID"]);
                        break;
                    case "Post":
                        hlTitle.NavigateUrl = string.Format(@"~\discussions\ViewDiscussionTopic.aspx?contextID={0}&postID={1}", drv["DiscussionTopic"], drv["ID"]);
                        break;
                }
                break;
        }
    }

    #endregion
}