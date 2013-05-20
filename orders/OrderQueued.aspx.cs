using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class orders_OrderQueued : PortalPage 
{

   
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

      
        if(MultiStepWizards.PlaceAnOrder.ReloadEntityOnOrderComplete)
        {
            MultiStepWizards.PlaceAnOrder.ReloadEntityOnOrderComplete = false;
            ConciergeAPI.CurrentEntity = LoadObjectFromAPI<msEntity>(ConciergeAPI.CurrentEntity.ID);
        }

       
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        
    }

   
}