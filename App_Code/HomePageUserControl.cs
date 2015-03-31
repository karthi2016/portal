using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for HomePageUserControl
/// </summary>
public class HomePageUserControl : UserControl
{

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!IsPostBack)
            InitializeWidget();
    }

    protected virtual void InitializeWidget()
    {

    }

    /// <summary>
    /// Gets the fields needed for main search.
    /// </summary>
    /// <returns></returns>
    /// <remarks>Each home page controls needs specific fields from the main search</remarks>
    public virtual List<string> GetFieldsNeededForMainSearch()
    {
        return new List<string>();
    }



    /// <summary>
    /// Generates the searches to be run for this control
    /// </summary>
    /// <param name="searchesToRun">The search dictionary.</param>
    /// <remarks>Since each widget may need specific searches to be run, and we want to run them in
    /// parallel, this method collects them</remarks>
    public virtual void GenerateSearchesToBeRun(List<Search> searchesToRun)
    {

    }

    protected DataRow drMainRecord = null;
    /// <summary>
    /// Delivers the search results to the widget
    /// </summary>
    /// <param name="results">The results.</param>
    public virtual void DeliverSearchResults(List<SearchResult> results)
    {
        results.RemoveAll(a => a == null);
        if (results.Single(x => x.ID == "Main").Table.Rows.Count > 0)
            drMainRecord = results.Single(x => x.ID == "Main").Table.Rows[0];
    }

    public virtual void GenerateFormLinks(List<PortalFormInfo> formsToDisplay)
    {
        var phForms = FindControl("divForms") as HtmlGenericControl;
        if (phForms == null) return;
        if (formsToDisplay == null || formsToDisplay.Count == 0) return;

        phForms.Visible = true;
        HtmlGenericControl ul = new HtmlGenericControl("ul");
        phForms.Controls.Add(ul);
        foreach (var f in formsToDisplay)
        {

            if (f.CanCreate)
            {
                // MS-5957 (Modified 12/11/2014) If the portal form allows the user to create a form instance, then
                // the default label for the link should be "Create {PortalForm Name}"
                var createLink = f.CreateLink ?? string.Format("Create {0}", f.FormName);
                ul.Controls.Add(new HyperLink
                {
                    Text = string.Format("<LI>{0}</LI>", createLink),
                    NavigateUrl = "/forms/CreateFormInstance.aspx?contextID=" + f.FormID
                });
            }

            if (f.CanView && f.ManageLink != null)
                ul.Controls.Add(new HyperLink
                {
                    Text = string.Format("<LI>{0}</LI>", f.ManageLink),
                    NavigateUrl = "/forms/ManageFormInstances.aspx?contextID=" + f.FormID
                });
        }

    }

    /// <summary>
    /// Binds the search result.
    /// </summary>
    /// <param name="searchResult">The search result.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="label">The label.</param>
    /// <param name="formatSpecifier">The format specifier.</param>
    protected string bindSearchResult(DataRow searchResult, string columnName, Label label, string formatSpecifier)
    {
        if (searchResult == null)
            return null;

        if (searchResult[columnName] == DBNull.Value)    // don't show anything
            return null;

        string txt;
        if (formatSpecifier == null)
            txt = Convert.ToString(searchResult[columnName]);
        else
            txt = string.Format("{0:" + formatSpecifier + "}", searchResult[columnName]);

        if (label != null)
            label.Text = txt;

        return txt;
    }

    public bool isActiveMember()
    {
        if (drMainRecord == null)
            throw new ApplicationException("This method is only valid after the primary search has been executed and the main row is available");

        //Check if the appropriate fields exist - if they do not then the membership module is inactive
        if (!(drMainRecord.Table.Columns.Contains("Membership") && drMainRecord.Table.Columns.Contains("Membership.ReceivesMemberBenefits") && drMainRecord.Table.Columns.Contains("Membership.TerminationDate")))
            return false;

        //Check there is a membership
        if (string.IsNullOrWhiteSpace(Convert.ToString(drMainRecord["Membership"])))
            return false;

        //Check the membership indicates membership benefits
        // MS-6070 (Modified 1/15/2015) Modified to account for a null 'Membership.ReceivesMemberBenefits' value
        var receivesMemberBenefits = drMainRecord.Field<bool?>("Membership.ReceivesMemberBenefits");
        if (receivesMemberBenefits == null || !Convert.ToBoolean(receivesMemberBenefits))
            return false;

        //At this point if the termination date is null the member should be able to see the restricted directory
        DateTime? terminationDate = drMainRecord.Field<DateTime?>("Membership.TerminationDate");

        if (terminationDate == null)
            return true;

        //There is a termination date so check if it's future dated
        return terminationDate > DateTime.Now;
    }


}