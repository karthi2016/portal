using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class profile_ChangeLoginId : PortalPage
{
    #region Fields

    protected msPortalUser user;

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

        user = LoadObjectFromAPI<msPortalUser>(ConciergeAPI.CurrentUser.ID);

        if(user == null)
        {
            GoToMissingRecordPage();
            return;
        }
    }

    #endregion

    #region Event Handlers

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsPostBack || !IsValid)
            return;

        user.Name = tbNewLoginId.Text;

        using (var api = GetConciegeAPIProxy())
        {
            var sr = api.Save(user);
            ConciergeAPI.CurrentUser = sr.ResultValue.ConvertTo<msPortalUser>();
        }

        GoHome("Your login ID has been changed successfully.");
    }
    
    #endregion
}