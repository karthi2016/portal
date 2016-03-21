using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_ViewAbstracts : PortalPage
{
    #region Fields

    protected msEvent targetEvent;

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
        targetEvent = LoadObjectFromAPI<msEvent>(ContextID);

        if (targetEvent == null) GoToMissingRecordPage();
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

        Search s = new Search(msAbstract.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        s.AddCriteria(Expr.Equals("Owner", ConciergeAPI.CurrentEntity.ID));
        s.AddSortColumn("Name");
        s.AddOutputColumn("LocalID");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("PresenterName");
        s.AddOutputColumn("Status.Name");
        s.AddOutputColumn("CreatedDate");

        gvAbstracts.DataSource = APIExtensions.GetSearchResult(s, 0, null).Table;
        gvAbstracts.DataBind();

        
        //Set the visibility on the edit column
        gvAbstracts.Columns[6].Visible = CanEdit();

        CustomTitle.Text = string.Format("My {0} Abstracts", targetEvent.Name);
    }

    #endregion

    #region Methods

    protected bool CanEdit()
    {
        if (!targetEvent.EnableAbstracts) return false;

        if (targetEvent.AcceptAbstractsFrom != null && targetEvent.AcceptAbstractsFrom.Value > DateTime.Now)
            return false;

        if (targetEvent.AcceptAbstractsUntil != null && targetEvent.AcceptAbstractsUntil.Value < DateTime.Now)
            return false;

        return targetEvent.AllowEditAbstracts;
    }

    #endregion

    #region Event Handlers

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("ViewEvent.aspx?contextID=" + targetEvent.ID);
    }

    #endregion
}