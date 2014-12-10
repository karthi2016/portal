using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Command.Views;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;

public partial class events_ViewEventRegistration : PortalPage
{
    #region Fields

    protected msEventRegistration targetRegistration;
    protected List<ControlMetadata> targetRegistrationFields;
    protected msEntity targetEntity;
    protected msOrderLineItem targetOrderLineItem;
    protected msUser createdBy;

    protected msEvent targetEvent;
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected DataRow drOrder;

    protected DataView dvPayments;
    protected DataView dvHistoricalTransactions;
    protected DataView dvOrderLineItems;
    protected string status;

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

        targetRegistration = LoadObjectFromAPI<msEventRegistration>(ContextID);

        if (targetRegistration == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEvent = LoadObjectFromAPI<msEvent>(targetRegistration.Event);
        targetEntity = LoadObjectFromAPI<msEntity>(targetRegistration.Owner);

        if (targetEvent == null || targetEntity == null)
        {
            GoToMissingRecordPage();
            return;
        }

        initializeRegistrationFields();

        createdBy = LoadObjectFromAPI<msUser>(targetRegistration.CreatedBy);
        status = getStatus();

        loadEventOwners();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        //If this is done in InitializePage instead of Page_Load then drOrder would be null on postbacks like Cancel or Delete
        loadDataFromConcierge();

        setLeaderTaskVisibility();

        if (!IsPostBack)
        {
            loadAndBind();

            divPayments.Visible = string.IsNullOrWhiteSpace(targetEvent.MerchantAccount) || dvPayments.Count > 0;
            divHistoricalTransactions.Visible = !string.IsNullOrWhiteSpace(targetEvent.MerchantAccount) || dvHistoricalTransactions.Count > 0;
            divAddHistoricalTransaction.Visible = divHistoricalTransactions.Visible && !string.IsNullOrWhiteSpace(targetRegistration.Order);
            hlPrintAgenda.NavigateUrl = string.Format("{0}/events/registrations/agenda/print?a={1}&r={2}",
                                                      ConfigurationManager.AppSettings["ImageServerUri"],
                                                      ConciergeAPI.CurrentAssociation.PartitionKey,
                                                      targetRegistration.ID);
        }


    }

    protected override void InstantiateCustomFields(MemberSuite.SDK.Concierge.IConciergeAPIService proxy)
    {
        base.InstantiateCustomFields(proxy);

        //Dynamically render the registration fields
        cfsRegistrationFields.Metadata = createClassMetadataFromRegistrationFields();
        cfsRegistrationFields.PageLayout = createViewMetadataFromRegistrationFields();
        if (cfsRegistrationFields.PageLayout == null || cfsRegistrationFields.PageLayout.IsEmpty())
            return;

        cfsRegistrationFields.MemberSuiteObject = targetRegistration;

        cfsRegistrationFields.AddReferenceNamesToTargetObject(proxy);

        cfsRegistrationFields.Render();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return hasPermission();
    }

    #endregion

    #region Methods

    protected void loadAndBind()
    {
        loadDataFromConcierge();

        //Bind the payments
        gvPayments.DataSource = dvPayments;
        gvPayments.DataBind();

        //Bind the historical transaction
        gvHistoricalTransactions.DataSource = dvHistoricalTransactions;
        gvHistoricalTransactions.DataBind();
    }

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();

        //Search for the order
        Search sOrder = new Search { Type = msOrder.CLASS_NAME };
        sOrder.AddOutputColumn("BalanceDue");
        sOrder.AddCriteria(Expr.Equals("ID", targetRegistration.Order));
        sOrder.AddSortColumn("ID");
        searches.Add(sOrder);

        //Search for payments
        Search sPayments = new Search { Type = msPaymentLineItem.CLASS_NAME };
        sPayments.AddOutputColumn("Payment.ID");
        sPayments.AddOutputColumn("Payment.Name");
        sPayments.AddOutputColumn("Payment.Date");
        sPayments.AddOutputColumn("Amount");
        sPayments.AddCriteria(Expr.Equals("Invoice.Order", targetRegistration.Order));
        sPayments.AddSortColumn("Payment.ID");
        sPayments.AddSortColumn("Payment.Date");
        searches.Add(sPayments);

        //Search for Historical Transactions
        Search sHistoricalTransactions = new Search(msHistoricalTransaction.CLASS_NAME);
        sHistoricalTransactions.AddOutputColumn("ID");
        sHistoricalTransactions.AddOutputColumn("Name");
        sHistoricalTransactions.AddOutputColumn("Date");
        sHistoricalTransactions.AddOutputColumn("Type");
        sHistoricalTransactions.AddOutputColumn("Total");
        sHistoricalTransactions.AddCriteria(Expr.Equals("Order", targetRegistration.Order));
        sHistoricalTransactions.AddSortColumn("Date");
        searches.Add(sHistoricalTransactions);

        List<SearchResult> searchResults = ExecuteSearches(searches, 0, null);
        if (searchResults[0].Table.Rows.Count > 0)
            drOrder = searchResults[0].Table.Rows[0];

        dvPayments = new DataView(searchResults[1].Table);
        dvHistoricalTransactions = new DataView(searchResults[2].Table);
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

    protected void setLeaderTaskVisibility()
    {
        liEditRegistration.Visible = isLeader();
        liCancelRegistration.Visible = !targetRegistration.CancellationDate.HasValue && isLeader();
        liDeleteRegistration.Visible = targetRegistration.CancellationDate.HasValue && isLeader();
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

    protected bool hasPermission()
    {
        return ConciergeAPI.CurrentEntity.ID == targetRegistration.Owner || isLeader();
    }

    protected bool isLeader()
    {
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

    protected void initializeRegistrationFields()
    {
        using (var api = GetServiceAPIProxy())
        {
            targetRegistrationFields = api.GetOrderFormForProduct(targetRegistration.Fee).ResultValue;
        }

        foreach (var field in targetRegistrationFields)
        {
            field.DataSource = Formats.GetSafeFieldName(field.DataSource); // rename the data source
            field.DataSourceExpression = field.Name; // set the name
        }
    }

    protected ClassMetadata createClassMetadataFromRegistrationFields()
    {
        ClassMetadata result = new ClassMetadata();
        foreach (var targetRegistrationField in targetRegistrationFields)
        {
            FieldMetadata fieldMetadata = new FieldMetadata();
            fieldMetadata.DisplayType = targetRegistrationField.DisplayType.HasValue
                                            ? targetRegistrationField.DisplayType.Value
                                            : FieldDisplayType.TextBox;

            fieldMetadata.PortalPrompt = targetRegistrationField.PortalPrompt;
            fieldMetadata.HelpText = targetRegistrationField.HelpText;
            fieldMetadata.PortalAccessibility = PortalAccessibility.ReadOnly;
            fieldMetadata.Name = Formats.GetSafeFieldName(targetRegistrationField.Name);
            result.Fields.Add(fieldMetadata);
        }
        return result;
    }

    protected DataEntryViewMetadata createViewMetadataFromRegistrationFields()
    {
        DataEntryViewMetadata result = new DataEntryViewMetadata();
        ViewMetadata.ControlSection baseSection = new ViewMetadata.ControlSection();
        baseSection.SubSections = new List<ViewMetadata.ControlSection>();

        var currentSection = new ViewMetadata.ControlSection();
        currentSection.LeftControls = new List<ControlMetadata>();
        baseSection.SubSections.Add(currentSection);
        foreach (var field in targetRegistrationFields)
        {
            if (field.DisplayType == FieldDisplayType.Separator)
            {
                if (currentSection.LeftControls != null && currentSection.LeftControls.Count > 0)
                // we have to create a new section
                {
                    currentSection = new ViewMetadata.ControlSection();
                    currentSection.LeftControls = new List<ControlMetadata>();
                    baseSection.SubSections.Add(currentSection);
                }

                currentSection.Name = Guid.NewGuid().ToString();
                currentSection.Label = field.PortalPrompt ?? field.Label; // set the section label
                continue;
            }


            currentSection.LeftControls.Add(field);
        }
        result.Sections = new List<ViewMetadata.ControlSection> { baseSection };
        return result;
    }

    protected string getStatus()
    {
        if (targetRegistration.CancellationDate.HasValue)
            return "Cancelled";

        if (targetRegistration.OnWaitList)
            return "On Wait List";

        if (!targetRegistration.Approved)
            return "Pending";

        if (drOrder != null && (decimal)drOrder["BalanceDue"] > 0)
            return "Pending";

        return "Active";
    }

    #endregion

    #region Event Handlers

    protected void lbCancelRegistration_Click(object sender, EventArgs e)
    {
        targetRegistration.CancellationDate = DateTime.UtcNow;
        targetRegistration = SaveObject(targetRegistration).ConvertTo<msEventRegistration>();
    }

    protected void lbDeleteRegistration_Click(object sender, EventArgs e)
    {
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            proxy.Delete(targetRegistration.ID);
            GoHome(string.Format("Registration {0} was deleted successfully.", targetRegistration.LocalID));
        }
    }

    protected void gvHistoricalTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                e.Row.Cells[3].Visible = isLeader();
                e.Row.Cells[4].Visible = isLeader();
                e.Row.Cells[5].Visible = isLeader();
                break;
        }
    }

    protected void gvHistoricalTransactions_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToLower())
        {
            case "deletehistoricaltransaction":
                using (IConciergeAPIService serviceProxy = GetServiceAPIProxy())
                {
                    serviceProxy.Delete(e.CommandArgument.ToString());
                }
                Refresh();
                QueueBannerMessage("Historical Transaction deleted successfully.");
                break;
        }
    }

    #endregion
}