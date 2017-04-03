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
    protected override bool IsPublic
    {
        get
        {
            return PortalConfiguration.Current.MembershipDirectoryIsPublic;
        }
    }

    #region Fields

    private const string DetailLabelOverridePrefix = "DetailLabel.";

    protected SearchManifest targetManifest;
    protected DataView dvDetailsFields;
    protected DataRow targetDetailsRow;

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

        if (Master != null)
        {
            ((App_Master_GeneralPage) Master).AddCustomOverrideEligibleControls += AddCustomOverrideEligibleControls;
        }
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

        if (PortalConfiguration.Current.MembershipDirectoryIsPublic)
            return true;

        //If the directory is enabled and not restricted to members it's available and no need to check membership status
        if (!PortalConfiguration.Current.MembershipDirectoryForMembersOnly)
            return true;

        //Directory is for members only
        return MembershipLogic.IsActiveMember();
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search s = Search.FromManifest(targetManifest);
        s.AddCriteria(Expr.Equals("Owner.ID", ContextID));

        SearchResult searchResult = APIExtensions.GetSearchResult(s, 0, null);
        if (searchResult.Table != null && searchResult.Table.Rows.Count > 0)
            targetDetailsRow = searchResult.Table.Rows[0];
    }

    private SearchManifest buildSearchManifest()
    {
        SearchManifest result;

        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            result = proxy.DescribeSearch("MembershipWithFlowdown", null).ResultValue;
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

            var labelOverride = PortalConfiguration.GetOverrideFor(
                Request.Url.LocalPath, DetailLabelOverridePrefix + (field != null ? field.Name : column.ColumnName), "Text");
            if (labelOverride != null)
            {
                row["FieldName"] = labelOverride.Value;
            }

            row["FieldValue"] = targetDetailsRow[column].ToString();
            fieldTable.Rows.Add(row);
        }   

        dvDetailsFields = new DataView(fieldTable);
    }

    protected override void AddCustomOverrideEligibleControls(List<msPortalControlPropertyOverride> eligibleControls)
    {
        base.AddCustomOverrideEligibleControls(eligibleControls);

        foreach (var targetField in targetManifest.Fields)
        {
            eligibleControls.Add(new msPortalControlPropertyOverride
            {
                PageName = Request.Url.LocalPath,
                ControlName = DetailLabelOverridePrefix + targetField.Name,
                PropertyName = "Text",
                Value = Convert.ToString(targetField.Label)
            });
        }
    }

    #endregion

}