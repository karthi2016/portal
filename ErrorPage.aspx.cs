using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class ErrorPage : PortalPage
{
    #region Fields

    protected msAuditLog targetAuditLog;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get { return true; }
    }

    #endregion

    protected override void checkThrowManualException()
    {
        // no-op, avoid loops
    }

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        if(!string.IsNullOrWhiteSpace(ContextID))
            targetAuditLog = LoadObjectFromAPI<msAuditLog>(ContextID);
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        if (!string.IsNullOrWhiteSpace(ContextID))
        {
            litErrorSummary.Text =
                string.Format("In order to help diagnose the cause of this error, details have been logged in Audit Log <b>#{0}</b>.",
                              ContextID);
            trDetails.Visible = true;
        }

        divErrorDetails.Visible = false;
        if(targetAuditLog != null)
        {
            divErrorDetails.Visible = true;
            litErrorDetails.Text = targetAuditLog.Description;
        }

        Response.StatusCode = 500;
        Response.TrySkipIisCustomErrors = true; // important - keeps IIS from trashing our pretty error page
        
    }

    #endregion
}