using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class financial_RequestRefund : PortalPage
{
    #region Fields

    protected DataView dvCredits;
    protected msRefund targetRefund;
    protected msEntity targetEntity;
    protected decimal totalAvailable;

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

        targetRefund = CreateNewObject<msRefund>();
        targetRefund.RefundTo = targetEntity.ID;
        targetRefund.Date = DateTime.UtcNow;
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        if (CurrentEntity.ID != targetEntity.ID)
            PageTitleExtension.Text = string.Format("for {0}", targetEntity.Name);
    }

    #endregion

    #region Data Binding

    protected void unbindRefund()
    {
          targetRefund.LineItems = new List<msRefundLineItem>();
        
        // Pull the selected credits
        foreach (GridViewRow row in gvCredits.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                CheckBox cbUse = (CheckBox) row.FindControl("cbUse");
                TextBox tbAmountToUse = (TextBox) row.FindControl("tbAmountToUse");

                if (!cbUse.Checked) continue;
                decimal amt = decimal.Parse(tbAmountToUse.Text); // validator should ensure this is valid
                string id = dvCredits[row.DataItemIndex]["ID"].ToString();
                msRefundLineItem li = new msRefundLineItem
                                           {Amount = amt, Credit = id };
                targetRefund.LineItems.Add(li);
            }
        }

        targetRefund.Total = targetRefund.LineItems.Sum(x => x.Amount);
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search sCredits = new Search {Type = msCredit.CLASS_NAME};
        sCredits.AddOutputColumn("ID");
        sCredits.AddOutputColumn("Name");
        sCredits.AddOutputColumn("Date");
        sCredits.AddOutputColumn("UseThrough");
        sCredits.AddOutputColumn("AmountAvailable");
        sCredits.AddCriteria(Expr.Equals("BillTo.ID", targetEntity.ID));
        sCredits.AddCriteria(Expr.IsGreaterThan("AmountAvailable", 0));

        SearchResult srCredits = APIExtensions.GetSearchResult(sCredits, 0, null);
        dvCredits = new DataView(srCredits.Table);

        object objAvailable = srCredits.Table.Compute("SUM(AmountAvailable)", null);

        totalAvailable = objAvailable != System.DBNull.Value ? (decimal) objAvailable : 0M;
    }

    protected void processRefund()
    {
        using (var api = GetServiceAPIProxy())
        {
            targetRefund = api.ProcessRefund(targetRefund).ResultValue.ConvertTo<msRefund>();

            // now, send a confirmation email
            api.SendEmail(EmailTemplates.Financial.RefundRequest, new List<string> { targetRefund.ID }, ConciergeAPI.CurrentUser.EmailAddress);
        }
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
            //Bind the credits gridview to the data from the concierge
            if (dvCredits.Table.Rows.Count > 0)
            {
                gvCredits.DataSource = dvCredits;
                gvCredits.DataBind();
            }
            else
            {
                btnContinue.Visible = false;    // don't show this - you can't move forward
            }
        }
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid )
            return;

        unbindRefund();

        //If this happens someone has tried to bypass our validators.  Return with no information.
        if(targetRefund.Total > totalAvailable)
            return;

        processRefund();

        QueueBannerMessage(string.Format("Your refund request for {0:C} has been processed.", targetRefund.Total));

        GoHome();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
       GoHome();
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

            lTotalScript.Text += string.Format(" + parseFloat( document.getElementById('{0}').value) ",
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