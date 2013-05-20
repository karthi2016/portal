using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for NavigationLogic
/// </summary>
public static class NavigationLogic
{
 

    public static string GetUrlFor(MemberSuiteObject mso)
    {
        if (mso == null) throw new ArgumentNullException("mso");

        switch( mso.ClassType )
        {
            case msCommittee.CLASS_NAME:
                return "/committees/ViewCommittee.aspx?contextID=" + mso["ID"];

            case msChapter.CLASS_NAME:
                return "/chapters/ViewChapter.aspx?contextID=" + mso["ID"];

            case msSection.CLASS_NAME:
                return "/sections/ViewSection.aspx?contextID=" + mso["ID"];

            case msOrganizationalLayer.CLASS_NAME:
                return "/organizationallayers/ViewOrganizationalLayer.aspx?contextID=" + mso["ID"];

            default:
                throw new NotSupportedException("Cannot find url for class type " + mso.ClassType );
        }
    }
}