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

public partial class discussions_ViewMySubscriptions : PortalPage
{
    #region Fields

    protected DataTable dtSubscriptions;

    #endregion

    #region Initialization

    protected override void InitializePage()
    {
        base.InitializePage();

        loadDataFromConcierge();

        gvSubscriptions.DataSource = dtSubscriptions;
        gvSubscriptions.DataBind();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sSubscription = new Search(msDiscussionTopicSubscription.CLASS_NAME);
        sSubscription.AddOutputColumn("ID");
        sSubscription.AddOutputColumn("Topic.Name");
        sSubscription.AddOutputColumn("Topic.Forum.Name");
        sSubscription.AddOutputColumn("Topic.Forum.DiscussionBoard.Name");
        sSubscription.AddCriteria(Expr.Equals("Subscriber", ConciergeAPI.CurrentEntity.ID));
        sSubscription.AddSortColumn("Topic.Forum.DiscussionBoard.Name");
        sSubscription.AddSortColumn("Topic.Forum.Name");
        sSubscription.AddSortColumn("Topic.Name");

        SearchResult srSubscription = APIExtensions.GetSearchResult(sSubscription, PageStart, PAGE_SIZE);
        dtSubscriptions = srSubscription.Table;

        SetCurrentPageFromResults(srSubscription, hlFirstPage, hlPrevPage, hlNextPage, lNumPages, null, null,
                  null, lCurrentPage);

    }

    #endregion

    #region Event Handlers

    protected void gvSubscriptions_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "unsubscribe":
                using(IConciergeAPIService proxy = GetConciegeAPIProxy())
                {
                    proxy.Delete(e.CommandArgument.ToString());
                }
                Refresh();
                break;
        }

    }

    #endregion
}