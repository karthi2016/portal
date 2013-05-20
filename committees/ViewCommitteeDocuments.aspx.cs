using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class committees_ViewCommitteeDocuments : PortalPage 
{
    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return true;

        using (var api = GetConciegeAPIProxy())
            return CommitteeLogic.IsMember(api, ContextID, ConciergeAPI.CurrentEntity.ID);
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        using( var api = GetConciegeAPIProxy())
        {
            var folder = api.GetFileCabinetRootFolder(ContextID).ResultValue.SafeGetValue<string>("ID");
            GoTo("/documents/BrowseFileFolder.aspx?contextID=" + folder);
        }
    }
  
}