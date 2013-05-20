using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

public partial class documents_CreateEditFolder  :PortalPage 
{
    
    public msFileFolder targetFolder;
    protected FolderInfo foldersAndFiles;
    protected bool isEdit;
  
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetFolder = LoadObjectFromAPI<msFileFolder>(ContextID);
        if (targetFolder == null)
            GoToMissingRecordPage();

        isEdit = Request.QueryString["isEdit"] == "true";
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

        if (isEdit)
        {
            rblType.SelectedValue = targetFolder.Type.ToString();
            tbName.Text = targetFolder.Name;
            tbDescription.Text = targetFolder.Description;
            cbAdmin.Checked = targetFolder.OnlyLeadersCanUploadFiles;
        }

        cbAdmin.Visible = DocumentsLogic.HasAdministrativeAccessTo(targetFolder, ConciergeAPI.CurrentEntity.ID);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("BrowseFileFolder.aspx?contextID=" + targetFolder.ID);
    }

    protected void btnSaveChanges_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        msFileFolder folderToSave;
        if (isEdit)
            folderToSave = targetFolder;
        else
        {
            folderToSave = new msFileFolder();
            folderToSave.ParentFolder = targetFolder.ID;
            folderToSave.FileCabinet = targetFolder.FileCabinet;
        }

        folderToSave.Type = (FileFolderType)Enum.Parse(typeof(FileFolderType), rblType.SelectedValue);
        folderToSave.Name = tbName.Text;
        folderToSave.Description = tbDescription.Text;
        folderToSave.OnlyLeadersCanUploadFiles = cbAdmin.Checked;

        SaveObject(folderToSave);

        GoTo("BrowseFileFolder.aspx?contextID=" + targetFolder.ID, string.Format( "Folder '{0}' has been saved successfully.", folderToSave.Name ));
    }
}