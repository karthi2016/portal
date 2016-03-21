using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Manages all communication and storage of session information.
/// </summary>
/// <remarks>This class was designed to work with out-of-proc session stores, and thus be a single 
/// gateway for getting/setting values. It does some work to make actual stored values a lot smaller
/// than they would be otherwise.</remarks>
public static class SessionManager
{
    /// <summary>
    /// These Session values should not always be cleared as there may be some items that should pass from a 
    /// logged out session into the logged in session (such as Shopping Cart)
    /// </summary>
    private static List<string> SessionClearExceptions = new List<string>
    {
        "MemberSuite:PlaceAnOrder.ShoppingCart",
        "MemberSuite:CreateAccount.CompleteUrl",
        "PortalInformation",
        "AssociationId",
        "ConciergeAPIBrowserID",
        "APMUser",
        "msAssociationConfigurationContainer",
        "ConciergeAPIBackgroundUser",
        "ConciergeAPICurrentAssociation",
        "ConsoleReturnUrl",
        "LogoutUrl",
        "ConsoleUrl",
        "ConciergeAPISessionID",
        "APIMessageHeader"
    };

    public static T Get<T>(string key)
    {
        if (HttpContext.Current == null || HttpContext.Current.Session == null)
            return default(T);

        // first, is it in context items?
        object sessionValue = HttpContext.Current.Items[key];
        if (sessionValue != null)
            return (T) sessionValue;

        // no, try the session
        sessionValue = HttpContext.Current.Session[key];
        if (sessionValue == null)
            return default(T);

        return (T) sessionValue;
    }

    /// <summary>
    /// Clear data out of Session.
    /// </summary>
    /// <param name="clearAll">Clear entire Session. If false, will leave a list of exceptions.</param>
    public static void Clear(bool clearAll = true)
    {
        // Only really need to clear this. HttpContext.Current.Items should be fresh on each new Request already.
        if (HttpContext.Current.Session != null)
        {
            var persistValues = new Dictionary<string, object>();
            if (!clearAll && SessionClearExceptions.Count > 0)
            {
                foreach (var sessionClearException in SessionClearExceptions)
                {
                    var value = HttpContext.Current.Session[sessionClearException];
                    if (value != null)
                    {
                        persistValues.Add(sessionClearException, value);
                    }
                }
            }

            HttpContext.Current.Session.Clear();

            foreach (var persistValue in persistValues)
            {
                HttpContext.Current.Session.Add(persistValue.Key, persistValue.Value);
            }
        }
    }

    public static void Set<T>(string key, T valueToSet)
    {
        if (HttpContext.Current == null || HttpContext.Current.Session == null)
            return;

        HttpContext.Current.Items[key] = valueToSet; // always store in items

        object valueToActuallyStoreInSession = valueToSet;

        //// is this a membersuite object?
        //MemberSuiteObject mso = valueToSet as MemberSuiteObject;

        //if (mso != null && mso.SafeGetValue("SystemTimestamp") != null)   // it's in the DB
        //    valueToActuallyStoreInSession = mso.SafeGetValue<string>("ID");

        HttpContext.Current.Session[key] = valueToActuallyStoreInSession;
    }

    /// <summary>
    /// Get value from Session Cache and if it does not exist, generate value and cache it. This function only works with nullable types.
    /// </summary>
    /// <typeparam name="T">Type of the object we expect to find in Session Cache.</typeparam>
    /// <param name="key">Unique identifier to look for in Session Cache.</param>
    /// <param name="retrieveValue">Function to retrieve value if not found in Session Cache.</param>
    /// <returns>Value found in or added to Session Cache.</returns>
    public static T Get<T>(string key, Func<T> retrieveValue)
    {
        // See if already cached
        var cachedValue = Get<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        // Exececute function to generate value
        cachedValue = retrieveValue();

        // Cache value before returning it
        Set(key, cachedValue);
        return cachedValue;
    }

    /* NoSQL
    static SessionManager()
    {
        _sessionProvider = NoSQLEngine.GetProvider();
        noSQLTableName = ConfigurationManager.AppSettings["NoSQL_SessionsTable"];
    }

    static INoSQLProvider _sessionProvider;
    static readonly string noSQLTableName;

    public const int Timeout = 60;

    public static T Get<T>( string key )  
	{
        if (HttpContext.Current == null || HttpContext.Current.Session == null)
            return default(T);

        var keyValues = _sessionProvider.GetItem(noSQLTableName, key);
        if (keyValues == null)
            return default(T);

        object valToReturn;
        if (keyValues.TryGetValue("Value", out valToReturn))
            return (T)valToReturn;

        return default(T);
	}

    public static void Set<T>(string key, T valueToSet)
    {
        if (HttpContext.Current == null || HttpContext.Current.Session == null)
            return;

        var items = new Dictionary<string,object>();
        items.Add("ID", key);
        items.Add( "Value", valueToSet );
        _sessionProvider.PutItem(noSQLTableName, items);
        //HttpContext.Current.Session[key] = valueToSet;
    }
    */
}