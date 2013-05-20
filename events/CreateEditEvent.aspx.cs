using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;


public partial class events_CreateEditEvent : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected DataView dvRegistrationFees;
    protected DataView dvRegistrationQuestions;
    protected DataView dvPromoCodes;
    protected DataView dvInformationLinks;
    protected DataView dvWaivedRegistrationLists;
    protected DataView dvConfirmationEmails;
    protected DataView dvEventMerchandise;
    protected DataView dvExternalMerchantAccounts;

    protected ClassMetadata eventClassMetadata;
    protected Dictionary<string, FieldMetadata> eventFieldMetadata;

    #endregion

    #region Properties

    public string Owner
    {
        get { return Request.QueryString["owner"]; }
    }

    public bool IsInEditMode { get; set; }

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

        //Describe an event.  We will need this metadata to bind to the acceptable values for certain fields and for creating a new event
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            eventClassMetadata = proxy.DescribeObject(msEvent.CLASS_NAME).ResultValue;
            eventFieldMetadata = eventClassMetadata.GenerateFieldDictionary();
        }

        targetEvent = msEvent.FromClassMetadata(eventClassMetadata);

        if (!string.IsNullOrWhiteSpace(ContextID))
        {
            //Load the context object and determine the owner from the class type
            MemberSuiteObject contextobject = LoadObjectFromAPI(ContextID);
            if (contextobject == null)
            {
                GoToMissingRecordPage();
                return;
            }


            switch (contextobject.ClassType)
            {
                case msChapter.CLASS_NAME:
                    targetChapter = contextobject.ConvertTo<msChapter>();
                    targetEvent.Chapter = ContextID;
                    break;
                case msSection.CLASS_NAME:
                    targetSection = contextobject.ConvertTo<msSection>();
                    targetEvent.Section = ContextID;
                    break;
                case msOrganizationalLayer.CLASS_NAME:
                    targetOrganizationalLayer = contextobject.ConvertTo<msOrganizationalLayer>();
                    targetEvent.OrganizationalLayer = ContextID;
                    break;
                case msEvent.CLASS_NAME:
                    targetEvent = LoadObjectFromAPI<msEvent>(ContextID);
                    IsInEditMode = true;
                    if (targetEvent == null)
                    {
                        GoToMissingRecordPage();
                        return;
                    }
                    break;
                default:
                    QueueBannerError(string.Format("Invalid context object type specified '{0}'",
                                                   contextobject.ClassType));
                    GoHome();
                    return;
            }
        }

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

        ddlCategory.DataSource = eventFieldMetadata["Category"].PickListEntries;
        ddlCategory.DataBind();

        ddlConfirmationEmail.DataSource = eventFieldMetadata["ConfirmationEmail"].PickListEntries;
        ddlConfirmationEmail.DataBind();

        ddlRegistrationMode.DataSource = eventFieldMetadata["RegistrationMode"].PickListEntries;
        ddlRegistrationMode.DataBind();
 

        ddlType.DataSource = eventFieldMetadata["Type"].PickListEntries;
        ddlType.DataBind();


        lblRegistrationFeeSaveFirst.Visible =
            lblRegistrationQuestionSaveFirst.Visible =
            lblPromoCodeSaveFirst.Visible =
            lblInformationLinkSaveFirst.Visible =
            lblWaivedRegistrationListSaveFirst.Visible = 
            lblConfirmationEmailSaveFirst.Visible = 
            lblEventMerchandiseSaveFirst.Visible =
            !IsInEditMode;

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");

        if (!IsInEditMode)
        {
            lblTitleAction.Text = "Create";
            return;
        }

        lblTitleAction.Text = "Edit";

        //In edit mode so event already exists and we can now display related items
        loadRelatedRecordsAndBind();

        bindEvent();
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

    protected void loadRelatedRecordsAndBind()
    {
        loadDataFromConcierge();

        gvRegistrationFees.DataSource = dvRegistrationFees;
        gvRegistrationFees.DataBind();

        gvRegistrationQuestions.DataSource = dvRegistrationQuestions;
        gvRegistrationQuestions.DataBind();

        gvPromoCodes.DataSource = dvPromoCodes;
        gvPromoCodes.DataBind();

        gvInformationLinks.DataSource = dvInformationLinks;
        gvInformationLinks.DataBind();

        gvWaivedRegistrationLists.DataSource = dvWaivedRegistrationLists;
        gvWaivedRegistrationLists.DataBind();

        gvConfirmationEmails.DataSource = dvConfirmationEmails;
        gvConfirmationEmails.DataBind();

        gvEventMerchandise.DataSource = dvEventMerchandise;
        gvEventMerchandise.DataBind();

        ddlMerchant.DataSource = dvExternalMerchantAccounts;
        ddlMerchant.DataBind();
    }

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();

        //Load Registration Fees
        Search sFees = new Search(msRegistrationFee.CLASS_NAME);
        sFees.AddOutputColumn("ID");
        sFees.AddOutputColumn("DisplayOrder");
        sFees.AddOutputColumn("Code");
        sFees.AddOutputColumn("Name");
        sFees.AddOutputColumn("Price");
        sFees.AddOutputColumn("IsActive");
        sFees.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        sFees.AddSortColumn("DisplayOrder");
        sFees.AddSortColumn("Name");
        sFees.AddSortColumn("Code");

        searches.Add(sFees);

        //Load Registration Questions
        Search sQuestions = new Search(msCustomField.CLASS_NAME);
        sQuestions.AddOutputColumn("ID");
        sQuestions.AddOutputColumn("DisplayOrder");
        sQuestions.AddOutputColumn("Label");
        sQuestions.AddOutputColumn("DataType");
        sQuestions.AddOutputColumn("DisplayType");
        sQuestions.AddOutputColumn("IsRequired");
        sQuestions.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        sQuestions.AddSortColumn("DisplayOrder");
        sQuestions.AddSortColumn("Label");

        searches.Add(sQuestions);

        //Load Discounts/Promo Codes
        Search sPromoCodes = new Search(msEventDiscountCode.CLASS_NAME);
        sPromoCodes.AddOutputColumn("ID");
        sPromoCodes.AddOutputColumn("Name");
        sPromoCodes.AddOutputColumn("Code");
        sPromoCodes.AddOutputColumn("IsActive");
        sPromoCodes.AddOutputColumn("ValidUntil");
        sPromoCodes.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        sPromoCodes.AddSortColumn("Name");
        sPromoCodes.AddSortColumn("Code");


        searches.Add(sPromoCodes);

        //Load Supplemental Information Links
        Search sInformationLinks = new Search(msEventInformationLink.CLASS_NAME);
        sInformationLinks.AddOutputColumn("ID");
        sInformationLinks.AddOutputColumn("DisplayOrder");
        sInformationLinks.AddOutputColumn("Name");
        sInformationLinks.AddOutputColumn("Code");
        sInformationLinks.AddOutputColumn("IsActive");
        sInformationLinks.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        sInformationLinks.AddSortColumn("DisplayOrder");
        sInformationLinks.AddSortColumn("Name");
        sInformationLinks.AddSortColumn("Code");

        searches.Add(sInformationLinks);

        //Load Waived Registration Lists
        Search sWaivedRegistrationLists = new Search(msWaivedRegistrationList.CLASS_NAME);
        sWaivedRegistrationLists.AddOutputColumn("ID");
        sWaivedRegistrationLists.AddOutputColumn("Name");
        sWaivedRegistrationLists.AddOutputColumn("MemberCount");
        sWaivedRegistrationLists.AddOutputColumn("RegisteredMemberCount");
        sWaivedRegistrationLists.AddOutputColumn("IsActive");
        sWaivedRegistrationLists.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        sWaivedRegistrationLists.AddSortColumn("Name");

        searches.Add(sWaivedRegistrationLists);

        //Load Confirmation Emails
        Search sConfirmationEmails = new Search(msEmailTemplateContainer.CLASS_NAME);
        sConfirmationEmails.AddOutputColumn("ID");
        sConfirmationEmails.AddOutputColumn("Name");
        sConfirmationEmails.AddOutputColumn("ApplicableType");
        sConfirmationEmails.AddCriteria(Expr.Equals("Context", targetEvent.ID));
        sConfirmationEmails.AddSortColumn("Name");

        searches.Add(sConfirmationEmails);

        //Load Event Merchandise
        Search sEventMerchandise = new Search(msEventMerchandise.CLASS_NAME);
        sEventMerchandise.AddOutputColumn("ID");
        sEventMerchandise.AddOutputColumn("Code");
        sEventMerchandise.AddOutputColumn("Name");
        sEventMerchandise.AddOutputColumn("Price");
        sEventMerchandise.AddCriteria(Expr.Equals("Event", targetEvent.ID));
        sEventMerchandise.AddSortColumn("Name");

        searches.Add(sEventMerchandise);

        //Load External Merchant Accounts
        Search sExternalMerchantAccounts = new Search(msMerchantAccount.CLASS_NAME);
        sExternalMerchantAccounts.AddOutputColumn("ID");
        sExternalMerchantAccounts.AddOutputColumn("Name");
        sExternalMerchantAccounts.AddCriteria(Expr.Equals("IsExternalAccount",true));
        sExternalMerchantAccounts.AddSortColumn("Name");

        searches.Add(sExternalMerchantAccounts);

        List<SearchResult> searchResults = ExecuteSearches(searches, 0, null);

        dvRegistrationFees = new DataView(searchResults[0].Table);
        dvRegistrationQuestions = new DataView(searchResults[1].Table);
        dvPromoCodes = new DataView(searchResults[2].Table);
        dvInformationLinks = new DataView(searchResults[3].Table);
        dvWaivedRegistrationLists = new DataView(searchResults[4].Table);
        dvConfirmationEmails = new DataView(searchResults[5].Table);
        dvEventMerchandise = new DataView(searchResults[6].Table);
        dvExternalMerchantAccounts = new DataView(searchResults[7].Table);
    }

    protected void deleteItem(string id)
    {
        using (IConciergeAPIService serviceProxy = GetServiceAPIProxy())
        {
            serviceProxy.Delete(id);
        }
    }

    protected void unbindAndSaveEvent()
    {
        unbindEvent();
        targetEvent = SaveObject(targetEvent).ConvertTo<msEvent>();
    }

    #endregion

    #region Data Binding

    protected void bindEvent()
    {
        ddlType.SelectedValue = targetEvent.Type;
        tbCode.Text = targetEvent.Code;
        tbName.Text = targetEvent.Name;
        ddlCategory.SelectedValue = targetEvent.Category;
        tbUrl.Text = targetEvent.Url;

        if (targetEvent.StartDate >= new DateTime(1753, 1, 1))
            dtpStartDate.SelectedDate = targetEvent.StartDate;

        if (targetEvent.EndDate >= new DateTime(1753, 1, 1))
            dtpEndDate.SelectedDate = targetEvent.EndDate;

       
        chkClosed.Checked = targetEvent.IsClosed;

        if (!string.IsNullOrWhiteSpace(targetEvent.MerchantAccount))
            ddlMerchant.SelectedValue = targetEvent.MerchantAccount;

        reDescription.Content = targetEvent.Description;

        ddlRegistrationMode.SelectedValue = targetEvent.RegistrationMode.ToString("D");
        chkRequiresApproval.Checked = targetEvent.RequiresApproval;
        tbCapacity.Text = targetEvent.Capacity.HasValue ? targetEvent.Capacity.Value.ToString() : null;
        chkWaitList.Checked = targetEvent.AllowWaitList;
        ddlConfirmationEmail.SelectedValue = targetEvent.ConfirmationEmail;
        tbRegistrationUrl.Text = targetEvent.Url;

        dtpRegistrationOpens.SelectedDate = targetEvent.RegistrationOpenDate;
        dtpRegistrationCloses.SelectedDate = targetEvent.RegistrationCloseDate;
        dtpPreRegistration.SelectedDate = targetEvent.PreRegistrationCutOffDate;
        dtpEarlyRegistration.SelectedDate = targetEvent.EarlyRegistrationCutOffDate;
        dtpRegularRegistration.SelectedDate = targetEvent.RegularRegistrationCutOffDate;
        dtpLateRegistration.SelectedDate = targetEvent.LateRegistrationCutOffDate;

        tbProjectedAttendance.Text = targetEvent.ProjectedAttendance.HasValue ? targetEvent.ProjectedAttendance.Value.ToString() : null;
        tbGuaranteedAttendance.Text = targetEvent.GuaranteedAttendance.HasValue ? targetEvent.GuaranteedAttendance.Value.ToString() : null;

        tbRevenueGoal.Text = string.Format("{0:F}", targetEvent.RevenueGoal);
        tbRegistrationGoal.Text = targetEvent.RegistrationGoal.ToString();
    }

    protected void unbindEvent()
    {
        targetEvent.Type = ddlType.SelectedValue;
        targetEvent.Code = tbCode.Text;
        targetEvent.Name = tbName.Text;
        targetEvent.Category = ddlCategory.SelectedValue;
        targetEvent.Url = tbUrl.Text;

        targetEvent.StartDate = dtpStartDate.SelectedDate.HasValue ? dtpStartDate.SelectedDate.Value : DateTime.MinValue;
        targetEvent.EndDate = dtpEndDate.SelectedDate.HasValue ? dtpEndDate.SelectedDate.Value : DateTime.MinValue;
        
        targetEvent.IsClosed = chkClosed.Checked;
        targetEvent.MerchantAccount = string.IsNullOrWhiteSpace(ddlMerchant.SelectedValue) ? null : ddlMerchant.SelectedValue;

        targetEvent.Description = reDescription.Content;

        targetEvent.RegistrationMode = (EventRegistrationMode)Enum.Parse(typeof(EventRegistrationMode), ddlRegistrationMode.SelectedValue);
        targetEvent.RequiresApproval = chkRequiresApproval.Checked;

        if (string.IsNullOrWhiteSpace(tbCapacity.Text))
            targetEvent.Capacity = null;
        else
            targetEvent.Capacity = int.Parse(tbCapacity.Text);
        targetEvent.AllowWaitList = chkWaitList.Checked;
        targetEvent.ConfirmationEmail = string.IsNullOrWhiteSpace(ddlConfirmationEmail.SelectedValue) ? null : ddlConfirmationEmail.SelectedValue;
        targetEvent.RegistrationUrl = tbRegistrationUrl.Text;

        targetEvent.RegistrationOpenDate = dtpRegistrationOpens.SelectedDate;
        targetEvent.RegistrationCloseDate = dtpRegistrationCloses.SelectedDate;
        targetEvent.PreRegistrationCutOffDate = dtpPreRegistration.SelectedDate;
        targetEvent.EarlyRegistrationCutOffDate = dtpEarlyRegistration.SelectedDate;
        targetEvent.RegularRegistrationCutOffDate = dtpRegularRegistration.SelectedDate;
        targetEvent.LateRegistrationCutOffDate = dtpLateRegistration.SelectedDate;

        if (string.IsNullOrWhiteSpace(tbProjectedAttendance.Text))
            targetEvent.ProjectedAttendance = null;
        else
            targetEvent.ProjectedAttendance = int.Parse(tbProjectedAttendance.Text);
        if (string.IsNullOrWhiteSpace(tbGuaranteedAttendance.Text))
            targetEvent.GuaranteedAttendance = null;
        else
            targetEvent.GuaranteedAttendance = int.Parse(tbGuaranteedAttendance.Text);

        targetEvent.RevenueGoal = !string.IsNullOrWhiteSpace(tbRevenueGoal.Text) ? decimal.Parse(tbRevenueGoal.Text) : 0;
        targetEvent.RegistrationGoal = !string.IsNullOrWhiteSpace(tbRegistrationGoal.Text) ? int.Parse(tbRegistrationGoal.Text) : 0;
    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        unbindAndSaveEvent();
        GoTo(string.Format("~/events/ViewEvent.aspx?ContextID={0}", targetEvent.ID));
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void gvRegistrationFees_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deleteregistrationfee":
                deleteItem(e.CommandArgument.ToString());
                QueueBannerMessage("Registration Fee deleted successfully.");
                Refresh();
                break;
            case "editregistrationfee":
                unbindAndSaveEvent();
                GoTo(string.Format("~/events/CreateEditRegistrationFee.aspx?contextID={0}", e.CommandArgument));
                break;
        }
    }

    protected void lbAddRegistrationFee_Click(object sender, EventArgs e)
    {
        unbindAndSaveEvent();
        GoTo(string.Format("~/events/CreateEditRegistrationFee.aspx?contextID={0}", targetEvent.ID));
    }

    protected void gvRegistrationQuestions_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deleteregistrationquestions":
                deleteItem(e.CommandArgument.ToString());
                QueueBannerMessage("Registration Question deleted successfully.");
                Refresh();
                break;
            case "editregistrationquestions":
                unbindAndSaveEvent();
                GoTo(string.Format("~/events/CreateEditRegistrationQuestion.aspx?contextID={0}", e.CommandArgument));
                break;
        }
    }

    protected void lbAddRegistrationQuestion_Click(object sender, EventArgs e)
    {
        unbindAndSaveEvent();
        GoTo(string.Format("~/events/CreateEditRegistrationQuestion.aspx?contextID={0}", targetEvent.ID));
    }

    protected void gvPromoCodes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deletepromocode":
                deleteItem(e.CommandArgument.ToString());
                QueueBannerMessage("Discount / Promo Code deleted successfully.");
                Refresh();
                break;
            case "editpromocode":
                unbindAndSaveEvent();
                GoTo(string.Format("~/events/CreateEditPromoCode.aspx?contextID={0}", e.CommandArgument));
                break;
        }
    }

    protected void lbAddPromoCode_Click(object sender, EventArgs e)
    {
        unbindAndSaveEvent();
        GoTo(string.Format("~/events/CreateEditPromoCode.aspx?contextID={0}", targetEvent.ID));
    }

    protected void gvInformationLinks_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deleteinformationlink":
                deleteItem(e.CommandArgument.ToString());
                QueueBannerMessage("Supplemental Information Link deleted successfully.");
                Refresh();
                break;
            case "editinformationlink":
                unbindAndSaveEvent();
                GoTo(string.Format("~/events/CreateEditInformationLink.aspx?contextID={0}", e.CommandArgument));
                break;
        }
    }

    protected void lbAddInformationLink_Click(object sender, EventArgs e)
    {
        unbindAndSaveEvent();
        GoTo(string.Format("~/events/CreateEditInformationLink.aspx?contextID={0}", targetEvent.ID));
    }

    protected void gvWaivedRegistrationLists_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deletewaivedregistrationlist":
                deleteItem(e.CommandArgument.ToString());
                QueueBannerMessage("Waived Registration List deleted successfully.");
                Refresh();
                break;
            case "editwaivedregistrationlist":
                unbindAndSaveEvent();
                GoTo(string.Format("~/events/CreateEditWaivedRegistrationList.aspx?contextID={0}", e.CommandArgument));
                break;
        }
    }

    protected void lbAddWaivedRegistrationList_Click(object sender, EventArgs e)
    {
        unbindAndSaveEvent();
        GoTo(string.Format("~/events/CreateEditWaivedRegistrationList.aspx?contextID={0}", targetEvent.ID));
    }

    protected void gvConfirmationEmails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deleteconfirmationemail":
                deleteItem(e.CommandArgument.ToString());
                QueueBannerMessage("Confirmation Email deleted successfully.");
                Refresh();
                break;
            case "editconfirmationemail":
                unbindAndSaveEvent();
                GoTo(string.Format("~/events/CreateEditConfirmationEmail.aspx?contextID={0}", e.CommandArgument));
                break;
        }
    }

    protected void lbAddConfirmationEmail_Click(object sender, EventArgs e)
    {
        unbindAndSaveEvent();
        GoTo(string.Format("~/events/CreateEditConfirmationEmail.aspx?contextID={0}", targetEvent.ID));
    }

    protected void gvEventMerchandise_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deleteeventmerchandise":
                deleteItem(e.CommandArgument.ToString());
                QueueBannerMessage("Event Merchandise deleted successfully.");
                Refresh();
                break;
            case "editeventmerchandise":
                unbindAndSaveEvent();
                GoTo(string.Format("~/events/CreateEditEventMerchandise.aspx?contextID={0}", e.CommandArgument));
                break;
        }
    }

    protected void lbAddEventMerchandise_Click(object sender, EventArgs e)
    {
        unbindAndSaveEvent();
        GoTo(string.Format("~/events/CreateEditEventMerchandise.aspx?contextID={0}", targetEvent.ID));
    }

    #endregion
}
