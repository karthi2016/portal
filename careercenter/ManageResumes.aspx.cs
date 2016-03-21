using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class careercenter_ManageResumes : PortalPage
{
    #region Fields

    protected DataView dvResumes;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();


        //This has to happen in Page_Load not InitializePage so that it refreshes properly after a delete command
        loadDataFromConcierge();

        gvResumes.DataSource = dvResumes;
        gvResumes.DataBind();


        setUploadLink();
    }


    #endregion

    #region Methods

    protected void setUploadLink()
    {
        //If the user has less than 5 resumes on file give them the opportunity to upload a new one.  Otherwise inform them why they can't apply yet.
        WebControl wcCreateResume;

        if (dvResumes != null && dvResumes.Count >= 5)
        {
            wcCreateResume = new Label
            {
                ID = "lblCreateResume",
                Text = "Create New Resume (maximum 5 resumes)"
            };
        }
        else
        {
            wcCreateResume = new HyperLink()
            {
                ID = "hlCreateResume",
                Text = "Create New Resume",
                NavigateUrl = "~/careercenter/UploadResume.aspx"
            };
        }

        liCreateResume.Controls.Add(wcCreateResume);
    }

    protected void loadDataFromConcierge()
    {
        Search sResumes = new Search(msResume.CLASS_NAME);
        sResumes.AddOutputColumn("ID");
        sResumes.AddOutputColumn("LocalID");
        sResumes.AddOutputColumn("Name");
        sResumes.AddOutputColumn("LastModifiedDate");
        sResumes.AddOutputColumn("File");
        sResumes.AddOutputColumn("IsActive");
        sResumes.AddCriteria(Expr.Equals("Owner", ConciergeAPI.CurrentEntity.ID));

        SearchResult srResume = APIExtensions.GetSearchResult(sResumes, 0, null);
        dvResumes = new DataView(srResume.Table);
    }

    #endregion

    #region Event Handlers

    protected void gvResumes_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataRowView drResume = (DataRowView)e.Row.DataItem;

        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                HyperLink hlViewResume = (HyperLink) e.Row.Cells[3].FindControl("hlViewResume");
                hlViewResume.Visible = false;

                if (string.IsNullOrWhiteSpace(drResume["File"].ToString()))
                    return;

                string fileUrl = GetImageUrl( Convert.ToString( drResume["File"]) );

                if (!Uri.IsWellFormedUriString(fileUrl, UriKind.Absolute))
                    return;

                hlViewResume.NavigateUrl = fileUrl;
                hlViewResume.Visible = true;
                break;
        }
    }

    protected void gvResumes_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.ToLower() == "deleteresume")
        {
            using (IConciergeAPIService serviceProxy = GetServiceAPIProxy())
            {
                serviceProxy.Delete(e.CommandArgument.ToString());
            }

            QueueBannerMessage("Resume deleted successfully.");
        }

        GoTo("~/careercenter/ManageResumes.aspx");
    }

    #endregion
}