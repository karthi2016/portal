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
using Telerik.Web.UI;

public partial class continuingeducation_ViewCertification : PortalPage 
{
    protected msCertification targetCertification;
    protected string programName;
  

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetCertification = LoadObjectFromAPI<msCertification>(ContextID);
        if (targetCertification == null)
        {
            GoToMissingRecordPage();
            return;
        }

        programName = GetConciegeAPIProxy().GetName(targetCertification.Program).ResultValue;
        
    }

    #region Custom Fields

    protected override void InstantiateCustomFields(IConciergeAPIService proxy)
    {
        CustomFieldSet1.MemberSuiteObject = targetCertification ;

        var pageLayout = GetAppropriatePageLayout(targetCertification);
        if (pageLayout == null || pageLayout.Metadata == null || pageLayout.Metadata.IsEmpty())
            return;

        // setup the metadata
        CustomFieldSet1.Metadata = proxy.DescribeObject(targetCertification.ClassType ).ResultValue;
        CustomFieldSet1.PageLayout = pageLayout.Metadata;

        CustomFieldSet1.AddReferenceNamesToTargetObject(proxy);

        CustomFieldSet1.Render();
    }

    #endregion



    protected override bool CheckSecurity()
    {
        if (targetCertification != null && targetCertification.Certificant != ConciergeAPI.CurrentEntity.ID)
            return false;

        return base.CheckSecurity();
    }
    protected void btnHistory_Click(object sender, EventArgs e)
    {
        GoTo("ViewCertificationHistory.aspx");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        setupCEURequirements();
        setupRecommendations();
        setupExamRequirements();
    }

    private void setupExamRequirements()
    {
        Search s = new Search("CertificationRequiredExam");
        s.AddCriteria(Expr.Equals("Certification", targetCertification.ID));
        s.AddOutputColumn("Type.Name");
        s.AddOutputColumn("Passed");
        s.AddSortColumn("Type.Name");

        var dt = ExecuteSearch(s, 0, null).Table;

        if (dt.Rows.Count == 0) return;

        pnlExamRequirements.Visible = true;
        rgExams.DataSource = dt;
        rgExams.DataBind();
    }

    private void setupRecommendations()
    {
        Search s = new Search(msCertificationRecommendation.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msCertificationRecommendation.FIELDS.Certification, targetCertification.ID));
        s.AddOutputColumn("Name");
        s.AddOutputColumn("EmailAddress");
        s.AddOutputColumn("Status");

        var dt = ExecuteSearch(s, 0, null).Table;

        if (dt.Rows.Count == 0) return;

        pnlRecommendations.Visible = true;
        rgRecommendations.DataSource = dt;
        rgRecommendations.DataBind();
    }

    private void setupCEURequirements()
    {
        Search s = new Search("CertificationRequiredCEUCredit");
        s.AddCriteria(Expr.Equals("Certification", targetCertification.ID));
        s.AddOutputColumn("Type.Name");
        s.AddOutputColumn("QuantityRequired");
        s.AddOutputColumn("Quantity");
        s.AddOutputColumn("QuantityNeeded");
        s.AddSortColumn("Type.Name");
        
        var dt = ExecuteSearch(s, 0, null).Table;

        if ( dt.Rows.Count  == 0 ) return;
         
        pnlCEURequirements.Visible = true;
        rgCEU.DataSource = dt;
        rgCEU.DataBind();

    }

    protected void rgRecommendations_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (Page.IsPostBack)
            return;

        DataRowView drv = (DataRowView) e.Item.DataItem;

        if (drv == null) return;

        LinkButton lbResend = (LinkButton)e.Item.FindControl("lbResend");

        if (lbResend == null) return ;

        if (Convert.ToString(drv["Status"]) != "Pending")
            lbResend.Visible = false;   // no need to show it

        lbResend.CommandName = "Resend";
        lbResend.CommandArgument = Convert.ToString(drv["ID"]);
    }

    protected void rgRecommendations_ItemCommand(object sender, GridCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Resend":
                string id = Convert.ToString(e.CommandArgument);
                using (var api = GetServiceAPIProxy())
                    api.SendEmail("BuiltIn:CertificationRecommendation", new List<string> {id}, null);

                var rec = LoadObjectFromAPI<msCertificationRecommendation>(id);
                QueueBannerMessage("An recommendation submission request has successfully been sent to " + rec.EmailAddress);
                Refresh();
                break;
        }
    }
}