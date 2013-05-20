using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class exhibits_RegistrationNotAvailable : PortalPage 
{
    public msExhibitShow targetShow;
    public msEntity targetEntity;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetShow = LoadObjectFromAPI<msExhibitShow>(ContextID);
        if (targetShow == null) GoToMissingRecordPage();
        targetEntity = LoadObjectFromAPI<msEntity>(Request.QueryString["entityID"]);
        if (targetEntity == null) GoToMissingRecordPage();


    }

    protected override void InitializePage()
    {
        base.InitializePage();
        lblExhibitorName.Text = string.Format("#{0} - {1}",
            targetEntity.LocalID, targetEntity.Name);
    }
}