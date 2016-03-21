using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.ElasticBeanstalk.Model;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Command.Views;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;

public partial class events_Register_RegistrationForm : PortalPage
{
    #region Field

    protected msEvent targetEvent;
    protected msOrder targetOrder;
    protected msRegistrationFee targetRegistrationFee;
    protected List<msOrderLineItem> targetAdditionalItems;
    protected List<ControlMetadata> targetRegistrationFields;

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

        targetRegistrationFee = MultiStepWizards.RegisterForEvent.RegistrationFee;
        targetAdditionalItems = MultiStepWizards.RegisterForEvent.AdditionalLineItems;

        if (MultiStepWizards.RegisterForEvent.Order == null || targetRegistrationFee == null)
        {
            Restart();
            return;
        }

        targetEvent = LoadObjectFromAPI<msEvent>(targetRegistrationFee.Event);

        //Clone the order held in the session
        initializeClonedOrder();

        if(string.IsNullOrWhiteSpace(targetOrder.BillTo) || string.IsNullOrWhiteSpace(targetOrder.ShipTo))
        {
            GoTo(string.Format("~/events/Register_SelectFee.aspx?contextID={0}", ContextID));
            return;
        }

        initializeRegistrationFields();
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

        divAcknowledgement.Visible = cvAcknowledgement.Enabled = !string.IsNullOrWhiteSpace(targetEvent.AcknowledgementText);

        initializeGroupRegistration();
    }

    protected void initializeRegistrationFields()
    {
        using (var api = GetServiceAPIProxy())
        {
            targetRegistrationFields = api.GetOrderForm(targetOrder).ResultValue;
            
        }

        if (targetRegistrationFields != null)
            targetRegistrationFields.RemoveAll(x => (x.IsReadOnly.HasValue && x.IsReadOnly.Value)
                || ! x.Enabled );  // get rid of the read only fields

        if (string.IsNullOrWhiteSpace(targetEvent.AcknowledgementText) && (targetRegistrationFields == null || 
            !targetRegistrationFields.Where( x=>x.DisplayType != FieldDisplayType.Separator ).Any()))
        {
            // go directly the order screen
            GoToOrderForm();
            return;
        }

        //MS-1831
        //The API should have sorted the registration questions by display order per product
        //and put a seperator between the set of questions for each product
        foreach (var field in targetRegistrationFields)
        {
            field.DataSource = Formats.GetSafeFieldName(field.DataSource); // rename the data source
            field.DataSourceExpression = string.Format( "{0}|{1}", field.DataSource, field.Name); // set the name
        }
    }
    
    protected void initializeClonedOrder()
    {
        msOrder newOrder = MultiStepWizards.RegisterForEvent.Order.Clone().ConvertTo<msOrder>();
        newOrder.LineItems = new List<msOrderLineItem>(); // this is NECESSARY b/c clone doesn't work with lists!

        // add the main fee
        if (!MultiStepWizards.RegisterForEvent.IsSessionSwap)
        {
            var mode = Request.QueryString["mode"];
            if (mode == "group")
                newOrder.LineItems.Add(new msOrderLineItem
                {
                    Product = targetRegistrationFee.ID,
                    Quantity = 1,
                    OrderLineItemID = new Guid().ToString(),
                    OverrideMemberPricingTo = Request.QueryString["individualID"]
                });
            else
                newOrder.LineItems.Add(new msOrderLineItem
                {
                    Product = targetRegistrationFee.ID,
                    Quantity = 1,
                    OrderLineItemID = Guid.NewGuid().ToString()
                });
        }

        if (targetAdditionalItems != null)
            foreach (var item in targetAdditionalItems)
            {
                if (item.OrderLineItemID == null)
                    item.OrderLineItemID = Guid.NewGuid().ToString();
                newOrder.LineItems.Add(item);
            }

        targetOrder = newOrder;    
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
            fieldMetadata.PortalAccessibility = PortalAccessibility.Full;
            fieldMetadata.Name = Formats.GetSafeFieldName(targetRegistrationField.Name);
            fieldMetadata.IsRequired = fieldMetadata.IsRequiredInPortal = targetRegistrationField.IsRequiredInPortal.HasValue && targetRegistrationField.IsRequiredInPortal.Value;
            fieldMetadata.LookupTableID = targetRegistrationField.LookupTableID;    // MS-6019 (Modified 1/9/2015) Need to account for lookup tables
            result.Fields.Add(fieldMetadata);
        }
        return result;
    }

    protected DataEntryViewMetadata createViewMetadataFromRegistrationFields()
    {
        DataEntryViewMetadata result = new DataEntryViewMetadata();
        ViewMetadata.ControlSection baseSection = null;
        
        result.Sections = new List<ViewMetadata.ControlSection> ();
        foreach (var field in targetRegistrationFields)
        {
            if (field.DisplayType == FieldDisplayType.Separator || baseSection == null )
            {
                // create a new section
                baseSection = new ViewMetadata.ControlSection
                    {
                        Label = field.Label,
                        SubSections = new List<ViewMetadata.ControlSection>()
                    };
                result.Sections.Add(baseSection);
                continue;
            }


            //MS-1358
            field.IsRequired = field.IsRequiredInPortal;

            //MS-1359
            if (field.DisplayType == FieldDisplayType.LargeTextBox)
                field.UseEntireRow = true;

            baseSection.LeftControls.Add(field);
        }

        // remove empty sections
        result.Sections.RemoveAll(x => x.LeftControls  == null || x.LeftControls.Count == 0);
        
        return result;
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

        if (ConciergeAPI.CurrentEntity.ID == targetOrder.BillTo)
            return targetEvent.VisibleInPortal;

        if (targetChapter != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetChapter.Leaders);

        if (targetSection != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetOrganizationalLayer.Leaders);

        var mode = Request.QueryString["mode"];
        if (mode == "group" && ConciergeAPI.CurrentEntity.ID != targetOrder.BillTo)
            return targetEvent.VisibleInPortal;

        //Default to false for now because currently only leaders can create events in the portal
        return false;
    }

    #endregion

    #region Data Binding

    protected void unbindControls()
    {
        cfsRegistrationFields.Harvest();
        MemberSuiteObject mso = cfsRegistrationFields.MemberSuiteObject;

        if (targetOrder.LineItems == null)
            targetOrder.LineItems = new List<msOrderLineItem>();

        if (mso == null)
            throw new ApplicationException("No MemberSuiteObject");

        foreach (var li in targetOrder.LineItems)
        {
            if (li.OrderLineItemID == null) continue;

            // otherwise, copy all of the values
            li.Options = new List<NameValueStringPair>();
            var relevantFields = (from f in mso.Fields
                                  where f.Key != null && f.Key.StartsWith(Formats.GetSafeFieldName(li.OrderLineItemID))
                                  select f);
            foreach (var entry in relevantFields)
            {
                object value = null;

                if (entry.Value != null)
                {
                    if (entry.Value is List<string>)
                    {
                        var valueAsList = (List<string>) entry.Value;
                        value = string.Join("|", valueAsList);
                    }

                    if (entry.Key.EndsWith("_Contents"))
                    {
                        value = entry.Value;
                    }
                    else
                    {
                        if(value == null)
                            value = entry.Value.ToString();
                    }


                    var name = entry.Key.Split('|')[1];

                    // MS-4322 - need to account for demographics here
                    // MS-4970 - need to account for name on registration field (ie: guest registration)
                    if (name.EndsWith("__q") || (name == MemberSuiteConstants.Events.NAME_ON_REGISTRATION_FIELD))
                        li.Options.Add(new NameValueStringPair(name, value.ToString()));
                    else
                        li[name] = value;

                }
            }
        }
    }

    #endregion

    #region Methods


    protected void GoToOrderForm()
    {
        // now - is this a group registration? Because if it is, let's go back to the group screen
        var group = MultiStepWizards.GroupRegistration.Group;
        var order = MultiStepWizards.GroupRegistration.Order;
        var ev = MultiStepWizards.GroupRegistration.Event;

        var thisRegistrationOrder = targetOrder;

        if (group != null && ev != null && ev.ID == targetEvent.ID)    // make sure event IDs match
        {
            // let's make sure all of the order items point to the right place
            foreach (msOrderLineItem li in thisRegistrationOrder.LineItems)
            {
                li.OverrideShipTo = MultiStepWizards.GroupRegistration.RegistrantID;
                if (li.Options == null) li.Options = new List<NameValueStringPair>();
                li.Options.Add(new NameValueStringPair(OrderLineItemOptions.Events.GroupId, group.ID));
            }

            if (order == null) // then create one
                // just use this as the first order
                MultiStepWizards.GroupRegistration.Order = thisRegistrationOrder;
            else
                order.LineItems.AddRange(thisRegistrationOrder.LineItems);

            // now go back to the management page
            GoTo(string.Format("ManageGroupRegistration.aspx?contextID={0}&organizationID={1}",
                ev.ID, group.ID));

        }

        //Add the cloned order as the "shopping cart" for the order processing wizard
        MultiStepWizards.PlaceAnOrder.OrderCompleteUrl = "/events/ViewEvent.aspx?contextID=" + targetEvent.ID;
        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess( targetOrder );
      
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

        //Dynamically render the registration fields
        cfsRegistrationFields.Metadata = createClassMetadataFromRegistrationFields();
        cfsRegistrationFields.PageLayout = createViewMetadataFromRegistrationFields();
        cfsRegistrationFields.MemberSuiteObject = new MemberSuiteObject();
        cfsRegistrationFields.Render();
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MultiStepWizards.RegisterForEvent.Order = null;
        MultiStepWizards.RegisterForEvent.RegistrationFee = null;
        MultiStepWizards.RegisterForEvent.AdditionalLineItems = null;

        MultiStepWizards.GroupRegistration.NavigateBackToGroupRegistrationIfApplicable( targetEvent.ID );

        GoHome();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        var mode = Request.QueryString["mode"];
        GoTo(mode == "group"
                 ? string.Format("~/events/Register_CreateRegistration.aspx?contextID={0}&mode=group&individualID={1}", ContextID, Request.QueryString["individualID"])
                 : string.Format("~/events/Register_CreateRegistration.aspx?contextID={0}", ContextID));
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        unbindControls();

      
        GoToOrderForm();
    }

    private void Restart()
    {
        GoTo(string.Format("~/events/Register_SelectFee.aspx?contextID={0}", ContextID));
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
        
        var individual = GetServiceAPIProxy().GetName(MultiStepWizards.GroupRegistration.RegistrantID).ResultValue;
        lblRegistrant.Text = individual;
    }

    #endregion
}