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

public partial class events_ManageGroupRegistration : PortalPage 
{
    protected msEvent targetEvent;
    protected msOrganization targetOrganization;

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

        
        targetEvent = LoadObjectFromAPI<msEvent>( ContextID );
        if (targetEvent == null) GoToMissingRecordPage();

        targetOrganization = LoadObjectFromAPI<msOrganization >( Request.QueryString["organizationID"] );
        if (targetOrganization == null) GoToMissingRecordPage();
    }


    protected override bool CheckSecurity()
    {
        var api = GetConciegeAPIProxy();
        var entities = GroupRegistrationLogic.GetEntitiesEligibleForGroupRegistration(targetEvent, CurrentEntity, api);

        if ( entities == null || ! entities.Contains( targetOrganization.ID ) )
            GoTo("/AccessDenied.aspx"); // security violation
        return base.CheckSecurity();
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        gvRegistrants.DataSource = null;
        gvRegistrants.DataBind();

        hlRegistration.NavigateUrl += targetEvent.ID + "&organizationID=" + targetOrganization.ID;
        hlBackToEvents.NavigateUrl += targetEvent.ID;

        initializePendingRegistrations();
        initializeRegistrations();

        if (!GroupRegistrationLogic.IsGroupRegistrationOpen(targetEvent))
        {
            lblGroupRegStatus.Text = "CLOSED";
            lblGroupRegStatus.ForeColor = Color.Red;
            hlRegistration.Visible = false;
        }
        else
        {
            DateTime? dt = targetEvent.AllowGroupRegistrationsUntil;
            if (dt == null)
                dt = targetEvent.RegistrationCloseDate;

            if (dt == null)
                dt = targetEvent.EndDate;

            lblGroupRegStatus.Text = string.Format("Open until {0:d}", dt);
        }

        CustomTitle.Text = string.Format("{0} Group Registration - {1}", targetEvent.Name, targetOrganization.Name);
    }

    private void initializeRegistrations()
    {
        Search s = new Search(msEventRegistration.CLASS_NAME);
        s.Context = targetEvent.ID;
        s.AddCriteria(Expr.Equals("Group", targetOrganization.ID));

        s.AddOutputColumn("Owner.LocalID");
        s.AddOutputColumn("Owner.Name");
        s.AddOutputColumn("Fee.Name");
        s.AddOutputColumn("Owner.LocalID");
        s.AddOutputColumn(msRegistrationBase.FIELDS.CancellationDate);

        var dt = APIExtensions.GetSearchResult(s, 0, null).Table;
        dt.Columns.Add("Status");

        foreach (DataRow dr in dt.Rows)
            if (dr["CancellationDate"] != DBNull.Value)
                dr["Status"] = "Cancelled";
            else
                dr["Status"] = "Active";

        gvRegistrants.DataSource = dt;
        gvRegistrants.DataBind();
    }

    private void initializePendingRegistrations()
    {
        var group = MultiStepWizards.GroupRegistration.Group;
        msEvent ev = MultiStepWizards.GroupRegistration.Event;
        var order = MultiStepWizards.GroupRegistration.Order;

        if (group == null || group.ID != targetOrganization.ID)   // no group, or wrong group
            return;

        if (ev == null || ev.ID != targetEvent.ID)    // no event or wrong event
            return;

        if (order == null || order.LineItems == null || order.LineItems.Count == 0)
            return;

        var api = GetConciegeAPIProxy();

        // now, process the order to get prices
        order = api.PreProcessOrder(order).ResultValue.FinalizedOrder.ConvertTo<msOrder>();

      

        DataTable dt = new DataTable();
        dt.Columns.Add("ID");
        dt.Columns.Add("LocalID");
        dt.Columns.Add("Name");
        dt.Columns.Add("Fee");
        dt.Columns.Add("Cost", typeof(decimal));

        DataRow currentRow = null;
        
        foreach (msOrderLineItem li in order.LineItems)
        {
            // now, do we have an override specified - or, is it duped?
            msIndividual i = LoadObjectFromAPI<msIndividual>(li.OverrideShipTo);
            if (i == null)
                continue;

            // this code assumes the registration fee is first
            if (currentRow == null || Convert.ToString(currentRow["ID"]) != i.ID)   // new row
            {
                
                currentRow = dt.NewRow();
                dt.Rows.Add(currentRow);

                currentRow["LocalID"] = i.LocalID;
                currentRow["Name"] = i.Name;
                var productName = api.GetName(li.Product).ResultValue;
                currentRow["Fee"] = productName;
                currentRow["Cost"] = li.Total;
                currentRow["ID"] = i.ID;

            }
            else
                currentRow["Cost"] = (decimal)currentRow["Cost"] + li.Total;    // increment the fee

           
        }

        


        gvPendingRegistrations.DataSource = dt;
        gvPendingRegistrations.DataBind();
  
        btnCompleteGroup.Visible = true;
        pnlPending.Visible = true;

        lblTotalPending.Text = order.LineItems.Sum(x => x.Total).ToString("C");
    }

    protected void btnCompleteGroup_Click(object sender, EventArgs e)
    {

        var order = MultiStepWizards.GroupRegistration.Order;

        order.BillTo = order.ShipTo = targetOrganization.ID;
        MultiStepWizards.PlaceAnOrder.OrderCompleteUrl = Request.Url.ToString();    // come back here

        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(order);
      
         
    }

    protected void btnCancelGroup_Click(object sender, EventArgs e)
    {
        MultiStepWizards.GroupRegistration.NavigateBackToGroupRegistrationIfApplicable( targetEvent.ID  );
    }

    protected void gvRegistrants_Command(object sender, GridViewCommandEventArgs e)
    {
            
    }

    protected void gvPendingRegistrations_Command(object sender, GridViewCommandEventArgs e)
    {

        switch (e.CommandName)
        {
            case "Remove":
                string userToRemove = (string) e.CommandArgument;

                var order = MultiStepWizards.GroupRegistration.Order;
                if (order != null)
                    order.LineItems.RemoveAll(x => x.OverrideShipTo == userToRemove);
                Refresh();
                break;
        }
    }

    protected void gvPendingRegistrations_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {

        DataRowView pt = (DataRowView)e.Row.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                LinkButton lbCancel = (LinkButton)e.Row.FindControl("lbCancel");


                lbCancel.CommandArgument = Convert.ToString(pt["ID"]);  // set the ID

                break;
        }
    }

}