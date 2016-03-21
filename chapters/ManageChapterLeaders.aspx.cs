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

public partial class chapters_ManageChapterLeaders : PortalPage
{
    #region Fields

    protected msChapter targetChapter;
    protected DataView dvLeaders;
    
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

        targetChapter = LoadObjectFromAPI<msChapter>(ContextID);

        if(targetChapter == null)
        {
            GoToMissingRecordPage();
            return;
        }
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        if (targetChapter.Leaders == null)
            // no leaders to speak of
            return false;

        var leader = targetChapter.Leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanManageLeaders;
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sLeaders = new Search("ChapterLeader");
        sLeaders.AddOutputColumn("Individual.ID");
        sLeaders.AddOutputColumn("Individual.Name");
        sLeaders.AddCriteria(Expr.Equals("Chapter.ID", targetChapter.ID));
        sLeaders.AddSortColumn("Individual.Name");

        SearchResult srLeaders = APIExtensions.GetSearchResult(sLeaders, 0, null);
        dvLeaders = new DataView(srLeaders.Table);
    }

    private void loadAndBindLeaders()
    {
        loadDataFromConcierge();

        //Bind the leaders gridview
        gvLeaders.DataSource = dvLeaders;
        gvLeaders.DataBind();
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

        if(!IsPostBack)
            loadAndBindLeaders();
    }

    protected void gvLeaders_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch(e.CommandName.ToLower())
        {
            case "deleteleader":
                targetChapter.Leaders.RemoveAll(
                    x => x.Individual == e.CommandArgument.ToString());
                using(IConciergeAPIService serviceProxy = GetServiceAPIProxy())
                {
                    targetChapter = serviceProxy.Save(targetChapter).ResultValue.ConvertTo<msChapter>();
                }
                loadAndBindLeaders();
                break;
            case "editleader":
                string nextUrl = string.Format("~/chapters/CreateEditChapterLeader.aspx?contextID={0}&individualID={1}", ContextID,
                                               e.CommandArgument);
                GoTo(nextUrl);
                break;
        }
        
    }

    protected void gvLeaders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataRowView drvLeader = (DataRowView)e.Row.DataItem;

        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                //Hide the edit/delete commands when the leader listed is the logged in entity (you cannot edit/delete yourself)
                if (ConciergeAPI.CurrentEntity.ID == drvLeader["Individual.ID"].ToString())
                {
                    LinkButton btnEdit = (LinkButton) e.Row.FindControl("btnEdit");
                    btnEdit.Visible = false;

                    LinkButton btnDelete = (LinkButton) e.Row.FindControl("btnDelete");
                    btnDelete.Visible = false;
                }
                break;
        }
    }

    #endregion
}