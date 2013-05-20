using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Types;

public static class MemberSuiteConfigurationSettings
{
    private static string ns;
    private static readonly object threadLock = new object();

    private static List<NameValuePair> Settings
    {
        get
        {
            lock (threadLock)
            {
                //Storing settings in the session so that changing the settings doesn't require
                //an application restart - but we still get some benefit of caching instead of going back to the API
                //each time
                var settings = SessionManager.Get<List<NameValuePair>>("MemberSuiteConfigurationSettings");

                if (settings != null)
                    return settings;

                settings = getAllMemberSuiteConfigurationSettings(ns);
                if (settings != null)
                {
                    SessionManager.Set("MemberSuiteConfigurationSettings",settings);
                    return settings;
                }

                return new List<NameValuePair>();
            }
        }
    }

    static MemberSuiteConfigurationSettings()
    {
        lock(threadLock)
        {
            ns = ConfigurationManager.AppSettings["MemberSuiteConfigurationSettingsNamespace"];
            if (string.IsNullOrWhiteSpace(ns))
                ns = "portal";
        }
    }

    public static string GetSetting(string settingName)
    {
        var setting =
            Settings.FirstOrDefault(x => string.Equals(x.Name, settingName, StringComparison.InvariantCultureIgnoreCase));

        return string.IsNullOrWhiteSpace(setting.Name) ? null : setting.Value as string;
    }

    private static List<NameValuePair> getAllMemberSuiteConfigurationSettings(string ns)
    {
        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            ConciergeResult<List<NameValuePair>> result = proxy.GetAllConfigurationSettings(ns);
            if (!result.Success)
                return null;

            return result.ResultValue;
        }
    }
}