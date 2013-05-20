using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logout : System.Web.UI.Page
{
    protected string NextUrl
    {
        get { return Request.QueryString["n"]; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ConciergeAPI.ClearSession();
        SessionManager.Set<object>("PortalLinks", null );  // force portal link reload, so non-public links don't show

        if (!string.IsNullOrWhiteSpace(NextUrl) && Uri.IsWellFormedUriString(NextUrl, UriKind.RelativeOrAbsolute))
        {
            Response.Redirect(NextUrl);
            return;
        }

        if (!string.IsNullOrWhiteSpace(ConciergeAPI.LogoutUrl))
        {
            Response.Redirect(ConciergeAPI.LogoutUrl);
            return;
        }

        Response.Redirect("~/Login.aspx");
    }
}