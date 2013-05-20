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

public partial class membership_PurchaseMembership2 : PortalPage
{
    #region Fields

    private msMembership targetMembership;
    protected msEntity targetEntity;

    #endregion

    #region Properties

    public bool Complete
    {
        get
        {
            bool result;
            if (!bool.TryParse(Request.QueryString["complete"], out result))
                return false;

            return result;
        }
    }

    public string OrderId
    {
        get { return Request.QueryString["OrderID"]; }
    }

    #endregion

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

        if (Complete)
            ClearAndRedirect();

        targetMembership = MultiStepWizards.RenewMembership.Membership;
        targetEntity = MultiStepWizards.RenewMembership.Entity;

        if (targetMembership == null || targetMembership.MembershipOrganization == null || targetEntity == null) // session expired, go back home
            GoHome();

    }

    protected override void InitializePage()
    {
        base.InitializePage();

        lblMembershipFee.Text = targetMembership.SafeGetValue<string>("Product_Name");
        lblMembershipType.Text = targetMembership.SafeGetValue<string>("Type_Name");

        // we know the membership organization isn't null
        var mo = LoadObjectFromAPI<msMembershipOrganization>(targetMembership.MembershipOrganization);

        if (mo == null) GoHome(); // maybe if it got deleted in the middle of this process

        setupChapters(mo);
        setupSections(mo);
        setupOtherInformation(mo);

        // ok, let's add the additional products
        Search sProducts = new Search(msProduct.CLASS_NAME);
        sProducts.AddCriteria(Expr.Equals(msProduct.FIELDS.ShowOnMembershipForm, true));


        var results = ExecuteSearch(sProducts, 0, null);

        List<string> productsToDescribe = new List<string>();

        foreach (DataRow dr in results.Table.Rows)
            productsToDescribe.Add(Convert.ToString(dr["ID"]));

        List<ProductInfo> describedProducts = DescribeProducts(targetEntity.ID, productsToDescribe);

        setupAdditionalItems(mo, describedProducts);
        setupFundraisingProducts(mo, describedProducts);

    }

    private void setupFundraisingProducts(msMembershipOrganization mo, List<ProductInfo> dtAllProducts)
    {
        var donationProducts = dtAllProducts.FindAll(x => x.ProductType == msFundraisingProduct.CLASS_NAME);

        if (donationProducts.Count == 0)
        {
            divDonations.Visible = false;
            return;
        }
        rptDonations.DataSource = donationProducts;
        rptDonations.DataBind();
    }

    private void setupAdditionalItems(msMembershipOrganization mo, List<ProductInfo> dtAllProducts)
    {
        // we only want non-fundraising products
        var additionalItems = dtAllProducts.FindAll(x => x.ProductType != msFundraisingProduct.CLASS_NAME);

        if (additionalItems.Count == 0)
        {
            divOtherProducts.Visible = false;
            return;
        }

        rptAdditionalItems.DataSource = additionalItems;
        rptAdditionalItems.DataBind();
    }

    #region Setup Methods


    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetMembership;

        var pageLayout = GetAppropriatePageLayout(targetMembership);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(msMembership.CLASS_NAME).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;

        CustomFieldSet1.Render();
    }

    private void setupOtherInformation(msMembershipOrganization mo)
    {
        cbMembershipDirectoryOptOut.Checked = targetMembership.MembershipDirectoryOptOut;
        cbAutomaticallyPay.Checked = targetMembership.AutomaticallyPayForRenewal;

        CustomFieldSet1.MemberSuiteObject = targetMembership;
        CustomFieldSet1.DataBind();
    }

    protected void ClearAndRedirect()
    {
        MultiStepWizards.RenewMembership.Clear();
        GoTo(string.Format("~/orders/OrderComplete.aspx?contextID={0}", OrderId));
    }

    private DataTable _sections;
    private void setupSections(msMembershipOrganization mo)
    {
        switch (mo.SectionMode)
        {
            case SectionMode.SectionsDisabled:
                divSections.Visible = false;
                return;



        }

        // let's pull all of the chapters
        Search sSections = new Search { Type = msSection.CLASS_NAME };
        sSections.AddCriteria(Expr.Equals(msSection.FIELDS.MembershipOrganization, targetMembership.MembershipOrganization));
        sSections.AddCriteria(Expr.Equals(msSection.FIELDS.IsActive, true));
        sSections.AddSortColumn("Type.Name");
        sSections.AddSortColumn("Name");
        sSections.AddOutputColumn("Name");
        sSections.AddOutputColumn("Type.Name");

        _sections = ExecuteSearch(sSections, 0, null).Table;


        // ok, let's pull out all of the section types
        List<string> sectionTypes = new List<string>();
        foreach (DataRow dr in _sections.Rows)
        {
            string typeName = "";
            if (dr["Type.Name"] != DBNull.Value)
                typeName = Convert.ToString(dr["Type.Name"]);
            if (!sectionTypes.Contains(typeName))
                sectionTypes.Add(typeName);
        }

        rptSections.DataSource = sectionTypes;
        rptSections.DataBind();

    }

    private void setupChapters(msMembershipOrganization mo )
    {
        switch (mo.ChapterMode)
        {
            case ChapterMode.ChaptersDisabled:
                divChapter.Visible = false; // don't show it
                return;

            case ChapterMode.MemberJoinsOneChapter:
                divChapter.Visible = true;
                divAdditionalChapters.Visible = false;
                break;

            case ChapterMode.MemberCanJoinMultipleChapters:
                divChapter.Visible = true;
                divAdditionalChapters.Visible = true;
                break;

        }

        // let's pull all of the chapters
        List<NameValueStringPair> tblChapters;
        using (var api = GetServiceAPIProxy())
        {
            tblChapters = api.GetApplicableChaptersForMembershipType(targetMembership.Type).ResultValue ;
        }
        ddlSelectChapter.DataSource = tblChapters;
            ddlSelectChapter.DataTextField = "Name";
            ddlSelectChapter.DataValueField = "Value";
            ddlSelectChapter.DataBind();

        
        if (divAdditionalChapters.Visible)    // bind the list box, too
        {
            lbAdditionalChapters.DataSource = tblChapters;
            lbAdditionalChapters.DataTextField = "Name";
            lbAdditionalChapters.DataValueField = "Value";
            lbAdditionalChapters.DataBind();
        }

        ddlSelectChapter.Items.Insert(0, new ListItem("---- Select a Chapter ----", ""));

        // ok - are we suggesting a chapter based on zip code?
        if (mo.ChapterPostalCodeMappingMode != ChapterPostalCodeMappingMode.Disabled)
        {
            MemberSuiteObject msoChapter = null;
            using (var api = GetServiceAPIProxy())
                msoChapter = api.SuggestChapter(mo.ID, targetEntity.ID).ResultValue;

            if (msoChapter != null)   // have have a chapter
            {
                ListItem li = ddlSelectChapter.Items.FindByValue(msoChapter.SafeGetValue<string>("ID"));

                if (li != null)   // we have a match
                {
                    li.Selected = true;

                    if (mo.ChapterPostalCodeMappingMode == ChapterPostalCodeMappingMode.Assign)
                    {
                        ddlSelectChapter.Enabled = false; // can't be changed
                        trChapterAssigned.Visible = true;
                    }

                    // if a chapter matches, that's it, we're done
                    return;
                }
            }

        }


        // let's try and set to the default chapter
        if (targetMembership != null && targetMembership.Chapters != null)
        {
            // find the primary
            var cPrimary = (targetMembership.Chapters.Find(x => x.IsPrimary));
            if (cPrimary!= null)
                ddlSelectChapter.SafeSetSelectedValue(cPrimary.Chapter);

            // now, let's try to select the additional
            if ( divAdditionalChapters.Visible )
            {
                foreach (var c in targetMembership.Chapters)
                    if ((c.ExpirationDate == null || c.ExpirationDate == targetMembership.ExpirationDate) &&
                        (cPrimary == null || c.Chapter != cPrimary.Chapter) )
                {
                    ListItem li = lbAdditionalChapters.Items.FindByValue(c.Chapter);
                    if (li != null) li.Selected = true;
                }
            }
        }



    }

    #endregion

    #region Unbinding

    private msOrder unbindObjectsFromPage()
    {
        msOrder mso = new msOrder();
        mso.BillTo = mso.ShipTo = targetEntity.ID;

        // let's add the membership product
        string parentItem = Guid.NewGuid().ToString();
        msOrderLineItem msPrimaryMembershipItem = new msOrderLineItem { Quantity = 1, Product = targetMembership.Product, OrderLineItemID = parentItem };
        mso.LineItems.Add(msPrimaryMembershipItem);

        msPrimaryMembershipItem.Options = new List<NameValueStringPair>();

        if (cbAutomaticallyPay.Checked)
            msPrimaryMembershipItem.Options.Add(new NameValueStringPair(msMembership.FIELDS.AutomaticallyPayForRenewal, true.ToString()));

        if (cbMembershipDirectoryOptOut.Checked)
            msPrimaryMembershipItem.Options.Add(new NameValueStringPair(msMembership.FIELDS.MembershipDirectoryOptOut, true.ToString()));

        CustomFieldSet1.Harvest();

        foreach (var fieldValuePair in targetMembership.Fields)
            if (fieldValuePair.Key.EndsWith("__c"))  // make it an option
            {
                string value = null;

                if (fieldValuePair.Value != null)
                {
                    if (fieldValuePair.Value is List<string>)
                    {
                        var valueAsList = (List<string>) fieldValuePair.Value;
                        value = string.Join("|", valueAsList);
                    }
                    else
                    {
                        value = fieldValuePair.Value.ToString();
                    }
                }

                msPrimaryMembershipItem.Options.Add(new NameValueStringPair(fieldValuePair.Key, value));
            }


        // now, everything else
        unbindChapters(mso, parentItem);
        unbindSections(mso, parentItem);
        unbindAdditionalItems(mso, parentItem);
        unbindDonations(mso, parentItem);

        return mso;
    }

    private void unbindDonations(msOrder mso, string parentItem)
    {
        if (!divDonations.Visible)
            return;

        foreach (RepeaterItem ri in rptDonations.Items)
        {
            TextBox tbAmount = (TextBox)ri.FindControl("tbAmount");
            if (string.IsNullOrEmpty(tbAmount.Text))
                continue;

            HiddenField hfProductID = (HiddenField)ri.FindControl("hfProductID");

            msOrderLineItem li = new msOrderLineItem();
            li.Total = decimal.Parse(tbAmount.Text);
            li.PriceOverride = true;        // IMPORTANT - all donations are price overriden!
            li.UnitPrice = li.Total;

            if (li.Total <= 0)
                continue;   // don't add

            li.Product = hfProductID.Value;
            li.LinkedOrderLineItemID = parentItem;

            mso.LineItems.Add(li);

        }
    }

    private void unbindAdditionalItems(msOrder mso, string parentItem)
    {
        if (!divOtherInformation.Visible)
            return;

        foreach (RepeaterItem ri in rptAdditionalItems.Items)
        {
            TextBox tbQuantity = (TextBox) ri.FindControl("tbQuantity");
            HiddenField hfProductID = (HiddenField) ri.FindControl("hfProductID");

            msOrderLineItem li = new msOrderLineItem();
            li.Quantity = decimal.Parse(tbQuantity.Text);
            if (li.Quantity <= 0)
                continue;   // don't add

            li.Product = hfProductID.Value;
            li.LinkedOrderLineItemID = parentItem;

            mso.LineItems.Add(li);

        }
    }

    private void unbindSections(msOrder mso, string membershipItemGuid)
    {
        if (!divSections.Visible)
            return; // they're not even enabled

        using (var api = GetConciegeAPIProxy())
        {
            foreach (RepeaterItem ri in rptSections.Items)
            {
                var cbSections = (CheckBoxList) ri.FindControl("cbSections");
                if (cbSections == null) continue;


                foreach (ListItem item in cbSections.Items)
                    if (item.Selected) // they ased for it
                    {
                        var productID = api.GetDefaultSectionProduct(targetMembership.Type, item.Value ).ResultValue.SafeGetValue<string>("ID");
                        mso.LineItems.Add(new msOrderLineItem { Quantity = 1, Product = productID, LinkedOrderLineItemID = membershipItemGuid });
      
                    }
            }
        }
    }

    private void unbindChapters(msOrder orderToUse, string membershipItemGuid)
    {

        if (!divChapter.Visible)    // chapters aren't even enabled
            return;


        List<string> chapters = new List<string>();

        // ok, first let's pull the primary chapter
        var primaryChapter = ddlSelectChapter.SelectedValue;
        if (primaryChapter != null)
            chapters.Add(primaryChapter);

        // now, if additional chapters are supported, we'll pull those
        if (divAdditionalChapters.Visible)
            foreach (ListItem secondaryChapter in lbAdditionalChapters.Items)
                if (secondaryChapter.Selected && !chapters.Contains(secondaryChapter.Value))    // don't duplicate
                    chapters.Add(secondaryChapter.Value);

        // ok, now we need to get the products for these chapters
        using (var api = GetConciegeAPIProxy())
        {
            foreach (var chapter in chapters)
            {
                var productID = api.GetDefaultChapterProduct(targetMembership.Type, chapter).ResultValue.SafeGetValue<string>("ID");
                orderToUse.LineItems.Add(new msOrderLineItem { Quantity = 1, Product = productID, LinkedOrderLineItemID = membershipItemGuid });
            }
        }

        // ok, that's it

    }

    #endregion

    #region Buttons/Events

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        // set our transient shopping cart
   
        MultiStepWizards.PlaceAnOrder.OrderCompleteUrl = string.Format("~/membership/PurchaseMembership2.aspx?contextID={0}&complete=true", ContextID);

        MultiStepWizards.PlaceAnOrder.InitiateOrderProcess(unbindObjectsFromPage());
    }



    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MultiStepWizards.RenewMembership.Clear();
        GoHome();
    }

    #endregion

    protected void rptSections_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        string sectionTypeName = (string)e.Item.DataItem;

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
                Label lblSectionType = (Label)e.Item.FindControl("lblSectionType");
                CheckBoxList cbSections = (CheckBoxList)e.Item.FindControl("cbSections");

                lblSectionType.Text = sectionTypeName;
             
                foreach (DataRow dr in _sections.Rows)
                {
                    string typeName = "";
                    if (dr["Type.Name"] != DBNull.Value)
                        typeName = Convert.ToString(dr["Type.Name"]);

                    if (typeName != sectionTypeName) continue;  // doesn't apply

                    var sectionID = Convert.ToString(dr["ID"]);
                    ListItem li = new ListItem(Convert.ToString(dr["Name"]),
                                                     sectionID);
                    cbSections.Items.Add(li);

                    // now, is this selected? (are they in this section already)
                    if (targetMembership != null &&
                        targetMembership.Sections != null &&
                        targetMembership.Sections.Exists(x => x.Section == sectionID
                            && (x.ExpirationDate == null || x.ExpirationDate == targetMembership.ExpirationDate)))
                        li.Selected = true;
                }


                break;
        }
    }


    protected void rptDonations_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ProductInfo pi = (ProductInfo)e.Item.DataItem;

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
                TextBox tbAmount = (TextBox)e.Item.FindControl("tbAmount");
                CompareValidator cvAmount = (CompareValidator)e.Item.FindControl("cvAmount");
                Label lblProductName = (Label)e.Item.FindControl("lblProductName");
                 HiddenField hfProductID = (HiddenField)e.Item.FindControl("hfProductID");

                hfProductID.Value = pi.ProductID;

                tbAmount.Text = pi.Price.ToString("N2");
                cvAmount.ErrorMessage = string.Format("You have entered an invalid donation amount for {0}", pi.ProductName);
                lblProductName.Text = pi.ProductName;

                break;
        }

    }
    protected void rptAdditionalItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ProductInfo pi = (ProductInfo)e.Item.DataItem;

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
                TextBox tbQuantity = (TextBox)e.Item.FindControl("tbQuantity");
                CompareValidator cvQuantity = (CompareValidator)e.Item.FindControl("cvQuantity");
                Label lblProductName = (Label)e.Item.FindControl("lblProductName");
                Label lblProductPrice = (Label)e.Item.FindControl("lblProductPrice");
                HiddenField hfProductID = (HiddenField)e.Item.FindControl("hfProductID");

                hfProductID.Value = pi.ProductID;


                cvQuantity.ErrorMessage = string.Format("You have entered an invalid donation amount for {0}", pi.ProductName);
                lblProductName.Text = pi.ProductName;
                lblProductPrice.Text = pi.DisplayPriceAs ?? pi.Price.ToString("C");

                break;
        }
    }
}