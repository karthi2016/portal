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

public partial class profile_CreateAccount_BasicInfo : PortalPage
{
    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected bool InitiatedByLeader
    {
        get
        {
            if (Request.QueryString["Leader"] == null)
                return false;

            bool result;
            if (!bool.TryParse(Request.QueryString["Leader"], out result))
                return false;

            return result;
        }
    }

    protected string Type
    {
        get
        {
            return Request.QueryString["t"];
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();

        if (PortalConfiguration.Current.PortalLoginRedirect != null) // MS-3932 we need to be somewhere else
        {
            // Instead of redirecting, just do not allow login, but keep the Create Account form
            tdExistingAccount.Visible = false;
        }

        MultiStepWizards.CreateAccount.InitiatedByLeader = InitiatedByLeader;

        string pageTitle = Request.QueryString["pageTitle"];
        if (pageTitle != null)
            lblTitle.Text = pageTitle;

        if (!string.IsNullOrWhiteSpace(Type))
            switch (Type.ToLower())
            {
                case "membership":
                    MultiStepWizards.CreateAccount.CompleteUrl = "~/membership/Join.aspx";
                    break;
                case "donation":
                    MultiStepWizards.CreateAccount.CompleteUrl = "~/donation/MakeDonation.aspx";
                    break;
                case "storefront":
                    MultiStepWizards.CreateAccount.CompleteUrl = "~/orders/EnterShippingInformation.aspx";
                    break;
                case "jobposting":
                    MultiStepWizards.CreateAccount.CompleteUrl = "~/careercenter/CreateEditJobPosting.aspx";
                    break;

                case "exhibitshow":
                    MultiStepWizards.CreateAccount.CompleteUrl = "/exhibits/RegisterForShow.aspx?contextID=" + Request.QueryString["showID"];
                    break;

                case "subscription":
                    MultiStepWizards.CreateAccount.CompleteUrl = "/subscriptions/Subscribe.aspx";
                    break;

                case "portalform":
                    MultiStepWizards.CreateAccount.CompleteUrl = "/forms/CreateFormInstance.aspx?contextID=" + Request.QueryString["formID"];
                    break;
            }

        string completeUrl = Request.QueryString["completionUrl"];
        if (completeUrl != null)
            MultiStepWizards.CreateAccount.CompleteUrl = completeUrl;

        // If there is a logged in user and it isn't a leader creating a new member then bypass the whole user creation process and go directly to the completion URL
        if (!MultiStepWizards.CreateAccount.InitiatedByLeader && ConciergeAPI.CurrentEntity != null && !ConciergeAPI.HasBackgroundConsoleUser)
            GoTo(MultiStepWizards.CreateAccount.CompleteUrl ?? "~/");

        // If we have a forwarding URL, be sure to pass this along to the Login page and ultimately any SSO pages
        if (!string.IsNullOrEmpty(MultiStepWizards.CreateAccount.CompleteUrl))
        {
            var masterPage = Master as App_Master_GeneralPage;
            if (masterPage != null)
            {
                masterPage.UpdateLoginLinkTab(string.Format("/Login.aspx?redirectURL={0}", Server.UrlEncode(MultiStepWizards.CreateAccount.CompleteUrl)));
            }
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

        if (InitiatedByLeader)
        {
            // Deactivate login if part of the Leader wizard, but do not reactivate if previously removed.
            tdExistingAccount.Visible = false;
        }
    }

    protected void btnNewContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            checkEmailExistsGetOrCreatePortalUser(proxy);
        }

        NewUserRequest request = new NewUserRequest
                                     {
                                         EmailAddress = tbEmail.Text,
                                         FirstName = tbFirstName.Text,
                                         LastName = tbLastName.Text,
                                         PostalCode = tbPostalCode.Text,
                                         Name = Request.QueryString["pageTitle"]
                                     };

        MultiStepWizards.CreateAccount.Request = request;
        GoTo("~/profile/CreateAccount_DuplicateCheck.aspx");

    }

    protected void btnExistingContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        using (var api = GetConciegeAPIProxy())
        {
            // Clear the previous session before starting a new one (like cached Online Store data), but keep the exceptions (like Shopping Cart)
            SessionManager.Clear(false);

            var result = api.LoginToPortal(
                tbLoginID.Text,
                tbPassword.Text);

            if (!result.Success)
            {
                DisplayBannerMessage(true, "Login Failure: " + result.FirstErrorMessage);
                return;
            }

            setSessionAndForward(result.ResultValue);
        }
    }

    #endregion

    #region Methods

    private void checkEmailExistsGetOrCreatePortalUser(IConciergeAPIService serviceProxy)
    {
        //The API GetOrCreatePortalUser will attempt to match the supplied credentials to a portal user, individual, or organization. 
        //If a portal user is found it will be returned.  If not and an individual / organization uses the email address it will create and return a new Portal User
        ConciergeResult<msPortalUser> result = serviceProxy.SearchAndGetOrCreatePortalUser(tbEmail.Text);

        //The portal user in the result will be null if the e-mail didn't match anywhere
        if (result.ResultValue == null)
            return;

        var portalUser = result.ResultValue.ConvertTo<msPortalUser>();

        if (MultiStepWizards.CreateAccount.InitiatedByLeader)
        {
            MultiStepWizards.CreateAccount.InitiatedByLeader = false;
            GoTo(string.Format("~/membership/Join.aspx?entityID={0}", portalUser.Owner));
        }
        else
            GoTo(
                string.Format(
                    "~/profile/EmailLoginInformation.aspx?contextID={0}",
                    portalUser.ID));

    }

    private void setSessionAndForward(LoginResult loginResult)
    {
        ConciergeAPI.SetSession(loginResult); // set the session

        if (!string.IsNullOrEmpty(MultiStepWizards.CreateAccount.CompleteUrl))
            GoTo(MultiStepWizards.CreateAccount.CompleteUrl);

        GoHome();   // go home
    }

    #endregion
}