using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_Register_SelectFee : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msOrder targetOrder;
    protected msEntity targetEntity;
    protected List<EventRegistrationProductInfo> fees;
    protected List<ProductInfo> describedFees;
    protected int registrationsCount;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    #endregion

    #region Properties

    protected string EntityId
    {
        get
        {
            return Request.QueryString["entityID"];
        }
    }

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

        targetOrder = new msOrder();


        if(!string.IsNullOrWhiteSpace(EntityId))
        {
            targetEntity = LoadObjectFromAPI<msEntity>(EntityId);
            if (targetEntity == null)
            {
                GoToMissingRecordPage();
                return;
            }
        }
        else targetEntity = ConciergeAPI.CurrentEntity;

        targetOrder.BillTo = targetOrder.ShipTo = targetEntity.ID;

        if (!string.IsNullOrWhiteSpace(targetEvent.Chapter))
            targetChapter = LoadObjectFromAPI<msChapter>(targetEvent.Chapter);

        if (!string.IsNullOrWhiteSpace(targetEvent.Section))
            targetSection = LoadObjectFromAPI<msSection>(targetEvent.Section);

        if (!string.IsNullOrWhiteSpace(targetEvent.OrganizationalLayer))
            targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(targetEvent.OrganizationalLayer);
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

        //MS-2100 
        //Has to happen before describing products because this may change the target entity
        initializeGroupRegistration();

        setFeesAndContinueIfApplicable();
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
        lblGroup.Text =  group.Name;

        // now, let's set the target entity
        // MS-2100
        var entity = LoadObjectFromAPI<msEntity>( MultiStepWizards.GroupRegistration.RegistrantID);
        lblRegistrant.Text = entity.Name ;
        targetEntity = entity;
    }

    protected void setFeesAndContinueIfApplicable()
    {
        
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            fees = proxy.GetApplicableRegistrationFees(targetEvent.ID, targetEntity.ID).ResultValue;
            fees.RemoveAll(x => !x.SellOnline || x.IsGuestRegistration);

            describedFees =
                proxy.DescribeProducts(targetEntity.ID, fees.Select(x => x.ProductID).ToList()).ResultValue;
        }
        fees = fees.OrderBy(x => x.DisplayOrder).ThenBy(x => x.ProductName).ToList();

        foreach (var fee in fees)
        {
            if(describedFees != null)
            {
                var describedFee = describedFees.SingleOrDefault(x => x.ProductID == fee.ProductID);
                if(describedFee != null)
                {
                    fee.Price = describedFee.Price;
                    fee.DisplayPriceAs = describedFee.DisplayPriceAs;
                }
            }

            fee.ProductName = fee.ProductName.Replace(targetEvent.Name + " - ", ""); 

            fee.ProductName = string.Format("{0} - <font color=green>{1}</font>", fee.ProductName,
                string.IsNullOrWhiteSpace(fee.DisplayPriceAs) ? fee.Price.ToString("C") : fee.DisplayPriceAs);
            if (fee.IsSoldOut)
                fee.ProductName += " SOLD OUT";

            if (!fee.IsEligible)
                fee.ProductName += " (ineligible)";
        }

        switch (fees.Count(x => x.IsEligible))
        {
            case 0:
                lblNoRegistrationFees.Visible = true;
                btnContinue.Enabled = false;
                break;
            case 1: //If there's only one fee then auto-select it
                SetRegistrationFee(fees.Single(x => x.IsEligible).ProductID);
                if (registrationsCount == 0) // but only move if we don't need to tell them about the existing reg
                    MoveToNextStep();
                break;
        }

        rblRegistrationFees.DataSource = fees;
        rblRegistrationFees.DataBind();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

        if (targetEvent.SafeGetValue<bool>("InviteOnly") && !EventLogic.IsInvited( targetEvent.ID , targetEntity.ID  ) )
            return false;

        if (targetEntity.ID == ConciergeAPI.CurrentEntity.ID)
            return true;

        if (targetChapter != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetChapter.Leaders);

        if (targetSection != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetOrganizationalLayer.Leaders);

        var mode = Request.QueryString["mode"];
        if (mode == "group" && targetEntity.ID == ConciergeAPI.CurrentEntity.ID)
            return true;

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


    protected void setExistingRegistrationCountLabel()
    {
        //Search for existing registrations for the current entity/event from the Concierge API
        Search sRegistered = new Search { Type = msEventRegistration.CLASS_NAME, Context = targetEvent.ID };
        sRegistered.AddCriteria(Expr.Equals("Owner", targetEntity.ID));
        sRegistered.AddCriteria(Expr.IsBlank("CancellationDate"));

        sRegistered.OutputColumns.Add(new SearchOutputColumn { Name = "Name", AggregateFunction = SearchOuputAggregate.Count });

        // add one more criteria - we only care about existing if the name hasn't been changed
        sRegistered.AddCriteria(Expr.Equals("Name", targetEntity.Name));

        registrationsCount = (int)ExecuteSearch(sRegistered, 0, null).Table.Rows[0]["Name"];

        //Display a message if there are existing registrations for the current entity
        if (registrationsCount > 0)
        {
            lblExistingRegistration.Text = string.Format("Note, you already have {0} registration(s) for this event.",
                                                         registrationsCount);
            lblExistingRegistration.Visible = true;
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

    protected void MoveToNextStep()
    {
        MultiStepWizards.RegisterForEvent.Order = targetOrder;
        var mode = Request.QueryString["mode"];
        GoTo(mode == "group"
                 ? string.Format("~/events/Register_CreateRegistration.aspx?contextID={0}&mode=group&individualID={1}", targetEvent.ID, Request.QueryString["individualID"])
                 : string.Format("~/events/Register_CreateRegistration.aspx?contextID={0}", targetEvent.ID));
    }

    protected void SetRegistrationFee(string registrationFeeID)
    {
        MultiStepWizards.RegisterForEvent.RegistrationFee =
            LoadObjectFromAPI(registrationFeeID).ConvertTo<msRegistrationFee>();
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

        setExistingRegistrationCountLabel();
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        SetRegistrationFee(rblRegistrationFees.SelectedValue);
        MoveToNextStep();
    }

    protected void rblRegistrationFees_DataBound(object sender, EventArgs e)
    {
        foreach (ListItem item in rblRegistrationFees.Items)
        {
            if (item.Text.EndsWith("(ineligible)"))
                item.Enabled = false;
        }
    }

    #endregion
}