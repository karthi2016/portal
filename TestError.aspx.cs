using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TestError : PortalPage
{
    protected override void InitializeTargetObject()
    {
        throw new ApplicationException("Test Error");
    }
}