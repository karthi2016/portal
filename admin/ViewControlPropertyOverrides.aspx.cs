using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class admin_ViewControlPropertyOverrides : PortalPage 
{
    protected override bool CheckSecurity()
    {
        // no need to do anything here - the only way I can go to this page
        // is through Session State being set, so security has already been checked
        return true;
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        var controls = MultiStepWizards.CustomizePage.ControlsEligibleForQuickOverride;
        if (MultiStepWizards.CustomizePage.PageName == null ||
            controls == null)
        {
            goBackToReferrer();
            return;
        }
 
        controls.Sort( (x,y) => string.Compare( x.ControlName, y.ControlName) );
        
        foreach (var nvp in controls)
        {

          
            if ( string.IsNullOrWhiteSpace( nvp.ID ) )
                nvp.ID = Guid.NewGuid().ToString();
        }

        // now, add any manual overrides
        foreach (var o in PortalConfiguration.Current.ControlOverrides)
            if ( o.PageName == MultiStepWizards.CustomizePage.PageName &&
                !controls.Exists(x => x.ControlName == o.ControlName && x.PropertyName == o.PropertyName ))
                controls.Add(o);

        
        gvValues.DataSource = controls;
        gvValues.DataBind();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        goBackToReferrer();
    }

    private void goBackToReferrer()
    {
        if ( MultiStepWizards.CustomizePage.Referrer != null )
            Response.Redirect(MultiStepWizards.CustomizePage.Referrer);
        else
            GoHome();
    }

    protected void btnAddManual_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditControlProperty.aspx");   
    }

    protected void gvValues_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();

        var ppo = MultiStepWizards.CustomizePage.ControlsEligibleForQuickOverride.Find(x => x.ID == id);
        if (ppo == null) return;

         switch( e.CommandName )
         {
             case "Edit":
                 MultiStepWizards.CustomizePage.EditModeControlName = ppo.ControlName ;
                 MultiStepWizards.CustomizePage.EditModeControlPropertyName = ppo.PropertyName ;
                 MultiStepWizards.CustomizePage.EditModeControlPropertyValue = ppo.Value ;
                 MultiStepWizards.CustomizePage.EditModeControlPropertyDescription = ppo.Description;
                 Response.Redirect("EditControlProperty.aspx");
                 break;

             case "Reset":
                 using (var api = GetServiceAPIProxy())
                 {
                     api.Delete(id);    // delete it

                     // remove it from session
                     PortalConfiguration.Current.ControlOverrides.RemoveAll(x => x.ID == id);
                   
                     Refresh();
                 }
                 break;
         }
    }

    protected void gvValues_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        msPortalControlPropertyOverride ppo = (msPortalControlPropertyOverride)e.Row.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;

            case DataControlRowType.Footer:
                break;



            case DataControlRowType.DataRow:
                LinkButton lbEdit = (LinkButton)e.Row.FindControl("lbEdit");
                LinkButton lbReset = (LinkButton)e.Row.FindControl("lbReset");
                var lValue = (Literal)e.Row.FindControl("lValue");

                lbReset.Visible = false;
                  lValue.Text = ppo.Value;

                 var ppoExisting = PortalConfiguration.GetOverrideFor(ppo.PageName, ppo.ControlName, ppo.PropertyName);
                if (ppoExisting != null)
                {
                    lbReset.Visible = true;
                    lValue.Text = ppoExisting.Value;
                    ppo.ID = ppoExisting.ID;
                }

                 lbEdit.CommandArgument = ppo.ID;    // guaranteed to be set above
                lbReset.CommandArgument = ppo.ID;    // guaranteed to be set above
             

                RegisterJavascriptConfirmationBox(lbReset, "Are you sure you want to reset the specified text back to its default value?");

           

                if (lValue.Text != null && lValue.Text.Length > 100)
                    lValue.Text = lValue.Text.Substring(0, 100) + "...";

                
                

                break;
        }
    }
}