using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Web.Controls;
using Image = System.Drawing.Image;

public partial class profile_EditOrganizationInfo : PortalPage
{
    #region Fields

    protected msOrganization targetObject;

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        string targetOrganizationID = string.IsNullOrWhiteSpace(ContextID) ? CurrentEntity.ID : ContextID;
        targetObject = LoadObjectFromAPI<msOrganization>(targetOrganizationID);

        if (targetObject == null)
            GoToMissingRecordPage();

    }


    protected override void InitializePage()
    {
        base.InitializePage();

        setProfileImage(img,targetObject);
        bindObjectToPage();
    }

  
    #endregion

    #region Data binding

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        // important - custom fields need to know who their context is
        CustomFieldSet1.MemberSuiteObject = targetObject;

        var pageLayout = targetObject.GetAppropriatePageLayout();
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = targetObject.DescribeObject();
        CustomFieldSet1.PageLayout = pageLayout.Metadata;

        CustomFieldSet1.Render();
    }

    private void bindObjectToPage()
    {
        tbName.Text = targetObject.Name;

        // now, the email
        tbEmail.Text = targetObject.EmailAddress;
        tbBillingContactName.Text = targetObject.SafeGetValue<string>("BillingContactName");
        tbBillingContactPhoneNumber.Text = targetObject.SafeGetValue<string>("BillingContactPhoneNumber");

        // ok, we need to get all of the phone number types
        List<msPhoneNumberType> phoneNumberTypes = getPhoneNumberTypes();

        // now, let's bind them
        if (phoneNumberTypes.Count > 0)
        {
            phPhoneNumbers.Visible = true;
            gvPhoneNumbers.DataKeyNames = new string[]{ "Code" };
            gvPhoneNumbers.DataSource = phoneNumberTypes;
            gvPhoneNumbers.DataBind();
        }

        // ok, same with with the addresses
        List<msAddressType> addressTypes = getAddressTypes();

        if (addressTypes.Count > 0)
        {
            phAddresses.Visible = true;
            rptAddresses.DataSource = addressTypes;
            rptAddresses.DataBind();

            // set the preferred addresss
            ddlPreferredAddress.DataSource = addressTypes;
            
            ddlPreferredAddress.DataTextField = "Name";
            ddlPreferredAddress.DataValueField = "ID";
            ddlPreferredAddress.DataBind();

            ListItem li = ddlPreferredAddress.Items.FindByValue(targetObject.PreferredAddressType);
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
        addressTypes.Sort((x, y) => x.DisplayOrder.GetValueOrDefault().CompareTo(y.DisplayOrder.GetValueOrDefault()));
        return addressTypes;
    }

    private List<msPhoneNumberType> getPhoneNumberTypes()
    {
        var phoneNumberTypes = GetAllObjects<msPhoneNumberType>( msPhoneNumberType.CLASS_NAME );

        // we only want active phone number types, and phone number types that apply to organizations
        // let's remove what we don't want
        phoneNumberTypes.RemoveAll(x => !x.IsActive || !x.ShowInPortal || x.CustomerType == CustomerType.Individual);
        phoneNumberTypes.Sort((x, y) => x.DisplayOrder.GetValueOrDefault().CompareTo(y.DisplayOrder.GetValueOrDefault()));
        return phoneNumberTypes;
    }

    private void unbindObjectFromPage()
    {
        targetObject.Name = tbName.Text;

        //unbind the image if applicable
        if (imageUpload.HasFile)
        {
            targetObject["Image_Contents"] = getImageFile();

            //Set the session variable forcing any cached images to be refreshed from the MemberSuite Image content server
            SessionManager.Set("ImageUrlUpper", !SessionManager.Get<bool>("ImageUrlUpper"));
        }

        // now, the email
        targetObject.EmailAddress = tbEmail.Text;
        targetObject["BillingContactName"] = tbBillingContactName.Text ;
        targetObject["BillingContactPhoneNumber"] = tbBillingContactPhoneNumber.Text  ;

        // now, the phone numbers
        foreach (GridViewRow grvPhoneNumberType in gvPhoneNumbers.Rows)
        {
            DataKey dataKey = gvPhoneNumbers.DataKeys[grvPhoneNumberType.RowIndex];
            if(dataKey == null)
                continue;

            var code = dataKey.Value;
            TextBox tb = grvPhoneNumberType.FindControl("tbPhoneNumber") as TextBox;
            

            if (tb == null) return;
            targetObject[code + "_PhoneNumber"] = tb.Text;
        }

        // the preferred
        string preferredPhoneNumberID = Request.Form["PhoneNumberPreferredType"];
        if (preferredPhoneNumberID != null)
            targetObject.PreferredPhoneNumberType = preferredPhoneNumberID;
   

        // now, let's unbind the addresses
        foreach (RepeaterItem riAddress in rptAddresses.Items)
        {
            AddressControl ac = (AddressControl) riAddress.FindControl("acAddress");
            HiddenField hfAddressCode = (HiddenField)riAddress.FindControl("hfAddressCode");

            if (ac == null || hfAddressCode == null) continue;  // defensive programmer

            string code = hfAddressCode.Value;  // remember we stuck the code in there during databinding
            targetObject[code + "_Address"] = ac.Address;
        }

        // let's get the preferred address
        targetObject.PreferredAddressType = ddlPreferredAddress.SelectedValue;

      

        // finally, the custom fields
        CustomFieldSet1.Harvest();
    }

    protected void imageValidate(object sender, ServerValidateEventArgs e)
    {
        CustomValidator validator = (CustomValidator)sender;
        FileUpload upload = (FileUpload) validator.NamingContainer.FindControl(validator.ControlToValidate);

        if (string.IsNullOrWhiteSpace(imageUpload.PostedFile.ContentType) || !imageUpload.PostedFile.ContentType.ToLower().StartsWith("image/"))
        {
            e.IsValid = false;
            return;
        }

        MemoryStream stream = new MemoryStream(imageUpload.FileBytes);
        try
        {
            Image image = Image.FromStream(stream);
        }
        catch (ArgumentException )
        {
            e.IsValid = false;
            return;
        }

        e.IsValid = true;
    }

    private MemberSuiteFile getImageFile()
    {
        if (!imageUpload.HasFile)
            return null;

        MemoryStream stream = new MemoryStream(imageUpload.FileBytes);
        System.Drawing.Image image = Image.FromStream(stream);
        
        //Resize the image if required - perserve scale
        if (image.Width > 120 || image.Height > 120)
        {
            int largerDimension = image.Width > image.Height ? image.Width : image.Height;
            decimal ratio = largerDimension/120m; //Target size is 120x120 so this works for either dimension

            int thumbnailWidth = Convert.ToInt32(image.Width/ratio); //Explicit convert will round
            int thumbnailHeight = Convert.ToInt32(image.Height/ratio); //Explicit convert will round

            //Create a thumbnail to resize the image.  The delegate is not used and IntPtr.Zero is always required.
            //For more information see http://msdn.microsoft.com/en-us/library/system.drawing.image.getthumbnailimage.aspx
            Image resizedImage = image.GetThumbnailImage(thumbnailWidth, thumbnailHeight, () => false, IntPtr.Zero);
            
            //Replace the stream containing the original image with the resized image
            stream = new MemoryStream();
            resizedImage.Save(stream,image.RawFormat);
        }

        var result = new MemberSuiteFile
                    {
                        FileContents = stream.ToArray(),
                        FileName = imageUpload.FileName,
                        FileType = imageUpload.PostedFile.ContentType
                    };

        return result;
    }

    #endregion

    #region Event Handlers

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;


        unbindObjectFromPage();

        targetObject = SaveObject(targetObject).ConvertTo<msOrganization>();

        if (targetObject.ID == CurrentEntity.ID)
            ConciergeAPI.CurrentEntity = targetObject;

        if (PortalConfiguration.Current.SendEmailWhenUserUpdatesInformation)
        {
            //Send the update confirmation email
            using (IConciergeAPIService proxy = GetConciegeAPIProxy())
                proxy.SendTransactionalEmail(EmailTemplates.CRM.UserInformationUpdate, targetObject.ID, null);
            
        }

        QueueBannerMessage("Your profile was updated successfully.");

        GoHome();
    }

   
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
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
                Literal lPhoneNumberRequired = (Literal)e.Row.FindControl("lPhoneNumberRequired");
                RequiredFieldValidator rfvPhoneNumber = (RequiredFieldValidator)e.Row.FindControl("rfvPhoneNumber");
                

                bool requiredInPortal = pt.SafeGetValue<bool>("RequiredInPortal");
                lPhoneNumberRequired.Visible = requiredInPortal;
                rfvPhoneNumber.Enabled = requiredInPortal;
                rfvPhoneNumber.ErrorMessage = string.Format("You must enter a {0} phone number.", pt.Name);

                lblPhoneNumberType.Text = pt.Name;

                /* We need to extract the phone number from the underlying MemberSuiteObject
                 * phone numbers are flattened, and they are in the format of <Code>_PhoneNumber
                 * where <Code> is the API Code of the phone number type*/

                tbPhoneNumber.Text = targetObject.SafeGetValue<string>(pt.Code + "_PhoneNumber");

                /* We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                    in repeater controls
                    http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->*/

                bool isSelected = targetObject.PreferredPhoneNumberType == pt.ID;
                lRadioButtonMarkup.Text = string.Format( 
                    @"<input type=radio name=PhoneNumberPreferredType " +
                    @"id=RowSelector{0} value='{1}' {2} />", e.Row.RowIndex, 
                    pt.ID,
                    isSelected ? "checked" : "" );


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
                acAddress.Address = targetObject.SafeGetValue<Address>(at.Code + "_Address");

              
                /* we're doing THREE columns, so if the index is divisible by three, we want to generate a
                 * start row tag (<tr>)and an end row tag (</tr>)
                 */

                if ( e.Item.ItemIndex != 0 &&  (e.Item.ItemIndex + 1) % 3 == 0)
                    lNewRowTag.Text = "</TR><TR>";

                

                break;
        }
    }

    protected void rptAddresses_OnItemCreated(object sender, RepeaterItemEventArgs e)
    {
        // we need to set the control host property every time the item is created
        var acAddress = (AddressControl) e.Item.FindControl("acAddress");
        var at = e.Item.DataItem as msAddressType;

        if (acAddress != null)
        {
            acAddress.Host = this;
            if (at != null) acAddress.IsRequired = at.SafeGetValue<bool>("RequiredInPortal");
        }
    }

    #endregion
}