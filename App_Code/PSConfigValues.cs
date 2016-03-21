using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

/// <summary>
/// Repsponsible for accessing data stored in cofiguration settings.
/// It is helpful to have it centralized in one location when the need to find out how many confiration settings are being used by a portal instance.
/// 
/// Create a readonly property for the configuration value you want to get. When the value is requested, pass the key to GetConfigValue method.
/// <example>See SAMPLE USAGE below</example>
/// </summary>
public static class PSConfigValues
{
    private static readonly Dictionary<string, MemberSuiteObject> _cache = new Dictionary<string, MemberSuiteObject>();
	
    /// <summary>
    /// By throwing exceptions of non exisiting configurations, 
    /// it lets support/testers know the issue immediately so they can take appropriate action without having to contact engineers.
    /// </summary>
    /// <param name="key">Configuration Key</param>
    /// <returns></returns>
    private static string GetConfigValue(string key)
    {
        var error = String.Format("No configuration found for {0}.", key);
        var v = ConciergeSettings.GetConfigurationSetting(key);
        if (String.IsNullOrWhiteSpace(v))
            throw new ConciergeClientException(ConciergeErrorCode.GeneralException, error);
        return v.ToLower();

    }
    
    /// <summary>
    /// Generic MemberSuiteObject accessor
    /// </summary>
    /// <param name="id">GUID- object Id</param>
    /// <returns></returns>
    private static MemberSuiteObject GetConfigObject(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        if (_cache.ContainsKey(id))
            return _cache[id];

        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            
            var result = api.Get(id).ResultValue;
            if (result == null)
                return null;
            _cache.Add(id, result);
            return result;
        }

    }
     //SAMPLE USAGE
    /*
    public static string CustomDuesProductId {
        get
        {
            return GetConfigValue("THE CONFIG KEY");
        }
    }

    public static msMembershipDuesProduct CustomDuesProduct
    {
        get { return GetConfigObject(CustomDuesProductId).ConvertTo<msMembershipDuesProduct>(); }
    }     
     */
}