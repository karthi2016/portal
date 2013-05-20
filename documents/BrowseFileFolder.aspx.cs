using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.WCF;

public partial class documents_BrowseFileFolder :  PortalPage 
{
    // this page was largely copied from the console/FolderBrowserView.aspx

    public msFileFolder targetFolder;
    protected FolderInfo foldersAndFiles;
    protected bool hasWriteAccess;

    const string DELIMITER = "∙";

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

        return DocumentsLogic.CanAccess(targetFolder.FileCabinet, targetFolder.ID , ConciergeAPI.CurrentEntity.ID);
    }

    protected override void InitializePage()
    {
        base.InitializePage();
        establishWhetherUserHasUploadAccess();

        bindFilesAndFolders();
        using( var api = GetServiceAPIProxy() )
        DocumentsLogic.SetupContextAndParentFolders(api, targetFolder, hlFolderContext, foldersAndFiles, rptParentFolders, false  );

        lblFolderName.Text = targetFolder.Name;

        setupLinks();
        setupContextButton();
      

    }

    private void setupContextButton()
    {
        MemberSuiteObject mso;
        using( var api = GetConciegeAPIProxy())
        mso = DocumentsLogic.GetFileCabinetContext( api, targetFolder);

        if (mso == null)
            btnBack.Text = "Back to My Digital Library";
        else 
            btnBack.Text  = "Back to " + mso["Name"];
        
    }

    private void setupLinks()
    {
        hlCreateSubFolder.Visible = hasWriteAccess;
        hlUploadFile.Visible = hasWriteAccess;
        hlChangeFolder.Visible = hasWriteAccess;
        lblNoWriteAccess.Visible = !hasWriteAccess;
        btnDeleteSelected.Visible = hasWriteAccess;
        spanCheckAll.Visible = hasWriteAccess;
        divFolderType.Visible = hasWriteAccess;

        hlCreateSubFolder.NavigateUrl = "CreateEditFolder.aspx?contextID=" + targetFolder.ID;
        hlUploadFile.NavigateUrl = "UploadFile.aspx?contextID=" + targetFolder.ID;
        hlChangeFolder.NavigateUrl = "CreateEditFolder.aspx?isEdit=true&contextID=" + targetFolder.ID;
        RegisterJavascriptConfirmationBox(btnDeleteSelected, "Are you sure want to delete the selected items? All files and subfolders within selected folders will be deleted and this CANNOT be undone.");

    }

    private void establishWhetherUserHasUploadAccess()
    {
        hasWriteAccess = DocumentsLogic.CanWriteTo(targetFolder, ConciergeAPI.CurrentEntity.ID);
    }

   

    private void bindFilesAndFolders()
    {
        
        using( var api = GetServiceAPIProxy() )
        foldersAndFiles = api.DescribeFolder( targetFolder.ID ).ResultValue;


        if (!string.IsNullOrWhiteSpace(foldersAndFiles.FolderDescription))
            lFolderDescription.Text = foldersAndFiles.FolderDescription;

        lFolderType.Text = foldersAndFiles.FolderType.ToString();
        if (foldersAndFiles.RestrictedAccess)
            lFolderType.Text += "/Read Only Access By Members";

        var noSubFolders = (foldersAndFiles.SubFolders == null || foldersAndFiles.SubFolders.Count == 0);
        bool noFiles = foldersAndFiles.Files == null || foldersAndFiles.Files.Count == 0;
        if (noSubFolders && noFiles) // empty
        {
            divEmptyFolder.Visible = true;
            phMainFolderTable.Visible = false;
        }
        else
        {
            divEmptyFolder.Visible = false;
            phMainFolderTable.Visible = true;

            // sort this first
            if (foldersAndFiles.SubFolders != null)
                foldersAndFiles.SubFolders.Sort((x, y) => string.Compare(x.FolderName, y.FolderName));
            rptFolders.DataSource = foldersAndFiles.SubFolders;
            rptFolders.DataBind();

            _sortFilesBasedOnPreferences(foldersAndFiles.Files);
            rptFiles.DataSource = foldersAndFiles.Files;
            rptFiles.DataBind();
        }
    }

    private void _sortFilesBasedOnPreferences(List<FileInfo> files)
    {
        if (files == null) return;
        files.Sort((x, y) => string.Compare(x.FileName, y.FileName));

    }

    protected void rptFolders_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        PartialFolderInfo fi = (PartialFolderInfo)e.Item.DataItem;

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
                Image imgIcon = (Image)e.Item.FindControl("imgIcon");
                Label lblItemDescription = (Label)e.Item.FindControl("lblItemDescription");
                HyperLink hlItemName = (HyperLink)e.Item.FindControl("hlItemName");
                CheckBox cbSelect = (CheckBox)e.Item.FindControl("cbSelect");

                HyperLink hlEdit = (HyperLink)e.Item.FindControl("hlEdit");
                HyperLink hlMove = (HyperLink)e.Item.FindControl("hlMove");
                HyperLink hlCopy = (HyperLink)e.Item.FindControl("hlCopy");
                LinkButton hlDelete = (LinkButton)e.Item.FindControl("hlDelete");

                hlEdit.Visible = hasWriteAccess;
                hlMove.Visible = hasWriteAccess;
                hlDelete.Visible = hasWriteAccess;
                cbSelect.Visible = hasWriteAccess;


                HiddenField hfID = (HiddenField)e.Item.FindControl("hfID");

                // we'll need this later, for bulk actions
                hfID.Value = fi.FolderID;

                // support the check/uncheck all script
                lSelectAllScript.Text += Environment.NewLine + string.Format("document.getElementById('{0}').checked = cb.checked;",
                    cbSelect.ClientID);



                if (fi.FolderType == FileFolderType.Public)
                    imgIcon.ImageUrl = "/images/documenticons/publicfoldericon.jpg";
                else
                    imgIcon.ImageUrl = "/images/documenticons/foldericon.jpg";

                hlItemName.Text = fi.FolderName;
                hlItemName.NavigateUrl = "BrowseFileFolder.aspx?contextID=" + fi.FolderID;

                if (fi.RestrictedAccess)
                    hlItemName.Text += " [restricted]";
                if (fi.FolderType == FileFolderType.Public)
                    hlItemName.Text += " [public]";

                lblItemDescription.Text = string.Format("Contains {1:N0} file(s) {2} {3} ",
                    fi.TotalNumberOfSubFolders, fi.TotalNumberOfFiles, DELIMITER, _computeFileSizeDisplay(fi.TotalFileSize));

                // process the actions
                hlEdit.NavigateUrl = "CreateEditFolder.aspx?isEdit=true&contextID="  + fi.FolderID;
                hlMove.NavigateUrl = "MoveFolder.aspx?contextID=" + fi.FolderID;
                
                hlDelete.CommandArgument = fi.FolderID;

                RegisterJavascriptConfirmationBox(hlDelete, "Are you sure want to delete this folder? All files and subfolders will be erased and this CANNOT be undone.");
                break;
        }
    }

    protected void rptFiles_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        FileInfo fi = (FileInfo) e.Item.DataItem;

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
                Image imgIcon = (Image) e.Item.FindControl("imgIcon");
                HyperLink hlItemName = (HyperLink) e.Item.FindControl("hlItemName");
                Label lblItemDescription = (Label) e.Item.FindControl("lblItemDescription");
                CheckBox cbSelect = (CheckBox) e.Item.FindControl("cbSelect");
                HiddenField hfID = (HiddenField) e.Item.FindControl("hfID");

                HyperLink hlEdit = (HyperLink) e.Item.FindControl("hlEdit");
                HyperLink hlMove = (HyperLink) e.Item.FindControl("hlMove");
                HyperLink hlCopy = (HyperLink) e.Item.FindControl("hlCopy");
                LinkButton hlDelete = (LinkButton) e.Item.FindControl("hlDelete");

                hlEdit.Visible = hasWriteAccess;
                hlMove.Visible = hasWriteAccess;
                hlDelete.Visible = hasWriteAccess;
                cbSelect.Visible = hasWriteAccess;

                // we'll need this later, for bulk actions
                hfID.Value = fi.FileID;

                // support the check/uncheck all script
                lSelectAllScript.Text += Environment.NewLine +
                                         string.Format("document.getElementById('{0}').checked = cb.checked;",
                                                       cbSelect.ClientID);


                hlEdit.NavigateUrl = "EditFile.aspx?contextID=" + fi.FileID;

                imgIcon.ImageUrl = _getImageIconForExtension(fi.FileExtension);

                hlItemName.Text = fi.FileName;
                hlItemName.NavigateUrl = "DownloadFile.aspx?contextID=" + fi.FileID;

                lblItemDescription.Text = string.Format("Last updated by {0} on {1:D} at {1:t} {2} {3}",
                                                        fi.LastModifiedByName == ConciergeAPI.CurrentUser.Name
                                                            ? "YOU"
                                                            : fi.LastModifiedByName, fi.LastModifiedDate, DELIMITER,
                                                        fi.FileSize);

                // process the actions
                hlEdit.NavigateUrl = "EditFile.aspx?contextID=" + fi.FileID;
                hlMove.NavigateUrl = "MoveFile.aspx?contextID=" + fi.FileID;
                hlDelete.CommandArgument = fi.FileID;

                RegisterJavascriptConfirmationBox(hlDelete, "Are you sure want to delete this file? This CANNOT be undone.");
                break;
        }
    }


    protected void rptFiles_OnItemCommand(object source, RepeaterCommandEventArgs e)
    {
        using (var api = GetServiceAPIProxy())
            api.Delete(e.CommandArgument.ToString());

        QueueBannerMessage( "The file has been successfully deleted.");
        Refresh();
    }

    protected void rptFolders_OnItemCommand(object source, RepeaterCommandEventArgs e)
    {
        var folderId = e.CommandArgument.ToString();    
        msFileFolder f = LoadObjectFromAPI<msFileFolder>(folderId );
        if (!DocumentsLogic.CanWriteTo(f, ConciergeAPI.CurrentEntity.ID))
            throw new ConciergeClientException( ConciergeErrorCode.AccessDenied, "You do not have access the delete the specified folder.");

        using (var api = GetServiceAPIProxy())
            api.DeleteFolderTree(folderId);
        

        QueueBannerMessage( "The folder (and all subfolders and files) have been successfully deleted.");
        Refresh();
    }


    

    private string _computeFileSizeDisplay(int totalFileSize)
    {
        decimal fs = (decimal)totalFileSize;
        if (totalFileSize < 1000)
            return totalFileSize.ToString("N2") + " bytes";

        if (totalFileSize < 1000000)
            return (fs / 1000M).ToString("N2") + " kb";

        if (totalFileSize < 1000000000)
            return (fs / 1000000M).ToString("N2") + " mb";

        return (fs / 1000000000M).ToString("N2") + " gb";
    }


    private string _getImageIconForExtension(string fileExtension)
    {
        if (fileExtension == null)
            fileExtension = "";

        switch (fileExtension.ToLower())
        {
            case "ppt":
            case "pptx":
                return "/images/documenticons/powerpoint.png";

            case "pdf":
                return "/images/documenticons/pdf.jpg";

            case "doc":
            case "docx":
                return "/images/documenticons/word.jpg";

            case "xls":
            case "xlsx":
                return "/images/documenticons/excel.jpg";

            case "jpg":
            case "jpeg":
                return "/images/documenticons/jpg.jpg";

            case "bmp":
                return "/images/documenticons/bmp.jpg";

            case "png":
                return "/images/documenticons/png.png";

            case "gif":
                return "/images/documenticons/gif.png";
            case "tif":
            case "tiff":
                return "/images/documenticons/tif.png";

            case "mp3":
                return "/images/documenticons/mp3.png";

            case "wav":
                return "/images/documenticons/wav.png";

            case "mp4":
            case "mpeg":
                return "/images/documenticons/mpeg.jpg";

            default:
                return "/images/documenticons/generic.jpg";

        }
    }

    protected void btnDeleteSelected_Click(object sender, EventArgs e)
    {
        List<string> itemsToDelete = new List<string>();
        using (var api = GetServiceAPIProxy())
        {
            foreach (RepeaterItem ri in rptFolders.Items)
            {
                HiddenField hfID = (HiddenField)ri.FindControl("hfID");
                CheckBox cbSelect = (CheckBox)ri.FindControl("cbSelect");

                if ( cbSelect != null &&cbSelect.Checked )
                api.DeleteFolderTree(hfID.Value);
            }

            // now the files
            foreach (RepeaterItem ri in rptFiles.Items)
            {
                HiddenField hfID = (HiddenField) ri.FindControl("hfID");
                CheckBox cbSelect = (CheckBox) ri.FindControl("cbSelect");
                if (cbSelect != null && cbSelect.Checked)
                    api.Delete(hfID.Value);
            }
        }
        if ( itemsToDelete.Count == 0 )
            QueueBannerError("No items have been selected.");
        else 
            QueueBannerMessage( "The item(s) have been successfully deleted.");
        Refresh();

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        MemberSuiteObject mso;
        using( var api = GetServiceAPIProxy())
        mso = DocumentsLogic.GetFileCabinetContext(api, targetFolder);
        
        if (mso == null)
            GoTo("/documents/DigitalLibrary.aspx");
        
        GoTo( NavigationLogic.GetUrlFor( mso ));
        
            
    }
}