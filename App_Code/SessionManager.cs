using System.Web;

/// <summary>
/// Manages all communication and storage of session information.
/// </summary>
/// <remarks>This class was designed to work with out-of-proc session stores, and thus be a single 
/// gateway for getting/setting values. It does some work to make actual stored values a lot smaller
/// than they would be otherwise.</remarks>
public static class SessionManager
{
    public static T Get<T>( string key )  
	{
        if (HttpContext.Current == null || HttpContext.Current.Session == null)
            return default(T);

        // first, is it in context items?
        object sessionValue = HttpContext.Current.Items[key];
        if (sessionValue != null)
            return (T)sessionValue;

        // no, try the session
        sessionValue = HttpContext.Current.Session[key];
        if (sessionValue == null)
            return default(T);
        
        return (T)sessionValue ;
	}

    public static void Clear()
    {
        // Must Clear this first as it will be lost on the "Current.Items.Clear()" call
        if (HttpContext.Current.Session != null)
        {
            HttpContext.Current.Session.Clear();
        }

        if (HttpContext.Current.Items != null)
        {
            HttpContext.Current.Items.Clear();
        }
    }

    public static void Set<T>(string key, T valueToSet)
    {
        if (HttpContext.Current == null || HttpContext.Current.Session == null)
            return;

        HttpContext.Current.Items[key] = valueToSet;    // always store in items

        object valueToActuallyStoreInSession = valueToSet;

        //// is this a membersuite object?
        //MemberSuiteObject mso = valueToSet as MemberSuiteObject;

        //if (mso != null && mso.SafeGetValue("SystemTimestamp") != null)   // it's in the DB
        //    valueToActuallyStoreInSession = mso.SafeGetValue<string>("ID");

        HttpContext.Current.Session[key] = valueToActuallyStoreInSession;
    }

    /*NoSQL
     * static SessionManager()
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
     * */
}