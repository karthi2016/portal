using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;

public partial class membership_Join : PortalPage
{
    #region Fields

    protected List<LoginResult.AccessibleEntity> relatedOrganizations;
    protected msEntity targetEntity;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get { return true; }
    }

    protected string EntityId
    {
        get { return Request.QueryString["entityID"]; }
    }

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
        //If there's no logged in user send the user through the CreateAccount process with the variable
        //to kick them back to this page on completion
        if(ConciergeAPI.CurrentEntity == null)
        {
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=Membership");
            return;
        }
        
        base.InitializeTargetObject();

        if (string.IsNullOrWhiteSpace(EntityId) || EntityId.ToLower() == ConciergeAPI.CurrentEntity.ID.ToLower())
        {
            targetEntity = ConciergeAPI.CurrentEntity;
            if (ConciergeAPI.AccessibleEntities != null)
                relatedOrganizations = ConciergeAPI.AccessibleEntities.Where(x => x.Type == msOrganization.CLASS_NAME && !x.ID.Equals(ConciergeAPI.CurrentEntity.ID, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
        else
        {
            targetEntity = LoadObjectFromAPI<msEntity>(EntityId);
            relatedOrganizations = getRelatedOrganizations(EntityId);

        }

        if (relatedOrganizations == null || relatedOrganizations.Count == 0)
        {
            StringBuilder nextUrl = new StringBuilder("~/membership/PurchaseMembership1.aspx?");
            if (!string.IsNullOrWhiteSpace(ContextID))
                nextUrl.AppendFormat("contextID={0}&", ContextID);

            if (!string.IsNullOrWhiteSpace(EntityId))
                nextUrl.AppendFormat("entityID={0}", EntityId);

            GoTo(nextUrl.ToString());
        }
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

        //Add the item for the current entity
        rblEntity.Items.Add(new ListItem(string.Format("I am joining/renewing for myself, <b>{0}</b>", ConciergeAPI.CurrentEntity.Name), ConciergeAPI.CurrentEntity.ID));
        rblEntity.Items[0].Selected = true;

        rblEntity.DataSource = relatedOrganizations;
        rblEntity.DataBind();
    }

    #endregion

    #region Methods

   

    #endregion

    #region Event Handlers

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        StringBuilder nextUrl = new StringBuilder("~/membership/PurchaseMembership1.aspx?");
        if (!string.IsNullOrWhiteSpace(ContextID))
            nextUrl.AppendFormat("contextID={0}&", ContextID);


        nextUrl.AppendFormat("entityID={0}", rblEntity.SelectedValue);

        GoTo(nextUrl.ToString());
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    #endregion
}