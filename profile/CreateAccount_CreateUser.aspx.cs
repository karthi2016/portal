using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Web.Controls;
using Image = System.Drawing.Image;

public partial class profile_CreateAccount_Complete : PortalPage
{

    #region Fields

    protected msIndividual targetIndividual;
    protected msOrganization targetOrganization;
    protected msRelationship targetOrganizationRelationship;
    protected msPortalUser targetPortalUser;
    protected NewUserRequest newUserRequest;
    protected List<msPhoneNumberType> individualPhoneNumberTypes;
    protected List<msPhoneNumberType> organizationPhoneNumberTypes;
    protected List<msAddressType> individualAddressTypes;
    protected List<msAddressType> organizationAddressTypes;
    protected DataTable dtPortalSignupRelationshipTypes;
    protected DataView dvAllOrganizations;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
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

        targetIndividual = MultiStepWizards.CreateAccount.TargetIndividual ?? new msIndividual();
        targetPortalUser = MultiStepWizards.CreateAccount.TargetPortalUser ?? new msPortalUser();
        targetOrganization = MultiStepWizards.CreateAccount.TargetOrganization ?? new msOrganization();
        targetOrganizationRelationship = MultiStepWizards.CreateAccount.TargetOrganizationRelationship;

        newUserRequest = MultiStepWizards.CreateAccount.Request;
        if (newUserRequest == null)
        {
            newUserRequest = new NewUserRequest();
            MultiStepWizards.CreateAccount.Request = newUserRequest;
        }

        string completeUrl = Request.QueryString["completionUrl"];
        if (!string.IsNullOrWhiteSpace(completeUrl))
            MultiStepWizards.CreateAccount.CompleteUrl = completeUrl;
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

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            getDataFromConcierge(proxy);
        }

        bindIndividualToPage();
        bindOrganizationToPage();

        string pageTitle = Request.QueryString["pageTitle"];
        if (pageTitle == null)
            pageTitle = MultiStepWizards.CreateAccount.Request.Name;

        if (pageTitle != null)
            lblTitle.Text = pageTitle;

        ddlPortalSignupRelationshipTypes.DataSource = dtPortalSignupRelationshipTypes;
        ddlPortalSignupRelationshipTypes.DataBind();

        ddlAllOrganizations.DataSource = dvAllOrganizations;
        ddlAllOrganizations.DataBind();

        divOrganizationInformation.Visible = dtPortalSignupRelationshipTypes.Rows.Count > 0;
        trOrganizationRole.Visible = dtPortalSignupRelationshipTypes.Rows.Count > 1;


    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        // important - custom fields need to know who their context is
        cfsIndividualCustomFields.MemberSuiteObject = targetIndividual;

        var pageLayout = GetAppropriatePageLayout(targetIndividual);
        if ((pageLayout != null && pageLayout.Metadata != null) && !pageLayout.Metadata.IsEmpty())
        {
            cfsIndividualCustomFields.Metadata = proxy.DescribeObject(msIndividual.CLASS_NAME).ResultValue;
            cfsIndividualCustomFields.PageLayout = pageLayout.Metadata;

            cfsIndividualCustomFields.Render();

            
        }

        // important - custom fields need to know who their context is
        cfsOrganizationCustomFields.MemberSuiteObject = targetOrganization;

        var orgPageLayout = GetAppropriatePageLayout(targetOrganization);
        if ((orgPageLayout != null && orgPageLayout.Metadata != null) && !orgPageLayout.Metadata.IsEmpty())
        {
            cfsOrganizationCustomFields.Metadata = proxy.DescribeObject(msOrganization.CLASS_NAME).ResultValue;
            cfsOrganizationCustomFields.PageLayout = orgPageLayout.Metadata;

            cfsOrganizationCustomFields.Render();
        }
    }


    #endregion

    #region Data binding

    private void bindIndividualToPage()
    {
        tbLoginID.Text = newUserRequest.EmailAddress;
        tbIndividualFirstName.Text = newUserRequest.FirstName;
        tbIndividualLastName.Text = newUserRequest.LastName;

        // now, the email
        tbIndividualEmail.Text = newUserRequest.EmailAddress;
        
        // now, let's bind  phone number types
        if (individualPhoneNumberTypes.Count > 0)
        {
            phIndividualPhoneNumbers.Visible = true;
            gvIndividualPhoneNumbers.DataKeyNames = new string[] { "Code" };
            gvIndividualPhoneNumbers.DataSource = individualPhoneNumberTypes;
            gvIndividualPhoneNumbers.DataBind();
        }

        // ok, same with with the addresses
        if (individualAddressTypes.Count > 0)
        {
            phIndividualAddresses.Visible = true;
            rptIndividualAddresses.DataSource = individualAddressTypes;
            rptIndividualAddresses.DataBind();

            // set the preferred addresss
            ddlIndividualPreferredAddress.DataSource = individualAddressTypes;

            ddlIndividualPreferredAddress.DataTextField = "Name";
            ddlIndividualPreferredAddress.DataValueField = "ID";
            ddlIndividualPreferredAddress.DataBind();

            ListItem li = ddlIndividualPreferredAddress.Items.FindByValue(targetIndividual.PreferredAddressType);
            if (li != null)
                li.Selected = true; // select the preferred address

            // do we have a seasonal address
            var seasonal = individualAddressTypes.Find(x => x.IsSeasonal);
            if (seasonal != null) // yes we do
            {
                trIndividualSeasonality.Visible = true;
                lblIndividualSeasonalAddressType.Text = seasonal.Name;

                mdpIndividualSeasonalStart.Date = targetIndividual.SeasonalAddressStart;
                mdpIndividualSeasonalEnd.Date = targetIndividual.SeasonalAddressEnd;

                if (targetIndividual.SeasonalAddressStart != null || targetIndividual.SeasonalAddressEnd != null)
                    rbIndividualSeasonalYes.Checked = true;
                else
                    rbIndividualSeasonalNo.Checked = true;
            }


        }

    }

    private void bindOrganizationToPage()
    {
        if (organizationPhoneNumberTypes.Count > 0)
        {
            phOrganizationPhoneNumbers.Visible = true;
            gvOrganizationPhoneNumbers.DataKeyNames = new string[] { "Code" };
            gvOrganizationPhoneNumbers.DataSource = organizationPhoneNumberTypes;
            gvOrganizationPhoneNumbers.DataBind();
        }

        if (organizationAddressTypes.Count > 0)
        {
            phOrganizationAddresses.Visible = true;
            rptOrganizationAddresses.DataSource = organizationAddressTypes;
            rptOrganizationAddresses.DataBind();

            // set the preferred addresss
            ddlOrganizationPreferredAddress.DataSource = organizationAddressTypes;

            ddlOrganizationPreferredAddress.DataTextField = "Name";
            ddlOrganizationPreferredAddress.DataValueField = "ID";
            ddlOrganizationPreferredAddress.DataBind();
        }
    }

    private void unbindOrganizationRelationship()
    {
        if (!string.IsNullOrWhiteSpace(ddlAllOrganizations.SelectedValue))
        {
            targetOrganizationRelationship = new msRelationship();

            if(dtPortalSignupRelationshipTypes == null)
                using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
                {
                    getPortalSignupRelationshipTypes(proxy);
                }

            DataRow drRelationshipType = dtPortalSignupRelationshipTypes.Rows.Count == 1 ? dtPortalSignupRelationshipTypes.Rows[0] : 
                dtPortalSignupRelationshipTypes.Rows.Find(ddlPortalSignupRelationshipTypes.SelectedValue);
            targetOrganizationRelationship.Type = drRelationshipType["ID"].ToString();

            switch (drRelationshipType["LeftSideType"].ToString())
            {
                case msIndividual.CLASS_NAME:
                    targetOrganizationRelationship.LeftSide = targetIndividual.ID;
                    targetOrganizationRelationship.RightSide = ddlAllOrganizations.SelectedValue.ToLower() != "notlisted" ? ddlAllOrganizations.SelectedValue : targetOrganization.ID;
                    break;
                case msOrganization.CLASS_NAME:
                    targetOrganizationRelationship.LeftSide = ddlAllOrganizations.SelectedValue.ToLower() != "notlisted" ? ddlAllOrganizations.SelectedValue : targetOrganization.ID;
                    targetOrganizationRelationship.RightSide = targetIndividual.ID;
                    break;
                default:
                    throw new ApplicationException("A Relationship Type has EnablePortalSignup set to true but is NOT an Individual/Organization Relationship Type.");
            }

            MultiStepWizards.CreateAccount.TargetOrganizationRelationship = targetOrganizationRelationship;
        }
    }

    private bool unbindPortalUserAndIndividual()
    {
        //Portal User
        targetPortalUser.FirstName = tbIndividualFirstName.Text;
        targetPortalUser.LastName = tbIndividualLastName.Text;
        targetPortalUser.EmailAddress = tbIndividualEmail.Text;
        targetPortalUser.Name = tbLoginID.Text;
        targetPortalUser["Password"] = tbPassword.Text;

        targetIndividual.Title = tbIndividualTitle.Text;
        targetIndividual.FirstName = tbIndividualFirstName.Text;
        targetIndividual.MiddleName = tbIndividualMiddleName.Text;
        targetIndividual.LastName = tbIndividualLastName.Text;
        targetIndividual.Nickname = tbIndividualNickName.Text;
        targetIndividual.Suffix = tbIndividualSuffix.Text;

        //unbind the image if applicable
        if (individualImageUpload.HasFile)
        {
            targetIndividual["Image_Contents"] = getIndividualImageFile();

            //Set the session variable forcing any cached images to be refreshed from the MemberSuite Image content server
            SessionManager.Set("ImageUrlUpper", !SessionManager.Get<bool>("ImageUrlUpper") );
            
        }

        // now, the email
        targetIndividual.EmailAddress = tbIndividualEmail.Text;
        targetIndividual.EmailAddress2 = tbIndividualEmail2.Text;
        targetIndividual.EmailAddress3 = tbIndividualEmail3.Text;

        // now, the phone numbers
        bool atLeastOnePhoneNumber = false;
        foreach (GridViewRow grvPhoneNumberType in gvIndividualPhoneNumbers.Rows)
        {
            DataKey dataKey = gvIndividualPhoneNumbers.DataKeys[grvPhoneNumberType.RowIndex];
            if (dataKey == null)
                continue;

            var code = dataKey.Value;
            TextBox tb = grvPhoneNumberType.FindControl("tbIndividualPhoneNumber") as TextBox;


            if (tb == null) continue;
            targetIndividual[code + "_PhoneNumber"] = tb.Text;
            if (!string.IsNullOrWhiteSpace(tb.Text))
                atLeastOnePhoneNumber = true;
        }

        //if (!atLeastOnePhoneNumber)
        //{
        //    cvIndividualPhoneNumber.IsValid = false;
        //    return false;
        //}

        // the preferred
        string preferredPhoneNumberID = Request.Form["IndividualPhoneNumberPreferredType"];
        if (preferredPhoneNumberID != null)
            targetIndividual.PreferredPhoneNumberType = preferredPhoneNumberID;


        // now, let's unbind the addresses
        bool atLeastOneAddress = false;
        foreach (RepeaterItem riAddress in rptIndividualAddresses.Items)
        {
            AddressControl ac = (AddressControl)riAddress.FindControl("acIndividualAddress");
            HiddenField hfAddressCode = (HiddenField)riAddress.FindControl("hfIndividualAddressCode");

            if (ac == null || hfAddressCode == null) continue;  // defensive programmer

            string code = hfAddressCode.Value;  // remember we stuck the code in there during databinding
            targetIndividual[code + "_Address"] = ac.Address;

            if (ac.Address != null && !string.IsNullOrWhiteSpace(ac.Address.Line1))
                atLeastOneAddress = true;
        }

        //if (!atLeastOneAddress)
        //{
        //    cvIndividualAddress.IsValid = false;
        //    return false;
        //}

        // let's get the preferred address
        targetIndividual.PreferredAddressType = ddlIndividualPreferredAddress.SelectedValue;

        // and, the seasonal settings
        if (rbIndividualSeasonalNo.Checked)
        {
            targetIndividual.SeasonalAddressStart = null;
            targetIndividual.SeasonalAddressEnd = null;
        }
        else
        {
            targetIndividual.SeasonalAddressStart = mdpIndividualSeasonalStart.Date;
            targetIndividual.SeasonalAddressEnd = mdpIndividualSeasonalEnd.Date;
        }

        // finally, the custom fields
        cfsIndividualCustomFields.Harvest();

        MultiStepWizards.CreateAccount.TargetIndividual = targetIndividual;
        MultiStepWizards.CreateAccount.TargetPortalUser = targetPortalUser;

        return true;
    }

    protected bool unbindOrganization()
    {
        targetOrganization.Name = tbOrganizationName.Text;

        ////unbind the image if applicable
        //if (organizationImageUpload.HasFile)
        //    targetOrganization["Image_Contents"] = getOrganizationImageFile();

        // now, the email
        targetOrganization.EmailAddress = tbOrganizationEmail.Text;
        targetOrganization["BillingContactName"] = tbBillingContactName.Text;
        targetOrganization["BillingContactPhoneNumber"] = tbBillingContactPhoneNumber.Text;

        // now, the phone numbers
        foreach (GridViewRow grvPhoneNumberType in gvOrganizationPhoneNumbers.Rows)
        {
            DataKey dataKey = gvOrganizationPhoneNumbers.DataKeys[grvPhoneNumberType.RowIndex];
            if (dataKey == null)
                continue;

            var code = dataKey.Value;
            TextBox tb = grvPhoneNumberType.FindControl("tbOrganizationPhoneNumber") as TextBox;


            if (tb == null) return false;
            targetOrganization[code + "_PhoneNumber"] = tb.Text;
        }

        // the preferred
        string preferredPhoneNumberID = Request.Form["OrganizationPhoneNumberPreferredType"];
        if (preferredPhoneNumberID != null)
            targetOrganization.PreferredPhoneNumberType = preferredPhoneNumberID;


        // now, let's unbind the addresses
        foreach (RepeaterItem riAddress in rptOrganizationAddresses.Items)
        {
            AddressControl ac = (AddressControl)riAddress.FindControl("acOrganizationAddress");
            HiddenField hfAddressCode = (HiddenField)riAddress.FindControl("hfOrganizationAddressCode");

            if (ac == null || hfAddressCode == null) continue;  // defensive programmer

            string code = hfAddressCode.Value;  // remember we stuck the code in there during databinding
            targetOrganization[code + "_Address"] = ac.Address;
        }

        // let's get the preferred address
        targetOrganization.PreferredAddressType = ddlOrganizationPreferredAddress.SelectedValue;

        // finally, the custom fields
        cfsOrganizationCustomFields.Harvest();

        MultiStepWizards.CreateAccount.TargetOrganization = targetOrganization;

        return true;
    }

    private MemberSuiteFile getIndividualImageFile()
    {
        if (!individualImageUpload.HasFile)
            return null;

        MemoryStream stream = new MemoryStream(individualImageUpload.FileBytes);
        System.Drawing.Image image = Image.FromStream(stream);

        //Resize the image if required - perserve scale
        if (image.Width > 120 || image.Height > 120)
        {
            int largerDimension = image.Width > image.Height ? image.Width : image.Height;
            decimal ratio = largerDimension / 120m; //Target size is 120x120 so this works for either dimension

            int thumbnailWidth = Convert.ToInt32(image.Width / ratio); //Explicit convert will round
            int thumbnailHeight = Convert.ToInt32(image.Height / ratio); //Explicit convert will round

            //Create a thumbnail to resize the image.  The delegate is not used and IntPtr.Zero is always required.
            //For more information see http://msdn.microsoft.com/en-us/library/system.drawing.image.getthumbnailimage.aspx
            Image resizedImage = image.GetThumbnailImage(thumbnailWidth, thumbnailHeight, () => false, IntPtr.Zero);

            //Replace the stream containing the original image with the resized image
            stream = new MemoryStream();
            resizedImage.Save(stream, image.RawFormat);
        }

        var result = new MemberSuiteFile
        {
            FileContents = stream.ToArray(),
            FileName = individualImageUpload.FileName,
            FileType = individualImageUpload.PostedFile.ContentType
        };

        return result;
    }

    //private MemberSuiteFile getOrganizationImageFile()
    //{
    //    if (!organizationImageUpload.HasFile)
    //        return null;

    //    MemoryStream stream = new MemoryStream(organizationImageUpload.FileBytes);
    //    System.Drawing.Image image = Image.FromStream(stream);

    //    //Resize the image if required - perserve scale
    //    if (image.Width > 120 || image.Height > 120)
    //    {
    //        int largerDimension = image.Width > image.Height ? image.Width : image.Height;
    //        decimal ratio = largerDimension / 120m; //Target size is 120x120 so this works for either dimension

    //        int thumbnailWidth = Convert.ToInt32(image.Width / ratio); //Explicit convert will round
    //        int thumbnailHeight = Convert.ToInt32(image.Height / ratio); //Explicit convert will round

    //        //Create a thumbnail to resize the image.  The delegate is not used and IntPtr.Zero is always required.
    //        //For more information see http://msdn.microsoft.com/en-us/library/system.drawing.image.getthumbnailimage.aspx
    //        Image resizedImage = image.GetThumbnailImage(thumbnailWidth, thumbnailHeight, () => false, IntPtr.Zero);

    //        //Replace the stream containing the original image with the resized image
    //        stream = new MemoryStream();
    //        resizedImage.Save(stream, image.RawFormat);
    //    }

    //    var result = new MemberSuiteFile
    //    {
    //        FileContents = stream.ToArray(),
    //        FileName = organizationImageUpload.FileName,
    //        FileType = organizationImageUpload.PostedFile.ContentType
    //    };

    //    return result;
    //}

    #endregion

    #region Methods

    protected void getDataFromConcierge(IConciergeAPIService serviceProxy)
    {
        getPortalSignupRelationshipTypes(serviceProxy);

        if (dtPortalSignupRelationshipTypes.Rows.Count > 0)
            getAllOrganizations(serviceProxy);

        getAddressTypes(serviceProxy);
        getPhoneNumberTypes(serviceProxy);
    }

    private void getPortalSignupRelationshipTypes(IConciergeAPIService proxy)
    {
        Search sRelationshipTypes = new Search(msRelationshipType.CLASS_NAME);
        sRelationshipTypes.AddOutputColumn("ID");
        sRelationshipTypes.AddOutputColumn("Name");
        sRelationshipTypes.AddOutputColumn("LeftSideType");
        sRelationshipTypes.AddOutputColumn("RightSideType");
        sRelationshipTypes.AddCriteria(Expr.Equals("EligibleForPortalSignup", true));
        //sRelationshipTypes.AddCriteria(Expr.DoesNotEqual("Multiplicity", "One To One"));

        SearchOperationGroup individualGroup = new SearchOperationGroup
                                                   {
                                                       FieldName = "LeftSideType",
                                                       GroupType = SearchOperationGroupType.Or
                                                   };
        individualGroup.Criteria.Add(Expr.Equals("LeftSideType", msIndividual.CLASS_NAME));
        individualGroup.Criteria.Add(Expr.Equals("RightSideType", msIndividual.CLASS_NAME));

        SearchOperationGroup organizationGroup = new SearchOperationGroup
                                                     {
                                                         FieldName = "RightSideType",
                                                         GroupType = SearchOperationGroupType.Or
                                                     };
        organizationGroup.Criteria.Add(Expr.Equals("LeftSideType", msOrganization.CLASS_NAME));
        organizationGroup.Criteria.Add(Expr.Equals("RightSideType", msOrganization.CLASS_NAME));

        sRelationshipTypes.AddCriteria(individualGroup);
        sRelationshipTypes.AddCriteria(organizationGroup);

        sRelationshipTypes.AddSortColumn("DisplayOrder");
        sRelationshipTypes.AddSortColumn("Name");

        SearchResult srRelationshipTypes = ExecuteSearch(proxy, sRelationshipTypes, 0, null);
        DataTable result = srRelationshipTypes.Table;
        result.PrimaryKey = new DataColumn[] { result.Columns["ID"] };

        dtPortalSignupRelationshipTypes = result;
    }

    private void getAllOrganizations(IConciergeAPIService proxy)
    {
        Search sOrganizations = new Search(msOrganization.CLASS_NAME);
        sOrganizations.AddOutputColumn("ID");
        sOrganizations.AddOutputColumn("Name");
        sOrganizations.AddSortColumn("Name");

        SearchResult srOrganizations = ExecuteSearch(proxy, sOrganizations, 0, null);

        dvAllOrganizations = new DataView(srOrganizations.Table);
    }

    private void getAddressTypes(IConciergeAPIService proxy)
    {
        individualAddressTypes = GetAllObjects<msAddressType>(proxy, msAddressType.CLASS_NAME);
        organizationAddressTypes = individualAddressTypes.ToList();

        // we only want active address types, and address types that apply to individuals
        // let's remove what we don't want
        individualAddressTypes.RemoveAll(x => !x.IsActive || !x.ShowInPortal || x.CustomerType == CustomerType.Organization);
        individualAddressTypes.Sort((x, y) => x.DisplayOrder.GetValueOrDefault().CompareTo(y.DisplayOrder.GetValueOrDefault()));

        // we only want active address types, and address types that apply to organizations
        // let's remove what we don't want
        organizationAddressTypes.RemoveAll(x => !x.IsActive || !x.ShowInPortal || x.CustomerType == CustomerType.Individual);
        organizationAddressTypes.Sort((x, y) => x.DisplayOrder.GetValueOrDefault().CompareTo(y.DisplayOrder.GetValueOrDefault()));
    }

    private void getPhoneNumberTypes(IConciergeAPIService proxy)
    {
        individualPhoneNumberTypes = GetAllObjects<msPhoneNumberType>(proxy, msPhoneNumberType.CLASS_NAME);
        organizationPhoneNumberTypes = individualPhoneNumberTypes.ToList();

        // we only want active phone number types, and phone number types that apply to individuals
        // let's remove what we don't want
        individualPhoneNumberTypes.RemoveAll(x => !x.IsActive || !x.ShowInPortal || x.CustomerType == CustomerType.Organization);
        individualPhoneNumberTypes.Sort((x, y) => x.DisplayOrder.GetValueOrDefault().CompareTo(y.DisplayOrder.GetValueOrDefault()));

        // we only want active phone number types, and phone number types that apply to organizations
        // let's remove what we don't want
        organizationPhoneNumberTypes.RemoveAll(x => !x.IsActive || !x.ShowInPortal || x.CustomerType == CustomerType.Individual);
        organizationPhoneNumberTypes.Sort((x, y) => x.DisplayOrder.GetValueOrDefault().CompareTo(y.DisplayOrder.GetValueOrDefault()));
    }

    protected void imageValidate(object sender, ServerValidateEventArgs e)
    {
        CustomValidator validator = (CustomValidator)sender;
        FileUpload upload = (FileUpload)validator.NamingContainer.FindControl(validator.ControlToValidate);

        if (string.IsNullOrWhiteSpace(individualImageUpload.PostedFile.ContentType) || !individualImageUpload.PostedFile.ContentType.ToLower().StartsWith("image/"))
        {
            e.IsValid = false;
            return;
        }

        MemoryStream stream = new MemoryStream(individualImageUpload.FileBytes);
        try
        {
            Image image = Image.FromStream(stream);
        }
        catch (ArgumentException)
        {
            e.IsValid = false;
            return;
        }

        e.IsValid = true;
    }

    protected void saveIndividualAndPortalUser(IConciergeAPIService proxy)
    {
        //Save the individual
        targetIndividual = proxy.Save(targetIndividual).ResultValue.ConvertTo<msIndividual>();

        //Save the portal user
        targetPortalUser.Owner = targetIndividual.ID;
        string password = (string) targetPortalUser["Password"];
        targetPortalUser = proxy.Save(targetPortalUser).ResultValue.ConvertTo<msPortalUser>();
        targetPortalUser["Password"] = password;

        proxy.ResetPassword(targetPortalUser["ID"].ToString(), password);
        
        
        //Send the welcome email
        proxy.SendEmail("BuiltIn:Welcome", new List<string> { targetIndividual.ID }, null);
    }

    protected void saveOrganization(IConciergeAPIService proxy)
    {
        if (MultiStepWizards.CreateAccount.TargetOrganization != null)
            targetOrganization = proxy.Save(targetOrganization).ResultValue.ConvertTo<msOrganization>();
    }

    protected void saveOrganizationRelationship(IConciergeAPIService proxy)
    {
        if (targetOrganizationRelationship != null)
            targetOrganizationRelationship = proxy.Save(targetOrganizationRelationship).ResultValue.ConvertTo<msRelationship>();
    }

    protected void setConfirmationText()
    {
        litPortalUser.Text = string.Format("Account <b>{0}</b>", targetPortalUser.Name);
        litIndividual.Text = string.Format("Individual <b>{0} {1}</b>", targetIndividual.FirstName, targetIndividual.LastName);

        liOrganization.Visible = MultiStepWizards.CreateAccount.TargetOrganization != null;
        litOrganization.Text = string.Format("Organization <b>{0}</b>", targetOrganization.Name);

        liRelationship.Visible = false;
        if (!string.IsNullOrWhiteSpace(ddlAllOrganizations.SelectedValue))
        {
            liRelationship.Visible = true;

            if (dtPortalSignupRelationshipTypes == null)
                using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
                {
                    getPortalSignupRelationshipTypes(proxy);
                }

            DataRow drRelationshipType = dtPortalSignupRelationshipTypes.Rows.Count == 1 ? dtPortalSignupRelationshipTypes.Rows[0] : 
                dtPortalSignupRelationshipTypes.Rows.Find(ddlPortalSignupRelationshipTypes.SelectedValue);
            litRelationship.Text = string.Format("<b>{0}</b> relationship between <b>{1} {2}</b> and <b>{3}</b>", drRelationshipType["Name"], targetIndividual.FirstName, targetIndividual.LastName, targetOrganization != null && !string.IsNullOrWhiteSpace(targetOrganization.Name) ? targetOrganization.Name : ddlAllOrganizations.SelectedItem.Text);
        }
    }

    protected void saveAndGoHome()
    {
        //Set the password using the association credentials
        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            saveIndividualAndPortalUser(proxy);

            if (MultiStepWizards.CreateAccount.TargetOrganization != null)
                saveOrganization(proxy);

            unbindOrganizationRelationship();

            if (targetOrganizationRelationship != null)
                saveOrganizationRelationship(proxy);
        }

        MultiStepWizards.CreateAccount.TargetIndividual = null;
        MultiStepWizards.CreateAccount.TargetOrganization = null;
        MultiStepWizards.CreateAccount.TargetOrganizationRelationship = null;
        MultiStepWizards.CreateAccount.TargetPortalUser = null;

        if (MultiStepWizards.CreateAccount.InitiatedByLeader)
        {
            MultiStepWizards.CreateAccount.InitiatedByLeader = false;
            GoTo(string.Format("~/membership/PurchaseMembership1.aspx?entityID={0}", targetPortalUser.Owner));
            return;
        }

        //Log the user in
        using (var proxy = GetServiceAPIProxy())
        {
            var loginResult = proxy.LoginToPortal((string)targetPortalUser["Name"],
                                                  (string)targetPortalUser["Password"]);
            ConciergeAPI.SetSession(loginResult.ResultValue);
        }

        QueueBannerMessage(string.Format("Individual #{0} - {1} has been saved sucessfully.", targetIndividual.LocalID, targetIndividual.Name));

        if (!string.IsNullOrWhiteSpace(MultiStepWizards.CreateAccount.CompleteUrl))
        {
            string nextUrl = MultiStepWizards.CreateAccount.CompleteUrl;
            nextUrl = nextUrl.Contains("?")
                          ? string.Format("{0}&entityID={1}", nextUrl, targetIndividual.ID)
                          : string.Format("{0}?entityID={1}", nextUrl, targetIndividual.ID);
            MultiStepWizards.CreateAccount.InitiatedByLeader = false;
            GoTo(nextUrl);
            return;
        }

        GoHome();
    }

    #endregion

    #region Event Handlers

    #region Wizard Step Event Handlers

    protected void wizCreateAccount_FinishButtonClick(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        saveAndGoHome();
    }

    protected void wizCreateAccount_CancelButtonClick(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void wizCreateAccount_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            case 0:
                if (!unbindPortalUserAndIndividual())
                {
                    e.Cancel = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlAllOrganizations.SelectedValue) || ddlAllOrganizations.SelectedValue.ToLower() != "notlisted")
                {
                    MultiStepWizards.CreateAccount.TargetOrganization = null;
                    saveAndGoHome();
                }

                setConfirmationText();
                break;
            case 1:
                if (!unbindOrganization())
                {
                    e.Cancel = true;
                    return;
                }
                setConfirmationText();
                break;
        }
    }

    #endregion

    #region Individual Event Handlers

    protected void gvIndividualPhoneNumbers_OnRowDataBound(object sender, GridViewRowEventArgs e)
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
                Label lblPhoneNumberType = (Label)e.Row.FindControl("lblIndividualPhoneNumberType");
                TextBox tbPhoneNumber = (TextBox)e.Row.FindControl("tbIndividualPhoneNumber");
                var lRadioButtonMarkup = (Literal)e.Row.FindControl("lIndividualRadioButtonMarkup");
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

                tbPhoneNumber.Text = targetIndividual.SafeGetValue<string>(pt.Code + "_PhoneNumber");

                /* We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                    in repeater controls
                    http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->*/

                bool isSelected = targetIndividual.PreferredPhoneNumberType == pt.ID;
                lRadioButtonMarkup.Text = string.Format(
                    @"<input type=radio name=IndividualPhoneNumberPreferredType " +
                    @"id=RowSelector{0} value='{1}' {2} />", e.Row.RowIndex,
                    pt.ID,
                    isSelected ? "checked" : "");


                break;
        }
    }

    protected void rptIndividualAddresses_OnItemDataBound(object sender, RepeaterItemEventArgs e)
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
                Label lblAddressType = (Label)e.Item.FindControl("lblIndividualAddressType");
                AddressControl acAddress = (AddressControl)e.Item.FindControl("acIndividualAddress");
                Literal lNewRowTag = (Literal)e.Item.FindControl("lIndividualNewRowTag");
                HiddenField hfAddressCode = (HiddenField)e.Item.FindControl("hfIndividualAddressCode");


                /* We need to extract the adress from the underlying MemberSuiteObject
                 * address are flattened, and they are in the format of <Code>_Address
                 * where <Code> is the API Code of the address type*/

                lblAddressType.Text = at.Name;
                hfAddressCode.Value = at.Code;  // we'll need this for when we unbind
                acAddress.Address = targetIndividual.SafeGetValue<Address>(at.Code + "_Address");

                /* we're doing THREE columns, so if the index is divisible by three, we want to generate a
                 * start row tag (<tr>)and an end row tag (</tr>)
                 */

            
                if (e.Item.ItemIndex != 0 && (e.Item.ItemIndex + 1) % 3 == 0)
                    lNewRowTag.Text = "</TR><TR>";



                break;
        }
    }

    protected void rptIndividualAddresses_OnItemCreated(object sender, RepeaterItemEventArgs e)
    {
        // we need to set the control host property every time the item is created
        AddressControl acAddress = (AddressControl)e.Item.FindControl("acIndividualAddress");
        msAddressType at = (msAddressType)e.Item.DataItem;


        if (acAddress != null)
        {
            acAddress.Host = this;
            if (at != null) acAddress.IsRequired = at.SafeGetValue<bool>("RequiredInPortal");

        }

    }

    #endregion

    #region Organization Event Handlers

    protected void gvOrganizationPhoneNumbers_OnRowDataBound(object sender, GridViewRowEventArgs e)
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
                Label lblPhoneNumberType = (Label)e.Row.FindControl("lblOrganizationPhoneNumberType");
                TextBox tbPhoneNumber = (TextBox)e.Row.FindControl("tbOrganizationPhoneNumber");
                var lRadioButtonMarkup = (Literal)e.Row.FindControl("lOrganizationRadioButtonMarkup");
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

                tbPhoneNumber.Text = targetOrganization.SafeGetValue<string>(pt.Code + "_PhoneNumber");

                /* We have to use a literal for our radio button due to an ASP.NET bug with radiobuttons
                    in repeater controls
                    http://www.asp.net/learn/data-access/tutorial-51-cs.aspx-->*/

                bool isSelected = targetIndividual.PreferredPhoneNumberType == pt.ID;
                lRadioButtonMarkup.Text = string.Format(
                    @"<input type=radio name=OrganizationPhoneNumberPreferredType " +
                    @"id=RowSelector{0} value='{1}' {2} />", e.Row.RowIndex,
                    pt.ID,
                    isSelected ? "checked" : "");


                break;
        }
    }

    protected void rptOrganizationAddresses_OnItemDataBound(object sender, RepeaterItemEventArgs e)
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
                Label lblAddressType = (Label)e.Item.FindControl("lblOrganizationAddressType");
                AddressControl acAddress = (AddressControl)e.Item.FindControl("acOrganizationAddress");
                Literal lNewRowTag = (Literal)e.Item.FindControl("lOrganizationNewRowTag");
                HiddenField hfAddressCode = (HiddenField)e.Item.FindControl("hfOrganizationAddressCode");

               
                /* We need to extract the adress from the underlying MemberSuiteObject
                 * address are flattened, and they are in the format of <Code>_Address
                 * where <Code> is the API Code of the address type*/

                lblAddressType.Text = at.Name;
                hfAddressCode.Value = at.Code;  // we'll need this for when we unbind
                acAddress.Address = targetOrganization.SafeGetValue<Address>(at.Code + "_Address");

                /* we're doing THREE columns, so if the index is divisible by three, we want to generate a
                 * start row tag (<tr>)and an end row tag (</tr>)
                 */

                if (e.Item.ItemIndex != 0 && (e.Item.ItemIndex + 1) % 3 == 0)
                    lNewRowTag.Text = "</TR><TR>";



                break;
        }
    }

    protected void rptOrganizationAddresses_OnItemCreated(object sender, RepeaterItemEventArgs e)
    {
        // we need to set the control host property every time the item is created
        AddressControl acAddress = (AddressControl)e.Item.FindControl("acOrganizationAddress");
        msAddressType at = (msAddressType)e.Item.DataItem;

        if (acAddress != null)
        {
            acAddress.Host = this;
            if (at != null) acAddress.IsRequired = at.SafeGetValue<bool>("RequiredInPortal");

        }

    }

    #endregion

    #endregion
}