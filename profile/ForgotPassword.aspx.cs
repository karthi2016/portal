using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class profile_ForgotPassword : PortalPage
{
    #region Fields

    protected msPortalUser portalUser;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    #endregion

    #region Methods

    protected void tryGetOrCreatePortalUser(IConciergeAPIService serviceProxy)
    {
        //The API GetOrCreatePortalUser will attempt to match the supplied credentials to a portal user, individual, or organization. 
        //If a portal user is found it will be returned.  If not and an individual / organization uses the email address it will create and return a new Portal User
        ConciergeResult<msPortalUser> result = serviceProxy.SearchAndGetOrCreatePortalUser(tbLoginID.Text);

        //The portal user in the result will be null if the e-mail didn't match anywhere
        if (result.ResultValue == null)
            return;

        portalUser = result.ResultValue.ConvertTo<msPortalUser>();

        //Send the forgotten password email
        var sendEmailResult = serviceProxy.SendForgottenPortalPasswordEmail(portalUser.Name,null);
    }

    #endregion

    #region Event Handlers

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            tryGetOrCreatePortalUser(proxy);
        }

        if(portalUser == null)
            QueueBannerError("Unable to determine Portal User from supplied Login ID/E-mail Address.");
        else
            QueueBannerMessage("Password reset email sent successfully.");
        GoTo("/login.aspx");
    }

    #endregion
}