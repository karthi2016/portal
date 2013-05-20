using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;
using Telerik.Web.UI;

/// <summary>
/// Summary description for GridLogic
/// </summary>
public static  class GridLogic
{

    public static void GenerateRadGridColumnsFromFieldMetadata(RadGrid rgMainDataGrid, List<FieldMetadata> columnsToDisplay)
    {
        string tz = TimeZoneUtils.GetTimeZoneAbbreviatedTime(DateTime.Now,
                           ConciergeAPI.CurrentTimeZone);
        if (columnsToDisplay == null) return;
        foreach (FieldMetadata col in columnsToDisplay)
        {
            GridBoundColumn c;

            //if ( col.DataType == FieldDataType.Date || col.DataType == FieldDataType.DateTime )
            //    c= new GridDateTimeColumn();
            //else
            c = new GridBoundColumn();


            rgMainDataGrid.Columns.Add(c); // need to add this RIGHT AWAY, or properties don't persist
            c.DataField = col.Name;

            string label = col.Label;

            c.HeaderText = label;
            if (col.ColumnWidth != null)
                c.HeaderStyle.Width = col.ColumnWidth.Value;

            switch (col.DataType)
            {
                case FieldDataType.Date:
                    switch (col.DisplayType)
                    {
                        case FieldDisplayType.MonthDayPicker:
                            c.DataFormatString = "{0:d}";
                            break;

                        case FieldDisplayType.MonthYearPicker:
                            c.DataFormatString = "{0:MM - dd}";
                            break;

                        default:
                            c.DataFormatString = "{0:MM/dd/yyyy}";
                            break;
                    }
                    break;

                case FieldDataType.DateTime:
                    c.DataFormatString = "{0} " + tz;
                    break;

                case FieldDataType.Time:
                    c.DataFormatString = "{0:t}";
                    break;

                case FieldDataType.Integer:
                    //if ( col.Name == null || ( col.Name != "LocalID" && ! col.Name.EndsWith( ".LocalID") ))   // hack, don't format IDs - MS-308
                    //  c.DataFormatString = "{0:N0}";
                    break;

                case FieldDataType.Decimal:
                    int precision = col.Precision == default(int) ? 2 : col.Precision;
                    c.DataFormatString = string.Format("{{0:N{0}}}", precision);
                    break;

                case FieldDataType.Money:
                    c.DataFormatString = "{0:C}";   // for now, but we'll have to change when we go multi-currency
                    break;
            }
        }
    }
}