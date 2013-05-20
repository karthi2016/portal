using System;
using System.Collections.Generic;
using System.Configuration;
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

public partial class careercenter_ViewJobPosting : PortalPage
{
    #region Fields

    protected msJobPosting targetJobPosting;
    protected DataView dvResumes;
    protected msJobPostingLocation targetJobPostingLocation;
    protected DataRow drMembership;

    #endregion

    #region Properties

    protected override bool IsPublic
    {
        get { return true; }
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

        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            loadTargetObject(proxy);
        }

        if (targetJobPosting == null)
        {
            GoToMissingRecordPage();
            return;
        }

        if (ConciergeAPI.CurrentEntity != null)
        {
            using (var api = GetServiceAPIProxy())
            {
                var searchResult = api.ExecuteMSQL("select Name, ID from Resume where Owner='" +
                                                   ConciergeAPI.CurrentEntity.ID + "' order by Name", 0, null).
                    ResultValue.
                    SearchResult;
                ddlSelectResume.DataSource = searchResult.Table;

                if (searchResult.TotalRowCount == 0)
                {
                    lbApply.Visible = false;
                    lblApply.Visible = true;
                }
                else
                {
                    lbApply.Visible = true;
                    lblApply.Visible = false;
                }

            }
            ddlSelectResume.DataTextField = "Name";
            ddlSelectResume.DataValueField = "ID";
            ddlSelectResume.DataBind();
        }

        RegisterJavascriptConfirmationBox(btnApply,
                                          "Are you sure you would like to send the selected resume to this company in this posting?");
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

        if (ConciergeAPI.CurrentEntity == null)
        {
            divTasks.Visible = false;
            return;
        }

        //Load resumes and set visibility of the apply option.  This has to happen in Page_Load not InitializePage for AJAX to work properly
        if (!targetJobPosting.AllowOnlineResumeSubmission || string.IsNullOrWhiteSpace(targetJobPosting.ResumeEmail))
        {
            liApply.Visible = false;
            return;
        }



    }

    protected override bool CheckSecurity()
    {
        // MS-2142 - Owners should ALWAYS be able to view postings
        if (targetJobPosting != null && ConciergeAPI.CurrentEntity != null &&
            targetJobPosting.Owner == ConciergeAPI.CurrentEntity.ID)
            return true;

        switch (PortalConfiguration.Current.JobPostingAccessMode)
        {
            case JobPostingAccess.PublicAccess:
                return true;
            case JobPostingAccess.AnyRegisteredUser:
                return ConciergeAPI.CurrentEntity != null;
            case JobPostingAccess.MembersOnly:
                return isActiveMember();
        }

        return false;
    }

    #endregion

    #region Methods

    public bool isActiveMember()
    {
        if (drMembership == null)
            return false;

        //Check if the appropriate fields exist - if they do not then the membership module is inactive
        if (
            !(drMembership.Table.Columns.Contains("Membership") &&
              drMembership.Table.Columns.Contains("Membership.ReceivesMemberBenefits") &&
              drMembership.Table.Columns.Contains("Membership.TerminationDate")))
            return false;

        //Check there is a membership
        if (string.IsNullOrWhiteSpace(Convert.ToString(drMembership["Membership"])))
            return false;

        //Check the membership indicates membership benefits
        if (!drMembership.Field<bool>("Membership.ReceivesMemberBenefits"))
            return false;

        //At this point if the termination date is null the member should be able to see the restricted directory
        DateTime? terminationDate = drMembership.Field<DateTime?>("Membership.TerminationDate");

        if (terminationDate == null)
            return true;

        //There is a termination date so check if it's future dated
        return terminationDate > DateTime.Now;
    }

    protected void loadTargetObject(IConciergeAPIService proxy)
    {
        targetJobPosting = LoadObjectFromAPI<msJobPosting>(ContextID);

        if (targetJobPosting != null && !string.IsNullOrWhiteSpace(targetJobPosting.Location))
            targetJobPostingLocation = LoadObjectFromAPI<msJobPostingLocation>(targetJobPosting.Location);

        if (ConciergeAPI.CurrentEntity != null &&
            PortalConfiguration.Current.JobPostingAccessMode == JobPostingAccess.MembersOnly)
        {
            Search sMembership = new Search(msEntity.CLASS_NAME);
            sMembership.AddOutputColumn("ID");
            sMembership.AddOutputColumn("Membership");
            sMembership.AddOutputColumn("Membership.ReceivesMemberBenefits");
            sMembership.AddOutputColumn("Membership.TerminationDate");
            sMembership.AddCriteria(Expr.Equals("ID", ConciergeAPI.CurrentEntity.ID));
            sMembership.AddSortColumn("ID");

            SearchResult srMembership = ExecuteSearch(proxy, sMembership, 0, 1);
            drMembership = srMembership != null && srMembership.Table != null && srMembership.Table.Rows.Count > 0
                               ? srMembership.Table.Rows[0]
                               : null;
        }
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

    public void lbApply_Click(object sender, EventArgs e)
    {
        pnlSelectResume.Visible = true;
    }




    #endregion

    protected void btnApply_Click(object sender, EventArgs e)
    {
        using (IConciergeAPIService serviceProxy = GetServiceAPIProxy())
        {
            serviceProxy.ApplyToJobPosting(targetJobPosting.ID, ddlSelectResume.SelectedValue);
        }

        QueueBannerMessage("Resume successfully sent.");

        Refresh();
    }



}