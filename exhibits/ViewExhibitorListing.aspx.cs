using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using Telerik.Web.UI;

public partial class exhibits_ViewExhibitorListing : PortalPage 
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

        Search s = new Search(msExhibitor.CLASS_NAME);
        s.AddCriteria(Expr.Equals(msExhibitor.FIELDS.Show, targetShow.ID));
        s.AddSortColumn("Name");

        s.AddOutputColumn("AssignedBooths");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("Logo");
        s.AddOutputColumn("Bio");

        var dt = ExecuteSearch(s, 0, null).Table;

        if (dt.Rows.Count > 0)
        {
            lNoExhibitors.Visible = false;
            rptExhibitors.DataSource = dt;
            rptExhibitors.DataBind();
        }

    }


    protected void rptExhibitors_DataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Item.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;

            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
                goto case ListItemType.Item;

            case ListItemType.Item:
                Image imgLogo = (Image)e.Item.FindControl("imgLogo");
                Label lblExhibitorName = (Label)e.Item.FindControl("lblExhibitorName");
                Label lblBooths = (Label)e.Item.FindControl("lblBooths");
                RadToolTip rppExhibitorBio = (RadToolTip)e.Item.FindControl("rppExhibitorBio");
                var hlExhibitorDescription = (HyperLink)e.Item.FindControl("hlExhibitorDescription");

                string imageID = Convert.ToString(drv["Logo"]);
                if ( ! string.IsNullOrWhiteSpace( imageID ) )
                {
                    imgLogo.Visible = true;
                    imgLogo.ImageUrl = GetImageUrl(imageID);
                }

                lblExhibitorName.Text = Convert.ToString(drv["Name"]);
                lblBooths.Text = Convert.ToString(drv["AssignedBooths"]);

                var bio = Convert.ToString(drv["Bio"]);

                if (!string.IsNullOrWhiteSpace(bio))
                {
                    rppExhibitorBio.Text = bio;
                    hlExhibitorDescription.Visible = true;
                }

                break;
        }
    }
}