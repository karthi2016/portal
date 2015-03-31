using System;
using System.Collections.Generic;
using System.Data;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Constants;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class careercenter_ConfirmJobPosting : PortalPage
{
    #region Fields

    protected msJobPosting targetJobPosting;
    protected DataView dvResumes;
    protected msJobPostingLocation targetJobPostingLocation;

    #endregion

    #region Properties

    public bool Post
    {
        get
        {
            bool result;
            return bool.TryParse(Request.QueryString["post"], out result) && result;
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

        targetJobPosting = MultiStepWizards.PostAJob.JobPosting;
        
        if(targetJobPosting == null)
        {
            GoTo("~/careercenter/CreateEditJobPosting.aspx");
            return;
        }

        if(Post)
            Redirect();

        if (!string.IsNullOrWhiteSpace(targetJobPosting.Location))
            targetJobPostingLocation = LoadObjectFromAPI<msJobPostingLocation>(targetJobPosting.Location);
    }

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetJobPosting;

        var pageLayout = GetAppropriatePageLayout(targetJobPosting);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(msJobPosting.CLASS_NAME).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;

        CustomFieldSet1.AddReferenceNamesToTargetObject(proxy);

        CustomFieldSet1.Render();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        loadDataFromConcierge();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search s = new Search(msResume.CLASS_NAME);
        s.AddOutputColumn("ID");
        s.AddOutputColumn("LocalID");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("File");
        s.AddOutputColumn("IsActive");
        s.AddCriteria(Expr.Equals("Owner", ConciergeAPI.CurrentEntity.ID));

        SearchResult sr = ExecuteSearch(s, 0, null);
        dvResumes = new DataView(sr.Table);
    }

    protected void Redirect()
    {
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
                
            //Send the confirmation e-mail
            proxy.SendEmail(EmailTemplates.CareerCenter.JobPosted, new List<string> { targetJobPosting.ID },
                            ConciergeAPI.CurrentUser.EmailAddress);
        }

        QueueBannerMessage(string.Format("Job Posting #{0} has been posted successfully. A confirmation e-mail has been sent to {1}.", targetJobPosting.LocalID, ConciergeAPI.CurrentUser.EmailAddress));

        MultiStepWizards.PostAJob.JobPosting = null;
        GoHome();
    }

    #endregion

    #region Event Handlers

    protected override void InitializePage()
    {
        base.InitializePage();

        trDivision.Visible = ! string.IsNullOrWhiteSpace(targetJobPosting.Division);
        trCode.Visible = !string.IsNullOrWhiteSpace(targetJobPosting.InternalReferenceCode);
        trLocation.Visible = targetJobPostingLocation != null;
        if (targetJobPosting.Categories != null && targetJobPosting.Categories.Count > 0)
        {
            trCategories.Visible = true;
            foreach (var c in targetJobPosting.Categories)
                lblCategories.Text += c + ", ";
            lblCategories.Text = lblCategories.Text.Trim().TrimEnd(',');
        }
        else
            trCategories.Visible = false;

    }

    public void btnEdit_Click(object sender, EventArgs e)
    {
        GoTo("~/careercenter/CreateEditJobPosting.aspx");
    }

    public void btnPost_Click(object sender, EventArgs e)
    {
        using (var api = GetServiceAPIProxy())
        {
            var er =
                api.CheckEntitlement(msJobPostingEntitlement.CLASS_NAME, ConciergeAPI.CurrentEntity.ID, null).
                    ResultValue;

            if (er.IsEntitled && er.Quantity > 0)
            {
                api.Save(targetJobPosting);
                // MS-5386 Adjust entitlement (-1). Otherwise user will be able to use the entitlement forever.
                api.AdjustEntitlementAvailableQuantity(ConciergeAPI.CurrentEntity.ID, msJobPostingEntitlement.CLASS_NAME,
                    null, -1);
                Redirect();
            }
            else
            {
                MultiStepWizards.PostAJob.JobPosting = targetJobPosting;
                GoTo("~/careercenter/SelectCareerCenterProduct.aspx?transientJobPosting=true");
            }
        }


    }

    #endregion
}