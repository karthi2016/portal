using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class MultipleEntitiesDetected : PortalPage 
{
    protected override void InitializePage()
    {
        base.InitializePage();
        var entities = ConciergeAPI.AccessibleEntities ;

        if ( entities == null || entities.Count == 0)
            GoHome();

        // this is redundant rblUsers.Items.Add(new ListItem(ConciergeAPI.CurrentEntity.Name, ConciergeAPI.CurrentEntity.ID));
        foreach (var e in entities)
            rblUsers.Items.Add(new ListItem(string.Format("{0} ({1})", e.Name, e.Type), e.ID));

        rblUsers.SelectedIndex = 0;
    }
    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        string selectedEntity = rblUsers.SelectedValue;

        // is there a redirect URL?
        var redirectURL = Request.QueryString["redirectUrl"];
        if (string.IsNullOrWhiteSpace(redirectURL))
            redirectURL = "/";

        var entityChanged = !ConciergeAPI.CurrentEntity.ID.Equals(selectedEntity, StringComparison.Ordinal);

        // record that this was the last one logged in
        // let's re-load it from the database
        if (entityChanged || string.IsNullOrEmpty(ConciergeAPI.CurrentUser.LastLoggedInAs))
        {
            var pu = LoadObjectFromAPI<msPortalUser>(ConciergeAPI.CurrentUser.ID);
            pu.LastLoggedInAs = selectedEntity;
            var saveResult = SaveObject(pu);
            if (saveResult != null)
            {
                ConciergeAPI.CurrentUser = saveResult.ConvertTo<msPortalUser>();
            }
        }

        if (entityChanged)
        {
            var ent = LoadObjectFromAPI<msEntity>(selectedEntity);
            // now set the current entity
            ConciergeAPI.CurrentEntity = ent;
        }

        // now go home
        Response.Redirect(redirectURL);
    }
}