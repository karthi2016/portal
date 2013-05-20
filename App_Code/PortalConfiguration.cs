using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for PortalConfiguration
/// </summary>
public class PortalConfiguration : IConciergeAPIAssociationIdProvider
{
    #region Properties

    /// <summary>
    /// Gets the current portal configuration
    /// </summary>
    /// <value>The current.</value>
    public static PortalInformation Current
    {
        get
        {
            if (HttpContext.Current == null) return null;


            PortalInformation pi = (PortalInformation)HttpContext.Current.Items["PortalInformation"];
            if (pi != null) return pi;

            pi = SessionManager.Get<PortalInformation>("PortalInformation");


            if (pi != null)
            {
                HttpContext.Current.Items["PortalInformation"] = pi; // so we don't have to get from session

                return pi;
            }

            pi = retrievePortalInformation();
            SessionManager.Set("PortalInformation",pi);
            HttpContext.Current.Items["PortalInformation"] = pi;    // so we don't have to get from session

            return pi;
        }
    }

    /// <summary>
    /// Gets the current portal configuration
    /// </summary>
    /// <value>The current.</value>
    public static msAssociationConfigurationContainer CurrentConfig
    {
        get
        {
            if (HttpContext.Current == null) return null;
            msAssociationConfigurationContainer pi = 
                SessionManager.Get<msAssociationConfigurationContainer>( "msAssociationConfigurationContainer" );
            if (pi != null) return pi;
            pi = retrieveAssociationConfiguration();
            SessionManager.Set("msAssociationConfigurationContainer",pi);

            return pi;
        }
    }

    private static msAssociationConfigurationContainer retrieveAssociationConfiguration()
    {
        var context = HttpContext.Current;
        if (context == null) return null;

        // first, we need to see if we can locate the association ID
        // if we have that, then we are good - we can get portal information based on that

        // but if we DON'T, we have to try to use the host name
        string associationID = tryToRetrieveAssociationID();


        msAssociationConfigurationContainer pi = null;

        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            pi = api.GetAssociationConfiguration().ResultValue.ConvertTo<msAssociationConfigurationContainer>();
            
        return pi;
    }

    public static DataTable OrganizationalLayerTypes
    {
        get
        {
            DataTable dt = SessionManager.Get<DataTable>("OrganizationalLayerTypes") ;
            if ( dt == null)
            {
                dt = getOrganizationalLayerTypes();
                SessionManager.Set("OrganizationalLayerTypes",dt);
            }

            return dt;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Will attempt to determine the association id for the request by searching the session, configuration, then by request portal information from the Concierge API using the host name of the request.
    /// 
    /// If no association id can be determined an exception will be thrown.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static string tryToRetrieveAssociationID()
    {
        var context = HttpContext.Current;
        if (context == null) return null ;


        string associationID = null;

        // check the query string FIRST - this let's us override if necessary
        associationID = HttpContext.Current.Request.QueryString["a"];
        if (associationID != null) return associationID;

       
        // ok - is it specified explicitly in the web.config file?
        associationID = ConfigurationManager.AppSettings["AssociationID"];

        // yep
        if (associationID != null) return associationID;

       
        
        // ok, I give up
        return null;

       
    }

    /// <summary>
    /// Retrieves the portal information based on a number of factors
    /// </summary>
    /// <returns></returns>
    private static PortalInformation retrievePortalInformation()
    {
        var context = HttpContext.Current;
        if (context == null) return null;

        // first, we need to see if we can locate the association ID
        // if we have that, then we are good - we can get portal information based on that

        // but if we DON'T, we have to try to use the host name
        string associationID = tryToRetrieveAssociationID();


        ConciergeResult<PortalInformation> pi = null;

        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {

            // If there is no specific association id for this portal intance then it is running in multi-tenant mode.  If so, we need to get the association id
            // by passsing the host name into the Concierge API
            if (associationID != null)
            {
                pi = api.RetrievePortalInformationByID();
            }
            else
            {
                var urlString = string.Format("{0}://{1}", context.Request.Url.Scheme,
                                              context.Request.Url.Authority);
                pi = api.RetrievePortalInformationByUrl(urlString);
            }
        }
        // ok - we give up - return a 404 indicating that this portal url is not known
        if (pi == null || !pi.Success || pi.ResultValue == null)
            throw new ApplicationException(
                "Unable to determine association for this portal. Use the configuration file, a query string, or a valid host name.");

        SessionManager.Set( "PortalInformation", pi); // remember it

        return pi.ResultValue;

    }

    private static DataTable getOrganizationalLayerTypes()
    {
        Search sOrganizationalLayerTypes = new Search(msOrganizationalLayerType.CLASS_NAME);
        sOrganizationalLayerTypes.AddOutputColumn("ID");
        sOrganizationalLayerTypes.AddOutputColumn("Name");
        sOrganizationalLayerTypes.AddOutputColumn("MembershipOrganization");
        sOrganizationalLayerTypes.AddOutputColumn("ParentType");
        sOrganizationalLayerTypes.AddOutputColumn("IsDefault");
        
        using(IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            ConciergeResult<SearchResult> srOrganizationalLayerTypes = proxy.ExecuteSearch(sOrganizationalLayerTypes, 0, null);
            DataTable result = srOrganizationalLayerTypes.ResultValue.Table;
            result.PrimaryKey = new[] {result.Columns["ID"]};
            return result;
        }
    }

    #endregion

    public static msPortalControlPropertyOverride GetOverrideFor(string pageName, string controlName, string propertyName)
    {
        if (Current == null) return null;
        if (Current.ControlOverrides == null) return null;
        return Current.ControlOverrides.Find( x=> x.PageName == pageName && x.ControlName == controlName && x.PropertyName == propertyName );
    }

    #region IConciergeAPIAssociationIDProvider

    public bool TryGetAssociationId(out string associationId)
    {
        associationId = tryToRetrieveAssociationID();

        if (!string.IsNullOrWhiteSpace(associationId))
            return true;

        //See if there could be a cached portal information (don't evaluate the Current property because 
        //if it's null it will try to retrieve it from the API resulting in an infinite loop)
        if (HttpContext.Current == null || SessionManager.Get<object>("PortalInformation") == null  )
        {
            //Set the association ID to null because we haven't determined it yet
            //It's in multitenant mode and there is no cached portal information yet
            associationId = null;
        }
        else
        {
            associationId = Current.AssociationID;    
        }

        //Always return true because even a null association id is valid
        //In fact it's necessary when looking up portal information by URL in multitenant mode
        return true;
    }

    public bool SetAssociationId(string associationId)
    {
        //Not implemented because the assocition ID will be determined by the web.config or the Portal Configuration.  Never set manually
        throw new NotImplementedException();
    }

    public bool TryGetAssociationMessageHeader(out MessageHeader header)
    {
        if (HttpContext.Current == null)
        {
            header = null;
            return false;
        }

        //If the association id is configured then it's single-tenant so it will be in the application
        //Otherwise it will be in the session
        string associationId = ConfigurationManager.AppSettings["AssociationId"];

        if (string.IsNullOrWhiteSpace(associationId))
            //Couldn't load it from the web.config so the portal is in multitenant mode
            //and therefore the association ID will be different for differnt session
            //so it should be in the session
            header = SessionManager.Get<MessageHeader>("AssociationId");
        else
        {
            //Single tenant mode so it should be in the application for performance
            header = HttpContext.Current.Application["AssociationId"] as MessageHeader;
        }

        return true;
    }

    public bool SetAssociationMessageHeader(MessageHeader header)
    {
        if (HttpContext.Current == null)
            return false;

        //If the association id is configured then it's single-tenant so store in the application
        //Otherwise store in the session
        string associationId = ConfigurationManager.AppSettings["AssociationId"];

        if(string.IsNullOrWhiteSpace(associationId))
            //Couldn't load it from the web.config so the portal is in multitenant mode
            //and therefore the association ID will be different for differnt session
            //so store it there
            SessionManager.Set("AssociationId",Current.AssociationID);
        else
            //Single tenant mode so store it in the application for performance
            HttpContext.Current.Application["AssociationId"] = header;

        return true;
    }

    #endregion
}

        