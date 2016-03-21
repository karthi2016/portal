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

        var pageLayout = targetJobPosting.GetAppropriatePageLayout();
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = targetJobPosting.DescribeObject();
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

        SearchResult sr = APIExtensions.GetSearchResult(s, 0, null);
        dvResumes = new DataView(sr.Table);
    }

    protected void Redirect()
    {
        string postingId = targetJobPosting.ID;
        string postingLocalId = Convert.ToString(targetJobPosting.LocalID);

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            if (string.IsNullOrEmpty(postingId))
            {
                var orderId = Request.QueryString["OrderID"];
                var s = new Search(msJobPosting.CLASS_NAME);
                s.AddOutputColumn("ID");
                s.AddOutputColumn("LocalID");
                s.AddCriteria(Expr.Equals("Order", orderId));

                var sr = proxy.ExecuteSearch(s, 0, null);
                if (sr.Success && sr.ResultValue.TotalRowCount > 0)
                {
                    var dr = sr.ResultValue.Table.Rows[0];
                    postingId = Convert.ToString(dr["ID"]);
                    postingLocalId = Convert.ToString(dr["LocalID"]);
                }
                else
                {
                    QueueBannerError(string.Format("Cannot find Job Posting for Order: {0}", sr.Success ? orderId : sr.FirstErrorMessage));
                }
            }

            // Send the confirmation e-mail
            proxy.SendTransactionalEmail(EmailTemplates.CareerCenter.JobPosted, postingId ,
                            ConciergeAPI.CurrentUser.EmailAddress);
        }

        QueueBannerMessage(string.Format("Job Posting #{0} has been posted successfully. A confirmation e-mail has been sent to {1}.", postingLocalId, ConciergeAPI.CurrentUser.EmailAddress));

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
                var saveResult = api.Save(targetJobPosting);
                if (!saveResult.Success)
                {
                    QueueBannerError(string.Format("Unable to save Job Posting: {0}", saveResult.FirstErrorMessage));
                }

                targetJobPosting = saveResult.ResultValue.ConvertTo<msJobPosting>();

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