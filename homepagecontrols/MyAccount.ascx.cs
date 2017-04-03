using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;

public partial class homepagecontrols_MyAccount : HomePageUserControl
{
     
    public override List<string> GetFieldsNeededForMainSearch()
    {
        var fields = base.GetFieldsNeededForMainSearch();
        fields.Add("Invoices_OpenInvoiceBalance");
        fields.Add("CreditBalance");
        fields.Add("Payments_LastPayment.Date");
        fields.Add("Payments_LastPayment.Total");

        return fields;
    }


    private void populateMyAccount()
    {
        if (drMainRecord["Payments_LastPayment.Date"] == DBNull.Value) return;

        lblLastPayment.Text = string.Format("{0:d} for {1:C}",
             drMainRecord["Payments_LastPayment.Date"], drMainRecord["Payments_LastPayment.Total"]);

        lblCreditBalance.Text = string.Format("{0:C}", drMainRecord["CreditBalance"]);
    }

    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);

        //Now initialize - this cannot happen in initialize widget because it depends on search fields existing and they won't if the module is inactive and GetFieldsNeededForMainSearch is not run
        populateMyAccount();

        decimal openBalance = (decimal)drMainRecord["Invoices_OpenInvoiceBalance"];
        lblOutstandingBalance.Text = openBalance.ToString("C");
        if (openBalance > 0)
            hlMakeAPayment.Visible = true;

        
    }
}