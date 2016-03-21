using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

/// <summary>
/// Functions to streamline Page Layout functionality.
/// </summary>
public static class MetadataLogic
{
    public static ClassMetadata DescribeObject(this MemberSuiteObject mso)
    {
        return DescribeObject(mso.ClassType);
    }

    public static ClassMetadata DescribeObject(string className)
    {
        var key = string.Format("ClassMetadata::{0}", className);

        return SessionManager.Get(key, () =>
        {
            using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var classMetadata = api.DescribeObject(className).ResultValue;

                return classMetadata;
            }
        });
    }

    public static msPortalPageLayoutContainer GetAppropriatePageLayout(this MemberSuiteObject mso)
    {
        var typeId = mso.SafeGetValue<string>("Type");
        var objectType = mso.ClassType;

        if (string.IsNullOrWhiteSpace(typeId))
            return GetDefaultPageLayout(objectType);

        var key = string.Format("GetAppropriatePageLayout::{0}_{1}", objectType, typeId);

        return SessionManager.Get(key, () =>
        {
            // let's get the type
            using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var msoType = api.Get(typeId).ResultValue;
                var layoutId = msoType.SafeGetValue<string>("PortalPageLayout");

                if (string.IsNullOrWhiteSpace(layoutId))
                    return GetDefaultPageLayout(objectType);

                var pageLayout = api.Get(layoutId).ResultValue.ConvertTo<msPortalPageLayoutContainer>();

                return pageLayout;
            }
        });
    }

    public static msPortalPageLayoutContainer GetDefaultPageLayout(this MemberSuiteObject mso)
    {
        return GetDefaultPageLayout(mso.ClassType);
    }

    public static msPortalPageLayoutContainer GetDefaultPageLayout(string pageLayoutObject)
    {
        var key = string.Format("GetDefaultPageLayout::{0}", pageLayoutObject);

        return SessionManager.Get(key, () =>
        {
            using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var s = new Search(msPortalPageLayoutContainer.CLASS_NAME);
                s.AddCriteria(Expr.Equals("ApplicableType", pageLayoutObject));
                s.AddCriteria(Expr.Equals("IsDefault", true));

                var pageLayout = api.GetObjectBySearch(s, null).ResultValue.ConvertTo<msPortalPageLayoutContainer>();
             
                // Default should not be "null" so that we will cache something and stop querying.
                return pageLayout ?? new msPortalPageLayoutContainer();
            }
        });
    }


    public static msPortalPageLayoutContainer GetPageLayout(string pageLayoutId)
    {
        var key = string.Format("GetPageLayout::{0}", pageLayoutId);

        return SessionManager.Get(key, () =>
        {
            using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var pageLayout = api.Get(pageLayoutId).ResultValue.ConvertTo<msPortalPageLayoutContainer>();
                return pageLayout;
            }
        });
    }
}