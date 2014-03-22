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
using MemberSuite.SDK.Types;
using Telerik.Web.UI;

public partial class careercenter_CreateEditJobPosting : PortalPage
{

    #region Fields

    protected msJobPosting targetJobPosting;
    protected DataView dvLocations;
    protected DataView dvCategories;

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

        if (targetJobPosting == null)
        {
            // MS-4172 - respect settings
            targetJobPosting = CreateNewObject<msJobPosting>();
            targetJobPosting.Owner = ConciergeAPI.CurrentEntity.ID;
            // do NOT set this, let the default values to precedencetargetJobPosting.AllowOnlineResumeSubmission = , AllowOnlineResumeSubmission = true, IsApproved = false };
        }

        if (!string.IsNullOrWhiteSpace(ContextID))
        {
            targetJobPosting = LoadObjectFromAPI<msJobPosting>(ContextID);

            if (targetJobPosting == null)
            {
                GoToMissingRecordPage();
                return;
            }
        }

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

        loadDataFromConcierge();

        if(string.IsNullOrWhiteSpace(ContextID) && ConciergeAPI.CurrentEntity != null )
        {
            using (var api = GetServiceAPIProxy())
            {
                var er =
                    api.CheckEntitlement(msJobPostingEntitlement.CLASS_NAME, ConciergeAPI.CurrentEntity.ID, null).
                        ResultValue;

                if (er.IsEntitled && er.Quantity > 0)
                    lblJobPostingsAvailable.Text = string.Format("You currently have {0:N0} job posting(s) available.",
                                                                 er.Quantity);
                else
                    lblJobPostingsAvailable.Text +=
                        "  Payment information will be collected after you specify the job posting details.";
            }
            lblJobPostingsAvailable.Visible = true;
        }


        if(dvLocations.Count > 0)
        {
            ddlLocation.DataSource = dvLocations;
            ddlLocation.DataBind();

            trLocation.Visible = true;
        }
        else
        {
            trLocation.Visible = false;
        }


        trCategories.Visible = dvCategories.Count > 0;


        bindJobPostingToPage();

        
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

        CustomFieldSet1.Render();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        List<Search> searches = new List<Search>();

        //Job Posting Locations
        Search sLocations = new Search(msJobPostingLocation.CLASS_NAME);
        sLocations.AddOutputColumn("ID");
        sLocations.AddOutputColumn("Name");
        sLocations.AddSortColumn("Name");

        searches.Add(sLocations);

        //Job Posting Categories
        Search sCategories = new Search(msJobPostingCategory.CLASS_NAME);
        sCategories.AddOutputColumn("ID");
        sCategories.AddOutputColumn("Name");
        sCategories.AddSortColumn("Name");

        searches.Add(sCategories);

        List<SearchResult> results = ExecuteSearches(searches, 0, null);
        dvLocations = new DataView(results[0].Table);
        dvCategories = new DataView(results[1].Table);
    }

    protected void bindJobPostingToPage()
    {
        tbName.Text = targetJobPosting.Name;
        tbResumeEmail.Text = targetJobPosting.ResumeEmail;
        tbCompanyName.Text = targetJobPosting.CompanyName;
        tbDivision.Text = targetJobPosting.Division;
        tbInternalReferenceCode.Text = targetJobPosting.InternalReferenceCode;

        if (dvLocations.Count > 0)
            ddlLocation.SelectedValue = targetJobPosting.Location;

        if (targetJobPosting.PostOn.HasValue)
            dtpPostOn.SelectedDate = targetJobPosting.PostOn;
        else dtpPostOn.SelectedDate = DateTime.UtcNow;
        
        reBody.Content = targetJobPosting.Body;

        foreach (DataRowView drvCategory in dvCategories)
        {
            if ( targetJobPosting.Categories != null && targetJobPosting.Categories.Contains(drvCategory["Name"].ToString()))
                dlbCategories.Destination.Items.Add(new RadListBoxItem(drvCategory["Name"].ToString(), drvCategory["Name"].ToString())); // add it
            else dlbCategories.Source.Items.Add(new RadListBoxItem(drvCategory["Name"].ToString(), drvCategory["Name"].ToString())); // add it

        }
    }

    protected void unbindJobPosting()
    {
        if (targetJobPosting == null)
        {
            targetJobPosting = new msJobPosting
                                   {
                                       Owner = ConciergeAPI.CurrentEntity.ID,
                                       AllowOnlineResumeSubmission = true,
                                       IsApproved = false
                                   };
            CustomFieldSet1.MemberSuiteObject = targetJobPosting;
        }

        targetJobPosting.Name = tbName.Text;
        targetJobPosting.ResumeEmail = tbResumeEmail.Text;
        targetJobPosting.CompanyName = tbCompanyName.Text;
        targetJobPosting.Division = tbDivision.Text;
        targetJobPosting.InternalReferenceCode = tbInternalReferenceCode.Text;

        if (trLocation.Visible)
            targetJobPosting.Location = ddlLocation.SelectedValue;

        DateTime postDate = dtpPostOn.SelectedDate.HasValue ? dtpPostOn.SelectedDate.Value : DateTime.Now;
        targetJobPosting.PostOn = postDate;
        targetJobPosting.Body = reBody.Content;
        targetJobPosting.AllowOnlineResumeSubmission = true;
        targetJobPosting.ExpirationDate = postDate.AddDays(PortalConfiguration.Current.DefaultJobPostingExpiration);

        targetJobPosting.Categories = (from item in dlbCategories.Destination.Items
                                       select item.Value).ToList();

        CustomFieldSet1.Harvest();
    }

    #endregion

    #region Event Handlers

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        unbindJobPosting();
        MultiStepWizards.PostAJob.JobPosting = targetJobPosting;

        GoTo("~/careercenter/ConfirmJobPosting.aspx");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    #endregion
}