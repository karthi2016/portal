using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_CreateEditEventMerchandise : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msEventMerchandise targetEventMerchandise;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected DataView dvConfirmationEmails;

    protected ClassMetadata eventMerchandiseClassMetadata;
    protected Dictionary<string, FieldMetadata> eventMerchandiseFieldMetadata;

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

        //Describe a Registration Fee.  We will need this metadata to bind to the acceptable values for certain fields and for creating a new Registration Fee
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            eventMerchandiseClassMetadata = proxy.DescribeObject(msEventMerchandise.CLASS_NAME).ResultValue;
            eventMerchandiseFieldMetadata = eventMerchandiseClassMetadata.GenerateFieldDictionary();
        }

        var contextObject = APIExtensions.LoadObjectFromAPI(ContextID);

        if (contextObject.ClassType == msEvent.CLASS_NAME)
        {
            targetEvent = contextObject.ConvertTo<msEvent>();
            targetEventMerchandise = msEventMerchandise.FromClassMetadata(eventMerchandiseClassMetadata);
            lblTitleAction.Text = "Create";
        }
        else
        {
            targetEventMerchandise = contextObject.ConvertTo<msEventMerchandise>();
            targetEvent = LoadObjectFromAPI<msEvent>(targetEventMerchandise.Event);
            lblTitleAction.Text = "Edit";
        }

        if (targetEvent == null || targetEventMerchandise == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEventMerchandise.Event = targetEvent.ID;

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
#pragma warning disable 0618
        reDescription.NewLineBr = false;
#pragma warning restore 0618
        loadDataFromConcierge();
        
        lblEventName.Text = targetEvent.Name;

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");

        ddlConfirmationEmail.DataSource = dvConfirmationEmails;
        ddlConfirmationEmail.DataBind();

        bindRegistrationFee();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (targetChapter != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetChapter.Leaders);

        if (targetSection != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetOrganizationalLayer.Leaders);

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

    protected void setOwnerBackLinks(string ownerId, string ownerName, string viewUrl, string manageEventsUrl)
    {
        hlEventOwner.NavigateUrl = string.Format("{0}?contextID={1}", viewUrl, ownerId);
        hlEventOwner.Text = string.Format("{0} >", ownerName);
        hlEventOwner.Visible = true;

        hlEventOwnerTask.Text = string.Format("Back to Manage {0} Events", ownerName);
        hlEventOwnerTask.NavigateUrl = string.Format("{0}?contextID={1}", manageEventsUrl, ownerId);
        liEventOwnerTask.Visible = true;
    }

    protected void loadDataFromConcierge()
    {
        var searches = new List<Search>();

        // Search for confirmation emails
        var sConfirmationEmails = new Search(msEmailTemplateContainer.CLASS_NAME);
        sConfirmationEmails.AddOutputColumn("ID");
        sConfirmationEmails.AddOutputColumn("Name");
        sConfirmationEmails.AddCriteria(Expr.Equals("ApplicableType", "OrderLineItem"));
        sConfirmationEmails.AddSortColumn("Name");

        searches.Add(sConfirmationEmails);

        var searchResults = APIExtensions.GetMultipleSearchResults(searches, 0, null);
        dvConfirmationEmails = new DataView(searchResults[0].Table);
    }

    protected void unbindAndSave()
    {
        unbindRegistrationFee();
        targetEventMerchandise = SaveObject(targetEventMerchandise).ConvertTo<msEventMerchandise>();
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

    #endregion

    #region Data Binding

    protected void bindRegistrationFee()
    {
        tbCode.Text = targetEventMerchandise.Code;
        tbName.Text = targetEventMerchandise.Name;
        
        tbPrice.Text = string.Format("{0:F}",targetEventMerchandise.Price);
        tbMemberPrice.Text = string.Format("{0:F}", targetEventMerchandise.MemberPrice);

        reDescription.Content = targetEventMerchandise.Description;

        chkSellOnline.Checked = targetEventMerchandise.SellOnline;
        chkAllowCustomersToPayLater.Checked = targetEventMerchandise.AllowCustomersToPayLater;
        tbDisplayPriceAs.Text = targetEventMerchandise.DisplayPriceAs;
        ddlConfirmationEmail.SelectedValue = targetEventMerchandise.ConfirmationEmail;

        dtpSellFrom.SelectedDate = targetEventMerchandise.SellFrom;
        dtpSellUntil.SelectedDate = targetEventMerchandise.SellUntil;
        dtpNewUntil.SelectedDate = targetEventMerchandise.NewUntil;
    }

    protected void unbindRegistrationFee()
    {
        targetEventMerchandise.Code = tbCode.Text;
        targetEventMerchandise.Name = tbName.Text;

        targetEventMerchandise.Price = decimal.Parse(tbPrice.Text);


        if (string.IsNullOrWhiteSpace(tbMemberPrice.Text)) 
            targetEventMerchandise.MemberPrice = null;
        else targetEventMerchandise.MemberPrice = decimal.Parse(tbMemberPrice.Text);

        targetEventMerchandise.Description = reDescription.Content;

        targetEventMerchandise.SellOnline = chkSellOnline.Checked;
        targetEventMerchandise.AllowCustomersToPayLater = chkAllowCustomersToPayLater.Checked;
        targetEventMerchandise.DisplayPriceAs = tbDisplayPriceAs.Text;
        targetEventMerchandise.ConfirmationEmail = ddlConfirmationEmail.SelectedValue;

        targetEventMerchandise.SellFrom = dtpSellFrom.SelectedDate;
        targetEventMerchandise.SellUntil = dtpSellUntil.SelectedDate;
        targetEventMerchandise.NewUntil = dtpNewUntil.SelectedDate;
    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        unbindAndSave();
        GoTo(string.Format("~/events/CreateEditEvent.aspx?contextID={0}",targetEvent.ID));
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("~/events/CreateEditEvent.aspx?contextID={0}", targetEvent.ID));
    }

    #endregion
}