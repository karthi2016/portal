using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class exhibits_ViewShow : PortalPage 
{
    public msExhibitShow targetShow;
    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetShow = LoadObjectFromAPI<msExhibitShow>(ContextID);
        if (targetShow == null) GoToMissingRecordPage();
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        if (targetShow.ShowFloor != null)
        {
            hlDownloadShowFloor.Visible = true;
            hlDownloadShowFloor.NavigateUrl = GetImageUrl(targetShow.ShowFloor);
        }

        setupRegistrationLinks();

        // is there a linked event?
        setupLinkedEvent();

        setupViewExhibitorLinks();

        hlExhibitorList.Visible = targetShow.ShowExhibitorListInPortal;
        hlExhibitorList.NavigateUrl += targetShow.ID;

        CustomTitle.Text = string.Format("{0}", targetShow.Name);
    }

    private void setupViewExhibitorLinks()
    {
        if (ConciergeAPI.CurrentEntity == null) return;

        using (var api = GetServiceAPIProxy())
        {
            rptVisibleExhibitorRecords.DataSource = api.RetrieveAccessibleExhibitorRecords(targetShow.ID, ConciergeAPI.CurrentEntity.ID).ResultValue;
            rptVisibleExhibitorRecords.DataBind();
        }

    }

    private void setupLinkedEvent()
    {
        if (targetShow.Event == null) return;

        // we have an event linked to this show, so we need to show it
        hlEvent1.Visible = liEvent.Visible = true;
        string eventName;
        using (var api = GetConciegeAPIProxy())
            eventName = api.GetName(targetShow.Event).ResultValue;

        hlEvent1.Text = eventName;
        hlEvent2.Text = "Go to " + eventName + " Home Page";

        hlEvent1.NavigateUrl += targetShow.Event;
        hlEvent2.NavigateUrl += targetShow.Event;
    }

    private void setupRegistrationLinks()
    {
        hlRegistration.NavigateUrl += targetShow.ID;
        if (ConciergeAPI.CurrentEntity == null)
            return;

        ExhibitorRegistrationPermissionPacket pp;
        using (var api = GetConciegeAPIProxy())
        {
            pp = api.GetAvailableExhibitorRegistrationWindows(targetShow.ID, ConciergeAPI.CurrentEntity.ID).ResultValue;
        }

        // now, let's check each window to make sure the entity doesn't have a registration
        foreach (var p in pp.Permissions)
        {
            // is there a registration already?
            Search s = new Search(msExhibitor.CLASS_NAME);
            s.AddCriteria(Expr.Equals(msExhibitor.FIELDS.Show, targetShow.ID));
            s.AddCriteria(Expr.Equals(msExhibitor.FIELDS.Customer, p.EntityID ));

            if ( APIExtensions.GetSearchResult(s, 0, 1).TotalRowCount == 0)
            {
                hlRegistration.Visible = true;  // good to go!
                return;
            }
        }

        hlRegistration.Visible = false; // no eligible registration

    }


}