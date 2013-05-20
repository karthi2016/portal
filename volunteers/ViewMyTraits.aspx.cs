using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class volunteers_ViewMyTraits : PortalPage 
{
    protected msVolunteer targetVolunteer;
    protected msVolunteerTraitType targetTraitType;

    protected DataRow drSearchResults;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetVolunteer = LoadObjectFromAPI<msVolunteer>(ContextID);
        if (targetVolunteer == null)
            GoToMissingRecordPage();

        targetTraitType = LoadObjectFromAPI<msVolunteerTraitType>(Request.QueryString["traitTypeID"]);
        if (targetTraitType == null)
            GoToMissingRecordPage();
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        setupPageTitle();

        bindTraits();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        return targetVolunteer.Individual == ConciergeAPI.CurrentEntity.ID;
    }


    private void bindTraits()
    {
        var traits = targetVolunteer.Traits.FindAll(x => x.Type == targetTraitType.ID);

        if (!string.IsNullOrWhiteSpace(targetTraitType.FirstSubTypeName))
            gvTraits.Columns[0].HeaderText = targetTraitType.FirstSubTypeName;
        
        else
            gvTraits.Columns[0].Visible = false;

        if (!string.IsNullOrWhiteSpace(targetTraitType.SecondSubTypeName))
            gvTraits.Columns[1].HeaderText = targetTraitType.SecondSubTypeName;

        else
            gvTraits.Columns[1].Visible = false;

        if (!string.IsNullOrWhiteSpace(targetTraitType.FreeTextName))
            gvTraits.Columns[2].HeaderText = targetTraitType.FreeTextName;

        else
            gvTraits.Columns[2].Visible = false;


        using( var api = GetServiceAPIProxy ())
            foreach (var t in traits)
            {
                if (t.FirstSubType != null) t["FirstSubTypeName"] = api.GetName(t.FirstSubType).ResultValue ;
                if (t.SecondSubType != null) t["SecondSubTypeName"] = api.GetName(t.SecondSubType).ResultValue ;
            }


        gvTraits.DataSource = traits;
        gvTraits.DataBind();
        
    }

    private void setupPageTitle()
    {
        lPageHeader.Text = "View My " + targetTraitType.NamePlural;
    }

    protected void btnGoHome_Click(object sender, EventArgs e)
    {
        GoTo( "/volunteers/ViewMyVolunteerProfile.aspx?contextID=" + targetVolunteer.ID );
    }
}