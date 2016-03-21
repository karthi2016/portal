using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK;
using MemberSuite.SDK.Types;

public partial class exhibits_RegisterForBoothPreferences : PortalPage
{
    public msExhibitorRegistrationWindow targetWindow;
    public msEntity targetEntity;
    public msExhibitShow targetShow;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetWindow = LoadObjectFromAPI<msExhibitorRegistrationWindow>(ContextID);
        if (targetWindow == null) GoToMissingRecordPage();
        targetEntity = LoadObjectFromAPI<msEntity>(Request.QueryString["entityID"]);
        if (targetEntity == null) GoToMissingRecordPage();

        targetShow = LoadObjectFromAPI<msExhibitShow>(targetWindow.Show);
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity()) return false;

        using (var api = GetServiceAPIProxy())
        {
            var ps =
                api.GetAvailableExhibitorRegistrationWindows(targetWindow.Show, targetEntity.ID).ResultValue.Permissions;
            if (ps.Count == 0 || 
                (ps[0].RegistrationMode != ExhibitorRegistrationMode.PurchaseBoothsByType && ps[0].RegistrationMode != ExhibitorRegistrationMode.IndicateBoothPreferencesOnly ))
                return false;
        }

        return true;
    }

    private List<ExhibitBoothInfo> openBooths;

    protected override void InitializePage()
    {
        base.InitializePage();
        lMainInstructions.Text = targetShow.RegistrationInstructions;
        lRegistrationWindowInstructions.Text = targetWindow.RegistrationInstructions;

        if (targetShow.ShowFloor != null)
            lShowFloor.NavigateUrl = GetImageUrl(targetShow.ShowFloor);
        else
            lShowFloor.Visible = false;

        int numberOfChoices = GetNumberOfChoices();

        object[] emptyRows = new object[numberOfChoices];

        using (var api = GetServiceAPIProxy())
            openBooths = api.GetAvaialbleExhibitBooths(targetShow.ID, targetEntity.ID ).ResultValue;

        rptChoices.DataSource = emptyRows;
        rptChoices.DataBind();

        RegisterJavascriptConfirmationBox(btnSave, "Please review your selections, as your record will be saved immediately. Press OK to save.");

        CustomTitle.Text = string.Format("{0} Registration", targetShow.Name);
    }

    private int GetNumberOfChoices()
    {
        return 3;   // for now, though we might make this configruable
    }


    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        List<String> booths = new List<string>();
        foreach (RepeaterItem ri in rptChoices.Items)
        {
            DropDownList ddlChoice = (DropDownList)ri.FindControl("ddlChoice");
            if (string.IsNullOrWhiteSpace(ddlChoice.SelectedValue)) continue;

            booths.Add(ddlChoice.SelectedValue);
        }

        if (booths.Count == 0)
        {
            cvAtLeastOneBooth.IsValid = false;
            return;
        }

        msExhibitor ex;
        using (var api = GetServiceAPIProxy())
        {
            ex =
                api.RetrieveOrCreateExhibitorRecord(targetShow.ID, targetEntity.ID).ResultValue.ConvertTo<msExhibitor>();


            if (ex.BoothPreferences == null) ex.BoothPreferences = new List<msExhibitorAssignedBooth>();
            ex.BoothPreferences.Clear();
            foreach (RepeaterItem ri in rptChoices.Items)
            {
                DropDownList ddlChoice = (DropDownList) ri.FindControl("ddlChoice");
                if (string.IsNullOrWhiteSpace(ddlChoice.SelectedValue)) continue;

                ex.BoothPreferences.Add(new msExhibitorAssignedBooth {Booth = ddlChoice.SelectedValue});
            }

            api.Save(ex);
        }

        GoTo("ViewExhibitor.aspx?contextID=" + ex["ID"], "Your preferences have been recorded successfully.");

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("ViewShow.aspx?contextID=" + targetShow.ID);
    }

    protected void rptChoices_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (Page.IsPostBack)
            return; // only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;

            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
                goto case ListItemType.Item;

            case ListItemType.Item:
                Literal lChoiceLabel = (Literal) e.Item.FindControl("lChoiceLabel");
                DropDownList ddlChoice = (DropDownList) e.Item.FindControl("ddlChoice");

                lChoiceLabel.Text = string.Format("Choice #{0}", e.Item.ItemIndex + 1);

                ddlChoice.DataSource = openBooths;
                ddlChoice.DataTextField = "BoothName";
                ddlChoice.DataValueField = "BoothID";
                ddlChoice.DataBind();

                ddlChoice.Items.Insert(0, new ListItem(" --- Select a Booth ---", ""));

                break;
        }
    }
}