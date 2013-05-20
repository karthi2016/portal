using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class exhibits_ViewExhibitor : PortalPage 
{
    public msExhibitor targetExhibitor;
    public msExhibitShow targetShow;
    public msEntity targetEntity;
    public string assignedBooths = "No booths have been assigned.";
    public string preferredBooths = "No booth preferences set.";
    public string boothTypes = "No booth types set.";

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetExhibitor = LoadObjectFromAPI<msExhibitor>(ContextID);
        if (targetExhibitor == null) GoToMissingRecordPage();

        targetShow = LoadObjectFromAPI<msExhibitShow>( targetExhibitor.Show );
        if (targetShow == null) GoToMissingRecordPage();

        targetEntity = LoadObjectFromAPI<msEntity>(targetExhibitor.Customer);
        if (targetEntity == null) GoToMissingRecordPage();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        using (var api = GetConciegeAPIProxy())
            return ExhibitorLogic.CanViewExhibitorRecord(api, targetExhibitor, ConciergeAPI.CurrentEntity.ID);
    }



    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetExhibitor;

        var pageLayout = GetAppropriatePageLayout(targetExhibitor);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(msExhibitor.CLASS_NAME).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;
        CustomFieldSet1.AddReferenceNamesToTargetObject(proxy);


        CustomFieldSet1.Render();
    }
    protected override void InitializePage()
    {
        base.InitializePage();

        setupAssignedBooths();
        setupPreferredBooths();
        setupBoothTypes();

        setupAdditionalProducts();

        setupContacts();

        hlUpdateExhibitorInfo.NavigateUrl += targetExhibitor.ID;
        hlPurchaseAdditionalProducts.NavigateUrl += targetExhibitor.ID;
        hlAddExhibitorContact.NavigateUrl += targetExhibitor.ID;

        if (targetShow.ShowFloor != null)
        {
            hlDownloadShowFloor.NavigateUrl = GetImageUrl(targetShow.ShowFloor);
            hlDownloadShowFloor.Visible = true;
        }

        if (targetShow.StartDate < DateTime.Now)  // can't edit it
        {
            hlPurchaseAdditionalProducts.Visible = false;
            hlUpdateExhibitorInfo.Visible = false;
            hlAddExhibitorContact.Visible = false;
            lStartDatePassed.Visible = true;
        }

        CustomFieldSet1.DataBind();
    }

    private void setupContacts()
    {
        Search s = new Search("ExhibitorContact");
        s.AddCriteria(Expr.Equals("Exhibitor", targetExhibitor.ID));
        s.AddSortColumn("Type.DisplayOrder");
        s.AddOutputColumn("Type.Name");
        s.AddOutputColumn("FirstName");
        s.AddOutputColumn("LastName");
        s.AddOutputColumn("EmailAddress");
        s.AddOutputColumn("WorkPhone");
        s.AddOutputColumn("MobilePhone");

        gvBoothContacts.DataSource = ExecuteSearch(s, 0, null).Table;
        gvBoothContacts.DataBind();
    }

    private void setupAdditionalProducts()
    {
        Search s = new Search("OrderLineItem");
        s.AddCriteria(Expr.Equals("Order.BillTo", targetExhibitor.Customer));
        s.AddCriteria(Expr.Equals("Product.Show", targetExhibitor.Show));
        s.AddCriteria(Expr.Equals("Product.ClassType", "EXMER"));

        s.AddOutputColumn("Product.Name");
        s.AddOutputColumn("Quantity");
        s.AddOutputColumn("UnitPrice");
        s.AddOutputColumn("Total");
        s.AddOutputColumn("Order.ID");
        s.AddOutputColumn("Order.Name");

        s.AddSortColumn("Product.Name");

        gvAdditionalProducts.DataSource = ExecuteSearch(s, 0, null).Table;
        gvAdditionalProducts.DataBind();

    }

    private void setupBoothTypes()
    {
        if (targetExhibitor.BoothTypes == null || targetExhibitor.BoothTypes.Count == 0) return;
        trBoothTypes.Visible = true;
         
        StringBuilder sb = new StringBuilder();

        using (var api = GetServiceAPIProxy())
            foreach (var b in targetExhibitor.BoothTypes)
                sb.AppendFormat("{0}, ", api.GetName(b.Type).ResultValue);

        boothTypes = sb.ToString().Trim().TrimEnd(',');
    }

    private void setupPreferredBooths()
    {
        if (targetExhibitor.BoothPreferences == null || targetExhibitor.BoothPreferences.Count == 0) return;
        trPreferred.Visible = true;

        StringBuilder sb = new StringBuilder();

        using (var api = GetServiceAPIProxy())
            foreach (var b in targetExhibitor.BoothPreferences)
                sb.AppendFormat("{0}, ", api.GetName(b.Booth).ResultValue);

        preferredBooths = sb.ToString().Trim().TrimEnd(',');
    }

    private void setupAssignedBooths()
    {
        if (targetExhibitor.Booths == null || targetExhibitor.Booths.Count == 0) return;
        trAssigned.Visible = true;

        StringBuilder sb = new StringBuilder();

        using (var api = GetServiceAPIProxy())
            foreach (var b in targetExhibitor.Booths)
                sb.AppendFormat("{0}, ", api.GetName(b.Booth ).ResultValue);

        assignedBooths = sb.ToString().Trim().TrimEnd(',');

    }

    protected void gvBoothContacts_Command(object sender, GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32( e.CommandArgument );
        switch (e.CommandName)
        {
            case "Edit":
                GoTo("AddEditExhibitorContact.aspx?contextID=" + targetExhibitor.ID + "&itemIndex=" + e.CommandArgument);
                break;

            case "Delete":
                if (targetExhibitor.Contacts != null && targetExhibitor.Contacts.Count >= index)
                {
                    targetExhibitor.Contacts.RemoveAt(index);
                    SaveObject(targetExhibitor);
                    QueueBannerMessage("Exhibitor contact has been removed successfully.");
                    Refresh();
                }
                break;
        }
    }

    protected void gvBoothContacts_DataBound(object sender, GridViewRowEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Row.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                LinkButton lbDelete = (LinkButton)e.Row.FindControl("lbDelete");

                lbDelete.CommandArgument = e.Row.DataItemIndex.ToString();

                RegisterJavascriptConfirmationBox(lbDelete, "Are you sure you want to remove this contact?");

                break;
        }
    }
}