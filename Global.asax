<%@ Application Language="C#" %>
<%@ Import Namespace="MemberSuite.SDK.Concierge" %>
<%@ Import Namespace="MemberSuite.SDK.WCF" %>
<%@ Import Namespace="MemberSuite.SDK.Web.ControlManagers" %>
<%@ Import Namespace="System.ServiceModel" %>

<script runat="server">

    //private Amazon.SessionProvider.DynamoDBSessionStateStore ds;
    void Application_Start(object sender, EventArgs e) 
    {
         
        // let's register the session ID provider, which keeps the session ID
        // for a Concierge API connection in Session State
        ConciergeAPIProxyGenerator.RegisterSessionIDProvider(new ConciergeAPI());
        ConciergeAPIProxyGenerator.RegisterAssociationIdProvider(new PortalConfiguration());
        ConciergeAPIProxyGenerator.RegisterBrowserIdProvider(new ConciergeAPI());
        
        ConciergeAPIProxyGenerator.SetAccessKeyId(ConfigurationManager.AppSettings["AccessKeyId"]);
        ConciergeAPIProxyGenerator.SetSecretAccessKey(ConfigurationManager.AppSettings["SecretAccessKey"]);
        
        /* Now, we have to register and listen for an event anytime that an API call is made
         and the session is expired. When this happens, we'll want to automatically redirect to the login page */
        ConciergeClientExtensions.SessionExpired += new EventHandler(ConciergeClientExtensions_SessionExpired);

        /* Finally, we want to listen for an event anytime an API result fails for any reason. We want to throw an
         * exception when that happens, which is caught by PortalPage.RaisePostbackEvent and shown to the user gracefully.
         * This way, we don't have to interrogate every API result for success every time we make an API call - our code can assume
         * that the API call was successful */
        ConciergeClientExtensions.OnResultError += new EventHandler<ConciergeResultErrorArgs>(ConciergeClientExtensions_OnResultError);

        HtmlTextBoxControlManager.ToolsFile = "/controls/Telerik/ToolsFileDeluxe.xml";
    }

    void ConciergeClientExtensions_SessionExpired(object sender, EventArgs e)
    {
        var httpContext = HttpContext.Current;

          
        ConciergeAPI.ClearSession();
        
        // are we dealing with a portal page?
        PortalPage pp = httpContext.Handler as PortalPage;

        if (string.Equals(httpContext.Request.Url.PathAndQuery, "/login.aspx", StringComparison.CurrentCultureIgnoreCase))
        {
            if ( pp != null ) pp.QueueBannerError("There appears to be a problem with the master login credentials.");
            return;
        }
        if (pp != null)
            pp.QueueBannerError("Your session has expired. Please login again.");

        httpContext.Response.Redirect("/Login.aspx");
    }

    void ConciergeClientExtensions_OnResultError(object sender, ConciergeResultErrorArgs e)
    {
        if (ConfigurationManager.AppSettings["ExposeRawErrors"] == "true")
            return;
        
        var cnt = HttpContext.Current;

        //if (e.Code == ConciergeErrorCode.LoginInvalid && cnt.Request.CurrentExecutionFilePath != "/profile/ResetPassword.aspx")
        //    throw new ApplicationException("Login credentials invalid. The API username/login is invalid for this portal - unable to proceed. You must select a valid API user and update the web.config with the login/password. Make sure the user is API enabled! Message: " + e.Message);

        if (cnt == null)
            // we're going to throw a client exception on errorsGr
            throw new ConciergeClientException(e.ErrorID, e.Code, e.Message);

        PortalPage pp = cnt.Handler as PortalPage;

        if (pp == null) // just throw the exception
            throw new ConciergeClientException(e.ErrorID, e.Code, e.Message);

        // now - if we're in a postback, we want to throw an exception
        // the PortalPage.RaisePostBackEvent is overridden to catch this exception
        // and display it gracefully to the user
        if (pp.IsPostBack)
            throw new ConciergeClientException(e.ErrorID, e.Code, e.Message);

        if (e.Code == ConciergeErrorCode.LoginInvalid)
            cnt.Response.Redirect("/loginFailure.html");

        //If we're on the login page and we're not in a postback (would have thrown an exception by now) and it's a login error
        //Then the API user is invalid or API Access is disabled - this causes all kinds of problems so abort to a plain aspx page with an error message.
        if (string.Equals(cnt.Request.Url.AbsolutePath, "/Login.aspx", StringComparison.CurrentCultureIgnoreCase) && e.Code == ConciergeErrorCode.IllegalParameter && e.Message.EndsWith("Unknown association ID supplied."))
            cnt.Response.Redirect("/SystemUnavailable.html?AdditionalInfo=Unknown association ID supplied."); 

        if (string.Equals(cnt.Request.Url.AbsolutePath, "/Login.aspx", StringComparison.CurrentCultureIgnoreCase) && e.Code == ConciergeErrorCode.LoginInvalid)
            cnt.Response.Redirect("/SystemUnavailable.html?AdditionalInfo=API User credentials are invalid.");

        if (string.Equals(cnt.Request.Url.AbsolutePath, "/Login.aspx", StringComparison.CurrentCultureIgnoreCase) && e.Code == ConciergeErrorCode.AccessDenied && e.Message.Contains("API Access is disabled"))
            cnt.Response.Redirect("/SystemUnavailable.html?AdditionalInfo=API Access for this association is disabled.");

        if (e.Code == ConciergeErrorCode.AccessDenied && e.Message.Contains("Unable to authenticate API user to an association"))
            cnt.Response.Redirect("/SystemUnavailable.html?AdditionalInfo=Invalid access key defined for this portal.");

        if (e.Code == ConciergeErrorCode.AccessDenied && e.Message.Contains("You do not have access to the association you have specified"))
            cnt.Response.Redirect("/SystemUnavailable.html?AdditionalInfo=Invalid API credentials for the specified association."); 
                
        // it's not a postback, so let's queue up the error message
        // and send the user to default.aspx - unless we're already at default.aspx, at which point
        // we want to send you to SystemUnavailable, which is just a plain old page
        if ( string.Equals( cnt.Request.Url.AbsolutePath, "/default.aspx", StringComparison.CurrentCultureIgnoreCase ) ||
            cnt.Request.Url.AbsolutePath == "/" )
            cnt.Response.Redirect("/SystemUnavailable.html?AdditionalInfo=" + HttpUtility.UrlEncode( e.Message ));

        // ok, now queue up the banner, and go home
        pp.QueueBannerError(e.Message);
        cnt.Response.Redirect("/default.aspx");
    }

    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        if (ConfigurationManager.AppSettings["ExposeRawErrors"] == "true")
            return;
        // Code that runs when an unhandled error occurs
        HttpContext context = HttpContext.Current;
        Exception ex = Server.GetLastError();
        
        if(ex != null && ex.InnerException != null && ex.InnerException.Message == "Unable to determine association for this portal. Use the configuration file, a query string, or a valid host name.")
        {
            Server.Transfer("/SystemUnavailable.html?AdditionalInfo=" + HttpUtility.UrlEncode("Unable to load portal configuration. This portal may be disabled."));
            Server.ClearError();
            return;
        }

        if (ex != null && ex.InnerException != null && (ex.InnerException is EndpointNotFoundException || ex.InnerException is CommunicationObjectFaultedException))
        {
            Server.Transfer("/SystemUnavailable.html");
            Server.ClearError();
            return;
        }

        if (context != null && ConfigurationManager.AppSettings["DisableErrorScreen"] != "true" )
        {
            string auditLogId = ConciergeAPI.LogException(ex, context.Request.Url);
            Server.Transfer("/ErrorPage.aspx?contextId=" + auditLogId);
            Server.ClearError();
        }
    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
