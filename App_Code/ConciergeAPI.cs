using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Console;
using MemberSuite.SDK.Manifests.Resource;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;

/// <summary>
/// Summary description for ConciergeAPISessionIDProvider
/// </summary>
public class ConciergeAPI : IConciergeAPISessionIdProvider, IConciergeAPIBrowserIdProvider
{
    private const string BrowserCacheKey = "ConciergeAPIBrowserID";
    private const string COOKIE_APM_USER = "APMUser"; //For APM (DynaTrace) user identification - not to be used for any security function

    private static void setUserIdentificationCookie()
    {
        //For APM (DynaTrace) user identification - not to be used for any security function
        HttpCookie apmUserCookie = new HttpCookie(COOKIE_APM_USER);

        //Can't just check BackgroundUser because that will always have a value
        //It will be a user like "MemberSuite Agent" if there is no interactive console user
        //Use any console user doing impersonation first because that's the user that is experiencing any issues
        if (HasBackgroundConsoleUser && BackgroundUser != null)
        {
            apmUserCookie.Value = BackgroundUser.Name;
        }
        else
        {
            //No console user so use the portal user
            if (CurrentUser != null)
            {
                apmUserCookie.Value = CurrentUser.Name;
            }
            else
            {
                //No portal user either - delete the cookie if it exists
                apmUserCookie.Expires = DateTime.Now.AddDays(-1);
            }
        }

        HttpContext.Current.Response.SetCookie(apmUserCookie);
    }

    public static msPortalUser CurrentUser
    {
        get { return SessionManager.Get<msPortalUser>("ConciergeAPICurrentUser");}
        set { SessionManager.Set("ConciergeAPICurrentUser", value); }
    }

    public static msUser BackgroundUser
    {
        get { return SessionManager.Get<msUser>("ConciergeAPIBackgroundUser"); }
        set { SessionManager.Set("ConciergeAPIBackgroundUser", value); }
    }

    public static msEntity CurrentEntity
    {
        get { return SessionManager.Get<msEntity>("ConciergeAPICurrentEntity"); }
        set { SessionManager.Set("ConciergeAPICurrentEntity",value); }
    }

    public static List<LoginResult.AccessibleEntity> AccessibleEntities
    {
        get { 
            
            string rawValue = SessionManager.Get<string>("ConciergeAPIAccessibleEntities");            
            if (rawValue == null) return null;

            return Xml.Deserialize<List<LoginResult.AccessibleEntity>>(rawValue);
        
        }
        set { SessionManager.Set("ConciergeAPIAccessibleEntities",value != null ? Xml.Serialize( value ) : null ); }
    }

    public static msAssociation CurrentAssociation
    {
        get { return SessionManager.Get<msAssociation>("ConciergeAPICurrentAssociation"); }
        set { SessionManager.Set("ConciergeAPICurrentAssociation",value); }
    }

    private static List<string> CurrentTabs
    {
        get { return SessionManager.Get<List<string>>("ConciergeAPITabs"); }
        set { SessionManager.Set("ConciergeAPITabs",value); }
    }

    

    public static string ConsoleReturnText
    {
        get { return SessionManager.Get<string>("ConsoleReturnText"); }
        set { SessionManager.Set("ConsoleReturnText",value); }
    }

    public static string ConsoleReturnUrl
    {
        get { return SessionManager.Get<string>("ConsoleReturnUrl"); }
        set { SessionManager.Set("ConsoleReturnUrl",value); }
    }


      

       private static TimeZoneInfo _currenTimeZone
       {
           get { return SessionManager.Get<TimeZoneInfo>("_currenTimeZone"); }
           set { SessionManager.Set("_currenTimeZone",value); }
       }

    public static TimeZoneInfo CurrentTimeZone
    {
        get
        {
            if (_currenTimeZone != null)
                return _currenTimeZone;

            if (CurrentUser == null)
                return TimeZoneInfo.Local;
            ;

            var timeZone = CurrentUser.SafeGetValue<string>("TimeZone");
            if (timeZone == null)
                return TimeZoneInfo.Local;
            ;

            try
            {
                _currenTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
            catch
            {
                return TimeZoneInfo.Local;
                ;
            }

            return _currenTimeZone;

        }
    }

    public static string LogoutUrl
    {
        get { return SessionManager.Get<string>("LogoutUrl"); }
        set { SessionManager.Set("LogoutUrl",value); }
    }

    public static string CurrentLocale
    {
        get { return SessionManager.Get<string>("Locale") ?? "en-US"; }
        set { SessionManager.Set("Locale", value); }
    }

    public static string ConsoleUrl
    {
        get
        {
            return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ConsoleUrl"])
                       ? "https://console.production.membersuite.com"
                       : ConfigurationManager.AppSettings["ConsoleUrl"];
        }
    }
           

    public static bool HasBackgroundConsoleUser
    {
        get
        {
            //Can't just check BackgroundUser because that will always have a value
            //It will be a user like "MemberSuite Agent" if there is no interactive console user
            //So instead check if there's a return URL from the console - that's always set when a
            //console user is impersonating a portal user
            if (string.IsNullOrWhiteSpace(ConsoleReturnUrl))
                return false;

            if (ConsoleReturnUrl.ToLower().StartsWith("http://localhost") ||
                ConsoleReturnUrl.ToLower().StartsWith("https://localhost"))
                return true;

            return ConsoleReturnUrl.ToLower().StartsWith(ConsoleUrl.ToLower());
        }
    }

    /// <summary>
    /// Sets the session.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <returns></returns>
    public static void SetSession( LoginResult result )
    {
        //Now that the Message Header is created in the ConciergeAPIProxyGenerator.SessionID setter and cached, 
        //we MUST use that property which will call SetSessionId on the registered IConciergeAPISessionIdProvider below
        ConciergeAPIProxyGenerator.SessionID = result.SessionID;
        CurrentUser  = result.PortalUser.ConvertTo<msPortalUser>();
        CurrentEntity = result.PortalEntity.ConvertTo<msEntity>();
        CurrentAssociation = result.Association.ConvertTo<msAssociation>();

        BackgroundUser = result.User.ConvertTo<msUser>();

        if (result.ConsoleMetadata != null && result.ConsoleMetadata.Tabs != null)
        {
            List<string> tabs = new List<string>();
            foreach (var t in result.ConsoleMetadata.Tabs)
                if (t.IsActive)
                    tabs.Add(t.ID);

            CurrentTabs = tabs;
        }
        else
            CurrentTabs = null;

        CurrentLocale = result.Locale;

        // MS-6753
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            var config = api.GetAssociationConfiguration().ResultValue;
            if (config != null)
            {
                var assocConfig = config.ConvertTo<msAssociationConfigurationContainer>();
                CurrentLocale = assocConfig.Locale;
            }
        }

        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(CurrentLocale);

        AccessibleEntities = result.AccessibleEntities;

        setUserIdentificationCookie();
    }

    public static bool SetSessionId(string sessionId)
    {
       
        SessionManager.Set("ConciergeAPISessionID",sessionId);

        return true;
    }

    public static void ClearSession()
    {
        SessionManager.Set<object>("ConciergeAPISessionID", null);
        SessionManager.Set<object>("APIMessageHeader", null);

        CurrentUser = null;
        CurrentEntity = null;
        CurrentAssociation = null;
        CurrentTabs = null;
        AccessibleEntities = null;

        MultiStepWizards.ClearAll();

        setUserIdentificationCookie();
    }

    public bool TryGetSessionId(out string sessionId)
    {
        // A.R. 4/30/2013 - unnecessary, we're always in context
        //if (HttpContext.Current == null || HttpContext.Current.Session == null)
        //{
        //    sessionId = null;
        //    return false;
        //}

        sessionId = SessionManager.Get<string>("ConciergeAPISessionID");

        return true;
    }
     
    /// <summary>
    /// Determines whether [is module active] [the specified module to check].
    /// </summary>
    /// <param name="moduleToCheck">The module to check.</param>
    /// <returns>
    /// 	<c>true</c> if [is module active] [the specified module to check]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsModuleActive(string moduleToCheck)
    {
        if (moduleToCheck == null) throw new ArgumentNullException("moduleToCheck");

        if (CurrentTabs == null) return false;

        return CurrentTabs.Contains(moduleToCheck);

    }


    public static string ResolveResource(string resourceName, bool returnNullIfNothingFound)
    {
        // we disabled this because the resource list was just way too big to store in session
        //if (Resources != null && Resources.Exists(x => x.Name.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)))
        //    return Resources.First(x => x.Name.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)).Value;

        return !returnNullIfNothingFound ? resourceName : null;
    }

    public static string LogException(Exception ex, Uri u)
    {
        //if there's no http context do not log
        //If we don't know what association this is we can't log either because a auditlog requires a wall to save
        if (HttpContext.Current == null ||   CurrentAssociation == null)
            return null;

        try
        {
            NameValueCollection queryStringVariables = new NameValueCollection();
            if (u != null)
                queryStringVariables = HttpUtility.ParseQueryString(u.Query);
            string contextId = queryStringVariables["contextID"];

            MemberSuiteObject contextObject = null;

            using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                //Get the current user details from the Concierge API
                LoginResult loginResult = proxy.WhoAmI().ResultValue;
                msUser currentUser = loginResult.PortalUser == null ? loginResult.User.ConvertTo<msUser>() : loginResult.PortalUser.ConvertTo<msUser>();

                //If there's a specified context then get the details on the context object
                if (!string.IsNullOrWhiteSpace(contextId))
                {
                    ConciergeResult<MemberSuiteObject> getResult = proxy.Get(contextId);
                    if (getResult.Success)
                        contextObject = getResult.ResultValue;
                }

                msAuditLog auditLog = constructAuditLog(currentUser, ex, contextId, contextObject);
                ConciergeResult<MemberSuiteObject> saveResult = proxy.RecordErrorAuditLog(auditLog);
                
                if (saveResult.Success)
                    return saveResult.ResultValue.SafeGetValue<string>("ID");
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Unable to log exception.");
            Console.WriteLine(e.ToString());
            Console.WriteLine(ex.ToString());
        }

        return null;
    }

    private static msAuditLog constructAuditLog(msUser currentUser, Exception ex, string contextId, MemberSuiteObject contextObject)
    {
        msAuditLog result = new msAuditLog();
        result.AffectedRecord_ID = contextId;
        result.Type = AuditLogType.Error;

        if (currentUser != null)
            result.Actor = currentUser.ID;

        if (contextObject != null)
        {
            result.AffectedRecord_Type = contextObject.ClassType;
            result.AffectedRecord_Name = contextObject.SafeGetValue<string>("Name");
        }

        result.Description = ex.ToString();

        return result;
    }

    #region IConciergeAPISessionIdProvider Implemetation

    bool IConciergeAPISessionIdProvider.TryGetSessionId(out string sessionId)
    {
        return TryGetSessionId(out sessionId);
    }

    bool IConciergeAPISessionIdProvider.SetSessionId(string sessionId)
    {
        return SetSessionId(sessionId);
    }

    #endregion

    #region IConciergeAPIBrowserIdProvider Implementation

    public static bool TryGetBrowserId(out string browserId)
    {
        if (HttpContext.Current == null)
        {
            browserId = null;
            return false;
        }

        //Try the per-request cache first - if a value exists, use that one
        if (HttpContext.Current.Items.Contains(BrowserCacheKey))
        {
            browserId = HttpContext.Current.Items[BrowserCacheKey] as string;

            if (!string.IsNullOrWhiteSpace(browserId))
                return true;
        }

        //Check the cookie if the browser id wasn't found in Items
        HttpCookie browserIdCookie =
            HttpContext.Current.Request.Cookies[BrowserCacheKey];

        //If there's no cookie then create a new one and set a random browser id
        if (browserIdCookie == null || (browserIdCookie.Expires != DateTime.MinValue && browserIdCookie.Expires < DateTime.Now) || string.IsNullOrWhiteSpace(browserIdCookie.Value))
        {
            browserId = Guid.NewGuid().ToString();

            browserIdCookie = new HttpCookie(BrowserCacheKey, browserId);
        }

        //The browser id should now be in the cookie
        browserId = browserIdCookie.Value;

        //Save the cookie
        browserIdCookie.Expires = DateTime.Now.AddYears(10);
        var existingBrowserCookie = HttpContext.Current.Response.Cookies[BrowserCacheKey];
        if (existingBrowserCookie == null)
            HttpContext.Current.Response.AppendCookie(browserIdCookie);
        else
            HttpContext.Current.Response.SetCookie(browserIdCookie);

        //Put the value in the Items collection to access it quicker
        HttpContext.Current.Items[BrowserCacheKey] = browserId;
        return true;
    }

    bool IConciergeAPIBrowserIdProvider.TryGetBrowserId(out string browserId)
    {
        return TryGetBrowserId(out browserId);
    }

    #endregion
}
