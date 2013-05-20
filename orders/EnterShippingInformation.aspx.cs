using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class orders_EnterShippingInformation : PortalPage
{
    #region Fields

    private msOrder targetOrder;
    private bool isTransient;
    private PreProcessedOrderPacket preProcessedOrderPacket;


    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the target object for the page
    /// </summary>
    /// <remarks>Many pages have "target" objects that the page operates on. For instance, when viewing
    /// an event, the target object is an event. When looking up a directory, that's the target
    /// object. This method is intended to be overriden to initialize the target object for
    /// each page that needs it.</remarks>
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        if (string.Equals(Request.QueryString["useTransient"], "true", StringComparison.CurrentCultureIgnoreCase))
        {
            targetOrder = MultiStepWizards.PlaceAnOrder.TransientShoppingCart;
            isTransient = true;
        }
        else
            targetOrder = MultiStepWizards.PlaceAnOrder.ShoppingCart;

        if(targetOrder == null)
        {
            QueueBannerError("Unable to checkout without an active shopping cart.");
            GoHome();
            return;
        }

        //MS-2823
        if (targetOrder.BillTo == null)
            targetOrder.BillTo = ConciergeAPI.CurrentEntity.ID;

        if (targetOrder.ShipTo == null)
            targetOrder.ShipTo = targetOrder.BillTo;
    }

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();

        // let's preprocess and figure out whether we need shipping information
        using (var api = GetConciegeAPIProxy())
        {
            preProcessedOrderPacket = api.PreProcessOrder(targetOrder).ResultValue;
        }


        if (!preProcessedOrderPacket.ShippingMethodRequired)    // no shipping method needed
            GoTo("ConfirmOrder.aspx?useTransient=" + isTransient);

     
       setupShipping();

    }


    #endregion

    #region Methods

    /// <summary>
    /// Setups the shipping section, which only dislays if the shipping method is required
    /// </summary>
    private void setupShipping()
    {
       
        // when the order is preprocessed, a default shipping method is selected

        // let's harvest that from the pre-processed order
        targetOrder.ShippingMethod = preProcessedOrderPacket.FinalizedOrder.SafeGetValue<string>(msOrder.FIELDS.ShippingMethod);
        targetOrder.ShippingAddress = preProcessedOrderPacket.FinalizedOrder.SafeGetValue<Address>(msOrder.FIELDS.ShippingAddress);

        // ok, so let's show the shipping panel
        divShipping.Visible = true;

        if (targetOrder.ShippingAddress == null)
            targetOrder.ShippingAddress = CurrentEntityPreferredAddress;

        acShipping.Address = targetOrder.ShippingAddress;

        // we need to get all of the available shipping methods from the API
        Search sShippingMethods = new Search { Type = msShippingMethod.CLASS_NAME };
        sShippingMethods.AddCriteria(Expr.Equals("IsActive", true));
        sShippingMethods.AddOutputColumn("Name");
        sShippingMethods.AddOutputColumn("IsDefault");
        sShippingMethods.AddSortColumn("Name");

        var dtShippingMethods = ExecuteSearch(sShippingMethods, 0, null).Table;

        foreach (DataRow dr in dtShippingMethods.Rows)
        {
            ListItem li = new ListItem(Convert.ToString(dr["Name"]), Convert.ToString(dr["ID"]));

            // select the default method
            li.Selected = li.Value == targetOrder.ShippingMethod;
            rblShipping.Items.Add(li); // add it to the list
        }

        if (rblShipping.Items.Count == 0)
            rblShipping.Items.Add(new ListItem("No shipping method available.", null));

       if (rblShipping.SelectedIndex < 0)
             rblShipping.SelectedIndex = 0;  // almost select the first


    }

    #endregion

    #region Event Handlers

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        if (isTransient)
        {
            MultiStepWizards.PlaceAnOrder.TransientShoppingCart = null;
            MultiStepWizards.PlaceAnOrder.CrossSellItems = null;
        }


        GoHome();
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return ;

        targetOrder.ShippingAddress = acShipping.Address;
        targetOrder.ShippingMethod = rblShipping.SelectedValue;
       

        GoTo("ConfirmOrder.aspx?useTransient=" + isTransient );

    }

    #endregion
}