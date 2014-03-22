using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class financial_ManagePaymentOptions : PortalPage 
{
    

    protected void rgMainDataGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        string msql = string.Format("select Name from SavedPaymentMethod where Owner='{0}' order by Name",
                                    ConciergeAPI.CurrentEntity.ID);

        using (var api = GetServiceAPIProxy())
        {
            var result = api.ExecuteMSQL(msql, 0, null).ResultValue.SearchResult;

            if (result.Table != null)
                rgMainDataGrid.DataSource = result.Table.DefaultView;


            rgMainDataGrid.VirtualItemCount = result.TotalRowCount;

            lNoSavedOptions.Visible = result.TotalRowCount == 0;
            rgMainDataGrid.Visible = !lNoSavedOptions.Visible;
        }
    }



    protected void rgMainDataGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem) // to set the Command Name/Argument conditionally  
        {
            GridDataItem dataItem = (GridDataItem) e.Item;
            LinkButton btn = (LinkButton) dataItem["DeleteButton"].Controls[0];
            btn.CommandArgument = dataItem.GetDataKeyValue("ID").ToString();
        }
    }

    protected void rgMainDataGrid_ItemCommand(object sender, GridCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Delete":
                string id = e.CommandArgument.ToString();
                using (var api = GetServiceAPIProxy())
                    api.Delete(id);
                QueueBannerMessage("The payment option has been deleted successfully.");
                Refresh();
                break;
        }
    }
}