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

        RegisterJavascriptConfirmationBox(lbCancel, "Are you sure you want to cancel this order?");
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
        // MS-4855 - don't forget about cross-sell
        var completeOrder = targetOrder.Clone().ConvertTo<msOrder>();
        var csi = MultiStepWizards.PlaceAnOrder.CrossSellItems;
        if (csi != null && csi.Count > 0)
            completeOrder.LineItems.AddRange(csi.FindAll(x => x.Quantity != 0)); // add any cross sell items
            

        using (var api = GetConciegeAPIProxy())
        {
            preProcessedOrderPacket = api.PreProcessOrder(completeOrder).ResultValue;
        }


        if (!preProcessedOrderPacket.ShippingMethodRequired)    // no shipping method needed
            GoTo("EnterBillingInfo.aspx?useTransient=" + isTransient);

     
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

        // setup billing addresses
        var addresses = ConciergeAPI.CurrentEntity.Addresses;

        rptBillingAddress.DataSource = addresses;
        rptBillingAddress.DataBind();

        if (targetOrder.ShippingAddress != null)
        {
            rbNewBillingAddress.Checked = true;
            acBillingAddress.Address = targetOrder.ShippingAddress;
        }

        

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

        targetOrder.ShippingAddress = GetBillingAddress();
        targetOrder.ShippingMethod = rblShipping.SelectedValue;

        


        GoTo("EnterBillingInfo.aspx?useTransient=" + isTransient);

    }


    public Address GetBillingAddress()
    {
        if (rbNewBillingAddress.Checked)
            return acBillingAddress.Address;

        string addressIndex = Request[rbNewBillingAddress.NamingContainer.UniqueID + "$BillingAddress"];

        if (addressIndex == null)
            return null;

        int index;

        if (!int.TryParse(addressIndex, out index))
            return null;

        List<msEntityAddress> addresses = ConciergeAPI.CurrentEntity.Addresses;
        if (addresses == null || addresses.Count <= index)
            return null;

        // we also need to reset the radio button, again working around the ASP.NET bug regarding radiobuttons and repeaters
        RadioButton rb = (RadioButton)rptBillingAddress.Items[index].FindControl("rbAddress");
        rb.Checked = true;

        return addresses[index].Address;


    }

    #endregion

    protected void rptBillingAddress_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        msEntityAddress address = (msEntityAddress)e.Item.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;

            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
                goto case ListItemType.Item;

            case ListItemType.Item:
                RadioButton rbAddress = (RadioButton)e.Item.FindControl("rbAddress");

                Literal lAddress = (Literal)e.Item.FindControl("lAddress");

                if (address == null || address.Address == null)
                {
                    rbAddress.Visible = false;
                    return;
                }
                lAddress.Text = address.Address.ToHtmlString();

                rbAddress.Attributes["value"] = e.Item.ItemIndex.ToString();


                break;
        }
    }

    protected void cvInvalidAddress_OnServerValidate(object source, ServerValidateEventArgs args)
    {
        var a = GetBillingAddress();

        args.IsValid =
            a != null &&
            ! string.IsNullOrWhiteSpace(a.Line1) &&
            ! string.IsNullOrWhiteSpace(a.City) &&
            ! string.IsNullOrWhiteSpace(a.State) &&
            ! string.IsNullOrWhiteSpace(a.PostalCode);
    }
}