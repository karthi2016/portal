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

public partial class events_CreateEditConfirmationEmail : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msEmailTemplateContainer targetEmailTemplateContainer;
    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    protected ClassMetadata emailTemplateContainerClassMetadata;
    protected Dictionary<string, FieldMetadata> emailTemplateContainerFieldMetadata;

    protected List<NameValueStringPair> mergeFields;

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
            emailTemplateContainerClassMetadata = proxy.DescribeObject(msEmailTemplateContainer.CLASS_NAME).ResultValue;
            emailTemplateContainerFieldMetadata = emailTemplateContainerClassMetadata.GenerateFieldDictionary();
        }

        MemberSuiteObject contextObject = LoadObjectFromAPI(ContextID);

        if (contextObject.ClassType == msEvent.CLASS_NAME)
        {
            targetEvent = contextObject.ConvertTo<msEvent>();
            targetEmailTemplateContainer = msEmailTemplateContainer.FromClassMetadata(emailTemplateContainerClassMetadata);
            targetEmailTemplateContainer.Template = new EmailTemplate { SearchType = "EventRegistration" };
            lblTitleAction.Text = "Create";
        }
        else
        {
            targetEmailTemplateContainer = contextObject.ConvertTo<msEmailTemplateContainer>();
            targetEvent = LoadObjectFromAPI<msEvent>(targetEmailTemplateContainer.Context);
            if (targetEmailTemplateContainer.Template == null)
                targetEmailTemplateContainer.Template = new EmailTemplate{SearchType = "EventRegistration"};
            lblTitleAction.Text = "Edit";
        }

        if (targetEvent == null || targetEmailTemplateContainer == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEmailTemplateContainer.Context = targetEmailTemplateContainer.Template.SearchContext = targetEvent.ID;

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

        bindEmailTemplateContainer();

        loadDataFromConcierge();

        ddlMergeFields.DataSource = mergeFields;
        ddlMergeFields.DataBind();
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
        //Load merge fields
        mergeFields = new List<NameValueStringPair>();

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            List<FieldMetadata> fields = proxy.DescribeSearch("EventRegistration", targetEvent.ID).ResultValue.Fields;


            foreach (var f in fields)
            {
                if (f.DataType != FieldDataType.Reference) // add it
                {
                    mergeFields.Add(new NameValueStringPair(f.Label, f.Name));
                    continue;
                }

                if (f.ReferenceType == null) continue;

                // we have a reference field - let's look it up!
                var referenceFieldSubfields = proxy.DescribeSearch(f.ReferenceType, null).ResultValue.Fields;

                foreach (var sf in referenceFieldSubfields)
                {
                    string fieldLabel = string.Format("{0} -> {1}", f.Label, sf.Label);
                    string fieldName = string.Format("{0}.{1}", f.Name, sf.Name);
                    if (sf.DataType == FieldDataType.Reference) // IT'S a reference field - so we'll just add the name
                    {
                        fieldLabel += " -> Name";
                        fieldName += ".Name";
                    }

                    mergeFields.Add(new NameValueStringPair(fieldLabel, fieldName));
                }
            }
        }

        mergeFields.Sort((x, y) => string.Compare(x.Name, y.Name));
        mergeFields.Insert(0, new NameValueStringPair("--- Select a Merge Field ---", ""));
    }

    protected void unbindAndSave()
    {
        unbindEventDiscountCode();
        targetEmailTemplateContainer = SaveObject(targetEmailTemplateContainer).ConvertTo<msEmailTemplateContainer>();
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

    protected void bindEmailTemplateContainer()
    {
        tbName.Text = targetEmailTemplateContainer.Template.Name;
        ddlTemplateTarget.SelectedValue = targetEmailTemplateContainer.Template.SearchType;
        tbDescription.Text = targetEmailTemplateContainer.Description;

        tbFrom.Text = targetEmailTemplateContainer.Template.FromName;
        tbSubject.Text = targetEmailTemplateContainer.Template.Subject;
        tbCC.Text = targetEmailTemplateContainer.Template.Cc;
        tbBCC.Text = targetEmailTemplateContainer.Template.Bcc;

        reHtmlMessageBody.Content = targetEmailTemplateContainer.Template.HtmlBody;

        tbTextOnlyMessageBody.Text = targetEmailTemplateContainer.Template.TextBody;
    }

    protected void unbindEventDiscountCode()
    {
        targetEmailTemplateContainer.Name = targetEmailTemplateContainer.Template.Name = tbName.Text;
        targetEmailTemplateContainer.Template.SearchType = ddlTemplateTarget.SelectedValue;
        targetEmailTemplateContainer.Description = tbDescription.Text;

        targetEmailTemplateContainer.Template.FromName = tbFrom.Text;
        targetEmailTemplateContainer.Template.Subject = tbSubject.Text;
        targetEmailTemplateContainer.Template.Cc = tbCC.Text;
        targetEmailTemplateContainer.Template.Bcc = tbBCC.Text;

        targetEmailTemplateContainer.Template.HtmlBody = reHtmlMessageBody.Content;

        targetEmailTemplateContainer.Template.TextBody = tbTextOnlyMessageBody.Text;
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