using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class documents_DownloadFile : Page 
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
            var file = api.Get(ContextID).ResultValue ;
            if (file == null)
                throw new ApplicationException("unable to locate file");

            Response.ClearContent();
            Response.Clear();
            Response.ContentType = file.SafeGetValue<string>(msFile.FIELDS.ContentType);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + file.SafeGetValue<string>("Name"));

            var strFileContents = file.SafeGetValue<string>(msFile.FIELDS.FileContents); 
            Response.BinaryWrite( Convert.FromBase64String( strFileContents ) );
        }
        Response.End();
    }
}