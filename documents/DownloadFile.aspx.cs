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
    public string ContextID;

    protected override void OnLoad(EventArgs e)
    {

        base.OnLoad(e);

        ContextID = Request.QueryString["contextID"];

        if (!CheckSecurity())
            Response.Redirect("/AccessDenied.aspx");

        if (!IsPostBack)
            InitializePage();
    }
    protected  bool CheckSecurity()
    {
        if (DocumentsLogic.CheckTemporaryFileAccess(ContextID))
            return true;

        return DocumentsLogic.CanAccessFile( ContextID );
    }

    protected void InitializePage()
    {
        
        
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            var fileURL = api.DownloadFile(ContextID).ResultValue;
            Response.Redirect(fileURL);
            
        }
        
    }
}