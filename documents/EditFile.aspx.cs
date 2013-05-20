using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class documents_EditFile : PortalPage
{

    public msFile targetFile;
    public msFileFolder targetFolder;
    protected FolderInfo foldersAndFiles;

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetFile = LoadObjectFromAPI<msFile >(ContextID);
        if (targetFile == null)
            GoToMissingRecordPage();

        targetFolder = LoadObjectFromAPI<msFileFolder>(targetFile.FileFolder);
        if ( targetFolder == null ) // this isn't a folder based file
            Response.Redirect("/AccessDenied.aspx");
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
            DocumentsLogic.SetupContextAndParentFolders(api, targetFolder, hlFolderContext, foldersAndFiles,
                                                        rptParentFolders, true);
        }

        tbName.Text = targetFile.Name;
        tbDescription.Text = targetFile.Description;

        if (targetFolder.Type == FileFolderType.Public)
            lPublicLink.Text = GetImageUrl(  targetFile.ID );
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("BrowseFileFolder.aspx?contextID=" + targetFolder.ID);
    }

    protected void btnSaveChanges_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        targetFile.Name = tbName.Text;
        targetFile.Description = tbDescription.Text;

        SaveObject(targetFile);

        
        GoTo("BrowseFileFolder.aspx?contextID=" + targetFolder.ID, string.Format( "File '{0}' has been updated successfully.", targetFile.Name ));
            
    }

    
}