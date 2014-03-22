using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Amazon.DynamoDB;
//using Amazon.SessionProvider;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;

public partial class Login : PortalPage
{
    #region Properties

    /// <summary>
    /// Gets a value indicating whether this page is public, meaning you don't
    /// have to be logged in to access it.
    /// </summary>
    /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
    protected override bool IsPublic { get { return true; } }

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

        if (Request.QueryString["testDynamo"] == "true")
        {
            testDynamoDB();
            return;
        }

        if (Request.Form.AllKeys.Contains("Token"))
            loginWithToken();

        if (Request["uid"] != null)
        {
            tbLoginID.Text = Request["uid"];
            tbPassword.Text = Request["pwd"];
            loginToPortal();
        }

        ((App_Master_GeneralPage)Page.Master).HideHomeBreadcrumb = true;

        if (PortalConfiguration.Current.PortalLoginRedirect != null) // MS-3932 we need to be somewhere else
            GoTo(PortalConfiguration.Current.PortalLoginRedirect);

        setupForms();
    }

    private void testDynamoDB()
    {
        //DynamoDBSessionStateStore store = new DynamoDBSessionStateStore();
        //NameValueCollection nvc = new NameValueCollection();

        //nvc.Add("region", "us-east-1");

        //string table = Request.QueryString["table"] ?? "dev_portalsessions";
        //nvc.Add("table", table );

        //try
        //{
        //    store.Initialize("SomeSession", nvc);
        //}
        //catch (AmazonDynamoDBException ex)
        //{
        //    Response.Write("Dynamo DB Exception<BR/>Code:" + ex.ErrorCode + "<BR/>Type:" + ex.ErrorType);
        //}
    }

    private void setupForms()
    {
        PortalFormsManifest forms;
        using (var api = GetServiceAPIProxy())
            forms = api.GetAccessiblePortalForms( null ).ResultValue;

        forms.Forms.RemoveAll(x => !x.DisplayOnLoginScreen );

        rptForms.DataSource = forms.Forms ;
        rptForms.DataBind();
    }

    #endregion

    #region Methods

    private void loginWithToken()
    {
        string tokenString = Request.Form["Token"];
        if (string.IsNullOrWhiteSpace(tokenString))
            return;

        byte[] token = Convert.FromBase64String(tokenString);

        //Sign the data using the portal's private key.  Concierge API will use this to authenticate the originator of the request
        byte[] portalTokenSignature = Sign(token);

        ConciergeResult<LoginResult> result;

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            result = proxy.LoginWithToken(token,
                                          ConfigurationManager.AppSettings[
                                              "SigningCertificateId"],
                                          portalTokenSignature);
        }

        if (result == null || !result.Success)
            return;

        //Set the return URL to be used when the user wishes to return to the Console
        ConciergeAPI.ConsoleReturnText = Request.Form["ReturnText"];
        ConciergeAPI.ConsoleReturnUrl = Request.Form["ReturnUrl"];
        ConciergeAPI.LogoutUrl = Request.Form["LogoutUrl"];

        setSessionAndForward(result.ResultValue);
    }

    private byte[] Sign(byte[] data)
    {
        //If this portal does not have a portal specific RSA key attempt to use the default certificate (this only works for servers on the MemberSuite network)
        string keyFilePath = Server.MapPath("signingcertificate.xml");
        if (!File.Exists(keyFilePath))
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SigningCertificateSubject"]))
                Response.Redirect(
                    "/SystemUnavailable.html?AdditionalInfo=Invalid portal configuration. Either a signingcertificate.xml file must be present or a SigningCertificateSubject must be defined in the web.config");
            try
            {
                return CryptoManager.Sign(data, ConfigurationManager.AppSettings["SigningCertificateSubject"],
                                          ConfigurationManager.AppSettings["CertificatesStoreName"]);
            }
            catch (Exception ex)
            {
                Response.Redirect(
                    "/SystemUnavailable.html?AdditionalInfo=Unable to sign token with portal certificate defined in web.config");
                return null;
            }
        }

        try
        {
            return CryptoManager.Sign(data, keyFilePath);
        }
        catch
        {
            Response.Redirect(
                    "/SystemUnavailable.html?AdditionalInfo=Unable to sign token with the key in Portal.key file");
            return null;
        }
    }

    private void setLinks()
    {
        //Hide the Create a User Account link by default and only add if explicitly enabled
        divCreateAccount.Visible = PortalConfiguration.Current.PortalDisplayCreateUserAccount;

        //Add the Become a Member link by default and only hide if explicitly disabled
        divBecomeMember.Visible = PortalConfiguration.Current.PortalDisplayBecomeMember;

        //Hide the Make a Donation link by default and only add if explicitly enabled
        divMakeDonation.Visible = PortalConfiguration.Current.PortalDisplayMakeDonation;
    }


    private void setLoginText()
    {
        if (!string.IsNullOrWhiteSpace(PortalConfiguration.Current.PortalLoginScreenText))
            SkinLoginText.Text = PortalConfiguration.Current.PortalLoginScreenText;

        if (!string.IsNullOrWhiteSpace(PortalConfiguration.Current.PortalLoginScreenTitle))
            lLoginScreenTitle1.Text = PortalConfiguration.Current.PortalLoginScreenTitle;
        else
            lLoginScreenTitle1.Text = "Login to " + PortalConfiguration.Current.AssociationName;
    }

    private void setSessionAndForward(LoginResult loginResult)
    {
        ConciergeAPI.SetSession(loginResult); // set the session
        SessionManager.Set<object>("PortalLinks", null ); // force portal link reload, so non-public links now show

        checkToSeeIfMultipleIdentitiesAreAccessible();

        //If this login was initiated by a Single Sign On then check for the next URL to show the user
        if (!string.IsNullOrWhiteSpace(Request.Form["NextUrl"]))
        {
            string nextUrl = Request.Form["NextUrl"];
            if (!string.IsNullOrWhiteSpace(nextUrl) && Uri.IsWellFormedUriString(nextUrl, UriKind.RelativeOrAbsolute))
            {
                Response.Redirect(nextUrl);
            }
        }

        // is there a redirect URL?
        var redirectURL = Request.QueryString["redirectUrl"];   
        if ( ! string.IsNullOrWhiteSpace( redirectURL ))
            Response.Redirect(redirectURL);
        

        GoHome(); // go home
    }

    private void checkToSeeIfMultipleIdentitiesAreAccessible()
    {
        // now - are there alternate accessible entities?
        var entities = ConciergeAPI.AccessibleEntities;
        if (entities == null || entities.Count == 0) // well, what entity do we login as?
            return;
        
        // automatically insert the current entity
        if (!entities.Exists(x => x.ID == ConciergeAPI.CurrentEntity.ID))
        {
            entities.Insert(0, new LoginResult.AccessibleEntity
                {
                    ID = ConciergeAPI.CurrentEntity.ID,
                    Name = ConciergeAPI.CurrentEntity.Name,
                    Type = ConciergeAPI.CurrentEntity.ClassType
                });

            ConciergeAPI.AccessibleEntities = entities;
                // remember, the session is out of proc, so we need to restore it
        }

        msPortalUser currentUser = ConciergeAPI.CurrentUser;
        if (currentUser.LastLoggedInAs != null && entities.Exists(x => x.ID == currentUser.LastLoggedInAs))
        {
            // use this one
            ConciergeAPI.CurrentEntity = LoadObjectFromAPI<msEntity>(currentUser.LastLoggedInAs);
            return;
        }

        // we have multiple identiites
        Response.Redirect("/MultipleEntitiesDetected.aspx?redirectURL=" + Request.QueryString["redirectURL"]);
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

        if (!IsPostBack)
        {
            setLoginText();
            setLinks();
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        loginToPortal();
    }

    private void loginToPortal()
    {
        using (var api = GetConciegeAPIProxy())
        {
            var result = api.LoginToPortal(tbLoginID.Text, tbPassword.Text);

            if (!result.Success)
            {
                DisplayBannerMessage("Login Failure: " + result.FirstErrorMessage, true);
                return;
            }

            setSessionAndForward(result.ResultValue);
        }
    }

    #endregion
}