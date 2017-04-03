using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class financial_MakePayment : PortalPage
{
    protected DataView dvInvoices;
    protected DataView dvCredits;
    protected DataView dvOptionalItems;
    protected msPayment targetPayment;
    protected msEntity targetEntity;    

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

        targetEntity = string.IsNullOrWhiteSpace(ContextID) ? CurrentEntity : LoadObjectFromAPI<msEntity>(ContextID);

        if (targetEntity == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetPayment = CreateNewObject<msPayment>();
        targetPayment.Owner = targetEntity.ID;
        targetPayment.Date = DateTime.UtcNow;
        // We can't trust defult cash account set by describer.
        targetPayment.CashAccount = string.Empty;
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        btnContinue.Attributes.Add("onclick", "this.disabled=true;" + Page.ClientScript.GetPostBackEventReference(btnContinue, "").ToString());
        ViewState["AntiDupeKey"] = Guid.NewGuid().ToString();

        if (CurrentEntity.ID != targetEntity.ID)
            PageTitleExtension.Text = string.Format(" for {0}", targetEntity.Name);
    }

    protected void loadDataFromConcierge()
    {
        var searches = new List<Search>();

        var sInvoices = new Search { Type = msInvoice.CLASS_NAME };
        sInvoices.AddOutputColumn("ID");
        sInvoices.AddOutputColumn("Name");
        sInvoices.AddOutputColumn("Date");
        sInvoices.AddOutputColumn("BalanceDue");
        sInvoices.AddOutputColumn("BusinessUnit.DoNotAllowPartialPaymentsInPortal");
        sInvoices.AddCriteria(Expr.Equals("BillTo.ID", targetEntity.ID));
        sInvoices.AddCriteria(Expr.IsGreaterThan("BalanceDue", 0));
        sInvoices.AddSortColumn("LocalID");
        searches.Add(sInvoices);

        var sCredits = new Search { Type = msCredit.CLASS_NAME };
        sCredits.AddOutputColumn("ID");
        sCredits.AddOutputColumn("Name");
        sCredits.AddOutputColumn("Date");
        sCredits.AddOutputColumn("AmountAvailable");
        sCredits.AddCriteria(Expr.Equals(msCredit.FIELDS.Owner, targetEntity.ID));
        sCredits.AddCriteria(Expr.IsGreaterThan("AmountAvailable", 0));
        sCredits.AddSortColumn("LocalID");
        searches.Add(sCredits);

        var sInvoiceItems = new Search { Type = msInvoiceLineItem.CLASS_NAME };
        sInvoiceItems.AddOutputColumn("Invoice");
        sInvoiceItems.AddOutputColumn("Total");
        sInvoiceItems.AddOutputColumn("Description");
        sInvoiceItems.AddOutputColumn("Product.Name");
        
        sInvoiceItems.AddCriteria(Expr.Equals("Invoice.BillTo.ID", targetEntity.ID));
        sInvoiceItems.AddCriteria(Expr.IsGreaterThan("Invoice.BalanceDue", 0));
        sInvoiceItems.AddCriteria(Expr.Equals("Optional", true));
        sInvoiceItems.AddSortColumn("Invoice");
        sInvoiceItems.AddSortColumn("InvoiceLineItemID");
        searches.Add(sInvoiceItems);

        var results = APIExtensions.GetMultipleSearchResults(searches, 0, null);
        dvInvoices = new DataView(results[0].Table);
        dvCredits = new DataView(results[1].Table);
        dvOptionalItems = new DataView(results[2].Table);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        loadDataFromConcierge();

        if (!IsPostBack)
        {
            //Bind the invoices gridview to the data from the concierge
            rptInvoices.DataSource = dvInvoices;
            rptInvoices.DataBind();

            // Until fully fixed, continue to hide credits
            secCredits.Visible = false;

            //Bind the credits gridview to the data from the concierge
            if (dvCredits.Count > 0 && secCredits.Visible)
            {
                gvCredits.DataSource = dvCredits;
                gvCredits.DataBind();
            }
            else
            {
                secCredits.Visible = false;
            }
        }
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        unbindPayment();

        MultiStepWizards.MakePayment.Payment = targetPayment;
        GoTo("MakePayment2.aspx?antiDupeKey=" + ViewState["AntiDupeKey"]);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MultiStepWizards.MakePayment.Clear();
        GoHome();
    }

    protected void unbindPayment()
    {
        targetPayment.Total = decimal.Parse(tbAmount.Text);
        targetPayment.LineItems = new List<msPaymentLineItem>();

        // Create a list of the possible Credit Usages
        var creditUsages = new List<msCreditUsage>();
        foreach (GridViewRow row in gvCredits.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                var cbUse = (CheckBox)row.FindControl("cbUse");
                var tbAmountToUse = (TextBox)row.FindControl("tbAmountToUse");

                if (!cbUse.Checked) continue;
                var amt = decimal.Parse(tbAmountToUse.Text); // validator should ensure this is valid
                var id = dvCredits[row.DataItemIndex]["ID"].ToString();

                var creditUsage = new msCreditUsage
                {
                    Credit = id,
                    Total = amt,
                    LineItems = new List<msCreditUsageLineItem>()
                };
                creditUsage.Fields["Credit.Name"] = ((HyperLink)row.Cells[1].Controls[0]).Text;

                creditUsages.Add(creditUsage);
            }
        }
        
        // pointer to the "current" Credit Usage and enumerator to iterate through list
        var creditEnumerator = creditUsages.GetEnumerator();
        var haveCredit = creditEnumerator.MoveNext();

        // let's pull all of the open invoices
        foreach (RepeaterItem row in rptInvoices.Items)
        {
            if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
            {
                var cbUse = (CheckBox) row.FindControl("cbUse");
                var tbAmountToPay = (TextBox) row.FindControl("tbAmountToPay");

                if (!cbUse.Checked) continue;
                decimal amt; // validator should ensure this is valid

                // hidden balance is set if partial payments is disabled
                var hiddenBalance = (HiddenField)row.FindControl("hiddenBalance");
                if (string.IsNullOrWhiteSpace(hiddenBalance.Value) )
                    amt = decimal.Parse(tbAmountToPay.Text);
                else
                {
                    
                    amt = decimal.Parse(hiddenBalance.Value);
                }

                decimal optAmt;
                var tbOptionalAmount = (TextBox) row.FindControl("tbOptionalAmount");
                if (decimal.TryParse(tbOptionalAmount.Text, out optAmt))
                {
                    amt += optAmt;
                }

                string id = dvInvoices[row.ItemIndex]["ID"].ToString();

                // If we have Credit Usages, process those first
                while (amt > 0 && haveCredit && creditEnumerator.Current != null)
                {
                    var creditRemaining = creditEnumerator.Current.Total -
                                          creditEnumerator.Current.LineItems.Sum(ci => ci.Total);

                    decimal creditAmt;
                    if (creditRemaining >= amt)
                    {
                        creditAmt = amt;
                        amt = 0;
                    }
                    else
                    {
                        creditAmt = creditRemaining;
                        amt -= creditRemaining;
                    }

                    creditEnumerator.Current.LineItems.Add(new msCreditUsageLineItem {Total = creditAmt, Invoice = id});

                    // If this Credit Usage is complete, move to the next one
                    if (creditEnumerator.Current.LineItems.Sum(ci => ci.Total) == creditEnumerator.Current.Total)
                    {
                        haveCredit = creditEnumerator.MoveNext();
                    }
                }

                if (amt > 0)
                {
                    var li = new msPaymentLineItem {Total = amt, Invoice = id, Type = PaymentLineItemType.Invoice};
                    targetPayment.LineItems.Add(li);
                }
            }
        }

        foreach (var creditUsage in creditUsages)
        {
            using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                var result = proxy.Save(creditUsage);
                if (!result.Success)
                {
                    // Queue error and completely refresh page to get updates on Credit and Invoice Totals
                    QueueBannerError(string.Format(
                        "Unable to process {0}: {1}", 
                        creditUsage.SafeGetValue<string>("Credit.Name"), 
                        result.FirstErrorMessage));
                    Refresh();
                }
            }
        }
    }

    protected void rptInvoices_OnRowDataBound(object sender, RepeaterItemEventArgs e)
    {
        switch (e.Item.ItemType)
        {
            case ListItemType.Item:
            case ListItemType.AlternatingItem:
                var drv = (DataRowView) e.Item.DataItem;
                var cbUse = (CheckBox) e.Item.FindControl("cbUse");
                var tbAmountToPay = (TextBox) e.Item.FindControl("tbAmountToPay");

                var invoiceID = Convert.ToString(drv["ID"]);

                var hlInvoice = (HyperLink) e.Item.FindControl("hlInvoice");
                var lDate = (Literal) e.Item.FindControl("lDate");
                var lBalanceDue = (Literal) e.Item.FindControl("lBalanceDue");
                var trOptional = e.Item.FindControl("trOptional");
                var hiddenBalance = (HiddenField) e.Item.FindControl("hiddenBalance");


                bool dontAllowPartialPayments = Convert.ToBoolean(drv["BusinessUnit.DoNotAllowPartialPaymentsInPortal"]);

                decimal optionalTotal = 0;
                var optionalLabel = new List<string>();
                foreach (DataRow dr in dvOptionalItems.Table.Select(string.Format("Invoice = '{0}'", invoiceID)))
                {
                    optionalTotal += (decimal) dr["Total"];

                    var label = Convert.ToString(dr["Description"]);
                    if (string.IsNullOrEmpty(label))
                    {
                        label = Convert.ToString(dr["Product.Name"]);
                    }

                    optionalLabel.Add(label);
                }

                var tbOptionalAmount = (TextBox)e.Item.FindControl("tbOptionalAmount");
                if (optionalTotal > 0)
                {
                    var lOptLabel = (Label)e.Item.FindControl("lOptLabel");
                    lOptLabel.Text = lOptionalLabel.Text;

                    var lOptionalDescr = (Literal)e.Item.FindControl("lOptionalDescr");
                    lOptionalDescr.Text = string.Join(", ", optionalLabel);

                    tbOptionalAmount.Text = optionalTotal.ToString("F2");
                    tbOptionalAmount.Attributes["onchange"] = "onTotalChange(this);";

                    // Include the Optional Amount in the "Total" calculation
                    lOptionalScript.Text += string.Format(
                        "if ($('#{0}').is(':visible')) result += parseFloat(document.getElementById('{0}').value);{1}",
                        tbOptionalAmount.ClientID,
                        Environment.NewLine);

                    // At least one optional item, activate message
                    trOptionalMessage.Visible = true;
                }

                hlInvoice.Text = Convert.ToString(drv["Name"]);
                hlInvoice.NavigateUrl = string.Format(@"~\financial\ViewInvoice.aspx?contextID={0}", invoiceID);
                lDate.Text = string.Format("{0:d}", drv["Date"]);
                lBalanceDue.Text = string.Format("{0:C}", drv["BalanceDue"]);

                if ( dontAllowPartialPayments )
                    hiddenBalance.Value = drv["BalanceDue"].ToString();

                if (string.IsNullOrEmpty(tbAmountToPay.Text)) tbAmountToPay.Text = "0.00";

                cbUse.Attributes["onclick"] = string.Format(
                    "setTextBoxValue( this, '{0}', '{1:F2}', '{2}','{3}');",
                    tbAmountToPay.ClientID,
                    (decimal)drv["BalanceDue"] - optionalTotal,
                    optionalTotal > 0 ? trOptional.ClientID : string.Empty, dontAllowPartialPayments);

                lTotalScript.Text += string.Format(" + parseFloat( document.getElementById('{0}').value) ",
                    tbAmountToPay.ClientID);


                // MSIV-330
                if ( ! dontAllowPartialPayments)
                    lStartupScript.Text +=
                        string.Format(" document.getElementById('{0}').disabled = !document.getElementById('{1}').checked;",
                            tbAmountToPay.ClientID, cbUse.ClientID);
                else
                    lStartupScript.Text +=
                         string.Format(" document.getElementById('{0}').disabled = true;",
                            tbAmountToPay.ClientID);

                tbAmountToPay.Attributes["onchange"] = "onTotalChange(this);";

                // is there a preset invoice?
                if (targetPayment.LineItems != null)
                {
                    var existingItem = targetPayment.LineItems.Find(li => li.Invoice == invoiceID);
                    if (existingItem != null)
                    {
                        cbUse.Checked = true;
                        tbAmountToPay.Text = existingItem.Total.ToString("F2");
                    }
                }

                tbAmountToPay.Enabled = cbUse.Checked;
                break;
            case ListItemType.Footer:
                if (dvInvoices.Count > 0)
                    e.Item.Visible = false;
                break;
        }
    }
    
    protected void gvCredits_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var drv = (DataRowView)e.Row.DataItem;
            var cbUse = (CheckBox)e.Row.FindControl("cbUse");
            var tbAmountToUse = (TextBox)e.Row.FindControl("tbAmountToUse");

            if (String.IsNullOrEmpty(tbAmountToUse.Text)) tbAmountToUse.Text = "0.00";


            cbUse.Attributes["onclick"] = string.Format("setTextBoxValue( this, '{0}', '{1:F2}');",
                                                        tbAmountToUse.ClientID, drv["AmountAvailable"]);

            lTotalScript.Text += string.Format(" - parseFloat( document.getElementById('{0}').value) ",
                                               tbAmountToUse.ClientID);
            lStartupScript.Text +=
                string.Format(" document.getElementById('{0}').disabled = !document.getElementById('{1}').checked;",
                              tbAmountToUse.ClientID, cbUse.ClientID);

            tbAmountToUse.Attributes["onchange"] = "onTotalChange(this);";
            tbAmountToUse.Enabled = cbUse.Checked;
        }
    }
}