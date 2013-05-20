using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Web.Controls;

public partial class chapters_EditChapterInfo : PortalPage
{
    #region Fields

    protected msChapter targetChapter;
    protected msOrganization linkedOrganization;

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

        targetChapter = LoadObjectFromAPI<msChapter>(ContextID);

        if (targetChapter == null)
        {
            GoToMissingRecordPage();
            return;
        }

        if (!string.IsNullOrWhiteSpace(targetChapter.LinkedOrganization))
            linkedOrganization = LoadObjectFromAPI<msOrganization>(targetChapter.LinkedOrganization);
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

        bindObjectToPage();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        if (targetChapter.Leaders == null)
            // no leaders to speak of
            return false;

        var leader = targetChapter.Leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanUpdateInformation;
    }

    #endregion

    #region Data binding

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        // important - custom fields need to know who their context is
        cfsChapterFields.MemberSuiteObject = targetChapter;

        var pageLayout = GetAppropriatePageLayout(targetChapter);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
        {
            divOtherInformation.Visible = false;
            return;
        }

        // setup the metadata
        cfsChapterFields.Metadata = proxy.DescribeObject(msChapter.CLASS_NAME).ResultValue;
        cfsChapterFields.PageLayout = pageLayout.Metadata;

        cfsChapterFields.Render();
    }

    private void bindObjectToPage()
    {
        tbName.Text = targetChapter.Name;
        reDescription.Content = targetChapter.Description;

        if(linkedOrganization == null)
        {
            divAddressInformation.Visible = false;
            divPhoneNumbers.Visible = false;
            return;
        }

        // ok, we need to get all of the phone number types
        List<msPhoneNumberType> phoneNumberTypes = getPhoneNumberTypes();

        // now, let's bind them
        if (phoneNumberTypes.Count > 0)
        {
            divPhoneNumbers.Visible = true;
            gvPhoneNumbers.DataKeyNames = new string[] { "Code" };
            gvPhoneNumbers.DataSource = phoneNumberTypes;
            gvPhoneNumbers.DataBind();
        }

        // ok, same with with the addresses
        List<msAddressType> addressTypes = getAddressTypes();

        if (addressTypes.Count > 0)
        {
            divAddressInformation.Visible = true;
            rptAddresses.DataSource = addressTypes;
            rptAddresses.DataBind();

            // set the preferred addresss
            ddlPreferredAddress.DataSource = addressTypes;

            ddlPreferredAddress.DataTextField = "Name";
            ddlPreferredAddress.DataValueField = "ID";
            ddlPreferredAddress.DataBind();

            ListItem li = ddlPreferredAddress.Items.FindByValue(linkedOrganization.PreferredAddressType);
            if (li != null)
                li.Selected = true; // select the preferred address
        }

    }

    private List<msAddressType> getAddressTypes()
    {
        var addressTypes = GetAllObjects<msAddressType>(msAddressType.CLASS_NAME);

        // we only want active address types, and address types that apply to organizations
        // let's remove what we don't want
        addressTypes.RemoveAll(x => !x.IsActive || !x.ShowInPortal || x.CustomerType == CustomerType.Individual);
        return addressTypes;
    }

    private List<msPhoneNumberType> getPhoneNumberTypes()
    {
        var phoneNumberTypes = GetAllObjects<msPhoneNumberType>(msPhoneNumberType.CLASS_NAME);

        // we only want active phone number types, and phone number types that apply to Organizations
        // let's remove what we don't want
        phoneNumberTypes.RemoveAll(x => !x.IsActive || !x.ShowInPortal || x.CustomerType == CustomerType.Individual);
        return phoneNumberTypes;
    }

    private void unbindObjectFromPage()
    {
        targetChapter.Name = tbName.Text;
        targetChapter.Description = reDescription.Content ;

        // custom fields
        cfsChapterFields.Harvest();

        if (linkedOrganization == null)
            return;

        // now, the phone numbers
        foreach (GridViewRow grvPhoneNumberType in gvPhoneNumbers.Rows)
        {
            DataKey dataKey = gvPhoneNumbers.DataKeys[grvPhoneNumberType.RowIndex];
            if (dataKey == null)
                continue;

            var code = dataKey.Value;
            TextBox tb = grvPhoneNumberType.FindControl("tbPhoneNumber") as TextBox;


            if (tb == null) return;
            linkedOrganization[code + "_PhoneNumber"] = tb.Text;
        }

        // the preferred
        string preferredPhoneNumberID = Request.Form["PhoneNumberPreferredType"];
        if (preferredPhoneNumberID != null)
            linkedOrganization.PreferredPhoneNumberType = preferredPhoneNumberID;


        // now, let's unbind the addresses
        foreach (RepeaterItem riAddress in rptAddresses.Items)
        {
            AddressControl ac = (AddressControl)riAddress.FindControl("acAddress");
            HiddenField hfAddressCode = (HiddenField)riAddress.FindControl("hfAddressCode");

            if (ac == null || hfAddressCode == null) continue;  // defensive programmer

            string code = hfAddressCode.Value;  // remember we stuck the code in there during databinding
            linkedOrganization[code + "_Address"] = ac.Address;
        }

        // let's get the preferred address
        linkedOrganization.PreferredAddressType = ddlPreferredAddress.SelectedValue;
    }


    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;


        unbindObjectFromPage();

        targetChapter = SaveObject(targetChapter).ConvertTo<msChapter>();

        if (linkedOrganization != null)
            linkedOrganization = SaveObject(linkedOrganization).ConvertTo<msOrganization>();

        QueueBannerMessage("Chapter information was updated sucessfully.");

        GoTo(string.Format("~/chapters/ViewChapter.aspx?contextID={0}",ContextID));
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo(string.Format("~/chapters/ViewChapter.aspx?contextID={0}", ContextID));
    }

    protected void gvPhoneNumbers_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        msPhoneNumberType pt = (msPhoneNumberType)e.Row.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                Label lblPhoneNumberType = (Label)e.Row.FindControl("lblPhoneNumberType");
                TextBox tbPhoneNumber = (TextBox)e.Row.FindControl("tbPhoneNumber");
                var lRadioButtonMarkup = (Literal)e.Row.FindControl("lRadioButtonMarkup");

                lblPhoneNumberType.Text = pt.Name;

                /* We need to extract the phone number from the underlying MemberSuiteObject
                 * phone numbers are flattened, and they are in the format of <Code>_PhoneNumber
                 * where <Code> is the API Code of the phone number type*/

                tbPhoneNumber.Text = linkedOrganization.SafeGetValue<string>(pt.Code + "_PhoneNumber");

                /* We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                    in repeater controls
                    http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->*/

                bool isSelected = linkedOrganization.PreferredPhoneNumberType == pt.ID;
                lRadioButtonMarkup.Text = string.Format(
                    @"<input type=radio name=PhoneNumberPreferredType " +
                    @"id=RowSelector{0} value='{1}' {2} />", e.Row.RowIndex,
                    pt.ID,
                    isSelected ? "checked" : "");


                break;
        }
    }

    protected void rptAddresses_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        msAddressType at = (msAddressType)e.Item.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;

            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
                goto case ListItemType.Item;

            case ListItemType.Item:
                Label lblAddressType = (Label)e.Item.FindControl("lblAddressType");
                AddressControl acAddress = (AddressControl)e.Item.FindControl("acAddress");
                Literal lNewRowTag = (Literal)e.Item.FindControl("lNewRowTag");
                HiddenField hfAddressCode = (HiddenField)e.Item.FindControl("hfAddressCode");


                /* We need to extract the adress from the underlying MemberSuiteObject
                 * address are flattened, and they are in the format of <Code>_Address
                 * where <Code> is the API Code of the address type*/

                lblAddressType.Text = at.Name;
                hfAddressCode.Value = at.Code;  // we'll need this for when we unbind
                acAddress.Address = linkedOrganization.SafeGetValue<Address>(at.Code + "_Address");

                /* we're doing THREE columns, so if the index is divisible by three, we want to generate a
                 * start row tag (<tr>)and an end row tag (</tr>)
                 */

                if (e.Item.ItemIndex != 0 && (e.Item.ItemIndex + 1) % 3 == 0)
                    lNewRowTag.Text = "</TR><TR>";



                break;
        }
    }

    protected void rptAddresses_OnItemCreated(object sender, RepeaterItemEventArgs e)
    {
        // we need to set the control host property every time the item is created
        AddressControl acAddress = (AddressControl)e.Item.FindControl("acAddress");

        if (acAddress != null)
            acAddress.Host = this;

    }

    #endregion
}