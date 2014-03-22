using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_ViewTableSeats : PortalPage 
{
    #region Initialization

    protected msEvent targetEvent;
    protected msEntity targetEntity;
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

        targetEvent = LoadObjectFromAPI(ContextID).ConvertTo<msEvent>();
        if (targetEvent == null)
        {
            GoToMissingRecordPage();
            return;
        }

        targetEntity = ConciergeAPI.CurrentEntity;

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

        using (var api = GetServiceAPIProxy())
        {
            Search s = new Search(msEventTableReservation.CLASS_NAME);
            s.AddCriteria(Expr.Equals("Event", targetEvent.ID));
            s.AddCriteria(Expr.Equals(msEventTableReservation.FIELDS.Owner, targetEntity.ID));
            s.AddCriteria(Expr.IsBlank(msEventTableReservation.FIELDS.CancellationDate));

            s.AddOutputColumn(msEventTableReservation.FIELDS.NumberOfSeats);
            s.AddOutputColumn(msEventTableReservation.FIELDS.Table + ".Name" );
            s.AddOutputColumn(msEventTableReservation.FIELDS.TableType + ".Name");
            s.AddOutputColumn(msEventTableReservation.FIELDS.Fee + ".Name");

            var results = api.ExecuteSearch(s, 0, null).ResultValue;

            if (results.TotalRowCount == 0)
                lNoRecords.Visible = true;
            else
            {
                var dt   = results.Table;

                foreach (DataRow dr in dt.Rows)
                    if (dr["Table.Name"] == DBNull.Value)
                        dr["Table.Name"] = "Unassigned";

                rgTableReservations.DataSource = dt;
                rgTableReservations.DataBind();
            }
        }

    }


    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

        if (targetEvent.RegistrationMode != EventRegistrationMode.Tabled)
            return false;


        return true;
    }


    #endregion

    protected void btnBack_Click(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}