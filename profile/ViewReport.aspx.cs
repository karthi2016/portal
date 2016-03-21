using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;
using Telerik.Web.UI;

public partial class profile_ViewReport : PortalPage
{
    protected msSavedSearch targetSearch;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

    }
    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;


        using (var api = GetServiceAPIProxy())
            return api.CheckEntitlement(msSearchEntitlement.CLASS_NAME, ConciergeAPI.CurrentEntity.ID, targetSearch.ID).ResultValue.IsEntitled;


    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);  // first things first


        targetSearch = LoadObjectFromAPI<msSavedSearch>(ContextID);
        if (targetSearch == null)
            GoToMissingRecordPage();

        rgMainDataGrid.AutoGenerateColumns = false;

        if (!IsPostBack)
            using (var api = GetServiceAPIProxy())
            {
                var meta = api.DescribeCompiledSearch(targetSearch.Search).ResultValue;
                GridLogic.GenerateRadGridColumnsFromFieldMetadata(rgMainDataGrid, meta.Fields);
            }
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        lblReportName.Text = targetSearch.Name;
    }

    protected void btnGoHome_Cick(object sender, EventArgs e)
    {
        GoHome();
    }

    protected void btnMyReports_Cick(object sender, EventArgs e)
    {
        GoTo("MyReports.aspx");
    }

    protected void rgMainDataGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        SearchResult result = APIExtensions.GetSearchResult(targetSearch.Search, rgMainDataGrid.CurrentPageIndex * rgMainDataGrid.PageSize,
            rgMainDataGrid.PageSize);

        if (result.Table != null)
            rgMainDataGrid.DataSource = result.Table.DefaultView;


        rgMainDataGrid.VirtualItemCount = result.TotalRowCount;


    }

    protected void rgMainDataGrid_ItemCreated(object sender, GridItemEventArgs e)
    {

    }


    protected void lbExcel_Click(object sender, EventArgs e)
    {
        using (var api = GetServiceAPIProxy())
        {
            var loc = api.ExecuteSearchWithFileOutput(targetSearch.Search, BuiltInSearchOutputTypes.ExcelFormatted, false).ResultValue;
            Response.Redirect(loc ?? "~/SearchQueued.aspx");


        }
    }
}