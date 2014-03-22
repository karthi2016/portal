using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.WCF;

public partial class financial_MakePayment : PortalPage
{
    #region Fields

    protected DataView dvInvoices;
    protected DataView dvCredits;
    protected msPayment targetPayment;
    protected msEntity targetEntity;

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

        targetEntity = string.IsNullOrWhiteSpace(ContextID) ? CurrentEntity : LoadObjectFromAPI<msEntity>(ContextID);

        if(targetEntity == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetPayment = CreateNewObject<msPayment>();
        targetPayment.Owner = targetEntity.ID;
        targetPayment.Date = DateTime.UtcNow;
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        btnContinue.Attributes.Add("onclick", "this.disabled=true;" + Page.ClientScript.GetPostBackEventReference(btnContinue, "").ToString());
        ViewState["AntiDupeKey"] = Guid.NewGuid().ToString();
    }

    #endregion

    #region Data Binding

    

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();

        Search sInvoices = new Search { Type = msInvoice.CLASS_NAME };
        sInvoices.AddOutputColumn("ID");
        sInvoices.AddOutputColumn("Name");
        sInvoices.AddOutputColumn("Date");
        sInvoices.AddOutputColumn("BalanceDue");
        sInvoices.AddCriteria(Expr.Equals("BillTo.ID", targetEntity.ID));
        sInvoices.AddCriteria(Expr.IsGreaterThan("BalanceDue", 0));
        searches.Add(sInvoices);

        Search sCredits = new Search { Type = msCredit.CLASS_NAME };
        sCredits.AddOutputColumn("ID");
        sCredits.AddOutputColumn("Name");
        sCredits.AddOutputColumn("Date");
        sCredits.AddOutputColumn("UseThrough");
        sCredits.AddOutputColumn("AmountAvailable");
        sCredits.AddCriteria(Expr.Equals("BillTo.ID", targetEntity.ID));
        sCredits.AddCriteria(Expr.IsGreaterThan("AmountAvailable", 0));
        searches.Add(sCredits);

        List<SearchResult> results = ExecuteSearches(searches, 0, null);
        dvInvoices = new DataView(results[0].Table);
        dvCredits = new DataView(results[1].Table);
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

        loadDataFromConcierge();

        if (!IsPostBack)
        {
            //Bind the invoices gridview to the data from the concierge
            gvInvoices.DataSource = dvInvoices;
            gvInvoices.DataBind();

            //Bind the credits gridview to the data from the concierge
            gvCredits.DataSource = dvCredits;
            gvCredits.DataBind();
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

        // let's pull all of the open invoices
        foreach (GridViewRow row in gvInvoices.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                CheckBox cbUse = (CheckBox)row.FindControl("cbUse");
                TextBox tbAmountToPay = (TextBox)row.FindControl("tbAmountToPay");

                if (!cbUse.Checked) continue;
                decimal amt = decimal.Parse(tbAmountToPay.Text); // validator should ensure this is valid
                string id = dvInvoices[row.DataItemIndex]["ID"].ToString();
                msPaymentLineItem li = new msPaymentLineItem { Amount = amt, Invoice = id, Type = PaymentLineItemType.Invoice };
                targetPayment.LineItems.Add(li);
            }
        }

        // now, let's pull any and all credits
        foreach (GridViewRow row in gvCredits.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                CheckBox cbUse = (CheckBox)row.FindControl("cbUse");
                TextBox tbAmountToUse = (TextBox)row.FindControl("tbAmountToUse");

                if (!cbUse.Checked) continue;
                decimal amt = decimal.Parse(tbAmountToUse.Text); // validator should ensure this is valid
                string id = dvCredits[row.DataItemIndex]["ID"].ToString();
                msPaymentLineItem li = new msPaymentLineItem { Amount = amt * -1, Credit = id, Type = PaymentLineItemType.Credit };
                targetPayment.LineItems.Add(li);
            }
        }
    }


    protected void gvInvoices_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView drv = (DataRowView) e.Row.DataItem;
            CheckBox cbUse = (CheckBox)e.Row.FindControl("cbUse");
            TextBox tbAmountToPay = (TextBox)e.Row.FindControl("tbAmountToPay");

            if (String.IsNullOrEmpty(tbAmountToPay.Text)) tbAmountToPay.Text = "0.00";

            cbUse.Attributes["onclick"] = string.Format("setTextBoxValue( this, '{0}', '{1:F2}');",
                                                        tbAmountToPay.ClientID, drv["BalanceDue"]);

            lTotalScript.Text += string.Format(" + parseFloat( document.getElementById('{0}').value) ",
                                               tbAmountToPay.ClientID);

            lStartupScript.Text +=
                string.Format(" document.getElementById('{0}').disabled = !document.getElementById('{1}').checked;",
                              tbAmountToPay.ClientID, cbUse.ClientID);

            tbAmountToPay.Attributes["onchange"] = "onTotalChange();";
           
            // is there a preset invoice?
            if (targetPayment.LineItems != null)
            {
                var existingItem = targetPayment.LineItems.Find(li => li.Invoice == drv["ID"].ToString());
                if (existingItem != null)
                {
                    cbUse.Checked = true;
                    tbAmountToPay.Text = existingItem.Amount.ToString("F2");

                }
            }

            tbAmountToPay.Enabled = cbUse.Checked;

        }
    }

    protected void gvCredits_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView drv = (DataRowView) e.Row.DataItem;
            CheckBox cbUse = (CheckBox)e.Row.FindControl("cbUse");
            TextBox tbAmountToUse = (TextBox)e.Row.FindControl("tbAmountToUse");

            if (String.IsNullOrEmpty(tbAmountToUse.Text)) tbAmountToUse.Text = "0.00";


            cbUse.Attributes["onclick"] = string.Format("setTextBoxValue( this, '{0}', '{1:F2}');",
                                                        tbAmountToUse.ClientID, drv["AmountAvailable"]);

            lTotalScript.Text += string.Format(" - parseFloat( document.getElementById('{0}').value) ",
                                               tbAmountToUse.ClientID);
            lStartupScript.Text +=
                string.Format(" document.getElementById('{0}').disabled = !document.getElementById('{1}').checked;",
                              tbAmountToUse.ClientID, cbUse.ClientID);

            tbAmountToUse.Attributes["onchange"] = "onTotalChange();";
            tbAmountToUse.Enabled = cbUse.Checked;
        }
    }

    #endregion
}