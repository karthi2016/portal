using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;

public partial class admin_EditControlProperty : PortalPage 
{
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        if (MultiStepWizards.CustomizePage.PageName == null)
            GoHome();

        
            
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        if (!string.IsNullOrWhiteSpace(MultiStepWizards.CustomizePage.EditModeControlName))
        {
            lControlName.Text = MultiStepWizards.CustomizePage.EditModeControlName;
            lControlName.Visible = true;
        }
        else
        {
            // populate all controls
            ddlAllControls.Visible = true;
            var allEligibleControls = MultiStepWizards.CustomizePage.AllEligibleControls;
            allEligibleControls.Sort();
            ddlAllControls.DataSource = allEligibleControls;
            ddlAllControls.DataBind();
        }

        if (!string.IsNullOrWhiteSpace(MultiStepWizards.CustomizePage.EditModeControlPropertyName))
        {
            tbProperty.Text = MultiStepWizards.CustomizePage.EditModeControlPropertyName;
            tbProperty.Enabled = false;
        }

        reValue.Content = MultiStepWizards.CustomizePage.EditModeControlPropertyValue;
        

        // now - does there already exist an override for this page?
        var ppo = _getCurrentOverride();
        if (ppo != null)
        {
            reValue.Content = ppo.Value; // use the updated content
            tbNotes.Text = ppo.Description;
        }
    }

    private msPortalControlPropertyOverride _getCurrentOverride()
    {
        if (!string.IsNullOrWhiteSpace(MultiStepWizards.CustomizePage.EditModeControlName) &&
           !string.IsNullOrWhiteSpace(MultiStepWizards.CustomizePage.EditModeControlPropertyName))

            return PortalConfiguration.GetOverrideFor(MultiStepWizards.CustomizePage.PageName , MultiStepWizards.CustomizePage.EditModeControlName,
                MultiStepWizards.CustomizePage.EditModeControlPropertyName);

        return null;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return ;

        var ppo = _getCurrentOverride();
        if (ppo == null)
            ppo = CreateNewObject<msPortalControlPropertyOverride>();

        ppo.PageName = MultiStepWizards.CustomizePage.PageName;

        if (ddlAllControls.Visible)
            ppo.ControlName = ddlAllControls.SelectedValue;
        else
            ppo.ControlName = MultiStepWizards.CustomizePage.EditModeControlName;

        ppo.PropertyName = tbProperty.Text ;
        ppo.Value = reValue.Content;
        ppo.Description = tbNotes.Text;
        
        SaveObject(ppo);    // save the object

        // now, reset the overrides
        using (var api = GetServiceAPIProxy())
        {
            Search s = new Search(msPortalControlPropertyOverride.CLASS_NAME);

            PortalConfiguration.Current.ControlOverrides
                =
                api.GetObjectsBySearch(s, null, 0, null).ResultValue.Objects.ConvertTo<msPortalControlPropertyOverride>();

        }

        MultiStepWizards.CustomizePage.EditModeControlName = null;
        MultiStepWizards.CustomizePage.EditModeControlPropertyValue = null;
        MultiStepWizards.CustomizePage.EditModeControlPropertyDescription = null;
        MultiStepWizards.CustomizePage.EditModeControlPropertyName= null;
        GoTo("ViewControlPropertyOverrides.aspx", "Property override saved successfully.");

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MultiStepWizards.CustomizePage.EditModeControlName = null;
        MultiStepWizards.CustomizePage.EditModeControlPropertyValue = null;
        MultiStepWizards.CustomizePage.EditModeControlPropertyName = null;
        MultiStepWizards.CustomizePage.EditModeControlPropertyDescription = null;
        GoTo("ViewControlPropertyOverrides.aspx" );
    }
}