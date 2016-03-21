using System;
using System.Collections.Generic;
using System.Configuration;
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

public partial class careercenter_SearchResume_ViewDetails : PortalPage
{
    #region Fields

    protected SearchManifest targetManifest;
    protected DataView dvResumeDetailsFields;
    protected DataRow targetResumeDetailsRow;

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

        if (targetManifest == null || string.IsNullOrWhiteSpace(ContextID))
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

        if (targetResumeDetailsRow == null)
        {
            GoToMissingRecordPage();
            return;
        }

        //Generate and bind the configured dynamic columns
        generateFieldsDataView();
        rptResumeFields.DataSource = dvResumeDetailsFields;
        rptResumeFields.DataBind();

        //Set the download link
        hlDownloadResume.Visible = !string.IsNullOrWhiteSpace(targetResumeDetailsRow["File"].ToString());
        string fileUrl = GetImageUrl(Convert.ToString(targetResumeDetailsRow["File"]));

        if (!Uri.IsWellFormedUriString(fileUrl, UriKind.Absolute))
            hlDownloadResume.Visible = false;

        hlDownloadResume.NavigateUrl = fileUrl;

        //Attempt to get a textual representation (suppress errors because you can still download the file if it can't be converted to html)
        //If we get one then display it and the title of the details section.  Otherwise hide both as we only have the details section and the title is in the page header
        try
        {
            string resumeAsHtml;

            using (IConciergeAPIService proxy = GetConciegeAPIProxy())
            {
                resumeAsHtml = proxy.GetResumeAsHtml(targetResumeDetailsRow["ID"].ToString()).ResultValue;
            }

            litResumeText.Text = string.IsNullOrWhiteSpace(resumeAsHtml) ? "Unable to produce textual representation of the resume.  You can download the file below." : resumeAsHtml;
        }
        catch
        {
            divTextualRepresentation.Visible = false;
            divResumeDetailsTitle.Visible = false;
        }
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        Search s = Search.FromManifest(targetManifest);
        s.AddCriteria(Expr.Equals("ID", ContextID));

        SearchResult searchResult = APIExtensions.GetSearchResult(s, 0, null);
        if (searchResult.Table != null && searchResult.Table.Rows.Count > 0)
            targetResumeDetailsRow = searchResult.Table.Rows[0];
    }

    private SearchManifest buildSearchManifest()
    {
        SearchManifest result;

        using (IConciergeAPIService proxy = GetServiceAPIProxy())
        {
            result = proxy.DescribeSearch(msResume.CLASS_NAME, null).ResultValue;
            result.Fields.Clear();
            result.Fields =
                proxy.GetSearchFieldMetadataFromFullPath(msResume.CLASS_NAME, null, PortalConfiguration.Current.ResumeDetailsFields).ResultValue;
        }

        result.DefaultSelectedFields = (
            from field in result.Fields
            select new SearchOutputColumn { Name = field.Name, DisplayName = field.Label }).ToList();

        //Make sure the ID is included so we can get the textual representation
        if (!result.DefaultSelectedFields.Exists(x => x.Name == "ID"))
            result.DefaultSelectedFields.Add(new SearchOutputColumn { Name = "ID", DisplayName = "ID" });

        //Make sure the FileID is included so we can let the user download the resume
        if (!result.DefaultSelectedFields.Exists(x => x.Name == "File" || x.Name == "File.ID"))
            result.DefaultSelectedFields.Add(new SearchOutputColumn { Name = "File", DisplayName = "File" });

        return result;
    }

    protected void generateFieldsDataView()
    {
        DataTable fieldTable = new DataTable();
        fieldTable.Columns.Add(new DataColumn("FieldName", typeof(string)));
        fieldTable.Columns.Add(new DataColumn("FieldValue", typeof(string)));

        foreach (DataColumn dcDetailsColumn in targetResumeDetailsRow.Table.Columns)
        {
            if (dcDetailsColumn.ColumnName == "ID" || dcDetailsColumn.ColumnName == "ROW_NUMBER" || dcDetailsColumn.ColumnName == "File" || dcDetailsColumn.ColumnName == "File.ID")
                continue;

            DataColumn column = dcDetailsColumn;
            var field = targetManifest.Fields.Where(x => x.Name == column.ColumnName).FirstOrDefault();
            string fieldName = field != null ? field.Label : column.ColumnName;

            DataRow row = fieldTable.NewRow();
            row["FieldName"] = fieldName;
            row["FieldValue"] = targetResumeDetailsRow[column].ToString();
            fieldTable.Rows.Add(row);
        }

        dvResumeDetailsFields = new DataView(fieldTable);
    }

    #endregion
}