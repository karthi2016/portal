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

public partial class events_CreateEditRegistrationFee : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msRegistrationFee targetRegistrationFee;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected DataView dvRegistrationClasses;
    protected DataView dvRegistrationCategories;
    protected DataView dvConfirmationEmails;

    protected ClassMetadata registrationFeeClassMetadata;
    protected Dictionary<string,FieldMetadata> registrationFeeFieldMetadata;

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
            registrationFeeClassMetadata = proxy.DescribeObject(msRegistrationFee.CLASS_NAME).ResultValue;
            registrationFeeFieldMetadata = registrationFeeClassMetadata.GenerateFieldDictionary();
        }

        MemberSuiteObject contextObject = LoadObjectFromAPI(ContextID);

        if (contextObject.ClassType == msEvent.CLASS_NAME)
        {
            targetEvent = contextObject.ConvertTo<msEvent>();
            targetRegistrationFee = msRegistrationFee.FromClassMetadata(registrationFeeClassMetadata);
            lblTitleAction.Text = "Create";
        }
        else
        {
            targetRegistrationFee = contextObject.ConvertTo<msRegistrationFee>();
            targetEvent = LoadObjectFromAPI<msEvent>(targetRegistrationFee.Event);
            lblTitleAction.Text = "Edit";
        }

        if(targetEvent == null || targetRegistrationFee == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetRegistrationFee.Event = targetEvent.ID;

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

        ddlRegistrantClass.DataSource = dvRegistrationClasses;
        ddlRegistrantClass.DataBind();

        ddlRegistrantCategory.DataSource = dvRegistrationCategories;
        ddlRegistrantCategory.DataBind();

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
        List<Search> searches = new List<Search>();

        //Search for registration classes - these are not described as pick list entries
        Search sRegistrationClasses = new Search(msRegistrationClass.CLASS_NAME);
        sRegistrationClasses.AddOutputColumn("ID");
        sRegistrationClasses.AddOutputColumn("Name");
        sRegistrationClasses.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        sRegistrationClasses.AddSortColumn("Name");

        searches.Add(sRegistrationClasses);

        //Search for registration categories - these are not described as pick list entries
        Search sRegistrationCategories = new Search(msRegistrationCategory.CLASS_NAME);
        sRegistrationCategories.AddOutputColumn("ID");
        sRegistrationCategories.AddOutputColumn("Name");
        sRegistrationCategories.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        sRegistrationCategories.AddSortColumn("Name");

        searches.Add(sRegistrationCategories);

        //Search for confirmation emails
        Search sConfirmationEmails = new Search(msEmailTemplateContainer.CLASS_NAME);
        sConfirmationEmails.AddOutputColumn("ID");
        sConfirmationEmails.AddOutputColumn("Name");
        sConfirmationEmails.AddCriteria(Expr.Equals("Context", targetEvent.ID));
        sConfirmationEmails.AddSortColumn("Name");

        searches.Add(sConfirmationEmails);

        List<SearchResult> searchResults = ExecuteSearches(searches, 0, null);
        dvRegistrationClasses = new DataView(searchResults[0].Table);
        dvRegistrationCategories = new DataView(searchResults[1].Table);
        dvConfirmationEmails = new DataView(searchResults[2].Table);
    }

    protected void unbindAndSave()
    {
        unbindRegistrationFee();
        targetRegistrationFee = SaveObject(targetRegistrationFee).ConvertTo<msRegistrationFee>();
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
        if(targetRegistrationFee.DisplayOrder.HasValue)
            tbDisplayOrder.Text = targetRegistrationFee.DisplayOrder.Value.ToString();
        tbCode.Text = targetRegistrationFee.Code;
        tbName.Text = targetRegistrationFee.Name;
        
        tbRegularPrice.Text = string.Format("{0:F}",targetRegistrationFee.Price);
        if(targetRegistrationFee.PreRegistrationPrice.HasValue)
            tbPreRegPrice.Text = string.Format("{0:F}", targetRegistrationFee.PreRegistrationPrice);
        if (targetRegistrationFee.EarlyRegistrationPrice.HasValue)
            tbEarlyRegPrice.Text = string.Format("{0:F}", targetRegistrationFee.EarlyRegistrationPrice);
        if (targetRegistrationFee.LateRegistrationPrice.HasValue)
            tbLateRegPrice.Text = string.Format("{0:F}", targetRegistrationFee.LateRegistrationPrice);

        
        chkRequiresApproval.Checked = targetRegistrationFee.RequiresApproval;
        chkIsGuestRegistration.Checked = targetRegistrationFee.IsGuestRegistration;

        ddlConfirmationEmail.SelectedValue = targetRegistrationFee.ConfirmationEmail;
        ddlRegistrantClass.SelectedValue = targetRegistrationFee.RegistrantClass;
        ddlRegistrantCategory.SelectedValue = targetRegistrationFee.RegistrantCategory;

        reDescription.Content = targetRegistrationFee.Description;

        chkSellOnline.Checked = targetRegistrationFee.SellOnline;
        chkAllowCustomersToPayLater.Checked = targetRegistrationFee.AllowCustomersToPayLater;
        tbDisplayPriceAs.Text = targetRegistrationFee.DisplayPriceAs;

        dtpSellFrom.SelectedDate = targetRegistrationFee.SellFrom;
        dtpSellUntil.SelectedDate = targetRegistrationFee.SellUntil;
        dtpNewUntil.SelectedDate = targetRegistrationFee.NewUntil;
    }

    protected void unbindRegistrationFee()
    {
        if (!string.IsNullOrWhiteSpace(tbDisplayOrder.Text))
            targetRegistrationFee.DisplayOrder = int.Parse(tbDisplayOrder.Text);
        targetRegistrationFee.Code = tbCode.Text;
        targetRegistrationFee.Name = tbName.Text;

        targetRegistrationFee.Price = decimal.Parse(tbRegularPrice.Text);

        if (string.IsNullOrWhiteSpace(tbPreRegPrice.Text))
            targetRegistrationFee.PreRegistrationPrice = null;
        else
            targetRegistrationFee.PreRegistrationPrice = decimal.Parse(tbPreRegPrice.Text);
        if (string.IsNullOrWhiteSpace(tbEarlyRegPrice.Text))
            targetRegistrationFee.EarlyRegistrationPrice = null;
        else
            targetRegistrationFee.EarlyRegistrationPrice = decimal.Parse(tbEarlyRegPrice.Text);
        if (string.IsNullOrWhiteSpace(tbLateRegPrice.Text))
            targetRegistrationFee.LateRegistrationPrice = null;
        else
            targetRegistrationFee.LateRegistrationPrice = decimal.Parse(tbLateRegPrice.Text);

        
        targetRegistrationFee.RequiresApproval = chkRequiresApproval.Checked;
        targetRegistrationFee.IsGuestRegistration = chkIsGuestRegistration.Checked;

        targetRegistrationFee.ConfirmationEmail = ddlConfirmationEmail.SelectedValue;
        targetRegistrationFee.RegistrantClass = ddlRegistrantClass.SelectedValue;
        targetRegistrationFee.RegistrantCategory = ddlRegistrantCategory.SelectedValue;

        targetRegistrationFee.Description = reDescription.Content;

        targetRegistrationFee.SellOnline = chkSellOnline.Checked;
        targetRegistrationFee.AllowCustomersToPayLater = chkAllowCustomersToPayLater.Checked;
        targetRegistrationFee.DisplayPriceAs = tbDisplayPriceAs.Text;

        targetRegistrationFee.SellFrom = dtpSellFrom.SelectedDate;
        targetRegistrationFee.SellUntil = dtpSellUntil.SelectedDate;
        targetRegistrationFee.NewUntil = dtpNewUntil.SelectedDate;
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