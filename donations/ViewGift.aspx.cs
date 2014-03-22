using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using Telerik.Web.UI;

public partial class donations_ViewGift : PortalPage
{

    protected msGift targetGift { get; set; }
    protected DataRow dr;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetGift = LoadObjectFromAPI<msGift>(ContextID);

        if (targetGift == null)
            GoToMissingRecordPage();
    }

    protected override bool CheckSecurity()
    {
        return targetGift.Donor == ConciergeAPI.CurrentEntity.ID;
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        Search s = new Search(msGift.CLASS_NAME);
        s.AddCriteria(Expr.Equals("ID", targetGift.ID));

        s.AddOutputColumn("Type");
        s.AddOutputColumn("MasterGift.Name");
        s.AddOutputColumn("MasterGift.ID");

        s.AddOutputColumn("Date");
        s.AddOutputColumn("Amount");
        s.AddOutputColumn("PaymentMethod");
        s.AddOutputColumn("Fund.Name");
        s.AddOutputColumn("SavedPaymentMethod.Name");


        s.AddOutputColumn("NextTransactionDue");
        s.AddOutputColumn("NextTransactionAmount");
        s.AddOutputColumn("BalanceDue");
        s.AddOutputColumn("DateOfInstallmentSuspension");

        dr = ExecuteSearch(s, 0, 1).Table.Rows[0];

        pnlRecurring.Visible = dr["NextTransactionDue"] != DBNull.Value;
        pnlInstallmentHistory.Visible = targetGift.Installments != null && targetGift.Installments.Count > 0;

        // update status
        if (targetGift.DateOfInstallmentSuspension == null)
        {
            lblStatus.Text = "Active";
            lblStatus.ForeColor = Color.Green;
        }
        else
        {
            lblStatus.Text = "Suspended";
            lblStatus.ForeColor = Color.Red;
            if (targetGift.LastInstallmentError != null)
                lblStatus.Text += " - Reason: " + targetGift.LastInstallmentError;
        }

        if (dr["MasterGift.ID"] != DBNull.Value)
        {
            phMasterGift.Visible = true;
            lblNoMasterGift.Visible = false;
        }

        hlUpdatePaymentMethod.Visible = pnlRecurring.Visible;

        hlUpdatePaymentMethod2.NavigateUrl = hlUpdatePaymentMethod.NavigateUrl = string.Format("/orders/updatebillinginformation.aspx?contextID={0}&returnURL={1}",
                                                           ContextID, HttpUtility.UrlEncode( Request.Url.ToString()));
    }

    protected void rgInstallments_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        // sort in ascending order
        // MS-4858
        targetGift.Installments.Sort((x, y) => x.Date.CompareTo(y.Date));

        rgInstallments.DataSource = targetGift.Installments;

    }
}