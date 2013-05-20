using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class orders_InitiateOrder : Page 
{


#region Fields

    private msOrder targetOrder;
    private bool isTransient;
    

    #endregion

    #region Initialization
 

    protected override void OnLoad(EventArgs e)
    {


        if (Request.QueryString["useTransient"] == "true")
        {
            targetOrder = MultiStepWizards.PlaceAnOrder.TransientShoppingCart;
            isTransient = true;
        }
        else
            targetOrder = MultiStepWizards.PlaceAnOrder.ShoppingCart;

        if (targetOrder == null || targetOrder.LineItems == null || targetOrder.LineItems.Count == 0)
        {

            Response.Redirect( "/");
            return;
        }

        if (targetOrder.BillTo == null)
            targetOrder.BillTo = ConciergeAPI.CurrentEntity.ID;

        if (targetOrder.ShipTo == null)
            targetOrder.ShipTo = ConciergeAPI.CurrentEntity.ID;

        Response.Redirect("CrossSellItems.aspx?useTransient=" + isTransient);
         


       

    }


    #endregion

 
}