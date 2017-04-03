using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Results;

public partial class events_ViewEvent : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected DataView dvEventLinks;
    protected DataView dvRegistrations;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
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

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadDataFromConcierge(proxy);
        }
        if (targetEvent == null) GoToMissingRecordPage();
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

        blInformationLinks.DataSource = dvEventLinks;
        blInformationLinks.DataBind();

        setRegistrationLink();
        initializeExhibits();

        // Bind any registrations (only if logged in)
        gvRegistrations.DataSource = dvRegistrations;
        gvRegistrations.DataBind();

        if (!targetEvent.AllowRegistrantsToChangeSessions ||
            (targetEvent.DeadlineForChangingSessions.HasValue && targetEvent.DeadlineForChangingSessions < DateTime.Now) ||
            !EventLogic.HasSessions(targetEvent.ID))
        {
            gvRegistrations.Columns[3].Visible = false;
        }

        setupAbstracts();

        liRegisterSomeoneElse.Visible = liEditThisEvent.Visible = liSearchEventRegistrations.Visible = isLeader();

        initializeGroupRegistration();

        hlDiscussionBoard.Visible = IsModuleActive("Discussions");
        hlDiscussionBoard.NavigateUrl += ContextID;

        hlDownloadIcal.NavigateUrl = string.Format("{0}/ical?a={1}&e={2}",
                                                   ConfigurationManager.AppSettings["ImageServerUri"],
                                                   ConciergeAPIProxyGenerator.AssociationId ,
                                                   targetEvent.ID);

        CustomTitle.Text = string.Format("{0}", targetEvent.Name);
    }

    private void initializeExhibits()
    {
        // show all exhibit shows linked to this event
        Search s = new Search(msExhibitShow.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msExhibitShow.FIELDS.Event, targetEvent.ID));

        if (!ConciergeAPI.HasBackgroundConsoleUser) // there's no background user
            s.AddCriteria(Expr.Equals(msExhibitShow.FIELDS.VisibleInPortal, true));

        s.AddSortColumn("Name");
        s.AddOutputColumn("Name");
        s.AddOutputColumn(msExhibitShow.FIELDS.VisibleInPortal);

        rptExhibits.DataSource = APIExtensions.GetSearchResult(s, 0, null).Table;
        rptExhibits.DataBind();
    }

    protected string DisplayHiddenMessage(object o)
    {
        if (o != DBNull.Value && o != null && Convert.ToBoolean(o))
        {
            return string.Empty;
        }

        return " [HIDDEN]";
    }

    private void initializeGroupRegistration()
    {
        // is group registration even enabled?
        if (!targetEvent.EnableGroupRegistrations)
            return; // NOPE!
        
        var api = GetConciegeAPIProxy();

        List<string> entitiesEligibleForGroupRegistration = GroupRegistrationLogic.GetEntitiesEligibleForGroupRegistration(targetEvent, CurrentEntity, api );

        if (entitiesEligibleForGroupRegistration == null || entitiesEligibleForGroupRegistration.Count == 0)
            return; // nothing to do - this person isn't eligible to group register anyone

        List<NameValueStringPair> companies = new List<NameValueStringPair>();

        foreach (var e in entitiesEligibleForGroupRegistration)
        {
            string name = api.GetName(e).ResultValue;
            if (string.IsNullOrWhiteSpace(name)) continue; // nothing to do
            companies.Add(new NameValueStringPair(name, e));
        }


        rptGroupRegistration.DataSource = companies;
        rptGroupRegistration.DataBind();
    }

   

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (targetEvent.SafeGetValue<bool>("InviteOnly") && 
            ( ConciergeAPI.CurrentEntity== null ||
            !EventLogic.IsInvited( targetEvent.ID , ConciergeAPI.CurrentEntity.ID  ) ) )
            return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

        return targetEvent.VisibleInPortal;
    }

    #endregion

    #region Methods


    protected void loadDataFromConcierge(IConciergeAPIService serviceProxy)
    {
        targetEvent = serviceProxy.LoadObjectFromAPI<msEvent>(ContextID);
        loadEventOwners(serviceProxy);

        var searches = new List<Search>();

        // Event information links related to current event
        var sEventLinks = new Search { Type = msEventInformationLink.CLASS_NAME };
        sEventLinks.AddOutputColumn("ID");
        sEventLinks.AddOutputColumn("Name");
        sEventLinks.AddCriteria(Expr.Equals("IsActive", 1));
        sEventLinks.AddCriteria(Expr.Equals("Event.ID", ContextID));
        sEventLinks.AddSortColumn("DisplayOrder");
        searches.Add(sEventLinks);

        //If we're logged in search for event registrations for the current entity
        if (ConciergeAPI.CurrentEntity != null)
        {
            Search sRegistrations = new Search {Type = msEventRegistration.CLASS_NAME, ID = "Registrations"};
            sRegistrations.AddOutputColumn("ID");
            sRegistrations.AddOutputColumn("Name");
            sRegistrations.AddOutputColumn("Fee.Name");
            sRegistrations.AddOutputColumn("Fee.IsGuestRegistration");
            sRegistrations.AddOutputColumn("CreatedDate");
            sRegistrations.AddCriteria(Expr.Equals("Owner.ID", ConciergeAPI.CurrentEntity.ID));
            sRegistrations.AddCriteria(Expr.Equals("Event.ID", ContextID));
            sRegistrations.AddCriteria(Expr.IsBlank(msRegistrationBase.FIELDS.CancellationDate));
            searches.Add(sRegistrations);
        }

        var searchResults = APIExtensions.GetMultipleSearchResults(searches, 0, null);
        dvEventLinks = new DataView(searchResults[0].Table);

        if (searchResults.Exists(x => x.ID == "Registrations"))
            dvRegistrations = new DataView(searchResults.Single(x => x.ID == "Registrations").Table);
    }

    protected void setRegistrationLink()
    {
        // is the registration open?
        if (targetEvent.RegistrationOpenDate != null && targetEvent.RegistrationOpenDate > DateTime.Now)
        {
            lblRegistrationClosed.Text = string.Format("Registration for this event opens on {0} at {1}.", targetEvent.RegistrationOpenDate.Value.ToLongDateString(),
                targetEvent.RegistrationOpenDate.Value.ToShortTimeString());
                
            hlRegistration.Visible= false;
            return;
        }

        // is it closed?
        if (EventLogic.IsRegistrationClosed( targetEvent ))
        {
            lblRegistrationClosed.Text = "Registration for this event is closed.";
            hlRegistration.Visible = false;
            return;
        }

        // CORE-1018
        hlRegistration.NavigateUrl = !string.IsNullOrWhiteSpace(targetEvent.RegistrationUrl) ? targetEvent.RegistrationUrl : string.Format("~/events/RegisterForEvent.aspx?contextID={0}", ContextID);

        if ( targetEvent.RegistrationMode == EventRegistrationMode.Normal &&
            ConciergeAPI.CurrentEntity != null && EventLogic.IsRegistered( targetEvent.ID, ConciergeAPI.CurrentEntity.ID )
            
            )
        {
            // MS-3032
            lblRegistrationClosed.Text = "You have already registered for this event.";
            hlRegistration.Visible = false; 

            // now, can we register a guest?
            using (var api = GetServiceAPIProxy())
            {
                if (
                    api.GetApplicableRegistrationFees(targetEvent.ID, CurrentEntity.ID)
                       .ResultValue.Exists(x => x.IsGuestRegistration))
                    // guest registration is available
                    liRegisterAGuest.Visible = true;
            }

            return;
        }

        if (targetEvent.RegistrationMode == EventRegistrationMode.Tabled) // tabled event
        {
            hlRegistration.Visible = false;
            hlPurchaseSeats.Visible = true;
            hlViewMySeats.Visible = true;
            hlPurchaseSeats.NavigateUrl = string.Format("PurchaseTableSeats.aspx?contextID=" + ContextID);
            hlViewMySeats.NavigateUrl = string.Format("ViewTableSeats.aspx?contextID=" + ContextID);
        }        
    }

    protected void loadEventOwners(IConciergeAPIService proxy)
    {
        if (!string.IsNullOrWhiteSpace(targetEvent.Chapter))
            targetChapter = proxy.LoadObjectFromAPI<msChapter>(targetEvent.Chapter);

        if (!string.IsNullOrWhiteSpace(targetEvent.Section))
            targetSection = proxy.LoadObjectFromAPI<msSection>(targetEvent.Section);

        if (!string.IsNullOrWhiteSpace(targetEvent.OrganizationalLayer))
            targetOrganizationalLayer = proxy.LoadObjectFromAPI<msOrganizationalLayer>(targetEvent.OrganizationalLayer);
    }

    protected bool isLeader()
    {
        if (CurrentEntity == null)
            return false;

        if (targetChapter != null)
            return canManageEvents(targetChapter.Leaders);

        if (targetSection != null)
            return canManageEvents(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return canManageEvents(targetOrganizationalLayer.Leaders);

        //Default to false for now because currently only leaders can create events in the portal
        return false;
    }

    protected bool canManageEvents(List<msMembershipLeader> leaders)
    {
        if (leaders == null)
            // no leaders to speak of
            return false;

        var leader = leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanManageEvents;
    }

    #endregion

    #region Event Handlers

    protected void setupAbstracts()
    {
        if (! targetEvent.EnableAbstracts) return;

        hlSubmitAbstracts.Visible = true;
        
        if ( targetEvent.AcceptAbstractsFrom != null && targetEvent.AcceptAbstractsFrom.Value > DateTime.Now )
        {
            lblAbstracts.Text = string.Format( "<BR/>Abstracts not accepted until {0}.", targetEvent.AcceptAbstractsFrom.Value.ToLongDateString() );
            hlSubmitAbstracts.Visible = false;
            return;
        }

        if ( targetEvent.AcceptAbstractsUntil != null && targetEvent.AcceptAbstractsUntil.Value < DateTime.Now )
        {
            lblAbstracts.Text += "<BR/>Abstracts submission is closed.";
            hlSubmitAbstracts.Visible = false;
            return;
        }

        hlSubmitAbstracts.NavigateUrl = hlSubmitAbstracts.NavigateUrl + targetEvent.ID;

        if (ConciergeAPI.CurrentUser == null) return; // we need a logged in user 

        hlViewMyAbstracts.Visible = true;
        hlViewMyAbstracts.NavigateUrl += targetEvent.ID;


    }
    protected void blInformationLinks_Click(object sender, BulletedListEventArgs e)
    {
        ListItem li = blInformationLinks.Items[e.Index];
        GoTo(string.Format("~/events/ViewEventInformationLink.aspx?contextID={0}&eventInformationLinkID={1}", targetEvent.ID, li.Value));
    }

    #endregion

    protected void gvRegistrations_RowDataBoud(object sender, GridViewRowEventArgs e)
    {
        var dr = (DataRowView)e.Row.DataItem;

        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                if (dr["Fee.IsGuestRegistration"] is bool && (bool)dr["Fee.IsGuestRegistration"])
                {
                    e.Row.Cells[3].Visible = false;
                }

                break;
        }
    }
}