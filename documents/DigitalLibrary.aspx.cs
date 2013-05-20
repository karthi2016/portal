using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class documents_DigitalLibrary : PortalPage 
{

    protected override void InitializePage()
    {
        base.InitializePage();

        // let's get all of the entitlements

        List<FolderInfo> folders = new List<FolderInfo>();
        using (var api = GetServiceAPIProxy())
        {
            var reports = api.ListEntitlements(ConciergeAPI.CurrentEntity.ID, msFileFolderEntitlement.CLASS_NAME).ResultValue;

            // ok, we'll convert them to something the repeater will use
            foreach (var r in reports)
            {
                var folderInfo = new FolderInfo
                                     {
                                         FolderName = api.GetName(r.Context).ResultValue,
                                         FolderID = r.Context
                    
                                     };
                if (r.AvailableUntil == null)
                    folderInfo.FolderDescription = "You have access to this folder indefinitely.";
                else
                    folderInfo.FolderDescription = string.Format("You have access to this folder until {0} at {1}", r.AvailableUntil.Value.ToLongDateString(),
                        r.AvailableUntil.Value.ToShortTimeString() );
                folders.Add(folderInfo);
            }
        }
        
        // of course, sort them by name
        folders.Sort((x, y) => string.Compare(x.FolderName, y.FolderName));

        // and then bind them
        rptFolders.DataSource = folders;
        rptFolders.DataBind();
    }
    protected void btnGoHome_Cick(object sender, EventArgs e)
    {
        GoHome();
    }
}