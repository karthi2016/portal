using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class chapters_MakePaymentForChapterMember : PortalPage
{
    #region Fields

    protected msChapter targetChapter;
    protected DataView dvMembers;

    #endregion

    #region Intialization

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
        return leader != null && leader.CanMakePayments;
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sMembers = new Search{Type = msChapterMembership.CLASS_NAME};
        sMembers.AddOutputColumn("Membership.Owner.ID");
        sMembers.AddOutputColumn("Membership.Owner.LocalID");
        sMembers.AddOutputColumn("Membership.Owner.Name");
        sMembers.AddOutputColumn("Membership.Owner.Invoices_OpenInvoiceCount");
        sMembers.AddCriteria(Expr.Equals("Chapter.ID", targetChapter.ID));
        sMembers.AddCriteria(Expr.IsGreaterThan("Membership.Owner.Invoices_OpenInvoiceCount",0));
        sMembers.AddSortColumn("Membership.Owner.Name");

        SearchResult srMembers = ExecuteSearch(sMembers, 0, null);
        dvMembers = new DataView(srMembers.Table);
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

        loadDataFromConcierge();

        gvMembers.DataSource = dvMembers;
        gvMembers.DataBind();
    }

    #endregion
}