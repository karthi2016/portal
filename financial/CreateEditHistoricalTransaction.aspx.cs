using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class financial_CreateEditHistoricalTransaction : PortalPage
{
    #region Fields

    protected msHistoricalTransaction targetHistoricalTransaction;
    protected msOrder targetOrder;

    protected ClassMetadata historicalTransactionClassMetadata;
    protected Dictionary<string, FieldMetadata> historicalTransactionFieldMetadata;

    protected msEvent targetEvent;
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    #endregion

    #region Properties

    protected string CompleteUrl
    {
        get { return Request.QueryString["completeUrl"]; }
    }

    protected string EventId
    {
        get { return Request.QueryString["eventId"]; }
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

        //Describe a Historical Transaction.  We will need this metadata to bind to the acceptable values for certain fields and for creating a new event
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            historicalTransactionClassMetadata = proxy.DescribeObject(msHistoricalTransaction.CLASS_NAME).ResultValue;
            historicalTransactionFieldMetadata = historicalTransactionClassMetadata.GenerateFieldDictionary();
        }

        MemberSuiteObject contextObject = LoadObjectFromAPI(ContextID);
        if(contextObject == null)
        {
            GoToMissingRecordPage();
            return;
        }

        switch (contextObject.ClassType)
        {
            case msOrder.CLASS_NAME:
                targetOrder = contextObject.ConvertTo<msOrder>();
                targetHistoricalTransaction =
                    msHistoricalTransaction.FromClassMetadata(historicalTransactionClassMetadata);
                targetHistoricalTransaction.Type = HistoricalTransactionType.Payment;
                break;
            case msHistoricalTransaction.CLASS_NAME:
                targetHistoricalTransaction = contextObject.ConvertTo<msHistoricalTransaction>();
                targetOrder = LoadObjectFromAPI<msOrder>(targetHistoricalTransaction.Order);
                break;
            default:
                QueueBannerError(string.Format("Unknown context object type supplied '{0}'.", contextObject.ClassType));
                GoHome();
                return;
        }

        targetEvent = LoadObjectFromAPI<msEvent>(EventId);

        if(targetOrder == null || targetHistoricalTransaction == null || targetEvent == null)
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
    protected override void  InitializePage()
    {
        base.InitializePage();

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");

        ddlType.DataSource = historicalTransactionFieldMetadata["Type"].PickListEntries;
        ddlType.DataBind();

        bindHistoricalTransaction();
    }


    /// <summary>
    /// Checks to make sure that this page is being access properly.
    /// </summary>
    /// <returns></returns>
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

        hlEventOwnerTask.Text = string.Format("Back to Manage {0} Events", ownerName);
        hlEventOwnerTask.NavigateUrl = string.Format("{0}?contextID={1}", manageEventsUrl, ownerId);
        liEventOwnerTask.Visible = true;
    }

    protected void GoToNextUrl(string message)
    {
        if (!string.IsNullOrWhiteSpace(CompleteUrl))
            GoTo(CompleteUrl, message);

        GoHome(message);
    }

        protected void GoToNextUrl()
    {
        if (!string.IsNullOrWhiteSpace(CompleteUrl))
            GoTo(CompleteUrl);

        GoHome();
    }

    #endregion

    #region Data Binding

    protected void bindHistoricalTransaction()
    {
        tbName.Text = targetHistoricalTransaction.Name;

        if(targetHistoricalTransaction.Date != DateTime.MinValue)
            dtpDate.SelectedDate = targetHistoricalTransaction.Date;
        ddlType.SelectedValue = targetHistoricalTransaction.Type.ToString("D");
        tbReferenceNumber.Text = targetHistoricalTransaction.ReferenceNumber;
        tbTotal.Text = string.Format("{0:F}",targetHistoricalTransaction.Total);
        tbMemo.Text = targetHistoricalTransaction.Memo;
        tbNotes.Text = targetHistoricalTransaction.Notes;
    }

    protected void unbindHistoricalTransaction()
    {
        targetHistoricalTransaction.Name = tbName.Text;

        if(dtpDate.SelectedDate.HasValue)
            targetHistoricalTransaction.Date = dtpDate.SelectedDate.Value;

        targetHistoricalTransaction.Type = (HistoricalTransactionType)Enum.Parse(typeof(HistoricalTransactionType), ddlType.SelectedValue);
        targetHistoricalTransaction.ReferenceNumber = tbReferenceNumber.Text;
        targetHistoricalTransaction.Total = decimal.Parse(tbTotal.Text);
        targetHistoricalTransaction.Memo = tbMemo.Text;
        targetHistoricalTransaction.Notes = tbNotes.Text;

        if (string.IsNullOrWhiteSpace(targetHistoricalTransaction.Owner))
            targetHistoricalTransaction.Owner = targetOrder.BillTo;

        if(string.IsNullOrWhiteSpace(targetHistoricalTransaction.Order))
            targetHistoricalTransaction.Order = targetOrder.ID;
    }

    #endregion

    #region Event Handlers

    protected void lbDeleteHistoricalTransaction_Click(object sender, EventArgs e)
    {
        using(IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            proxy.Delete(targetHistoricalTransaction.ID);
        }

        GoToNextUrl("Historical Transaction was deleted successfully.");
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        unbindHistoricalTransaction();
        targetHistoricalTransaction = SaveObject(targetHistoricalTransaction).ConvertTo<msHistoricalTransaction>();
        
        GoToNextUrl("Historical Transaction was saved successfully.");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoToNextUrl();
    }

    #endregion
}