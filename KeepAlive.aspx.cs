using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class KeepAlive : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        StringBuilder sbCloseScript = new StringBuilder();
        sbCloseScript.AppendLine("<script language=\"JavaScript\">");
        sbCloseScript.AppendLine("HideThisRadWindow();");
        sbCloseScript.AppendLine("</script>");
        
        ClientScript.RegisterStartupScript(typeof(Page), "closeme", sbCloseScript.ToString(), false);
    }
}