using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class profile_ChangePassword : PortalPage
{
    #region Properties

    protected override bool CheckMustChangePassword
    {
        get
        {
            return false;
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
        if (!IsPostBack)
            return;

        using (var api = GetConciegeAPIProxy())
        {
            api.ChangePassword(tbCurrentPassword.Text,
                              tbNewPassword.Text);
        }

        ConciergeAPI.CurrentUser.MustChangePassword = false;

        GoHome("Your password has been changed successfully.");
    }

    protected void cvPasswordValidator_Validate(object source, ServerValidateEventArgs args)
    {

    }

    #endregion
}