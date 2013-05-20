using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class continuingeducation_ReportComponentAttendance : PortalPage 
{
    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return PortalConfiguration.CurrentConfig.ComponentSelfReportingMode != MemberSuite.SDK.Types.CertificationsSelfReportingMode.Disabled;
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        Button btnFinish =
                (Button)wzComponent.FindControl("FinishNavigationTemplateContainerID").FindControl("FinishButton");
        RegisterJavascriptConfirmationBox(btnFinish,
                                          "Are you sure you want to submit this registration information?");


    }

    protected void wzComponent_Click(object sender, WizardNavigationEventArgs e)
    {
        if ( ! IsValid )
            return;

         // let's try to find the component

        var result = findComponent();

        if (result.TotalRowCount == 0)    // not found
        {
            cvComponentNotFound.IsValid = false;
            e.Cancel = true;
            return;
        }
        
        // now, has the person registered already?
        DataRow dr = result.Table.Rows[0];
        if ( hasPreviouslyRegisteredForComponent( Convert.ToString( dr["ID"])))
        {
            cvDuplicateRegistration.IsValid = false;
            e.Cancel = true;
            return;
        }

        // set up the fields
        hfComponentID.Value = Convert.ToString(dr["ID"]);   // important
        lblComponentID.Text = Convert.ToString(dr["LocalID"]);
        lblComponentCode.Text = Convert.ToString(dr["Code"]);
        lblComponentName.Text = Convert.ToString(dr["Name"]);

        if (dr["StartDate"] == DBNull.Value)
            lblDate.Text = "n/a/";
        else
        {
            var dtComponentStart = ((DateTime) dr["StartDate"]);
            lblDate.Text = dtComponentStart.ToLongDateString();

            // the date of attendance should default to the start date of the component
            dpDateOfAttendance.SelectedDate = dtComponentStart;
        }

        if (dr[msCertificationComponent.FIELDS.Description] != DBNull.Value)
            lDescription.Text = (string)dr[msCertificationComponent.FIELDS.Description];

        pnlAttendance.Visible = (bool)dr[msCertificationComponent.FIELDS.AllowPartialParticipation];

        if ( dr["Address"] != DBNull.Value )
            lblComponentLocation.Text = Convert.ToString( dr["Address"]);

    }

    private SearchResult findComponent()
    {
        Search s = new Search(msCertificationComponent.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Code", tbComponentID.Text.Trim()));

        int number;
        if (int.TryParse(tbComponentID.Text, out number))
            s.AddCriteria(Expr.Equals("LocalID", number));

        s.GroupType = SearchOperationGroupType.Or; // either or

        s.AddOutputColumn("Name");
        s.AddOutputColumn("Code");
        s.AddOutputColumn("LocalID");
        s.AddOutputColumn(msCertificationComponent.FIELDS.StartDate);
        s.AddOutputColumn(msCertificationComponent.FIELDS.Description);
        s.AddOutputColumn(msCertificationComponent.FIELDS.AllowPartialParticipation);
        s.AddOutputColumn("Address");

        var result = ExecuteSearch(s, 0, 1);
        return result;
    }

    private bool hasPreviouslyRegisteredForComponent( string componentID )
    {
        Search s = new Search(msCertificationComponentRegistration.CLASS_NAME);
        s.AddCriteria(Expr.Equals("Component", componentID ) );
        s.AddCriteria(Expr.Equals("Student", ConciergeAPI.CurrentEntity.ID ));


        return ExecuteSearch(s, 0, 1).TotalRowCount > 0;
    }

   
    protected void wzComponent_OnFinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        if (!IsValid)
        {
            e.Cancel = true;
            return;
        }

        msCertificationComponentRegistration r = CreateNewObject<msCertificationComponentRegistration>();

        r["StartDate"] = dpDateOfAttendance.SelectedDate;
        r.Student = ConciergeAPI.CurrentEntity.ID;
        r.Component = hfComponentID.Value;
        r.DigitalSignature = tbDigitalSignature.Text;
        r.Verified = PortalConfiguration.CurrentConfig.ComponentSelfReportingMode == CertificationsSelfReportingMode.AllowVerified;
        r.ParticipationPercentage = 100;
        if (pnlAttendance.Visible && rbPartial.Checked)
            r.ParticipationPercentage = int.Parse(tbPercentage.Text);

        // let's figure out how many CEU credits to give
        // load the component first
        msCertificationComponent c = LoadObjectFromAPI<msCertificationComponent>(hfComponentID.Value);

        r =  SaveObject(r);

        // now, save the CEU credits
        if ( c.CEUCredits != null )
            foreach (var credit in c.CEUCredits)
            {
                msCEUCredit cc = CreateNewObject<msCEUCredit>();
                cc.Owner = r.Student;
                cc.ComponentRegistration = r.ID;
                cc.CreditDate = c.StartDate ?? DateTime.Today;
                cc.Quantity = credit.Quantity * ( (decimal)r.ParticipationPercentage/100M);
                cc.Type = credit.Type;
                cc.Verified = r.Verified;
                SaveObject(cc); // assign the credit
            }


        QueueBannerMessage("Your component registration was saved successfully.");

        if (PortalConfiguration.CurrentConfig.ShowComponentRegistrationsInPortal)
            GoTo("ViewMyComponents.aspx");
        else
            GoHome();
    }
}