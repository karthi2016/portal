using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Searching;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class directory_SearchDirectory_ViewDetails : PortalPage
{
    #region Fields

    protected SearchManifest targetManifest;
    protected DataView dvDetailsFields;
    protected DataRow targetDetailsRow;
    protected DataRow drMembership;

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

        targetManifest = buildSearchManifest();
        
        if(targetManifest == null || string.IsNullOrWhiteSpace(ContextID))
        {
            GoToMissingRecordPage();
            return;
        }

        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            Search sMembership = new Search(msEntity.CLASS_NAME) { ID = msMembership.CLASS_NAME };
            sMembership.AddOutputColumn("ID");
            sMembership.AddOutputColumn("Membership");
            sMembership.AddOutputColumn("Membership.ReceivesMemberBenefits");
            sMembership.AddOutputColumn("Membership.TerminationDate");
            sMembership.AddCriteria(Expr.Equals("ID", ConciergeAPI.CurrentEntity.ID));
            sMembership.AddSortColumn("ID");

            SearchResult srMembership = proxy.ExecuteSearch(sMembership, 0, 1).ResultValue;
            drMembership = srMembership != null && srMembership.Table != null &&
                           srMembership.Table.Rows.Count > 0
                               ? srMembership.Table.Rows[0]
                               : null;
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

        if (targetDetailsRow == null)
        {
            GoToMissingRecordPage();
            return;
        }

        generateFieldsDataView();
        rptDirectoryFields.DataSource = dvDetailsFields;
        rptDirectoryFields.DataBind();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return isMembershipDirectoryAvailable();
    }

    private bool isMembershipDirectoryAvailable()
    {
        if (!PortalConfiguration.Current.MembershipDirectoryEnabled)
            return false;

        //If the directory is enabled and not restricted to members it's available and no need to check membership status
        if (!PortalConfiguration.Current.MembershipDirectoryForMembersOnly)
            return true;

        //Directory is for members only
        return isActiveMember();
    }

    protected bool isActiveMember()
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

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search s = Search.FromManifest(targetManifest);
        s.AddCriteria(Expr.Equals("Owner.ID", ContextID));

        SearchResult searchResult = ExecuteSearch(s, 0, null);
        if (searchResult.Table != null && searchResult.Table.Rows.Count > 0)
            targetDetailsRow = searchResult.Table.Rows[0];
    }

    private SearchManifest buildSearchManifest()
    {
        SearchManifest result;

        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            result = proxy.DescribeSearch(msMembership.CLASS_NAME, null).ResultValue;
            result.Fields.Clear();
            result.Fields =
                proxy.GetSearchFieldMetadataFromFullPath(msMembership.CLASS_NAME, null, PortalConfiguration.Current.MembershipDirectoryDetailsFields).ResultValue;
        }

        result.DefaultSelectedFields = (
            from field in result.Fields
            select new SearchOutputColumn { Name = field.Name, DisplayName = field.Label }).ToList();

        return result;
    }

    protected void generateFieldsDataView()
    {
        DataTable fieldTable = new DataTable();
        fieldTable.Columns.Add(new DataColumn("FieldName", typeof (string)));
        fieldTable.Columns.Add(new DataColumn("FieldValue", typeof (string)));

        foreach (DataColumn dcDetailsColumn in targetDetailsRow.Table.Columns)
        {
            if(dcDetailsColumn.ColumnName == "ID" || dcDetailsColumn.ColumnName == "ROW_NUMBER")
                continue;

            DataColumn column = dcDetailsColumn;
            var field = targetManifest.Fields.Where(x => x.Name == column.ColumnName).FirstOrDefault();
            string fieldName = field != null ? field.Label : column.ColumnName;
            
            DataRow row = fieldTable.NewRow();
            row["FieldName"] = fieldName;
            row["FieldValue"] = targetDetailsRow[column].ToString();
            fieldTable.Rows.Add(row);
        }   

        dvDetailsFields = new DataView(fieldTable);
    }

    #endregion

}