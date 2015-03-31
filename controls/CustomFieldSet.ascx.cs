using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Command.Views;
using MemberSuite.SDK.Manifests.Searching;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Web;
using MemberSuite.SDK.Web.ControlManagers;

public partial class controls_CustomFieldSet : System.Web.UI.UserControl, IControlHost
{
    public bool EditMode
    {
        get
        {
            var obj = ViewState["EditMode"];
            if (obj != null) return (bool)obj;
            return true;
        }
        set { ViewState["EditMode"] = value; }
    }

    public bool SuppressNullLabelReplacement
    {
        get
        {
            var obj = ViewState["SuppressNullLabelReplacement"];
            if (obj != null) return (bool)obj;
            return true;
        }
        set { ViewState["SuppressNullLabelReplacement"] = value; }
    }

    public bool SuppressValidation { get; set; }

    /// <summary>
    /// Gets or sets the fields this control should display
    /// </summary>
    /// <value>The fields.</value>
    /// <remarks>This needs to be set on every postback, and should optimally 
    /// be set in the OnInit method of the parent page</remarks>
    public DataEntryViewMetadata PageLayout { get; set; }
    public ClassMetadata Metadata { get; set; }
    public MemberSuiteObject MemberSuiteObject { get; set; }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!IsPostBack)
            Bind();
    }


    #region Custom Field Generation


    public void Bind()
    {
        foreach (var entry in _controlManagerDictionary.Values)
            entry.DataBind();
    }

    public void Render()
    {
        if (PageLayout == null || PageLayout.IsEmpty()) return;
        if (Metadata == null) return;

        foreach (var section in PageLayout.Sections)
        {
            _renderSection(section);

            if (section.SubSections != null)
                foreach (var controlSection in section.SubSections)
                    _renderSection(controlSection);
        }
    }

    private void _renderSection(ViewMetadata.ControlSection section)
    {
        if (section.Label != null && section.Label.Trim() != "")
        {
            // add the section header
            var header = new HtmlGenericControl("h2");
            header.InnerText = section.Label;
            phCustomFields.Controls.Add(header);


        }

        // render text
        if (!string.IsNullOrWhiteSpace(section.Text))
            phCustomFields.Controls.Add(new LiteralControl(section.Text));

        // now, render controls
        _renderControls(section, phCustomFields);
    }

    protected Dictionary<ControlMetadata, ControlManager> _controlManagerDictionary = new Dictionary<ControlMetadata, ControlManager>();
    protected void registerControlManager(ControlManager manager)
    {
        // pre-conditions
        if (manager == null) throw new ArgumentNullException("manager");
        if (manager.ControlMetadata == null) throw new ArgumentException("Unable to register manager with null control metadata");

        _controlManagerDictionary.Add(manager.ControlMetadata, manager);

    }
    private void _renderControls(ViewMetadata.ControlSection section, PlaceHolder container)
    {
        HtmlTable ht = new HtmlTable();
        container.Controls.Add(ht);

        if (section.LeftControls == null) section.LeftControls = new List<ControlMetadata>();
        if (section.RightControls == null) section.RightControls = new List<ControlMetadata>();


        var leftControls = new List<ControlMetadata>(section.LeftControls);
        var rightControls = new List<ControlMetadata>(section.RightControls);

        // now, let's remove all disabled controls, b/c they probably have a portal accessibility restriction
        // MS-4675
        leftControls.RemoveAll(x => !x.Enabled);
        rightControls.RemoveAll(x => !x.Enabled);

        int maxRows = leftControls.Count > rightControls.Count ? leftControls.Count : rightControls.Count;

        for (int i = 0; i <= maxRows; i++)
        {
            HtmlTableRow tableRow = new HtmlTableRow();
            ht.Rows.Add(tableRow);

            //HtmlTableCell tcLeftLabel = new HtmlTableCell();
            //tableRow.Cells.Add(tcLeftLabel);

            //HtmlTableCell tcLeftControl = new HtmlTableCell();
            //tableRow.Cells.Add(tcLeftControl);

            // first, render the left side

            if (leftControls.Count > i)
                renderControl(tableRow, leftControls[i]);

            if (rightControls.Count > i)
                renderControl(tableRow, rightControls[i]);

            //return new ControlGroupInstructions(manager.RowSpan - 1, createNewRow); 


        }
    }

    private void renderControl(HtmlTableRow tableRow, ControlMetadata control)
    {
        ControlManager manager;

        if (control == null) return;

        var nameOfField = control.DataSourceExpression;

        FieldMetadata fieldMeta = null;

        if (Metadata != null && Metadata.Fields != null)
            fieldMeta = Metadata.Fields.Find(x => x.Name == nameOfField);

        // MS-4803. Make sure that DataSourceExpression is not empty.
        if (fieldMeta == null && !string.IsNullOrWhiteSpace(nameOfField)) // check to see if we're splitting
        {
            var splits = nameOfField.Split('|');
            nameOfField = splits[splits.Length - 1];

            if (Metadata != null && Metadata.Fields != null)
                fieldMeta = Metadata.Fields.Find(x => x.Name == nameOfField);
        }
        if (fieldMeta == null && control.DisplayType == null) return;

        if (fieldMeta != null)
            if (EditMode)
            {
                if (fieldMeta.PortalAccessibility != PortalAccessibility.Full) return; // we can't show it
            }
            else
            {
                if (fieldMeta.PortalAccessibility == PortalAccessibility.None) return; // we can't;
            }

        if (fieldMeta == null)
            manager = ControlManagerResolver.Resolve(control.DisplayType.Value);
        else
            manager = EditMode ? ControlManagerResolver.Resolve(fieldMeta.DisplayType) : new LabelControlManager(SuppressNullLabelReplacement);

        manager.IsInPortal = true;
        manager.Initialize(this, control);
        registerControlManager(manager);

        tableRow.Attributes["style"] = "vertical-align: top";
        // add the form group
        var tdLabel = new HtmlTableCell();
        var tdControl = new HtmlTableCell("td");

        if (control.UseEntireRow)
            tdControl.Attributes["class"] = "columnHeader customFieldCell";
        else tdLabel.Attributes["class"] = "columnHeader customFieldCell";

        //Commented out per MS-1995 When using address fields as custom fields, overlap occurs
        //if (manager.RowSpan > 1)
        //    tdLabel.Attributes["RowSpan"] = tdControl.Attributes["RowSpan"] = manager.RowSpan.ToString();

        // add the label (no labels for Seperators)
        if (!manager.CustomLabel)// MS-2675&& !control.UseEntireRow)
        {
            var label = new HtmlGenericControl("label");
            var controlLabel = manager.GetLabel();

            if (fieldMeta != null && fieldMeta.PortalPrompt != null)
                controlLabel = fieldMeta.PortalPrompt;

            if (!SuppressValidation && manager.IsRequired() &&
                !(manager is CheckBoxControlManager) && !(manager is LabelControlManager))   // special exception - don't show required for checbkoxes/labels it makes no sense
                controlLabel += " <span class=\"requiredField\">*</span>";

            label.Controls.Add(new LiteralControl(controlLabel));

            // now, add the help text
            if (fieldMeta != null && fieldMeta.HelpText != null)
                label.Controls.Add(new LiteralControl(string.Format("<BR/><span class='helpText'>{0}</span><BR/>&nbsp;",
                    fieldMeta.HelpText.Replace("\n", "<BR/>"))));

            tdLabel.Controls.Add(label);

            // only  add the label cell if we need to
            tableRow.Controls.Add(tdLabel);
            tdControl.Attributes["class"] = "controlCell";

        }
        else
        {
            if (control.UseEntireRow)
            {
                tdControl.Attributes["colspan"] = "4"; // of course, the control spans two controls
                tdControl.Attributes["Class"] = "singleControlCell";

            }
            else
            {
                tdControl.Attributes["colspan"] = "2"; // of course, the control spans two controls
                tdControl.Attributes["Class"] = "singleControlCell";
            }
        }

        // now, add the field


        tableRow.Controls.Add(tdControl);

        var controls = manager.InstantiateWithPostprocessing(
            ctls =>
            {
                if (manager.CustomLabel)
                {
                    var labelBuilder = new StringBuilder();
                    labelBuilder.Append(manager.GetLabel());
                    if (manager.IsRequired())
                    {
                        labelBuilder.Append("<span class=\"requiredField\">*</span>");
                    }
                    labelBuilder.Append("<p/>");
                    ctls.Insert(0, new LiteralControl(labelBuilder.ToString()));
                }
            }
            );

        if (manager.CustomLabel && controls.Count > 0 && controls[0] is LiteralControl)
        {
            controls.Insert(0, new LiteralControl("<span class=\"columnHeader customFieldCell\">"));
            controls.Add(new LiteralControl("</span>"));
        }

        if (controls != null)
            foreach (var c in controls)
                tdControl.Controls.Add(c);

        if (SuppressValidation) return;

        var validationContorls = manager.InstantiateValidationControls();
        if (validationContorls != null)
            foreach (var c in validationContorls)
                tdControl.Controls.Add(c);
    }

    #endregion

    #region Implementation of IControlHost

    /// <summary>
    /// Applies a control metadata against the view metadata to find a value
    /// </summary>
    /// <param name="cMeta">The c meta.</param>
    /// <returns></returns>
    public object Resolve(ControlMetadata cMeta)
    {
        if (cMeta == null) throw new ArgumentNullException("cMeta");
        if (MemberSuiteObject == null || cMeta.DataSourceExpression == null) return null;

        return MemberSuiteObject.SafeGetValue(cMeta.DataSourceExpression);
    }

    /// <summary>
    /// Attempts to determine a resource value
    /// </summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <param name="returnNullIfNothingFound">if set to <c>true</c> [return null if nothing found].</param>
    /// <returns></returns>
    public string ResolveResource(string resourceName, bool returnNullIfNothingFound)
    {
        return null;
    }

    public string ResolveComplexExpression(string complexExpression)
    {
        return complexExpression;
    }

    /// <summary>
    /// Gets the field metadata associated with the specified control metadata
    /// </summary>
    /// <param name="meta">The meta.</param>
    /// <returns></returns>
    public FieldMetadata GetBoundFieldFor(ControlMetadata meta)
    {
        if (meta == null) throw new ArgumentNullException("meta");
        if (Metadata == null)
            return null;

        // MS-6019 (Modified 1/9/2015) Sometimes the DataSourceExpression on the ControlMetadata does not match the name of any field.
        // It is worth checking whether the Name on the ControlMetadata matches any of the fields as well.
        return Metadata.Fields.Find(x => x.Name == meta.DataSourceExpression) ?? Metadata.Fields.Find(x => x.Name == meta.Name);
    }

    /// <summary>
    /// Gets the service API proxy.
    /// </summary>
    /// <returns></returns>
    public IConciergeAPIService GetServiceAPIProxy()
    {
        return ((PortalPage)Page).GetConciegeAPIProxy();
    }

    /// <summary>
    /// Sets the value of the control in the model
    /// </summary>
    /// <param name="ControlMetadata">The control metadata.</param>
    /// <param name="valueToSet">The value to set.</param>
    public void SetModelValue(ControlMetadata ControlMetadata, object valueToSet)
    {
        SetModelValue(ControlMetadata, valueToSet, false);
    }

    /// <summary>
    /// Sets the value of the control in the model
    /// </summary>
    /// <param name="ControlMetadata">The control metadata.</param>
    /// <param name="valueToSet">The value to set.</param>
    public void SetModelValue(ControlMetadata ControlMetadata, object valueToSet, bool onlyIfMemberSuiteObject)
    {
        MemberSuiteObject[ControlMetadata.DataSourceExpression] = valueToSet;
    }

    /// <summary>
    /// Resolves the acceptable values.
    /// </summary>
    /// <param name="ControlMetadata">The control metadata.</param>
    /// <returns></returns>
    public object ResolveAcceptableValues(ControlMetadata ControlMetadata)
    {
        return null;
    }

    /// <summary>
    /// Describes the search.
    /// </summary>
    /// <param name="searchType">Type of the search.</param>
    /// <param name="searchContext">The search context.</param>
    /// <returns></returns>
    public SearchManifest DescribeSearch(string searchType, string searchContext)
    {
        return GetServiceAPIProxy().DescribeSearch(searchType, searchContext).ResultValue;
    }

    /// <summary>
    /// Gets the current time zone.
    /// </summary>
    /// <returns></returns>
    public TimeZoneInfo GetCurrentTimeZone()
    {
        PortalPage pp = (PortalPage)Page;
        if (pp.CurrentUser == null)
            return TimeZoneInfo.Local;

        var tzo = TimeZoneInfo.FindSystemTimeZoneById(pp.CurrentUser.TimeZone);
        if (tzo != null)
            return tzo;

        return TimeZoneInfo.Local;
    }

    #endregion

    /// <summary>
    ///  Pulls the values from the custom fields back into the MemberSuiteObject
    /// </summary>
    public void Harvest()
    {
        if (this.MemberSuiteObject == null)
            throw new ApplicationException("Cannot harvest with null MemberSuite Object");

        foreach (var entry in _controlManagerDictionary)
            entry.Value.DataUnbind();
    }

    public void AddReferenceNamesToTargetObject(IConciergeAPIService proxy)
    {
        if (EditMode || PageLayout == null || MemberSuiteObject == null || Metadata == null) return;

        foreach (var controlSection in PageLayout.Sections)
        {
            foreach (var controlMetadata in controlSection.LeftControls.Union(controlSection.RightControls))
            {
                if (string.IsNullOrWhiteSpace(controlMetadata.DataSourceExpression) ||
                    !Metadata.Fields.Exists(x => x.Name == controlMetadata.DataSourceExpression)) continue;

                FieldMetadata fieldMetadata = Metadata.Fields.Single(x => x.Name == controlMetadata.DataSourceExpression);
                if (fieldMetadata.DataType != FieldDataType.Reference) continue;

                string value = MemberSuiteObject.SafeGetValue(controlMetadata.DataSourceExpression) as string;

                if (string.IsNullOrWhiteSpace(value)) continue;

                MemberSuiteObject referencedObject = proxy.Get(value).ResultValue;
                MemberSuiteObject[controlMetadata.DataSourceExpression + "_Name__transient"] = referencedObject["Name"];
            }
        }
    }
}