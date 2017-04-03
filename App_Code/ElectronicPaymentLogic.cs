using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for ElectronicPaymentLogic
/// </summary>
public static class ElectronicPaymentLogic
{
    public static msBusinessUnit GetDefaultBusinessUnit()
    {
        var key = "GetDefaultBusinessUnit";
        return SessionManager.Get(key, () =>
        {
            using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var s = new Search { Type = msBusinessUnit.CLASS_NAME };
                s.AddOutputColumn("ID");
                s.AddCriteria(Expr.Equals("IsDefault", true));
                var businessUnit = api.GetObjectBySearch(s, null).ResultValue.ConvertTo<msBusinessUnit>();

                return businessUnit;
            }
        });
    }

    public static msMerchantAccount GetDefaultMerchantAccount(string businessUnit)
    {
        var key = string.Format("GetDefaultMerchantAccount::{0}", businessUnit);
        return SessionManager.Get(key, () =>
        {
            using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var s = new Search { Type = msMerchantAccount.CLASS_NAME };
                s.AddOutputColumn("ID");
                s.AddCriteria(Expr.Equals("IsDefault", true));
                s.AddCriteria(Expr.Equals("BusinessUnit", businessUnit));
                var merchantAccount = api.GetObjectBySearch(s, null).ResultValue.ConvertTo<msMerchantAccount>();

                return merchantAccount;
            }
            
        });
    }
}