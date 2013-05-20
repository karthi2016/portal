using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;
using MemberSuite.SDK.WCF;

public partial class donations_MakeDonation : PortalPage
{
    #region Fields

    protected DataView dvProducts;
    protected msOrder targetOrder;
    protected msOrder cleanOrder;
    protected msIndividual targetIndividual;
    protected CreditCard targetCreditCard;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this page is public, meaning you don't
    /// have to be logged in to access it.
    /// </summary>
    /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

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

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            searchForProducts(proxy);
        }

        targetOrder = new msOrder();

        if (ConciergeAPI.CurrentEntity != null)
            targetIndividual = ConciergeAPI.CurrentEntity.ConvertTo<msIndividual>();
  
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

        if(targetIndividual != null)
            bindObjectToPage();

        // set the anti-dupe key
        ViewState["AntiDupeKey"] = Guid.NewGuid().ToString();
    }

    protected void searchForProducts(IConciergeAPIService serviceProxy)
    {
        Search sDonationProducts = new Search{Type=msProduct.CLASS_NAME};
        sDonationProducts.AddOutputColumn("ID");
        sDonationProducts.AddOutputColumn("Name");
        sDonationProducts.AddOutputColumn("Price");
        sDonationProducts.AddCriteria(Expr.Equals("ProductType", "Fundraising"));
        sDonationProducts.AddCriteria(Expr.Equals("SellOnline",true));

        SearchResult srDonationProducts = ExecuteSearch(serviceProxy, sDonationProducts, 0, null);
        srDonationProducts.Table.PrimaryKey = new[]{srDonationProducts.Table.Columns["ID"]};
        dvProducts = new DataView(srDonationProducts.Table);
    }

    #endregion

    #region Data Binding

    protected void bindObjectToPage()
    {
        if (targetIndividual == null)
            return;

        tbFirstName.Text = targetIndividual.FirstName;
        tbLastName.Text = targetIndividual.LastName;
        tbEmailAddress.Text = targetIndividual.EmailAddress;
        acBillingAddress.Address = getBillingAddress();
        
    }

    protected void unbindIndividual(msIndividual individual)
    {
        individual.FirstName = tbFirstName.Text;
        individual.LastName = tbLastName.Text;
        individual.EmailAddress = tbEmailAddress.Text;
    }

    protected void bindAmount(string productID)
    {
        DataRow drSelectedProduct = dvProducts.Table.Rows.Find(productID);
        decimal amount = (decimal)drSelectedProduct["Price"];

        rbAmount25.Checked = amount == 25m;
        rbAmount100.Checked = amount == 100m;
        rbAmount500.Checked = amount == 500m;
        rbAmountOther.Checked = !(rbAmount25.Checked || rbAmount100.Checked || rbAmount500.Checked);

        tbAmountOther.Text = rbAmountOther.Checked ? string.Format("{0:0.00}",amount) : null;
    }

    protected void unbindOrder()
    {
        var lineItem = new msOrderLineItem
                           {
                               Product = rblProducts.SelectedValue,
                               Quantity = 1,
                               OrderLineItemID = Guid.NewGuid().ToString(),
                               PriceOverride = true
                           };

        targetOrder.Total = lineItem.Total = unbindAmountToDonate();
        targetOrder.BillTo = targetOrder.ShipTo = targetIndividual.ID;
        targetOrder.BillingEmailAddress = tbEmailAddress.Text;
        targetOrder.BillingAddress = targetOrder.ShippingAddress = acBillingAddress.Address;
        targetOrder.LineItems = new List<msOrderLineItem>
                                    {
                                        lineItem
                                    };

        
    }

    protected decimal unbindAmountToDonate()
    {
        if (rbAmount25.Checked)
            return 25m;

        if (rbAmount100.Checked)
            return 100m;

        if (rbAmount500.Checked)
            return 500m;

        decimal result = decimal.Parse(tbAmountOther.Text);
        return result;
    }


    protected void unbindCreditCard()
    {
        CreditCard result = new CreditCard
        {
            CardNumber = tbCreditCardNumber.Text,
            CardExpirationDate = myExpiration.Date.Value,
            CCVCode = tbCVV.Text,
            NameOnCard = tbName.Text,
            BillingAddress = acBillingAddress.Address
        };

        targetCreditCard = result;
    }

    #endregion

    #region Methods

    protected void findOrCreateIndividual(IConciergeAPIService serviceProxy)
    {
        //First try to locate an individual by e-mail
        Search sIndividualByEmail = new Search {Type = msIndividual.CLASS_NAME};
        sIndividualByEmail.AddOutputColumn("ID");

        SearchOperationGroup emailFilter = new SearchOperationGroup();
        emailFilter.Criteria.Add(Expr.Equals("EmailAddress", tbEmailAddress.Text));
        emailFilter.Criteria.Add(Expr.Equals("EmailAddress2", tbEmailAddress.Text));
        emailFilter.Criteria.Add(Expr.Equals("EmailAddress3", tbEmailAddress.Text));
        emailFilter.GroupType = SearchOperationGroupType.Or;

        sIndividualByEmail.AddCriteria(emailFilter);

        //Search for individuals by email
        SearchResult srIndividualByEmail = ExecuteSearch(serviceProxy, sIndividualByEmail, 0, null);
        if (srIndividualByEmail.Table != null && srIndividualByEmail.Table.Rows.Count == 1)
        {
            DataRow drIndividual = srIndividualByEmail.Table.Rows[0];
            targetIndividual =
                LoadObjectFromAPI<msIndividual>(serviceProxy, drIndividual["ID"].ToString());
        }
        else
        {
            //Could not locate an individual by e-mail or had multiple hits - need to create a new individual
            targetIndividual = CreateNewObject<msIndividual>();
            unbindIndividual(targetIndividual);
            var saveResult = serviceProxy.Save(targetIndividual);
            targetIndividual = saveResult.ResultValue.ConvertTo<msIndividual>();
        }
    }

    protected msAuditLog processOrder(IConciergeAPIService api)
    {
        var processedOrderPacket = api.PreProcessOrder(targetOrder).ResultValue;
        cleanOrder = processedOrderPacket.FinalizedOrder.ConvertTo<msOrder>();

        //Reset the total because a donation can be any amount - the product price returned from preprocessing is just the "suggested donation"
        cleanOrder.LineItems[0].Total = targetOrder.Total;

        // now, let's set the total - in case the product has added additional stuff like shipping/taxes
        cleanOrder.Total = cleanOrder.LineItems.Sum(x => x.Total);

        cleanOrder.PaymentMethod = OrderPaymentMethod.CreditCard;
        cleanOrder.CreditCardNumber = targetCreditCard.CardNumber;
        cleanOrder.CreditCardExpirationDate = targetCreditCard.CardExpirationDate;
        cleanOrder.CCVCode = targetCreditCard.CCVCode;


        cleanOrder.BillingAddress = acBillingAddress.Address;
        cleanOrder.BillingEmailAddress = tbEmailAddress.Text;

        string antiDupeKey = (string) ViewState["AntiDupeKey"];
        var trackingKey = api.ProcessOrder(cleanOrder, antiDupeKey ).ResultValue ;

        // let's wait
        return OrderUtilities.WaitForOrderToComplete(api, trackingKey);


    }

    protected Address getBillingAddress()
    {
        if (targetIndividual == null)
            return null;
        
        msEntityAddress entityAddress =
            targetIndividual.Addresses.Where(x => x.Type == targetIndividual.PreferredAddressType).
                FirstOrDefault();
        if (entityAddress == null)
            return null;

        return entityAddress.Address;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        if (!IsPostBack)
        {
            //Bind the fundraising products
            if (dvProducts != null && dvProducts.Table != null && dvProducts.Table.Rows.Count > 0)
            {
                lblNoFunds.Visible = false;
                rblProducts.DataSource = dvProducts;
                rblProducts.DataBind();
            }
            else
                btnContinue.Enabled = false;
        }
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        if(targetIndividual == null)
            using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                findOrCreateIndividual(proxy);
            }

        if(targetIndividual == null)
        {
            QueueBannerError("Unable to find/create individual");
            Refresh();
            return;
        }

        //ONLY if the individual does not have a preferred address set and a Home_Address type is defined then set the Home_Address to be the billing address supplied (MS-1083)
        if (targetIndividual.PreferredAddressType == null && targetIndividual.Fields.ContainsKey("Home_Address"))
        {
            targetIndividual["Home_Address"] = acBillingAddress.Address;
            targetIndividual = SaveObject(targetIndividual).ConvertTo<msIndividual>();
        }

        unbindOrder();
        unbindCreditCard();

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            var log = processOrder(proxy);
            
            if (log == null )
                GoTo("/orders/OrderQueued.aspx");

            if (log.Type == AuditLogType.OrderFailure)
                throw new ConciergeClientException(ConciergeErrorCode.IllegalOperation, log.Description);

            QueueBannerMessage(string.Format("Your donation was processed successfully.",
                                             targetOrder.LocalID));
            GoTo(string.Format("~/donations/DonationComplete.aspx?contextID={0}", log.AffectedRecord_ID ));
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void rblProducts_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindAmount(rblProducts.SelectedValue);
    }

    #endregion
}