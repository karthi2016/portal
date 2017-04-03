using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class controls_BillingInfo : System.Web.UI.UserControl
{
    public PortalPage PortalPage
    {
        get { return (PortalPage)Page; }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            InitializeControl();

        acBillingAddress.Host = PortalPage;
    }

    public PortalPaymentMethods AllowableMethods { get; set; }
    public ElectronicPaymentManifest ExistingPaymentInfo { private get; set; }

    private void InitializeControl()
    {

        if (AllowableMethods == null)
            throw new ApplicationException("Allowable payment methods not set on this control");


        // let's bind the saved payments

        if (AllowableMethods.SavedPaymentMethods != null && AllowableMethods.SavedPaymentMethods.Count > 0)
        {
            rptSavedPaymentOptions.DataSource = AllowableMethods.SavedPaymentMethods;
            rptSavedPaymentOptions.DataBind();
            lNoOptions.Visible = false;
        }

        divNewCreditCard.Visible = AllowableMethods.AllowCreditCardPayments;
        divNewChecking.Visible = AllowableMethods.AllowEChecks;
        divPayrollDeduction.Visible = AllowableMethods.AllowPayrollDeduction;
        divBillMeLater.Visible = AllowableMethods.AllowBillMeLater;

        // can people save payment methods
        switch (AllowableMethods.DefaultSettingForSavingPaymentMethods)
        {
            case SavePaymentMethodSetting.Checked:
                cbSaveCheckingAccount.Checked = cbSaveCreditCard.Checked = true;
                break;

            case SavePaymentMethodSetting.Unchecked:
                cbSaveCheckingAccount.Checked = cbSaveCreditCard.Checked = false;
                break;

            case SavePaymentMethodSetting.Disabled:
                cbSaveCheckingAccount.Checked = cbSaveCreditCard.Checked = false;
                cbSaveCheckingAccount.Visible = cbSaveCreditCard.Visible = false;
                break;
        }


        if (AllowableMethods.Messages != null)
            foreach (var msg in AllowableMethods.Messages)
                lMessages.Text += msg + "<BR/>";

        // setup billing addresses
        var addresses = ConciergeAPI.CurrentEntity.Addresses;

        rptBillingAddress.DataSource = addresses;
        rptBillingAddress.DataBind();

        if (ExistingPaymentInfo != null)
            bindExistingPaymentInfo(ExistingPaymentInfo);

    }

    private void bindExistingPaymentInfo(ElectronicPaymentManifest existingPaymentInfo)
    {
        throw new NotImplementedException();
    }

    private string currentSavedPaymentMethodID;
    protected void rptSavedPaymentOptions_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        NameValueStringPair spm = (NameValueStringPair)e.Item.DataItem;

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
                RadioButton rbSavedPaymentMethod = (RadioButton)e.Item.FindControl("rbSavedPaymentMethod");
                HiddenField hfPaymentMethodID = (HiddenField)e.Item.FindControl("hfPaymentMethodID");

                rbSavedPaymentMethod.Attributes["value"] = spm.Value;
                rbSavedPaymentMethod.Text = spm.Name;
                hfPaymentMethodID.ID = spm.Value;

                rbSavedPaymentMethod.Checked = spm.Value == currentSavedPaymentMethodID;
                break;
        }
    }

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


    protected void cvBillingValidator_OnServerValidate(object source, ServerValidateEventArgs args)
    {
        var method = GetSelectedPaymentMethod();

        if (method == null)
        {
            args.IsValid = false;
            cvBillingValidator.ErrorMessage = "You have not selected a payment method.";
            return;
        }

        var isCreditCard = method.Value == OrderPaymentMethod.CreditCard;
        rfvCreditCardNumber.Enabled = isCreditCard;
        rfvNameOnCard.Enabled = isCreditCard;
        //MS-6920 Disabled CCV validation
        //rfvSecurityCode.Enabled = isCreditCard;

        var isECheck = method.Value == OrderPaymentMethod.ElectronicCheck;
        rfvRoutingNumber.Enabled = isECheck;
        revRoutingNumber.Enabled = isECheck;
        rfvBankAccountNumber.Enabled = isECheck;
        rfvBankAccountNumberConfirm.Enabled = isECheck;

        Address billingAddress = GetBillingAddress();

        if (method.Value == OrderPaymentMethod.CreditCard || method.Value == OrderPaymentMethod.PurchaseOrder)
        {
            if (billingAddress == null)
            {
                cvBillingValidator.ErrorMessage =
                    "A billing address is required. Please select an address or enter a new one.";
                args.IsValid = false;
                return;
            }

            var billingAddressIsValid = !string.IsNullOrWhiteSpace(billingAddress.Line1) && !string.IsNullOrWhiteSpace(billingAddress.City);

            if (!string.IsNullOrWhiteSpace(billingAddress.Country) &&
                (billingAddress.Country.Equals("CA", StringComparison.CurrentCultureIgnoreCase)
                || billingAddress.Country.Equals("US", StringComparison.CurrentCultureIgnoreCase)))
                billingAddressIsValid = billingAddressIsValid && !string.IsNullOrWhiteSpace(billingAddress.PostalCode);

            if (!billingAddressIsValid)
            {
                cvBillingValidator.ErrorMessage = "An incomplete billing address was detected. Please complete the address.";
                args.IsValid = false;
            }
        }
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

    private OrderPaymentMethod? GetSelectedPaymentMethod()
    {
        if (rbNewCard.Checked)
            return OrderPaymentMethod.CreditCard;

        if (rbNewChecking.Checked)
            return OrderPaymentMethod.ElectronicCheck;

        if (rbPayrollDeduction.Checked)
            return OrderPaymentMethod.PayrollDeduction;

        if (rbBillMeLater.Checked)
            return OrderPaymentMethod.None;


        // was one manually specified?
        // we have to do some tricks to get around the ASP.NET bug with radio buttons
        var savedPaymentID = getSavedPaymentID();

        if (savedPaymentID != null)   // let's set the button again, since viewstate won't kick in
            foreach (RepeaterItem ri in rptSavedPaymentOptions.Items)
            {
                RadioButton rbSavedPaymentMethod = (RadioButton)ri.FindControl("rbSavedPaymentMethod");
                if (rbSavedPaymentMethod != null && rbSavedPaymentMethod.Attributes["value"] == savedPaymentID)
                {
                    rbSavedPaymentMethod.Checked = true;
                    return OrderPaymentMethod.SavedPaymentMethod;
                }
            }

        return null;    // nothing could be found
    }

    protected virtual string getSavedPaymentID()
    {
        string savedPaymentID = Request[rbNewCard.NamingContainer.UniqueID + "$PaymentType"];
        return savedPaymentID;
    }

    public ElectronicPaymentManifest GetPaymentInfo()
    {

        var method = GetSelectedPaymentMethod();
        if (method == null)
            return null;

        switch (method.Value)
        {
            case OrderPaymentMethod.CreditCard:
                CreditCard cc = new CreditCard();
                cc.CardNumber = tbCardNumber.Text;
                cc.CCVCode = tbCCV.Text;
                cc.NameOnCard = tbNameOnCard.Text;
                if (myExpiration.Date.HasValue)
                    cc.CardExpirationDate = myExpiration.Date.Value;
                cc.SavePaymentMethod = cbSaveCreditCard.Checked;
                cc.BillingAddress = GetBillingAddress();
                return cc;

            case OrderPaymentMethod.ElectronicCheck:
                ElectronicCheck ec = new ElectronicCheck();
                ec.BankAccountNumber = tbBankAccountNumber.Text;
                ec.RoutingNumber = tbRoutingNumber.Text;
                ec.SavePaymentMethod = cbSaveCheckingAccount.Checked;
                return ec;

            case OrderPaymentMethod.SavedPaymentMethod:
                SavedPaymentInfo spi = new SavedPaymentInfo();
                spi.SavedPaymentMethodID = getSavedPaymentID();

                if (spi.SavedPaymentMethodID != null)
                    using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
                        spi.SavedPaymentMethodName = api.GetName(spi.SavedPaymentMethodID).ResultValue;
                return spi;

            default:
                NonElectronicPayment np = new NonElectronicPayment();
                np._OrderPaymentMethod = method.Value;
                return np;
        }
    }

    public string GetReferenceNumber()
    {
        return tbReferenceNumber.Text;
    }

    public void SetPaymentInfo(ElectronicPaymentManifest pm)
    {

        switch (pm.OrderPaymentMethod)
        {
            case OrderPaymentMethod.CreditCard:
                rbNewCard.Checked = true;
                CreditCard cc = pm as CreditCard;
                if (cc == null)
                    throw new ApplicationException("Payment method is credit card, but no card provided");

                tbCardNumber.Text = cc.CardNumber;
                tbCCV.Text = cc.CCVCode;
                tbNameOnCard.Text = cc.NameOnCard;
                myExpiration.Date = cc.CardExpirationDate;
                cbSaveCreditCard.Checked = cc.SavePaymentMethod;
                break;

            case OrderPaymentMethod.ElectronicCheck:
                rbNewChecking.Checked = true;
                ElectronicCheck ec = pm as ElectronicCheck;
                if (ec == null)
                    throw new ApplicationException("Payment method is electronic check, but no check provided");

                tbBankAccountNumber.Text = tbBankAccountNumberConfirm.Text = ec.BankAccountNumber;
                tbRoutingNumber.Text = ec.RoutingNumber;
                cbSaveCheckingAccount.Checked = ec.SavePaymentMethod;
                break;

            case OrderPaymentMethod.PayrollDeduction:
                rbPayrollDeduction.Checked = true;
                break;

            case OrderPaymentMethod.PurchaseOrder:
                rbBillMeLater.Checked = true;
                NonElectronicPayment nep = pm as NonElectronicPayment;
                if (nep == null)
                    throw new ApplicationException("Non electronic payment packet expected.");
                tbReferenceNumber.Text = nep.ReferenceNumber;
                break;

            case OrderPaymentMethod.SavedPaymentMethod:
                SavedPaymentInfo spi = pm as SavedPaymentInfo;
                if (spi == null)
                    throw new ApplicationException("Payment method is saved info, but no info provided");

                currentSavedPaymentMethodID = spi.SavedPaymentMethodID;
                break;
        }
    }

    public void SetBillingAddress(Address billingAddress)
    {
        rbNewBillingAddress.Checked = true;
        acBillingAddress.Address = billingAddress;
    }
}