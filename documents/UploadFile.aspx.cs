using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Web.Controls;

public partial class documents_UploadFile : PortalPage 
{

    public msFileFolder targetFolder;
    protected FolderInfo foldersAndFiles;
  
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetFolder = LoadObjectFromAPI<msFileFolder>(ContextID);
        if (targetFolder == null)
            GoToMissingRecordPage();
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return DocumentsLogic.CanWriteTo(targetFolder, ConciergeAPI.CurrentEntity.ID);
    }

    protected override void InitializePage()
    {
        base.InitializePage();


        using (var api = GetServiceAPIProxy())
        {
            foldersAndFiles = api.DescribeFolder(targetFolder.ID).ResultValue;
            DocumentsLogic.SetupContextAndParentFolders(api, targetFolder,  hlFolderContext, foldersAndFiles,
                                                        rptParentFolders, true );
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("BrowseFileFolder.aspx?contextID=" + targetFolder.ID);
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        msFile fi = new msFile();
        fi.Description = tbDescription.Text;
        fi.FileCabinet = targetFolder.FileCabinet;
        fi.FileFolder = targetFolder.ID;

        switch (fuFile.State)
        {
            case FileUploadCoordinator.FileUploadState.NoFileSpecified:
                return;

            case FileUploadCoordinator.FileUploadState.NewFileSpecified:
                var f = new MemberSuiteFile();
                f.FileContents = fuFile.FileUpload.FileBytes;
                f.FileName = fuFile.FileUpload.FileName;
                f.FileType = fuFile.FileUpload.PostedFile.ContentType;
                fi["FileContents_Contents"] = f;
                break;
        }

        SaveObject(fi);

        GoTo("BrowseFileFolder.aspx?contextID=" + targetFolder.ID,string.Format( "File '{0}' has been updated successfully.", fi.Name ) );
    }
}