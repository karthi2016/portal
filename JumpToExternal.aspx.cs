using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Utilities;

public partial class JumpToExternal : PortalPage
{
    /// <summary>
    /// Gets a value indicating whether this page is public, meaning you don't
    /// have to be logged in to access it.
    /// </summary>
    /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
    protected override bool IsPublic { get { return true; } }

    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        byte[] portalLoginToken = null;

        string targetURL = Request["TargetUrl"];

        if (string.IsNullOrEmpty(targetURL))
        {
            // If no target URL provided, just go back to the home page
            GoHome();
        }

        if (CurrentUser == null)
        {
            // No Logged-in User, just redirect to target URL
            Response.Redirect(targetURL);
        }

        // Generate an SSO Token based on logged-in User
        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            // Request a portal security token specifying the portal username, ID of the signing certificate, and digital signature
            ConciergeResult<byte[]> portalLoginTokenResult = proxy.CreatePortalSecurityToken(
                CurrentUser.Name,
                ConfigurationManager.AppSettings["SigningCertificateId"],
                Sign(Encoding.ASCII.GetBytes(CurrentUser.Name)));

            //If the Concierge API indicated a fault, write a friendly error message
            if (!portalLoginTokenResult.Success)
            {
                GoToErrorPage(string.IsNullOrWhiteSpace(portalLoginTokenResult.FirstErrorMessage) ? "API operation failed" : portalLoginTokenResult.FirstErrorMessage);
                return;
            }

            //Set the portal security token in the web user's session so it can be used by the RedirectToPortal page
            portalLoginToken = portalLoginTokenResult.ResultValue;
        }

        // If there's no portal security token in the web user's session send them back to the main page
        if (Request.UrlReferrer == null || portalLoginToken == null)
        {
            GoToErrorPage("Unable to jump to target page.");
            return;
        }

        // Define the form variables to POST to the portal Login.aspx

        // This will let the new site know exactly where you are coming from
        litReturnUrl.Text = Request.UrlReferrer.ToString();

        // This token allows the Single Sign On
        litToken.Text = Convert.ToBase64String(portalLoginToken);

        // This OPTIONAL value tells the page where to redirect the user after login.
        litNextUrl.Text = Request["NextUrl"]; 

        // This is the page that will perform the login
        litAction.Text = targetURL;

        // This page should never be cached
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    private byte[] Sign(byte[] data)
    {
        //If this portal does not have a portal specific RSA key attempt to use the default certificate (this only works for servers on the MemberSuite network)
        string keyFilePath = Server.MapPath("signingcertificate.xml");
        if (!File.Exists(keyFilePath))
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SigningCertificateSubject"]))
                GoToErrorPage("Invalid portal configuration. Either a signingcertificate.xml file must be present or a SigningCertificateSubject must be defined in the web.config");
            try
            {
                return CryptoManager.Sign(data, ConfigurationManager.AppSettings["SigningCertificateSubject"],
                                          ConfigurationManager.AppSettings["CertificatesStoreName"]);
            }
            catch
            {
                GoToErrorPage("Unable to sign token with portal certificate defined in web.config");
                return null;
            }
        }

        try
        {
            return CryptoManager.Sign(data, keyFilePath);
        }
        catch
        {
            GoToErrorPage("Unable to sign token with the key in Portal.key file");
            return null;
        }
    }

    private void GoToErrorPage(string message)
    {
        Response.Redirect(string.Format("/SystemUnavailable.html?AdditionalInfo={0}", HttpUtility.UrlPathEncode(message)));
    }
}