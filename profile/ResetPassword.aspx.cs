using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.WCF;
using Spring.Globalization.Formatters;

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
            // MS-6037 (Modified 1/7/2015) Apparently using Request.QueryString["t"] removes the special character '+' and replaces it with a space.            
            var passwordHash = Request.QueryString["t"];
            if (!string.IsNullOrWhiteSpace(passwordHash))
                passwordHash = passwordHash.Replace(" ", "+");
            return passwordHash;
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
            catch(ConciergeClientException)
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