using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Resource;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;
using MemberSuite.SDK.Web.Controls.CascadingDropDown;

public partial class events_CreateEditRegistrationQuestion : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msCustomField targetCustomField;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected DataView dvLookupTables;
    protected List<PartialObjectDefinition> allTypes;

    protected ClassMetadata customFieldClassMetadata;
    protected Dictionary<string, FieldMetadata> customFieldFieldMetadata;

    protected List<NameValuePair> displayTypes;
    protected List<NameValuePair> dataTypes;

    private bool isInEditMode;

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

        //Describe an Event Discount Code.  We will need this metadata to bind to the acceptable values for certain fields and for creating a new Discount / Promo Code
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            customFieldClassMetadata = proxy.DescribeObject(msCustomField.CLASS_NAME).ResultValue;
            customFieldFieldMetadata = customFieldClassMetadata.GenerateFieldDictionary();
        }

        MemberSuiteObject contextObject = LoadObjectFromAPI(ContextID);

        if (contextObject.ClassType == msEvent.CLASS_NAME)
        {
            targetEvent = contextObject.ConvertTo<msEvent>();
            targetCustomField = msCustomField.FromClassMetadata(customFieldClassMetadata);
            targetCustomField.FieldDefinition = new FieldMetadata();
            lblTitleAction.Text = "Create";
            isInEditMode = false;
        }
        else
        {
            targetCustomField = contextObject.ConvertTo<msCustomField>();
            targetEvent = LoadObjectFromAPI<msEvent>(targetCustomField.Event);
            if (targetCustomField.FieldDefinition == null)
                targetCustomField.FieldDefinition = new FieldMetadata();
            lblTitleAction.Text = "Edit";
            isInEditMode = true;
        }

        if (targetEvent == null || targetCustomField == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetCustomField.Event = targetEvent.ID;
        targetCustomField.ApplicableType = "DEMOGRAPHIC";
        targetCustomField.Type = CustomFieldType.EventRegistration;

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

        lblEventName.Text = targetEvent.Name;

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");

        loadDataFromConcierge();

        buildDataTypes();
        ddlDataType.DataSource = dataTypes;
        ddlDataType.DataBind();

        buildDisplayTypes();
        ddlDisplayType.DataSource = displayTypes;
        ddlDisplayType.DataBind();

        ddlReferenceType.DataSource = allTypes;
        ddlReferenceType.DataBind();

        ddlLookupTables.DataSource = dvLookupTables;
        ddlLookupTables.DataBind();

        bindCustomField();

        setupShowHidePanelsScripts();
        setupDisplayDataCascadingDropDown();
        setupLookupTableScripts();

        ddlDataType.Enabled = tbApiName.Enabled = !isInEditMode;
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

    protected void loadDataFromConcierge()
    {
        //Load Lookup Tables
        Search sLookupTable = new Search(msLookupTable.CLASS_NAME);
        sLookupTable.AddOutputColumn("ID");
        sLookupTable.AddOutputColumn("Name");
        sLookupTable.AddSortColumn("Name");

        SearchResult srLookupTable = ExecuteSearch(sLookupTable, 0, null);
        dvLookupTables = new DataView(srLookupTable.Table);

        //Load Reference Types
        using (IConciergeAPIService api = GetServiceAPIProxy())
        {
            allTypes = api.ListAllObjects(false, msAggregate.CLASS_NAME).ResultValue;
        }
    }

    protected void buildDisplayTypes()
    {
        displayTypes = new List<NameValuePair>();

        foreach (var name in Enum.GetNames(typeof(FieldDisplayType)))
            displayTypes.Add(new NameValuePair(ResolveResource(("FieldDisplayType." + name)), name));

        displayTypes.RemoveAll(n => (string)n.Value == FieldDisplayType.Label.ToString());    // pull this out
    }

    protected void buildDataTypes()
    {
        dataTypes = new List<NameValuePair>();

        foreach (var name in Enum.GetNames(typeof(FieldDataType)))
            dataTypes.Add(new NameValuePair(ResolveResource(("FieldDataType." + name)), name));

        dataTypes.RemoveAll(n => (string)n.Value == FieldDataType.Timestamp.ToString());    // pull this out
        dataTypes.RemoveAll(n => (string)n.Value == FieldDataType.Money.ToString());    // pull this out
        dataTypes.RemoveAll(n => (string)n.Value == FieldDataType.Address.ToString());    // pull this out
        dataTypes.RemoveAll(n => (string)n.Value == FieldDataType.Metadata.ToString());    // pull this out
        dataTypes.RemoveAll(n => (string)n.Value == FieldDataType.ListOfObjects.ToString());    // pull this out
        dataTypes.RemoveAll(n => (string)n.Value == FieldDataType.Type.ToString());    // pull this out
        dataTypes.RemoveAll(n => (string)n.Value == FieldDataType.Binary.ToString());    // pull this out
        dataTypes.RemoveAll(n => (string)n.Value == FieldDataType.Enum.ToString());    // pull this out
    }

    protected void setupDisplayDataCascadingDropDown()
    {
        // now, let's set up the cascading
        CascadingPair pair = new CascadingPair() { ParentDropDownID = ddlDataType.ID, ChildDropDownID = ddlDisplayType.ID };

        // and let's apply the dependencies properly
        foreach (ListItem dataType in ddlDataType.Items)
        {
            var dependentDisplayTypes =
                FieldMappings.GetDependentDisplayTypesFor(dataType.Value.ToEnum<FieldDataType>());
            ParentDropDownValue v = new ParentDropDownValue() { Value = dataType.Value };
            foreach (var dependentDisplayType in dependentDisplayTypes)
                v.ChildDropDownValues.Add(new ChildDropDownValue() { Value = dependentDisplayType.ToString() });

            pair.ParentDropDownValues.Add(v);
        }
        CascadingDropDownManager1.CascadingPairs.Add(pair);
    }

    protected void setupShowHidePanelsScripts()
    {
        var scriptLocation = "MemberSuite.SDK.Web.Controls.ShowHideFieldPanels.js";
        string script = EmbeddedResource.LoadAsString(scriptLocation, Assembly.GetAssembly(typeof(CascadingDropDownManager)));

        script = script.Replace("%displayTypeDropDownID%", ddlDisplayType.ClientID).Replace("%dataTypeDropDownID%", ddlDataType.ClientID);
        ClientScript.RegisterClientScriptBlock(typeof(string), "onchangeScript", script, true);
        ClientScript.RegisterStartupScript(typeof(string), "showValues", "<script>showAppropriatePanelsForDisplayType(); </script>");

        ddlDataType.Attributes["onchange"] += "showAppropriatePanelsForDisplayType(  );";
        ddlDisplayType.Attributes["onchange"] += "showAppropriatePanelsForDisplayType(  );";
    }

    protected void setupLookupTableScripts()
    {
        ddlLookupTables.Attributes["onchange"] += "SetLookupTableAjaxComboBox();";
        ddlLookupTables.Attributes["onload"] += "SetLookupTableAjaxComboBox();";
    }

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

    protected void unbindAndSave()
    {
        unbindCustomField();
        targetCustomField = SaveObject(targetCustomField).ConvertTo<msCustomField>();
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

    protected void bindCustomField()
    {
        tbFieldLabel.Text = targetCustomField.FieldDefinition.Label;
        ddlDataType.SelectedValue = targetCustomField.FieldDefinition.DataType.ToString();
        ddlDisplayType.SelectedValue = targetCustomField.FieldDefinition.DisplayType.ToString();

        tbApiName.Text = targetCustomField.Name;
        chkRequired.Checked = targetCustomField.FieldDefinition.IsRequired;
        tbDefaultValue.Text = targetCustomField.FieldDefinition.DefaultValue;

        tbAcceptableValues.Text = targetCustomField.FieldDefinition.ToPickListEntriesString();
        ddlLookupTables.SelectedValue = targetCustomField.FieldDefinition.LookupTableID;

        ddlReferenceType.SelectedValue = targetCustomField.FieldDefinition.ReferenceType;

        tbDisplayOrder.Text = targetCustomField.DisplayOrder.ToString();
        chkRequiredInPortal.Checked = targetCustomField.FieldDefinition.IsRequiredInPortal;
        tbHelpText.Text = targetCustomField.FieldDefinition.HelpText;
    }

    protected void unbindCustomField()
    {
        targetCustomField.FieldDefinition.Label = targetCustomField.FieldDefinition.PortalPrompt = tbFieldLabel.Text;
        targetCustomField.FieldDefinition.DataType =
            (FieldDataType) Enum.Parse(typeof (FieldDataType), ddlDataType.SelectedValue);
        targetCustomField.FieldDefinition.DisplayType =
            (FieldDisplayType) Enum.Parse(typeof (FieldDisplayType), ddlDisplayType.SelectedValue);

        targetCustomField.Name = string.IsNullOrWhiteSpace(tbApiName.Text)
                                     ? Formats.GetSafeFieldName(tbFieldLabel.Text)
                                     : tbApiName.Text;
        targetCustomField.FieldDefinition.IsRequired = chkRequired.Checked;
        targetCustomField.FieldDefinition.DefaultValue = tbDefaultValue.Text;

        targetCustomField.FieldDefinition.PickListEntries = FieldMetadata.Parse(tbAcceptableValues.Text);
        targetCustomField.FieldDefinition.LookupTableID = ddlLookupTables.SelectedValue == "0" ? null : ddlLookupTables.SelectedValue;

        targetCustomField.FieldDefinition.ReferenceType = ddlReferenceType.SelectedValue;

        targetCustomField.DisplayOrder = int.Parse(tbDisplayOrder.Text);
        targetCustomField.FieldDefinition.IsRequiredInPortal = chkRequiredInPortal.Checked;
        targetCustomField.FieldDefinition.HelpText = tbHelpText.Text;
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