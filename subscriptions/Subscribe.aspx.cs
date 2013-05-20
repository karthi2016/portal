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

public partial class subscriptions_Subscribe : PortalPage
{
    protected override bool IsPublic
    {
        get { return true; }
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        if (ConciergeAPI.CurrentEntity == null) // we need them to go through registration
        {
            GoTo("~/profile/CreateAccount_BasicInfo.aspx?t=Subscription");
            return;
        }

        populateSubscriptionPlans();
    }

    private void populateSubscriptionPlans()
    {
        Search s = new Search(msSubscriptionFee.CLASS_NAME);
        s.AddCriteria(Expr.Equals("IsActive", true));
        s.AddCriteria(Expr.Equals("SellOnline", true));

        var dtProducts = ExecuteSearch(s, 0, null).Table;

        List<string> productIDs = new List<string>();
        foreach (DataRow dr in dtProducts.Rows)
            productIDs.Add(Convert.ToString(dr["ID"]));

        List<ProductInfo> describedProducts;
        using (var api = GetServiceAPIProxy())
        {
            describedProducts = api.DescribeProducts(ConciergeAPI.CurrentEntity.ID, productIDs).ResultValue;
        }

        describedProducts.Sort((x, y) => string.Compare(x.ProductName, y.ProductName));

        if (describedProducts.Count > 0)
        {
            foreach (var p in describedProducts)
            {
                string text = string.Format("{0} - <font color=green>{1}</font>", p.ProductName,
                                            p.DisplayPriceAs ?? p.Price.ToString("C"));
                rbSubscriptionPlans.Items.Add(new ListItem(text, p.ProductID));
            }

            rbSubscriptionPlans.SelectedIndex = 0;
            lNoSubscriptionFees.Visible = false;
            rbSubscriptionPlans.Visible = true;
        }
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        string selectedValue = rbSubscriptionPlans.SelectedValue;
        if ( string.IsNullOrWhiteSpace( selectedValue )) return;

        // ok, let's create our order
        msOrder o = new msOrder();
        o.ShipTo = o.BillTo = ConciergeAPI.CurrentEntity.ID;
        o.LineItems = new List<msOrderLineItem>();

        // add the primary booth

        var oli = new msOrderLineItem { Product = selectedValue , Quantity = 1 };
        o.LineItems.Add(oli);

        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(o);

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }
}