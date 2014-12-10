using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class documents_DownloadFile : PortalPage
{
    protected override bool CheckSecurity()
    {
        return DocumentsLogic.CheckTemporaryFileAccess(ContextID) || DocumentsLogic.CanAccessFile(ContextID);
    }

    protected override void InitializePage()
    {
        if (ContextID == null)
        {
            QueueBannerError("No File Provided.");
            GoHome();
        }

        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            var r = api.DownloadFile(ContextID);
            var fileURL = r.ResultValue;
            Response.Redirect(fileURL);
        }
    }
}