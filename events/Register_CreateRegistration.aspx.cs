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
using MemberSuite.SDK.Utilities;
using Telerik.Web.UI;

public partial class events_Register_CreateRegistration : PortalPage
{
    #region Fields

    protected msOrder targetOrder;
    protected msEvent targetEvent;
    protected msEntity targetEntity;
    protected msRegistrationFee targetRegistrationFee;
    protected ProductInfo describedRegistrationFee;
    protected List<msSessionTimeSlot> timeslots;
    protected EventManifest manifest;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

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

        targetEvent = LoadObjectFromAPI(ContextID).ConvertTo<msEvent>();
        if (targetEvent == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetOrder = MultiStepWizards.RegisterForEvent.Order;
        targetRegistrationFee = MultiStepWizards.RegisterForEvent.RegistrationFee;

        if (targetOrder == null || string.IsNullOrWhiteSpace(targetOrder.BillTo) || string.IsNullOrWhiteSpace(targetOrder.ShipTo) || targetRegistrationFee == null)
        {
            GoTo(string.Format("~/events/Register_SelectFee.aspx?contextID={0}", targetEvent.ID));
            return;
        }

        using (var proxy = GetServiceAPIProxy())
        {
            describedRegistrationFee =
                proxy.DescribeProducts(ConciergeAPI.CurrentEntity.ID, new List<string>() { targetRegistrationFee.ID }).
                    ResultValue[0];
        }

        targetEntity = LoadObjectFromAPI<msEntity>(targetOrder.BillTo);
        if (targetEntity == null)
        {
            GoToMissingRecordPage();
            return;
        }

        loadEventOwners();
    }

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();


        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");


        setManifest();
        setTimeSlots();
        setGuestRegistrationFees();
        setMerchandise();

        lblRegistrant.Text = targetEntity.Name;
        lblFee.Text = string.Format("{0} - <font color=green>{1}</font>", targetRegistrationFee.Name, string.IsNullOrWhiteSpace(describedRegistrationFee.DisplayPriceAs) ? describedRegistrationFee.Price.ToString("C") : describedRegistrationFee.DisplayPriceAs);

        // if we have nothing, move on 
        if (!pnlGuests.Visible && !pnlMerchandise.Visible && !pnlSessions.Visible)
        {
            var mode = Request.QueryString["mode"];
            GoTo(mode == "group"
                     ? string.Format("~/events/Register_RegistrationForm.aspx?contextID={0}&mode=group&individualID={1}", ContextID, Request.QueryString["individualID"])
                     : string.Format("~/events/Register_RegistrationForm.aspx?contextID={0}", ContextID));
        }

        initializeGroupRegistration();

    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

        if (ConciergeAPI.CurrentEntity.ID == targetEntity.ID)
            return targetEvent.VisibleInPortal;

        if (targetChapter != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetChapter.Leaders);

        if (targetSection != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetOrganizationalLayer.Leaders);

        var mode = Request.QueryString["mode"];
        if (mode == "group" && ConciergeAPI.CurrentEntity.ID != targetEntity.ID)
            return targetEvent.VisibleInPortal;

        //Default to false for now because currently only leaders can create events in the portal
        return false;
    }

    #endregion

    #region Methods

    protected bool canManageEvents(List<msMembershipLeader> leaders)
    {
        if (leaders == null)
            // no leaders to speak of
            return false;

        var leader = leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanManageEvents;
    }

    protected void setTimeSlots()
    {
        timeslots = new List<msSessionTimeSlot>();

        using (var api = GetServiceAPIProxy())
        {
            Search sTimeSlots = new Search { Type = msSessionTimeSlot.CLASS_NAME };
            sTimeSlots.AddOutputColumn("ID");
            sTimeSlots.AddOutputColumn("Name");
            sTimeSlots.AddOutputColumn("StartTime");
            sTimeSlots.AddOutputColumn("EndTime");
            sTimeSlots.AddOutputColumn("AllowMultipleSessions");
            sTimeSlots.AddCriteria(Expr.Equals(msSessionTimeSlot.FIELDS.Event, targetEvent.ID));
            sTimeSlots.AddSortColumn("StartTime");
            sTimeSlots.AddSortColumn("Name");


            foreach (DataRow drResult in api.ExecuteSearch(sTimeSlots, 0, null).ResultValue.Table.Rows)
                timeslots.Add(MemberSuiteObject.FromDataRow(drResult).ConvertTo<msSessionTimeSlot>());
        }


        pnlSessions.Visible = false;

        // add the NULL timeslot, for sessions that have not been placed into time slots
        timeslots.Insert(0, new msSessionTimeSlot { Name = "Events Happening During this Event", ID = null });

        // if there's one session, one session at all, then the panel is shown
        rptSessions.DataSource = timeslots;
        rptSessions.DataBind();

        if (targetRegistrationFee.MaximumNumberOfSessions != null)
            cvMaxSession.ErrorMessage =
                  string.Format(
                      "You have selected too many sessions. When registering using the '{0}' fee, you are allowed a maximum of {1} sessions.",
                      targetRegistrationFee.Name, targetRegistrationFee.MaximumNumberOfSessions.Value);

    }

    protected void setGuestRegistrationFees()
    {
        if (targetRegistrationFee.IsGuestRegistration)    // already registering a guest
            return;

        if (manifest.GuestRegistrationFees.Count > 0)
        {
            pnlGuests.Visible = true;
            gvGuests.DataSource = manifest.GuestRegistrationFees;
            gvGuests.DataBind();
        }
    }

    protected void setMerchandise()
    {
        // MS-4854. Show only the merchandise which is visible online.
        var visibleMerchandize = manifest.Merchandise.Where(m => m.SellOnline);
        if (visibleMerchandize.Any())
        {
            pnlMerchandise.Visible = true;
            gvMerchandise.DataSource = visibleMerchandize;
            gvMerchandise.DataBind();
        }
    }

    protected void setManifest()
    {
        using (var api = GetServiceAPIProxy())
        {
            manifest = api.GetEventManifest(targetEvent.ID, targetEntity.ID, targetRegistrationFee.ID).ResultValue;

            // Now, let's clean up the fees so the event name doesn't show 
            foreach (var session in manifest.Sessions)
                if (session.Fees != null)
                    foreach (var fee in session.Fees)
                        fee.ProductName = fee.ProductName.Replace(targetEvent.Name + " - ", "");

            foreach (var fee in manifest.GuestRegistrationFees)
                fee.ProductName = fee.ProductName.Replace(targetEvent.Name + " - ", "");

            foreach (var fee in manifest.Merchandise)
                fee.ProductName = fee.ProductName.Replace(targetEvent.Name + " - ", "");
        }
    }

    protected void loadEventOwners()
    {
        if (!string.IsNullOrWhiteSpace(targetEvent.Chapter))
            targetChapter = LoadObjectFromAPI<msChapter>(targetEvent.Chapter);

        if (!string.IsNullOrWhiteSpace(targetEvent.Section))
            targetSection = LoadObjectFromAPI<msSection>(targetEvent.Section);

        if (!string.IsNullOrWhiteSpace(targetEvent.OrganizationalLayer))
            targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(targetEvent.OrganizationalLayer);
    }

    protected void setOwnerBackLinks(string ownerId, string ownerName, string viewUrl, string manageEventsUrl)
    {
        hlEventOwner.NavigateUrl = string.Format("{0}?contextID={1}", viewUrl, ownerId);
        hlEventOwner.Text = string.Format("{0} >", ownerName);
        hlEventOwner.Visible = true;
    }

    #endregion

    #region Data Binding
    int numberOfSessions = 0;
    protected void unbindControls()
    {
        List<msOrderLineItem> lineItemsToAdd = new List<msOrderLineItem>();


        // add the sessions
        foreach (RepeaterItem riSession in rptSessions.Items)
        {
            GridView gvSessions = riSession.FindControl("gvSessions") as GridView;

            foreach (GridViewRow row in gvSessions.Rows)
            {
                DropDownList ddlFee = (DropDownList)row.FindControl("ddlFee");
                if (ddlFee == null || String.IsNullOrEmpty(ddlFee.SelectedValue))
                    continue;   // for instance, this might be a "None selected" row

                decimal quantity = 0;
                CheckBox cbRegister = (CheckBox)row.FindControl("cbRegister");
                TextBox tbQuantity = (TextBox)row.FindControl("tbQuantity");
                RadioButton rbRegister = (RadioButton)row.FindControl("rbRegister");

                if (cbRegister.Visible && cbRegister.Checked)
                    quantity = 1;
                else if (rbRegister.Visible && rbRegister.Checked)
                    quantity = 1;
                else if (tbQuantity.Visible)
                    quantity = decimal.Parse(tbQuantity.Text);  // we know this is valid cuz of the validator

                if (quantity <= 0) continue;

                msOrderLineItem li = new msOrderLineItem
                {
                    Quantity = quantity,
                    Product = ddlFee.SelectedValue
                };

                numberOfSessions++;
                lineItemsToAdd.Add(li); // add it

            }
        }

        // now, the guests
        foreach (GridViewRow row in gvGuests.Rows)
        {
            TextBox tbQuantity = (TextBox)row.FindControl("tbQuantity");
            string product = (string)gvGuests.DataKeys[row.RowIndex].Value;

            if (String.IsNullOrEmpty(tbQuantity.Text))
                continue;

            decimal qty = decimal.Parse(tbQuantity.Text);
            if (qty <= 0) continue;

            // we need to add each item on it's own line, so it can have it's own demographics
            for( int i=0;i< qty;i++ )
            {
                msOrderLineItem li = new msOrderLineItem
                    {
                        Quantity = 1,
                        Product = product
                    };
                lineItemsToAdd.Add(li); // add it
            }
        }

        // finally, the merchandise
        foreach (GridViewRow row in gvMerchandise.Rows)
        {
            TextBox tbQuantity = (TextBox)row.FindControl("tbQuantity");
            string product = (string)gvMerchandise.DataKeys[row.RowIndex].Value;

            if (String.IsNullOrEmpty(tbQuantity.Text))
                continue;

            decimal qty = decimal.Parse(tbQuantity.Text);
            if (qty <= 0) continue;

            msOrderLineItem li = new msOrderLineItem
            {
                Quantity = qty,
                Product = product
            };
            lineItemsToAdd.Add(li); // add it
        }

        MultiStepWizards.RegisterForEvent.AdditionalLineItems = lineItemsToAdd;
    }

    #endregion

    #region Event Handlers

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MultiStepWizards.RegisterForEvent.Order = null;
        MultiStepWizards.RegisterForEvent.RegistrationFee = null;
        MultiStepWizards.RegisterForEvent.AdditionalLineItems = null;

        MultiStepWizards.GroupRegistration.NavigateBackToGroupRegistrationIfApplicable(targetEvent.ID);
        GoHome();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        MultiStepWizards.RegisterForEvent.Order = null;
        var mode = Request.QueryString["mode"];
        GoTo(mode == "group"
                 ? string.Format("~/events/Register_SelectFee.aspx?contextID={0}&mode=group&individualID={1}", ContextID, Request.QueryString["individualID"])
                 : string.Format("~/events/Register_SelectFee.aspx?contextID={0}", ContextID));
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        unbindControls();

        // wait - have we exceeded the maximum number of sessions we're suppsoed to have?
        if (targetRegistrationFee.MaximumNumberOfSessions != null &&
            numberOfSessions > targetRegistrationFee.MaximumNumberOfSessions.Value)
        {

            cvMaxSession.IsValid = false;
            
            return;
        }
        var mode = Request.QueryString["mode"];
        GoTo(mode == "group"
                 ? string.Format("~/events/Register_RegistrationForm.aspx?contextID={0}&mode=group&individualID={1}", targetEvent.ID, Request.QueryString["individualID"])
                 : string.Format("~/events/Register_RegistrationForm.aspx?contextID={0}", targetEvent.ID));
    }


    private void initializeGroupRegistration()
    {
        var group = MultiStepWizards.GroupRegistration.Group;
        if (group == null)
            return;

        var ev = MultiStepWizards.GroupRegistration.Event;
        if (ev == null || ev.ID != targetEvent.ID)
            return; // not a matching event

        pnlGroupRegistration.Visible = true;
        lblGroup.Text = group.Name;


    }
    protected void gvSessions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        EventManifestSession session = (EventManifestSession)e.Row.DataItem;

        if (Page.IsPostBack)
            return; // only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                msSessionTimeSlot timeSlot = timeslots.Where(x => x.ID == session.TimeSlotID).FirstOrDefault();
                CheckBox cbRegister = (CheckBox)e.Row.FindControl("cbRegister");
                TextBox tbQuantity = (TextBox)e.Row.FindControl("tbQuantity");
                RadioButton rbRegister = (RadioButton)e.Row.FindControl("rbRegister");
                DropDownList ddlFee = (DropDownList)e.Row.FindControl("ddlFee");
                Label lblPrice = (Label)e.Row.FindControl("lblPrice");
                RadToolTip rtpSessionDescription = (RadToolTip)e.Row.FindControl("rtpSessionDescription");
                Label lblSessionName = (Label)e.Row.FindControl("lblSessionName");

                lblSessionName.Text = session.SessionName;
                rtpSessionDescription.Title = session.SessionName;
                rtpSessionDescription.Text = session.SessionDescription;

                if (session.RegistrationMode == EventRegistrationMode.Ticketed)
                {
                    tbQuantity.Visible = true;
                }
                else
                    if (session.TimeSlotID == null || (timeSlot != null && timeSlot.AllowMultipleSessions))
                    {
                        // use checkboxes
                        cbRegister.Visible = true;
                        rbRegister.Visible = false;


                    }
                    else
                    {


                        rbRegister.Visible = true;
                        rbRegister.Checked = session.SessionID == null; // select the "do not register" option by default
                        string groupName = RegularExpressions.GetSafeFieldName(session.TimeSlotID);

                        // hack to work around ASP.NET RadioButtons in repeater controls bug
                        // http://www.codeguru.com/csharp/csharp/cs_controls/custom/article.php/c12371/
                        rbRegister.GroupName = groupName;
                        string script = string.Format(
                             "SetUniqueRadioButton('gvSessions.*{0}',this)", groupName);
                        rbRegister.Attributes.Add("onclick", script);

                        cbRegister.Visible = false;
                    }

                if (session.Fees != null)
                {
                    //MS-1587 - add where filter
                    foreach (var fee in session.Fees.Where(x => x.IsEligible))
                    {
                        // let's show the fee w/o the session name
                        string name = string.Format("{0} - {1}", fee.ProductName.Replace(session.SessionName + " - ", "")
                            ,
                            !string.IsNullOrWhiteSpace(fee.DisplayPriceAs) ? fee.DisplayPriceAs : fee.Price.ToString("C"));
                        ddlFee.Items.Add(new ListItem(name, fee.ProductID));
                    }

                    if (session.DefaultFee != null)
                        ddlFee.SelectedValue = session.DefaultFee;

                    if (ddlFee.Items.Count == 1) // just show the dang label
                    {
                        ddlFee.Visible = false;
                        lblPrice.Visible = true;

                        //MS-1587
                        //Since there's one item in the list there must be one fee where IsEligible == true
                        EventRegistrationProductInfo erpi = session.Fees.Single(x => x.IsEligible);

                        lblPrice.Text = !string.IsNullOrWhiteSpace(erpi.DisplayPriceAs) ? erpi.DisplayPriceAs : erpi.Price.ToString("C");
                    }
                }
                else
                {
                    ddlFee.Visible = false;
                    lblPrice.Visible = false;
                }

                if (session.Ineligible)   // can't select this
                {
                    rbRegister.Enabled = cbRegister.Enabled = tbQuantity.Enabled
                                                              = ddlFee.Enabled = false;
                    ddlFee.Visible = false;
                    lblPrice.Visible = true;
                    lblPrice.Text = "INELIGIBLE";
                }
                break;
        }
    }

    protected void rptSessions_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        msSessionTimeSlot ts = (msSessionTimeSlot)e.Item.DataItem;

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
                Label lblTimeSlot = (Label)e.Item.FindControl("lblTimeSlot");
                GridView gvSessions = (GridView)e.Item.FindControl("gvSessions");

                lblTimeSlot.Text = ts.Name;

                // now, let's find all of the sessions
                var sessions = manifest.Sessions.FindAll(x => x.TimeSlotID == ts.ID);
                if (sessions.Count == 0)  // there are no sessions!
                {
                    e.Item.Visible = false;
                    return;
                }

                if (ts.ID != null && !ts.AllowMultipleSessions)    // we need to add a "Do Not Select" options
                    sessions.Insert(0, new EventManifestSession
                    {
                        SessionName = "No session selected during this time slot.",
                        RegistrationMode = EventRegistrationMode.Normal,
                        TimeSlotID = ts.ID
                    });

                gvSessions.DataSource = sessions;
                gvSessions.DataBind();

                if (sessions.Count > 0)
                    pnlSessions.Visible = true;

                break;
        }
    }

    protected void gvMerchandise_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        ProductInfo pi = (ProductInfo)e.Row.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                TextBox tbQuantity = (TextBox)e.Row.FindControl("tbQuantity");
                CompareValidator CompareValidator1 = (CompareValidator)e.Row.FindControl("CompareValidator1");
                Label lblPrice = (Label)e.Row.FindControl("lblPrice");

                lblPrice.Text = !string.IsNullOrWhiteSpace(pi.DisplayPriceAs) ? pi.DisplayPriceAs : pi.Price.ToString("C");


                break;
        }
    }

    #endregion
}