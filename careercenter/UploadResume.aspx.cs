using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class careercenter_UploadResume : PortalPage
{
    #region Fields

    protected msResume targetResume;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the target object for the page
    /// </summary>
    /// <remarks>Many pages have "target" objects that the page operates on. For instance, when viewing
    /// an event, the target object is an event. When looking up a directory, that's the target
    /// object. This method is intended to be overriden to initialize the target object for
    /// each page that needs it.</remarks>
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        //If there's no resume create a new one and default IsApproved to true
        targetResume = new msResume {IsApproved = true};

        if (!string.IsNullOrWhiteSpace(ContextID))
        {
            targetResume = LoadObjectFromAPI<msResume>(ContextID);
            if (targetResume == null)
            {
                GoToMissingRecordPage();
                return;
            }
        }

    }

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();

        bindResume();

        divUploadResume.Visible = true;
        lbDifferentFile.Visible = false;
        hlView.Visible = false;

        if(!string.IsNullOrWhiteSpace(targetResume.File))
        {
            divUploadResume.Visible = false;
            lbDifferentFile.Visible = true;
            hlView.Visible = true;
        }

        litTitle.Text = string.Format("{0}", targetResume.Name);
    }

    #endregion

    #region Methods

    protected void bindResume()
    {
        if (targetResume == null)
            return;

        tbName.Text = targetResume.Name;
        cbIsActive.Checked = targetResume.IsActive;

        if (string.IsNullOrWhiteSpace(targetResume.File))
            return;

        string fileUrl = GetImageUrl( Convert.ToString( targetResume.File) );

        if (!Uri.IsWellFormedUriString(fileUrl, UriKind.Absolute))
            return;

        hlView.NavigateUrl = fileUrl;
    }

    protected void unbindResume()
    {
        //If there's no resume create a new one and default IsApproved to true
        if(targetResume == null)
            targetResume = new msResume{IsApproved = true};

        targetResume.Owner = ConciergeAPI.CurrentEntity.ID;
        targetResume.Name = tbName.Text;
        targetResume.IsActive = cbIsActive.Checked;

        if (fuUploadResume.HasFile)
        {
            targetResume.File = null;

            var file = new MemberSuiteFile
            {
                FileContents = fuUploadResume.FileBytes,
                FileName = fuUploadResume.FileName,
                FileType = fuUploadResume.FileName.EndsWith(".doc", StringComparison.CurrentCultureIgnoreCase) && fuUploadResume.PostedFile.ContentType == "application/octet-stream" ? "application/msword" : fuUploadResume.PostedFile.ContentType
            };

            targetResume["File_Contents"] = file;
        }
    }

    #endregion

    #region Event Handlers

    protected void fuUploadResume_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = false;

        if(fuUploadResume.PostedFile == null)
        {
            e.IsValid = !string.IsNullOrWhiteSpace(targetResume.File);
            return;
        }

        if (string.IsNullOrWhiteSpace(fuUploadResume.PostedFile.ContentType))
            return;

        if (fuUploadResume.PostedFile.ContentType.ToLower() == "application/pdf")
        {
            e.IsValid = true;
            return;
        }

        if(fuUploadResume.PostedFile.ContentType.ToLower() == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        {
            e.IsValid = true;
            return;
        }

        if (fuUploadResume.PostedFile.ContentType.ToLower() == "application/msword")
        {
            e.IsValid = true;
            return;
        }

        if (fuUploadResume.PostedFile.ContentType.ToLower() == "application/octet-stream" && fuUploadResume.FileName.EndsWith(".doc", StringComparison.CurrentCultureIgnoreCase))
        {
            e.IsValid = true;
            return;
        }
    }

    protected void lbDifferentFile_Click(object sender, EventArgs e)
    {
        lbDifferentFile.Visible = false;
        divUploadResume.Visible = true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        unbindResume();
        targetResume = SaveObject(targetResume).ConvertTo<msResume>();

        QueueBannerMessage(string.Format("Resume {0} has been saved successfully.",targetResume.LocalID));
        GoTo("~/careercenter/ManageResumes.aspx");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("~/careercenter/ManageResumes.aspx");
    }

    #endregion
}