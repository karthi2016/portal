using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.WCF;

public partial class profile_ResetPassword : PortalPage
{
    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected string UserId
    {
        get
        {
            return Request.QueryString["u"];
        }
    }

    protected string PasswordHash
    {
        get
        {
            return Request.QueryString["t"];
        }
    }

    protected string NextUrl
    {
        get
        {
            return Request.QueryString["n"];
        }
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

        if (string.IsNullOrWhiteSpace(PasswordHash))
            GoTo("~/Login.aspx");

    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        using (var serviceProxy = GetServiceAPIProxy())
        {
            LoginResult login;

            try
            {
                login = serviceProxy.LoginPortalUserWithHash(
                    UserId, PasswordHash).ResultValue;
            }
            catch(ConciergeClientException ex)
            {
                QueueBannerError("Unable to reset password.");
                GoTo("~/Login.aspx");
                return;
            }
            ConciergeAPI.SetSession(login);

            serviceProxy.ResetPassword(ConciergeAPI.CurrentUser.ID, tbPassword.Text);
            ConciergeAPI.CurrentUser.MustChangePassword = false;
        }

        GoTo(!string.IsNullOrWhiteSpace(NextUrl) ? NextUrl : "/");
    }

    #endregion
}