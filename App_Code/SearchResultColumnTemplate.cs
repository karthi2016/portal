using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for SearchResultColumnTemplate
/// </summary>
public class SearchResultColumnTemplate : ITemplate
{
    private string _columnName;

    public SearchResultColumnTemplate(string columnName)
    {
        _columnName = columnName;
    }

    public void InstantiateIn(Control container)
    {
        LiteralControl litResultColumn = new LiteralControl();
        litResultColumn.ID = string.Format("lit{0}", _columnName);
        litResultColumn.DataBinding += new EventHandler(litResultColumn_DataBinding);
        container.Controls.Add(litResultColumn);
    }

    void litResultColumn_DataBinding(object sender, EventArgs e)
    {
        LiteralControl litResultColumn = (LiteralControl)sender;
        GridViewRow gvr = (GridViewRow)litResultColumn.NamingContainer;

        object value = DataBinder.GetPropertyValue(gvr.DataItem, _columnName);

        if (value != null)
            litResultColumn.Text = string.Format("<p>{0}</p>", value);
    }
}