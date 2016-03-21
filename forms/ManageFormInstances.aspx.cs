using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using Telerik.Web.UI;

public partial class forms_ManageFormInstances : PortalPage 
{
    protected msCustomObjectPortalForm targetForm;
    protected PortalFormInfo targetFormManifest;
    protected msCustomObject targetObject;
    protected Search targetSearch;
    
    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;
        
        return targetFormManifest.CanView;
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);  // first things first
        if (ConciergeAPI.CurrentEntity == null) return;

        targetForm = LoadObjectFromAPI<msCustomObjectPortalForm>(ContextID);
        if (targetForm == null)
            GoToMissingRecordPage();

        targetObject = LoadObjectFromAPI<msCustomObject>(targetForm.CustomObject);

        using (var api = GetServiceAPIProxy())
            targetFormManifest = api.DescribePortalForm(targetForm.ID, ConciergeAPI.CurrentEntity.ID).ResultValue;
 
        rgMainDataGrid.AutoGenerateColumns = false;

        targetSearch = new Search( targetObject.Name );
        targetSearch.AddCriteria(Expr.Equals("Owner", ConciergeAPI.CurrentEntity.ID));

        var columns = targetForm.ManagementFieldsToDisplay;
        if ( columns == null || columns.Count == 0 )
            columns = new List<string>{ "Name" };

        columns.ForEach(targetSearch.AddOutputColumn);


        if (!IsPostBack)
            using (var api = GetServiceAPIProxy())
            {
                var meta = api.DescribeCompiledSearch(targetSearch).ResultValue;
                GridLogic.GenerateRadGridColumnsFromFieldMetadata(rgMainDataGrid, meta.Fields);
            }

        GridHyperLinkColumn gc = new GridHyperLinkColumn();
        rgMainDataGrid.Columns.Add(gc); // remember we have to do this right away for column settings to persist
        gc.DataNavigateUrlFormatString= "ViewFormInstance.aspx?contextID={0}&formID=" + targetForm.ID ;
        gc.DataNavigateUrlFields = new string[] {"ID"};
        gc.Text = "(view)";
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        if (!targetFormManifest.CanCreate) return;
        // MS-5957 (Modified 12/11/2014) If the portal form allows the user to create a form instance, then
        // the default label for the link should be "Create {PortalForm Name}"
        var createLink = targetFormManifest.CreateLink ?? string.Format("Create {0}", targetFormManifest.FormName);
        hlCreateInstance.Visible = true;
        hlCreateInstance.Text = string.Format("<LI>{0}</LI>", createLink);
        hlCreateInstance.NavigateUrl += targetForm.ID;

        CustomTitle.Text = string.Format("{0}", targetFormManifest.ManageLink);
    }

    protected void rgMainDataGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        SearchResult result = APIExtensions.GetSearchResult(targetSearch, rgMainDataGrid.CurrentPageIndex * rgMainDataGrid.PageSize,
       rgMainDataGrid.PageSize);

        if (result.Table != null)
            rgMainDataGrid.DataSource = result.Table.DefaultView;


        rgMainDataGrid.VirtualItemCount = result.TotalRowCount;
    }
}