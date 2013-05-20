using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;

public partial class continuingeducation_ReportCredit : PortalPage 
{
    protected msCEUCredit targetCredit;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        if (ContextID != null)
            targetCredit = LoadObjectFromAPI<msCEUCredit>(ContextID);
    }


    protected override void InitializePage()
    {
        base.InitializePage();

        // load the types
        Search s = new Search(msCEUType.CLASS_NAME);
        s.AddOutputColumn("Name");

        foreach (DataRow dr in ExecuteSearch(s, 0, null).Table.Rows)
            ddlType.Items.Add(new ListItem(dr["Name"].ToString(),
                                           dr["ID"].ToString()));

        if (targetCredit != null)
        {
            ddlType.ClearSelection();
            ListItem li = ddlType.Items.FindByValue(targetCredit.Type);
            if (li == null)
            {
                li = new ListItem(targetCredit.Type);
                ddlType.Items.Add(li);
            }

            li.Selected = true;

            dpDate.SelectedDate = targetCredit.CreditDate;
            tbQuantity.Text = targetCredit.Quantity.ToString();
            tbNotes.Text = targetCredit.Notes;
        }
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        // credit must be self reported AND not already verified
        if (targetCredit != null
            && (targetCredit.Verified || !targetCredit.SelfReported))
            return false;

        return true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        if (targetCredit == null)
            targetCredit = new msCEUCredit();

        targetCredit.Type = ddlType.SelectedValue;
        targetCredit.Quantity = decimal.Parse(tbQuantity.Text);
        targetCredit.Notes = tbNotes.Text;
        targetCredit.CreditDate = dpDate.SelectedDate.Value ;
        targetCredit.Verified = false;
        targetCredit.SelfReported = true;
        targetCredit.Owner = ConciergeAPI.CurrentEntity.ID;

        // save the credit
        SaveObject(targetCredit);

        GoTo("ViewCEUCreditHistory.aspx", "Your CEU credits were recorded successfully.");

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewCEUCreditHistory.aspx");
    }
}